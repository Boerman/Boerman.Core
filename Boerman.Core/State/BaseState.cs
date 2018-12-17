using System.Threading.Tasks;

namespace Boerman.Core.State
{
    /// <summary>
    /// The abstract <see cref="BaseState"/> class which is used to implement custom states. These states can be used with the <see cref="BaseContext"/> class to create a state machine.
    /// </summary>
    public abstract class BaseState
    {
        public BaseContext Context { get; protected set; }

        protected BaseState(BaseContext context)
        {
            Context = context;
        }

        public abstract Task Run();
    }

    /// <summary>
    /// The abstract <see cref="BaseState{T}"/> class which is used to implement custom states. This state implementation enforces a certain context implementation.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="BaseContext"/> implementation.</typeparam>
    public abstract class BaseState<T> : BaseState where T : BaseContext
    {
        public new T Context => base.Context as T;

        protected BaseState(T context) : base(context)
        {
            base.Context = context;
        }

        public abstract override Task Run();
    }
}
