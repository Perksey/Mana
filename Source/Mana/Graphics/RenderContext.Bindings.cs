using System;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shaders;
using Mana.Graphics.Textures;
using osuTK.Graphics.OpenGL4;

using TextureUnitGL = osuTK.Graphics.OpenGL4.TextureUnit;

namespace Mana.Graphics
{
    public partial class RenderContext
    {
        internal GLHandle VertexBufferHandle = GLHandle.Zero;
        internal VertexBuffer VertexBuffer = null;

        internal GLHandle IndexBufferHandle = GLHandle.Zero;
        internal IndexBuffer IndexBuffer = null;

        internal GLHandle PixelBufferHandle = GLHandle.Zero;
        internal PixelBuffer PixelBuffer = null;

        internal GLHandle FrameBufferHandle = GLHandle.Zero;
        internal FrameBuffer FrameBuffer = null;

        internal GLHandle ShaderProgramHandle = GLHandle.Zero;
        internal ShaderProgram ShaderProgram = null;

        internal readonly TextureUnit[] TextureUnits;
        internal int ActiveTexture = 0;

        /// <summary>
        /// Binds the given <see cref="VertexBuffer"/> object to the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="vbo">The <see cref="VertexBuffer"/> object to bind.</param>
        public void BindVertexBuffer(VertexBuffer vbo)
        {
            if (vbo == null)
            {
                if (VertexBufferHandle == GLHandle.Zero)
                {
                    return;
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, GLHandle.Zero);

                VertexBufferHandle = GLHandle.Zero;
                VertexBuffer.BoundContext = null;
                VertexBuffer = null;

                return;
            }

            vbo.EnsureUndisposed();

            if (VertexBufferHandle == vbo.Handle)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.Handle);

            VertexBufferHandle = vbo.Handle;
            VertexBuffer = vbo;
            VertexBuffer.BoundContext = this;
        }

        /// <summary>
        /// Ensures that the given <see cref="VertexBuffer"/> object is unbound.
        /// </summary>
        /// <param name="vbo">The <see cref="VertexBuffer"/> object to ensure is unbound.</param>
        public void UnbindVertexBuffer(VertexBuffer vbo)
        {
            if (vbo == null)
            {
                throw new ArgumentNullException(nameof(vbo));
            }

            if (VertexBufferHandle != vbo.Handle)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, GLHandle.Zero);

