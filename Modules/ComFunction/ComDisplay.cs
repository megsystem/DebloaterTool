using System;

namespace DebloaterTool
{
    internal class ComDisplay
    {
        public static void DisplayMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static bool RequestYesOrNo(string message)
        {
            while (true)
            {
                Console.Write($"{message} (yes/no): ");
                string response = Console.ReadLine()?.Trim().ToLower();

                if (response == "yes") return true;
                if (response == "no") return false;

                Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
            }
        }
    }
}
