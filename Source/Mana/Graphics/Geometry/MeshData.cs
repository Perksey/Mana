using Mana.Graphics.Vertex.Types;

namespace Mana.Graphics.Geometry
{
    public class MeshData : MeshData<VertexPositionNormalTexture, uint>
    {
        public MeshData(VertexPositionNormalTexture[] vertices, uint[] indices)
            : base(vertices, indices)
        {
        }
    }
    
    public class MeshData<TVertex, TIndex>
        where TVertex : unmanaged
        where TIndex : unmanaged
    {
        public readonly TVertex[] Vertices;
        public readonly TIndex[] Indices;

        public MeshInfo MeshInfo { get; set; } = null;
        
        public MeshData(TVertex[] vertices, TIndex[] indices)
        {
            Vertices = vertices;
            Indices = indices;
        }
    }
}