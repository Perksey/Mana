using System.IO;
using Mana.Asset.Data;
using Mana.Graphics.Shaders;
using Newtonsoft.Json;

namespace Mana.Asset.Loaders
{
    public class ShaderProgramLoader : IAssetLoader<ShaderProgram>
    {
        public ShaderProgram Load(AssetManager manager, AssetSource assetSource)
        {
            ShaderProgramDescription description;
            using (StreamReader streamReader = new StreamReader(assetSource.Stream))
            {
                description = JsonConvert.DeserializeObject<ShaderProgramDescription>(streamReader.ReadToEnd());
            }
            
            
            string containingPath = Path.GetDirectoryName(assetSource.Path);
            if (containingPath == null)
                throw new System.Exception("Error getting parent for path: " + assetSource.Path);

            string vertexShaderPath = Path.Combine(containingPath, description.Vertex);
            string fragmentShaderPath = Path.Combine(containingPath, description.Fragment);

            VertexShader vertexShader = manager.Load<VertexShader>(vertexShaderPath);
            FragmentShader fragmentShader = manager.Load<FragmentShader>(fragmentShaderPath);

            ShaderProgram shaderProgram = new ShaderProgram(manager.GraphicsDevice);

            shaderProgram.Link(vertexShader, fragmentShader);

            manager.Unload(fragmentShader);
            manager.Unload(vertexShader);

            ShaderHelper.GetAttributeInfo(shaderProgram.Handle,
                                          out int attributeCount,
                                          out var attributes,
                                          out var attributesByLocation);
            shaderProgram.AttributeCount = attributeCount;
            shaderProgram.Attributes = attributes;
            shaderProgram.AttributesByLocation = attributesByLocation;

            ShaderHelper.GetUniformInfo(shaderProgram.Handle,
                                        out int uniformCount,
                                        out var uniforms);
            shaderProgram.UniformCount = uniformCount;
            shaderProgram.Uniforms = uniforms;

            shaderProgram.SourcePath = assetSource.Path;
            shaderProgram.VertexShaderPath = vertexShaderPath;
            shaderProgram.FragmentShaderPath = fragmentShaderPath;

            return shaderProgram;
        }
    }
}