namespace Boerman.Core.EventArgs
{
    /// <summary>
    /// WillEnqueue event arguments.
    /// </summary>
    public class WillEnqueueEventArgs<T> : System.EventArgs
    {
        /// <summary>
        /// Gets the peeked item.
        /// </summary>
        /// <value>The peeked item.</value>
        /// <remarks>May be a default value if the queue is empty. Make sure to check for a default value (i.e. null) before using.</remarks>
        public T CurrentHeadOfQueue { get; }

        /// <summary>
        /// Gets the item to be enqueued.
        /// </summary>
        /// <value>The item to be enqueued.</value>
        public T ItemToEnqueue { get; }

        /// <summary>
        /// Initializes a new instance of the WillEnqueueEventArgs class.
        /// </summary>
        /// <param name="currentHeadOfQueue">Current head of queue.</param>
        /// <param name="itemToEnqueue">Item to enqueue.</param>
        public WillEnqueueEventArgs(T currentHeadOfQueue, T itemToEnqueue)
        {
            ItemToEnqueue = itemToEnqueue;
            CurrentHeadOfQueue = currentHeadOfQueue;
        }
    }
}
