using System;
using System.Collections.Generic;
using System.IO;

namespace Mana.Audio
{
    /// <summary>
    /// Represents an audio backend platform (such as OpenAL or FMOD).
    /// </summary>
    public abstract class AudioBackend : IDisposable
    {
        /// <summary>
        /// Gets the current <see cref="AudioBackend"/> object.
        /// </summary>
        public static AudioBackend Backend { get; private set; }
        
        /// <summary>
        /// A list of all currently active <see cref="SoundInstance"/> objects.
        /// </summary>
        public readonly List<SoundInstance> SoundInstances = new List<SoundInstance>();
        
        /// <summary>
        /// Sets the current <see cref="AudioBackend"/> object.
        /// </summary>
        /// <param name="backend"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void SetBackend(AudioBackend backend)
        {
            if (Backend != null)
                throw new InvalidOperationException("AudioBackend already set.");
            
            Backend = backend;
        }

        /// <summary>
        /// Updates the backend's active <see cref="SoundInstance"/> objects.
        /// </summary>
        /// <param name="time">The current time.</param>
        /// <param name="deltaTime">The time delta from the previous frame to the current frame.</param>
        public virtual void Update(float time, float deltaTime)
        {
            foreach (SoundInstance soundInstance in SoundInstances)
            {
                soundInstance.Update(time, deltaTime);
            }
        }

        /// <summary>
        /// Creates a <see cref="Sound"/> from a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>The newly created <see cref="Sound"/> object.</returns>
        public abstract Sound CreateSoundFromStream(Stream stream);

        /// <inheritdoc/>
        public abstract void Dispose();

        /// <summary>
        /// Removes a SoundInstance from the AudioBackend.
        /// </summary>
        /// <param name="soundInstance">The <see cref="SoundInstance"/> to remove.</param>
        internal void RemoveSoundInstance(SoundInstance soundInstance)
        {
            SoundInstances.Remove(soundInstance);
        }
    }
}