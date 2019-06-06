using System.Numerics;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shader;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Geometry
{
    public class Mesh : GraphicsResource
    {
        public readonly MeshData MeshData;
        public MeshTextures Textures = new MeshTextures();
        
        public Matrix4x4 Transform = Matrix4x4.Identity;

        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;
        
        public Mesh(RenderContext renderContext, MeshData meshData)
            : base(renderContext.ResourceManager)
        {
            MeshData = meshData;
            
            VertexBuffer = VertexBuffer.Create(renderContext, meshData.Vertices, BufferUsageHint.StaticDraw, true);
            IndexBuffer = IndexBuffer.Create(renderContext, meshData.Indices, BufferUsageHint.StaticDraw, true);
        }
        
        public void Render(RenderContext renderContext, ShaderProgram shaderProgram)
        {
            shaderProgram.TrySetUniform("transform", ref Transform);

            Textures.Apply(renderContext, shaderProgram);
            
            renderContext.Render(VertexBuffer, IndexBuffer, shaderProgram);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            VertexBuffer?.Dispose();
            IndexBuffer?.Dispose();
        }

        /// <summary>
        /// Updates the Mesh's <see cref="VertexBuffer"/> and <see cref="IndexBuffer"/> with any changes to the Mesh's
        /// MeshData object.
        /// </summary>
        public void UpdateBuffers(RenderContext renderContext)
        {
            if (VertexBuffer.Count == MeshData.Vertices.Length)
            {
                VertexBuffer.SubData(renderContext, MeshData.Vertices, 0, MeshData.Vertices.Length);
            }
            else
            {
                VertexBuffer.Dispose();
                VertexBuffer = VertexBuffer.Create(renderContext, MeshData.Vertices, BufferUsageHint.StaticDraw, true);
            }
            
            if (IndexBuffer.Count == MeshData.Indices.Length)
            {
                IndexBuffer.SubData(renderContext, MeshData.Indices, 0, MeshData.Indices.Length);
            }
            else
            {
                IndexBuffer.Dispose();
                IndexBuffer = IndexBuffer.Create(renderContext, MeshData.Indices, BufferUsageHint.StaticDraw, true);
            }
        }
    }
}