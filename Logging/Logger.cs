﻿using DebloaterTool.Settings;
using System;
using System.IO;
using System.Collections.Generic;

namespace DebloaterTool.Logging
{
    public static class Logger
    {
        public static void Log(string message, Level level = Level.INFO, bool Save = true, bool Return = false)
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
                if (Return) Console.Write("\r" + linePrefix + wrappedLines[i]);
                else Console.WriteLine(linePrefix + wrappedLines[i]);
            }
            Console.ResetColor();

            // Write log to file (only if not progress log)
            if (Save) WriteLogToFile(timestamp + message);
        }

        private static void WriteLogToFile(string logEntry)
        {
            try
            {
                // Ensure the directory exists
                string directory = Path.GetDirectoryName(Global.LogFilePath);
                Directory.CreateDirectory(directory);

                if (!File.Exists(Global.LogFilePath))
                {
                    // Create file with header
                    File.WriteAllText(Global.LogFilePath, string.Join(Environment.NewLine, Global.Logo) + Environment.NewLine);
                }

                // Append the log entry
                File.AppendAllText(Global.LogFilePath, logEntry + Environment.NewLine);
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

        public static ConsoleColor GetColor(this Level level)
        {
            return LevelColors.Colors.TryGetValue(level, out var color) ? color : ConsoleColor.White;
        }
    }
}
