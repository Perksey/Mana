namespace Mana.Asset.Reloading
{
    public interface IReloadableAsset : IAsset
    {
        void Reload(AssetManager assetManager);
        string[] GetLiveReloadAssetPaths();
    }
}
