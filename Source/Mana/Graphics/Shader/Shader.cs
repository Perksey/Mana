using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shader
{
    public abstract class Shader : GraphicsResource
    {
        protected Shader(ResourceManager resourceManager, ShaderType shaderType, string shaderSource)
            : base(resourceManager)
        {
            Handle = (GLHandle)GL.CreateShader(shaderType);
            
            if (Handle == GLHandle.Zero)
                throw new InvalidOperationException();
            
            GL.ShaderSource(Handle, shaderSource);

            ShaderHelper.CompileShader(Handle);
            
            resourceManager.OnResourceCreated(this);
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            GL.DeleteShader(Handle);
        }
        
        protected override ObjectLabelIdentifier? LabelType => ObjectLabelIdentifier.Shader;
    }
}