using Mana.Utilities;
using OpenTK.Audio.OpenAL;

namespace Mana.Audio.OpenAL
{
    public class ALSound : Sound
    {
        private static Logger _log = Logger.Create();
        
        public int BufferHandle { get; private set; }
        public WaveAudio WaveAudio { get; private set; }
        
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
            var instance = new ALSoundInstance(this);
            instance.Play();
            return instance;
        }
    }
}