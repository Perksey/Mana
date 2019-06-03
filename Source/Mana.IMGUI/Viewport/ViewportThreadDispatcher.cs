using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Mana.IMGUI.Viewport
{
    internal class ViewportThreadDispatcher
    {
        private ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
        private int _managedThreadID = -1;
        private bool _closed = false;
        
        public ViewportThreadDispatcher()
        {
            _managedThreadID = Thread.CurrentThread.ManagedThreadId;
        }

        public void Invoke(Action action)
        {
            _actions.Enqueue(action);
        }
        
        public void InvokeAndWait(Action action)
        {
            if (Thread.CurrentThread.ManagedThreadId == _managedThreadID)
            {
                action.Invoke();
            }
            else
            {
                var task = new Task(action);
                _actions.Enqueue(() => { task.RunSynchronously(); });
                task.Wait();    
            }
        }

        public void InvokeActionsInQueue()
        {
            if (_closed)
            {
                return;
            }
            
            while (_actions.TryDequeue(out Action action))
            {
                action.Invoke();

                if (_closed)
                    break;
            }
        }

        public void SetManagedThreadID(int id)
        {
            _managedThreadID = id;
        }

        public void Close()
        {
            _closed = true;
        }
    }
}