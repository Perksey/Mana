using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    /// <summary>
    /// Represents an OpenGL geometry shader object.
    /// </summary>
    public class GeometryShader : Shader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryShader"/> class.
        /// </summary>
        /// <param name="parentContext">The <see cref="RenderContext"/> that will be used by the shader.</param>
        /// <param name="shaderSource">The geometry shader source code.</param>
        public GeometryShader(RenderContext parentContext, string shaderSource)
            : base(parentContext, ShaderType.GeometryShader, shaderSource)
        {
        }
    }
}
