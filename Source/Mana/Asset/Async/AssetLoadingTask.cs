using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using Mana.Utilities;

namespace Mana.Asset.Async
{
    /// <summary>
    /// A class that represents an asynchronous asset loading task.
    /// </summary>
    public class AssetLoadingTask
    {
        private List<IPendingAsyncAsset> _queue = new List<IPendingAsyncAsset>();
        private object _contextLock = new object();
        private int _amountCompleted = 0;
        private bool _closed = false;
        private LinkedList<Action> _onCompletedCallbacks = new LinkedList<Action>();
        
        public AssetLoadingTask(AssetManager assetManager)
        {
            AssetManager = assetManager;
        }
        
        /// <summary>
        /// Gets the <see cref="AssetManager"/> that the <see cref="AssetLoadingTask"/> belongs to.
        /// </summary>
        public AssetManager AssetManager { get; }

        /// <summary>
        /// Gets a value that indicates whether the asynchronous loading job has been completed.
        /// </summary>
        public bool Completed { get; private set; } = false;

        /// <summary>
        /// Gets a <see cref="float"/> value within the range [0, 1] representing how complete the task is.
        /// </summary>
        public float Progress => _amountCompleted / (float)_queue.Count;

        /// <summary>
        /// Gets a value that indicates how many elements of the batch loading task have been completed.
        /// </summary>
        public int ProgressCount => _amountCompleted;

        /// <summary>
        /// Gets a value that indicates the total number of elements that the task will load.
        /// </summary>
        public int Count => _queue.Count;

        /// <summary>
        /// Adds an item to the batch asynchronous loading job, loading the asset at the given path. When the batch
        /// loading job completes, the field referenced in the <see cref="getter"/> parameter will be set to the loaded
        /// asset value. This operation will occur on the main thread, so no locking is necessary.
        /// </summary>
        /// <param name="getter">The get expression that will be used to retrieve a setter and set the loaded asset.</param>
        /// <param name="path">The path to the asset.</param>
        /// <returns>itself, for method chaining purposes.</returns>
        public AssetLoadingTask Load<T>(Expression<Func<T>> getter, string path)
            where T : ManaAsset
        {
            if (_closed)
                throw new InvalidOperationException("Cannot call Load() after Begin() has been called.");
            
            var asset = new PendingAsyncAsset<T>(getter, path);
            _queue.Add(asset);
            return this;
        }

        /// <summary>
        /// Closes the <see cref="AssetLoadingTask"/> object so new items cannot be added, and starts the asynchronous
        /// loading task.
        /// </summary>
        /// <returns>itself, for method chaining purposes.</returns>
        public AssetLoadingTask Start()
        {
            if (_closed)
                throw new InvalidOperationException("Begin may only be called once, and has already been called.");

            new Thread(() =>
            {
                lock (_contextLock)
                {
                    AssetManager.AsyncContext.MakeCurrent(AssetManager.GraphicsDevice.Window.GameWindow.WindowInfo);
                    
                    foreach (var pending in _queue)
                    {
                        pending.Load(AssetManager);
                        Interlocked.Increment(ref _amountCompleted);
                    }
                    
                    AssetManager.AsyncContext.MakeCurrent(null);
                    
                    Dispatcher.RunOnMainThread(() =>
                    {
                        foreach (var pending in _queue)
                        {
                            pending.Set();
                        }

                        Completed = true;

                        foreach (var callback in _onCompletedCallbacks)
                        {
                            callback.Invoke();
                        }
                    });
                }
            }).Start();
            
            return this;
        }

        /// <summary>
        /// Queues an <see cref="Action"/> object to be invoked on the main thread once all assets have been loaded.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> object to be invoked.</param>
        /// <returns>itself, for method chaining purposes.</returns>
        public AssetLoadingTask OnCompleted(Action action)
        {
            _onCompletedCallbacks.AddLast(action);
            return this;
        }
        
        interface IPendingAsyncAsset
        {
            void Load(AssetManager assetManager);
            void Set();
        }
    
        class PendingAsyncAsset<T> : IPendingAsyncAsset
            where T : ManaAsset
        {
            private string _path;
            private Expression<Func<T>> _getter;
            private Action<T> _setter;
            private T _asset;
            
            public PendingAsyncAsset(Expression<Func<T>> getter, string path)
            {
                _getter = getter;
                _path = path;
            }

            public void Load(AssetManager assetManager)
            {
                _setter = Ref.Of(_getter).Setter;
                _asset = assetManager.Load<T>(_path);
            }

            public void Set()
            {
                _setter(_asset);
            }
        }
    }
}