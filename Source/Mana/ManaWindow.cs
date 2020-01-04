using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Mana.Graphics;
using Mana.Utilities;
using osuTK;
using osuTK.Graphics;
using osuTK.Graphics.OpenGL4;
using osuTK.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Mana
{
    public class ManaWindow : GameWindow, IGameHost
    {
        private static Logger _log = Logger.Create();

        public static ManaWindow MainWindow { get; private set; }

        private Game _game;
        private float _elapsedTime;
        private Matrix4x4 _projectionMatrix;

        public bool RenderOnResize { get; protected set; } = true;

        public ManaWindow()
            : base(1280,
                   720,
                   new GraphicsMode(32, 16, 0, 8),
                   "Mana Window",
                   GameWindowFlags.Default,
                   DisplayDevice.Default,
                   4,
                   6,
                   GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        {
            Console.Title = "Mana Console";
            VSync = VSyncMode.Off;

            if (MainWindow == null)
            {
                MainWindow = this;
            }

            RenderContext = RenderContext.WrapWindowContext(this);
            RenderContext.Validate(true);

            InputProvider = new ManaWindowInputProvider(this);
            Input.SetInputProvider(InputProvider);

            _projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0f, Width, Height, 0f, -1f, 1f);
        }

        /// <summary>
        /// Gets the <see cref="InputProvider"/> for this <see cref="ManaWindow"/>.
        /// </summary>
        public IInputProvider InputProvider { get; }

        public RenderContext RenderContext { get; }

        public ref Matrix4x4 ProjectionMatrix => ref _projectionMatrix;

        public void Run(Game game)
        {
            _game = game;
            _game.OnBeforeRun(this);

            Run();
        }

        public Image<Rgba32> TakeScreenshot()
        {
            var image = new Image<Rgba32>(ClientSize.Width, ClientSize.Height);

            GL.ReadPixels(0,
                          0,
                          image.Width,
                          image.Height,
                          PixelFormat.Rgba,
                          PixelType.UnsignedByte,
                          ref MemoryMarshal.GetReference(image.GetPixelSpan()));

            image.Mutate(c => c.Flip(FlipMode.Vertical));

            return image;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            InputProvider.Update();

            base.OnUpdateFrame(e);

            _elapsedTime += (float)e.Time;

            _game?.UpdateBase(_elapsedTime, (float)e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _game?.RenderBase(_elapsedTime, (float)e.Time);

            SwapBuffers();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                Close();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            RenderContext.ViewportRectangle = new Rectangle(0, 0, Width, Height);
            RenderContext.ScissorRectangle = new Rectangle(0, 0, Width, Height);

            _projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0f, Width, Height, 0f, -1f, 1f);

            if (RenderOnResize)
            {
                _game?.RenderBase(_elapsedTime, float.Epsilon);
                SwapBuffers();
            }
        }
    }
}
