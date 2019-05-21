using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public partial class GraphicsDevice
    {
        private Color _clearColor;
        private bool _depthTest;
        private bool _scissorTest;
        private bool _blend;
        private bool _cullBackfaces;
        private Rectangle _scissorRectangle;
        private Rectangle _viewportRectangle;
        
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
                    GL.CullFace(CullFaceMode.Back);
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

                _viewportRectangle = value;
            }
        }
        
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
        }
    }
}