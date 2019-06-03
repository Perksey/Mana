using Mana.Graphics.Textures;
using Mana.Utilities;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    /// <summary>
    /// Represents an abstraction over an OpenGL frame buffer object.
    /// </summary>
    public class FrameBuffer : GraphicsResource
    {
        public readonly GLHandle DepthHandle;
        public readonly Texture2D ColorTexture;
        public readonly FrameBufferFlags Flags;
        public readonly int Width;
        public readonly int Height;
        
        public FrameBuffer(RenderContext renderContext, 
                           int width, 
                           int height, 
                           FrameBufferFlags flags) 
            : base(renderContext.ResourceManager)
        {
            Width = width;
            Height = height;
            Flags = flags;

            Handle = GLHelper.CreateFrameBuffer();

            Assert.That(Handle != GLHandle.Zero);

            // Initialize Color Component (Texture2D)
            if ((Flags & FrameBufferFlags.Color) != 0)
            {
                ColorTexture = Texture2D.CreateEmpty(renderContext, width, height);

                if (GLInfo.HasDirectStateAccess)
                {
                    GL.NamedFramebufferTexture(Handle,
                                               FramebufferAttachment.ColorAttachment0,
                                               ColorTexture.Handle,
                                               0);
                }
                else
                {
                    GLHandle prevFrameBuffer = (GLHandle)GL.GetInteger(GetPName.FramebufferBinding);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
                    
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                                            FramebufferAttachment.ColorAttachment0,
                                            TextureTarget.Texture2D,
                                            ColorTexture.Handle,
                                            0);
                    
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, prevFrameBuffer);
                }
            }
            
            // Initialize Depth Component (Renderbuffer)
            if ((Flags & FrameBufferFlags.Depth) != 0)
            {
                if (GLInfo.HasDirectStateAccess)
                {
                    GL.CreateRenderbuffers(1, out int renderbuffer);
                    DepthHandle = (GLHandle)renderbuffer;
                    
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
                    DepthHandle = (GLHandle)GL.GenRenderbuffer();
                    
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
            
            ResourceManager.OnResourceCreated(this);
        }

        protected void Bind(RenderContext renderContext) => renderContext.BindFrameBuffer(this);
        
        protected void Unbind(RenderContext renderContext) => renderContext.UnbindFrameBuffer(this);

        private bool IsComplete()
        {
            GLHandle prevFBO = (GLHandle)GL.GetInteger(GetPName.FramebufferBinding);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
            
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, prevFBO);
            
            return status == FramebufferErrorCode.FramebufferComplete;
        }
    }
}