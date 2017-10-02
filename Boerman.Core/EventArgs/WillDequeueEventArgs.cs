namespace Boerman.Core.EventArgs
{
    /// <summary>
    /// WillDequeue event arguments.
    /// </summary>
    public class WillDequeueEventArgs<T> : System.EventArgs
    {
        /// <summary>
        /// Gets the peeked item.
        /// </summary>
        /// <value>The peeked item.</value>
        /// /// <remarks>May be a default value if the queue is empty. Make sure to check for a default value (i.e. null) before using.</remarks>
        public T ItemToBeDequeued { get; }

        /// <summary>
        /// Initializes a new instance of the WillDequeueEventArgs class.
        /// </summary>
        /// <param name="itemToBeDequeued">Item to be dequeued.</param>
        public WillDequeueEventArgs(T itemToBeDequeued)
        {
            ItemToBeDequeued = itemToBeDequeued;
        }
    }
}
