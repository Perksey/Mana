using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    public class IndexBuffer : Buffer
    {
        internal DrawElementsType DataType;
        internal int DataTypeSizeInBytes;
        
        private IndexBuffer(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, BufferTarget.ElementArrayBuffer)
        {
        }

        public static unsafe IndexBuffer Create<T>(GraphicsDevice graphicsDevice,
                                            T[] data,
                                            BufferUsage bufferUsage,
                                            bool immutable)
            where T : unmanaged
        {
            if (!IsElementType<T>())
                throw new ArgumentException("Invalid datatype", nameof(T));

            var ibo = new IndexBuffer(graphicsDevice)
            {
                DataType = GetDrawElementsType<T>(), 
                DataTypeSizeInBytes = sizeof(T)
            };

            ibo.Allocate<T>(data, bufferUsage, immutable);
            return ibo;
        }

        public static unsafe IndexBuffer Create<T>(GraphicsDevice graphicsDevice,
                                            int capacity,
                                            BufferUsage bufferUsage,
                                            bool immutable)
            where T : unmanaged
        {
            if (!IsElementType<T>())
                throw new ArgumentException("Invalid datatype", nameof(T));
            
            var ibo = new IndexBuffer(graphicsDevice)
            {
                DataType = GetDrawElementsType<T>(), 
                DataTypeSizeInBytes = sizeof(T)
            };
            
            ibo.Allocate<T>(capacity * sizeof(T), bufferUsage, immutable);
            return ibo;
        }

        private static bool IsElementType<T>()
            where T : unmanaged
        {
            return typeof(T) == typeof(ushort) ||
                   typeof(T) == typeof(byte) ||
                   typeof(T) == typeof(uint);
        }

        private static DrawElementsType GetDrawElementsType<T>()
            where T : unmanaged
        {
            if (typeof(T) == typeof(ushort))
                return DrawElementsType.UnsignedShort;

            if (typeof(T) == typeof(byte))
                return DrawElementsType.UnsignedByte;

            if (typeof(T) == typeof(uint))
                return DrawElementsType.UnsignedInt;

            throw new ArgumentException("Invalid data type.");
        }

        protected override void Bind()
        {
            GraphicsDevice.BindIndexBuffer(this);
        }

        protected override void Unbind()
        {
            GraphicsDevice.UnbindIndexBuffer(this);
        }
    }
}