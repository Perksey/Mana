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
            // Create GraphicsDevice
            GraphicsDevice = new GraphicsDevice();
        }
        
        public Game Game { get; private set; }

        public GraphicsDevice GraphicsDevice { get; }
        
        public int Width { get; set; }
        
        public int Height { get; set; }

        public float AspectRatio => Width / (float)Height;
        
        public bool Fullscreen { get; set; }
        
        public bool VSync { get; set; }

        public void Run(Game game)
        {
            _windowWrapper = new GameWindowWrapper(this, game);
            Game = game;

            Game.InitializeBase();
            
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