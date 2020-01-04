using System;
using System.Collections.Generic;
using System.Drawing;
using Mana.Asset;
using Mana.Graphics;
using Mana.Utilities;

namespace Mana
{
    public abstract class Game : IDisposable
    {
        private List<IGameSystem> _gameSystems = new List<IGameSystem>(4);

        protected Game()
        {
        }

        public IGameHost Window { get; private set; }

        public ServiceContainer Services { get; } = new ServiceContainer();

        public RenderContext RenderContext { get; private set; }

        public AssetManager AssetManager { get; private set; }

        public bool Disposed { get; private set; }

        public abstract void Initialize();

        public abstract void Update(float time, float deltaTime);

        public abstract void Render(float time, float deltaTime);

        public void Dispose()
        {
            if (Disposed)
            {
                return;
            }

            Dispose(true);
            GC.SuppressFinalize(this);

            Disposed = true;
        }

        /// <summary>
        /// Creates a <see cref="ManaWindow"/> and runs the game within it.
        /// </summary>
        public void Run()
        {
            using var window = new ManaWindow();
            Window = window;
            window.Run(this);
        }

        public void AddGameSystem(IGameSystem gameSystem)
        {
            _gameSystems.Add(gameSystem);
            gameSystem.OnAddedToGame(this);
        }

        internal void UpdateBase(float time, float deltaTime)
        {
            AssetManager.Update();

            foreach (var system in _gameSystems)
            {
                system.EarlyUpdate(time, deltaTime);
            }

            Update(time, deltaTime);

            foreach (var system in _gameSystems)
            {
                system.LateUpdate(time, deltaTime);
            }
        }

        internal void RenderBase(float time, float deltaTime)
        {
            foreach (var system in _gameSystems)
            {
                system.EarlyRender(time, deltaTime, RenderContext);
            }

            Render(time, deltaTime);

            foreach (var system in _gameSystems)
            {
                system.LateRender(time, deltaTime, RenderContext);
            }
        }

        internal void OnBeforeRun(IGameHost host)
        {
            if (Disposed)
                throw new InvalidOperationException("Cannot call OnBeforeRun on a disposed Game.");

            if (host == null)
                throw new ArgumentNullException(nameof(host));

            RenderContext = host.RenderContext ?? throw new ArgumentException("host's RenderContext may not be null", nameof(host));
            Services.AddService(RenderContext);

            AssetManager = new AssetManager(RenderContext);
            Services.AddService(AssetManager);

            Initialize();

            RenderContext.ViewportRectangle = new Rectangle(0, 0, host.Width, host.Height);
        }

        protected virtual void Dispose(bool disposing)
        {
            AssetManager.Dispose();
        }
    }
}
