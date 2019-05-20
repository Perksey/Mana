using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    public class PixelBuffer : Buffer
    {
        private PixelBuffer(GraphicsDevice graphicsDevice) 
            : base(graphicsDevice, BufferTarget.PixelUnpackBuffer)
        {
        }

        public static PixelBuffer Create<T>(GraphicsDevice graphicsDevice, 
                                            int sizeInBytes,
                                            bool immutable)
            where T : unmanaged
        {
            var pbo = new PixelBuffer(graphicsDevice);
            pbo.AllocateEmpty(sizeInBytes, BufferUsage.DynamicCopy, false);
            return pbo;
        }
        
        protected override void Bind() => GraphicsDevice.BindPixelBuffer(this);
        protected override void Unbind() => GraphicsDevice.UnbindPixelBuffer(this);
    }
}