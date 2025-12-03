using DebloaterTool.Modules;
using DebloaterTool.Helpers;
using System.Collections.Generic;

namespace DebloaterTool.Settings
{
    public class ModuleList
    {
        // All tweaks list
        public static IList<TweakModule> GetAllModules() => new List<TweakModule>
        {
            new TweakModule(WinComponent.WinDefenderUninstall, "Uninstall Windows Defender", false),
            new TweakModule(WinComponent.DisableWindowsUpdateV1, "Disable Windows Update (Regedit Version)", true),
            new TweakModule(WinComponent.DisableWindowsUpdateV2, "Disable Windows Update (Overwriter Version)", false),
            new TweakModule(WinComponent.WinStoreUninstall, "Uninstall Microsoft Store", false),
            new TweakModule(DebloaterTools.RunChrisTool, "Run Chris Titus debloat tool", true),
            new TweakModule(DebloaterTools.RunRaphiTool, "Run Raphi debloat tool", true),
            new TweakModule(RemoveUnnecessary.ApplyOptimizationTweaks, "Apply system optimization tweaks", true),
            new TweakModule(RemoveUnnecessary.UninstallEdge, "Uninstall Microsoft Edge", true),
            new TweakModule(RemoveUnnecessary.CleanOutlookAndOneDrive, "Remove Outlook and OneDrive remnants", true),
            new TweakModule(SecurityPerformance.DisableRemAssistAndRemDesk, "Disable Remote Assistance and Desktop", false),
            new TweakModule(SecurityPerformance.DisableSpectreAndMeltdown, "Disable Spectre/Meltdown mitigations", false),
            new TweakModule(SecurityPerformance.DisableWinErrorReporting, "Disable Windows Error Reporting", false),
            new TweakModule(SecurityPerformance.ApplySecurityPerformanceTweaks, "Apply general security/performance tweaks", false),
            new TweakModule(SecurityPerformance.DisableSMBv1, "Disable outdated SMBv1 protocol", false),
            new TweakModule(DataCollection.DisableAdvertisingAndContentDelivery, "Disable ad/content delivery settings", true),
            new TweakModule(DataCollection.DisableDataCollectionPolicies, "Disable data collection policies", true),
            new TweakModule(DataCollection.DisableTelemetryServices, "Disable telemetry services", true),
            new TweakModule(WinCustomization.DisableSnapTools, "Disable Snap Assist tools", true),
            new TweakModule(WinCustomization.EnableUltimatePerformance, "Enable Ultimate Performance mode", true),
            new TweakModule(SoftwareThemes.ExplorerTheme, "Apply custom File Explorer theme", false),
            new TweakModule(SoftwareThemes.BorderTheme, "Apply custom window border theme", false),
            new TweakModule(SoftwareThemes.WindhawkInstaller, "Installs Windhawk with silent installation", false),
            new TweakModule(SoftwareThemes.StartAllBackInstaller, "Installs StartAllBack with silent installation", false),
            new TweakModule(SoftwareThemes.WindowsActivator, "Windows Activator", false),
            new TweakModule(SoftwareThemes.SevenZipInstaller, "Downloads and installs the 7-Zip file manager", true),
            new TweakModule(SoftwareThemes.AlwaysOnTop, "Install AlwaysOnTop to add a 'Top' button to any program", true),
            new TweakModule(SoftwareThemes.TakeOwnershipMenu, "Adds a “Take Ownership” entry to the right-click context menu", true),
            new TweakModule(SoftwareThemes.ApplyThemeTweaks, "Apply general theme tweaks", true),
            new TweakModule(BrowserDownloader.FireFox, "Install FireFox Browser", false),
            new TweakModule(BrowserDownloader.Ungoogled, "Install Ungoogled Browser", true),
            new TweakModule(BootLogo.Install, "Install custom boot logo", false),
            new TweakModule(Compression.CompressOS, "Compress the OS binaries with LZX", false),
            new TweakModule(Compression.CleanupWinSxS, "Clean up WinSxS to reduce disk usage", true),
            new TweakModule(CustomWallpapers.InstallWallpapers, "Install custom wallpaper for lockscreen and desktop", true),
        };
    }
}
