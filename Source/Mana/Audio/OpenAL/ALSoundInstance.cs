using System;
using Mana.Utilities;
using OpenTK.Audio.OpenAL;

namespace Mana.Audio.OpenAL
{
    public class ALSoundInstance : SoundInstance
    {
        private static Logger _log = Logger.Create();
        
        private bool _looping = false;
        private float _volume = 1.0f;
        private float _pitch = 1.0f;
        
        private int _source;

        internal ALSoundInstance(ALSound sound) 
            : base(sound)
        {
            _source = AL.GenSource();
            ALHelper.CheckLastError();

            
            
            AL.Source(_source, ALSourcei.Buffer, sound.BufferHandle);
            ALHelper.CheckLastError();
        }

        ~ALSoundInstance()
        {
            _log.Debug("~ALSoundInstance: Sound Instance");
        }

        public override bool Looping
        {
            get => _looping;
            set
            {
                if (_looping != value)
                {
                    if (_source == -1)
                        throw new InvalidOperationException();
                    
                    AL.Source(_source, ALSourceb.Looping, value);
                    ALHelper.CheckLastError();
                    
                    _looping = value;
                }
            }
        }

        public override float Volume
        {
            get => _volume;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_volume != value)
                {
                    _volume = value;
                    
                    if (_source == -1)
                        throw new InvalidOperationException();

                    AL.Source(_source, ALSourcef.Gain, value);
                    ALHelper.CheckLastError();
                }
            }
        }

        public override float Pitch
        {
            get => _pitch;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_pitch != value)
                {
                    _pitch = value;
                    
                    if (_source == -1)
                        throw new InvalidOperationException();

                    AL.Source(_source, ALSourcef.Pitch, value);
                    ALHelper.CheckLastError();
                }
            } 
        }

        public override void Play()
        {
            if (State == SoundState.NotStarted || State == SoundState.Paused)
            {
                if (_source == -1)
                    throw new InvalidOperationException();
                
                AL.SourcePlay(_source);
                ALHelper.CheckLastError();

                State = SoundState.Playing;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override void Stop()
        {
            if (State == SoundState.Playing)
            {
                if (_source == -1)
                    throw new InvalidOperationException();
                
                State = SoundState.Stopped;

                AL.SourceStop(_source);
                ALHelper.CheckLastError();
                
                Delete();
            }
        }

        public override void Pause()
        {
            if (State == SoundState.Playing)
            {
                if (_source == -1)
                    throw new InvalidOperationException();
                
                State = SoundState.Paused;

                AL.SourcePause(_source);
                ALHelper.CheckLastError();
            }
        }

        protected override void OnSoundFinished()
        {
            State = SoundState.Finished;
            Delete();
        }

        private void Delete()
        {
            if (_source == -1)
                throw new InvalidOperationException();
            
            AL.DeleteSource(_source);
            ALHelper.CheckLastError();

            _source = -1;

            OpenALBackend.Instance.RemoveSoundInstance(this);
        }
    }
}