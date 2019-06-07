using System;
using System.Collections.Generic;
using System.IO;

namespace Mana.Audio
{
    public abstract class AudioBackend : IDisposable
    {
        public static AudioBackend Backend { get; private set; }
        
        public List<SoundInstance> SoundInstances = new List<SoundInstance>();
        
        public static void SetBackend(AudioBackend backend)
        {
            if (Backend != null)
                throw new InvalidOperationException("AudioBackend already set.");
            
            Backend = backend;
        }

        public virtual void Update(float time, float deltaTime)
        {
            foreach (SoundInstance soundInstance in SoundInstances)
            {
                soundInstance.Update(time, deltaTime);
            }
        }

        public abstract Sound CreateAudioClipFromStream(Stream stream);

        public void RemoveSoundInstance(SoundInstance soundInstance)
        {
            SoundInstances.Remove(soundInstance);
        }

        public abstract void Dispose();
    }
}