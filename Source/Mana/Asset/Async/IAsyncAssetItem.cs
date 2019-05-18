namespace Mana.Asset.Async
{
    public interface IAsyncAssetItem
    {
        void Load(AssetManager assetManager);
        void Complete();
    }
}