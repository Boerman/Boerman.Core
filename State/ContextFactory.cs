using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;

namespace Boerman.Core.State
{
    public abstract class ContextFactory<T> where T : BaseContext
    {
        private readonly ConcurrentDictionary<Guid, T> _contextInstances = new ConcurrentDictionary<Guid, T>();
        
        protected ContextFactory(double timeout = 0)
        {
            if (timeout != 0)
            {
                // Please note that this code is just a quick fix to remove unneeded context instances over a while.
                _timer = new Timer(1000);
                if (timeout > 0) timeout = -timeout;

                _timer.Elapsed += (sender, args) =>
                {
                    var keys = _contextInstances.Where(q => q.Value.LastActive < DateTime.UtcNow.AddMilliseconds(timeout)).Select(q => q.Key);
                    foreach (var key in keys) RemoveContext(key);
                };

                _timer.Start();
            }
        }

        private readonly Timer _timer;
        
        /// <summary>
        /// Checks if there is a context already which evaluates to the given predicate
        /// </summary>
        /// <param name="func">The predicate to compare context instances to. Please note a context must be uniquely identifiable.</param>
        /// <returns></returns>
        public bool CheckIfContextExists(Func<BaseContext, bool> func)
        {
            return RetrieveContext(func) != null;
        }

        public T RetrieveContext(Func<T, bool> func)
        {
            var instances = _contextInstances.Values.Where(func.Invoke).ToList();

            if (instances.Count > 1)
                throw new ArgumentException("Predicate returns more then one context instance", nameof(func));

            return instances.FirstOrDefault();
        }

        public T RetrieveContext(Guid guid)
        {
            return _contextInstances[guid];
        }

        public Guid AddContext(T context)
        {
            var guid = Guid.NewGuid();

            if (_contextInstances.TryAdd(guid, context)) return guid;

            throw new Exception("Could not add context instance");
        }

        public void RemoveContext(Guid guid)
        {
            _contextInstances.TryRemove(guid, out T context);
        }
    }
}
