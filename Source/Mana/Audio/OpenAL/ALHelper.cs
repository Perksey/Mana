using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Audio.OpenAL;

namespace Mana.Audio.OpenAL
{
    /// <summary>
    /// A static class of helper functions for working with OpenAL.
    /// </summary>
    public static class ALHelper
    {
        /// <summary>
        /// Checks to see if the last OpenAL API call resulted in an error,
        /// and throws an <see cref="Exception"/> if an error was found.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckLastError()
        {
            ALError error = AL.GetError();

            if (error != ALError.NoError)
                throw new Exception($"OpenAL error: {error}");
        }
        
        /// <summary>
        /// Gets an <see cref="ALFormat"/> value from the given channel count and bit depth.
        /// </summary>
        /// <param name="channels">The audio data's channel count.</param>
        /// <param name="bitDepth">The audio data's bit depth.</param>
        /// <returns>An <see cref="ALFormat"/> value from the given channel count and bit depth.</returns>
        public static ALFormat GetSoundFormat(int channels, int bitDepth)
        {
            switch (channels)
            {
                case 1:
                    switch (bitDepth)
                    {
                        case 8:
                            return ALFormat.Mono8;
                        case 16:
                            return ALFormat.Mono16;
                        default:
                            throw new NotSupportedException("The specified sound format is not supported.");
                    }
                case 2:
                    switch (bitDepth)
                    {
                        case 8:
                            return ALFormat.Stereo8;
                        case 16:
                            return ALFormat.Stereo16;
                        default:
                            throw new NotSupportedException();
                    }
                default:
                    throw new NotSupportedException("The specified sound format is not supported.");
            }
        }
    }
}