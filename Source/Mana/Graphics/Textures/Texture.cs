using System;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Textures
{
    public abstract class Texture : GraphicsResource
    {
        protected Texture(ResourceManager resourceManager) 
            : base(resourceManager)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            if (!TextureTargetType.HasValue)
                throw new InvalidOperationException("Texture subtype does not specify a TextureTarget.");

            Handle = GLHelper.CreateTexture(TextureTargetType.Value);
            
            resourceManager.OnResourceCreated(this);
        }

        internal abstract TextureTarget? TextureTargetType { get; }

        public abstract void Bind(int slot, RenderContext renderContext);
        
        public abstract void Unbind(RenderContext renderContext);
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (BoundContext != null)
            {
                Unbind(BoundContext);
                BoundContext = null;
            }

            GL.DeleteTexture(Handle);
        }
    }
}