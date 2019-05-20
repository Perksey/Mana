using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public static class GLHelper
    {
        /// <summary>
        /// Checks to see if the last OpenGL API call resulted in an error,
        /// and throws an <see cref="Exception"/> if an error was found.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckLastError()
        {
            ErrorCode errorCode = GL.GetError();

            if (errorCode != ErrorCode.NoError)
            {
                throw new Exception(errorCode.ToString());
            }
        }
    }
}