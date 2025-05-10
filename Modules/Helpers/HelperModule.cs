using System;
using System.Collections.Generic;

namespace DebloaterTool
{
    public class HelperModule 
    {
        // All tweaks list
        public static Dictionary<Action, TweakInfo> allTweaks = new Dictionary<Action, TweakInfo>
        {
            { WinDefender.Uninstall, new TweakInfo("Uninstall Windows Defender", false) },
            { WinUpdate.DisableWindowsUpdateV1, new TweakInfo("Disable Windows Update (Version 1)", true) },
            { WinUpdate.DisableWindowsUpdateV2, new TweakInfo("Disable Windows Update (Version 2)", false) },
            { DebloaterTools.RunChrisTool, new TweakInfo("Run Chris Titus debloat tool", true) },
            { DebloaterTools.RunRaphiTool, new TweakInfo("Run Raphi debloat tool", true) },
            { RemoveUnnecessary.ApplyOptimizationTweaks, new TweakInfo("Apply system optimization tweaks", true) },
            { RemoveUnnecessary.UninstallEdge, new TweakInfo("Uninstall Microsoft Edge", true) },
            { RemoveUnnecessary.CleanOutlookAndOneDrive, new TweakInfo("Remove Outlook and OneDrive remnants", true) },
            { SecurityPerformance.DisableRemAssistAndRemDesk, new TweakInfo("Disable Remote Assistance and Desktop", true) },
            { SecurityPerformance.DisableSpectreAndMeltdown, new TweakInfo("Disable Spectre/Meltdown mitigations", false) },
            { SecurityPerformance.DisableWinErrorReporting, new TweakInfo("Disable Windows Error Reporting", true) },
            { SecurityPerformance.ApplySecurityPerformanceTweaks, new TweakInfo("Apply general security/performance tweaks", true) },
            { SecurityPerformance.DisableSMBv1, new TweakInfo("Disable outdated SMBv1 protocol", true) },
            { DataCollection.DisableAdvertisingAndContentDelivery, new TweakInfo("Disable ad/content delivery settings", true) },
            { DataCollection.DisableDataCollectionPolicies, new TweakInfo("Disable data collection policies", true) },
            { DataCollection.DisableTelemetryServices, new TweakInfo("Disable telemetry services", true) },
            { WinStore.Uninstall, new TweakInfo("Uninstall Microsoft Store", false) },
            { WinCustomization.DisableSnapTools, new TweakInfo("Disable Snap Assist tools", true) },
            { WinCustomization.EnableUltimatePerformance, new TweakInfo("Enable Ultimate Performance mode", true) },
            { Ungoogled.Install, new TweakInfo("Install Ungoogled Chromium", true) },
            { WindowsTheme.ExplorerTheme, new TweakInfo("Apply custom File Explorer theme", false) },
            { WindowsTheme.BorderTheme, new TweakInfo("Apply custom window border theme", false) },
            { WindowsTheme.ApplyThemeTweaks, new TweakInfo("Apply general theme tweaks", true) },
            { BootLogo.Install, new TweakInfo("Install custom boot logo", true) }
        };
    }

    public class TweakInfo
    {
        public string Description { get; set; }
        public bool IsDefaultEnabled { get; set; }

        public TweakInfo(string description, bool isDefaultEnabled)
        {
            Description = description;
            IsDefaultEnabled = isDefaultEnabled;
        }
    }

    public class TweakModule
    {
        public Action Action { get; set; }
        public TweakInfo Info { get; set; }

        public TweakModule(Action action, TweakInfo info)
        {
            Action = action;
            Info = info;
        }
    }
}
