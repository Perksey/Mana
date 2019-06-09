using System;
using Mana.Utilities;
using OpenTK.Audio.OpenAL;

namespace Mana.Audio.OpenAL
{
    /// <summary>
    /// Represents a <see cref="Sound"/> object for an OpenAL <see cref="AudioBackend"/>. 
    /// </summary>
    public class ALSound : Sound
    {
        private bool _disposed;
        
        /// <summary>
        /// The OpenAL buffer handle.
        /// </summary>
        public int BufferHandle { get; }
        
        /// <summary>
        /// The wave audio for the sound.
        /// </summary>
        public WaveAudio WaveAudio { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ALSound"/> class.
        /// </summary>
        /// <param name="waveAudio">The wave audio for the sound.</param>
        public ALSound(WaveAudio waveAudio)
            : base(waveAudio.Duration)
        {
            WaveAudio = waveAudio;
            
            BufferHandle = AL.GenBuffer();
            ALHelper.CheckLastError();
            
            AL.BufferData(BufferHandle,
                          ALHelper.GetSoundFormat(WaveAudio.WaveInfo.Channels, WaveAudio.WaveInfo.BitDepth),
                          WaveAudio.WaveData,
                          WaveAudio.WaveData.Length,
                          WaveAudio.WaveInfo.SampleRate);
            ALHelper.CheckLastError();
        }

        public override SoundInstance Play()
        {
            if (_disposed)
                throw new InvalidOperationException("Cannot play a disposed sound.");
            
            var instance = new ALSoundInstance(this);
            
            AudioBackend.Backend.SoundInstances.Add(instance);
            
            instance.Play();
            
            return instance;
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                AL.DeleteBuffer(BufferHandle);
                ALHelper.CheckLastError();

                _disposed = true;
            }
        }
    }
}