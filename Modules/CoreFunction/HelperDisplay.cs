using System;

namespace DebloaterTool
{
    internal class HelperDisplay
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
                Console.Write($"{message} (y/n): ");
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.Y)
                {
                    Console.WriteLine(); // Only move to the next line if input is valid
                    return true;
                }

                if (keyInfo.Key == ConsoleKey.N)
                {
                    Console.WriteLine(); // Only move to the next line if input is valid
                    return false;
                }

                Console.WriteLine("\nInvalid input. Please press 'y' or 'n'.");
            }
        }
    }
}
