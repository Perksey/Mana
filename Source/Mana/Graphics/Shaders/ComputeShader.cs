using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    /// <summary>
    /// Represents an OpenGL compute shader object.
    /// </summary>
    public class ComputeShader : Shader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComputeShader"/> class.
        /// </summary>
        /// <param name="parentContext">The <see cref="RenderContext"/> that will be used by the shader.</param>
        /// <param name="shaderSource">The compute shader source code.</param>
        public ComputeShader(RenderContext parentContext, string shaderSource)
            : base(parentContext, ShaderType.ComputeShader, shaderSource)
        {
        }
    }
}
