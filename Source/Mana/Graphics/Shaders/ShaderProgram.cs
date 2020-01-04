using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Mana.Asset;
using Mana.Asset.Reloading;
using Mana.Utilities;
using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    /// <summary>
    /// Represents an OpenGL shader program object.
    /// </summary>
    public partial class ShaderProgram : GraphicsResource, IReloadableAsset
    {
        private static Logger _log = Logger.Create();

        private List<GLHandle> _attachedShaders = new List<GLHandle>(3);

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgram"/> class.
        /// </summary>
        /// <param name="parentContext">The <see cref="RenderContext"/> that will be used by the shader program.</param>
        public ShaderProgram(RenderContext parentContext)
            : base(parentContext)
        {
            Handle = (GLHandle)GL.CreateProgram();
            GLHelper.EnsureValid(Handle);
        }

        /// <summary>
        /// Gets the number of vertex attributes the shader program contains.
        /// </summary>
        public int AttributeCount { get; internal set; } = -1;

        /// <summary>
        /// Gets the number of uniforms the shader program contains.
        /// </summary>
        public int UniformCount { get; internal set; } = -1;

        /// <summary>
        /// Gets a value that indicates whether the program has been linked.
        /// </summary>
        public bool Linked { get; private set; }

        /// <summary>
        /// Gets a dictionary mapping each shader attribute name to its corresponding <see cref="ShaderAttributeInfo"/>.
        /// </summary>
        public Dictionary<string, ShaderAttributeInfo> Attributes { get; private set; }

        /// <summary>
        /// Gets a dictionary mapping each shader attribute location to its corresponding <see cref="ShaderAttributeInfo"/>.
        /// </summary>
        public Dictionary<uint, ShaderAttributeInfo> AttributesByLocation { get; private set; }

        /// <summary>
        /// Gets a dictionary mapping each shader uniform name to its corresponding <see cref="ShaderUniformInfo"/>.
        /// </summary>
        public Dictionary<string, ShaderUniformInfo> Uniforms { get; private set; }

        /// <summary>
        /// Gets a dictionary mapping each shader uniform name to its location.
        /// </summary>
        public Dictionary<string, int> UniformLocations { get; private set; }

        /// <summary>
        /// Gets a hash set of valid shader uniform locations.
        /// </summary>
        public HashSet<int> ValidUniformLocations { get; private set; }

        /// <summary>
        /// Attaches a <see cref="Shader"/> object to the program.
        /// </summary>
        /// <param name="shader">The shader to attach to the program.</param>
        public void AttachShader(Shader shader)
        {
            if (shader == null)
            {
                throw new ArgumentNullException(nameof(shader));
            }

            if (shader.Disposed)
            {
                throw new ObjectDisposedException(nameof(shader));
            }

            if (Linked)
            {
                throw new InvalidOperationException("Cannot attach a shader to an already linked shader program.");
            }

            if (_attachedShaders.Contains(shader.Handle))
            {
                throw new InvalidOperationException("Cannot attach the same shader to a shader program more than once.");
            }

            GL.AttachShader(Handle, shader.Handle);
            _attachedShaders.Add(shader.Handle);
        }

        /// <summary>
        /// Detaches a <see cref="Shader"/> object to the program.
        /// </summary>
        /// <param name="shader">The shader to detach to the program.</param>
        public void DetachShader(Shader shader)
        {
            if (shader == null)
            {
                throw new ArgumentNullException(nameof(shader));
            }

            if (shader.Disposed)
            {
                throw new ObjectDisposedException(nameof(shader));
            }

            if (!_attachedShaders.Contains(shader.Handle))
            {
                throw new InvalidOperationException("Cannot detach a shader from a program that does not contain that shader.");
            }

            GL.DetachShader(Handle, shader.Handle);
            _attachedShaders.Remove(shader.Handle);
        }

        /// <summary>
        /// Links the <see cref="ShaderProgram"/> with the attached shader programs.
        /// </summary>
        public void Link()
        {
            if (Linked)
            {
                throw new InvalidOperationException("Cannot link an already linked shader program.");
            }

            GL.LinkProgram(Handle);
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkStatus);

            if (linkStatus != 1)
            {
                string programInfoLog = GL.GetProgramInfoLog(Handle);
                throw new ShaderProgramLinkException(programInfoLog);
            }

            Linked = true;

            GenerateAttributeInfo();
            GenerateUniformInfo();
        }

        public void Bind(RenderContext renderContext)
        {
            BoundContext = renderContext;
            renderContext.BindShaderProgram(this);
        }

        public void Unbind(RenderContext renderContext)
        {
            BoundContext = null;
            renderContext.UnbindShaderProgram(this);
        }

        /// <inheritdoc/>
        protected override ObjectLabelIdentifier? LabelType => ObjectLabelIdentifier.Program;

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            EnsureUndisposed();

            if (BoundContext != null)
            {
                Unbind(BoundContext);
                BoundContext = null;
            }

            GL.DeleteProgram(Handle);
        }

        private void GenerateAttributeInfo()
        {
            if (Attributes != null)
            {
                throw new InvalidOperationException("Cannot generate shader program attribute info multiple times.");
            }

            GL.GetProgram(Handle, GetProgramParameterName.ActiveAttributes, out int attributeCount);
            AttributeCount = attributeCount;

            Attributes = new Dictionary<string, ShaderAttributeInfo>(AttributeCount);
            AttributesByLocation = new Dictionary<uint, ShaderAttributeInfo>(AttributeCount);

            for (int i = 0; i < AttributeCount; i++)
            {
                GL.GetActiveAttrib(Handle,
                                   i,
                                   256,
                                   out _,
                                   out int size,
                                   out ActiveAttribType type,
                                   out string name);

                // Location != Index
                int location = GL.GetAttribLocation(Handle, name);

                var info = new ShaderAttributeInfo(name, location, size, type);
                Attributes.Add(name, info);
                AttributesByLocation.Add((uint)location, info);
            }
        }

        private void GenerateUniformInfo()
        {
            if (Uniforms != null)
            {
                throw new InvalidOperationException("Cannot generate shader program uniform info multiple times.");
            }

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int uniformCount);
            UniformCount = uniformCount;

            Uniforms = new Dictionary<string, ShaderUniformInfo>(UniformCount);
            UniformLocations = new Dictionary<string, int>(UniformCount);
            ValidUniformLocations = new HashSet<int>();

            for (int i = 0; i < UniformCount; i++)
            {
                GL.GetActiveUniform(Handle,
                                    i,
                                    256,
                                    out _,
                                    out int size,
                                    out ActiveUniformType type,
                                    out string name);

                int location = GL.GetUniformLocation(Handle, name);

                var info = new ShaderUniformInfo(name, location, size, type);
                Uniforms.Add(name, info);
                UniformLocations.Add(name, location);
                ValidUniformLocations.Add(location);
            }
        }

        public string SourcePath { get; set; }
        public AssetManager AssetManager { get; set; }
        public string[] ShaderSourcePaths { get; set; }

        public void OnAssetLoaded()
        {
        }

        public void Reload(AssetManager assetManager)
        {
            // TODO: This almost certainly creates a lot of garbage, and memory leaks on the GPU. Probably want to make
            // the ShaderProgramLoader clean up in the event of shader loading failure so that it's more robust.

            ShaderProgram program = null;

            if (BoundContext != null)
            {
                Unbind(BoundContext);
            }

            try
            {
                using var stream = assetManager.GetStreamFromPath(SourcePath);

                var createdProgram = AssetManager.ShaderProgramLoader.Load(assetManager, assetManager.RenderContext, stream, SourcePath);
                program = createdProgram;
            }
            catch (Exception e)
            {
                _log.Error("Error reloading ShaderProgram: " + e.Message);
            }

            if (program != null)
            {
                var initialHandle = this.Handle;

                GL.DeleteProgram(Handle);

                this.Handle = program.Handle;
                this.AttributeCount = program.AttributeCount;
                this.UniformCount = program.UniformCount;
                this.Attributes = program.Attributes;
                this.AttributesByLocation = program.AttributesByLocation;
                this.ShaderSourcePaths = program.ShaderSourcePaths;
                this.Uniforms = program.Uniforms;
                this.UniformLocations = program.UniformLocations;
                this.ValidUniformLocations = program.ValidUniformLocations;

                _log.Info($"Shader reloaded successfully! {initialHandle} -> {Handle}");
            }
        }

        public string[] GetLiveReloadAssetPaths()
        {
            var arr = new string[ShaderSourcePaths.Length + 1];

            for (int i = 0; i < arr.Length - 1; i++)
            {
                arr[i] = ShaderSourcePaths[i];
            }

            arr[arr.Length - 1] = SourcePath;

            return arr;
        }
    }
}
