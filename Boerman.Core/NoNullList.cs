using System.Collections.Generic;

namespace Boerman.Core
{
    public class NoNullList<T> : List<T>
    {
        public new void Add(T item)
        {
            if (item == null) return;

            base.Add(item);
        }
    }
}
