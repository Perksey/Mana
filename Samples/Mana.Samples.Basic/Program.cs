using Mana.Graphics;

namespace Mana.Samples.Basic
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            using (SampleGame game = new SampleGame())
            {
                using (OpenTKWindow window = new OpenTKWindow())
                {
                    window.Run(game);
                }
            }
        }
    }
}