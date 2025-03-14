﻿using System;
using System.Collections.Generic;
using System.IO;

namespace DebloaterTool
{
    public static class Logger
    {
        // Log file path (same directory as the executable)
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebloaterTool.log");

        public static void Log(string message, Level level = Level.INFO, bool isProgress = false)
        {
            Console.ForegroundColor = GetColor(level);

            int levelWidth = 10; // Fixed width for Level, e.g., "[INFO]  " or "[WARNING]"
            string levelText = $"[{level}]".PadRight(levelWidth); // Ensures fixed width

            string timestamp = $"[{DateTime.Now:yyyy.MM.dd HH:mm:ss}] {levelText} - ";
            int consoleWidth = Console.WindowWidth;
            int availableWidth = consoleWidth - timestamp.Length - 1; // Subtract 1 for the vertical scrollbar

            // Wrap text by fixed character count
            List<string> wrappedLines = WrapText(message, availableWidth);
            string padding = new string(' ', timestamp.Length);

            // Print first line with timestamp; subsequent lines with padding.
            for (int i = 0; i < wrappedLines.Count; i++)
            {
                string linePrefix = (i == 0) ? timestamp : padding;
                if (isProgress) Console.Write("\r" + linePrefix + wrappedLines[i]);
                else Console.WriteLine(linePrefix + wrappedLines[i]);
            }
            Console.ResetColor();

            // Write log to file (only if not progress log)
            if (!isProgress) WriteLogToFile(timestamp + message);
        }

        private static void WriteLogToFile(string logEntry)
        {
            try
            {
                File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }

        // Splits text into substrings of maxWidth characters.
        private static List<string> WrapText(string text, int maxWidth)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(text))
            {
                lines.Add(string.Empty);
                return lines;
            }

            for (int i = 0; i < text.Length; i += maxWidth)
            {
                int length = Math.Min(maxWidth, text.Length - i);
                lines.Add(text.Substring(i, length));
            }
            return lines;
        }

        public static ConsoleColor GetColor(Level level)
        {
            ConsoleColor color;
            return LevelHelper.LevelColors.TryGetValue(level, out color) ? color : ConsoleColor.White;
        }
    }
}
