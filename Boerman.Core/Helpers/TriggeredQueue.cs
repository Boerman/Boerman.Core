// Thanks to https://gist.github.com/jsauve/b2e8496172fdabd370c4

using System;
using System.Collections.Generic;
using Boerman.Core.EventArgs;

namespace Boerman.Core.Helpers
{
    /// <summary>
    /// Triggered queue. Provides events immediately before and after enqueueuing and dequeuing.
    /// </summary>
    public class TriggeredQueue<T>
    {
        /// <summary>
        /// The internal queue.
        /// </summary>
        readonly Queue<T> queue = new Queue<T>();

        /// <summary>
        /// Occurs immediately before an item is enqueued. Provides peek access to the queue before enqueueuing.
        /// </summary>
        public event WillEnqueueEventHandler<T> WillEnqueue;

        /// <summary>
        /// Occurs immediately before an item is dequeued. Provides peek access to the queue before dequeuing.
        /// </summary>
        public event WillDequeueEventHandler<T> WillDequeue;

        /// <summary>
        /// Occurs immediately after an item is enqueued. Provides access to the item that was just enqueued.
        /// </summary>
        public event DidEnqueueEventHandler<T> DidEnqueue;

        /// <summary>
        /// Occurs immediately after an item is dequeued. Provides access to the item that was just dequeued.
        /// </summary>
        public event DidDequeueEventHandler<T> DidDequeue;

        /// <summary>
        /// Raises the WillEnqueue event.
        /// </summary>
        /// <param name="currentHeadOfQueue">Current head of queue.</param>
        /// <param name="itemToEnqueue">Item to enqueue.</param>
        protected virtual void OnWillEnqueue(T currentHeadOfQueue, T itemToEnqueue)
        {
            WillEnqueue?.Invoke(this, new WillEnqueueEventArgs<T>(currentHeadOfQueue, itemToEnqueue));
        }

        /// <summary>
        /// Raises the DidEnqueue event.
        /// </summary>
        /// <param name="enqueuedItem">Enqueued item.</param>
        /// <param name="previousHeadOfQueue">Previous head of queue.</param>
        protected virtual void OnDidEnqueue(T enqueuedItem, T previousHeadOfQueue)
        {
            DidEnqueue?.Invoke(this, new DidEnqueueEventArgs<T>(enqueuedItem, previousHeadOfQueue));
        }

        /// <summary>
        /// Raises the WillDequeue event.
        /// </summary>
        /// <param name="itemToBeDequeued">Item to be dequeued.</param>
        protected virtual void OnWillDequeue(T itemToBeDequeued)
        {
            if (WillDequeue != null)
                WillDequeue(this, new WillDequeueEventArgs<T>(itemToBeDequeued));
        }

        /// <summary>
        /// Raises the did dequeue event.
        /// </summary>
        /// <param name="item">Item.</param>
        protected virtual void OnDidDequeue(T dequeuedItem, T nextItem)
        {
            DidDequeue?.Invoke(this, new DidDequeueEventArgs<T>(dequeuedItem, nextItem));
        }

        /// <summary>
        /// Enqueue the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public virtual void Enqueue(T item)
        {
            T peekItem;

            try { peekItem = queue.Peek(); }
            catch (InvalidOperationException) { peekItem = (default(T)); }

            OnWillEnqueue(peekItem, item);

            queue.Enqueue(item);

            OnDidEnqueue(item, peekItem);
        }

        /// <summary>
        /// Dequeue this instance.
        /// </summary>
        public virtual T Dequeue()
        {
            T peekItem;

            try { peekItem = queue.Peek(); }
            catch (InvalidOperationException) { peekItem = (default(T)); }

            OnWillDequeue(peekItem);

            T dequeuedItem = queue.Dequeue();

            try { peekItem = queue.Peek(); }
            catch (InvalidOperationException) { peekItem = (default(T)); }

            OnDidDequeue(dequeuedItem, peekItem);

            return dequeuedItem;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => queue.Count;

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty => queue.Count < 1;
    }

    /// <summary>
    /// WillEnqueue event handler.
    /// </summary>
    public delegate void WillEnqueueEventHandler<T>(object sender, WillEnqueueEventArgs<T> e);

    /// <summary>
    /// DidEnqueue event handler.
    /// </summary>
    public delegate void DidEnqueueEventHandler<T>(object sender, DidEnqueueEventArgs<T> e);

    /// <summary>
    /// WillDequeue event handler.
    /// </summary>
    public delegate void WillDequeueEventHandler<T>(object sender, WillDequeueEventArgs<T> e);

    /// <summary>
    /// DidDequeue event handler.
    /// </summary>
    public delegate void DidDequeueEventHandler<T>(object sender, DidDequeueEventArgs<T> e);
}