            VertexBufferHandle = GLHandle.Zero;
            VertexBuffer.BoundContext = null;
            VertexBuffer = null;
        }

        /// <summary>
        /// Binds the given <see cref="IndexBuffer"/> object to the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="ibo">The <see cref="IndexBuffer"/> object to bind.</param>
        public void BindIndexBuffer(IndexBuffer ibo)
        {
            if (ibo == null)
            {
                if (IndexBufferHandle == GLHandle.Zero)
                {
                    return;
                }

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, GLHandle.Zero);

                IndexBufferHandle = GLHandle.Zero;
                IndexBuffer.BoundContext = null;
                IndexBuffer = null;

                return;
            }

            ibo.EnsureUndisposed();

            if (IndexBufferHandle == ibo.Handle)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo.Handle);

            IndexBufferHandle = ibo.Handle;
            IndexBuffer = ibo;
            IndexBuffer.BoundContext = this;
        }

        /// <summary>
        /// Ensures that the given <see cref="IndexBuffer"/> object is unbound.
        /// </summary>
        /// <param name="ibo">The <see cref="IndexBuffer"/> object to ensure is unbound.</param>
        public void UnbindIndexBuffer(IndexBuffer ibo)
        {
            if (ibo == null)
            {
                throw new ArgumentNullException(nameof(ibo));
            }

            if (IndexBufferHandle != ibo.Handle)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GLHandle.Zero);

            IndexBufferHandle = GLHandle.Zero;
            IndexBuffer.BoundContext = null;
            IndexBuffer = null;
        }

        /// <summary>
        /// Binds the given <see cref="PixelBuffer"/> object to the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="pbo">The <see cref="PixelBuffer"/> object to bind.</param>
        public void BindPixelBuffer(PixelBuffer pbo)
        {
            if (pbo == null)
            {
                if (PixelBufferHandle == GLHandle.Zero)
                {
                    return;
                }

                GL.BindBuffer(BufferTarget.PixelUnpackBuffer, GLHandle.Zero);

                PixelBufferHandle = GLHandle.Zero;
                PixelBuffer.BoundContext = null;
                PixelBuffer = null;

                return;
            }

            pbo.EnsureUndisposed();

            if (PixelBufferHandle == pbo.Handle)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, pbo.Handle);

            PixelBufferHandle = pbo.Handle;
            PixelBuffer = pbo;
            PixelBuffer.BoundContext = this;
        }

        /// <summary>
        /// Ensures that the given <see cref="PixelBuffer"/> object is unbound.
        /// </summary>
        /// <param name="pbo">The <see cref="PixelBuffer"/> object to ensure is unbound.</param>
        public void UnbindPixelBuffer(PixelBuffer pbo)
        {
            if (pbo == null)
            {
                throw new ArgumentNullException(nameof(pbo));
            }

            if (PixelBufferHandle != pbo.Handle)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, GLHandle.Zero);

            PixelBufferHandle = GLHandle.Zero;
            PixelBuffer.BoundContext = null;
            PixelBuffer = null;
        }

        /// <summary>
        /// Binds the given <see cref="FrameBuffer"/> object to the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="fbo">The <see cref="FrameBuffer"/> object to bind.</param>
        public void BindFrameBuffer(FrameBuffer fbo)
        {
            if (fbo == null)
            {
                if (FrameBufferHandle == GLHandle.Zero)
                {
                    return;
                }

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLHandle.Zero);

                FrameBufferHandle = GLHandle.Zero;
                FrameBuffer.BoundContext = null;
                FrameBuffer = null;

                return;
            }

            fbo.EnsureUndisposed();

            if (FrameBufferHandle == fbo.Handle)
            {
                return;
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo.Handle);

            FrameBufferHandle = fbo.Handle;
            FrameBuffer = fbo;
            FrameBuffer.BoundContext = this;
        }

        /// <summary>
        /// Ensures that the given <see cref="FrameBuffer"/> object is unbound.
        /// </summary>
        /// <param name="fbo">The <see cref="FrameBuffer"/> object to ensure is unbound.</param>
        public void UnbindFrameBuffer(FrameBuffer fbo)
        {
            if (fbo == null)
            {
                throw new ArgumentNullException(nameof(fbo));
            }

            if (FrameBufferHandle != fbo.Handle)
            {
                return;
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLHandle.Zero);

            FrameBufferHandle = GLHandle.Zero;
            FrameBuffer.BoundContext = null;
            FrameBuffer = null;
        }

        /// <summary>
        /// Binds the given <see cref="ShaderProgram"/> object to the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="program">The <see cref="ShaderProgram"/> object to bind.</param>
        public void BindShaderProgram(ShaderProgram program)
        {
            if (program == null)
            {
                if (ShaderProgramHandle == GLHandle.Zero)
                {
                    return;
                }

                GL.UseProgram(GLHandle.Zero);

                ShaderProgramHandle = GLHandle.Zero;
                ShaderProgram.BoundContext = null;
                ShaderProgram = null;

                return;
            }

            program.EnsureUndisposed();

            if (ShaderProgramHandle == program.Handle)
            {
                return;
            }

            GL.UseProgram(program.Handle);

            ShaderProgramHandle = program.Handle;
            ShaderProgram = program;
            ShaderProgram.BoundContext = this;
        }

        /// <summary>
        /// Ensures that the given <see cref="ShaderProgram"/> object is unbound.
        /// </summary>
        /// <param name="program">The <see cref="ShaderProgram"/> object to ensure is unbound.</param>
        public void UnbindShaderProgram(ShaderProgram program)
        {
            if (program == null)
            {
                throw new ArgumentNullException(nameof(program));
            }

            if (ShaderProgramHandle != program.Handle)
            {
                return;
            }

            GL.UseProgram(GLHandle.Zero);

            ShaderProgramHandle = GLHandle.Zero;
            ShaderProgram.BoundContext = null;
            ShaderProgram = null;
        }

        /// <summary>
        /// Binds the given <see cref="Texture2D"/> to the GraphicsDevice at the given texture unit location.
        /// </summary>
        /// <param name="slot">The texture unit slot that the texture will be bound to.</param>
        /// <param name="texture">The <see cref="Texture2D"/> to bind.</param>
        public void BindTexture(int slot, Texture2D texture)
        {
            if (slot < 0 || slot >=  GLInfo.MaxTextureImageUnits)
                throw new ArgumentOutOfRangeException(nameof(slot));

            SetActiveTextureSlot(slot);

            if (texture == null)
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                TextureUnits[slot].Texture2DHandle = GLHandle.Zero;
                TextureUnits[slot].Texture2D.BoundContext = null;
                TextureUnits[slot].Texture2D = null;

                return;
            }

            texture.EnsureUndisposed();

            if (TextureUnits[slot].Texture2DHandle == texture.Handle)
                return;

            GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
            TextureUnits[slot].Texture2DHandle = texture.Handle;
            TextureUnits[slot].Texture2D = texture;
            TextureUnits[slot].Texture2D.BoundContext = this;
        }

        /// <summary>
        /// Ensures that the given <see cref="Texture2D"/> object is unbound.
        /// </summary>
        /// <param name="texture">The <see cref="Texture2D"/> object to ensure is unbound.</param>
        public void UnbindTexture(Texture2D texture)
        {
            for (int i = 0; i < GLInfo.MaxTextureImageUnits; i++)
            {
                if (TextureUnits[i].Texture2DHandle == texture.Handle)
                {
                    SetActiveTextureSlot(i);

                    GL.BindTexture(TextureTarget.Texture2D, GLHandle.Zero);
                    TextureUnits[i].Texture2DHandle = GLHandle.Zero;
                    TextureUnits[i].Texture2D.BoundContext = null;
                    TextureUnits[i].Texture2D = null;
                }
            }
        }

        /// <summary>
        /// Sets the active texture unit slot.
        /// </summary>
        /// <param name="slot">The slot to set as the active texture unit.</param>
        public void SetActiveTextureSlot(int slot)
        {
            if (slot < 0 || slot >=  GLInfo.MaxTextureImageUnits)
                throw new ArgumentOutOfRangeException(nameof(slot));

            if (ActiveTexture == slot)
                return;

            ActiveTexture = slot;
            GL.ActiveTexture((TextureUnitGL)((int)TextureUnitGL.Texture0 + slot));
        }

        internal struct TextureUnit
        {
            public GLHandle Texture2DHandle;
            public Texture2D Texture2D;
        }
    }
}
