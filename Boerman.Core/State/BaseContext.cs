using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Boerman.Core.Reflection;

namespace Boerman.Core.State
{
    /// <summary>
    /// The abstract <see cref="BaseContext" /> class is used as the base for a state machine.
    /// 
    /// This class can be used to keep track of data which is needed by multiple states invoked by the context.
    /// 
    /// For the abstract state class, see <seealso cref="BaseState"/>.
    /// </summary>
    public abstract class BaseContext
    {
        /// <summary>
        /// Constructor for the abstract <see cref="BaseContext"/> class.
        /// </summary>
        protected BaseContext()
        {
            _lastActive = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Constructor for the abstract <see cref="BaseContext"/> class.
        /// </summary>
        /// <param name="customEntryPoint">The entry point of this context. Must be derived from <see cref="BaseState"/>.</param>
        protected BaseContext(Type customEntryPoint)
        {
            QueueState(customEntryPoint);
            _lastActive = DateTime.UtcNow;
        }
        
        private DateTime _lastActive;

        /// <summary>
        /// Property which keeps track of the last time a state has been executed in this context.
        /// </summary>
        public DateTime LastActive => IsQueueRunning ? DateTime.UtcNow : _lastActive;
        
        /// <summary>
        /// Property which keeps track of the states enqueued for execution.
        /// </summary>
        private readonly ConcurrentQueue<Type> _stateQueue = new ConcurrentQueue<Type>();

        protected bool StateQueueContainsStates => _stateQueue.Any();
        
        private ManualResetEvent _waitForIdleProcess = new ManualResetEvent(true);

        /// <summary>
        /// This field provides a synchronisation mechanism to the context to wait until all queued states have been resolved.
        /// 
        /// Usage of this field in production is discouraged. See documentation for the recommended way to interact with the state machine.
        /// </summary>
        public Func<bool> WaitForIdleProcess => _waitForIdleProcess.WaitOne;

        private CancellationToken _cancellationToken;

        private bool _isQueueRunning;

        /// <summary>
        /// Field indicating whether the context is working on resolving the queued states.
        /// 
        /// In order to wait for completion, see the <seealso cref="WaitForIdleProcess"/> field.
        /// </summary>
        public bool IsQueueRunning
        {
            get {
                return _isQueueRunning;
            }
            protected set
            {
                lock (_queueRunningLock)
                {
                    _lastActive = DateTime.UtcNow;
                    _isQueueRunning = value;
                }
            }
        }

        private readonly object _queueRunningLock = new object();

        private BaseState _currentState;
        
        /// <summary>
        /// Queue a state for execution. Queued states will be executed in order of insertion.
        /// </summary>
        /// <param name="state">The type of the state. The type should be derived from <see cref="BaseState"/>.</param>
        /// <returns></returns>
        public BaseContext QueueState(Type state)
        {
            if (!state.IsSubclassOf(typeof(BaseState))) throw new ArgumentException(nameof(state));
            _stateQueue.Enqueue(state);

            _lastActive = DateTime.UtcNow;

            return this;
        }

        /// <summary>
        /// Signal the context to start processing states.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> which can be used to stop the context in processing it's enqueued states.</param>
        public void Run(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsQueueRunning) return;
            IsQueueRunning = true;

            _waitForIdleProcess.Reset();

            _cancellationToken = cancellationToken;
            
            ThreadPool.QueueUserWorkItem(async n =>
            {
                while (StateQueueContainsStates && !_cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        _stateQueue.TryDequeue(out Type enqueuedState);
                        _currentState = enqueuedState.CreateInstance<BaseState>(this);

                        await _currentState.Run();
                    }
                    catch (Exception)
                    {
                        IsQueueRunning = false;
                        _cancellationToken = default(CancellationToken);
                        _waitForIdleProcess.Set();
                        return;
                    }
                }

                IsQueueRunning = false;
                _cancellationToken = default(CancellationToken);
                _waitForIdleProcess.Set();
            }, null);
        }
    }
}
