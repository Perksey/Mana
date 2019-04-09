using System;
using System.Diagnostics;
using Mana.Graphics.Vertex;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    public class VertexBuffer : IGraphicsResource
    {
        public readonly GLHandle Handle;
        public readonly VertexTypeInfo VertexTypeInfo;

        internal bool Disposed = false;
        internal int VertexCount = -1;
        internal int SizeInBytes = -1;
        internal bool IsDynamic;


        private VertexBuffer(GraphicsDevice graphicsDevice, VertexTypeInfo vertexTypeInfo)
        {
            GraphicsDevice = graphicsDevice;
            GraphicsDevice.Resources.Add(this);
            
            Handle = (GLHandle)GL.GenBuffer();
            GLHelper.CheckLastError();

            VertexTypeInfo = vertexTypeInfo;
        }
        
        public static unsafe VertexBuffer Create<T>(GraphicsDevice graphicsDevice, 
                                                    T[] data, 
                                                    BufferUsage bufferUsage, 
                                                    bool dynamic = false)
            where T : unmanaged
        {
            VertexBuffer vbo = new VertexBuffer(graphicsDevice, VertexTypeInfo.Get<T>())
            {
                SizeInBytes = data.Length * sizeof(T),
                VertexCount = data.Length,
                IsDynamic = dynamic,
            };
            
            graphicsDevice.BindVertexBuffer(vbo);
            
            if (graphicsDevice.Extensions.ARB_BufferStorage || graphicsDevice.IsVersionAtLeast(4, 4))
            {
                GL.BufferStorage(BufferTarget.ArrayBuffer, 
                                 vbo.SizeInBytes, 
                                 data, 
                                 vbo.IsDynamic 
                                     ? BufferStorageFlags.DynamicStorageBit 
                                     : BufferStorageFlags.None);
                GLHelper.CheckLastError();    
            }
            else
            {
                GL.BufferData(BufferTarget.ArrayBuffer,
                              vbo.SizeInBytes,
                              data,
                              (BufferUsageHint)bufferUsage);
                GLHelper.CheckLastError();
            }

            return vbo;
        }
        
        public static VertexBuffer Create(GraphicsDevice graphicsDevice, 
                                          int sizeInBytes,
                                          VertexTypeInfo vertexTypeInfo,
                                          BufferUsage bufferUsage, 
                                          bool clear = true, 
                                          bool dynamic = true)
        {
            VertexBuffer vbo = new VertexBuffer(graphicsDevice, vertexTypeInfo)
            {
                SizeInBytes = sizeInBytes, 
                VertexCount = 0,
                IsDynamic = dynamic,
            };
            
            graphicsDevice.BindVertexBuffer(vbo);

            if (graphicsDevice.Extensions.ARB_BufferStorage || graphicsDevice.IsVersionAtLeast(4, 4))
            {
                GL.BufferStorage(BufferTarget.ArrayBuffer,
                                 vbo.SizeInBytes,
                                 clear ? new byte[sizeInBytes] : null,
                                 vbo.IsDynamic
                                     ? BufferStorageFlags.DynamicStorageBit
                                     : BufferStorageFlags.None);
                GLHelper.CheckLastError();
            }
            else
            {
                GL.BufferData(BufferTarget.ArrayBuffer,
                              vbo.SizeInBytes,
                              clear ? new byte[sizeInBytes] : null,
                              (BufferUsageHint)bufferUsage);
                GLHelper.CheckLastError();
            }

            return vbo;
        }
        
        public GraphicsDevice GraphicsDevice { get; }

        public unsafe void SetData<T>(T[] data)
            where T : unmanaged
        {
            if (data.Length * sizeof(T) > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));
            
            if (!IsDynamic)
                throw new InvalidOperationException("Cannot modify a non-dynamic VertexBuffer.");
            
            GraphicsDevice.BindVertexBuffer(this);

            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(T) * data.Length, data);
            GLHelper.CheckLastError();

            VertexCount = data.Length;
        }
        
        public unsafe void SetData<T>(T[] data, int sizeInBytes)
            where T : unmanaged
        {
            if (sizeInBytes > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));
            
            if (!IsDynamic)
                throw new InvalidOperationException("Cannot modify a non-dynamic VertexBuffer.");
            
            GraphicsDevice.BindVertexBuffer(this);

            GL.BufferSubData(BufferTarget.ArrayBuffer, 
                             IntPtr.Zero, 
                             sizeInBytes, 
                             data);
            GLHelper.CheckLastError();

            VertexCount = sizeInBytes / sizeof(T);
        }
        
        public unsafe void SubData<T>(T[] data, IntPtr offset)
            where T : unmanaged
        {
            if (data.Length * sizeof(T) > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));
            
            if (!IsDynamic)
                throw new InvalidOperationException("Cannot modify a non-dynamic VertexBuffer.");
            
            GraphicsDevice.BindVertexBuffer(this);

            GL.BufferSubData(BufferTarget.ArrayBuffer, offset, sizeof(T) * data.Length, data);
            GLHelper.CheckLastError();
        }

        public void Dispose()
        {
            Debug.Assert(!Disposed);

            GraphicsDevice.Resources.Remove(this);
            GraphicsDevice.UnbindVertexBuffer(this);

            GL.DeleteBuffer(Handle);
            GLHelper.CheckLastError();
            
            Disposed = true;
        }
    }
}