using System;
using System.Collections.Generic;

namespace DebloaterTool.Logging
{
    public enum Level
    {
        INFO,
        WARNING,
        VERBOSE,
        ERROR,
        CRITICAL,
        SUCCESS,
        DOWNLOAD,
        SKIP
    }

    public static class LevelColors
    {
        public static readonly Dictionary<Level, ConsoleColor> Colors = new Dictionary<Level, ConsoleColor>
        {
            { Level.INFO, ConsoleColor.DarkCyan },
            { Level.SUCCESS, ConsoleColor.DarkGreen },
            { Level.WARNING, ConsoleColor.DarkYellow },
            { Level.SKIP, ConsoleColor.DarkYellow },
            { Level.VERBOSE, ConsoleColor.Magenta },
            { Level.ERROR, ConsoleColor.Red },
            { Level.CRITICAL, ConsoleColor.DarkRed },
            { Level.DOWNLOAD, ConsoleColor.Magenta }
        };
    }
}