namespace Mana.Asset
{
    public interface IAssetLoader
    {
    }

    public interface IAssetLoader<out T> : IAssetLoader
        where T : ManaAsset
    {
        T Load(AssetManager manager, AssetSource assetSource);
    }
}