using System;
using System.Collections.Generic;

namespace DebloaterTool
{
    public enum Level
    {
        INFO,
        WARNING,
        VERBOSE,
        ERROR,
        CRITICAL,
        SUCCESS,
        ALERT,
        DOWNLOAD
    }

    internal static class LevelHelper
    {
        public static readonly Dictionary<Level, ConsoleColor> LevelColors = new Dictionary<Level, ConsoleColor>
        {
            { Level.INFO, ConsoleColor.DarkCyan },
            { Level.SUCCESS, ConsoleColor.DarkGreen },
            { Level.WARNING, ConsoleColor.DarkYellow },
            { Level.VERBOSE, ConsoleColor.Magenta },
            { Level.ERROR, ConsoleColor.Red },
            { Level.CRITICAL, ConsoleColor.DarkRed },
            { Level.ALERT, ConsoleColor.Yellow },
            { Level.DOWNLOAD, ConsoleColor.Magenta }
        };
    }
}