using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shader
{
    public class VertexShader : Shader
    {
        public VertexShader(ResourceManager resourceManager, string shaderSource)
            : base(resourceManager, ShaderType.VertexShader, shaderSource)
        {
        }
    }
}