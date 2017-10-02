using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using Boerman.Core.EventArgs;

namespace Boerman.Core.Helpers
{
    // A simple Queue of objects.  Internally it is implemented as a circular
    // buffer, so Enqueue can be O(n).  Dequeue is O(1).
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public partial class DateTimeQueue<T> : ICollection, ICloneable
    {
        private Func<T, DateTime> _dateTimeGetter;
        private T[] _array;
        private int _head;       // First valid element in the queue
        private int _tail;       // Last valid element in the queue
        private int _size;       // Number of elements.
        private int _growFactor; // 100 == 1.0, 130 == 1.3, 200 == 2.0
        private int _version;
        [NonSerialized]
        private Object _syncRoot;

        private const int _MinimumGrow = 4;
        private const int _ShrinkThreshold = 32;

        public DateTimeQueue(Func<T, DateTime> dateTimeFunc) : this(32, (float)2.0)
        {
            _dateTimeGetter = dateTimeFunc;
        }

        //// Creates a queue with room for capacity objects. The default initial
        //// capacity and grow factor are used.
        //public DateTimeQueue()
        //    : this(32, (float)2.0)
        //{
        //}

        // Creates a queue with room for capacity objects. The default grow factor
        // is used.
        //
        private DateTimeQueue(int capacity)
            : this(capacity, (float)2.0)
        {
        }

        // Creates a queue with room for capacity objects. When full, the new
        // capacity is set to the old capacity * growFactor.
        //
        private DateTimeQueue(int capacity, float growFactor)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            if (!(growFactor >= 1.0 && growFactor <= 10.0))
                throw new ArgumentOutOfRangeException(nameof(growFactor));

            _array = new T[capacity];
            _head = 0;
            _tail = 0;
            _size = 0;
            _growFactor = (int)(growFactor * 100);
        }

        // Fills a Queue with the elements of an ICollection.  Uses the enumerator
        // to get each of the elements.
        //
        internal DateTimeQueue(ICollection col) : this(col?.Count ?? 32)
        {
            if (col == null)
                throw new ArgumentNullException(nameof(col));

            IEnumerator en = col.GetEnumerator();
            while (en.MoveNext())
                Enqueue((T)en.Current);
        }

        /// <summary>
        /// Occurs immediately before an item is enqueued. Provides peek access to the queue before enqueueuing.
        /// </summary>
        public event WillEnqueueEventHandler<T> WillEnqueue;

        /// <summary>
        /// Occurs immediately before an item is dequeued. Provides peek access to the queue before dequeuing.
        /// </summary>
        public event WillDequeueEventHandler<T> WillDequeue;

        /// <summary>
        /// Occurs immediately after an item is enqueued. Provides access to the item that was just enqueued.
        /// </summary>
        public event DidEnqueueEventHandler<T> DidEnqueue;

        /// <summary>
        /// Occurs immediately after an item is dequeued. Provides access to the item that was just dequeued.
        /// </summary>
        public event DidDequeueEventHandler<T> DidDequeue;

        public virtual int Count => _size;

        public virtual Object Clone()
        {
            DateTimeQueue<T> q = new DateTimeQueue<T>(_size)
            {
                _size = _size,
                _version = _version
            };

            int numToCopy = _size;
            int firstPart = _array.Length - _head < numToCopy ? _array.Length - _head : numToCopy;
            Array.Copy(_array, _head, q._array, 0, firstPart);
            numToCopy -= firstPart;
            if (numToCopy > 0)
                Array.Copy(_array, 0, q._array, _array.Length - _head, numToCopy);
            
            return q;
        }

        public virtual bool IsSynchronized => false;

        public virtual Object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    System.Threading.Interlocked.CompareExchange(ref _syncRoot, new Object(), null);

