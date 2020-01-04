using System;

namespace Mana.Graphics.Shaders
{
    /// <summary>
    /// The exception that is thrown when OpenGL encounters an error during shader compilation.
    /// </summary>
    public class ShaderCompileException : Exception
    {
        public ShaderCompileException(string message)
            : base(message.TrimEnd('\n'))
        {
        }
    }
}
