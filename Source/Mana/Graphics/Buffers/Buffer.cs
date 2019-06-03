using System;
using Mana.Utilities;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    /// <summary>
    /// Represents a buffer graphics resource.
    /// </summary>
    public abstract class Buffer : GraphicsResource
    {
        internal int SizeInBytes = -1;
        
        protected Buffer(ResourceManager resourceManager)
            : base(resourceManager)
        {
            Handle = GLHelper.CreateBuffer();
            
            resourceManager.OnResourceCreated(this);
        }

        /// <summary>
        /// Gets the count of values currently contained within the buffer.
        /// </summary>
        public int Count { get; private set; } = -1;
        
        /// <summary>
        /// Gets a value that indicates whether the buffer set as immutable.
        /// </summary>
        public bool IsImmutable { get; private set; }
        
        internal abstract BufferTarget BufferTarget { get; }
        
        protected override ObjectLabelIdentifier? LabelType => ObjectLabelIdentifier.Buffer;

        /// <summary>
        /// Sets the buffer's internal data using the given <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="renderContext">The <see cref="RenderContext"/> to perform this operation with.</param>
        /// <param name="data">The data to be sent to the buffer.</param>
        /// <param name="bufferUsageHint">The buffer usage hint.</param>
        /// <typeparam name="T">The type of the buffer data.</typeparam>
        public unsafe void SetData<T>(RenderContext renderContext, T[] data, BufferUsageHint bufferUsageHint)
            where T : unmanaged
        {
            if (IsImmutable)
                throw new InvalidOperationException("This operation cannot be performed " +
                                                    "on an immutable Buffer object.");

            SizeInBytes = data.Length * sizeof(T);
            Count = data.Length;

            if (GLInfo.HasDirectStateAccess)
            {
                GL.NamedBufferData(Handle, new IntPtr(SizeInBytes), data, bufferUsageHint);
            }
            else
            {
                Bind(renderContext);
                GL.BufferData(BufferTarget, new IntPtr(SizeInBytes), data, bufferUsageHint);
            }
        }

        /// <summary>
        /// Sets the buffer's internal data using the given <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="renderContext">The <see cref="RenderContext"/> to perform this operation with.</param>
        /// <param name="data">An <see cref="IntPtr"/> to the start of the buffer data in memory.</param>
        /// <param name="length">The length, in elements, to copy from the data pointer.</param>
        /// <param name="bufferUsageHint">The buffer usage hint.</param>
        /// <typeparam name="T">The type of the buffer data.</typeparam>
        public unsafe void SetData<T>(RenderContext renderContext, IntPtr data, int length, BufferUsageHint bufferUsageHint)
            where T : unmanaged
        {
            if (IsImmutable)
                throw new InvalidOperationException("This operation cannot be performed " +
                                                    "on an immutable Buffer object.");
            
            SizeInBytes = length * sizeof(T);
            Count = length;

            if (GLInfo.HasDirectStateAccess)
            {
                GL.NamedBufferData(Handle, new IntPtr(SizeInBytes), data, bufferUsageHint);
            }
            else
            {
                Bind(renderContext);
                GL.BufferData(BufferTarget, new IntPtr(SizeInBytes), data, bufferUsageHint);
            }
        }

        /// <summary>
        /// Sets a subset of the buffer's internal data store using the given <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="renderContext">The <see cref="RenderContext"/> to perform this operation with.</param>
        /// <param name="data">The data to be sent to the buffer.</param>
        /// <param name="offset">The offset, in elements, from the start of the buffer data store.</param>
        /// <param name="length">The length, in elements, to copy from the data array parameter.</param>
        /// <typeparam name="T">The type of the buffer data.</typeparam>
        public unsafe void SubData<T>(RenderContext renderContext, T[] data, int offset, int length)
            where T : unmanaged
        {
            if ((offset + length) * sizeof(T) > SizeInBytes)
                throw new IndexOutOfRangeException();

            if (GLInfo.HasDirectStateAccess)
            {
                GL.NamedBufferSubData(Handle, new IntPtr(offset * sizeof(T)), new IntPtr(length * sizeof(T)), data);
            }
            else
            {
                Bind(renderContext);
                GL.BufferSubData(BufferTarget, new IntPtr(offset * sizeof(T)), new IntPtr(length * sizeof(T)), data);
            }
        }
        
        /// <summary>
        /// Sets a subset of the buffer's internal data store using the given <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="renderContext">The <see cref="RenderContext"/> to perform this operation with.</param>
        /// <param name="data">The data to be sent to the buffer.</param>
        /// <param name="offset">The offset, in elements, from the start of the buffer data store.</param>
        /// <typeparam name="T">The type of the buffer data.</typeparam>
        public unsafe void SubData<T>(RenderContext renderContext, T[] data, int offset)
            where T : unmanaged
        {
            if ((offset + data.Length) * sizeof(T) > SizeInBytes)
                throw new IndexOutOfRangeException();

            if (GLInfo.HasDirectStateAccess)
            {
                GL.NamedBufferSubData(Handle, new IntPtr(offset * sizeof(T)), new IntPtr(data.Length * sizeof(T)), data);
            }
            else
            {
                Bind(renderContext);
                GL.BufferSubData(BufferTarget, new IntPtr(offset * sizeof(T)), new IntPtr(data.Length * sizeof(T)), data);
            }
        }
        
        /// <summary>
        /// Sets a subset of the buffer's internal data store using the given <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="renderContext">The <see cref="RenderContext"/> to perform this operation with.</param>
        /// <param name="data">An <see cref="IntPtr"/> to the start of the buffer data in memory.</param>
        /// <param name="offset">The offset, in elements, from the start of the buffer data store.</param>
        /// <param name="length">The length, in elements, to copy from the data pointer.</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public unsafe void SubData<T>(RenderContext renderContext, IntPtr data, int offset, int length)
            where T : unmanaged
        {
            if ((offset + length) * sizeof(T) > SizeInBytes)
                throw new IndexOutOfRangeException();

            if (GLInfo.HasDirectStateAccess)
            {
                GL.NamedBufferSubData(Handle, new IntPtr(offset * sizeof(T)), new IntPtr(length * sizeof(T)), data);
            }
            else
            {
                Bind(renderContext);
                GL.BufferSubData(BufferTarget, new IntPtr(offset * sizeof(T)), new IntPtr(length * sizeof(T)), data);
            }
        }
        
        protected unsafe void Allocate<T>(RenderContext renderContext,
                                          T[] data,
                                          BufferUsageHint bufferUsageHint,
                                          bool immutable = true,
                                          BufferStorageFlags extraFlags = BufferStorageFlags.None)
            where T : unmanaged
        {
            if (renderContext == null)
                throw new ArgumentNullException(nameof(renderContext));
            
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            
            SizeInBytes = data.Length * sizeof(T);
            Count = data.Length;
            IsImmutable = immutable;
            
            InitializeBuffer(renderContext, data, bufferUsageHint, extraFlags);
        }

        protected unsafe void Allocate<T>(RenderContext renderContext,
                                          int sizeInBytes,
                                          BufferUsageHint bufferUsageHint,
                                          bool immutable = true,
                                          BufferStorageFlags extraFlags = BufferStorageFlags.None)
            where T : unmanaged
        {
            if (renderContext == null)
                throw new ArgumentNullException(nameof(renderContext));

            SizeInBytes = sizeInBytes;
            IsImmutable = immutable;
            
            float capacity = sizeInBytes / (float)sizeof(T);
            Assert.That(Math.Abs(capacity - (int)capacity) < float.Epsilon);
            Count = (int)capacity;

            InitializeBuffer<T>(renderContext, null, bufferUsageHint, extraFlags);
        }
        
        protected abstract void Bind(RenderContext renderContext);
        protected abstract void Unbind(RenderContext renderContext);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (BoundContext != null)
            {
                Unbind(BoundContext);
                BoundContext = null;
            }

            GL.DeleteBuffer(Handle);
        }

        private void InitializeBuffer<T>(RenderContext renderContext,
                                         T[] data,
                                         BufferUsageHint bufferUsageHint,
                                         BufferStorageFlags storageFlags)
            where T : unmanaged
        {
            if (GLInfo.HasDirectStateAccess)
            {
                if (IsImmutable && GLInfo.HasBufferStorage)
                {
                    GL.NamedBufferStorage(Handle,
                                          SizeInBytes,
                                          data,
                                          // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                                          BufferUsageToStorageFlags(bufferUsageHint) | storageFlags);    
                }
                else
                {
                    GL.NamedBufferData(Handle,
                                       SizeInBytes,
                                       data,
                                       bufferUsageHint);
                }
            }
            else
            {
                Bind(renderContext);

                if (IsImmutable && GLInfo.HasBufferStorage)
                {
                    GL.BufferStorage(BufferTarget,
                                     SizeInBytes,
                                     data,
                                     // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                                     BufferUsageToStorageFlags(bufferUsageHint) | storageFlags);
                }
                else
                {
                    GL.BufferData(BufferTarget,
                                  SizeInBytes,
                                  data,
                                  bufferUsageHint);
                }
            }
        }
        
        private BufferStorageFlags BufferUsageToStorageFlags(BufferUsageHint hint)
        {
            switch (hint)
            {
                case BufferUsageHint.StaticCopy:
                case BufferUsageHint.StaticRead:
                case BufferUsageHint.StaticDraw:
                    return BufferStorageFlags.None;
                default:
                    return BufferStorageFlags.DynamicStorageBit;
            }
        }
    }
}