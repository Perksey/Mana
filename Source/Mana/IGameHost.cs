using System.Numerics;
using Mana.Graphics;

namespace Mana
{
    public interface IGameHost
    {
        RenderContext RenderContext { get; }

        void Run(Game game);

        int Width { get; set; }

        int Height { get; set; }

        ref Matrix4x4 ProjectionMatrix { get; }
    }
}
