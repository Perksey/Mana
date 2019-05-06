using System;
using Mana.Graphics;
using Mana.Logging;

namespace Mana.Asset.Watchers
{
    public class Texture2DWatcher : AssetWatcher
    {
        private static Logger _log = Logger.Create();
        
        private AssetManager _assetManager;
        private Texture2D _texture2D;

        public Texture2DWatcher(AssetManager assetManager, Texture2D texture2D)
        {
            _assetManager = assetManager;
            _texture2D = texture2D;
            
            WatchPath(texture2D.SourcePath);
        }
        
        protected override void Reload()
        {
            if (_texture2D.Reload(_assetManager, null))
            {
                _log.LogMessage($"Reloaded Texture2D: {_texture2D.SourcePath}", LogLevel.Debug, ConsoleColor.Green);
            }
        }
    }
}