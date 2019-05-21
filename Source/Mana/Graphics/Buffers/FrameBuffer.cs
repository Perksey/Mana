using System;
using Mana.Utilities;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    public class FrameBuffer : IGraphicsResource
    {
        public readonly GLHandle Handle;
        public readonly Texture2D Texture;
        public readonly GLHandle DepthHandle;
        public readonly FrameBufferFlags Flags;
        public readonly int Width;
        public readonly int Height;
        
        internal bool Disposed;
        
        public FrameBuffer(GraphicsDevice graphicsDevice, int width, int height, FrameBufferFlags flags)
        {
            GraphicsDevice = graphicsDevice;
            Width = width;
            Height = height;
            Flags = flags;
            
            GraphicsDevice.Resources.Add(this);

            if (graphicsDevice.DirectStateAccessSupported)
            {
                GL.CreateFramebuffers(1, out int framebuffer);
                Handle = (GLHandle)framebuffer;
            }
            else
            {
                Handle = (GLHandle)GL.GenFramebuffer();
            }

            Assert.That(Handle != GLHandle.Zero);

            // Initialize Color Component (Texture2D)
            if ((Flags & FrameBufferFlags.Color) != 0)
            {
                Texture = Texture2D.CreateEmpty(graphicsDevice, width, height);

                if (GraphicsDevice.DirectStateAccessSupported)
                {
                    GL.NamedFramebufferTexture(Handle,
                                               FramebufferAttachment.ColorAttachment0,
                                               Texture.Handle,
                                               0);
                }
                else
                {
                    GLHandle prevFrameBuffer = (GLHandle)GL.GetInteger(GetPName.FramebufferBinding);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
                    
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                                          FramebufferAttachment.ColorAttachment0,
                                          TextureTarget.Texture2D,
                                          Texture.Handle,
                                          0);
                    
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, prevFrameBuffer);
                }
            }

            // Initialize Depth Component (Renderbuffer)
            if ((Flags & FrameBufferFlags.Depth) != 0)
            {
                if (graphicsDevice.DirectStateAccessSupported)
                {
                    GL.CreateRenderbuffers(1, out int renderbuffer);
                    DepthHandle = (GLHandle)renderbuffer;
                }
                else
                {
                    DepthHandle = (GLHandle)GL.GenRenderbuffer();
                }

                if (graphicsDevice.DirectStateAccessSupported)
                {
                    GL.NamedRenderbufferStorage(Handle,
                                                RenderbufferStorage.DepthComponent,
                                                width,
                                                height);
                    GL.NamedFramebufferRenderbuffer(Handle,
                                                    FramebufferAttachment.DepthAttachment,
                                                    RenderbufferTarget.Renderbuffer,
                                                    DepthHandle);
                }
                else
                {
                    GLHandle prevFrameBuffer = (GLHandle)GL.GetInteger(GetPName.FramebufferBinding);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
                    
                    GLHandle prevRenderBuffer = (GLHandle)GL.GetInteger(GetPName.RenderbufferBinding);
                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthHandle);

                    GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer,
                                           RenderbufferStorage.DepthComponent,
                                           width,
                                           height);
                    GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                                               FramebufferAttachment.DepthAttachment,
                                               RenderbufferTarget.Renderbuffer,
                                               DepthHandle);
                    
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, prevFrameBuffer);
                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, prevRenderBuffer);
                }
            }
            
            Assert.That(IsComplete());
        }

        public GraphicsDevice GraphicsDevice { get; }
        
        protected void Bind()
        {
            GraphicsDevice.BindFrameBuffer(this);
        }

        protected void Unbind()
        {
            GraphicsDevice.UnbindFrameBuffer(this);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            GraphicsDevice.Resources.Remove(this);
            Unbind();

            GL.DeleteFramebuffer(Handle);
            
            Disposed = true;
        }

        private bool IsComplete()
        {
            GLHandle prevFBO = (GLHandle)GL.GetInteger(GetPName.FramebufferBinding);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
            
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, prevFBO);
            
            return status == FramebufferErrorCode.FramebufferComplete;
        }

        public static implicit operator Texture2D(FrameBuffer frameBuffer)
        {
            return frameBuffer.Texture;
        }
    }
    
    [Flags]
    public enum FrameBufferFlags
    {
        Color = 1,
        Depth = 2,
    }
}