using System;
using System.Linq;

namespace Boerman.Core.Underscore
{
    public partial class _
    {
        public static class Console
        {
            public static void Write(string text, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
            {
                if (foregroundColor.HasValue) System.Console.ForegroundColor = foregroundColor.Value;
                if (backgroundColor.HasValue) System.Console.BackgroundColor = backgroundColor.Value;
                System.Console.Write(text);
                System.Console.ResetColor();
            }

            public static void WriteLine(string text, ConsoleColor? foregroundColor = null,
                ConsoleColor? backgroundColor = null, bool writeToEnd = true)
            {
                if (foregroundColor.HasValue) System.Console.ForegroundColor = foregroundColor.Value;
                if (backgroundColor.HasValue) System.Console.BackgroundColor = backgroundColor.Value;


                if (writeToEnd) { 
                    var width = System.Console.WindowWidth;
                    var length = text.Length;

                    Enumerable.Range(0, width - length % width).ToList().ForEach(arg => text += " ");
                }

                System.Console.WriteLine(text);


                System.Console.ResetColor();
            }
        }
    }
}
