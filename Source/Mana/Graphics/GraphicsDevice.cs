using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mana.Graphics.Buffers;
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

            _log.Info($"Max Texture Size: {GLHelper.GetInteger((GetPName.MaxTextureSize))}");
            
            //_log.Warn("Some random warning.");
            //_log.Error("An error occured.");
            //_log.Fatal("Fatal error occured.");

            _maxTextureImageUnits = GLHelper.GetInteger(GetPName.MaxTextureImageUnits);
            Bindings.TextureUnits = new GLHandle[_maxTextureImageUnits];
            
            int vao = GL.GenVertexArray();
            GLHelper.CheckLastError();
            
            GL.BindVertexArray(vao);
            GLHelper.CheckLastError();
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
        /// Binds the given <see cref="VertexBuffer"/> object to the GraphicsDevice.
        /// </summary>
        /// <param name="vbo">The <see cref="VertexBuffer"/> object to bind.</param>
        public void BindVertexBuffer(VertexBuffer vbo)
        {
            if (vbo == null)
            {
                BindBuffer(BufferTarget.ArrayBuffer, GLHandle.Zero);
                Bindings.VertexBuffer = GLHandle.Zero;
                return;
            }
            
            Debug.Assert(!vbo.Disposed);

            if (Bindings.VertexBuffer == vbo.Handle)
                return;

            BindBuffer(BufferTarget.ArrayBuffer, vbo.Handle);
            Bindings.VertexBuffer = vbo.Handle;
        }
        
        /// <summary>
        /// Ensures that the given <see cref="VertexBuffer"/> object is unbound.
        /// </summary>
        /// <param name="vbo">The <see cref="VertexBuffer"/> object to ensure is unbound.</param>
        public void UnbindVertexBuffer(VertexBuffer vbo)
        {
            if (vbo == null)
                throw new ArgumentNullException(nameof(vbo));

            if (Bindings.VertexBuffer != vbo.Handle)
                return;

            BindBuffer(BufferTarget.ArrayBuffer, GLHandle.Zero);
            Bindings.VertexBuffer = GLHandle.Zero;
        }
        
        /// <summary>
        /// Binds the given <see cref="IndexBuffer"/> object to the GraphicsDevice.
        /// </summary>
        /// <param name="ebo">The <see cref="IndexBuffer"/> object to bind.</param>
        public void BindIndexBuffer(IndexBuffer ebo)
        {
            if (ebo == null)
            {
                BindBuffer(BufferTarget.ElementArrayBuffer, GLHandle.Zero);
                Bindings.IndexBuffer = GLHandle.Zero;
                return;
            }

            Debug.Assert(!ebo.Disposed);

            if (Bindings.IndexBuffer == ebo.Handle)
                return;

            BindBuffer(BufferTarget.ElementArrayBuffer, ebo.Handle);
            Bindings.IndexBuffer = ebo.Handle;
        }

        /// <summary>
        /// Ensures that the given <see cref="IndexBuffer"/> object is unbound.
        /// </summary>
        /// <param name="ebo">The <see cref="IndexBuffer"/> object to ensure is unbound.</param>
        public void UnbindIndexBuffer(IndexBuffer ebo)
        {
            if (ebo == null)
                throw new ArgumentNullException(nameof(ebo));

            if (Bindings.IndexBuffer != ebo.Handle)
                return;

            BindBuffer(BufferTarget.ElementArrayBuffer, GLHandle.Zero);
            Bindings.IndexBuffer = GLHandle.Zero;
        }
        
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
                Metrics._clearCount++;
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
                Metrics._clearCount++;
            }
        }
        
        #endregion
        
        
        #region Render

        [Obsolete("This method is deprecated. Use VertexBuffer rendering instead.")]
        public void Render<T>(T[] vertexData, 
                              ShaderProgram shaderProgram, 
                              PrimitiveType primitiveType = PrimitiveType.Triangles)
            where T : unmanaged
        {
            if (vertexData.Length == 0)
                return;
            
            BindVertexBuffer(null);
            BindIndexBuffer(null);
            BindShaderProgram(shaderProgram);

            GCHandle pinned = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
            VertexTypeInfo.Get<T>().Apply(shaderProgram, pinned.AddrOfPinnedObject());
            
            GL.DrawArrays(primitiveType, 0, vertexData.Length);
            GLHelper.CheckLastError();

            pinned.Free();

            unchecked
            {
                Metrics._primitiveCount += vertexData.Length;
                Metrics._drawCalls++;
            }
        }

        public void Render(VertexBuffer vertexBuffer,
                           ShaderProgram shaderProgram,
                           PrimitiveType primitiveType = PrimitiveType.Triangles)
        {
            if (vertexBuffer.VertexCount == 0)
                return;
            
            BindVertexBuffer(vertexBuffer);
            BindIndexBuffer(null);
            BindShaderProgram(shaderProgram);

            vertexBuffer.VertexTypeInfo.Apply(shaderProgram);

            GL.DrawArrays(primitiveType, 0, vertexBuffer.VertexCount);
            GLHelper.CheckLastError();

            unchecked
            {
                Metrics._drawCalls++;
                Metrics._primitiveCount += vertexBuffer.VertexCount;
            }
        }

        public void Render(VertexBuffer vertexBuffer,
                           IndexBuffer indexBuffer,
                           ShaderProgram shaderProgram,
                           PrimitiveType primitiveType = PrimitiveType.Triangles)
        {
            if (vertexBuffer.VertexCount == 0 || indexBuffer.IndexCount == 0)
                return;

            BindVertexBuffer(vertexBuffer);
            BindIndexBuffer(indexBuffer);
            BindShaderProgram(shaderProgram);
            
            vertexBuffer.VertexTypeInfo.Apply(shaderProgram);
            
            GL.DrawElements(primitiveType, indexBuffer.IndexCount, indexBuffer.DataType, 0);
            
            unchecked
            {
                Metrics._drawCalls++;
                Metrics._primitiveCount += indexBuffer.IndexCount;
            }
        }
        
        #endregion


        public bool IsVersionAtLeast(int major, int minor)
        {
            int version = (Extensions.Major * 10) + Extensions.Minor;
            return version >= (major * 10) + minor;
        }
        
        
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
        internal void SetActiveTexture(int activeTexture)
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