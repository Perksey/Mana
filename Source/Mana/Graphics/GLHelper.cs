using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public static class GLHelper
    {
        /// <summary>
        /// Checks to see if the last OpenGL API call resulted in an error,
        /// and throws a <see cref="GLException"/> if an error was found.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckLastError()
        {
            //ErrorCode errorCode = GL.GetError();

            //if (errorCode != ErrorCode.NoError)
            {
                //throw new GLException(errorCode);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(StringName name)
        {
            string result = GL.GetString(name);
            CheckLastError();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(StringNameIndexed name, uint i)
        {
            string result = GL.GetString(name, i);
            CheckLastError();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetInteger(GetPName name)
        {
            int result = GL.GetInteger(name);
            CheckLastError();
            return result;
        }
    }
}