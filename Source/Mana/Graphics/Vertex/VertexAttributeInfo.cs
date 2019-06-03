using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Vertex
{
    internal struct VertexAttributeInfo
    {
        public readonly int Size;
        public readonly VertexAttribPointerType Type;
        public readonly int ComponentCount;
        public readonly bool Normalize;

        public VertexAttributeInfo(int size, VertexAttribPointerType type, int componentCount, bool normalize)
        {
            Size = size;
            Type = type;
            ComponentCount = componentCount;
            Normalize = normalize;
        }
    }
}