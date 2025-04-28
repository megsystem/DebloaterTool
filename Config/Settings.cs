using System.IO;
using System;

namespace DebloaterTool
{
    internal class Settings
    {
        // Logo
        public static string[] Logo = new string[]
        {
            @"+====================================================================================+",
            @"|  ________     ______ ______            _____            ________           ______  |",
            @"|  ___  __ \_______  /____  /___________ __  /_______________  __/______________  /  |",
            @"|  __  / / /  _ \_  __ \_  /_  __ \  __ `/  __/  _ \_  ___/_  /  _  __ \  __ \_  /   |",
            @"|  _  /_/ //  __/  /_/ /  / / /_/ / /_/ // /_ /  __/  /   _  /   / /_/ / /_/ /  /    |",
            @"|  /_____/ \___//_.___//_/  \____/\__,_/ \__/ \___//_/    /_/    \____/\____//_/     |",
            @"|                                                                                    |",
            @"+====================================================================================+",
        };

        // Log file path (same directory as the executable)
        public static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebloaterTool.log");

        // Default folder
        public static readonly string InstallPath = @"C:\DebloaterTool";

        // Version Program
        static readonly Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public static readonly string Version = $"V{version.Major}.{version.Minor}.{version.Build}";

        // Downloads links - fork and put your urls
        public static string christitusUrl = "christitus.com/win";
        public static string raphiToolUrl = "https://win11debloat.raphi.re/";
        public static string tabLink = "https://megsystem.github.io/materialYouNewTab/"; // forked - original by XengShi
        public static string wallpaper = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Wallpapers";
        public static string powerRun = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/PowerRun.exe";
        public static string bootlogo = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/bootlogo.zip";
        public static string bordertheme = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/tacky-borders.exe";
        public static string explorertheme = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/ExplorerTheme.zip";
    }
}
