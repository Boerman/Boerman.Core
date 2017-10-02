namespace Boerman.Core.EventArgs
{
    /// <summary>
    /// DidEnqueue event arguments.
    /// </summary>
    public class DidEnqueueEventArgs<T> : System.EventArgs
    {
        /// <summary>
        /// Gets the enqueued item.
        /// </summary>
        /// <value>The enqueued item.</value>
        public T EnqueuedItem { get; }

        /// <summary>
        /// Gets the previous head of queue.
        /// </summary>
        /// <value>The previous head of queue.</value>
        /// /// <remarks>May be a default value if the queue is empty. Make sure to check for a default value (i.e. null) before using.</remarks>
        public T PreviousHeadOfQueue { get; }

        /// <summary>
        /// Initializes a new instance of the DidEnqueueEventArgs class.
        /// </summary>
        /// <param name="enqueuedItem">Enqueued item.</param>
        /// <param name="previousHeadOfQueue">Previous head of queue.</param>
        public DidEnqueueEventArgs(T enqueuedItem, T previousHeadOfQueue)
        {
            EnqueuedItem = enqueuedItem;
            PreviousHeadOfQueue = previousHeadOfQueue;

        }
    }
}
