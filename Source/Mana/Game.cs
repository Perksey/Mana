using System.Collections.Generic;
using Mana.Asset;
using Mana.Graphics;

namespace Mana
{
    public abstract class Game
    {
        public List<GameSystem> GameSystems { get; } = new List<GameSystem>();

        protected Game()
        {
        }
        
        public ResourceManager ResourceManager { get; protected set; }

        public RenderContext RenderContext { get; protected set; }
        
        public AssetManager AssetManager { get; protected set; }
        
        public ManaWindow Window { get; protected set; }
        
        internal void UpdateBase(float time, float deltaTime)
        {
            for (int i = 0; i < GameSystems.Count; i++)
                GameSystems[i].EarlyUpdate(time, deltaTime);

            Update(time, deltaTime);
            
            for (int i = 0; i < GameSystems.Count; i++)
                GameSystems[i].LateUpdate(time, deltaTime);
        }

        internal void RenderBase(float time, float deltaTime)
        {
            for (int i = 0; i < GameSystems.Count; i++)
                GameSystems[i].EarlyRender(time, deltaTime, RenderContext);

            Render(time, deltaTime);
            
            for (int i = 0; i < GameSystems.Count; i++)
                GameSystems[i].LateRender(time, deltaTime, RenderContext);
        }

        internal void OnRun(ManaWindow manaWindow)
        {
            Window = manaWindow;
            
            ResourceManager = manaWindow.ResourceManager;
            RenderContext = ResourceManager.MainContext;
            AssetManager = new AssetManager(ResourceManager);
            
            Initialize();
        }

        protected void AddSystem(GameSystem system)
        {
            GameSystems.Add(system);
            system.OnAddedToGame(this);
        }

        protected abstract void Initialize();
        protected abstract void Update(float time, float deltaTime);
        protected abstract void Render(float time, float deltaTime);
    }
}