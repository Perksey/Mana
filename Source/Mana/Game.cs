using System;
using Mana.Asset;
using Mana.Graphics;
using Mana.Graphics.Vertex;
using Mana.Utilities;

namespace Mana
{
    public abstract class Game : IDisposable
    {
        private float _fpsAccumulator = 0;
        private int _fpsFrameCounter = 0;

        internal static long CurrentFrame = 0;
        
        protected Game()
        {
            VertexTypeInfo.Initialize();
            Components = new GameComponentCollection(this);
        }

        public IGameWindow Window { get; private set; }
        
        public GraphicsDevice GraphicsDevice { get; private set; }

        public AssetManager AssetManager { get; private set; }
        
        public GameComponentCollection Components { get; }
        
        public void Dispose()
        {
        }

        public void Quit()
        {
            Window.Close();
        }

        /// <summary>
        /// The base function that calls the Game's main Initialize function.
        /// </summary>
        public void InitializeBase(IGameWindow window)
        {
            Window = window;
            GraphicsDevice = window.GraphicsDevice;
            AssetManager = new AssetManager(GraphicsDevice);

            Input.Initialize(window);
            
            Initialize();
        }

        /// <summary>
        /// The base function that calls the Game's main Update function.  
        /// </summary>
        /// <param name="time">The time, in seconds, since the game was launched.</param>
        /// <param name="deltaTime">The time, in seconds, since the last frame.</param>
        public void UpdateBase(float time, float deltaTime)
        {
            // Early Update
            Dispatcher.EarlyUpdateDispatcher.InvokeActionsInQueue();
            Components.EarlyUpdate(time, deltaTime);
            
            // Update
            Update(time, deltaTime);
            Components.Update(time, deltaTime);

            // Late Update
            Components.LateUpdate(time, deltaTime);

            CurrentFrame++;
        }

        /// <summary>
        /// The base function that calls the Game's main Render function.
        /// </summary>
        /// <param name="time">The time, in seconds, since the game was launched.</param>
        /// <param name="deltaTime">The time, in seconds, since the last frame.</param>
        public void RenderBase(float time, float deltaTime)
        {
            // Early Render
            Components.EarlyRender(time, deltaTime);
            
            // Render
            Render(time, deltaTime);
            Components.Render(time, deltaTime);

            // Late Render
            Components.LateRender(time, deltaTime);
            Input.Update();
            UpdateMetrics(deltaTime);
        }

        protected abstract void Initialize();
        protected abstract void Update(float time, float deltaTime);
        protected abstract void Render(float time, float deltaTime);

        private void UpdateMetrics(float deltaTime)
        {
            _fpsAccumulator += deltaTime;

            if (_fpsAccumulator > 1.0f)
            {
                Metrics._framesPerSecond = _fpsFrameCounter;
                _fpsAccumulator %= 1.0f;
                _fpsFrameCounter = 0;
                Metrics._totalMegabytes = GC.GetTotalMemory(false) / 1000000f;
            }

            _fpsFrameCounter++;
            
            Metrics.Reset();
        }
    }
}