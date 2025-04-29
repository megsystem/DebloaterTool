using Microsoft.Win32;
using System.IO;

namespace DebloaterTool
{
    internal class WinUpdate
    {
        /// <summary>
        /// Disables Windows Update by modifying registry settings to block update connections, 
        /// disable automatic updates, and prevent peer-to-peer update distribution. 
        /// Additionally, it enforces an indefinite pause on updates through registry configurations.
        /// </summary>
        public static void DisableWindowsUpdateV1()
        {
            RegistryModification[] modifications = new RegistryModification[]
            {
                // 1. Block Windows Update from connecting to the internet.
                new RegistryModification(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate", "DoNotConnectToWindowsUpdateInternetLocations", RegistryValueKind.DWord, 1),
                // 2. Disable Automatic Updates.
                new RegistryModification(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU", "NoAutoUpdate", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU", "AUOptions", RegistryValueKind.DWord, 2),
                // 3. Disable Windows Update Delivery Optimization (Windows 10 feature).
                new RegistryModification(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\DeliveryOptimization\\Config", "DODownloadMode", RegistryValueKind.DWord, 0),
                // 4. Pause Windows Updates indefinitely.
                new RegistryModification(Registry.LocalMachine, "SOFTWARE\\Microsoft\\WindowsUpdate\\UX\\Settings", "PauseFeatureUpdatesStartTime", RegistryValueKind.String, "2000-01-01T00:00:00Z"),
                new RegistryModification(Registry.LocalMachine, "SOFTWARE\\Microsoft\\WindowsUpdate\\UX\\Settings", "PauseQualityUpdatesStartTime", RegistryValueKind.String, "2000-01-01T00:00:00Z"),
                new RegistryModification(Registry.LocalMachine, "SOFTWARE\\Microsoft\\WindowsUpdate\\UX\\Settings", "PauseFeatureUpdatesEndTime", RegistryValueKind.String, "3000-12-31T11:59:59Z"),
                new RegistryModification(Registry.LocalMachine, "SOFTWARE\\Microsoft\\WindowsUpdate\\UX\\Settings", "PauseQualityUpdatesEndTime", RegistryValueKind.String, "3000-12-31T11:59:59Z"),
                new RegistryModification(Registry.LocalMachine, "SOFTWARE\\Microsoft\\WindowsUpdate\\UX\\Settings", "PauseUpdatesExpiryTime", RegistryValueKind.String, "3000-12-31T11:59:59Z")
            };

            HelperRegedit.InstallRegModification(modifications);
        }

        /// <summary>
        /// Disables Windows Update using an alternative approach:
        /// downloads a supporting executable (PowerRun.exe), then disables update services,
        /// renames system files, updates the registry, deletes update files, and disables scheduled tasks.
        /// </summary>
        public static void DisableWindowsUpdateV2()
        {
            Directory.CreateDirectory(Settings.debloatersPath);
            Logger.Log($"Downloading from {Settings.powerRun}...");
            string powerRunPath = Path.Combine(Settings.debloatersPath, $"PowerRun.exe");
            if (!File.Exists(powerRunPath))
            {
                if (!HelperGlobal.DownloadFile(Settings.powerRun, powerRunPath))
                {
                    Logger.Log($"Failed to download {Settings.powerRun}. Skipping...", Level.ERROR);
                    return;
                }
            }
            else
            {
                Logger.Log("PowerRun.exe already exists. Skipping download.", Level.WARNING);
            }
            Logger.Log($"Download complete to {powerRunPath}");

            string[] services = { "wuauserv", "UsoSvc", "uhssvc", "WaaSMedicSvc" };
            foreach (var service in services)
            {
                HelperGlobal.RunCommand(powerRunPath, $"cmd.exe /c net stop {service}");
                HelperGlobal.RunCommand(powerRunPath, $"cmd.exe /c sc config {service} start= disabled");
                HelperGlobal.RunCommand(powerRunPath, $"cmd.exe /c sc failure {service} reset= 0 actions= \"\"");
            }

            string[] files = { "WaaSMedicSvc.dll", "wuaueng.dll" };
            foreach (var file in files)
            {
                string filePath = $"C:\\Windows\\System32\\{file}";
                string backupPath = $"{filePath}_BAK";

                HelperGlobal.RunCommand(powerRunPath, $"cmd.exe /c takeown /f {filePath}");
                HelperGlobal.RunCommand(powerRunPath, $"cmd.exe /c icacls {filePath} /grant Everyone:F");
                HelperGlobal.RunCommand(powerRunPath, $"cmd.exe /c rename {filePath} {backupPath}");
                HelperGlobal.RunCommand(powerRunPath, $"cmd.exe /c icacls {backupPath} /setowner \"NT SERVICE\\TrustedInstaller\" & icacls {backupPath} /remove Everyone");
            }

            HelperGlobal.RunCommand(powerRunPath, "cmd.exe /c reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WaaSMedicSvc\" /v Start /t REG_DWORD /d 4 /f");
            HelperGlobal.RunCommand(powerRunPath, "cmd.exe /c reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU\" /v NoAutoUpdate /t REG_DWORD /d 1 /f");

            HelperGlobal.RunCommand(powerRunPath, "cmd.exe /c erase /f /s /q C:\\Windows\\SoftwareDistribution\\*.*");
            HelperGlobal.RunCommand(powerRunPath, "cmd.exe /c rmdir /s /q C:\\Windows\\SoftwareDistribution");

            string powershellCmd = "Get-ScheduledTask -TaskPath '\\Microsoft\\Windows\\UpdateOrchestrator\\*' | Disable-ScheduledTask; " +
                       "Get-ScheduledTask -TaskPath '\\Microsoft\\Windows\\WaaSMedic\\*' | Disable-ScheduledTask; " +
                       "Get-ScheduledTask -TaskPath '\\Microsoft\\Windows\\WindowsUpdate\\*' | Disable-ScheduledTask;";
            HelperGlobal.RunCommand(powerRunPath, $"cmd.exe /c powershell -Command \"{powershellCmd}\"");
        }
    }
}
