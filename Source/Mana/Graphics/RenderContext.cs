using System;
using System.Drawing;
using System.Threading;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shader;
using Mana.Graphics.Vertex;
using Mana.Utilities;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Platform;

namespace Mana.Graphics
{
    /// <summary>
    /// Represents a single thread's OpenGL context and state.
    /// </summary>
    public partial class RenderContext
    {
        private static ThreadLocal<RenderContext> _currentContext = new ThreadLocal<RenderContext>();

        private bool _depthTest;
        private bool _scissorTest;
        private bool _blend;
        private bool _cullBackfaces;
        private Rectangle _scissorRectangle;
        private Rectangle _viewportRectangle;
        private Color _clearColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContext"/> class.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> to be associated with the context.</param>
        /// /// <param name="existingContext">The existing OpenGL context object for the <see cref="RenderContext"/> to use.</param>
        /// <param name="windowInfo">The <see cref="IWindowInfo"/> to create the context with.</param>
        public RenderContext(ResourceManager resourceManager, IGraphicsContext existingContext, IWindowInfo windowInfo)
        {
            GLInfo.Initialize();
            VertexTypeInfo.Initialize();
            
            ResourceManager = resourceManager;
            
            var prevContext = Current;
            
            if (existingContext == null)
            {
                // TODO: Remove duplicate graphics context creation parameters.
                GLContext = new GraphicsContext(GraphicsMode.Default,
                                                windowInfo,
                                                ResourceManager.ShareContext,
                                                4,
                                                5,
                                                GraphicsContextFlags.ForwardCompatible
                                                | GraphicsContextFlags.Debug);
            }
            else
            {
                GLContext = existingContext;
            }

            DebugMessageHandler.Initialize();
            
            // if (_currentContext.Value != null)
            //     throw new InvalidOperationException();
            
            _currentContext.Value = this;

            prevContext?.MakeCurrent();

            WindowInfo = windowInfo;
            
            InitializeBindings();

            DepthTest = true;
            ScissorTest = true;
            CullBackfaces = true;
            Blend = false;
            
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContext"/> class.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> to be associated with the context.</param>
        /// <param name="windowInfo">The <see cref="IWindowInfo"/> to create the context with.</param>
        public RenderContext(ResourceManager resourceManager, IWindowInfo windowInfo)
            : this(resourceManager, null, windowInfo)
        {
        }

        /// <summary>
        /// Gets the <see cref="RenderContext"/> bound to the current thread. 
        /// </summary>
        public static RenderContext Current => _currentContext.Value;

        /// <summary>
        /// Gets the OpenGL context object associated with the context.
        /// </summary>
        public IGraphicsContext GLContext { get; }
        
        /// <summary>
        /// Gets the <see cref="IWindowInfo"/> associated with the context.
        /// </summary>
        public IWindowInfo WindowInfo { get; }

        /// <summary>
        /// Gets the <see cref="ResourceManager"/> associated with the context.
        /// </summary>
        public ResourceManager ResourceManager { get; }
        
        /// <summary>
        /// Gets or sets a value that indicates whether the DepthTest capability is enabled.
        /// </summary>
        public bool DepthTest
        {
            get => _depthTest;
            set
            {
                if (value == _depthTest) 
                    return;
                
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
                    return;

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
                    return;

                GLHelper.SetCap(EnableCap.Blend, _blend = value);
                
                if (value)
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
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
                    return;

                GLHelper.SetCap(EnableCap.CullFace, _cullBackfaces = value);

                if (!value) 
                    return;
                
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
                    return;

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
                    return;

                GL.Viewport(value.X, value.Y, value.Width, value.Height);
                _viewportRectangle = value;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this instance is current in the calling thread.
        /// </summary>
        public bool IsCurrent => GLContext.IsCurrent;
        
        /// <summary>
        /// Makes the <see cref="RenderContext"/> current in the calling thread.
        /// </summary>
        public void MakeCurrent()
        {
            GLContext.MakeCurrent(WindowInfo);
            _currentContext.Value = this;
        }

        /// <summary>
        /// Makes the <see cref="RenderContext"/> no longer current in the calling thread.
        /// </summary>
        public void Release()
        {
            GLContext.MakeCurrent(null);
            _currentContext.Value = null;
        }

        /// <summary>
        /// Clears the framebuffer to the given color. 
        /// </summary>
        /// <param name="color">The color the buffer will be cleared to.</param>
        public void Clear(Color color)
        {
            if (_clearColor != color)
                GL.ClearColor(_clearColor = color);
            
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void Render(VertexBuffer vertexBuffer, IndexBuffer indexBuffer, ShaderProgram shaderProgram)
        {
            if (vertexBuffer.Count == 0)
                return;
            
            BindVertexBuffer(vertexBuffer);
            BindIndexBuffer(null);
            BindShaderProgram(shaderProgram);

            vertexBuffer.VertexTypeInfo.Apply(shaderProgram);

            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexBuffer.Count);

            unchecked
            {
                // TODO: This.
                //Metrics._drawCalls++;
                //Metrics._primitiveCount += vertexBuffer.Count;
            }
        }
    }
}
