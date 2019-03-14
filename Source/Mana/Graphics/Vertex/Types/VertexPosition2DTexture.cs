using System.Numerics;
using System.Runtime.InteropServices;

namespace Mana.Graphics.Vertex.Types
{
    [VertexType]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPosition2DTexture
    {
        public Vector2 Position;
        public Vector2 TexCoord;

        public VertexPosition2DTexture(Vector2 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }
    }
}
