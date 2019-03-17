using System;
using Mana.Graphics;

namespace Mana.Asset
{
    public abstract class GraphicsAsset : ManaAsset, IGraphicsResource
    {
        private bool _disposed = false;
        
        protected GraphicsAsset(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            GraphicsDevice.Resources.Add(this);
        }

        public GraphicsDevice GraphicsDevice { get; }

        public sealed override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                GraphicsDevice.Resources.Remove(this);
            }

            _disposed = true;
        }
    }
}