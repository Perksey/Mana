using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace Mana.Graphics
{
    public class OpenTKWindow : IGameWindow, IDisposable
    {
        private GameWindowWrapper _windowWrapper;
        
        public OpenTKWindow()
        {
            GraphicsDevice = new GraphicsDevice();
        }
        
        public Game Game { get; private set; }

        public GraphicsDevice GraphicsDevice { get; }

        public int Width
        {
            get => _windowWrapper.Width;
            set => _windowWrapper.Width = value;
        }

        public int Height
        {
            get => _windowWrapper.Height;
            set => _windowWrapper.Height = value;
        }

        public float AspectRatio => Width / (float)Height;

        public bool Fullscreen
        {
            get => _windowWrapper.WindowState == WindowState.Fullscreen;
            set
            {
                WindowState valueState = value ? WindowState.Fullscreen : WindowState.Normal;

                if (_windowWrapper.WindowState != valueState)
                    _windowWrapper.WindowState = valueState;
            }
        }

        public bool VSync
        {
            // TODO: Replace VSync value from bool to a proxy enum that exposes adaptive option
            get => _windowWrapper.VSync == VSyncMode.On;
            set
            {
                VSyncMode valueMode = value ? VSyncMode.On : VSyncMode.Off;

                if (_windowWrapper.VSync != valueMode)
                    _windowWrapper.VSync = valueMode;
            }
        }

        public void Run(Game game)
        {
            _windowWrapper = new GameWindowWrapper(this, game);

            Game = game;

            Game.InitializeBase();
            
            Console.WriteLine($"Vsync Mode: {VSync.ToString()}");
            
            _windowWrapper.Run();
        }

        public void Dispose()
        {
            _windowWrapper?.Dispose();
            Game?.Dispose();
        }

        private sealed class GameWindowWrapper : GameWindow
        {
            private Game _game;
            private OpenTKWindow _openTKWindow;
            private float _elapsedUpdateTime;
            private float _elapsedRenderTime;

            public GameWindowWrapper(OpenTKWindow openTKWindow, Game game)
                : base(1280, 
                       720, 
                       new GraphicsMode(32, 16, 0, 8),
                       "Mana",
                       GameWindowFlags.Default,
                       DisplayDevice.Default,
                       1, 
                       0,
                       GraphicsContextFlags.Debug)
            {
                _openTKWindow = openTKWindow;
                _game = game;
                //VSync = VSyncMode.Off;
                //WindowState = WindowState.Maximized;
            }

            protected override void OnUpdateFrame(FrameEventArgs e)
            {
                base.OnUpdateFrame(e);
                
                float deltaTime = (float)e.Time;

                _elapsedUpdateTime += deltaTime;
                _game.UpdateBase(_elapsedUpdateTime, deltaTime);
            }

            protected override void OnRenderFrame(FrameEventArgs e)
            {
                base.OnRenderFrame(e);

                float deltaTime = (float)e.Time;

                _elapsedRenderTime += deltaTime;
                _game.RenderBase(_elapsedRenderTime, deltaTime);
                
                SwapBuffers();
            }

            protected override void OnKeyDown(KeyboardKeyEventArgs e)
            {
                base.OnKeyDown(e);

                if (e.Key == Key.Escape)
                    Close();
            }
        }
    }
}