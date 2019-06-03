using System;

namespace Mana.Graphics.Shader
{
    /// <summary>
    /// The exception that is thrown when OpenGL encounters an error during shader compilation.
    /// </summary>
    public class ShaderCompileException : Exception
    {
        public ShaderCompileException(string shaderInfoLog)
            : base(shaderInfoLog.TrimEnd('\n'))
        {
        }
    }
}