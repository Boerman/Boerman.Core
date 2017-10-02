// Implements an enumerator for a DateTimeQueue<T>.  The enumerator uses the
// internal version number of the list to ensure that no modifications are
// made to the list while an enumeration is in progress.

using System;
using System.Collections;

namespace Boerman.Core.Helpers
{
    public partial class DateTimeQueue<T>
    {
        [Serializable]
        internal class DateTimeQueueEnumerator : IEnumerator, ICloneable
        {
            private DateTimeQueue<T> _q;
            private int _index;
            private int _version;
            private Object currentElement;

            internal DateTimeQueueEnumerator(DateTimeQueue<T> q)
            {
                _q = q;
                _version = _q._version;
                _index = 0;
                currentElement = _q._array;
                if (_q._size == 0)
                    _index = -1;
            }

            public Object Clone()
            {
                return MemberwiseClone();
            }

            public virtual bool MoveNext()
            {
                if (_version != _q._version)
                    throw new InvalidOperationException(
                       "Enumeration failed");

                if (_index < 0)
                {
                    currentElement = _q._array;
                    return false;
                }

                currentElement = _q.GetElement(_index);
                _index++;

                if (_index == _q._size)
                    _index = -1;
                return true;
            }

            public virtual Object Current
            {
                get
                {
                    if (currentElement == _q._array)
                    {
                        if (_index == 0)
                            throw new InvalidOperationException(
                                "Enumeration not started");
                        else
                            throw new InvalidOperationException(
                                "Enumeration ended");
                    }
                    return currentElement;
                }
            }

            public virtual void Reset()
            {
                if (_version != _q._version)
                    throw new InvalidOperationException(
                        "Enumeration failed");
                if (_q._size == 0)
                    _index = -1;
                else
                    _index = 0;
                currentElement = _q._array;
            }
        }
    }
}
