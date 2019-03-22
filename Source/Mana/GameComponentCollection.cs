using System.Collections.Generic;

namespace Mana
{
    public class GameComponentCollection
    {
        private List<GameComponent> _components = new List<GameComponent>();

        public GameComponentCollection(Game game)
        {
            Game = game;
        }
        
        public Game Game { get; }

        public void Add(GameComponent component)
        {
            _components.Add(component);
            component.OnAddedToGame(Game);
        }

        public bool Remove(GameComponent component)
        {
            bool result = _components.Remove(component);
            component.OnRemovedFromGame(Game);
            return result;
        }

        public T Get<T>()
            where T : GameComponent
        {
            foreach (GameComponent component in _components)
            {
                if (component is T typedComponent)
                    return typedComponent;
            }

            return null;
        }

        public void EarlyUpdate(float time, float deltaTime)
        {
            for (int i = 0; i < _components.Count; i++)
                if (_components[i].Enabled)
                    _components[i].EarlyUpdate(time, deltaTime);
        }

        public void Update(float time, float deltaTime)
        {
            for (int i = 0; i < _components.Count; i++)
                if (_components[i].Enabled)
                    _components[i].Update(time, deltaTime);
        }

        public void LateUpdate(float time, float deltaTime)
        {
            for (int i = 0; i < _components.Count; i++)
                if (_components[i].Enabled)
                    _components[i].LateUpdate(time, deltaTime);
        }

        public void EarlyRender(float time, float deltaTime)
        {
            for (int i = 0; i < _components.Count; i++)
                if (_components[i].Visible)
                    _components[i].EarlyRender(time, deltaTime);
        }

        public void Render(float time, float deltaTime)
        {
            for (int i = 0; i < _components.Count; i++)
                if (_components[i].Visible)
                    _components[i].Render(time, deltaTime);
        }

        public void LateRender(float time, float deltaTime)
        {
            for (int i = 0; i < _components.Count; i++)
                if (_components[i].Visible)
                    _components[i].LateRender(time, deltaTime);
        }
    }
}