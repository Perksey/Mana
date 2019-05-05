using System;
using System.Collections.Concurrent;

namespace Mana.Utilities
{
    /// <summary>
    /// A dispatcher type that allows actions to be queued, then executed.
    /// </summary>
    public class Dispatcher
    {
        private ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        public void Invoke(Action action)
        {
            _actions.Enqueue(action);
        }

        public void InvokeActionsInQueue()
        {
            while (_actions.TryDequeue(out Action action))
                action.Invoke();
        }
    }
}