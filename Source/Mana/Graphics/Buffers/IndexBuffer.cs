using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    public class IndexBuffer : IGraphicsResource
    {
        public readonly GLHandle Handle;

        internal bool Disposed = false;
        internal int IndexCount = -1;
        internal int ElementSizeInBytes = -1;
        internal int SizeInBytes = -1;
        internal DrawElementsType DataType;
        internal bool IsDynamic;

        private IndexBuffer(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            GraphicsDevice.Resources.Add(this);

            Handle = (GLHandle)GL.GenBuffer();
            GLHelper.CheckLastError();
        }

        public static unsafe IndexBuffer Create(GraphicsDevice graphicsDevice,
                                                ushort[] data,
                                                BufferUsage bufferUsage,
                                                bool dynamic = false)
        {
            IndexBuffer ibo = new IndexBuffer(graphicsDevice)
            {
                ElementSizeInBytes = sizeof(ushort),
                IndexCount = data.Length,
                IsDynamic = dynamic,
                DataType = DrawElementsType.UnsignedShort
            };

            ibo.SizeInBytes = ibo.ElementSizeInBytes * ibo.IndexCount;
            
            graphicsDevice.BindIndexBuffer(ibo);

            if (graphicsDevice.Extensions.ARB_BufferStorage || graphicsDevice.IsVersionAtLeast(4, 4))
            {
                GL.BufferStorage(BufferTarget.ElementArrayBuffer,
                                 ibo.SizeInBytes,
                                 data,
                                 ibo.IsDynamic
                                     ? BufferStorageFlags.DynamicStorageBit
                                     : BufferStorageFlags.None);
                GLHelper.CheckLastError();
            }
            else
            {
                GL.BufferData(BufferTarget.ElementArrayBuffer,
                              ibo.SizeInBytes,
                              data,
                              (BufferUsageHint)bufferUsage);
                GLHelper.CheckLastError();
            }

            return ibo;
        }
        
        public static unsafe IndexBuffer Create(GraphicsDevice graphicsDevice,
                                                int elementCount,
                                                int elementSizeInBytes,
                                                DrawElementsType dataType,
                                                BufferUsage bufferUsage,
                                                bool clear = true,
                                                bool dynamic = true)
        {
            IndexBuffer ibo = new IndexBuffer(graphicsDevice)
            {
                ElementSizeInBytes = elementSizeInBytes,
                SizeInBytes = elementSizeInBytes * elementCount,
                IndexCount = 0,
                IsDynamic = dynamic,
                DataType = dataType
            };

            graphicsDevice.BindIndexBuffer(ibo);

            if (graphicsDevice.Extensions.ARB_BufferStorage || graphicsDevice.IsVersionAtLeast(4, 4))
            {
                GL.BufferStorage(BufferTarget.ElementArrayBuffer,
                                 ibo.SizeInBytes,
                                 clear ? new byte[ibo.SizeInBytes] : null,
                                 ibo.IsDynamic
                                     ? BufferStorageFlags.DynamicStorageBit
                                     : BufferStorageFlags.None);
                GLHelper.CheckLastError();
            }
            else
            {
                GL.BufferData(BufferTarget.ElementArrayBuffer,
                              ibo.SizeInBytes,
                              clear ? new byte[ibo.SizeInBytes] : null,
                              (BufferUsageHint)bufferUsage);
                GLHelper.CheckLastError();
            }

            return ibo;
        }
        
        public GraphicsDevice GraphicsDevice { get; }

        public void SetData(byte[] data, int sizeInBytes)
        {
            if (sizeInBytes > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));
            
            if (!IsDynamic)
                throw new InvalidOperationException("Cannot modify a non-dynamic IndexBuffer.");

            GraphicsDevice.BindIndexBuffer(this);

            GL.BufferSubData(BufferTarget.ElementArrayBuffer,
                             IntPtr.Zero,
                             sizeInBytes,
                             data);
            GLHelper.CheckLastError();

            IndexCount = sizeInBytes / ElementSizeInBytes;
        }
        
        public void Dispose()
        {
            Debug.Assert(!Disposed);

            GraphicsDevice.Resources.Remove(this);
            GraphicsDevice.UnbindIndexBuffer(this);

            GL.DeleteBuffer(Handle);
            GLHelper.CheckLastError();

            Disposed = true;
        }
    }
}