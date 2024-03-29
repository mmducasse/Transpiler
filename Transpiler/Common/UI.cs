﻿using System;

namespace Transpiler
{
    public static class UI
    {
        public static ConsoleColor White => ConsoleColor.White;
        public static ConsoleColor Gray => ConsoleColor.DarkGray;
        public static ConsoleColor Black => ConsoleColor.Black;
        public static ConsoleColor Yellow => ConsoleColor.Yellow;
        public static ConsoleColor Blue => ConsoleColor.Blue;

        public static void Pr(string text, params object[] args)
        {
            Console.Write(text, args);
        }

        public static void Pr(string text,
                              ConsoleColor foregroundColor,
                              ConsoleColor backgroundColor = ConsoleColor.Black,
                              params object[] args)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(text, args);
            Console.ForegroundColor = White;
            Console.BackgroundColor = Black;
        }

        public static void PrLn() { Console.WriteLine(); }

        public static void PrLn(string text, params object[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine(text, args);
            }
            else
            {
                Console.WriteLine(text);
            }
        }

        public static void PrLn(string text,
                                ConsoleColor foregroundColor,
                                ConsoleColor backgroundColor = ConsoleColor.Black,
                                params object[] args)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine(text, args);
            Console.ForegroundColor = White;
            Console.BackgroundColor = Black;
        }

        public static void PrX(string text, int x, int y,
                               ConsoleColor? foreground = null,
                               ConsoleColor? background = null)
        {
            Console.SetCursorPosition(x, y);
            if (foreground.HasValue) { Console.ForegroundColor = foreground.Value; }
            if (background.HasValue) { Console.BackgroundColor = background.Value; }

            Console.Write(text);

            if (foreground.HasValue) { Console.ForegroundColor = White; }
            if (background.HasValue) { Console.BackgroundColor = Black; }
        }
    }
}
