using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Vertex
{
    internal static class VertexHelper
    {
        internal static readonly Dictionary<Type, VertexAttributeInfo> vertexAttributeInfoByType;

        static VertexHelper()
        {
            void Declare<T>(VertexAttribPointerType type, int count, int size, bool normalize)
            {
                vertexAttributeInfoByType.Add(typeof(T), new VertexAttributeInfo(size, type, count, normalize));
            }

            vertexAttributeInfoByType = new Dictionary<Type, VertexAttributeInfo>();

            Declare<sbyte>(VertexAttribPointerType.Byte, 1, 1, false);
            Declare<byte>(VertexAttribPointerType.UnsignedByte, 1, 1, false);
            Declare<short>(VertexAttribPointerType.Short, 1, 2, false);
            Declare<ushort>(VertexAttribPointerType.UnsignedShort, 1, 2, false);
            Declare<int>(VertexAttribPointerType.Int, 1, 4, false);
            Declare<uint>(VertexAttribPointerType.UnsignedInt, 1, 4, false);
            Declare<float>(VertexAttribPointerType.Float, 1, 4, false);
            Declare<Color>(VertexAttribPointerType.UnsignedByte, 4, 1, true);
            Declare<Vector2>(VertexAttribPointerType.Float, 2, 4, false);
            Declare<Vector3>(VertexAttribPointerType.Float, 3, 4, false);
            Declare<Vector4>(VertexAttribPointerType.Float, 4, 4, false);
        }

        internal static VertexAttributeInfo GetVertexAttributeInfo(Type fieldType)
        {
            if (vertexAttributeInfoByType.TryGetValue(fieldType, out VertexAttributeInfo info))
            {
                return info;
            }

            throw new InvalidOperationException("Vertex Attribute Type not supported: " + fieldType.FullName);
        }
    }
}