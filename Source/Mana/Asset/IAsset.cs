using System;

namespace Mana.Asset
{
    public interface IAsset : IDisposable
    {
        /// <summary>
        /// The source path of the <see cref="IAsset"/> object.
        /// </summary>
        string SourcePath { get; set; }
        
        /// <summary>
        /// The parent <see cref="AssetManager"/> of the <see cref="IAsset"/> object.
        /// </summary>
        AssetManager AssetManager { get; set; }

        void OnAssetLoaded();
    }
}