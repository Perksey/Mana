using System;
using System.Collections.Generic;

namespace Mana.Asset.Async
{
    public class AsyncAssetTask
    {
        private object _mutex = new object();
        private bool _isCompleted = false;
        private int _count;
        private int _progress;
        
        internal AsyncAssetTask(int count)
        {
            _count = count;
        }

        public void SetProgress(int progress)
        {
            _progress = progress;
        }
        
        public bool IsCompleted
        {
            get
            {
                lock (_mutex)
                {
                    return _isCompleted;
                }
            }

            internal set
            {
                lock (_mutex)
                {
                    _isCompleted = value;
                }
            }
        }
    }
}