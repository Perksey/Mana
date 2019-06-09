using Mana.Asset;

namespace Mana.Audio
{
    /// <summary>
    /// Represents a sound asset.
    /// </summary>
    public abstract class Sound : IAsset
    {
        /// <summary>
        /// Gets the duration, in seconds, of the sound.
        /// </summary>
        public float Duration { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class.
        /// </summary>
        /// <param name="duration">The duration, in seconds, of the sound asset.</param>
        protected Sound(float duration)
        {
            Duration = duration;
        }

        /// <inheritdoc/>
        public string SourcePath { get; set; }

        /// <inheritdoc/>
        public AssetManager AssetManager { get; set; }

        /// <inheritdoc/>
        public abstract void Dispose();

        public abstract SoundInstance Play();

        public void OnAssetLoaded()
        {
        }
    }
}