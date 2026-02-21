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

        // Downloads links - fork and put your urls
        public static string tabLink = "https://megsystem.github.io/materialYouNewTab/"; // forked - original by XengShi
        public static string wallpaper = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Wallpapers";
        public static string powerRun = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Dependencies/PowerRun.exe";
        public static string bootlogo = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Dependencies/bootlogo.zip";
        public static string bordertheme = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Dependencies/tacky-borders.exe";
        public static string explorertheme = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Dependencies/ExplorerTheme.zip";
        public static string alwaysontop = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Dependencies/AlwaysOnTop.exe";
        public static string defender = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Dependencies/defender.reg";
        public static string takeownershipmenu = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Dependencies/TakeOwnershipMenu.reg";
        public static string sevenzipscript = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Dependencies/Install7Zip.ps1";
        public static string windhawkinstaller = "https://ramensoftware.com/downloads/windhawk_setup.exe";
        public static string windowsactivator = "https://raw.githubusercontent.com/massgravel/Microsoft-Activation-Scripts/refs/heads/master/MAS/Separate-Files-Version/Activators/HWID_Activation.cmd";
        public static string startallback = "https://filehosting.urlshorter.it/installer.exe"; // modded dll for skip activation (sorry)
        public static string dwmblurglass = "https://github.com/Maplespe/DWMBlurGlass/releases/download/2.3.1r/DWMBlurGlass_2.3.1_x64.zip";

        // Updater
        public static string lastversionurl = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/DebloaterTool.exe";
        public static string updaterbat = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/Dependencies/apply_update.bat";

        // Debloaters Settings
        public static string christitusUrl = "https://christitus.com/win";
        public static string raphiToolUrl = "https://win11debloat.raphi.re/";
        public static byte[] christitusConfig = Resources.christitus;
        public static string raphiToolArgs = "-Silent " +
            "-RemoveApps -RemoveAppsCustom -RemoveGamingApps -RemoveCommApps -RemoveDevApps -RemoveW11Outlook -ForceRemoveEdge " +
            "-DisableDVR -DisableTelemetry -DisableBingSearches -DisableBing -DisableDesktopSpotlight -DisableLockscrTips -DisableLockscreenTips " +
            "-DisableWindowsSuggestions -DisableSuggestions -ShowKnownFileExt -HideDupliDrive -TaskbarAlignLeft -HideSearchTb " +
            "-HideTaskview -DisableStartRecommended -DisableCopilot -DisableRecall -DisableWidgets -HideWidgets -DisableChat -HideChat " +
            "-EnableEndTask -ClearStart -ClearStartAllUsers -RevertContextMenu -DisableMouseAcceleration -DisableStickyKeys " +
            "-ExplorerToThisPC -DisableOnedrive -HideOnedrive ";

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
