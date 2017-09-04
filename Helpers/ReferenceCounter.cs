using System.Threading;

namespace Boerman.Core.Helpers
{
    public class ReferenceCounter
    {
        private int _counter;

        public int Value => _counter;

        public ReferenceCounter(int initial)
        {
            _counter = initial;
        }

        public void Increment()
        {
            Interlocked.Increment(ref _counter);
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref _counter);
        }

        public void Reset()
        {
            Reset(0);
        }

        public void Reset(int value)
        {
            Interlocked.Exchange(ref _counter, value);
        }
    }
}
