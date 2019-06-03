using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shader
{
    public class FragmentShader : Shader
    {
        public FragmentShader(ResourceManager resourceManager, string shaderSource) 
            : base(resourceManager, ShaderType.FragmentShader, shaderSource)
        {
        }
    }
}