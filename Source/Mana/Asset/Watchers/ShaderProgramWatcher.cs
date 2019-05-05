using System;
using Mana.Graphics.Shaders;

namespace Mana.Asset.Watchers
{
    internal class ShaderProgramWatcher : AssetWatcher
    {
        private AssetManager _assetManager;
        private ShaderProgram _shaderProgram;
        
        public ShaderProgramWatcher(AssetManager assetManager, ShaderProgram shaderProgram)
        {
            _assetManager = assetManager;
            _shaderProgram = shaderProgram;
            
            WatchPaths(shaderProgram.SourcePath, 
                       shaderProgram.VertexShaderPath,
                       shaderProgram.FragmentShaderPath);

            Console.WriteLine("Created shader program watcher.");
        }

        protected override void Reload()
        {
            _shaderProgram.Reload(_assetManager, null);
        }
    }
}