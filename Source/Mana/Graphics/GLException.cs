using System;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    /// <summary>
    /// The exception that is thrown when an OpenGL API call resulted in an error.
    /// </summary>
    public class GLException : Exception
    {
        public GLException(string message)
            : base(message)
        {
        }
    }
}