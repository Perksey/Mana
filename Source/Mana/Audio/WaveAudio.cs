using System;
using System.IO;
using OpenTK.Audio.OpenAL;

namespace Mana.Audio
{
    public class WaveAudio
    {
        public byte[] WaveData { get; }
        public WaveInfo WaveInfo { get; }
        
        /// <summary>
        /// Gets the duration, in seconds, of the audio.
        /// </summary>
        public float Duration => WaveData.Length / (float)WaveInfo.SampleRate;

        public WaveAudio(byte[] waveData, WaveInfo waveInfo)
        {
            WaveData = waveData;
            WaveInfo = waveInfo;
        }

        public static WaveAudio LoadFromStream(Stream stream)
        {
            using var binaryReader = new BinaryReader(stream);
            return LoadWave(binaryReader);
        }

        public static WaveAudio LoadWave(BinaryReader reader)
        {
            var waveInfo = new WaveInfo();
            
            var signature = new string(reader.ReadChars(4));
            if (signature != "RIFF")
            {
                throw new NotSupportedException("Specified stream is not a wave file.");
            }

            var riffChunkSize = reader.ReadInt32();

            var format = new string(reader.ReadChars(4));
            if (format != "WAVE")
            {
                throw new NotSupportedException("Specified stream is not a wave file.");
            }

            // WAVE header
            var formatSignature = new string(reader.ReadChars(4));
            if (formatSignature != "fmt ")
            {
                throw new NotSupportedException("Specified wave file is not supported.");
            }

            var formatChunkSize = reader.ReadInt32();
            var audioFormat = reader.ReadInt16();
            var numChannels = reader.ReadInt16();
            var sampleRate = reader.ReadInt32();
            var byteRate = reader.ReadInt32();
            var blockAlign = reader.ReadInt16();
            var bitsPerSample = reader.ReadInt16();

            var listsFound = 0;
            var maxLists = 100;

            while (true)
            {
                string dataSignature = new string(reader.ReadChars(4));

                if (dataSignature == "LIST")
                {
                    var listChunkSize = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                    var listStuff = reader.ReadBytes(listChunkSize);
                    listsFound++;

                    if (listsFound > maxLists)
                    {
                        throw new Exception("wav file contained more than " + maxLists + " LIST chunks.");
                    }
                }
                else if (dataSignature == "data")
                {
                    break;
                }
					
            }

            var dataChunkSize = reader.ReadInt32();

            waveInfo.Channels = numChannels;
            waveInfo.SampleRate = sampleRate;
            waveInfo.BitDepth = bitsPerSample;
            
            return new WaveAudio(reader.ReadBytes((int)reader.BaseStream.Length), waveInfo);
        }
        
        
    }

    public class WaveInfo
    {
        // public string ChunkID;
        // public int FileSize;
        // public int RiffType;
        // public int FormatID;
        // public int FormatSize;
        // public int FormatExtraSize;
        // public int FormatCode;
        public int Channels;
        public int SampleRate;
        // public int FormatAverageBps;
        // public int FormatBlockAlign;
        public int BitDepth;
        // public int DataID;
        // public int DataSize;
    }
    
    
}