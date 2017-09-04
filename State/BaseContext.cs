using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Boerman.Core.Extensions;

namespace Boerman.Core.State
{
    public abstract class BaseContext
    {
        protected BaseContext(double timeout = 0)
        {
            _lastActive = DateTime.UtcNow;
        }
        
        protected BaseContext(Type customEntryPoint, double timeout = 0)
        {
            QueueState(customEntryPoint);
            _lastActive = DateTime.UtcNow;
        }
        
        private DateTime _lastActive;

        public DateTime LastActive => IsQueueRunning ? DateTime.UtcNow : _lastActive;
        
        private readonly ConcurrentQueue<Type> _stateQueue = new ConcurrentQueue<Type>();

        protected bool StateQueueContainsStates => _stateQueue.Any();

        // I'm not quite sure whether I should add this manual reset event here and whether to have it public.
        public ManualResetEvent WaitForIdleProcess = new ManualResetEvent(true);

        private CancellationToken _cancellationToken;

        private bool _isQueueRunning;
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
        
        public BaseContext QueueState(Type state)
        {
            //Logger.Trace($"{state.Name} queued");
            
            if (!state.IsSubclassOf(typeof(BaseState))) throw new ArgumentException(nameof(state));
            _stateQueue.Enqueue(state);

            _lastActive = DateTime.UtcNow;

            return this;
        }

        public void Run(CancellationToken ct = default(CancellationToken))
        {
            if (IsQueueRunning) return;
            IsQueueRunning = true;

            WaitForIdleProcess.Reset();

            _cancellationToken = ct;
            
            ThreadPool.QueueUserWorkItem(async n =>
            {
                while (StateQueueContainsStates && !_cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        _stateQueue.TryDequeue(out Type enqueuedState);
                        _currentState = enqueuedState.CreateInstance<BaseState>(this);

                        //Logger.Trace($"Starting execution for {_currentState.GetType().Name}");
                        await _currentState.Run();
                        //Logger.Trace($"Execution finished for {_currentState.GetType().Name}");
                    }
                    catch (Exception ex)
                    {
                        //Logger.Error(ex, $"Context has ended due to exception in state {_currentState.GetType().Name}");
                        IsQueueRunning = false;
                        return;
                    }
                }

                IsQueueRunning = false;

                //Logger.Trace(StateQueueContainsStates
                //    ? "Context has ended due to cancellation"
                //    : "Context has ended because no next state has been defined");
                
                // Basically invalidate the cancellationtoken
                _cancellationToken = default(CancellationToken);

                WaitForIdleProcess.Set();
            }, null);
        }
    }
}
