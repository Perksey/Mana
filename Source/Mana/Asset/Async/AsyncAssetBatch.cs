using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Mana.Utilities;

namespace Mana.Asset.Async
{
    public class AsyncAssetBatch
    {
        private static object _contextLock = new object();
        
        public AssetManager AssetManager { get; }

        private List<IAsyncAssetItem> _pendingAssets = new List<IAsyncAssetItem>();
        private bool _closed = false;
        private List<Action> _onCompletedActions = new List<Action>();
        
        public AsyncAssetBatch(AssetManager assetManager)
        {
            AssetManager = assetManager;
        }

        public AsyncAssetBatch Load<T>(Expression<Func<T>> getter, string path)
            where T : ManaAsset
        {
            if (_closed)
                throw new InvalidOperationException("AsyncAssetBatch is closed.");
            
            return Load(() => Ref.Of(getter).Setter, path);
        }
        
        public AsyncAssetBatch Load<T>(Action<T> setter, string path)
            where T : ManaAsset
        {
            return Load(() => setter, path);
        }
        
        private AsyncAssetBatch Load<T>(Func<Action<T>> setterFactory, string path)
            where T : ManaAsset
        {
            if (_closed)
                throw new InvalidOperationException("AsyncAssetBatch is closed.");
            
            _pendingAssets.Add(new AsyncAssetItem<T>(setterFactory, path));
            
            return this;
        }

        public AsyncAssetBatch OnCompleted(Action callback)
        {
            if (_closed)
                throw new InvalidOperationException("AsyncAssetBatch is closed.");
            
            _onCompletedActions.Add(callback);

            return this;
        }

        public AsyncAssetTask Run()
        {
            if (_closed)
                throw new InvalidOperationException("AsyncAssetBatch already closed");

            _closed = true;

            var task = new AsyncAssetTask(_pendingAssets.Count);

            new Thread(() => 
            {
                lock (_contextLock)
                {
                    AssetManager.AsyncContext.MakeCurrent(
                        AssetManager.GraphicsDevice.Window.GameWindow.WindowInfo
                    );

                    for (int i = 0; i < _pendingAssets.Count; i++)
                    {
                        _pendingAssets[i].Load(AssetManager);
                        task.SetProgress(i);
                    }
                
                    Dispatcher.EarlyUpdateDispatcher.Invoke(() =>
                    {
                        foreach (var pendingAsset3 in _pendingAssets)
                        {
                            pendingAsset3.Complete();
                        }
                    
                        task.IsCompleted = true;

                        foreach (var action1 in _onCompletedActions)
                        {
                            action1.Invoke();
                        }
                    });
                }
            }).Start();

            return task;
        }
    }
}