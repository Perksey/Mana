using System.Numerics;
using System.Runtime.InteropServices;

namespace Mana.Graphics.Vertex.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPosition2TextureColor : IVertexType
    {
        public Vector2 Position;
        public Vector2 TexCoord;
        public Color Color;

        public VertexPosition2TextureColor(Vector2 position, Vector2 texCoord, Color color)
        {
            Position = position;
            TexCoord = texCoord;
            Color = color;
        }
    }
}
