using System;
using Mana.Graphics;

namespace Mana.Samples.Basic
{
    internal static class Program
    {
        [STAThread]
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