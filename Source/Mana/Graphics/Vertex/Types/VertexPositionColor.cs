using System.Numerics;
using System.Runtime.InteropServices;

namespace Mana.Graphics.Vertex.Types
{
    [VertexType]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColor
    {
        public Vector3 Position;
        public Color Color;

        public VertexPositionColor(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
        }
    }
}
