using System;
using Mana.Graphics;
using Mana.Logging;

namespace Mana.Samples.Basic
{
    internal static class Program
    {
        private static Logger _log = Logger.Create();
        
        [STAThread]
        public static void Main(string[] args)
        {
            _log.Info("Start of program.");
            
            using (OpenTKWindow window = new OpenTKWindow())
            {
                // try
                // {
                window.Run(new SampleGame());
                // }
                // catch (Exception e)
                // {
                //     _log.Fatal($"{e.Message}{Environment.NewLine}{e.StackTrace}");
                //     _log.Fatal("Press any key to continue...");
                //     Console.ReadKey();
                // }
            }
            
            GC.Collect();
        }
    }
}