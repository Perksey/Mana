using System.IO;
using OpenTK.Audio;

namespace Mana.Audio.OpenAL
{
    public class OpenALBackend : AudioBackend
    {
        public static AudioBackend Instance { get; } = new OpenALBackend();

        private AudioContext _context;

        public OpenALBackend()
        {
            _context = new AudioContext();
        }
        
        public static void Initialize()
        {
            AudioBackend.SetBackend(OpenALBackend.Instance);
        }

        public override void Update(float time, float deltaTime)
        {
            base.Update(time, deltaTime);
        }

        public override Sound CreateSoundFromStream(Stream stream)
        {
            return new ALSound(WaveAudio.LoadFromStream(stream));
        }

        public override void Dispose()
        {
            _context?.Dispose();
        }
    }
}