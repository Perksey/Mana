using System;
using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics.Buffers
{
    /// <summary>
    /// Represents an OpenGL element buffer object.
    /// </summary>
    public class IndexBuffer : Buffer
    {
        internal DrawElementsType DrawElementsType;

        private IndexBuffer(RenderContext parentContext)
            : base(parentContext)
        {
        }

        internal override BufferTarget BufferTarget => BufferTarget.ElementArrayBuffer;

        public static IndexBuffer Create<T>(RenderContext renderContext,
                                            T[] data,
                                            BufferUsageHint bufferUsageHint,
                                            bool immutable)
            where T : unmanaged
        {
            if (!IsElementType<T>())
                throw new ArgumentException("Invalid datatype.", nameof(T));

            var ibo = new IndexBuffer(renderContext)
            {
                DrawElementsType = GetDrawElementsType<T>(),
            };

            ibo.Allocate<T>(data, bufferUsageHint, immutable);
            return ibo;
        }

        public static unsafe IndexBuffer Create<T>(RenderContext renderContext,
                                                   int capacity,
                                                   BufferUsageHint bufferUsageHint,
                                                   bool immutable)
            where T : unmanaged
        {
            if (renderContext == null)
                throw new ArgumentNullException(nameof(renderContext));

            if (!IsElementType<T>())
                throw new ArgumentException("Invalid datatype.", nameof(T));

            var ibo = new IndexBuffer(renderContext)
            {
                DrawElementsType = GetDrawElementsType<T>(),
            };

            ibo.Allocate<T>(capacity * sizeof(T), bufferUsageHint, immutable);
            return ibo;
        }

        public override void Bind(RenderContext renderContext) => renderContext.BindIndexBuffer(this);

        public override void Unbind(RenderContext renderContext) => renderContext.UnbindIndexBuffer(this);

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
    }
}
