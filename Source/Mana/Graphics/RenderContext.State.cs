using System.Drawing;
using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public partial class RenderContext
    {
        private bool _depthTest;
        private bool _scissorTest;
        private bool _blend;
        private bool _cullBackfaces;
        private Rectangle _scissorRectangle;
        private Rectangle _viewportRectangle;
        private Color _clearColor;

        /// <summary>
        /// Gets or sets a value that indicates whether the DepthTest capability is enabled.
        /// </summary>
        public bool DepthTest
        {
            get => _depthTest;
            set
            {
                if (value == _depthTest)
                {
                    return;
                }

                GLHelper.SetCap(EnableCap.DepthTest, _depthTest = value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the ScissorTest capability is enabled.
        /// </summary>
        public bool ScissorTest
        {
            get => _scissorTest;
            set
            {
                if (value == _scissorTest)
                {
                    return;
                }

                GLHelper.SetCap(EnableCap.ScissorTest, _scissorTest = value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Blend capability is enabled.
        /// </summary>
        public bool Blend
        {
            get => _blend;
            set
            {
                if (value == _blend)
                {
                    return;
                }

                GLHelper.SetCap(EnableCap.Blend, _blend = value);

                if (value)
                {
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether backface culling is enabled.
        /// </summary>
        public bool CullBackfaces
        {
            get => _cullBackfaces;
            set
            {
                if (value == _cullBackfaces)
                {
                    return;
                }

                GLHelper.SetCap(EnableCap.CullFace, _cullBackfaces = value);

                if (!value)
                {
                    return;
                }

                GL.FrontFace(FrontFaceDirection.Ccw);
                GL.CullFace(CullFaceMode.Back);
            }
        }

        /// <summary>
        /// Gets or sets a value representing the scissor rectangle region.
        /// </summary>
        public Rectangle ScissorRectangle
        {
            get => _scissorRectangle;
            set
            {
                if (value == _scissorRectangle)
                {
                    return;
                }

                GL.Scissor(value.X, value.Y, value.Width, value.Height);
                _scissorRectangle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value representing the viewport rectangle region.
        /// </summary>
        public Rectangle ViewportRectangle
        {
            get => _viewportRectangle;
            set
            {
                if (value == _viewportRectangle)
                {
                    return;
                }

                GL.Viewport(value.X, value.Y, value.Width, value.Height);
                _viewportRectangle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value representing the ClearColor.
        /// </summary>
        public Color ClearColor
        {
            get => _clearColor;
            set
            {
                if (value == _clearColor)
                {
                    return;
                }

                GL.ClearColor(_clearColor = value);
            }
        }

        public State GetState()
        {
            return new State(_depthTest,
                             _scissorTest,
                             _blend,
                             _cullBackfaces,
                             _scissorRectangle,
                             _viewportRectangle,
                             _clearColor);
        }

        public void SetState(State state)
        {
            DepthTest = state.DepthTest;
            ScissorTest = state.ScissorTest;
            Blend = state.Blend;
            CullBackfaces = state.CullBackfaces;
            ScissorRectangle = state.ScissorRectangle;
            ViewportRectangle = state.ViewportRectangle;
            ClearColor = state.ClearColor;
        }

        /// <summary>
        /// An object that encapsulates certain OpenGL state objects so that they can easily be stored and restored.
        /// </summary>
        public struct State
        {
            public readonly bool DepthTest;
            public readonly bool ScissorTest;
            public readonly bool Blend;
            public readonly bool CullBackfaces;
            public readonly Rectangle ScissorRectangle;
            public readonly Rectangle ViewportRectangle;
            public readonly Color ClearColor;

            public State(bool depthTest,
                         bool scissorTest,
                         bool blend,
                         bool cullBackfaces,
                         Rectangle scissorRectangle,
                         Rectangle viewportRectangle,
                         Color clearColor)
            {
                DepthTest = depthTest;
                ScissorTest = scissorTest;
                Blend = blend;
                CullBackfaces = cullBackfaces;
                ScissorRectangle = scissorRectangle;
                ViewportRectangle = viewportRectangle;
                ClearColor = clearColor;
            }
        }
    }
}
