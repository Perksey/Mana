using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mana.Graphics.Shaders;
using Mana.Graphics.Vertex;
using Mana.Logging;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public class GraphicsDevice
    {
        private static Logger _log = Logger.Create();
        private static GraphicsDevice _instance;
        private static int _maxTextureImageUnits;

        internal readonly GLExtensions Extensions;
        internal readonly GraphicsResourceCollection Resources;
        internal GraphicsDeviceBindings Bindings;

        private Color _clearColor;
        
        /* State */
        private bool _depthTest;
        private bool _scissorTest;
        private bool _blend;
        private bool _cullBackfaces;
        private Rectangle _scissorRectangle;
        private Rectangle _viewportRectangle;
                
        public GraphicsDevice()
        {
            if (_instance != null)
                throw new InvalidOperationException("An instance of GraphicsDevice already exists.");
            
            _instance = this;
            
            Extensions = new GLExtensions();
            Resources = new GraphicsResourceCollection();
            Bindings = new GraphicsDeviceBindings();

            DepthTest = true;
            ScissorTest = true;
            CullBackfaces = true;
            Blend = false;
            DepthTest = true;
            ScissorTest = true;
            
            _log.Info("--------- OpenGL Context Information ---------");
            _log.Info($"Vendor: {GLHelper.GetString(StringName.Vendor)}");
            _log.Info($"Renderer: {GLHelper.GetString(StringName.Renderer)}");
            _log.Info($"Version: {GLHelper.GetString(StringName.Version)}");
            _log.Info($"ShadingLanguageVersion: {GLHelper.GetString(StringName.ShadingLanguageVersion)}");
            
            _log.Info($"Number of Available Extensions: {GLHelper.GetInteger(GetPName.NumExtensions).ToString()}");
            
            _log.Warn("Some random warning.");
            _log.Error("An error occured.");
            _log.Fatal("Fatal error occured.");

            _maxTextureImageUnits = GLHelper.GetInteger(GetPName.MaxTextureImageUnits);
        }

        
        #region State
        
        /// <summary>
        /// Gets or sets a value that indicates whether depth testing is enabled on the GraphicsDevice.
        /// </summary>
        public bool DepthTest
        {
            get => _depthTest;
            set
            {
                if (value == _depthTest)
                    return;
                
                SetCapability(EnableCap.DepthTest, _depthTest = value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scissor testing is enabled on the GraphicsDevice.
        /// </summary>
        public bool ScissorTest
        {
            get => _scissorTest;
            set
            {
                if (value == _scissorTest)
                    return;
                
                SetCapability(EnableCap.ScissorTest, _scissorTest = value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether blending is enabled on the GraphicsDevice.
        /// </summary>
        public bool Blend
        {
            get => _blend;
            set
            {
                if (value == _blend)
                    return;

                SetCapability(EnableCap.Blend, _blend = value);
                
                if (value)
                {
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    GLHelper.CheckLastError();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether backface culling is enabled on the GraphicsDevice.
        /// </summary>
        public bool CullBackfaces
        {
            get => _cullBackfaces;
            set
            {
                if (value == _cullBackfaces)
                    return;

                SetCapability(EnableCap.CullFace, _cullBackfaces = value);

                if (value)
                {
                    GL.FrontFace(FrontFaceDirection.Ccw);
                    GLHelper.CheckLastError();
                    
                    GL.CullFace(CullFaceMode.Back);
                    GLHelper.CheckLastError();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that represents the GraphicsDevice scissor rectangle.
        /// </summary>
        public Rectangle ScissorRectangle
        {
            get => _scissorRectangle;
            set
            {
                if (value == _scissorRectangle)
                    return;
                
                GL.Scissor(value.X,
                           value.Y,
                           value.Width,
                           value.Height);
                GLHelper.CheckLastError();

                _scissorRectangle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that represents the GraphicsDevice viewport rectangle.
        /// </summary>
        public Rectangle ViewportRectangle
        {
            get => _viewportRectangle;
            set
            {
                if (value == _viewportRectangle)
                    return;

                GL.Viewport(value.X,
                            value.Y,
                            value.Width,
                            value.Height);
                GLHelper.CheckLastError();

                _viewportRectangle = value;
            }
        }

        #endregion
        
        
        #region Bindings

        /// <summary>
        /// Binds the given <see cref="ShaderProgram"/> object to the GraphicsDevice.
        /// </summary>
        /// <param name="program">The <see cref="ShaderProgram"/> object to bind.</param>
        public void BindShaderProgram(ShaderProgram program)
        {
            if (program == null)
            {
                GL.UseProgram(0);
                GLHelper.CheckLastError();

                Bindings.ShaderProgram = GLHandle.Zero;
                return;
            }

            Debug.Assert(!program.Disposed && program.Linked);

            if (Bindings.ShaderProgram == program.Handle)
                return;

            GL.UseProgram(program.Handle);
            GLHelper.CheckLastError();
            Bindings.ShaderProgram = program.Handle;
        }

        /// <summary>
        /// Ensures that the given <see cref="ShaderProgram"/> object is unbound.
        /// </summary>
        /// <param name="program">The <see cref="ShaderProgram"/> object to ensure is unbound.</param>
        public void UnbindShaderProgram(ShaderProgram program)
        {
            if (program == null)
                throw new ArgumentNullException(nameof(program));

            if (Bindings.ShaderProgram != program.Handle)
                return;

            GL.UseProgram(0);
            GLHelper.CheckLastError();

            Bindings.ShaderProgram = GLHandle.Zero;
        }

        /// <summary>
        /// Binds the given <see cref="Texture2D"/> to the GraphicsDevice at the given texture unit location.
        /// </summary>
        /// <param name="textureUnit">The texture unit that the texture will be bound to.</param>
        /// <param name="texture">The <see cref="Texture2D"/> to bind.</param>
        public void BindTexture(int textureUnit, Texture2D texture)
        {
            if (textureUnit < 0 || textureUnit >= _maxTextureImageUnits)
                throw new ArgumentOutOfRangeException();

            SetActiveTexture(textureUnit);

            if (texture == null)
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GLHelper.CheckLastError();

                Bindings.Texture = GLHandle.Zero;
                return;
            }
            
            Debug.Assert(!texture.Disposed);

            if (Bindings.Texture == texture.Handle)
                return;

            GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
            GLHelper.CheckLastError();
            Bindings.Texture = texture.Handle;
        }

        /// <summary>
        /// Ensures that the given <see cref="Texture2D"/> object is unbound.
        /// </summary>
        /// <param name="texture">The <see cref="Texture2D"/> object to ensure is unbound.</param>
        public void UnbindTexture(Texture2D texture)
        {
            for (int i = 0; i < _maxTextureImageUnits; i++)
            {
                if (Bindings.TextureUnits[i] == texture.Handle)
                {
                    SetActiveTexture(i);

                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    GLHelper.CheckLastError();

                    Bindings.Texture = GLHandle.Zero;
                }
            }
        }
        
        #endregion
        
        
        #region Clear
        
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
                GLHelper.CheckLastError();

                _clearColor = color;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GLHelper.CheckLastError();

            unchecked
            {
                GraphicsMetrics._clearCount++;
            }
        }

        /// <summary>
        /// Clears the depth buffer.
        /// </summary>
        public void ClearDepth()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GLHelper.CheckLastError();
            
            unchecked
            {
                GraphicsMetrics._clearCount++;
            }
        }
        
        #endregion
        
        
        #region Render

        public void Render<T>(T[] vertexData, 
                              ShaderProgram shaderProgram, 
                              PrimitiveType primitiveType = PrimitiveType.Triangles)
            where T : unmanaged
        {
            BindShaderProgram(shaderProgram);

            GCHandle pinned = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
            VertexTypeInfo.Get<T>().Apply(shaderProgram, pinned.AddrOfPinnedObject());
            
            GL.DrawArrays(primitiveType, 0, vertexData.Length);
            GLHelper.CheckLastError();

            pinned.Free();

            unchecked
            {
                GraphicsMetrics._primitiveCount += vertexData.Length;
                GraphicsMetrics._drawCalls++;
            }
        }
        
        #endregion
        
        
        #region Private Helpers
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetCapability(EnableCap enableCap, bool value)
        {
            if (value)
            {
                GL.Enable(enableCap);
            }
            else
            {
                GL.Disable(enableCap);
            }

            GLHelper.CheckLastError();
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BindBuffer(BufferTarget buffer, GLHandle handle)
        {
            GL.BindBuffer(buffer, handle);
            GLHelper.CheckLastError();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetActiveTexture(int activeTexture)
        {
            if (Bindings.ActiveTexture != activeTexture)
            {
                Bindings.ActiveTexture = activeTexture;
                GL.ActiveTexture((TextureUnit)(activeTexture + (int)TextureUnit.Texture0));
                GLHelper.CheckLastError();
            }
        }
        
        #endregion
    }
}