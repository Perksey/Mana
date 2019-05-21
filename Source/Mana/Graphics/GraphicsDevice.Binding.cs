using System;
using System.Runtime.CompilerServices;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shaders;
using Mana.Utilities;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public partial class GraphicsDevice
    {
        internal BindingPoints Bindings;
        
        /// <summary>
        /// Binds the given <see cref="VertexBuffer"/> object to the GraphicsDevice.
        /// </summary>
        /// <param name="vbo">The <see cref="VertexBuffer"/> object to bind.</param>
        public void BindVertexBuffer(VertexBuffer vbo)
        {
            if (vbo == null)
            {
                if (Bindings.VertexBuffer == GLHandle.Zero) 
                    return;
                
                GL.BindBuffer(BufferTarget.ArrayBuffer, GLHandle.Zero);
                Bindings.VertexBuffer = GLHandle.Zero;

                return;
            }
            
            Assert.That(!vbo.Disposed);

            if (Bindings.VertexBuffer == vbo.Handle)
                return;

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.Handle);
            Bindings.VertexBuffer = vbo.Handle;
        }
        
        /// <summary>
        /// Ensures that the given <see cref="VertexBuffer"/> object is unbound.
        /// </summary>
        /// <param name="vbo">The <see cref="VertexBuffer"/> object to ensure is unbound.</param>
        public void UnbindVertexBuffer(VertexBuffer vbo)
        {
            if (vbo == null)
                throw new ArgumentNullException(nameof(vbo));

            if (Bindings.VertexBuffer != vbo.Handle)
                return;

            GL.BindBuffer(BufferTarget.ArrayBuffer, GLHandle.Zero);
            Bindings.VertexBuffer = GLHandle.Zero;
        }
        
        /// <summary>
        /// Binds the given <see cref="IndexBuffer"/> object to the GraphicsDevice.
        /// </summary>
        /// <param name="ebo">The <see cref="IndexBuffer"/> object to bind.</param>
        public void BindIndexBuffer(IndexBuffer ebo)
        {
            if (ebo == null)
            {
                if (Bindings.IndexBuffer == GLHandle.Zero) 
                    return;
                
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, GLHandle.Zero);
                Bindings.IndexBuffer = GLHandle.Zero;
                return;
            }

            Assert.That(!ebo.Disposed);

            if (Bindings.IndexBuffer == ebo.Handle)
                return;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo.Handle);
            Bindings.IndexBuffer = ebo.Handle;
        }

        /// <summary>
        /// Ensures that the given <see cref="IndexBuffer"/> object is unbound.
        /// </summary>
        /// <param name="ebo">The <see cref="IndexBuffer"/> object to ensure is unbound.</param>
        public void UnbindIndexBuffer(IndexBuffer ebo)
        {
            if (ebo == null)
                throw new ArgumentNullException(nameof(ebo));

            if (Bindings.IndexBuffer != ebo.Handle)
                return;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GLHandle.Zero);
            Bindings.IndexBuffer = GLHandle.Zero;
        }
        
        /// <summary>
        /// Binds the given <see cref="PixelBuffer"/> object to the GraphicsDevice.
        /// </summary>
        /// <param name="pbo">The <see cref="PixelBuffer"/> object to bind.</param>
        public void BindPixelBuffer(PixelBuffer pbo)
        {
            if (pbo == null)
            {
                if (Bindings.PixelBuffer == GLHandle.Zero) 
                    return;
                
                GL.BindBuffer(BufferTarget.PixelUnpackBuffer, GLHandle.Zero);
                Bindings.PixelBuffer = GLHandle.Zero;
                return;
            }

            Assert.That(!pbo.Disposed);

            if (Bindings.PixelBuffer == pbo.Handle)
                return;

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, pbo.Handle);
            Bindings.PixelBuffer = pbo.Handle;
        }

        /// <summary>
        /// Ensures that the given <see cref="PixelBuffer"/> object is unbound.
        /// </summary>
        /// <param name="pbo">The <see cref="PixelBuffer"/> object to ensure is unbound.</param>
        public void UnbindPixelBuffer(PixelBuffer pbo)
        {
            if (pbo == null)
                throw new ArgumentNullException(nameof(pbo));

            if (Bindings.PixelBuffer != pbo.Handle)
                return;

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, GLHandle.Zero);
            Bindings.PixelBuffer = GLHandle.Zero;
        }

        /// <summary>
        /// Binds the given <see cref="FrameBuffer"/> object to the GraphicsDevice.
        /// </summary>
        /// <param name="fbo">The <see cref="FrameBuffer"/> object to bind.</param>
        public void BindFrameBuffer(FrameBuffer fbo)
        {
            if (fbo == null)
            {
                if (Bindings.FrameBuffer == GLHandle.Zero) 
                    return;
                
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLHandle.Zero);
                Bindings.FrameBuffer = GLHandle.Zero;
                return;
            }

            Assert.That(!fbo.Disposed);

            if (Bindings.FrameBuffer == fbo.Handle)
                return;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo.Handle);
            Bindings.FrameBuffer = fbo.Handle;
        }
        
        /// <summary>
        /// Ensures that the given <see cref="FrameBuffer"/> object is unbound.
        /// </summary>
        /// <param name="fbo">The <see cref="FrameBuffer"/> object to ensure is unbound.</param>
        public void UnbindFrameBuffer(FrameBuffer fbo)
        {
            if (fbo == null)
                throw new ArgumentNullException(nameof(fbo));

            if (Bindings.FrameBuffer != fbo.Handle)
                return;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLHandle.Zero);
            Bindings.FrameBuffer = GLHandle.Zero;
        }
        
        /// <summary>
        /// Binds the given <see cref="ShaderProgram"/> object to the GraphicsDevice.
        /// </summary>
        /// <param name="program">The <see cref="ShaderProgram"/> object to bind.</param>
        public void BindShaderProgram(ShaderProgram program)
        {
            if (program == null)
            {
                if (Bindings.ShaderProgram == GLHandle.Zero) 
                    return;
                
                GL.UseProgram(0);
                Bindings.ShaderProgram = GLHandle.Zero;
                return;
            }

            Assert.That(!program.Disposed && program.Linked);

            if (Bindings.ShaderProgram == program.Handle)
                return;

            GL.UseProgram(program.Handle);
            Bindings.ShaderProgram = program.Handle;
        }

        /// <summary>
        /// Ensures that the given <see cref="ShaderProgram"/> object is unbound.
        /// </summary>
        /// <param name="program">The <see cref="ShaderProgram"/> object to ensure is unbound.</param>
        public void UnbindShaderProgram(ShaderProgram program)
        {
            if (program == null)
                throw new ArgumentNullException(nameof(program));

            if (Bindings.ShaderProgram != program.Handle)
                return;

            GL.UseProgram(GLHandle.Zero);
            Bindings.ShaderProgram = GLHandle.Zero;
        }

        /// <summary>
        /// Binds the given <see cref="Texture2D"/> to the GraphicsDevice at the given texture unit location.
        /// </summary>
        /// <param name="textureUnit">The texture unit that the texture will be bound to.</param>
        /// <param name="texture">The <see cref="Texture2D"/> to bind.</param>
        public void BindTexture(int textureUnit, Texture2D texture)
        {
            if (textureUnit < 0 || textureUnit >= _maxTextureImageUnits)
                throw new ArgumentOutOfRangeException();

            SetActiveTexture(textureUnit);

            if (texture == null)
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);

                Bindings.Texture = GLHandle.Zero;
                return;
            }
            
            Assert.That(!texture.Disposed);

            if (Bindings.Texture == texture.Handle)
                return;

            GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
            Bindings.Texture = texture.Handle;
        }

        /// <summary>
        /// Ensures that the given <see cref="Texture2D"/> object is unbound.
        /// </summary>
        /// <param name="texture">The <see cref="Texture2D"/> object to ensure is unbound.</param>
        public void UnbindTexture(Texture2D texture)
        {
            for (int i = 0; i < _maxTextureImageUnits; i++)
            {
                if (Bindings.TextureUnits[i] == texture.Handle)
                {
                    SetActiveTexture(i);

                    GL.BindTexture(TextureTarget.Texture2D, GLHandle.Zero);
                    Bindings.Texture = GLHandle.Zero;
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetActiveTexture(int activeTexture)
        {
            if (Bindings.ActiveTexture != activeTexture)
            {
                Bindings.ActiveTexture = activeTexture;
                GL.ActiveTexture((TextureUnit)(activeTexture + (int)TextureUnit.Texture0));
            }
        }
        
        internal struct BindingPoints
        {
            public GLHandle VertexBuffer;
            public GLHandle IndexBuffer;
            public GLHandle FrameBuffer;
            public GLHandle PixelBuffer;
            public GLHandle ShaderProgram;

            public int ActiveTexture;
            public GLHandle[] TextureUnits;

            public GLHandle Texture
            {
                get => TextureUnits[ActiveTexture];
                set => TextureUnits[ActiveTexture] = value;
            }
        }
    }
}