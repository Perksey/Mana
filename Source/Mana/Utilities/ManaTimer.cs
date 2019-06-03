using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Mana.Utilities
{
    public class ManaTimer
    {
        private static Logger _log = new Logger("Stopwatch");
        private Stopwatch _stopwatch;
        private string _name;
        
        public ManaTimer(string name)
        {
            _name = name;
            _stopwatch = new Stopwatch();
        }

        public static ManaTimer StartNew([CallerFilePath] string callerFilePath = "")
        {
            var sw = new ManaTimer("Timer:" + Path.GetFileNameWithoutExtension(callerFilePath));
            sw.Start();
            return sw;
        }
        
        public void Start()
        {
            _stopwatch.Start();
        }

        public void Tally(string message)
        {
            _stopwatch.Stop();
            _log.Info($"{message} at {_stopwatch.Elapsed.TotalMilliseconds} ms");
            _stopwatch.Restart();
        }

        public void Restart()
        {
            _stopwatch.Restart();
        }
    }
}