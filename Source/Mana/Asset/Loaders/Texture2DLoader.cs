using System.IO;
using Mana.Graphics;
using Mana.Graphics.Textures;
using Mana.Utilities;

namespace Mana.Asset.Loaders
{
    public class Texture2DLoader : IAssetLoader<Texture2D>
    {
        public Texture2D Load(AssetManager manager, RenderContext renderContext, Stream sourceStream, string sourcePath)
        {
            var s = ManaTimer.StartNew();
            
            bool isCurr = manager.AsyncRenderContext.IsCurrent;

            s.Tally("isCurrent");
            
            var o = isCurr 
                    ? Texture2D.CreateFromStreamUnsynchronized(renderContext, sourceStream)
                    : Texture2D.CreateFromStream(renderContext, sourceStream);

            s.Tally("actual Load");

            return o;
        }
    }
}