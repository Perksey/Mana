using System;
using System.Collections.Concurrent;

namespace Mana.Utilities
{
    /// <summary>
    /// A dispatcher type that allows <see cref="Action"/>s to be queued, then executed.
    /// </summary>
    public class Dispatcher
    {
        #region Game Dispatch Methods
        
        internal static readonly Dispatcher EarlyUpdateDispatcher = new Dispatcher();

        public static void OnEarlyUpdate(Action action)
        {
            EarlyUpdateDispatcher.Invoke(action);
        }
        
        #endregion
        
        private ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        /// <summary>
        /// Queues the given <see cref="Action"/> to be invoked when the <see cref="Dispatcher"/> processes its queue.
        /// </summary>
        /// <param name="action">The action to be invoked.</param>
        public void Invoke(Action action)
        {
            _actions.Enqueue(action);
        }

        /// <summary>
        /// Dequeues and invokes all <see cref="Action"/>s in the <see cref="Dispatcher"/>'s queue.
        /// </summary>
        public void InvokeActionsInQueue()
        {
            while (_actions.TryDequeue(out Action action))
                action.Invoke();
        }
    }
}