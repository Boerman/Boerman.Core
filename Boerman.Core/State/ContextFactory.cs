using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;

namespace Boerman.Core.State
{
    /// <summary>
    /// The abstract <see cref="ContextFactory{T}"/> class helps creating and keeping track of multiple context instances.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="BaseContext"/> to use with this factory.</typeparam>
    public abstract class ContextFactory<T> where T : BaseContext
    {
        private readonly ConcurrentDictionary<Guid, T> _contextInstances = new ConcurrentDictionary<Guid, T>();
        
        /// <summary>
        /// The abstract constructor for the <see cref="ContextFactory{T}"/> class.
        /// </summary>
        /// <param name="timeout">The timeout after which to unreference a context if there is no activity.</param>
        protected ContextFactory(double timeout = 0)
        {
            if (timeout != 0)
            {
                // Please note that this code is just a quick fix to remove unneeded context instances over a while.
                _timer = new Timer(1000);   // We check for context instances when they were last active with this interval.
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

        /// <summary>
        /// Retrieve a specific context based on a given predicate.
        /// </summary>
        /// <param name="func">The predicate to evaluate</param>
        /// <returns>A single context, or null</returns>
        public T RetrieveContext(Func<T, bool> func)
        {
            return _contextInstances.Values.SingleOrDefault(func.Invoke);
        }

        /// <summary>
        /// Retrieve a context based on the identifier for said context
        /// </summary>
        /// <param name="guid">The identifier for the specific context.</param>
        /// <returns>Context or null</returns>
        public T RetrieveContext(Guid guid)
        {
            return _contextInstances[guid];
        }

        /// <summary>
        /// Add a context to be tracked by the factory.
        /// </summary>
        /// <param name="context">The context to track</param>
        /// <returns>The identifier by which this context is tracked</returns>
        public Guid AddContext(T context)
        {
            var guid = Guid.NewGuid();

            if (_contextInstances.TryAdd(guid, context)) return guid;

            throw new Exception("Could not add context instance");
        }

        /// <summary>
        /// Remove a context by its identifier
        /// </summary>
        /// <param name="guid">The identifier for the context to remove</param>
        public void RemoveContext(Guid guid)
        {
            _contextInstances.TryRemove(guid, out T context);
        }
    }
}
