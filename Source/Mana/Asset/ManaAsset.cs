using System;

namespace Mana.Asset
{
    public abstract class ManaAsset : IDisposable
    {
        internal string SourcePath { get; set; }

        public abstract void Dispose();

        internal virtual void OnAssetLoaded(AssetManager assetManager)
        {
        }
    }
}