using System.Collections.Generic;
using System.Diagnostics;
using Mana.Asset;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    public class ShaderProgram : ManaAsset, IGraphicsResource
    {
        internal bool Disposed = false;
        internal bool Linked = false;
        internal Dictionary<uint, ShaderAttributeInfo> AttributesByLocation;
        internal string VertexShaderPath;
        internal string FragmentShaderPath;
        
        public ShaderProgram(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            GraphicsDevice.Resources.Add(this);

            Handle = (GLHandle)GL.CreateProgram();
            GLHelper.CheckLastError();
        }

        public GLHandle Handle { get; private set; }

        public GraphicsDevice GraphicsDevice { get; }

        public int AttributeCount { get; internal set; }
        
        public Dictionary<string, ShaderAttributeInfo> Attributes { get; internal set; }
        
        public int UniformCount { get; internal set; }
        
        public Dictionary<string, ShaderUniformInfo> Uniforms { get; internal set; }
      
        public void Link(params Shader[] shaders)
        {
            ShaderHelper.AttachShaders(Handle, shaders);
            ShaderHelper.LinkShader(Handle);
            ShaderHelper.DetachShaders(Handle, shaders);

            Linked = true;
        }
        
        public override void Dispose()
        {
            Debug.Assert(!Disposed);
            
            GraphicsDevice.Resources.Remove(this);
            GraphicsDevice.UnbindShaderProgram(this);
            
            GL.DeleteProgram(Handle);
            GLHelper.CheckLastError();
           
            Disposed = true;
        }
    }
}