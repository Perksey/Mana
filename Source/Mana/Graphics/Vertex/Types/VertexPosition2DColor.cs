using System.Numerics;
using System.Runtime.InteropServices;

namespace Mana.Graphics.Vertex.Types
{
    [VertexType]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPosition2DColor
    {
        public Vector2 Position;
        public Color Color;

        public VertexPosition2DColor(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
        }
    }
}
