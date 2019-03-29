using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    /// <summary>
    /// The exception that is thrown when OpenGL encounters an error during shader compilation.
    /// </summary>
    public class ShaderCompileException : Exception
    {
        public ShaderCompileException(string shaderInfoLog)
            : base($"Error compiling shader:\n{shaderInfoLog}")
        {
        }
    }
}
