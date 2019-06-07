using Mana.Asset;

namespace Mana.Audio
{
    public abstract class Sound : IAsset
    {
        /// <summary>
        /// Gets the duration, in seconds, of the sound.
        /// </summary>
        public float Duration { get; }
        
        protected Sound(float duration)
        {
            Duration = duration;
        }

        public string SourcePath { get; set; }

        public AssetManager AssetManager { get; set; }

        public void Dispose()
        {
        }

        public void OnAssetLoaded()
        {
        }

        public abstract SoundInstance Play();
    }
}