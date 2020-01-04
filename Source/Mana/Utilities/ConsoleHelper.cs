using System;

namespace Mana.Utilities
{
    public static class ConsoleHelper
    {
        public static void Write(string message, ConsoleColor foregroundColor)
        {
            ConsoleColor oldForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;

            Console.Write(message);

            Console.ForegroundColor = oldForegroundColor;
        }

        public static void Write(string message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            ConsoleColor oldForegroundColor = Console.ForegroundColor;
            ConsoleColor oldBackgroundColor = Console.BackgroundColor;

            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;

            Console.Write(message);

            Console.BackgroundColor = oldBackgroundColor;
            Console.ForegroundColor = oldForegroundColor;
        }

        public static void WriteLine(string message, ConsoleColor foregroundColor)
        {
            ConsoleColor oldForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(message);

            Console.ForegroundColor = oldForegroundColor;
        }

        public static void WriteLine(string message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            ConsoleColor oldForegroundColor = Console.ForegroundColor;
            ConsoleColor oldBackgroundColor = Console.BackgroundColor;

            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;

            Console.WriteLine(message);

            Console.BackgroundColor = oldBackgroundColor;
            Console.ForegroundColor = oldForegroundColor;
        }
    }
}
