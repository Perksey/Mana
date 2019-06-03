using Mana.Asset;
using Mana.Graphics.Shader;

namespace Mana.Graphics.Geometry
{
    public class Model : GraphicsResource, IAsset
    {
        public readonly Mesh[] Meshes;
        
        public Model(ResourceManager resourceManager, Mesh[] meshes)
            : base (resourceManager)
        {
            Meshes = meshes;
        }

        public void Render(RenderContext renderContext, ShaderProgram shaderProgram)
        {
            for (int i = 0; i < Meshes.Length; i++)
            {
                Meshes[i].Render(renderContext, shaderProgram);
            }
        }

        public string SourcePath { get; set; }
        
        public AssetManager AssetManager { get; set; }
        
        public void OnAssetLoaded()
        {
        }
    }
}