using DebloaterTool.Helpers;
using DebloaterTool.Logging;
using DebloaterTool.Settings;
using Microsoft.Win32;
using System.IO;

namespace DebloaterTool.Modules
{
    internal class WinComponent
    {
        /// <summary>
        /// Uninstalls Windows Defender components by:
        /// - Downloading a helper executable (PowerRun.exe) to elevate commands.
        /// - Importing a registry file to apply Defender-related configuration changes.
        /// - Deleting specific files and directories associated with Windows Defender.
        /// </summary>
        public static void WinDefenderUninstall()
        {
            Logger.Log($"Downloading from {Global.powerRun}...");
            string powerRunPath = Path.Combine(Global.debloatersPath, $"PowerRun.exe");
            if (!Internet.DownloadFile(Global.powerRun, powerRunPath))
            {
                Logger.Log($"Failed to download {Global.powerRun}. Skipping...", Level.ERROR);
                return;
            }
            Logger.Log($"Download complete to {powerRunPath}");

            string[] filesToDelete =
            {
                "C:\\Windows\\WinSxS\\FileMaps\\wow64_windows-defender*.manifest",
                "C:\\Windows\\WinSxS\\FileMaps\\x86_windows-defender*.manifest",
                "C:\\Windows\\WinSxS\\FileMaps\\amd64_windows-defender*.manifest",
                "C:\\Windows\\System32\\SecurityHealthSystray.exe",
                "C:\\Windows\\System32\\SecurityHealthService.exe",
                "C:\\Windows\\System32\\SecurityHealthHost.exe",
                "C:\\Windows\\System32\\drivers\\WdDevFlt.sys",
                "C:\\Windows\\System32\\drivers\\WdBoot.sys",
                "C:\\Windows\\System32\\drivers\\WdFilter.sys",
                "C:\\Windows\\System32\\wscsvc.dll",
                "C:\\Windows\\System32\\smartscreen.dll",
                "C:\\Windows\\SysWOW64\\smartscreen.dll",
                "C:\\Windows\\System32\\DWWIN.EXE"
            };

            string[] directoriesToDelete =
            {
                "C:\\ProgramData\\Microsoft\\Windows Defender",
                "C:\\Program Files (x86)\\Windows Defender",
                "C:\\Program Files\\Windows Defender",
                "C:\\Windows\\System32\\SecurityHealth",
                "C:\\Windows\\System32\\WebThreatDefSvc"
            };

            string regFile = Path.Combine(Global.debloatersPath, "defenderkiller.reg");
            if (!Internet.DownloadFile(Global.defender, regFile))
            {
                Logger.Log("Failed to download DefenderKiller.reg. Exiting...", Level.ERROR);
                return;
            }

            // Import the registry file silently using regedit (/s switch).
            Runner.Command(powerRunPath, $"regedit.exe /s \"{regFile}\"");

            foreach (var file in filesToDelete)
            {
                Runner.Command(powerRunPath, $"cmd.exe /c del /f \"{file}\"");
            }

            foreach (var dir in directoriesToDelete)
            {
                Runner.Command(powerRunPath, $"cmd.exe /c rmdir /s /q \"{dir}\"");
            }
        }

        /// <summary>
        /// Uninstalls Microsoft Store by executing PowerShell commands to remove the Windows Store
        /// package for the current user as well as for all users on the system.
        /// </summary>
        public static void WinStoreUninstall()
        {
            // Remove Microsoft Store for the current user.
            Runner.Command("powershell", "-NoProfile -Command \"Get-AppxPackage *WindowsStore* | Remove-AppxPackage\"");

            // Remove Microsoft Store for all users.
            Runner.Command("powershell", "-NoProfile -Command \"Get-AppxPackage -AllUsers *WindowsStore* | Remove-AppxPackage\"");
        }

