using Mana.Asset;
using Mana.Graphics.Shaders;

namespace Mana.Graphics.Geometry
{
    public class Model : ManaAsset, IGraphicsResource
    {
        public readonly Mesh[] Meshes;
        
        public Model(GraphicsDevice graphicsDevice, Mesh[] meshes)
        {
            GraphicsDevice = graphicsDevice;
            Meshes = meshes;
            
            GraphicsDevice.Resources.Add(this);
        }

        public GraphicsDevice GraphicsDevice { get; }

        public void Render(ShaderProgram shaderProgram)
        {
            for (int i = 0; i < Meshes.Length; i++)
            {
                Meshes[i].Render(shaderProgram);
            }
        }
        
        public override void Dispose()
        {
            GraphicsDevice.Resources.Remove(this);
        }
    }
}