                return _syncRoot;
            }
        }

        // Removes all Objects from the queue.
        public virtual void Clear()
        {
            if (_head < _tail)
                Array.Clear(_array, _head, _size);
            else
            {
                Array.Clear(_array, _head, _array.Length - _head);
                Array.Clear(_array, 0, _tail);
            }

            _head = 0;
            _tail = 0;
            _size = 0;
            _version++;
        }

        // CopyTo copies a collection into an Array, starting at a particular
        // index into the array.
        // 
        public virtual void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1)
                throw new ArgumentException("Multidimensional arrays are not supported", nameof(array));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            int arrayLen = array.Length;
            if (arrayLen - index < _size)
                throw new ArgumentException("Array size from specified index too small");

            int numToCopy = _size;
            if (numToCopy == 0)
                return;
            int firstPart = (_array.Length - _head < numToCopy) ? _array.Length - _head : numToCopy;
            Array.Copy(_array, _head, array, index, firstPart);
            numToCopy -= firstPart;
            if (numToCopy > 0)
                Array.Copy(_array, 0, array, index + _array.Length - _head, numToCopy);
        }

        // Adds obj to the tail of the queue. And possibly reorders the queue
        //
        public virtual void Enqueue(T obj)
        {
            WillEnqueue?.Invoke(this, new WillEnqueueEventArgs<T>(_array[_tail], obj));

            if (_size == _array.Length)
            {
                int newcapacity = (int)((long)_array.Length * (long)_growFactor / 100);
                if (newcapacity < _array.Length + _MinimumGrow)
                {
                    newcapacity = _array.Length + _MinimumGrow;
                }
                SetCapacity(newcapacity);
            }

            DateTime? lastDateTime = null;
            DateTime? currentDateTime = null;

            if (_size > 0)
                lastDateTime = _dateTimeGetter.Invoke(_array[_tail - 1]);

            currentDateTime = _dateTimeGetter.Invoke(obj);

            // ToDo: Make this behaviour configurable
            if (_array.Any(q => q != null && _dateTimeGetter.Invoke(q) == currentDateTime.GetValueOrDefault()))
                return; // Silently ignore double datetimestamps.

            _array[_tail] = obj;

            if (currentDateTime < lastDateTime)
            {
                // Add to the list and reshuffle.
                List<T> list = new List<T>();
                foreach (var item in _array)
                    list.Add(item);

                _array = list.OrderBy(q => _dateTimeGetter.Invoke(q)).ToArray();
            }

            _tail = (_tail + 1) % _array.Length;
            _size++;
            _version++;

            DidEnqueue?.Invoke(this, new DidEnqueueEventArgs<T>(_array[_tail], _array[_tail - 1]));
        }

        // GetEnumerator returns an IEnumerator over this Queue.  This
        // Enumerator will support removing.
        // 
        public virtual IEnumerator GetEnumerator()
        {
            return new DateTimeQueueEnumerator(this);
        }

        // Removes the object at the head of the queue and returns it. If the queue
        // is empty, this method simply returns null.
        public virtual Object Dequeue()
        {
            if (Count == 0)
                throw new InvalidOperationException("Cannot dequeue when queue is empty");
            
            T removed = _array[_head];

            WillDequeue?.Invoke(this, new WillDequeueEventArgs<T>(removed));

            _array[_head] = default(T);
            _head = (_head + 1) % _array.Length;
            _size--;
            _version++;

            DidDequeue?.Invoke(this, new DidDequeueEventArgs<T>(removed, _array[_head]));

            return removed;
        }

        // Returns the object at the head of the queue. The object remains in the
        // queue. If the queue is empty, this method throws an 
        // InvalidOperationException.
        public virtual T Peek()
        {
            if (Count == 0)
                throw new InvalidOperationException("Cannot peek when queue is empty");
            
            return _array[_head];
        }

        // Returns a synchronized Queue.  Returns a synchronized wrapper
        // class around the queue - the caller must not use references to the
        // original queue.
        // 
        //[HostProtection(Synchronization = true)]
        public static DateTimeQueue<T> Synchronized(DateTimeQueue<T> queue)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            
            return new SynchronizedDateTimeQueue<T>(queue);
        }

        // Returns true if the queue contains at least one object equal to obj.
        // Equality is determined using obj.Equals().
        //
        // Exceptions: ArgumentNullException if obj == null.
        public virtual bool Contains(T obj)
        {
            int index = _head;
            int count = _size;

            while (count-- > 0)
            {
                if (obj == null)
                {
                    if (_array[index] == null)
                        return true;
                }
                else if (_array[index] != null && _array[index].Equals(obj))
                {
                    return true;
                }
                index = (index + 1) % _array.Length;
            }

            return false;
        }

        internal T GetElement(int i)
        {
            return _array[(_head + i) % _array.Length];
        }

        // Iterates over the objects in the queue, returning an array of the
        // objects in the Queue, or an empty array if the queue is empty.
        // The order of elements in the array is first in to last in, the same
        // order produced by successive calls to Dequeue.
        public virtual T[] ToArray()
        {
            var arr = new T[_size];
            if (_size == 0)
                return arr;

            if (_head < _tail)
            {
                Array.Copy(_array, _head, arr, 0, _size);
            }
            else
            {
                Array.Copy(_array, _head, arr, 0, _array.Length - _head);
                Array.Copy(_array, 0, arr, _array.Length - _head, _tail);
            }

            return arr;
        }


        // PRIVATE Grows or shrinks the buffer to hold capacity objects. Capacity
        // must be >= _size.
        private void SetCapacity(int capacity)
        {
            var newarray = new T[capacity];
            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, newarray, 0, _size);
                }
                else
                {
                    Array.Copy(_array, _head, newarray, 0, _array.Length - _head);
                    Array.Copy(_array, 0, newarray, _array.Length - _head, _tail);
                }
            }

            _array = newarray;
            _head = 0;
            _tail = _size == capacity ? 0 : _size;
            _version++;
        }

        public virtual void TrimToSize()
        {
            SetCapacity(_size);
        }

        /// <summary>
        /// WillEnqueue event handler.
        /// </summary>
        public delegate void WillEnqueueEventHandler<T>(object sender, WillEnqueueEventArgs<T> e);

        /// <summary>
        /// DidEnqueue event handler.
        /// </summary>
        public delegate void DidEnqueueEventHandler<T>(object sender, DidEnqueueEventArgs<T> e);

        /// <summary>
        /// WillDequeue event handler.
        /// </summary>
        public delegate void WillDequeueEventHandler<T>(object sender, WillDequeueEventArgs<T> e);

        /// <summary>
        /// DidDequeue event handler.
        /// </summary>
        public delegate void DidDequeueEventHandler<T>(object sender, DidDequeueEventArgs<T> e);
    }
}
