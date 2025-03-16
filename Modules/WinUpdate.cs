using Microsoft.Win32;
using System;
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
            try
            {
                // 1. Block Windows Update from connecting to the internet.
                using (RegistryKey wuKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate"))
                {
                    if (wuKey != null)
                    {
                        wuKey.SetValue("DoNotConnectToWindowsUpdateInternetLocations", 1, RegistryValueKind.DWord);
                        Logger.Log("Registry updated: DoNotConnectToWindowsUpdateInternetLocations set to 1.", Level.SUCCESS);
                    }
                    else
                    {
                        Logger.Log("Failed to open or create the WindowsUpdate registry key.", Level.ERROR);
                    }
                }

                // 2. Disable Automatic Updates by modifying Windows Update policies.
                using (RegistryKey auKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU"))
                {
                    if (auKey == null)
                        throw new Exception("Failed to open or create the WindowsUpdate\\AU registry key.");

                    auKey.SetValue("NoAutoUpdate", 1, RegistryValueKind.DWord);
                    auKey.SetValue("AUOptions", 2, RegistryValueKind.DWord);
                    Logger.Log("Registry updated: Automatic updates disabled (NoAutoUpdate=1, AUOptions=2).", Level.SUCCESS);
                }

                // 3. Disable Windows Update Delivery Optimization (applies to Windows 10).
                using (RegistryKey doKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DeliveryOptimization\Config", writable: true))
                {
                    if (doKey != null)
                    {
                        doKey.SetValue("DODownloadMode", 0, RegistryValueKind.DWord);
                        Logger.Log("Registry updated: Delivery Optimization disabled (DODownloadMode=0).", Level.SUCCESS);
                    }
                    else
                    {
                        Logger.Log("Delivery Optimization registry key not found.", Level.ERROR);
                    }
                }

                // 4. Set registry values to pause updates.
                using (RegistryKey uxKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings"))
                {
                    if (uxKey == null)
                        throw new Exception("Failed to open or create the WindowsUpdate\\UX\\Settings registry key.");

                    string dateStart = "2000-01-01T00:00:00Z";
                    string dateEnd = "3000-12-31T11:59:59Z";

                    uxKey.SetValue("PauseFeatureUpdatesStartTime", dateStart, RegistryValueKind.String);
                    uxKey.SetValue("PauseQualityUpdatesStartTime", dateStart, RegistryValueKind.String);
                    // Attempt to set PauseUpdatesStartTime; ignore errors if they occur.
                    try
                    {
                        uxKey.SetValue("PauseUpdatesStartTime", dateStart, RegistryValueKind.String);
                    }
                    catch { /* Silently ignore errors */ }
                    uxKey.SetValue("PauseFeatureUpdatesEndTime", dateEnd, RegistryValueKind.String);
                    uxKey.SetValue("PauseQualityUpdatesEndTime", dateEnd, RegistryValueKind.String);
                    uxKey.SetValue("PauseUpdatesExpiryTime", dateEnd, RegistryValueKind.String);
                    Logger.Log("Registry updated: Update pause settings configured.", Level.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Registry configuration error: " + ex.Message, Level.ERROR);
                Logger.Log("Registry configuration error stack trace: " + ex.StackTrace, Level.ERROR);
            }
        }

        /// <summary>
        /// Disables Windows Update using an alternative approach:
        /// downloads a supporting executable (PowerRun.exe), then disables update services,
        /// renames system files, updates the registry, deletes update files, and disables scheduled tasks.
        /// </summary>
        public static void DisableWindowsUpdateV2()
        {
            Logger.Log("Downloading...");
            string powerRunPath = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.exe");
            if (!ComGlobal.DownloadFile(ExternalLinks.powerRun, powerRunPath))
            {
                Logger.Log("Failed to download PowerRun.exe. Exiting...", Level.ERROR);
                return;
            }
            Logger.Log("Download complete.");

            string[] services = { "wuauserv", "UsoSvc", "uhssvc", "WaaSMedicSvc" };
            foreach (var service in services)
            {
                ComGlobal.RunCommand(powerRunPath, $"cmd.exe /c net stop {service}");
                ComGlobal.RunCommand(powerRunPath, $"cmd.exe /c sc config {service} start= disabled");
                ComGlobal.RunCommand(powerRunPath, $"cmd.exe /c sc failure {service} reset= 0 actions= \"\"");
            }

            string[] files = { "WaaSMedicSvc.dll", "wuaueng.dll" };
            foreach (var file in files)
            {
                string filePath = $"C:\\Windows\\System32\\{file}";
                string backupPath = $"{filePath}_BAK";

                ComGlobal.RunCommand(powerRunPath, $"cmd.exe /c takeown /f {filePath}");
                ComGlobal.RunCommand(powerRunPath, $"cmd.exe /c icacls {filePath} /grant Everyone:F");
                ComGlobal.RunCommand(powerRunPath, $"cmd.exe /c rename {filePath} {backupPath}");
                ComGlobal.RunCommand(powerRunPath, $"cmd.exe /c icacls {backupPath} /setowner \"NT SERVICE\\TrustedInstaller\" & icacls {backupPath} /remove Everyone");
            }

            ComGlobal.RunCommand(powerRunPath, "cmd.exe /c reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WaaSMedicSvc\" /v Start /t REG_DWORD /d 4 /f");
            ComGlobal.RunCommand(powerRunPath, "cmd.exe /c reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU\" /v NoAutoUpdate /t REG_DWORD /d 1 /f");

            ComGlobal.RunCommand(powerRunPath, "cmd.exe /c erase /f /s /q C:\\Windows\\SoftwareDistribution\\*.*");
            ComGlobal.RunCommand(powerRunPath, "cmd.exe /c rmdir /s /q C:\\Windows\\SoftwareDistribution");

            string powershellCmd = "Get-ScheduledTask -TaskPath '\\Microsoft\\Windows\\UpdateOrchestrator\\*' | Disable-ScheduledTask; " +
                       "Get-ScheduledTask -TaskPath '\\Microsoft\\Windows\\WaaSMedic\\*' | Disable-ScheduledTask; " +
                       "Get-ScheduledTask -TaskPath '\\Microsoft\\Windows\\WindowsUpdate\\*' | Disable-ScheduledTask;";
            ComGlobal.RunCommand(powerRunPath, $"cmd.exe /c powershell -Command \"{powershellCmd}\"");
        }
    }
}
