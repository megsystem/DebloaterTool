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

        // Default folder
        public static readonly string InstallPath = @"C:\DebloaterTool";

        // Log file path
        public static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DebloaterToolSaved.log"
        );

        // Version Program
        static readonly Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public static readonly string Version = $"V{version.Major}.{version.Minor}.{version.Build}";

        // Downloads links - fork and put your urls
        public static string tabLink = "https://megsystem.github.io/materialYouNewTab/"; // forked - original by XengShi
        public static string wallpaper = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Wallpapers";
        public static string powerRun = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/PowerRun.exe";
        public static string bootlogo = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/bootlogo.zip";
        public static string bordertheme = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/tacky-borders.exe";
        public static string explorertheme = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/ExplorerTheme.zip";

        // RaphiTool Settings
        public static string raphiToolUrl = "https://win11debloat.raphi.re/";
        public static string raphiToolArgs = "-Silent " +
            "-RemoveApps -RemoveAppsCustom -RemoveGamingApps -RemoveCommApps -RemoveDevApps -RemoveW11Outlook -ForceRemoveEdge " +
            "-DisableDVR -DisableTelemetry -DisableBingSearches -DisableBing -DisableDesktopSpotlight -DisableLockscrTips -DisableLockscreenTips " +
            "-DisableWindowsSuggestions -DisableSuggestions -ShowKnownFileExt -HideDupliDrive -TaskbarAlignLeft -HideSearchTb " +
            "-HideTaskview -DisableStartRecommended -DisableCopilot -DisableRecall -DisableWidgets -HideWidgets -DisableChat -HideChat " +
            "-EnableEndTask -ClearStart -ClearStartAllUsers -RevertContextMenu -DisableMouseAcceleration -DisableStickyKeys " +
            "-ExplorerToThisPC -DisableOnedrive -HideOnedrive ";

        // ChrisTool Settings
        public static string christitusUrl = "christitus.com/win";
        public static byte[] christitusConfig = Config.Resource.christitus;

        // DefenderReg Settings
        public static string defender = Config.Resource.defender;

        // Paths vars
        public static string logsPath = $@"{InstallPath}\Saved Logs";
        public static string debloatersPath = $@"{InstallPath}\Debloaters";
        public static string wallpapersPath = $@"{InstallPath}\Wallpapers";
        public static string themePath = $@"{InstallPath}\WinTheme";
        public static string bootlogoPath = $@"{InstallPath}\Bootlogo";
    }
}
