using System.Numerics;
using System.Runtime.InteropServices;

namespace Mana.Graphics.Vertex.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPosition2 : IVertexType
    {
        public Vector2 Position;

        public VertexPosition2(Vector2 position)
        {
            Position = position;
        }
    }
}
