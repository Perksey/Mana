using System;
using System.Diagnostics.CodeAnalysis;
using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    /// <summary>
    /// Represents a buffer graphics resource.
    /// </summary>
    public abstract class Buffer : GraphicsResource
    {
        /// <summary>
        /// Gets the size, in bytes, of the Buffer's internal data store.
        /// </summary>
        internal int SizeInBytes = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer"/> class.
        /// </summary>
        /// <param name="parentContext">The <see cref="RenderContext"/> the Buffer will be using.</param>
        protected Buffer(RenderContext parentContext)
            : base(parentContext)
        {
            Handle = GLHelper.CreateBuffer();
        }

        /// <summary>
        /// Gets the number of objects currently contained within the buffer.
        /// </summary>
        public int Count { get; private set; } = -1;

        /// <summary>
        /// Gets a value indicating whether whether the buffer's data store is immutable.
        /// </summary>
        public bool IsImmutable { get; private set; }

        /// <summary>
        /// Gets the OpenGL BufferTarget value associated with this Buffer.
        /// </summary>
        internal abstract BufferTarget BufferTarget { get; }

        /// <summary>
        /// Gets the OpenGL ObjectLabelIdentifier value associated with this Buffer.
        /// </summary>
        protected override ObjectLabelIdentifier? LabelType => ObjectLabelIdentifier.Buffer;

        /// <summary>
        /// Sends the given array of data to the Buffer's internal data store.
        /// </summary>
        /// <param name="data">The data to be sent to the buffer.</param>
        /// <param name="bufferUsageHint">The buffer usage hint.</param>
        /// <typeparam name="T">The type of the buffer data.</typeparam>
        public unsafe void SetData<T>(T[] data, BufferUsageHint bufferUsageHint)
            where T : unmanaged
        {
            if (IsImmutable)
            {
                throw new InvalidOperationException("This operation cannot be performed " +
                                                    "on an immutable Buffer object.");
            }

            SizeInBytes = data.Length * sizeof(T);
            Count = data.Length;

            if (GLInfo.HasDirectStateAccess)
            {
                GL.NamedBufferData(Handle, SizeInBytes, data, bufferUsageHint);
            }
            else
            {
                Bind(ParentContext);
                GL.BufferData(BufferTarget, SizeInBytes, data, bufferUsageHint);
            }
        }

        /// <summary>
        /// Sends the given span of data to the Buffer's internal data store.
        /// </summary>
        /// <param name="data">A <see cref="Span&lt;T&gt;"/> over the source data in memory.</param>
        /// <param name="bufferUsageHint">The buffer usage hint.</param>
        /// <typeparam name="T">The type of the buffer data.</typeparam>
        public unsafe void SetData<T>(Span<T> data, BufferUsageHint bufferUsageHint)
            where T : unmanaged
        {
            if (IsImmutable)
            {
                throw new InvalidOperationException("This operation cannot be performed " +
                                                    "on an immutable Buffer object.");
            }

            SizeInBytes = data.Length * sizeof(T);
            Count = data.Length;

            fixed (void* dataPtr = &data.GetPinnableReference())
            {
                if (GLInfo.HasDirectStateAccess)
                {
                    GL.NamedBufferData(Handle, SizeInBytes, new IntPtr(dataPtr), bufferUsageHint);
                }
                else
                {
                    Bind(ParentContext);
                    GL.BufferData(BufferTarget, SizeInBytes, new IntPtr(dataPtr), bufferUsageHint);
                }
            }
        }

        /// <summary>
        /// Sets a subset of the buffer's internal data store.
        /// </summary>
        /// <param name="data">The data to be sent to the buffer.</param>
        /// <param name="offset">The offset, in elements, from the start of the buffer data store.</param>
        /// <param name="length">The length, in elements, to copy from the data array parameter.</param>
        /// <typeparam name="T">The type of the buffer data.</typeparam>
        public unsafe void SubData<T>(T[] data, int offset, int length)
            where T : unmanaged
        {
            if ((offset + length) * sizeof(T) > SizeInBytes)
            {
                throw new IndexOutOfRangeException();
            }

            if (GLInfo.HasDirectStateAccess)
            {
                GL.NamedBufferSubData(Handle, new IntPtr(offset * sizeof(T)), length * sizeof(T), data);
            }
            else
            {
                Bind(ParentContext);
                GL.BufferSubData(BufferTarget, new IntPtr(offset * sizeof(T)), length * sizeof(T), data);
            }
        }

        /// <summary>
        /// Sets a subset of the buffer's internal data store.
        /// </summary>
        /// <param name="data">The data to be sent to the buffer.</param>
        /// <param name="offset">The offset, in elements, from the start of the buffer data store.</param>
        /// <typeparam name="T">The type of the buffer data.</typeparam>
        public unsafe void SubData<T>(T[] data, int offset)
            where T : unmanaged
        {
            if ((offset + data.Length) * sizeof(T) > SizeInBytes)
            {
                throw new IndexOutOfRangeException();
            }

            if (GLInfo.HasDirectStateAccess)
            {
                GL.NamedBufferSubData(Handle, new IntPtr(offset * sizeof(T)), data.Length * sizeof(T), data);
            }
            else
            {
                Bind(ParentContext);
                GL.BufferSubData(BufferTarget, new IntPtr(offset * sizeof(T)), data.Length * sizeof(T), data);
            }
        }

        /// <summary>
        /// Sets a subset of the buffer's internal data store.
        /// </summary>
        /// <param name="data">An <see cref="Span&lt;T&gt;"/> over the source data in memory.</param>
        /// <param name="offset">The offset, in elements, from the start of the buffer data store.</param>
        /// <typeparam name="T">The type of the buffer data.</typeparam>
        public unsafe void SubData<T>(Span<T> data, int offset)
            where T : unmanaged
        {
            if ((offset + data.Length) * sizeof(T) > SizeInBytes)
            {
                throw new IndexOutOfRangeException();
            }

            fixed (void* dataPtr = &data.GetPinnableReference())
            {
                if (GLInfo.HasDirectStateAccess)
                {
                    GL.NamedBufferSubData(Handle,
                                          new IntPtr(offset * sizeof(T)),
                                          data.Length * sizeof(T),
                                          new IntPtr(dataPtr));
                }
                else
                {
                    Bind(ParentContext);
                    GL.BufferSubData(BufferTarget,
                                     new IntPtr(offset * sizeof(T)),
                                     data.Length * sizeof(T),
                                     new IntPtr(dataPtr));
                }
            }
        }

        /// <summary>
        /// Sets a subset of the buffer's internal data store using the given <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="data">An <see cref="IntPtr"/> to the start of the buffer data in memory.</param>
        /// <param name="offset">The offset, in elements, from the start of the buffer data store.</param>
        /// <param name="length">The length, in elements, to copy from the data pointer.</param>
        /// <typeparam name="T">The type of the buffer data.</typeparam>
        public unsafe void SubData<T>(IntPtr data, int offset, int length)
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
                Bind(ParentContext);
                GL.BufferSubData(BufferTarget, new IntPtr(offset * sizeof(T)), new IntPtr(length * sizeof(T)), data);
            }
        }

        protected unsafe void Allocate<T>(T[] data,
                                          BufferUsageHint bufferUsageHint,
                                          bool immutable = true,
                                          BufferStorageFlags extraFlags = BufferStorageFlags.None)
            where T : unmanaged
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            SizeInBytes = data.Length * sizeof(T);
            Count = data.Length;
            IsImmutable = immutable;

            InitializeBuffer(data, bufferUsageHint, extraFlags);
        }

        protected unsafe void Allocate<T>(int sizeInBytes,
                                          BufferUsageHint bufferUsageHint,
                                          bool immutable = true,
                                          BufferStorageFlags extraFlags = BufferStorageFlags.None)
            where T : unmanaged
        {
            SizeInBytes = sizeInBytes;
            IsImmutable = immutable;

            float capacity = sizeInBytes / (float)sizeof(T);

            if (!(Math.Abs(capacity - (int)capacity) < float.Epsilon))
            {
                throw new ArgumentOutOfRangeException(nameof(sizeInBytes));
            }

            Count = (int)capacity;

            InitializeBuffer<T>(null, bufferUsageHint, extraFlags);
        }

        public abstract void Bind(RenderContext renderContext);

        public abstract void Unbind(RenderContext renderContext);

        protected override void Dispose(bool disposing)
        {
            EnsureUndisposed();

            if (BoundContext != null)
            {
                Unbind(BoundContext);
                BoundContext = null;
            }

            GL.DeleteBuffer(Handle);
        }


        [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
        private void InitializeBuffer<T>(T[] data,
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
                                          BufferUsageToStorageFlags(bufferUsageHint) | storageFlags);
                }
                else
                {
                    GL.NamedBufferData(Handle, SizeInBytes, data, bufferUsageHint);
                }
            }
            else
            {
                Bind(ParentContext);

                if (IsImmutable && GLInfo.HasBufferStorage)
                {
                    GL.BufferStorage(BufferTarget,
                                     SizeInBytes,
                                     data,
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
