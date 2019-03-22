using System;
using Mana.Asset;
using Mana.Graphics;
using Mana.Graphics.Vertex;

namespace Mana
{
    public abstract class Game : IDisposable
    {
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
            Components.EarlyUpdate(time, deltaTime);
            
            Update(time, deltaTime);

            Components.Update(time, deltaTime);

            Components.LateUpdate(time, deltaTime);

        }

        /// <summary>
        /// The base function that calls the Game's main Render function.
        /// </summary>
        /// <param name="time">The time, in seconds, since the game was launched.</param>
        /// <param name="deltaTime">The time, in seconds, since the last frame.</param>
        public void RenderBase(float time, float deltaTime)
        {
            Components.EarlyRender(time, deltaTime);
            
            Render(time, deltaTime);
            
            Components.Render(time, deltaTime);

            Components.LateRender(time, deltaTime);

            Input.Update();
            // TODO: Update Metrics Here
        }

        protected abstract void Initialize();
        protected abstract void Update(float time, float deltaTime);
        protected abstract void Render(float time, float deltaTime);
    }
}