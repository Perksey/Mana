using Mana.Graphics;

namespace Mana.Samples.Basic
{
    public class SampleGame : Game
    {
        protected override void Initialize()
        {
            this.Window.Title = "Hello";
        }

        protected override void Update(float time, float deltaTime)
        {
        }

        protected override void Render(float time, float deltaTime)
        {
            GraphicsDevice.Clear(Color.Cyan);
        }
    }
}