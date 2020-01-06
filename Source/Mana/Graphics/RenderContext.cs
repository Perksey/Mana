using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Mana.Graphics.Vertex;
using Mana.Utilities.OpenGL;
using osuTK.Graphics;
using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    /// <summary>
    /// Represents an OpenGL context capable of being made current on a thread, as well as encapsulating the
    /// OpenGL context's state.
    /// </summary>
    public partial class RenderContext : IDisposable
    {
        private static int _renderContextID = -1;

        public readonly int ID = -1;

        private static ThreadLocal<RenderContext> _threadLocalCurrent = new ThreadLocal<RenderContext>(false);
        private bool _selfCreatedWindow = false;

        private RenderContext(ManaWindow window, IGraphicsContext openGLContext)
        {
            ID = ++_renderContextID;

            Window = window;
            OpenGLContext = openGLContext;

            ThreadID = Thread.CurrentThread.ManagedThreadId;
            _threadLocalCurrent.Value = this;

            /* Initialize */

#if DEBUG
            DebugMessageHandler.Initialize();
#endif

            GLInfo.Initialize();
            VertexTypeInfo.Initialize();

            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            if (GLInfo.HasDebug)
            {
                string labelName = "Unused VertexArray";
                GL.ObjectLabel(ObjectLabelIdentifier.VertexArray, vao, labelName.Length, labelName);
            }

            TextureUnits = new TextureUnit[GLInfo.MaxTextureImageUnits];

            /* */
        }

        private RenderContext(ManaWindow window)
        {
            ID = ++_renderContextID;

            Window = window;
            OpenGLContext = CreateOffscreenContext();

            _selfCreatedWindow = true;

            /* Initialize */

#if DEBUG
            DebugMessageHandler.Initialize();
#endif

            GLInfo.Initialize();
            VertexTypeInfo.Initialize();

            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            if (GLInfo.HasDebug)
            {
                string labelName = "Unused VertexArray";
                GL.ObjectLabel(ObjectLabelIdentifier.VertexArray, vao, labelName.Length, labelName);
            }

            TextureUnits = new TextureUnit[GLInfo.MaxTextureImageUnits];

            /* */
        }

        public int ThreadID { get; set; }

        public ManaWindow Window { get; private set; }

        public IGraphicsContext OpenGLContext { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether this instance is current on the calling thread.
        /// </summary>
        public bool IsCurrent
        {
            get
            {
                if ((_threadLocalCurrent.Value == this && !OpenGLContext.IsCurrent) ||
                    (_threadLocalCurrent.Value != this && OpenGLContext.IsCurrent))
                    throw new InvalidOperationException("Invalid state.");

                return OpenGLContext.IsCurrent;
            }
        }

        /// <summary>
        /// Creates a <see cref="RenderContext"/> that wraps a window's own GraphicsContext object.
        /// </summary>
        /// <param name="window">The window to wrap.</param>
        /// <returns>The newly created <see cref="RenderContext"/> object.</returns>
        public static RenderContext WrapWindowContext(ManaWindow window)
        {
            return new RenderContext(window, window.Context);
        }

        /// <summary>
        /// Creates a new <see cref="RenderContext"/> that will be used for offscreen operations.
        /// </summary>
        /// <returns>The newly created <see cref="RenderContext"/> object.</returns>
        public static RenderContext CreateOffscreen()
        {
            return new RenderContext(null);
        }

        public static RenderContext GetCurrent()
        {
            return _threadLocalCurrent.Value;
        }

        public void MakeCurrent()
        {
            Validate(false);

            _threadLocalCurrent.Value = this;
            OpenGLContext.MakeCurrent(Window.WindowInfo);
            ThreadID = Thread.CurrentThread.ManagedThreadId;

            Validate(true);
        }

        public void MakeNonCurrent()
        {
            Validate(true);

            _threadLocalCurrent.Value = null;
            OpenGLContext.MakeCurrent(null);
            ThreadID = -1;

            Validate(false);
        }

        /// <summary>
        /// Checks to see if the calling thread is the same thread that the RenderContext is current on, and throws if
        /// not the case.
        /// </summary>
        [Conditional("DEBUG")]
        public void Validate(bool current)
        {
            if (current)
            {
                if ((_threadLocalCurrent.Value == this && !OpenGLContext.IsCurrent) ||
                    (_threadLocalCurrent.Value != this && OpenGLContext.IsCurrent))
                    throw new InvalidOperationException("Validation failed: Invalid state.");

                if (Thread.CurrentThread.ManagedThreadId != ThreadID)
                    throw new InvalidAsynchronousStateException("Validation failed: Invalid thread.");

                if (_threadLocalCurrent.Value != this || !OpenGLContext.IsCurrent)
                    throw new InvalidOperationException("Validation failed.");
            }
            else
            {
                if (Thread.CurrentThread.ManagedThreadId == ThreadID)
                    throw new InvalidAsynchronousStateException("Validation failed.");

                if (_threadLocalCurrent.Value == this)
                    throw new InvalidOperationException("Validation failed.");

                if (OpenGLContext.IsCurrent)
                    throw new InvalidOperationException("Validation failed.");
            }
        }

        private static IGraphicsContext CreateOffscreenContext()
        {
            if (_threadLocalCurrent.Value == null)
                throw new InvalidAsynchronousStateException();

            var currentContext = _threadLocalCurrent.Value;

            currentContext.Validate(true);

            var graphicsContext = new GraphicsContext(new GraphicsMode(32, 16, 0, 8), ManaWindow.MainWindow.WindowInfo);

            currentContext.OpenGLContext.MakeCurrent(ManaWindow.MainWindow.WindowInfo);

            currentContext.Validate(true);

            return graphicsContext;
        }

        private static IGraphicsContext CreateGraphicsContext()
        {
            if (RenderContext.GetCurrent() != null)
                throw new InvalidAsynchronousStateException("Cannot create a RenderContext on a thread with an active" +
                                                            "RenderContext.");

            // TODO: Set proper context request parameters here.
            return new GraphicsContext(GraphicsMode.Default, ManaWindow.MainWindow.WindowInfo);
        }

        public void Dispose()
        {
            if (_selfCreatedWindow)
            {
                OpenGLContext?.Dispose();
            }
        }
    }
}
