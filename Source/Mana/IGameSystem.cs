using Mana.Graphics;

namespace Mana
{
    public interface IGameSystem
    {
        void OnAddedToGame(Game game);

        void EarlyUpdate(float time, float deltaTime);
        void LateUpdate(float time, float deltaTime);

        void EarlyRender(float time, float deltaTime, RenderContext renderContext);
        void LateRender(float time, float deltaTime, RenderContext renderContext);
    }
}
