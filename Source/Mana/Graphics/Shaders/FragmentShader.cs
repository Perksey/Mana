using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    /// <summary>
    /// Represents an OpenGL fragment shader object.
    /// </summary>
    public class FragmentShader : Shader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FragmentShader"/> class.
        /// </summary>
        /// <param name="parentContext">The <see cref="RenderContext"/> that will be used by the shader.</param>
        /// <param name="shaderSource">The fragment shader source code.</param>
        public FragmentShader(RenderContext parentContext, string shaderSource)
            : base(parentContext, ShaderType.FragmentShader, shaderSource)
        {
        }
    }
}
