using System;

namespace Mana.Asset.Async
{
    public class AsyncAssetItem<T> : IAsyncAssetItem
        where T : ManaAsset
    {
        private Func<Action<T>> _setterFactory;
        private Action<T> _setter;
        private string _path;
        private T _asset;
            
        public AsyncAssetItem(Func<Action<T>> setterFactory, string path)
        {
            _setterFactory = setterFactory;
            _path = path;
        }

        public void Load(AssetManager assetManager)
        {
            _asset = assetManager.Load<T>(_path);
            _setter = _setterFactory.Invoke();
        }

        public void Complete()
        {
            _setter.Invoke(_asset);
        }
    }
}