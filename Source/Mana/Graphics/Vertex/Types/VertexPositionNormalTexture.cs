using System.Numerics;
using System.Runtime.InteropServices;

namespace Mana.Graphics.Vertex.Types
{
    [VertexType]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormalTexture
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;

        public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 texCoord) : this()
        {
            Position = position;
            Normal = normal;
            TexCoord = texCoord;
        }

        public static explicit operator VertexPositionNormal(VertexPositionNormalTexture vertex)
        {
            return new VertexPositionNormal(vertex.Position, vertex.Normal);
        }
    }
}
