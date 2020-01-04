using Mana.Utilities;
using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    /// <summary>
    /// Represents an OpenGL shader object of a given type (Vertex, Fragment, Geometry, Compute, etc.)
    /// </summary>
    public class Shader : GraphicsResource
    {
        private static Logger _log = Logger.Create();

        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class using the given ShaderType and source string.
        /// </summary>
        /// <param name="parentContext">The parent <see cref="RenderContext"/> to create the shader with.</param>
        /// <param name="shaderType">The <see cref="ShaderType"/> of the shader.</param>
        /// <param name="shaderSource">The shader source code.</param>
        /// <exception cref="ShaderCompileException">Thrown if there was an error during shader compilation.</exception>
        protected Shader(RenderContext parentContext, ShaderType shaderType, string shaderSource)
            : base(parentContext)
        {
            //_log.Info($"Created {shaderType} shader with RenderContext {parentContext.ID}");

            Handle = (GLHandle)GL.CreateShader(shaderType);
            GLHelper.EnsureValid(Handle);

            GL.ShaderSource(Handle, shaderSource);

            GL.CompileShader(Handle);
            GL.GetShader(Handle, ShaderParameter.CompileStatus, out int status);

            if (status != 1)
            {
                string shaderInfoLog = GL.GetShaderInfoLog(Handle);
                throw new ShaderCompileException(shaderInfoLog);
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            EnsureUndisposed();
            GL.DeleteShader(Handle);
        }

        public string SourcePath { get; set; }

        /// <inheritdoc/>
        protected override ObjectLabelIdentifier? LabelType => ObjectLabelIdentifier.Shader;
    }
}
