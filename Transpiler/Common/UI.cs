using System;

namespace Transpiler
{
    public static class UI
    {
        public const ConsoleColor DefaultForeground = ConsoleColor.Gray;
        public const ConsoleColor DefaultBackground = ConsoleColor.Black;

        public static bool Debug { get; set; }

        public static void Init()
        {
            Console.ForegroundColor = DefaultForeground;
            Console.BackgroundColor = DefaultBackground;
        }

        public static void DebugPrLn(object x)
        {
            if (Debug)
            {
                Console.WriteLine(x.ToString());
            }
        }

        public static void Pr(string text, params object[] args)
        {
            Console.Write(text, args);
        }

        public static void PrX(string text, int x, int y,
                               ConsoleColor? foreground = null,
                               ConsoleColor? background = null)
        {
            Console.SetCursorPosition(x, y);
            if (foreground.HasValue) { Console.ForegroundColor = foreground.Value; }
            if (background.HasValue) { Console.BackgroundColor = background.Value; }

            Console.Write(text);

            if (foreground.HasValue) { Console.ForegroundColor = DefaultForeground; }
            if (background.HasValue) { Console.BackgroundColor = DefaultBackground; }
        }

        public static void PrLn(string text, params object[] args)
        {
            Console.WriteLine(text, args);
        }

        public static void PrLnError(string text, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text, args);
            Console.ForegroundColor = UI.DefaultForeground;
        }
    }
}
