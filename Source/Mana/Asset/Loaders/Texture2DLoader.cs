using System.IO;
using Mana.Graphics;

namespace Mana.Asset.Loaders
{
    public class Texture2DLoader : IAssetLoader<Texture2D>
    {
        public Texture2D Load(AssetManager manager, Stream sourceStream, string sourcePath)
        {
            return Texture2D.CreateFromStream(manager.GraphicsDevice, sourceStream);
        }
    }
}