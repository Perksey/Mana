using System;
using Mana.Graphics.Vertex;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    public class VertexBuffer : Buffer
    {
        public readonly VertexTypeInfo VertexTypeInfo;
        
        private VertexBuffer(ResourceManager resourceManager, VertexTypeInfo vertexTypeInfo)
            : base(resourceManager)
        {
            VertexTypeInfo = vertexTypeInfo;
        }

        internal override BufferTarget BufferTarget => BufferTarget.ArrayBuffer;

        public static VertexBuffer Create<T>(RenderContext renderContext, 
                                             T[] data, 
                                             BufferUsageHint bufferUsageHint,
                                             bool immutable)
            where T : unmanaged
        {
            if (renderContext == null)
                throw new ArgumentNullException(nameof(renderContext));
            
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            
            var vbo = new VertexBuffer(renderContext.ResourceManager, VertexTypeInfo.Get<T>());
            vbo.Allocate<T>(renderContext, data, bufferUsageHint, immutable);
            return vbo;
        }

        public static unsafe VertexBuffer Create<T>(RenderContext renderContext,  
                                                    int capacity, 
                                                    BufferUsageHint bufferUsageHint,
                                                    bool immutable)
            where T : unmanaged
        {
            if (renderContext == null)
                throw new ArgumentNullException(nameof(renderContext));
            
            var vbo = new VertexBuffer(renderContext.ResourceManager, VertexTypeInfo.Get<T>());
            vbo.Allocate<T>(renderContext, capacity * sizeof(T), bufferUsageHint, immutable);
            return vbo;
        }

        protected override void Bind(RenderContext renderContext) => renderContext.BindVertexBuffer(this);
        protected override void Unbind(RenderContext renderContext) => renderContext.UnbindVertexBuffer(this);
        // protected override void Push(RenderContext renderContext) => renderContext.VertexBufferBinding.Push(this);
        // protected override void Pop(RenderContext renderContext) => renderContext.VertexBufferBinding.Pop();
    }
}