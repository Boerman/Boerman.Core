using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Boerman.Core.Helpers
{
    public partial class DateTimeQueue<T> : ICollection, ICloneable
    {
        // Implements a synchronization wrapper around a queue.
        [Serializable]
        private class SynchronizedDateTimeQueue<T> : DateTimeQueue<T>
        {
            private DateTimeQueue<T> _q;
            private Object root;

            internal SynchronizedDateTimeQueue(DateTimeQueue<T> q) : base(q)
            {
                this._q = q;
                root = _q.SyncRoot;
            }

            public override bool IsSynchronized => true;
            public override Object SyncRoot => root;

            public override int Count
            {
                get
                {
                    lock (root)
                    {
                        return _q.Count;
                    }
                }
            }

            public override void Clear()
            {
                lock (root)
                {
                    _q.Clear();
                }
            }

            public override Object Clone()
            {
                lock (root)
                {
                    return new SynchronizedDateTimeQueue<T>((DateTimeQueue<T>)_q.Clone());
                }
            }

            public override bool Contains(T obj)
            {
                lock (root)
                {
                    return _q.Contains(obj);
                }
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                lock (root)
                {
                    _q.CopyTo(array, arrayIndex);
                }
            }

            public override void Enqueue(T value)
            {
                lock (root)
                {
                    _q.Enqueue(value);
                }
            }

            [SuppressMessage("Microsoft.Contracts", "CC1055")]  // Thread safety problems with precondition - can't express the precondition as of Dev10.
            public override Object Dequeue()
            {
                lock (root)
                {
                    return _q.Dequeue();
                }
            }

            public override IEnumerator GetEnumerator()
            {
                lock (root)
                {
                    return _q.GetEnumerator();
                }
            }

            [SuppressMessage("Microsoft.Contracts", "CC1055")]  // Thread safety problems with precondition - can't express the precondition as of Dev10.
            public override T Peek()
            {
                lock (root)
                {
                    return _q.Peek();
                }
            }

            public override T[] ToArray()
            {
                lock (root)
                {
                    return _q.ToArray();
                }
            }

            public override void TrimToSize()
            {
                lock (root)
                {
                    _q.TrimToSize();
                }
            }
        }
    }
}
