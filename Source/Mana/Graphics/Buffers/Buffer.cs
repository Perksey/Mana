using System;
using Mana.Logging;
using Mana.Utilities;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    public abstract class Buffer : IGraphicsResource
    {
        private static Logger _log = Logger.Create();

        private string _label;

        public string Label
        {
            get => _label;
            set
            {
                if (GraphicsDevice.IsVersionAtLeast(4, 3) || GraphicsDevice.Extensions.KHR_Debug)
                {
                    GL.ObjectLabel(ObjectLabelIdentifier.Buffer, Handle, value.Length, value);    
                }

                _label = value;
            }
        }
        
        public readonly GLHandle Handle;
        internal readonly BufferTarget BufferTarget;
        
        internal int SizeInBytes = -1;
        internal bool Disposed = false;
        
        internal int Count = -1;
        
        internal bool IsImmutable = false;

        protected Buffer(GraphicsDevice graphicsDevice, BufferTarget bufferTarget)
        {
            GraphicsDevice = graphicsDevice;
            BufferTarget = bufferTarget;
            
            GraphicsDevice.Resources.Add(this);
            
            if (graphicsDevice.DirectStateAccessSupported)
            {
                GL.CreateBuffers(1, out int buffer);
                Handle = (GLHandle)buffer;
            }
            else
            {
                Handle = (GLHandle)GL.GenBuffer();
            }

            Assert.That(Handle != GLHandle.Zero);
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
                                   bool immutable = true)
            where T : unmanaged
        {
            unsafe
            {
                SizeInBytes = data.Length * sizeof(T);    
            }

            Count = data.Length;
            IsImmutable = immutable;
            
            if (GraphicsDevice.DirectStateAccessSupported)
            {
                if (IsImmutable && GraphicsDevice.ImmutableStorageSupported)
                {
                    GL.NamedBufferStorage(Handle,
                                          SizeInBytes,
                                          data,
                                          bufferUsage.GetBufferStorageFlags());
                }
                else
                {
                    GL.NamedBufferData(Handle,
                                       SizeInBytes,
                                       data,
                                       (BufferUsageHint)bufferUsage);
                }
            }
            else
            {
                Bind();

                if (IsImmutable && GraphicsDevice.ImmutableStorageSupported)
                {
                    GL.BufferStorage(BufferTarget,
                                     SizeInBytes,
                                     data,
                                     bufferUsage.GetBufferStorageFlags());
                }
                else
                {
                    GL.BufferData(BufferTarget,
                                  SizeInBytes,
                                  data,
                                  (BufferUsageHint)bufferUsage);
                }
            }
        }

        protected void AllocateEmpty(int sizeInBytes, BufferUsage bufferUsage, bool immutable = true)
        {
            SizeInBytes = sizeInBytes;    
            IsImmutable = immutable;
            
            if (GraphicsDevice.DirectStateAccessSupported)
            {
                if (IsImmutable && GraphicsDevice.ImmutableStorageSupported)
                {
                    GL.NamedBufferStorage(Handle,
                                          SizeInBytes,
                                          IntPtr.Zero,
                                          bufferUsage.GetBufferStorageFlags());
                }
                else
                {
                    GL.NamedBufferData(Handle,
                                       SizeInBytes,
                                       IntPtr.Zero,
                                       (BufferUsageHint)bufferUsage);
                }
            }
            else
            {
                Bind();

                if (IsImmutable && GraphicsDevice.ImmutableStorageSupported)
                {
                    GL.BufferStorage(BufferTarget,
                                     SizeInBytes,
                                     IntPtr.Zero,
                                     bufferUsage.GetBufferStorageFlags());
                }
                else
                {
                    GL.BufferData(BufferTarget,
                                  SizeInBytes,
                                  IntPtr.Zero,
                                  (BufferUsageHint)bufferUsage);
                }
            }
        }

        protected void Allocate<T>(int sizeInBytes,
                                   BufferUsage bufferUsage,
                                   bool immutable = true)
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
            
            if (GraphicsDevice.DirectStateAccessSupported)
            {
                if (IsImmutable && GraphicsDevice.ImmutableStorageSupported)
                {
                    GL.NamedBufferStorage(Handle,
                                          SizeInBytes,
                                          new byte[SizeInBytes],
                                          bufferUsage.GetBufferStorageFlags());
                }
                else
                {
                    GL.NamedBufferData(Handle,
                                       SizeInBytes,
                                       new byte[SizeInBytes],
                                       (BufferUsageHint)bufferUsage);
                }
            }
            else
            {
                Bind();

                if (IsImmutable && GraphicsDevice.ImmutableStorageSupported)
                {
                    GL.BufferStorage(BufferTarget,
                                     SizeInBytes,
                                     new byte[SizeInBytes],
                                     bufferUsage.GetBufferStorageFlags());
                }
                else
                {
                    GL.BufferData(BufferTarget,
                                  SizeInBytes,
                                  new byte[SizeInBytes],
                                  (BufferUsageHint)bufferUsage);
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
            }
            else
            {
                Bind();
                GL.BufferData(BufferTarget, new IntPtr(SizeInBytes), data, (BufferUsageHint)bufferUsage);
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
            }
            else
            {
                Bind();
                GL.BufferData(BufferTarget, new IntPtr(SizeInBytes), data, (BufferUsageHint)bufferUsage);
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
            }
            else
            {
                Bind();
                GL.BufferSubData(BufferTarget, new IntPtr(offset * sizeof(T)), new IntPtr(length * sizeof(T)), data);
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
            }
            else
            {
                Bind();
                GL.BufferSubData(BufferTarget, new IntPtr(offset * sizeof(T)), new IntPtr(length * sizeof(T)), data);
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

            Disposed = true;
        }

        protected abstract void Bind();
        protected abstract void Unbind();
    }
}