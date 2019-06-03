using System;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    public class PixelBuffer : Buffer
    {
        public PixelBuffer(ResourceManager resourceManager) 
            : base(resourceManager)
        {
        }

        public static PixelBuffer Create<T>(RenderContext context,
                                            int sizeInBytes,
                                            bool immutable,
                                            bool mapWrite = true)
            where T : unmanaged
        {
            var pbo = new PixelBuffer(context.ResourceManager);
            
            pbo.Allocate<T>(context,
                            sizeInBytes,
                            BufferUsageHint.DynamicCopy,
                            immutable,
                            mapWrite 
                                ? BufferStorageFlags.MapWriteBit
                                : BufferStorageFlags.None);
            return pbo;
        }

        internal override BufferTarget BufferTarget => BufferTarget.PixelUnpackBuffer;
        
        protected override void Bind(RenderContext renderContext) => renderContext.BindPixelBuffer(this);
        protected override void Unbind(RenderContext renderContext) => renderContext.UnbindPixelBuffer(this);
        // protected override void Push(RenderContext renderContext) => renderContext.PixelBufferBinding.Push(this);
        // protected override void Pop(RenderContext renderContext) => renderContext.PixelBufferBinding.Pop();
    }
}