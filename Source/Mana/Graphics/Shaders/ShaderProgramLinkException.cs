﻿using System;

namespace Mana.Graphics.Shaders
{
    /// <summary>
    /// The exception that is thrown when OpenGL encounters an error during shader program linking.
    /// </summary>
    public class ShaderProgramLinkException : Exception
    {
        public ShaderProgramLinkException(string message)
            : base(message.TrimEnd('\n'))
        {
        }
    }
}
