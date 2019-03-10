using System;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    /// <summary>
    /// The exception that is thrown when an OpenGL API call resulted in an error.
    /// </summary>
    public class GLException : Exception
    {
        public readonly ErrorCode Error;

        public GLException(ErrorCode error)
            // ReSharper disable once HeapView.BoxingAllocation
            : base($"OpenGL Internal Error:{error.ToString()}")
        {
            Error = error;
        }
    }
}