namespace Mana.Asset
{
    public abstract class ReloadableAsset 
        : ManaAsset
    {
        protected AssetWatcher Watcher { get; private set; }
        
        public abstract override void Dispose();
        public abstract bool Reload(AssetManager assetManager);

        internal override void OnAssetLoaded(AssetManager assetManager)
        {
            base.OnAssetLoaded(assetManager);
            
            if (assetManager.ReloadOnUpdate)
                Watcher = new AssetWatcher(assetManager, this);
        }
    }
}