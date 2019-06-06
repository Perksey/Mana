using System;
using System.Drawing;
using System.Threading;
using Mana.Graphics;
using Mana.Utilities;
using OpenTK;
using OpenTK.Graphics;

namespace Mana
{
    public class ManaWindow : GameWindow
    {
        public static ManaWindow MainWindow => _mainWindow;
        
        private static ManaWindow _mainWindow;
        private static RenderContext _mainContext = null;
        
        private Game _game;

        private float _elapsedUpdateTime;
        private float _elapsedRenderTime;
        
        public ManaWindow()
            : this(null)
        {
        }

        protected bool RenderScreenOnResize = true;
        
        // TODO: Remove duplicate graphics context creation parameters.
        public ManaWindow(ManaWindow parent)
            : base(1600, 900, GraphicsMode.Default, "Title", GameWindowFlags.Default, DisplayDevice.Default, 4, 5,
                   GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        {
            if (_mainWindow == null)
                _mainWindow = this;
            
            VSync = VSyncMode.Off;
            
            InputManager = new InputManager(this);
            
            Parent = parent;
            ResourceManager = ResourceManager.GetDefault(this);

            if (Equals(Context, ResourceManager.ShareContext))
            {
                if (_mainContext != null)
                    throw new Exception();
                
                DebugMessageHandler.Initialize();
                RenderContext = _mainContext = new RenderContext(ResourceManager, Context, WindowInfo);
                ResourceManager.SetMainContext(_mainContext);
                Thread.CurrentThread.Name = "Main Thread";
                Input.SetInputManager(InputManager);
            }
            else
            {
                RenderContext = new RenderContext(ResourceManager, WindowInfo);
            }
        }
     
        public ManaWindow Parent { get; }

        public InputManager InputManager { get; }
        
        public ResourceManager ResourceManager { get; }
        
        public RenderContext RenderContext { get; }

        public bool IsHovered => InputManager.HoveredWindow == this;

        public void Run(Game game)
        {
            _game = game;
            _game.OnRun(this);
            Run();
        }

        protected override void Dispose(bool manual)
        {
            RenderContext.Release();
            base.Dispose(manual);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            InputManager.Update();

            float deltaTime = (float)e.Time;
            _elapsedUpdateTime += deltaTime;

            _game?.UpdateBase(_elapsedUpdateTime, deltaTime);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            float deltaTime = (float)e.Time;
            _elapsedRenderTime += deltaTime;

            _game?.RenderBase(_elapsedRenderTime, deltaTime);
            
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            RenderContext.ViewportRectangle = new Rectangle(0, 0, Width, Height);
            RenderContext.ScissorRectangle = new Rectangle(0, 0, Width, Height);
            
            if (RenderScreenOnResize)
            {
                _game?.RenderBase(_elapsedRenderTime, float.Epsilon);
                SwapBuffers();
            }
        }
    }
}