using System;
using Mana.Logging;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    public abstract class Buffer : IGraphicsResource
    {
        private static Logger _log = Logger.Create();
        
        public readonly GLHandle Handle;
        internal readonly BufferTarget BufferTarget;
        
        internal int SizeInBytes = -1;
        internal bool Disposed = false;
        
        internal int Count = -1;
        
        internal bool IsImmutable = false;
        internal bool IsDynamic = false;

        protected Buffer(GraphicsDevice graphicsDevice, BufferTarget bufferTarget)
        {
            GraphicsDevice = graphicsDevice;
            BufferTarget = bufferTarget;
            
            GraphicsDevice.Resources.Add(this);
            
            if (graphicsDevice.DirectStateAccessSupported)
            {
                GL.CreateBuffers(1, out int buffer);
                GLHelper.CheckLastError();

                Handle = (GLHandle)buffer;
            }
            else
            {
                Handle = (GLHandle)GL.GenBuffer();
                GLHelper.CheckLastError();
            }
        }

#if DEBUG
        ~Buffer()
        {
            _log.Error("Buffer leaked: " + GetType().Name);
        }
#endif

        public GraphicsDevice GraphicsDevice { get; }

        #region Create methods

        protected void Allocate<T>(T[] data,
                                   BufferUsage bufferUsage,
                                   bool immutable = true,
                                   bool dynamic = true)
            where T : unmanaged
        {
            unsafe
            {
                SizeInBytes = data.Length * sizeof(T);    
            }

            Count = data.Length;
            IsImmutable = immutable;
            IsDynamic = dynamic;
            
            if (GraphicsDevice.DirectStateAccessSupported)
            {
                if (IsImmutable && GraphicsDevice.ImmutableStorageSupported)
                {
                    GL.NamedBufferStorage(Handle,
                                          SizeInBytes,
                                          data,
                                          IsDynamic
                                            ? BufferStorageFlags.DynamicStorageBit
                                            : BufferStorageFlags.None);
                    GLHelper.CheckLastError();
                }
                else
                {
                    GL.NamedBufferData(Handle,
                                       SizeInBytes,
                                       data,
                                       (BufferUsageHint)bufferUsage);
                    GLHelper.CheckLastError();
                }
            }
            else
            {
                Bind();

                if (IsImmutable && GraphicsDevice.ImmutableStorageSupported)
                {
                    GL.BufferStorage(BufferTarget.ArrayBuffer,
                                     SizeInBytes,
                                     data,
                                     IsDynamic
                                         ? BufferStorageFlags.DynamicStorageBit
                                         : BufferStorageFlags.None);
                    GLHelper.CheckLastError();
                }
                else
                {
                    GL.BufferData(BufferTarget.ArrayBuffer,
                                  SizeInBytes,
                                  data,
                                  (BufferUsageHint)bufferUsage);
                    GLHelper.CheckLastError();
                }
            }
        }

        protected void Allocate<T>(int sizeInBytes,
                                   BufferUsage bufferUsage,
                                   bool immutable = true,
                                   bool dynamic = true)
            where T : unmanaged
        {
            unsafe
            {
                SizeInBytes = sizeInBytes;
                
                if (sizeInBytes % sizeof(T) != 0)
                    throw new ArgumentException("Invalid value for parameter: SizeInBytes. Must be a multiple of sizeof(T)");
                
                Count = sizeInBytes / sizeof(T);
            }
            
            IsImmutable = immutable;
            IsDynamic = dynamic;
            
            if (GraphicsDevice.DirectStateAccessSupported)
            {
                if (IsImmutable && GraphicsDevice.ImmutableStorageSupported)
                {
                    GL.NamedBufferStorage(Handle,
                                          SizeInBytes,
                                          new byte[SizeInBytes],
                                          IsDynamic
                                            ? BufferStorageFlags.DynamicStorageBit
                                            : BufferStorageFlags.None);
                    GLHelper.CheckLastError();
                }
                else
                {
                    GL.NamedBufferData(Handle,
                                       SizeInBytes,
                                       new byte[SizeInBytes],
                                       (BufferUsageHint)bufferUsage);
                    GLHelper.CheckLastError();
                }
            }
            else
            {
                Bind();

                if (IsImmutable && GraphicsDevice.ImmutableStorageSupported)
                {
                    GL.BufferStorage(BufferTarget.ArrayBuffer,
                                     SizeInBytes,
                                     new byte[SizeInBytes],
                                     IsDynamic
                                         ? BufferStorageFlags.DynamicStorageBit
                                         : BufferStorageFlags.None);
                    GLHelper.CheckLastError();
                }
                else
                {
                    GL.BufferData(BufferTarget.ArrayBuffer,
                                  SizeInBytes,
                                  new byte[SizeInBytes],
                                  (BufferUsageHint)bufferUsage);
                    GLHelper.CheckLastError();
                }
            }
        }

        
        #endregion
        
        #region SetData methods

        public unsafe void SetData<T>(T[] data, BufferUsage bufferUsage)
            where T : unmanaged
        {
            if (IsImmutable)
                throw new InvalidOperationException("Cannot call SetData() for a Buffer marked as Immutable.");

            SizeInBytes = data.Length * sizeof(T);
            Count = data.Length;
            
            if (GraphicsDevice.DirectStateAccessSupported)
            {
                GL.NamedBufferData(Handle, new IntPtr(SizeInBytes), data, (BufferUsageHint)bufferUsage);
                GLHelper.CheckLastError();
            }
            else
            {
                Bind();
                GL.BufferData(BufferTarget, new IntPtr(SizeInBytes), data, (BufferUsageHint)bufferUsage);
                GLHelper.CheckLastError();
            }
        }
        
        public unsafe void SetData<T>(IntPtr data, int count, BufferUsage bufferUsage)
            where T : unmanaged
        {
            if (IsImmutable)
                throw new InvalidOperationException("Cannot call SetData() for a Buffer marked as Immutable.");

            SizeInBytes = count * sizeof(T);
            Count = count;
            
            if (GraphicsDevice.DirectStateAccessSupported)
            {
                GL.NamedBufferData(Handle, new IntPtr(SizeInBytes), data, (BufferUsageHint)bufferUsage);
                GLHelper.CheckLastError();
            }
            else
            {
                Bind();
                GL.BufferData(BufferTarget, new IntPtr(SizeInBytes), data, (BufferUsageHint)bufferUsage);
                GLHelper.CheckLastError();
            }
        }
        
        #endregion
        
        #region SubData methods

        public unsafe void SubData<T>(T[] data, int offset, int length)
            where T : unmanaged
        {
            if ((offset + length) * sizeof(T) > SizeInBytes)
                throw new ArgumentException("Data too large for the Buffer.");

            if (GraphicsDevice.DirectStateAccessSupported)
            {
                GL.NamedBufferSubData(Handle, new IntPtr(offset * sizeof(T)), new IntPtr(length * sizeof(T)), data);
                GLHelper.CheckLastError();
            }
            else
            {
                Bind();
                GL.BufferSubData(BufferTarget, new IntPtr(offset * sizeof(T)), new IntPtr(length * sizeof(T)), data);
                GLHelper.CheckLastError();
            }
        }

        public unsafe void SubData<T>(IntPtr data, int offset, int length)
            where T : unmanaged
        {
            if ((offset + length) * sizeof(T) > SizeInBytes)
                throw new ArgumentException("Data too large for the Buffer.");

            if (GraphicsDevice.DirectStateAccessSupported)
            {
                GL.NamedBufferSubData(Handle, new IntPtr(offset * sizeof(T)), new IntPtr(length * sizeof(T)), data);
                GLHelper.CheckLastError();
            }
            else
            {
                Bind();
                GL.BufferSubData(BufferTarget, new IntPtr(offset * sizeof(T)), new IntPtr(length * sizeof(T)), data);
                GLHelper.CheckLastError();
            }
        }
        
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            GraphicsDevice.Resources.Remove(this);
            Unbind();
            
            GL.DeleteBuffer(Handle);
            GLHelper.CheckLastError();

            Disposed = true;
        }

        protected abstract void Bind();
        protected abstract void Unbind();
    }
}