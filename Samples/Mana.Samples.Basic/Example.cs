using Mana.Asset;
using Mana.Graphics;

namespace Mana.Samples.Basic
{
    public abstract class Example
    {
        protected SampleGame Game { get; }

        protected Example(SampleGame game)
        {
            Game = game;
        }
        
        protected GraphicsDevice GraphicsDevice => Game.GraphicsDevice;
        protected AssetManager AssetManager => Game.AssetManager;
        
        public abstract void Initialize();
        public abstract void Dispose();
        
        public abstract void Update(float time, float deltaTime);
        public abstract void Render(float time, float deltaTime);
    }
}