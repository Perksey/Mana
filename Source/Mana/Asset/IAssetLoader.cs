using System.IO;

namespace Mana.Asset
{
    public interface IAssetLoader
    {
    }

    public interface IAssetLoader<T> : IAssetLoader
        where T : ManaAsset
    {
        T Load(AssetManager manager, Stream sourceStream, string sourcePath);
    }
}