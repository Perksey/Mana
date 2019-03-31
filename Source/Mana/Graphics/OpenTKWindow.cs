using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mana.Logging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

using MouseButtonTK = OpenTK.Input.MouseButton;

namespace Mana.Graphics
{
    public class OpenTKWindow : IGameWindow, IDisposable
    {
        private static Logger _log = Logger.Create();
        
        public IEnumerable<DisplayResolution> AvailableResolutions => 
            DisplayDevice.Default.AvailableResolutions.Distinct();
        
        private GameWindowWrapper _windowWrapper;
        
        public OpenTKWindow()
        {
            
        }
        
        #region IGameWindow Implementation
        
        public Game Game { get; private set; }

        public GraphicsDevice GraphicsDevice { get; private set; }

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

        public string Title
        {
            get => _windowWrapper.Title;
            set => _windowWrapper.Title = value;
        }

        public void Close()
        {
            _windowWrapper.Close();
        }

        #endregion
        
        public void Run(Game game)
        {
            _windowWrapper = new GameWindowWrapper(this, game);

            GraphicsDevice = new GraphicsDevice();
            
            Game = game;
            
            /*
            _log.Debug("Available Display Resolutions: ");

            foreach (var res in AvailableResolutions)
            {
                _log.Debug(res.ToString());
            }
            */

            Game.InitializeBase(this);
            
            _windowWrapper.Run();
        }

        public void Dispose()
        {
            _windowWrapper?.Dispose();
            Game?.Dispose();
        }

        private sealed class GameWindowWrapper : GameWindow
        {
            private static Logger _log = Logger.Create();
            
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
                VSync = VSyncMode.Off;
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

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                
                _openTKWindow.GraphicsDevice.ViewportRectangle = new Rectangle(0, 0, Width, Height);
                _openTKWindow.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Width, Height);
                
                _game.RenderBase(_elapsedRenderTime, 0f);
                SwapBuffers();
            }

            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                base.OnKeyPress(e);

                Input.OnKeyTyped(e.KeyChar);
            }

            protected override void OnKeyDown(KeyboardKeyEventArgs e)
            {
                base.OnKeyDown(e);

                Input.OnKeyPressed(KeyConverter.FromDesktopScanCode(e.ScanCode));
            }

            protected override void OnKeyUp(KeyboardKeyEventArgs e)
            {
                base.OnKeyUp(e);
                
                Input.OnKeyReleased(KeyConverter.FromDesktopScanCode(e.ScanCode));
            }

            protected override void OnMouseDown(MouseButtonEventArgs e)
            {
                base.OnMouseDown(e);
                
                Input.OnMouseButtonPressed(GetMouseButton(e.Button));
            }

            protected override void OnMouseUp(MouseButtonEventArgs e)
            {
                base.OnMouseUp(e);
                
                Input.OnMouseButtonReleased(GetMouseButton(e.Button));
            }

            protected override void OnMouseMove(MouseMoveEventArgs e)
            {
                base.OnMouseMove(e);

                Input.OnMouseMoved(e.Position);
            }

            protected override void OnMouseWheel(MouseWheelEventArgs e)
            {
                base.OnMouseWheel(e);

                Input.OnMouseScroll(e.Delta, e.Value);
            }

            private MouseButton GetMouseButton(MouseButtonTK buttonTK)
            {
                // Convert the mouse button from OpenTK's enum to Mana's enum.
                switch (buttonTK)
                {
                    case MouseButtonTK.Left:
                        return MouseButton.Left;
                    case MouseButtonTK.Middle:
                        return MouseButton.Middle;
                    case MouseButtonTK.Right:
                        return MouseButton.Right;
                    // TODO: Add more mouse buttons (?)
                    default:
                        return MouseButton.Unknown;
                }
            }
        }
    }
}