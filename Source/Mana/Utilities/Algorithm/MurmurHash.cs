using System.IO;

/*
 * Source: https://gist.github.com/automatonic/3725443
 */

namespace Mana.Utilities.Algorithm
{
    /// <summary>
    /// An implementation of the MurmurHash3 hashing algorithm using a buffered stream.
    /// </summary>
    public static class MurmurHash
    {
        /// <summary>
        /// Performs a MurmurHash3 on the given stream, using a buffered stream with the given buffer size in bytes.
        /// </summary>
        /// <param name="stream">The stream to hash.</param>
        /// <param name="bufferSize">The size, in bytes, of the BufferedStream that will be created.</param>
        /// <returns>The result of the MurmurHash3 algorithm.</returns>
        public static int Hash(Stream stream, int bufferSize = 2_000_000)
        {
            uint rotl32(uint x, byte r)
            {
                return (x << r) | (x >> (32 - r));
            }

            uint fmix(uint h)
            {
                h ^= h >> 16;
                h *= 0x85ebca6b;
                h ^= h >> 13;
                h *= 0xc2b2ae35;
                h ^= h >> 16;
                return h;
            }

            const uint SEED = 144;
            const uint C1 = 0xcc9e2d51;
            const uint C2 = 0x1b873593;

            uint h1 = SEED;
            uint streamLength = 0;

            using BufferedStream bufferedStream = new BufferedStream(stream, bufferSize);
            using BinaryReader reader = new BinaryReader(bufferedStream);

            var chunk = reader.ReadBytes(4);
            while (chunk.Length > 0)
            {
                streamLength += (uint)chunk.Length;
                uint k1 = 0;
                switch (chunk.Length)
                {
                    case 4:
                        k1 = (uint)(chunk[0]
                                    | chunk[1] << 8
                                    | chunk[2] << 16
                                    | chunk[3] << 24);

                        k1 *= C1;
                        k1 = rotl32(k1, 15);
                        k1 *= C2;

                        h1 ^= k1;
                        h1 = rotl32(h1, 13);
                        h1 = (h1 * 5) + 0xe6546b64;
                        break;
                    case 3:
                        k1 = (uint)(chunk[0]
                                    | chunk[1] << 8
                                    | chunk[2] << 16);
                        k1 *= C1;
                        k1 = rotl32(k1, 15);
                        k1 *= C2;
                        h1 ^= k1;
                        break;
                    case 2:
                        k1 = (uint)(chunk[0]
                                    | chunk[1] << 8);
                        k1 *= C1;
                        k1 = rotl32(k1, 15);
                        k1 *= C2;
                        h1 ^= k1;
                        break;
                    case 1:
                        k1 = chunk[0];
                        k1 *= C1;
                        k1 = rotl32(k1, 15);
                        k1 *= C2;
                        h1 ^= k1;
                        break;
                }

                chunk = reader.ReadBytes(4);
            }

            h1 ^= streamLength;
            h1 = fmix(h1);

            unchecked
            {
                return (int)h1;
            }
        }
    }
}
