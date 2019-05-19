using System.Diagnostics;
using Mana.Asset;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    public abstract class Shader : ManaAsset, IGraphicsResource
    {
        public readonly GLHandle Handle;
        internal bool Disposed = false;

        protected Shader(GraphicsDevice graphicsDevice, ShaderType shaderType, string shaderSource)
        {
            GraphicsDevice = graphicsDevice;
            GraphicsDevice.Resources.Add(this);

            Handle = (GLHandle)GL.CreateShader(shaderType);
            GL.ShaderSource(Handle, shaderSource);

            ShaderHelper.CompileShader(Handle);
        }
        
        public GraphicsDevice GraphicsDevice { get; }

        public override void Dispose()
        {
            Debug.Assert(!Disposed);
            
            GraphicsDevice.Resources.Remove(this);

            GL.DeleteShader(Handle);

            Disposed = true;
        }
    }
}