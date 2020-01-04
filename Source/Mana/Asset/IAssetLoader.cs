using System.IO;
using Mana.Graphics;

namespace Mana.Asset
{
    public interface IAssetLoader
    {
    }

    public interface IAssetLoader<out T> : IAssetLoader
        where T : IAsset
    {
        T Load(AssetManager assetManager,
               RenderContext renderContext,
               Stream stream,
               string sourcePath);
    }
}
