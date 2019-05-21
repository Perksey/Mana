using System;

namespace Mana.Asset
{
    public abstract class ManaAsset : IDisposable
    {
        internal string SourcePath { get; set; }
        internal AssetManager AssetManager { get; set; }
        internal bool IsUnloading { get; set; } = false; 

        public abstract void Dispose();

        internal virtual void OnAssetLoaded(AssetManager assetManager)
        {
        }
    }
}