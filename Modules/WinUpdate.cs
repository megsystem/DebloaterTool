using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.ServiceProcess;
using System.Text;

namespace DebloaterTool
{
    internal class WinUpdate
    {
        public static void DisableWindowsUpdate()
        {
            //First Try to disable Windows Update
            // Stop and disable the Windows Update service (wuauserv)
            StopService("wuauserv");
            SetServiceStartupType("wuauserv", "Disabled");

            // Stop and disable the Windows Update Medic Service (WaaSMedicSvc)
            StopService("WaaSMedicSvc");
            SetServiceStartupType("WaaSMedicSvc", "Disabled");

            // Disable any scheduled tasks related to Windows Update
            DisableWindowsUpdateScheduledTasks();

            // Block Windows Update from connecting to internet locations via the registry
            ConfigureWindowsUpdateRegistry();
        }

        static string powerRunPath = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.exe");
        public static void DisableWindowsUpdateV2()
        {
            string powerRunUrl = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/PowerRun.exe";

            Logger.Log("Downloading...");
            if (!ComFunction.DownloadFile(powerRunUrl, powerRunPath))
            {
                Logger.Log("Failed to download PowerRun.exe. Exiting...", Level.ERROR);
                return;
            }
            Logger.Log("Download complete.");

            DisableUpdateServices();
            RenameSystemFiles();
            UpdateRegistry();
            DeleteUpdateFiles();
            DisableScheduledTasks();
        }

        static void DisableUpdateServices()
        {
            string[] services = { "wuauserv", "UsoSvc", "uhssvc", "WaaSMedicSvc" };
            foreach (var service in services)
            {
                ComFunction.RunCommand(powerRunPath, $"cmd.exe /c net stop {service}");
                ComFunction.RunCommand(powerRunPath, $"cmd.exe /c sc config {service} start= disabled");
                ComFunction.RunCommand(powerRunPath, $"cmd.exe /c sc failure {service} reset= 0 actions= \"\"");
            }
        }

        static void RenameSystemFiles()
        {
            string[] files = { "WaaSMedicSvc.dll", "wuaueng.dll" };
            foreach (var file in files)
            {
                string filePath = $"C:\\Windows\\System32\\{file}";
                string backupPath = $"{filePath}_BAK";

                ComFunction.RunCommand(powerRunPath, $"cmd.exe /c takeown /f {filePath}");
                ComFunction.RunCommand(powerRunPath, $"cmd.exe /c icacls {filePath} /grant Everyone:F");
                ComFunction.RunCommand(powerRunPath, $"cmd.exe /c rename {filePath} {backupPath}");
                ComFunction.RunCommand(powerRunPath, $"cmd.exe /c icacls {backupPath} /setowner \"NT SERVICE\\TrustedInstaller\" & icacls {backupPath} /remove Everyone");
            }
        }

        static void UpdateRegistry()
        {
            ComFunction.RunCommand(powerRunPath, "cmd.exe /c reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WaaSMedicSvc\" /v Start /t REG_DWORD /d 4 /f");
            ComFunction.RunCommand(powerRunPath, "cmd.exe /c reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU\" /v NoAutoUpdate /t REG_DWORD /d 1 /f");
        }

        static void DeleteUpdateFiles()
        {
            ComFunction.RunCommand(powerRunPath, "cmd.exe /c erase /f /s /q C:\\Windows\\SoftwareDistribution\\*.*");
            ComFunction.RunCommand(powerRunPath, "cmd.exe /c rmdir /s /q C:\\Windows\\SoftwareDistribution");
        }

        static void DisableScheduledTasks()
        {
            string powershellCmd = "Get-ScheduledTask -TaskPath '\\Microsoft\\Windows\\UpdateOrchestrator\\*' | Disable-ScheduledTask; " +
                                   "Get-ScheduledTask -TaskPath '\\Microsoft\\Windows\\WaaSMedic\\*' | Disable-ScheduledTask; " +
                                   "Get-ScheduledTask -TaskPath '\\Microsoft\\Windows\\WindowsUpdate\\*' | Disable-ScheduledTask;";
            ComFunction.RunCommand(powerRunPath, $"cmd.exe /c powershell -Command \"{powershellCmd}\"");
        }

        static void StopService(string serviceName)
        {
            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    if (sc.Status != ServiceControllerStatus.Stopped &&
                        sc.Status != ServiceControllerStatus.StopPending)
                    {
                        Logger.Log($"Stopping service {serviceName}...", Level.WARNING);
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                        Logger.Log($"{serviceName} stopped successfully.", Level.SUCCESS);
                    }
                    else
                    {
                        Logger.Log($"{serviceName} is already stopped.", Level.WARNING);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error stopping service {serviceName}: {ex.Message}", Level.ERROR);
            }
        }

        static void SetServiceStartupType(string serviceName, string startMode)
        {
            try
            {
                // Use WMI to change the service startup type
                string query = $"Win32_Service.Name='{serviceName}'";
                using (ManagementObject service = new ManagementObject(query))
                {
                    ManagementBaseObject inParams = service.GetMethodParameters("ChangeStartMode");
                    inParams["StartMode"] = startMode;
                    ManagementBaseObject outParams = service.InvokeMethod("ChangeStartMode", inParams, null);
                    uint returnValue = (uint)outParams["ReturnValue"];
                    if (returnValue == 0)
                        Logger.Log($"{serviceName} startup type set to {startMode}.", Level.SUCCESS);
                    else
                        Logger.Log($"Failed to change startup type for {serviceName}. Error code: {returnValue}", Level.ERROR);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error setting startup type for {serviceName}: {ex.Message}", Level.ERROR);
            }
        }

        static void DisableWindowsUpdateScheduledTasks()
        {
            try
            {
                // Query all scheduled tasks in CSV format.
                Process proc = new Process();
                proc.StartInfo.FileName = "schtasks";
                proc.StartInfo.Arguments = "/query /FO CSV /V";
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();

                // Split output into lines.
                var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 1)
                {
                    // The first line is the CSV header.
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        string[] fields = SplitCsvLine(line);
                        if (fields.Length > 0 && fields[0].IndexOf("WindowsUpdate", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string taskName = fields[0];
                            // Disable the task using schtasks /Change.
                            Process disableProc = new Process();
                            disableProc.StartInfo.FileName = "schtasks";
                            disableProc.StartInfo.Arguments = $"/Change /TN \"{taskName}\" /Disable";
                            disableProc.StartInfo.UseShellExecute = false;
                            disableProc.StartInfo.CreateNoWindow = true;
                            disableProc.Start();
                            disableProc.WaitForExit();
                            Logger.Log($"Disabled scheduled task: {taskName}", Level.SUCCESS);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error disabling scheduled tasks: {ex.Message}", Level.ERROR);
            }
        }

        /// <summary>
        /// Splits a CSV line into fields, handling quoted values.
        /// </summary>
        static string[] SplitCsvLine(string line)
        {
            var fields = new List<string>();
            bool inQuotes = false;
            var fieldBuilder = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    // If next char is also a quote, it's an escaped quote.
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        fieldBuilder.Append('"');
                        i++; // Skip the escaped quote.
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(fieldBuilder.ToString());
                    fieldBuilder.Clear();
                }
                else
                {
                    fieldBuilder.Append(c);
                }
            }
            // Add the last field.
            fields.Add(fieldBuilder.ToString());
            return fields.ToArray();
        }

        static void ConfigureWindowsUpdateRegistry()
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
    }
}
