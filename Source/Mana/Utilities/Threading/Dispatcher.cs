using System;
using System.Collections.Concurrent;

namespace Mana.Utilities.Threading
{
    /// <summary>
    /// A dispatcher type that allows actions to be queued
    /// </summary>
    public class Dispatcher
    {
        private static Logger _log = Logger.Create();

        private ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        /// <summary>
        /// Queues the given action to be invoked on the main thread.
        /// </summary>
        /// <param name="action">The action to be invoked on the main thread.</param>
        public void Invoke(Action action)
        {
            _actions.Enqueue(action);
        }

        /// <summary>
        /// Dequeues and invokes all actions in the action queue.
        /// </summary>
        public void ProcessActionQueue()
        {
            while (_actions.TryDequeue(out Action action))
            {
                action.Invoke();
            }
        }
    }
}
