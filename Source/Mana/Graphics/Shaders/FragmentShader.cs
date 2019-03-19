using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    public class FragmentShader : Shader
    {
        public FragmentShader(GraphicsDevice graphicsDevice, string shaderSource) 
            : base(graphicsDevice, ShaderType.FragmentShader, shaderSource)
        {
        }
    }
}