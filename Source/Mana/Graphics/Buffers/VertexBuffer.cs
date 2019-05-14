using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mana.Graphics.Vertex;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    public class VertexBuffer : Buffer
    {
        public readonly VertexTypeInfo VertexTypeInfo;

        private VertexBuffer(GraphicsDevice graphicsDevice, VertexTypeInfo vertexTypeInfo)
            : base(graphicsDevice, BufferTarget.ArrayBuffer)
        {
            VertexTypeInfo = vertexTypeInfo;
        }
        
        public static VertexBuffer Create<T>(GraphicsDevice graphicsDevice, 
                                             T[] data, 
                                             BufferUsage bufferUsage,
                                             bool immutable = true,
                                             bool dynamic = false)
            where T : unmanaged
        {
            var vbo = new VertexBuffer(graphicsDevice, VertexTypeInfo.Get<T>());
            vbo.Allocate<T>(data, bufferUsage, immutable, dynamic);
            return vbo;
        }
        
        public static unsafe VertexBuffer Create<T>(GraphicsDevice graphicsDevice, 
                                                    int capacity, 
                                                    BufferUsage bufferUsage,
                                                    bool immutable = true,
                                                    bool dynamic = false)
            where T : unmanaged
        {
            var vbo = new VertexBuffer(graphicsDevice, VertexTypeInfo.Get<T>());
            vbo.Allocate<T>(capacity * sizeof(T), bufferUsage, immutable, dynamic);
            return vbo;
        }
        
        protected override void Bind() => GraphicsDevice.BindVertexBuffer(this);
        protected override void Unbind() => GraphicsDevice.UnbindVertexBuffer(this);
    }
}