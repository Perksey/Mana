using System.Numerics;
using System.Runtime.InteropServices;

namespace Mana.Graphics.Vertex.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPosition : IVertexType
    {
        public Vector3 Position;

        public VertexPosition(Vector3 position)
        {
            Position = position;
        }
    }
}
