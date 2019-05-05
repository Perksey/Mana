using System.Diagnostics;
using System.IO;
using Mana.Graphics.Shaders;

namespace Mana.Asset.Loaders
{
    public class VertexShaderLoader : IAssetLoader<VertexShader>
    {
        [DebuggerStepThrough]
        public VertexShader Load(AssetManager manager, Stream sourceStream, string sourcePath)
        {
            using (StreamReader streamReader = new StreamReader(sourceStream))
            {
                return new VertexShader(manager.GraphicsDevice, streamReader.ReadToEnd());
            }
        }
    }
}