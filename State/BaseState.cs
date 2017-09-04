using System.Threading.Tasks;

namespace Boerman.Core.State
{
    public abstract class BaseState
    {
        public BaseContext Context;

        protected BaseState(BaseContext context)
        {
            Context = context;
        }

        public abstract Task Run();
    }

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
