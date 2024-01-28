using System;

namespace MiniPC.Discord.Helpers
{
    public static class Log
    {
        private static readonly object _lock = new object();

        //Info
        public static void Info(string message)
        {
            lock (_lock)
            {
                System.Console.Write($"[{DateTime.Now}] ");

                System.Console.ForegroundColor = ConsoleColor.DarkCyan;

                System.Console.Write($"[Info] ");

                System.Console.ResetColor();

                System.Console.WriteLine($"{message}");
            }
        }

        public static void Warning(string message)
        {
            lock (_lock)
            {
                System.Console.Write($"[{DateTime.Now}] ");

                System.Console.ForegroundColor = ConsoleColor.DarkYellow;

                System.Console.Write($"[Warning] ");

                System.Console.ResetColor();

                System.Console.WriteLine($"{message}");
            }
        }

        public static void Error(string message)
        {
            lock (_lock)
            {
                System.Console.Write($"[{DateTime.Now}] ");

                System.Console.ForegroundColor = ConsoleColor.Red;

                System.Console.Write($"[Error] ");

                System.Console.ResetColor();

                System.Console.WriteLine($"{message}");
            }
        }
    }
}