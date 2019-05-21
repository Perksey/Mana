using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shaders;
using Mana.Graphics.Vertex;
using Mana.Logging;
using Mana.Utilities;
using Mana.Utilities.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public partial class GraphicsDevice
    {
        private static Logger _log = Logger.Create();
        private static Logger _glDebugLogger = new Logger("OpenGL");
        private static GraphicsDevice _instance;
        private static int _maxTextureImageUnits;

        public readonly OpenTKWindow Window;
        internal readonly GLExtensions Extensions;
        internal readonly GraphicsResourceContainer Resources;
        
        internal readonly bool DirectStateAccessSupported;
        internal readonly bool ImmutableStorageSupported;

        public GraphicsDevice(OpenTKWindow window)
        {
            if (_instance != null)
                throw new InvalidOperationException("An instance of GraphicsDevice already exists.");
            
            _instance = this;

            Window = window;
            Extensions = new GLExtensions();
            DirectStateAccessSupported = Extensions.ARB_DirectStateAccess || IsVersionAtLeast(4, 5);
            ImmutableStorageSupported = Extensions.ARB_BufferStorage || IsVersionAtLeast(4, 4);

            DirectStateAccessSupported = false;
            ImmutableStorageSupported = false;
            
            Resources = new GraphicsResourceContainer();
            Bindings = new BindingPoints();

            DepthTest = true;
            ScissorTest = true;
            CullBackfaces = true;
            Blend = false;
            DepthTest = true;
            ScissorTest = true;
            
            _log.Info("--------- OpenGL Context Information ---------");
            _log.Info($"Vendor: {GL.GetString(StringName.Vendor)}");
            _log.Info($"Renderer: {GL.GetString(StringName.Renderer)}");
            _log.Info($"Version: {GL.GetString(StringName.Version)}");
            _log.Info($"ShadingLanguageVersion: {GL.GetString(StringName.ShadingLanguageVersion)}");
            
            _log.Info($"Number of Available Extensions: {GL.GetInteger(GetPName.NumExtensions).ToString()}");

            _log.Info($"Max Texture Size: {GL.GetInteger((GetPName.MaxTextureSize)).ToString()}");

            _maxTextureImageUnits = GL.GetInteger(GetPName.MaxTextureImageUnits);
            Bindings.TextureUnits = new GLHandle[_maxTextureImageUnits];
            
            int vao = GL.GenVertexArray();
            
            GL.BindVertexArray(vao);
            
            _debugProcCallback = DebugCallback;
            GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
            
            unsafe
            {
                GL.DebugMessageControl(DebugSourceControl.DontCare, 
                                       DebugTypeControl.DontCare,
                                       DebugSeverityControl.DontCare, 
                                       0, 
                                       (int*)0, 
                                       true);
            }
            
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
        }
        
        /// <summary>
        /// Clears the color buffer to the given color.
        /// </summary>
        /// <param name="color">The color to clear the color buffer to.</param>
        public void Clear(Color color)
        {
            if (_clearColor != color)
            {
                GL.ClearColor(color.R / (float)byte.MaxValue, 
                              color.G / (float)byte.MaxValue, 
                              color.B / (float)byte.MaxValue, 
                              color.A / (float)byte.MaxValue);

                _clearColor = color;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            unchecked
            {
                Metrics._clearCount++;
            }
        }

        /// <summary>
        /// Clears the depth buffer.
        /// </summary>
        public void ClearDepth()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
            
            unchecked
            {
                Metrics._clearCount++;
            }
        }

        public void Render(VertexBuffer vertexBuffer,
                           ShaderProgram shaderProgram,
                           PrimitiveType primitiveType = PrimitiveType.Triangles)
        {
            if (vertexBuffer.Count == 0)
                return;
            
            BindVertexBuffer(vertexBuffer);
            BindIndexBuffer(null);
            BindShaderProgram(shaderProgram);

            vertexBuffer.VertexTypeInfo.Apply(shaderProgram);

            GL.DrawArrays(primitiveType, 0, vertexBuffer.Count);

            unchecked
            {
                Metrics._drawCalls++;
                Metrics._primitiveCount += vertexBuffer.Count;
            }
        }

        public void Render(VertexBuffer vertexBuffer,
                           IndexBuffer indexBuffer,
                           ShaderProgram shaderProgram,
                           PrimitiveType primitiveType = PrimitiveType.Triangles)
        {
            if (vertexBuffer.Count == 0 || indexBuffer.Count == 0)
                return;

            BindVertexBuffer(vertexBuffer);
            BindIndexBuffer(indexBuffer);
            BindShaderProgram(shaderProgram);
            
            vertexBuffer.VertexTypeInfo.Apply(shaderProgram);
            
            GL.DrawElements(primitiveType, indexBuffer.Count, indexBuffer.DataType, 0);
            
            unchecked
            {
                Metrics._drawCalls++;
                Metrics._primitiveCount += indexBuffer.Count;
            }
        }
        
        public bool IsVersionAtLeast(int major, int minor)
        {
            int version = (Extensions.Major * 10) + Extensions.Minor;
            return version >= (major * 10) + minor;
        }
    }
}