using System.Diagnostics;
using System.IO;
using Mana.Graphics.Shaders;

namespace Mana.Asset.Loaders
{
    public class FragmentShaderLoader : IAssetLoader<FragmentShader>
    {
        [DebuggerStepThrough]
        public FragmentShader Load(AssetManager manager, AssetSource assetSource)
        {
            using (StreamReader streamReader = new StreamReader(assetSource.Stream))
            {
                return new FragmentShader(manager.GraphicsDevice, streamReader.ReadToEnd());
            }
        }
    }
}