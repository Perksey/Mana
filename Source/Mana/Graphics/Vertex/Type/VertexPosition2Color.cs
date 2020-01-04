using System.Numerics;
using System.Runtime.InteropServices;

namespace Mana.Graphics.Vertex.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPosition2Color : IVertexType
    {
        public Vector2 Position;
        public Color Color;

        public VertexPosition2Color(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
        }
    }
}
