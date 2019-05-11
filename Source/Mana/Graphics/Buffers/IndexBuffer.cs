using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        internal bool IsImmutable = false;

        private IndexBuffer(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            GraphicsDevice.Resources.Add(this);

            if (graphicsDevice.Extensions.ARB_DirectStateAccess || graphicsDevice.IsVersionAtLeast(4, 5))
            {
                GL.CreateBuffers(1, out int bufferInt);
                GLHelper.CheckLastError();
                Handle = (GLHandle)bufferInt;
            }
            else
            {
                Handle = (GLHandle)GL.GenBuffer();
                GLHelper.CheckLastError();
            }
        }
        
        public GraphicsDevice GraphicsDevice { get; }
        
        public static IndexBuffer Create(GraphicsDevice graphicsDevice,
                                                ushort[] data,
                                                BufferUsage bufferUsage,
                                                bool dynamic = false,
                                                bool mutable = false)
        {
            var ibo = new IndexBuffer(graphicsDevice)
            {
                ElementSizeInBytes = sizeof(ushort),
                IndexCount = data.Length,
                IsDynamic = dynamic,
                DataType = DrawElementsType.UnsignedShort
            };

            ibo.SizeInBytes = ibo.ElementSizeInBytes * ibo.IndexCount;
            
            if (graphicsDevice.NamedBuffersSupported)
            {
                if (!mutable && graphicsDevice.ImmutableStorageSupported)
                {
                    GL.NamedBufferStorage(ibo.Handle,
                                     ibo.SizeInBytes,
                                     data,
                                     ibo.IsDynamic
                                         ? BufferStorageFlags.DynamicStorageBit
                                         : BufferStorageFlags.None);
                    GLHelper.CheckLastError();
                    ibo.IsImmutable = true;
                }
                else
                {
                    GL.NamedBufferData(ibo.Handle,
                                  ibo.SizeInBytes,
                                  data,
                                  (BufferUsageHint)bufferUsage);
                    GLHelper.CheckLastError();
                }
            }
            else
            {
                graphicsDevice.BindIndexBuffer(ibo);
                
                if (!mutable && graphicsDevice.ImmutableStorageSupported)
                {
                    GL.BufferStorage(BufferTarget.ElementArrayBuffer,
                                     ibo.SizeInBytes,
                                     data,
                                     ibo.IsDynamic
                                         ? BufferStorageFlags.DynamicStorageBit
                                         : BufferStorageFlags.None);
                    GLHelper.CheckLastError();
                    ibo.IsImmutable = true;
                }
                else
                {
                    GL.BufferData(BufferTarget.ElementArrayBuffer,
                                  ibo.SizeInBytes,
                                  data,
                                  (BufferUsageHint)bufferUsage);
                    GLHelper.CheckLastError();
                }
            }

            return ibo;
        }
        
        public static IndexBuffer Create(GraphicsDevice graphicsDevice,
                                                int elementCount,
                                                int elementSizeInBytes,
                                                DrawElementsType dataType,
                                                BufferUsage bufferUsage,
                                                bool clear = true,
                                                bool dynamic = true,
                                                bool mutable = false)
        {
            var ibo = new IndexBuffer(graphicsDevice)
            {
                ElementSizeInBytes = elementSizeInBytes,
                SizeInBytes = elementSizeInBytes * elementCount,
                IndexCount = 0,
                IsDynamic = dynamic,
                DataType = dataType
            };

            if (graphicsDevice.NamedBuffersSupported)
            {
                if (!mutable && graphicsDevice.ImmutableStorageSupported)
                {
                    GL.NamedBufferStorage(ibo.Handle,
                                     ibo.SizeInBytes,
                                     clear ? new byte[ibo.SizeInBytes] : null,
                                     ibo.IsDynamic
                                         ? BufferStorageFlags.DynamicStorageBit
                                         : BufferStorageFlags.None);
                    GLHelper.CheckLastError();
                    ibo.IsImmutable = true;
                }
                else
                {
                    GL.NamedBufferData(ibo.Handle,
                                  ibo.SizeInBytes,
                                  clear ? new byte[ibo.SizeInBytes] : null,
                                  (BufferUsageHint)bufferUsage);
                    GLHelper.CheckLastError();
                }
            }
            else
            {
                graphicsDevice.BindIndexBuffer(ibo);

                if (!mutable && graphicsDevice.ImmutableStorageSupported)
                {
                    GL.BufferStorage(BufferTarget.ElementArrayBuffer,
                                     ibo.SizeInBytes,
                                     clear ? new byte[ibo.SizeInBytes] : null,
                                     ibo.IsDynamic
                                         ? BufferStorageFlags.DynamicStorageBit
                                         : BufferStorageFlags.None);
                    GLHelper.CheckLastError();
                    ibo.IsImmutable = true;
                }
                else
                {
                    GL.BufferData(BufferTarget.ElementArrayBuffer,
                                  ibo.SizeInBytes,
                                  clear ? new byte[ibo.SizeInBytes] : null,
                                  (BufferUsageHint)bufferUsage);
                    GLHelper.CheckLastError();
                }
            }

            return ibo;
        }
        

        public void SetData(byte[] data, int sizeInBytes)
        {
            if (sizeInBytes > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));
            
            if (!IsDynamic)
                throw new InvalidOperationException("Cannot modify a non-dynamic IndexBuffer.");

            if (IsImmutable)
                throw new InvalidOperationException("Cannot modify an immutable IndexBuffer.");
            
            if (GraphicsDevice.NamedBuffersSupported)
            {
                GL.NamedBufferSubData(Handle,
                                      IntPtr.Zero,
                                      sizeInBytes,
                                      data);
                GLHelper.CheckLastError();
            }
            else
            {
                GraphicsDevice.BindIndexBuffer(this);

                GL.BufferSubData(BufferTarget.ElementArrayBuffer,
                                 IntPtr.Zero,
                                 sizeInBytes,
                                 data);
                GLHelper.CheckLastError();
            }

            IndexCount = sizeInBytes / ElementSizeInBytes;
        }
        
        public void SetData(ushort[] data, int sizeInBytes)
        {
            if (sizeInBytes > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));
            
            if (!IsDynamic)
                throw new InvalidOperationException("Cannot modify a non-dynamic IndexBuffer.");

            if (IsImmutable)
                throw new InvalidOperationException("Cannot modify an immutable IndexBuffer.");
            
            if (GraphicsDevice.NamedBuffersSupported)
            {
                GL.NamedBufferSubData(Handle,
                                 IntPtr.Zero,
                                 sizeInBytes,
                                 data);
                GLHelper.CheckLastError(); 
            }
            else
            {
                GraphicsDevice.BindIndexBuffer(this);

                GL.BufferSubData(BufferTarget.ElementArrayBuffer,
                                 IntPtr.Zero,
                                 sizeInBytes,
                                 data);
                GLHelper.CheckLastError();                
            }

            IndexCount = sizeInBytes / ElementSizeInBytes;
        }
        
        public void SubData(ushort[] data, IntPtr offset)
        {
            if (data.Length * sizeof(ushort) > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));
            
            if (!IsDynamic)
                throw new InvalidOperationException("Cannot modify a non-dynamic IndexBuffer");

            if (GraphicsDevice.NamedBuffersSupported)
            {
                GraphicsDevice.BindIndexBuffer(this);

                GL.NamedBufferSubData(Handle,
                                      offset,
                                      sizeof(ushort) * data.Length,
                                      data);
                GLHelper.CheckLastError();
            }
            else
            {
                GraphicsDevice.BindIndexBuffer(this);
            
                GL.BufferSubData(BufferTarget.ElementArrayBuffer,
                                 offset, 
                                 sizeof(ushort) * data.Length,
                                 data);
                GLHelper.CheckLastError();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubData(ushort[] data, int offsetIndex)
        {
            SubData(data, new IntPtr(offsetIndex * sizeof(ushort)));
        }

        public void SetDataPointer(IntPtr data, int dataSizeInBytes)
        {
            if (dataSizeInBytes > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));

            if (IsImmutable)
                throw new InvalidOperationException("Cannot modify an immutable IndexBuffer.");

            if (GraphicsDevice.NamedBuffersSupported)
            {
                GL.NamedBufferData(Handle, (IntPtr)dataSizeInBytes, data, BufferUsageHint.DynamicDraw);
                GLHelper.CheckLastError();                
            }
            else
            {
                GraphicsDevice.BindIndexBuffer(this);
                
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)dataSizeInBytes, data, BufferUsageHint.DynamicDraw);
                GLHelper.CheckLastError();
            }
        }
        
        public void SubDataPointer(int offsetInBytes,
                                   IntPtr data,
                                   int dataSizeInBytes)
        {
            if (dataSizeInBytes > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));

            if (GraphicsDevice.NamedBuffersSupported)
            {
                GL.NamedBufferSubData(Handle, (IntPtr)offsetInBytes, (IntPtr)dataSizeInBytes, data);
                GLHelper.CheckLastError();
            }
            else
            {
                GraphicsDevice.BindIndexBuffer(this);
                
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)offsetInBytes, (IntPtr)dataSizeInBytes, data);
                GLHelper.CheckLastError();
            }
        }
        
        internal void DiscardData()
        {
            SetDataPointer(IntPtr.Zero, SizeInBytes);
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