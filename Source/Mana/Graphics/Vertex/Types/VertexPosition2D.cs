using System.Numerics;
using System.Runtime.InteropServices;

namespace Mana.Graphics.Vertex.Types
{
    [VertexType]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPosition2D
    {
        public Vector2 Position;

        public VertexPosition2D(Vector2 position)
        {
            Position = position;
        }
    }
}