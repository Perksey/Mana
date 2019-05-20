using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Mana.Graphics.Shaders;

namespace Mana.Asset.Loaders
{
    public class FragmentShaderLoader : IAssetLoader<FragmentShader>
    {
        [DebuggerStepThrough]
        public FragmentShader Load(AssetManager manager, Stream sourceStream, string sourcePath)
        {
            using (StreamReader streamReader = new StreamReader(sourceStream))
            {
                return new FragmentShader(manager.GraphicsDevice, streamReader.ReadToEnd());
            }
        }
    }
}