using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    /// <summary>
    /// Represents an OpenGL vertex shader object.
    /// </summary>
    public class VertexShader : Shader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexShader"/> class.
        /// </summary>
        /// <param name="parentContext">The <see cref="RenderContext"/> that will be used by the shader.</param>
        /// <param name="shaderSource">The vertex shader source code.</param>
        public VertexShader(RenderContext parentContext, string shaderSource)
            : base(parentContext, ShaderType.VertexShader, shaderSource)
        {
        }
    }
}
