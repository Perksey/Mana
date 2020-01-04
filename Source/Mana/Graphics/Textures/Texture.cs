using System;
using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics.Textures
{
    /// <summary>
    /// Represents an OpenGL Texture object.
    /// </summary>
    public abstract class Texture : GraphicsResource
    {
        protected Texture(RenderContext renderContext)
            : base(renderContext)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            if (!TextureTargetType.HasValue)
                throw new InvalidOperationException("Texture subtype does not specify a TextureTarget.");

            // ReSharper disable once VirtualMemberCallInConstructor
            Handle = GLHelper.CreateTexture(TextureTargetType.Value);
        }

        internal abstract TextureTarget? TextureTargetType { get; }

        public abstract void Bind(int slot, RenderContext renderContext);

        public abstract void Unbind(RenderContext renderContext);

        protected override void Dispose(bool disposing)
        {
            if (BoundContext != null)
            {
                Unbind(BoundContext);
                BoundContext = null;
            }

            GL.DeleteTexture(Handle);
        }
    }
}
