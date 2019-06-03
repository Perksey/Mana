using System;

namespace Mana.Graphics.Shader
{
    /// <summary>
    /// The exception that is thrown when OpenGL encounters an error during shader program linking.
    /// </summary>
    public class ShaderProgramLinkException : Exception
    {
        public ShaderProgramLinkException(string shaderInfoLog)
            : base(shaderInfoLog.TrimEnd('\n'))
        {
        }
    }
}