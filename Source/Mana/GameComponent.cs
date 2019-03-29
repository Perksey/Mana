using Mana.Graphics;

namespace Mana
{
    public class GameComponent
    {
        public bool Enabled = true;
        
        public bool Visible = true;

        public Game Game { get; private set; }

        public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

        public virtual void OnAddedToGame(Game game)
        {
            Game = game;
        }

        public virtual void OnRemovedFromGame(Game game)
        {
        }

        public virtual void EarlyUpdate(float time, float deltaTime)
        {
        }

        public virtual void Update(float time, float deltaTime)
        {
        }

        public virtual void LateUpdate(float time, float deltaTime)
        {
        }

        public virtual void EarlyRender(float time, float deltaTime)
        {
        }

        public virtual void Render(float time, float deltaTime)
        {
        }

        public virtual void LateRender(float time, float deltaTime)
        {
        }
    }
}