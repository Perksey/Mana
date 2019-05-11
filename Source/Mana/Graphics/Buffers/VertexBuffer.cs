using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        internal bool IsImmutable = false;


        private VertexBuffer(GraphicsDevice graphicsDevice, VertexTypeInfo vertexTypeInfo)
        {
            GraphicsDevice = graphicsDevice;
            GraphicsDevice.Resources.Add(this);

            if (graphicsDevice.NamedBuffersSupported)
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

            VertexTypeInfo = vertexTypeInfo;
        }
        
        public GraphicsDevice GraphicsDevice { get; }
        
        public static unsafe VertexBuffer Create<T>(GraphicsDevice graphicsDevice, 
                                                    T[] data, 
                                                    BufferUsage bufferUsage, 
                                                    bool dynamic = false,
                                                    bool mutable = false)
            where T : unmanaged
        {
            var vbo = new VertexBuffer(graphicsDevice, VertexTypeInfo.Get<T>())
            {
                SizeInBytes = data.Length * sizeof(T),
                VertexCount = data.Length,
                IsDynamic = dynamic,
            };

            if (graphicsDevice.NamedBuffersSupported)
            {
                if (!mutable && graphicsDevice.ImmutableStorageSupported)
                {
                    GL.NamedBufferStorage(vbo.Handle,
                                          vbo.SizeInBytes,
                                          data,
                                          vbo.IsDynamic
                                              ? BufferStorageFlags.DynamicStorageBit
                                              : BufferStorageFlags.None);
                    GLHelper.CheckLastError();
                    vbo.IsImmutable = true;
                }
                else
                {
                    GL.NamedBufferData(vbo.Handle,
                                       vbo.SizeInBytes,
                                       data,
                                       (BufferUsageHint)bufferUsage);
                    GLHelper.CheckLastError();
                }
            }
            else
            {
                graphicsDevice.BindVertexBuffer(vbo);
                
                if (!mutable && graphicsDevice.ImmutableStorageSupported)
                {
                    GL.BufferStorage(BufferTarget.ArrayBuffer, 
                                     vbo.SizeInBytes, 
                                     data, 
                                     vbo.IsDynamic 
                                         ? BufferStorageFlags.DynamicStorageBit 
                                         : BufferStorageFlags.None);
                    GLHelper.CheckLastError();
                    vbo.IsImmutable = true;
                }
                else
                {
                    GL.BufferData(BufferTarget.ArrayBuffer,
                                  vbo.SizeInBytes,
                                  data,
                                  (BufferUsageHint)bufferUsage);
                    GLHelper.CheckLastError();
                }
            }

            return vbo;
        }
        
        public static VertexBuffer Create(GraphicsDevice graphicsDevice, 
                                          int sizeInBytes,
                                          VertexTypeInfo vertexTypeInfo,
                                          BufferUsage bufferUsage, 
                                          bool clear = true, 
                                          bool dynamic = true,
                                          bool mutable = false)
        {
            var vbo = new VertexBuffer(graphicsDevice, vertexTypeInfo)
            {
                SizeInBytes = sizeInBytes, 
                VertexCount = 0,
                IsDynamic = dynamic,
            };

            if (graphicsDevice.NamedBuffersSupported)
            {
                if (!mutable && graphicsDevice.ImmutableStorageSupported)
                {
                    GL.NamedBufferStorage(vbo.Handle,
                                     vbo.SizeInBytes,
                                     clear ? new byte[sizeInBytes] : null,
                                     vbo.IsDynamic
                                         ? BufferStorageFlags.DynamicStorageBit
                                         : BufferStorageFlags.None);
                    GLHelper.CheckLastError();
                    vbo.IsImmutable = true;
                }
                else
                {
                    GL.NamedBufferData(vbo.Handle,
                                  vbo.SizeInBytes,
                                  clear ? new byte[sizeInBytes] : null,
                                  (BufferUsageHint)bufferUsage);
                    GLHelper.CheckLastError();
                }
            }
            else
            {
                graphicsDevice.BindVertexBuffer(vbo);

                if (!mutable && graphicsDevice.ImmutableStorageSupported)
                {
                    GL.BufferStorage(BufferTarget.ArrayBuffer,
                                     vbo.SizeInBytes,
                                     clear ? new byte[sizeInBytes] : null,
                                     vbo.IsDynamic
                                         ? BufferStorageFlags.DynamicStorageBit
                                         : BufferStorageFlags.None);
                    GLHelper.CheckLastError();
                    vbo.IsImmutable = true;
                }
                else
                {
                    GL.BufferData(BufferTarget.ArrayBuffer,
                                  vbo.SizeInBytes,
                                  clear ? new byte[sizeInBytes] : null,
                                  (BufferUsageHint)bufferUsage);
                    GLHelper.CheckLastError();
                }
            }
            
            return vbo;
        }

        public unsafe void SetData<T>(T[] data)
            where T : unmanaged
        {
            if (data.Length * sizeof(T) > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));
            
            if (!IsDynamic)
                throw new InvalidOperationException("Cannot modify a non-dynamic VertexBuffer.");

            if (IsImmutable)
                throw new InvalidOperationException("Cannot modify an immutable VertexBuffer");
            
            if (GraphicsDevice.NamedBuffersSupported)
            {
                GL.NamedBufferSubData(Handle, IntPtr.Zero, sizeof(T) * data.Length, data);
                GLHelper.CheckLastError();                
            }
            else
            {
                GraphicsDevice.BindVertexBuffer(this);

                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(T) * data.Length, data);
                GLHelper.CheckLastError();
            }

            VertexCount = data.Length;
        }
        
        public unsafe void SetData<T>(T[] data, int sizeInBytes)
            where T : unmanaged
        {
            if (sizeInBytes > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));
            
            if (!IsDynamic)
                throw new InvalidOperationException("Cannot modify a non-dynamic VertexBuffer.");

            if (IsImmutable)
                throw new InvalidOperationException("Cannot modify an immutable VertexBuffer");
            
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
                GraphicsDevice.BindVertexBuffer(this);

                GL.BufferSubData(BufferTarget.ArrayBuffer, 
                                 IntPtr.Zero, 
                                 sizeInBytes, 
                                 data);
                GLHelper.CheckLastError();    
            }

            VertexCount = sizeInBytes / sizeof(T);
        }
        
        public unsafe void SubData<T>(T[] data, IntPtr offset)
            where T : unmanaged
        {
            if (data.Length * sizeof(T) > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));
            
            if (!IsDynamic)
                throw new InvalidOperationException("Cannot modify a non-dynamic VertexBuffer.");

            if (GraphicsDevice.NamedBuffersSupported)
            {
                GL.NamedBufferSubData(Handle, offset, sizeof(T) * data.Length, data);
                GLHelper.CheckLastError();
            }
            else
            {
                GraphicsDevice.BindVertexBuffer(this);

                GL.BufferSubData(BufferTarget.ArrayBuffer, offset, sizeof(T) * data.Length, data);
                GLHelper.CheckLastError();
            }
        }
        
        public unsafe void SubData<T>(T* data, int length, IntPtr offset)
            where T : unmanaged
        {
            if (length * sizeof(T) > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));
            
            if (!IsDynamic)
                throw new InvalidOperationException("Cannot modify a non-dynamic VertexBuffer.");
            
            if (GraphicsDevice.NamedBuffersSupported)
            {
                GL.NamedBufferSubData(Handle, offset, sizeof(T) * length, new IntPtr(data));
                GLHelper.CheckLastError();
            }
            else
            {
                GraphicsDevice.BindVertexBuffer(this);
                
                GL.BufferSubData(BufferTarget.ArrayBuffer, offset, sizeof(T) * length, new IntPtr(data));
                GLHelper.CheckLastError();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SubData<T>(T[] data, int offsetIndex)
            where T : unmanaged
        {
            SubData(data, new IntPtr(offsetIndex * sizeof(T)));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SubData<T>(T* data, int length, int offsetIndex)
            where T : unmanaged
        {
            SubData(data, length, new IntPtr(offsetIndex * sizeof(T)));
        }
        
        public void SetDataPointer(IntPtr data, int dataSizeInBytes)
        {
            if (dataSizeInBytes > SizeInBytes)
                throw new ArgumentException("Data too large", nameof(data));

            if (IsImmutable)
                throw new InvalidOperationException("Cannot modify an immutable VertexBuffer");
            
            if (GraphicsDevice.NamedBuffersSupported)
            {
                GL.NamedBufferData(Handle, (IntPtr)dataSizeInBytes, data, BufferUsageHint.DynamicDraw);
                GLHelper.CheckLastError();    
            }
            else
            {
                GraphicsDevice.BindVertexBuffer(this);
                
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)dataSizeInBytes, data, BufferUsageHint.DynamicDraw);
                GLHelper.CheckLastError();
            }
        }

        public void SubDataPointer(int offsetInBytes, IntPtr data, int dataSizeInBytes)
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
                GraphicsDevice.BindVertexBuffer(this);
                
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offsetInBytes, (IntPtr)dataSizeInBytes, data);
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
            GraphicsDevice.UnbindVertexBuffer(this);

            GL.DeleteBuffer(Handle);
            GLHelper.CheckLastError();
            
            Disposed = true;
        }
    }
}