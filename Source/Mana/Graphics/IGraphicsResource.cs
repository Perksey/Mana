using System;

namespace Mana.Graphics
{
    public interface IGraphicsResource : IDisposable
    {
        GraphicsDevice GraphicsDevice { get; }
    }
}