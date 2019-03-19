using System.Diagnostics;
using System.IO;
using Mana.Graphics.Shaders;

namespace Mana.Asset.Loaders
{
    public class VertexShaderLoader : IAssetLoader<VertexShader>
    {
        [DebuggerStepThrough]
        public VertexShader Load(AssetManager manager, AssetSource assetSource)
        {
            using (StreamReader streamReader = new StreamReader(assetSource.Stream))
            {
                return new VertexShader(manager.GraphicsDevice, streamReader.ReadToEnd());
            }
        }
    }
}