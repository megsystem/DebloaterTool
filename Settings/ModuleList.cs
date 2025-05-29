using System.Collections.Generic;
using DebloaterTool.Modules;
using DebloaterTool.Helpers;

namespace DebloaterTool.Settings
{
    public class ModuleList
    {
        // All tweaks list
        public static IList<TweakModule> GetAllModules() => new List<TweakModule>
        {
            new TweakModule(WinDefender.Uninstall, "Uninstall Windows Defender", false),
            new TweakModule(WinUpdate.DisableWindowsUpdateV1, "Disable Windows Update (Regedit Version)", true),
            new TweakModule(WinUpdate.DisableWindowsUpdateV2, "Disable Windows Update (Overwriter Version)", false),
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
            new TweakModule(WinStore.Uninstall, "Uninstall Microsoft Store", false),
            new TweakModule(WinCustomization.DisableSnapTools, "Disable Snap Assist tools", true),
            new TweakModule(WinCustomization.EnableUltimatePerformance, "Enable Ultimate Performance mode", true),
            new TweakModule(WindowsTheme.ExplorerTheme, "Apply custom File Explorer theme", false),
            new TweakModule(WindowsTheme.BorderTheme, "Apply custom window border theme", false),
            new TweakModule(WindowsTheme.WindhawkInstaller, "Installs Windhawk with silent installation", false),
            new TweakModule(WindowsTheme.AlwaysOnTop, "Install AlwaysOnTop to add a 'Top' button to any program", false),
            new TweakModule(WindowsTheme.ApplyThemeTweaks, "Apply general theme tweaks", true),
            new TweakModule(Ungoogled.Install, "Install Ungoogled Chromium", false),
            new TweakModule(BootLogo.Install, "Install custom boot logo", false),
            new TweakModule(Compression.CompressOS, "Compress the OS binaries with LZX", false),
            new TweakModule(Compression.CleanupWinSxS, "Clean up WinSxS to reduce disk usage", true),
            new TweakModule(CustomWallpapers.InstallWallpapers, "Install custom wallpaper for lockscreen and desktop", true),
        };
    }
}
