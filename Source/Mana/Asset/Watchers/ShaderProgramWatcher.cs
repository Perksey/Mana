using System;
using Mana.Graphics.Shaders;
using Mana.Logging;

namespace Mana.Asset.Watchers
{
    internal class ShaderProgramWatcher : AssetWatcher
    {
        private static Logger _log = Logger.Create();
        
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
            if (_shaderProgram.Reload(_assetManager, null))
            {
                _log.LogMessage($"Reloaded Shader: {_shaderProgram.SourcePath}", LogLevel.Debug, ConsoleColor.Green);
            }
        }
    }
}