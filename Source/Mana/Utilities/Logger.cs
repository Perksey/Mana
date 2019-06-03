using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Mana.Utilities
{
    public class Logger
    {
        private static readonly Dictionary<LogLevel, ConsoleColor> _backgroundColors = new Dictionary<LogLevel, ConsoleColor>
        {
            [LogLevel.Debug] = ConsoleColor.Black,
            [LogLevel.Info] = ConsoleColor.Black,
            [LogLevel.Warn] = ConsoleColor.Black,
            [LogLevel.Error] = ConsoleColor.Black,
            [LogLevel.Fatal] = ConsoleColor.Black,
        };

        public static readonly Dictionary<LogLevel, ConsoleColor> _foregroundColors = new Dictionary<LogLevel, ConsoleColor>
        {
            [LogLevel.Debug] = ConsoleColor.Cyan,
            [LogLevel.Info] = ConsoleColor.White,
            [LogLevel.Warn] = ConsoleColor.Yellow,
            [LogLevel.Error] = ConsoleColor.Red,
            [LogLevel.Fatal] = ConsoleColor.Magenta,
        };

        public static readonly Dictionary<LogLevel, string> _displayNames = new Dictionary<LogLevel, string>()
        {
            [LogLevel.Debug] = "DEBUG",
            [LogLevel.Info]  = "INFO",
            [LogLevel.Warn]  = "WARN",
            [LogLevel.Error] = "ERROR",
            [LogLevel.Fatal] = "FATAL",
        };
        
        private string _name;

        public static bool WriteTimestamps = false;

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
            if (WriteTimestamps)
            {
                ConsoleHelper.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), ConsoleColor.Gray);
                Console.Write(" - ");
            }

            string threadName = $"{(Thread.CurrentThread.Name ?? "Thread")}:{Thread.CurrentThread.ManagedThreadId.ToString()}";
            ConsoleHelper.Write($"[{threadName}]", ConsoleColor.Gray);
            Console.Write(" - ");
            ConsoleHelper.Write(_name, ConsoleColor.White);
            Console.Write(" - ");
            ConsoleHelper.Write($"{_displayNames[logLevel]}", _foregroundColors[logLevel], _backgroundColors[logLevel]);
            Console.Write(" - ");
            ConsoleHelper.Write(message, _foregroundColors[logLevel], _backgroundColors[logLevel]);
            Console.WriteLine();
        }
        
        public void LogMessage(string message, LogLevel logLevel, ConsoleColor foregroundColor)
        {
            if (WriteTimestamps)
            {
                ConsoleHelper.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), ConsoleColor.Gray);
                Console.Write(" - ");    
            }
            
            string threadName = $"{(Thread.CurrentThread.Name ?? "Thread")}:{Thread.CurrentThread.ManagedThreadId.ToString()}";
            ConsoleHelper.Write($"[{threadName}]", ConsoleColor.Gray);
            Console.Write(" - ");
            ConsoleHelper.Write(_name, ConsoleColor.White);
            Console.Write(" - ");
            ConsoleHelper.Write($"{_displayNames[logLevel]}", foregroundColor);
            Console.Write(" - ");
            ConsoleHelper.Write(message, foregroundColor);
            Console.WriteLine();
        }
        
        public void WriteLine(string message, ConsoleColor foregroundColor)
        {
            if (WriteTimestamps)
            {
                ConsoleHelper.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), ConsoleColor.Gray);
                Console.Write(" - ");    
            }
            
            string threadName = $"{(Thread.CurrentThread.Name ?? "Thread")}:{Thread.CurrentThread.ManagedThreadId.ToString()}";
            ConsoleHelper.Write($"[{threadName}]", ConsoleColor.Gray);
            Console.Write(" - ");
            ConsoleHelper.Write(_name, ConsoleColor.White);
            Console.Write(" - ");
            ConsoleHelper.Write(message, foregroundColor);
            Console.WriteLine();
        }
        
        public enum LogLevel
        {
            Debug,
            Info,
            Warn,
            Error,
            Fatal,
        }
    }
}