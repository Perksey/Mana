using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shader
{
    public partial class ShaderProgram : GraphicsResource
    {
        internal Dictionary<uint, ShaderAttributeInfo> AttributesByLocation;
        internal Dictionary<ShaderType, string> ShaderPaths = new Dictionary<ShaderType, string>();
        internal bool Linked = false;
        
        public ShaderProgram(ResourceManager resourceManager) 
            : base(resourceManager)
        {
            Handle = (GLHandle)GL.CreateProgram();
            
            resourceManager.OnResourceCreated(this);
        }
        
        public int AttributeCount { get; internal set; }
        
        public int UniformCount { get; internal set; }
        
        public Dictionary<string, ShaderAttributeInfo> Attributes { get; internal set; }
        
        public Dictionary<string, ShaderUniformInfo> Uniforms { get; internal set; }
        public Dictionary<string, int> UniformLocations { get; internal set; }
        public Dictionary<int, ShaderUniformInfo> UniformsByLocation { get; internal set; }

        protected override ObjectLabelIdentifier? LabelType => ObjectLabelIdentifier.Program;
        
        public void Link(params Shader[] shaders)
        {
            ShaderHelper.AttachShaders(Handle, shaders);
            ShaderHelper.LinkShader(Handle);
            ShaderHelper.DetachShaders(Handle, shaders);

            Linked = true;
        }

        public void Bind(RenderContext context) => context.BindShaderProgram(this);
        public void Unbind(RenderContext context) => context.UnbindShaderProgram(this);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            GL.DeleteProgram(Handle);
        }
    }
}