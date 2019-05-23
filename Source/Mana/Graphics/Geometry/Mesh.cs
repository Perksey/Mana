using System.Numerics;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shaders;

namespace Mana.Graphics.Geometry
{
    public class Mesh : IGraphicsResource
    {
        public readonly MeshData MeshData;
        public MeshTextures Textures = new MeshTextures();
        
        public Matrix4x4 Transform = Matrix4x4.Identity;

        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;
        
        public Mesh(GraphicsDevice graphicsDevice, MeshData meshData)
        {
            GraphicsDevice = graphicsDevice;
            MeshData = meshData;
            
            GraphicsDevice.Resources.Add(this);

            VertexBuffer = VertexBuffer.Create(graphicsDevice, meshData.Vertices, BufferUsage.StaticDraw, true);
            IndexBuffer = IndexBuffer.Create(graphicsDevice, meshData.Indices, BufferUsage.StaticDraw, true);
        }
        
        public GraphicsDevice GraphicsDevice { get; }

        public void Render(ShaderProgram shaderProgram)
        {
            shaderProgram.TrySetUniform("transform", ref Transform);

            Textures.Apply(GraphicsDevice, shaderProgram);
            
            GraphicsDevice.Render(VertexBuffer, IndexBuffer, shaderProgram);
        }
        
        public void Dispose()
        {
            GraphicsDevice.Resources.Remove(this);
            
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }

        /// <summary>
        /// Updates the Mesh's <see cref="VertexBuffer"/> and <see cref="IndexBuffer"/> with any changes to the Mesh's
        /// MeshData object.
        /// </summary>
        public void UpdateBuffers()
        {
            if (VertexBuffer.Count == MeshData.Vertices.Length)
            {
                VertexBuffer.SubData(MeshData.Vertices, 0, MeshData.Vertices.Length);
            }
            else
            {
                VertexBuffer.Dispose();
                VertexBuffer = VertexBuffer.Create(GraphicsDevice, MeshData.Vertices, BufferUsage.StaticDraw, true);
            }
            
            if (IndexBuffer.Count == MeshData.Indices.Length)
            {
                IndexBuffer.SubData(MeshData.Indices, 0, MeshData.Indices.Length);
            }
            else
            {
                IndexBuffer.Dispose();
                IndexBuffer = IndexBuffer.Create(GraphicsDevice, MeshData.Indices, BufferUsage.StaticDraw, true);
            }
        }
    }
}