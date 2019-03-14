using System.Numerics;
using System.Runtime.InteropServices;

namespace Mana.Graphics.Vertex.Types
{
    [VertexType]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPosition
    {
        public Vector3 Position;

        public VertexPosition(Vector3 position)
        {
            Position = position;
        }
    }
}