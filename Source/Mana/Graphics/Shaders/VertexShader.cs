using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    public class VertexShader : Shader
    {
        public VertexShader(GraphicsDevice graphicsDevice, string shaderSource) 
            : base(graphicsDevice, ShaderType.VertexShader, shaderSource)
        {
        }
    }
}