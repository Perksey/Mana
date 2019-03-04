namespace Mana.Graphics
{
    public interface IGameWindow
    {
        Game Game { get; }
        GraphicsDevice GraphicsDevice { get; }
        int Width { get; set; }
        int Height { get; set; }
        float AspectRatio { get; }
        bool Fullscreen { get; set; }
        bool VSync { get; set; }
    }
}