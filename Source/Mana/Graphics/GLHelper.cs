using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mana.Graphics.Textures;
using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    /// <summary>
    /// A static class containing useful functions for making certain OpenGL API calls.
    /// </summary>
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetCap(EnableCap cap, bool value)
        {
            if (value)
            {
                GL.Enable(cap);
            }
            else
            {
                GL.Disable(cap);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static GLHandle CreateFrameBuffer()
        {
            GLHandle handle;

            if (GLInfo.HasDirectStateAccess)
            {
                GL.CreateFramebuffers(1, out int framebuffer);
                handle = (GLHandle)framebuffer;
            }
            else
            {
                handle = (GLHandle)GL.GenFramebuffer();
            }

            EnsureValid(handle);
            return handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static GLHandle CreateBuffer()
        {
            GLHandle handle;

            if (GLInfo.HasDirectStateAccess)
            {
                GL.CreateBuffers(1, out int buffer);
                handle = (GLHandle)buffer;
            }
            else
            {
                handle = (GLHandle)GL.GenBuffer();
            }

            EnsureValid(handle);
            return handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static GLHandle CreateTexture(TextureTarget target)
        {
            GLHandle handle;

            if (GLInfo.HasDirectStateAccess)
            {
                GL.CreateTextures(target, 1, out int texture);
                handle = (GLHandle)texture;
            }
            else
            {
                handle = (GLHandle)GL.GenTexture();
            }

            EnsureValid(handle);
            return handle;
        }

        internal static void TextureParameter(RenderContext context,
                                              TextureTarget target,
                                              Texture2D texture,
                                              TextureParameterName parameter,
                                              int value)
        {
            if (GLInfo.HasDirectStateAccess)
            {
                GL.TextureParameter(texture.Handle, parameter, value);
            }
            else
            {
                context.BindTexture(0, texture);
                GL.TexParameter(target, parameter, value);
            }
        }

        internal static void EnsureValid(GLHandle handle)
        {
            if (handle == GLHandle.Zero)
            {
                throw new InvalidOperationException("OpenGL error: Invalid handle generated.");
            }
        }
    }
}
