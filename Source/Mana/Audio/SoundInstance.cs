namespace Mana.Audio
{
    public abstract class SoundInstance
    {
        public Sound Sound { get; }
        
        public SoundState State { get; protected set; }
        
        public abstract bool Looping { get; set; }
        public abstract float Volume { get; set; }
        public abstract float Pitch { get; set; }
        
        private float _currentTime = 0f;
        
        protected SoundInstance(Sound sound)
        {
            Sound = sound;
            State = SoundState.NotStarted;
        }

        /// <summary>
        /// Plays or resumes the sound instance.
        /// </summary>
        public abstract void Play();
        public abstract void Stop();
        public abstract void Pause();
        
        public void Update(float time, float deltaTime)
        {
            if (State == SoundState.Playing)
            {
                _currentTime += deltaTime;
                
                if (_currentTime >= Sound.Duration)
                {
                    State = SoundState.Finished;
                    OnSoundFinished();
                }
            }
        }

        protected abstract void OnSoundFinished();
    }
}