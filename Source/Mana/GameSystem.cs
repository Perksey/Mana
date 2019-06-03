using Mana.Graphics;

namespace Mana
{
    public abstract class GameSystem
    {
        public abstract void OnAddedToGame(Game game);
        
        public abstract void EarlyUpdate(float time, float deltaTime);
        public abstract void LateUpdate(float time, float deltaTime);
        
        public abstract void EarlyRender(float time, float deltaTime, RenderContext renderContext);
        public abstract void LateRender(float time, float deltaTime, RenderContext renderContext);
    }
}