        /// <summary>
        /// Disables Windows Update by modifying registry settings to block update connections, 
        /// disable automatic updates, and prevent peer-to-peer update distribution. 
        /// Additionally, it enforces an indefinite pause on updates through registry configurations.
        /// </summary>
        public static void DisableWindowsUpdateV1()
        {
            TweakRegistry[] modifications = new TweakRegistry[]
            {
                // 1. Block Windows Update from connecting to the internet.
                new TweakRegistry(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate", "DoNotConnectToWindowsUpdateInternetLocations", RegistryValueKind.DWord, 1),
                // 2. Disable Automatic Updates.
                new TweakRegistry(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU", "NoAutoUpdate", RegistryValueKind.DWord, 1),
                new TweakRegistry(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU", "AUOptions", RegistryValueKind.DWord, 2),
                // 3. Disable Windows Update Delivery Optimization (Windows 10 feature).
                new TweakRegistry(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\DeliveryOptimization\\Config", "DODownloadMode", RegistryValueKind.DWord, 0),
                // 4. Pause Windows Updates indefinitely.
                new TweakRegistry(Registry.LocalMachine, "SOFTWARE\\Microsoft\\WindowsUpdate\\UX\\Settings", "PauseFeatureUpdatesStartTime", RegistryValueKind.String, "2000-01-01T00:00:00Z"),
                new TweakRegistry(Registry.LocalMachine, "SOFTWARE\\Microsoft\\WindowsUpdate\\UX\\Settings", "PauseQualityUpdatesStartTime", RegistryValueKind.String, "2000-01-01T00:00:00Z"),
                new TweakRegistry(Registry.LocalMachine, "SOFTWARE\\Microsoft\\WindowsUpdate\\UX\\Settings", "PauseFeatureUpdatesEndTime", RegistryValueKind.String, "3000-12-31T11:59:59Z"),
                new TweakRegistry(Registry.LocalMachine, "SOFTWARE\\Microsoft\\WindowsUpdate\\UX\\Settings", "PauseQualityUpdatesEndTime", RegistryValueKind.String, "3000-12-31T11:59:59Z"),
                new TweakRegistry(Registry.LocalMachine, "SOFTWARE\\Microsoft\\WindowsUpdate\\UX\\Settings", "PauseUpdatesExpiryTime", RegistryValueKind.String, "3000-12-31T11:59:59Z")
            };

            Regedit.InstallRegModification(modifications);
        }

        /// <summary>
        /// Disables Windows Update using an alternative approach:
        /// downloads a supporting executable (PowerRun.exe), then disables update services,
        /// renames system files, updates the registry, deletes update files, and disables scheduled tasks.
        /// </summary>
        public static void DisableWindowsUpdateV2()
        {
            Logger.Log($"Downloading from {Global.powerRun}...");
            string powerRunPath = Path.Combine(Global.debloatersPath, $"PowerRun.exe");
            if (!Internet.DownloadFile(Global.powerRun, powerRunPath))
            {
                Logger.Log($"Failed to download {Global.powerRun}. Skipping...", Level.ERROR);
                return;
            }
            Logger.Log($"Download complete to {powerRunPath}");

            string[] services = { "wuauserv", "UsoSvc", "uhssvc", "WaaSMedicSvc" };
            foreach (var service in services)
            {
                Runner.Command(powerRunPath, $"cmd.exe /c net stop {service}");
                Runner.Command(powerRunPath, $"cmd.exe /c sc config {service} start= disabled");
                Runner.Command(powerRunPath, $"cmd.exe /c sc failure {service} reset= 0 actions= \"\"");
            }

            string[] files = { "WaaSMedicSvc.dll", "wuaueng.dll" };
            foreach (var file in files)
            {
                string filePath = $"C:\\Windows\\System32\\{file}";
                string backupPath = $"{filePath}_BAK";

                Runner.Command(powerRunPath, $"cmd.exe /c takeown /f {filePath}");
                Runner.Command(powerRunPath, $"cmd.exe /c icacls {filePath} /grant Everyone:F");
                Runner.Command(powerRunPath, $"cmd.exe /c rename {filePath} {backupPath}");
                Runner.Command(powerRunPath, $"cmd.exe /c icacls {backupPath} /setowner \"NT SERVICE\\TrustedInstaller\" & icacls {backupPath} /remove Everyone");
            }

            Runner.Command(powerRunPath, "cmd.exe /c reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WaaSMedicSvc\" /v Start /t REG_DWORD /d 4 /f");
            Runner.Command(powerRunPath, "cmd.exe /c reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU\" /v NoAutoUpdate /t REG_DWORD /d 1 /f");

            Runner.Command(powerRunPath, "cmd.exe /c erase /f /s /q C:\\Windows\\SoftwareDistribution\\*.*");
            Runner.Command(powerRunPath, "cmd.exe /c rmdir /s /q C:\\Windows\\SoftwareDistribution");

            string powershellCmd = "Get-ScheduledTask -TaskPath '\\Microsoft\\Windows\\UpdateOrchestrator\\*' | Disable-ScheduledTask; " +
                       "Get-ScheduledTask -TaskPath '\\Microsoft\\Windows\\WaaSMedic\\*' | Disable-ScheduledTask; " +
                       "Get-ScheduledTask -TaskPath '\\Microsoft\\Windows\\WindowsUpdate\\*' | Disable-ScheduledTask;";
            Runner.Command(powerRunPath, $"cmd.exe /c powershell -Command \"{powershellCmd}\"");
        }
    }
}
