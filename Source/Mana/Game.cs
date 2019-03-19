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
        }

        public IGameWindow Window { get; private set; }
        
        public GraphicsDevice GraphicsDevice { get; private set; }

        public AssetManager AssetManager { get; private set; }

        public void Dispose()
        {
        }

        /// <summary>
        /// The base function that calls the Game's main Initialize function.
        /// </summary>
        public void InitializeBase(IGameWindow window)
        {
            Window = window;
            GraphicsDevice = window.GraphicsDevice;
            AssetManager = new AssetManager(GraphicsDevice);
            
            Initialize();
        }

        /// <summary>
        /// The base function that calls the Game's main Update function.  
        /// </summary>
        /// <param name="time">The time, in seconds, since the game was launched.</param>
        /// <param name="deltaTime">The time, in seconds, since the last frame.</param>
        public void UpdateBase(float time, float deltaTime)
        {
            Update(time, deltaTime);
        }

        /// <summary>
        /// The base function that calls the Game's main Render function.
        /// </summary>
        /// <param name="time">The time, in seconds, since the game was launched.</param>
        /// <param name="deltaTime">The time, in seconds, since the last frame.</param>
        public void RenderBase(float time, float deltaTime)
        {
            Render(time, deltaTime);
        }

        protected abstract void Initialize();
        protected abstract void Update(float time, float deltaTime);
        protected abstract void Render(float time, float deltaTime);
    }
}