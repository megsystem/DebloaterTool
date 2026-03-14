using DebloaterTool.Properties;
using System;
using System.IO;
using System.Reflection;

namespace DebloaterTool.Settings
{
    internal class Global
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

        // Default folder
        public static readonly string InstallPath = @"C:\DebloaterTool";

        // Log file path
        public static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "debloatertool.saved"
        );

        // Version Program
        static readonly Version version = Assembly.GetExecutingAssembly().GetName().Version;
        public static readonly string Version = $"V{version.Major}.{version.Minor}.{version.Build}";

        // If you fork this repo, remember to update the raw URL to your fork
        static string githubRawUrl = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main";

        // Downloads links
        public static string tabLink = "https://megsystem.github.io/materialYouNewTab/"; // forked - original by XengShi
        public static string wallpaper = $"{githubRawUrl}/External/Wallpapers";
        public static string powerRun = $"{githubRawUrl}/External/Dependencies/PowerRun.exe";
        public static string bootlogo = $"{githubRawUrl}/External/Dependencies/bootlogo.zip";
        public static string bordertheme = $"{githubRawUrl}/External/Dependencies/tacky-borders.exe";
        public static string explorertheme = $"{githubRawUrl}/External/Dependencies/ExplorerTheme.zip";
        public static string alwaysontop = $"{githubRawUrl}/External/Dependencies/AlwaysOnTop.exe";
        public static string defender = $"{githubRawUrl}/External/Dependencies/defender.reg";
        public static string takeownershipmenu = $"{githubRawUrl}/External/Dependencies/TakeOwnershipMenu.reg";
        public static string sevenzipscript = $"{githubRawUrl}/External/Dependencies/Install7Zip.ps1";
        public static string dwmblurglass = $"{githubRawUrl}/External/Dependencies/DWMBlurGlass.zip";

        // Updater
        public static string lastversionurl = $"{githubRawUrl}/DebloaterTool.exe";
        public static string updaterbat = $"{githubRawUrl}/External/Dependencies/apply_update.bat";

        // External from this repo
        public static string windhawkinstaller = "https://filehosting.urlshorter.it/Windhawk";
        public static string windowsactivator = "https://filehosting.urlshorter.it/Activator";
        public static string startallback = "https://filehosting.urlshorter.it/StartAll"; // not original version, modded

        // Debloaters Settings
        public static string christitusUrl = "https://christitus.com/win";
        public static string raphiToolUrl = "https://debloat.raphi.re/";
        public static byte[] config = Resources.config;

        // Resources files
        public static string welcome = Resources.welcome;

        // Paths vars
        public static string logsPath = $@"{InstallPath}\Saved Logs";
        public static string debloatersPath = $@"{InstallPath}\Debloaters";
        public static string wallpapersPath = $@"{InstallPath}\Wallpapers";
        public static string themePath = $@"{InstallPath}\WinTheme";
        public static string bootlogoPath = $@"{InstallPath}\Bootlogo";
        public static string configFilePath = $@"{InstallPath}\config.json";
    }
}
