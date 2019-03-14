using System.Numerics;
using System.Runtime.InteropServices;

namespace Mana.Graphics.Vertex.Types
{
    [VertexType]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPosition2DNormal
    {
        public Vector2 Position;
        public Vector3 Normal;

        public VertexPosition2DNormal(Vector2 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }
    }
}