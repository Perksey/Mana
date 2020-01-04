using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    /// <summary>
    /// Represents an OpenGL pixel buffer object.
    /// </summary>
    public class PixelBuffer : Buffer
    {
        private PixelBuffer(RenderContext parentContext)
            : base(parentContext)
        {
        }

        public static PixelBuffer Create<T>(RenderContext renderContext,
                                            int sizeInBytes,
                                            bool immutable,
                                            bool mapWrite = true)
            where T : unmanaged
        {
            var pbo = new PixelBuffer(renderContext);

            pbo.Allocate<T>(sizeInBytes,
                            BufferUsageHint.DynamicCopy,
                            immutable,
                            mapWrite
                                ? BufferStorageFlags.MapWriteBit
                                : BufferStorageFlags.None);
            return pbo;
        }

        internal override BufferTarget BufferTarget => BufferTarget.PixelUnpackBuffer;

        public override void Bind(RenderContext renderContext) => renderContext.BindPixelBuffer(this);

        public override void Unbind(RenderContext renderContext) => renderContext.UnbindPixelBuffer(this);
    }
}
