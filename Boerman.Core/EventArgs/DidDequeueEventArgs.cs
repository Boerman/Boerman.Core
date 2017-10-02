namespace Boerman.Core.EventArgs
{
    /// <summary>
    /// DidDequeue event arguments.
    /// </summary>
    public class DidDequeueEventArgs<T> : System.EventArgs
    {
        /// <summary>
        /// Gets the dequeued item.
        /// </summary>
        /// <value>The dequeued item.</value>
        public T DequeuedItem { get; }

        /// <summary>
        /// Gets the next (peeked) item. This item is not yet dequeued.
        /// </summary>
        /// <value>The next item.</value>
        /// /// <remarks>May be a default value if the queue is emoty after dequeuing. Make sure to check for a default value (i.e. null) before using.</remarks>
        public T NextItem { get; }

        /// <summary>
        /// Initializes a new instance of the DidDequeueEventArgs class.
        /// </summary>
        /// <param name="dequeuedItem">Dequeued item.</param>
        /// <param name="nextItem">Next item.</param>
        public DidDequeueEventArgs(T dequeuedItem, T nextItem)
        {
            DequeuedItem = dequeuedItem;
            NextItem = nextItem;
        }
    }
}
