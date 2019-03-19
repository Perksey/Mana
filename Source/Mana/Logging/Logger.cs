using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Mana.Logging
{
    public class Logger
    {
        private static Dictionary<LogLevel, ConsoleColor> _backgroundColors = new Dictionary<LogLevel, ConsoleColor>
        {
            [LogLevel.Debug] = ConsoleColor.Black,
            [LogLevel.Info] = ConsoleColor.Black,
            [LogLevel.Warn] = ConsoleColor.Black,
            [LogLevel.Error] = ConsoleColor.Black,
            [LogLevel.Fatal] = ConsoleColor.Black,
        };

        private static Dictionary<LogLevel, ConsoleColor> _foregroundColors = new Dictionary<LogLevel, ConsoleColor>
        {
            [LogLevel.Debug] = ConsoleColor.Cyan,
            [LogLevel.Info] = ConsoleColor.White,
            [LogLevel.Warn] = ConsoleColor.Yellow,
            [LogLevel.Error] = ConsoleColor.Red,
            [LogLevel.Fatal] = ConsoleColor.Magenta,
        };

        private static Dictionary<LogLevel, string> _displayNames = new Dictionary<LogLevel, string>()
        {
            [LogLevel.Debug] = "DEBUG",
            [LogLevel.Info]  = "INFO ",
            [LogLevel.Warn]  = "WARN ",
            [LogLevel.Error] = "ERROR",
            [LogLevel.Fatal] = "FATAL",
        };
        
        private string _name;

        public Logger(string name)
        {
            _name = name;
        }

        public static Logger Create([CallerFilePath] string callerFilePath = "")
        {
            return new Logger(Path.GetFileNameWithoutExtension(callerFilePath));
        }

        public void Debug(string message)
        {
            LogMessage(message, LogLevel.Debug);
        }

        public void Info(string message)
        {
            LogMessage(message, LogLevel.Info);
        }

        public void Warn(string message)
        {
            LogMessage(message, LogLevel.Warn);
        }

        public void Error(string message)
        {
            LogMessage(message, LogLevel.Error);
        }

        public void Fatal(string message)
        {
            LogMessage(message, LogLevel.Fatal);
        }

        public void LogMessage(string message, LogLevel logLevel)
        {
            Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), ConsoleColor.Gray);
            Console.Write(" - ");
            Write(_name, ConsoleColor.White);
            Console.Write(" - ");
            Write($"{_displayNames[logLevel]}", _backgroundColors[logLevel], _foregroundColors[logLevel]);
            Console.Write(" - ");
            Write(message, _backgroundColors[logLevel], _foregroundColors[logLevel]);
            Console.WriteLine();
        }
        
        #region Formatting Helper Methods

        private static void Write(string message, 
                                  ConsoleColor backgroundColor, 
                                  ConsoleColor foregroundColor)
        {
            ConsoleColor oldForeground = Console.ForegroundColor;
            ConsoleColor oldBackground = Console.BackgroundColor;

            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;

            Console.Write(message);

            Console.ForegroundColor = oldForeground;
            Console.BackgroundColor = oldBackground;
        }
        
        private static void Write(string message, 
                                  ConsoleColor foregroundColor)
        {
            ConsoleColor oldForeground = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;

            Console.Write(message);

            Console.ForegroundColor = oldForeground;
        }
        
        #endregion
    }
}