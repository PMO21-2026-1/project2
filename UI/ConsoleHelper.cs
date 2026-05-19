using System;

namespace DentalClinic.UI
{
    public static class ConsoleHelper
    {
        public static void PrintHeader(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"╔══════════════════════════════════════════╗");
            Console.WriteLine($"  {title}");
            Console.WriteLine($"╚══════════════════════════════════════════╝");
        }

        public static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✓ {message}");
            Console.ResetColor();
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✗ {message}");
            Console.ResetColor();
        }

        public static void PrintInfo(string message)
        {
            Console.WriteLine($"  {message}");
        }

        public static string ReadLine(string prompt)
        {
            Console.Write($"  {prompt}: ");
            return Console.ReadLine()?.Trim() ?? "";
        }

        public static int ReadInt(string prompt, int defaultValue = 0)
        {
            Console.Write($"  {prompt}: ");
            var input = Console.ReadLine()?.Trim();
            return int.TryParse(input, out var val) ? val : defaultValue;
        }

        public static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("  Натисніть будь-яку клавішу...");
            Console.ReadKey(true);
        }
    }
}
