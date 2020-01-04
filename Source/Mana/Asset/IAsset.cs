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

        /// <summary>
        /// Called when the <see cref="IAsset"/> has been loaded by an <see cref="AssetManager"/>.
        /// </summary>
        void OnAssetLoaded();
    }
}
