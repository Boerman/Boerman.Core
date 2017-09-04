using Boerman.Core.Extensions;

namespace Boerman.Core.State
{
    public class ContextPersistenceModel
    {
        public string QueuedStates { get; set; }

        public bool IsContextRunning { get; set; }

        internal string[] QueuedStateArray {
            get { return QueuedStates.Split(','); }
            set { QueuedStates = value.Join(","); }
        }
    }
}
