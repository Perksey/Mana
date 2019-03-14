using System.Numerics;
using System.Runtime.InteropServices;

namespace Mana.Graphics.Vertex.Types
{
    [VertexType]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionTexture
    {
        public Vector3 Position;
        public Vector2 TexCoord;

        public VertexPositionTexture(Vector3 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }
    }
}
