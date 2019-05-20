using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Mana.Logging;

namespace Mana.Utilities
{
    /// <summary>
    /// A dispatcher type that allows <see cref="Action"/>s to be queued, then executed.
    /// </summary>
    public class Dispatcher
    {
        private static Logger _log = Logger.Create();
        
        #region Game Dispatch Methods
        
        internal static readonly Dispatcher EarlyUpdateDispatcher = new Dispatcher();

        public static void OnEarlyUpdate(Action action)
        {
            EarlyUpdateDispatcher.Invoke(action);
        }
        
        #endregion
        
        private ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        public static void RunOnMainThread(Action action)
        {
            if (ThreadHelper.IsMainThread)
            {
                action.Invoke();
            }
            else
            {
                EarlyUpdateDispatcher.Invoke(action);
            }
        }
        
        public static void RunOnMainThreadAndWait(Action action)
        {
            if (ThreadHelper.IsMainThread)
            {
                action.Invoke();
            }
            else
            {
                EarlyUpdateDispatcher.InvokeAndWait(action);
            }
        }

        /// <summary>
        /// Queues the given <see cref="Action"/> to be invoked when the <see cref="Dispatcher"/> processes its queue.
        /// </summary>
        /// <param name="action">The action to be invoked.</param>
        public void Invoke(Action action)
        {
            _actions.Enqueue(action);
        }
        
        /// <summary>
        /// Queues the given <see cref="Action"/> to be invoked when the <see cref="Dispatcher"/> processes its queue.
        /// This method will block the calling thread until the action has completed on the Dispatcher's thread.
        /// </summary>
        /// <param name="action">The action to be invoked.</param>
        public void InvokeAndWait(Action action)
        {
            var task = new Task(action);
            
            _actions.Enqueue(() => { task.RunSynchronously(); });

            task.Wait();
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