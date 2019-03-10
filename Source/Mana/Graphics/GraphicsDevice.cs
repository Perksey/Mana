using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public class GraphicsDevice
    {
        private static GraphicsDevice _instance;

        internal readonly GLExtensions Extensions;
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
            Bindings = new GraphicsDeviceBindings();

            DepthTest = true;
            ScissorTest = true;
            CullBackfaces = true;
            Blend = false;
            DepthTest = true;
            ScissorTest = true;
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
        
        
        /// <summary>
        /// Clears the color buffer to the given color.
        /// </summary>
        /// <param name="color">The color to clear the color buffer to.</param>
        public void Clear(Color color)
        {
            if (_clearColor != color)
            {
                GL.ClearColor(color.R / (float)byte.MaxValue, color.G / (float)byte.MaxValue, color.B / (float)byte.MaxValue, color.A / (float)byte.MaxValue);
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
        
        #endregion
    }
}