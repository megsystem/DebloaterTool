using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Security.Principal;
using System.Security.AccessControl;
using Microsoft.Win32;
using System.Text;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Management;
using System.Collections.Generic;
using System.Web.Script.Serialization;

// Created by @_giovannigiannone and ChatGPT
// Inspired from the Talon's Project!
namespace DebloaterTool
{
    class Program
    {
        static void Main(string[] args)
        {
            // For .NET Framework 4.0, enable TLS 1.2 by casting its numeric value.
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            // Run the Welcome Screen and EULA
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.Title = $"DebloaterTool V{version.Major}.{version.Minor}.{version.Build}";
            Console.WriteLine("+=================================================================+");
            Console.WriteLine("|    ____       _     _             _              ____           |");
            Console.WriteLine("|   |  _ \\  ___| |__ | | ___   __ _| |_ ___ _ __  | __ ) _   _    |");
            Console.WriteLine("|   | | | |/ _ \\ '_ \\| |/ _ \\ / _` | __/ _ \\ '__| |  _ \\| | | |   |");
            Console.WriteLine("|   | |_| |  __/ |_) | | (_) | (_| | ||  __/ |    | |_) | |_| |   |");
            Console.WriteLine("|   |____/ \\___|_.__/|_|\\___/ \\__,_|\\__\\___|_|    |____/ \\__, |   |");
            Console.WriteLine("|    _____ ____ _  ______ ___ _____     ___    _   _ _   |___/    |");
            Console.WriteLine("|   |  ___/ ___| |/ / ___|_ _/ _ \\ \\   / / \\  | \\ | | \\ | |_ _|   |");
            Console.WriteLine("|   | |_ | |   | ' / |  _ | | | | \\ \\ / / _ \\ |  \\| |  \\| || |    |");
            Console.WriteLine("|   |  _|| |___| . \\ |_| || | |_| |\\ V / ___ \\| |\\  | |\\  || |    |");
            Console.WriteLine("|   |_|   \\____|_|\\_\\____|___\\___/  \\_/_/   \\_\\_| \\_|_| \\_|___|   |");
            Console.WriteLine("|                                                                 |");
            Console.WriteLine("+=================================================================+");
            Console.WriteLine("End User License Agreement (EULA)");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("By using this software, you agree to the following terms:");
            Console.WriteLine("1. You may not distribute this software without permission.");
            Console.WriteLine("2. The developers are not responsible for any damages.");
            DisplayMessage("3. Please disable your antivirus before proceeding.", ConsoleColor.Red);
            Console.WriteLine("---------------------------------");

            // EULA Confirmation
            if (!RequestYesOrNo("Do you accept the EULA?"))
            {
                Console.WriteLine("EULA declined. Press ENTER to close.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            // Check if the program is runned with administrator rights!
            if (!IsAdministrator())
            {
                DisplayMessage("This application must be run with administrator rights!", ConsoleColor.Red);

                if (RequestYesOrNo("Do you want to run as administrator?"))
                {
                    RestartAsAdmin();
                }
                else
                {
                    DisplayMessage("Please restart the program with administrator rights to continue!", ConsoleColor.Red);
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            // Restart Confirmation
            bool restart = RequestYesOrNo("Do you want to restart after the process?");

            // Run debloater
            RunTweaks();
            RunWinConfig();
            ApplyRegistryChanges();
            UninstallEdge();
            CleanOutlookAndOneDrive();
            DisableWindowsUpdate();
            UngoogledInstaller();
            ChangeUngoogledHomePage();
            SetCustomWallpaper();

            // Process completed
            if (restart)
            {
                Process.Start("shutdown.exe", "-r -t 0"); // Restart the computer
            }
            else
            {
                DisplayMessage("Restart skipped. Process completed. Press ENTER to close.", ConsoleColor.Green);
                Console.ReadLine(); // Wait for user to press Enter
            }

            // End
            return;
        }

        static bool RequestYesOrNo(string message)
        {
            while (true)
            {
                Console.Write($"{message} (yes/no): ");
                string response = Console.ReadLine()?.Trim().ToLower();

                if (response == "yes") return true;
                if (response == "no") return false;

                Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
            }
        }

        static void RestartAsAdmin()
        {
            ProcessStartInfo proc = new ProcessStartInfo()
            {
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true,
                Verb = "runas"
            };
            try
            {
                Process.Start(proc);
            }
            catch (Exception ex)
            {
                DisplayMessage($"Failed to start as administrator: {ex.Message}", ConsoleColor.Red);
            }
            Environment.Exit(0);
        }

        static void ChangeUngoogledHomePage()
        {
            string argToAdd = "--custom-ntp=https://xengshi.github.io/materialYouNewTab/";
            // Directories to search: Desktop, Common Desktop, Start Menu, Programs, and Taskbar pinned shortcuts.
            string[] dirs = new string[] {
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar"
            };

            // A single void (Main) with an inline recursive delegate.
            Action<string> ProcessDirectory = null;
            ProcessDirectory = delegate (string path)
            {
                try
                {
                    foreach (string file in Directory.GetFiles(path, "*.lnk"))
                    {
                        try
                        {
                            var shell = new IWshRuntimeLibrary.WshShell();
                            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(file);
                            // Check if the target exe is "chrome.exe"
                            if (string.Equals(Path.GetFileName(shortcut.TargetPath), "chrome.exe", StringComparison.OrdinalIgnoreCase))
                            {
                                if (string.IsNullOrEmpty(shortcut.Arguments) || !shortcut.Arguments.Contains(argToAdd))
                                {
                                    shortcut.Arguments = (shortcut.Arguments + " " + argToAdd).Trim();
                                    shortcut.Save();
                                    DisplayMessage("Updated: " + file, ConsoleColor.Green);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DisplayMessage("Error processing shortcut " + file + ": " + ex.Message, ConsoleColor.Red);
                        }
                    }
                    foreach (string sub in Directory.GetDirectories(path))
                        ProcessDirectory(sub);
                }
                catch (Exception ex)
                {
                    DisplayMessage("Error processing directory " + path + ": " + ex.Message, ConsoleColor.Red);
                }
            };

            foreach (string dir in dirs)
            {
                if (Directory.Exists(dir))
                    ProcessDirectory(dir);
            }
            DisplayMessage("Shortcut update complete.", ConsoleColor.Green);
        }

        public static void UngoogledInstaller()
        {
            try
            {
                DisplayMessage("Fetching latest release information...", ConsoleColor.Cyan);

                string apiUrl = "https://api.github.com/repos/ungoogled-software/ungoogled-chromium-windows/releases/latest";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.UserAgent = "Mozilla/5.0 (compatible; AcmeInc/1.0)";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                {
                    string json = new StreamReader(responseStream).ReadToEnd();

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    dynamic release = serializer.Deserialize<dynamic>(json);

                    bool is64Bit = Environment.Is64BitOperatingSystem;
                    string searchPattern = is64Bit ? "installer_x64.exe" : "installer_x86.exe";
                    string downloadUrl = null;
                    string assetName = null;

                    foreach (var asset in release["assets"])
                    {
                        string name = asset["name"];
                        if (name.IndexOf(searchPattern, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            assetName = name;
                            downloadUrl = asset["browser_download_url"];
                            break;
                        }
                    }

                    if (downloadUrl == null)
                    {
                        DisplayMessage("Installer asset not found for pattern: " + searchPattern, ConsoleColor.Red);
                        return;
                    }

                    DisplayMessage("Latest installer found: " + assetName, ConsoleColor.Cyan);
                    DisplayMessage("Download URL: " + downloadUrl, ConsoleColor.Cyan);

                    string tempFile = Path.Combine(Path.GetTempPath(), assetName);
                    DisplayMessage("Downloading installer to " + tempFile + "...", ConsoleColor.Cyan);

                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; AcmeInc/1.0)");
                        ManualResetEvent downloadCompleted = new ManualResetEvent(false);

                        webClient.DownloadProgressChanged += (s, e) =>
                        {
                            int progress = e.ProgressPercentage;
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write($"\rDownloading... {progress}%   ");
                            Console.ResetColor();
                        };

                        webClient.DownloadFileCompleted += (s, e) =>
                        {
                            downloadCompleted.Set();
                        };

                        webClient.DownloadFileAsync(new Uri(downloadUrl), tempFile);
                        downloadCompleted.WaitOne(); // Wait until the download completes.
                        Console.WriteLine(); // Move to next line after progress bar.
                    }
                    DisplayMessage("Download completed.", ConsoleColor.Green);

                    DisplayMessage("Starting installer...", ConsoleColor.Green);
                    Process installerProcess = Process.Start(tempFile);
                    installerProcess.WaitForExit();
                    DisplayMessage("Installer process completed.", ConsoleColor.Green);

                    File.Delete(tempFile);
                }
            }
            catch (Exception ex)
            {
                DisplayMessage("An error occurred: " + ex.Message, ConsoleColor.Red);
            }
        }

        public static void DisableWindowsUpdate()
        {
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

        static void StopService(string serviceName)
        {
            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    if (sc.Status != ServiceControllerStatus.Stopped &&
                        sc.Status != ServiceControllerStatus.StopPending)
                    {
                        DisplayMessage($"Stopping service {serviceName}...", ConsoleColor.Yellow);
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                        DisplayMessage($"{serviceName} stopped successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        DisplayMessage($"{serviceName} is already stopped.", ConsoleColor.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error stopping service {serviceName}: {ex.Message}", ConsoleColor.Red);
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
                        DisplayMessage($"{serviceName} startup type set to {startMode}.", ConsoleColor.Green);
                    else
                        DisplayMessage($"Failed to change startup type for {serviceName}. Error code: {returnValue}", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error setting startup type for {serviceName}: {ex.Message}", ConsoleColor.Red);
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
                            DisplayMessage($"Disabled scheduled task: {taskName}", ConsoleColor.Green);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error disabling scheduled tasks: {ex.Message}", ConsoleColor.Red);
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
                        DisplayMessage("Registry updated: DoNotConnectToWindowsUpdateInternetLocations set to 1.", ConsoleColor.Green);
                    }
                    else
                    {
                        DisplayMessage("Failed to open or create the WindowsUpdate registry key.", ConsoleColor.Red);
                    }
                }

                // 2. Disable Automatic Updates by modifying Windows Update policies.
                using (RegistryKey auKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU"))
                {
                    if (auKey == null)
                        throw new Exception("Failed to open or create the WindowsUpdate\\AU registry key.");

                    auKey.SetValue("NoAutoUpdate", 1, RegistryValueKind.DWord);
                    auKey.SetValue("AUOptions", 2, RegistryValueKind.DWord);
                    DisplayMessage("Registry updated: Automatic updates disabled (NoAutoUpdate=1, AUOptions=2).", ConsoleColor.Green);
                }

                // 3. Disable Windows Update Delivery Optimization (applies to Windows 10).
                using (RegistryKey doKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DeliveryOptimization\Config", writable: true))
                {
                    if (doKey != null)
                    {
                        doKey.SetValue("DODownloadMode", 0, RegistryValueKind.DWord);
                        DisplayMessage("Registry updated: Delivery Optimization disabled (DODownloadMode=0).", ConsoleColor.Green);
                    }
                    else
                    {
                        DisplayMessage("Delivery Optimization registry key not found.", ConsoleColor.Red);
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
                    DisplayMessage("Registry updated: Update pause settings configured.", ConsoleColor.Green);
                }
            }
            catch (Exception ex)
            {
                DisplayMessage("Registry configuration error: " + ex.Message, ConsoleColor.Red);
                DisplayMessage("Registry configuration error stack trace: " + ex.StackTrace, ConsoleColor.Red);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        static void SetCustomWallpaper()
        {
            string url = "https://i.imgur.com/bXtHBpd.png";
            string picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string savePath = Path.Combine(picturesFolder, "DesktopBackgrDisplayMessageDisplayMessageDisplayMessageound.png");

            DownloadImage(url, savePath);
            SetWallpaper(savePath);
        }

        static void DownloadImage(string url, string savePath)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(url, savePath);
                    DisplayMessage($"Desktop background downloaded to: {savePath}", ConsoleColor.Green);
                }
            }
            catch (Exception ex)
            {
                DisplayMessage($"Failed to download desktop background: {ex.Message}", ConsoleColor.Red);
            }
        }

        static void SetWallpaper(string imagePath)
        {
            try
            {
                int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
                if (result != 0)
                {
                    DisplayMessage("Task completed.", ConsoleColor.Green);
                }
                else
                {
                    DisplayMessage("Task failed.", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error setting background: {ex.Message}", ConsoleColor.Red);
            }
        }

        // Checks if the current process is running as administrator.
        static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        // Helper method to run external commands.
        static void RunCommand(string command, string arguments, bool waitForExit = true)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                using (Process proc = Process.Start(psi))
                {
                    if (waitForExit)
                    {
                        proc.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error running command {command} {arguments}: {ex.Message}", ConsoleColor.Red);
            }
        }

        // Private nested class to encapsulate a registry modification.
        private class RegistryModification
        {
            public RegistryKey Root { get; }
            public string SubKey { get; }
            public string ValueName { get; }
            public RegistryValueKind ValueKind { get; }
            public object Value { get; }

            public RegistryModification(RegistryKey root, string subKey, string valueName, RegistryValueKind valueKind, object value)
            {
                Root = root;
                SubKey = subKey;
                ValueName = valueName;
                ValueKind = valueKind;
                Value = value;
            }
        }

        // ---------------------------
        // Script 1: Registry Tweaker
        // ---------------------------
        public static void ApplyRegistryChanges()
        {
            DisplayMessage("Applying registry changes...", ConsoleColor.Yellow);
            try
            {
                RegistryModification[] registryModifications = new RegistryModification[]
                {
                    // Visual changes
                    new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAl", RegistryValueKind.DWord, 0), // Align taskbar to the left
                    new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", RegistryValueKind.DWord, 0), // Set Windows to dark theme
                    new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", RegistryValueKind.DWord, 0), // Set Windows to dark theme
                    new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent", "AccentColorMenu", RegistryValueKind.DWord, 1), // Use accent color for taskbar/start menu
                    new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", RegistryValueKind.DWord, 1), // (Redundant?) Use accent color for taskbar/start menu
                    new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\DWM", "AccentColorInStartAndTaskbar", RegistryValueKind.DWord, 1), // Use accent color for taskbar/start menu
                    new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent", "AccentPalette", RegistryValueKind.Binary, new byte[32]), // Makes the taskbar black
                    new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", RegistryValueKind.DWord, 0), // Fix the app capture popup
                    new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\PolicyManager\default\ApplicationManagement\AllowGameDVR", "Value", RegistryValueKind.DWord, 0), // Disable Game DVR
                    new RegistryModification(Registry.CurrentUser, @"Control Panel\Desktop", "MenuShowDelay", RegistryValueKind.String, "0"), // Reduce menu delay
                    new RegistryModification(Registry.CurrentUser, @"Control Panel\Desktop\WindowMetrics", "MinAnimate", RegistryValueKind.DWord, 0), // Disable minimize/maximize animations
                    new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ExtendedUIHoverTime", RegistryValueKind.DWord, 1), // Reduce UI hover time
                    new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", RegistryValueKind.DWord, 0), // Show file extensions
                    new RegistryModification(Registry.CurrentUser, @"Control Panel\Colors", "Hilight", RegistryValueKind.String, "0 0 0"), // Sets highlight color to black
                    new RegistryModification(Registry.CurrentUser, @"Control Panel\Colors", "HotTrackingColor", RegistryValueKind.String, "0 0 0") // Sets click-and-drag box color to black
                };

                // Apply each registry modification.
                foreach (RegistryModification mod in registryModifications)
                {
                    try
                    {
                        using (RegistryKey key = mod.Root.CreateSubKey(mod.SubKey, RegistryKeyPermissionCheck.ReadWriteSubTree))
                        {
                            if (key != null)
                            {
                                key.SetValue(mod.ValueName, mod.Value, mod.ValueKind);
                                DisplayMessage($"Applied {mod.ValueName} to {mod.SubKey}", ConsoleColor.Green);
                            }
                            else
                            {
                                DisplayMessage($"Failed to open registry key: {mod.SubKey}", ConsoleColor.Red);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DisplayMessage($"Failed to modify {mod.ValueName} in {mod.SubKey}: {ex.Message}", ConsoleColor.Red);
                    }
                }
                DisplayMessage("Registry changes applied successfully.", ConsoleColor.Green);

                // Kill Explorer and restart it.
                ProcessStartInfo psiKill = new ProcessStartInfo("taskkill", "/F /IM explorer.exe")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psiKill)?.WaitForExit();
                Process.Start("explorer.exe");
                DisplayMessage("Explorer restarted to apply registry changes.", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error applying registry changes: {ex.Message}", ConsoleColor.Red);
            }
        }

        // -------------------------
        // Script 2: Edge Vanisher
        // -------------------------
        static void UninstallEdge()
        {
            DisplayMessage("Edge Vanisher started", ConsoleColor.Yellow);
            DisplayMessage("Starting Microsoft Edge uninstallation process...", ConsoleColor.Yellow);

            // Terminate Edge processes.
            DisplayMessage("Terminating Edge processes...", ConsoleColor.Cyan);
            var edgeProcesses = Process.GetProcesses()
                .Where(p => p.ProcessName.IndexOf("edge", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            if (edgeProcesses.Any())
            {
                foreach (var proc in edgeProcesses)
                {
                    try
                    {
                        DisplayMessage($"Terminated process: {proc.ProcessName} (PID: {proc.Id})", ConsoleColor.Cyan);
                        proc.Kill();
                    }
                    catch { }
                }
            }
            else
            {
                DisplayMessage("No running Edge processes found.", ConsoleColor.Cyan);
            }

            // Uninstall Edge via its setup.exe.
            DisplayMessage("Uninstalling Edge with setup...", ConsoleColor.Cyan);
            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            try
            {
                string edgeInstallerDirectory = Path.Combine(programFilesX86, "Microsoft", "Edge", "Application");
                if (Directory.Exists(edgeInstallerDirectory))
                {
                    var installerDirs = Directory.GetDirectories(edgeInstallerDirectory);
                    foreach (var dir in installerDirs)
                    {
                        string setupPath = Path.Combine(dir, "Installer", "setup.exe");
                        if (File.Exists(setupPath))
                        {
                            RunCommand(setupPath, "--uninstall --system-level --verbose-logging --force-uninstall");
                            break;
                        }
                    }
                }
            } catch { }

            // Remove all shortcut containing msedge.exe
            string[] directories =
            {
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)
            };

            foreach (string dir in directories)
            {
                if (!Directory.Exists(dir)) continue;

                foreach (string shortcut in Directory.GetFiles(dir, "*.lnk", SearchOption.AllDirectories))
                {
                    try
                    {
                        var shell = new IWshRuntimeLibrary.WshShell();
                        var link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcut);

                        if (link.TargetPath.EndsWith("msedge.exe", StringComparison.OrdinalIgnoreCase))
                        {
                            File.Delete(shortcut);
                            DisplayMessage($"Deleted: {shortcut}", ConsoleColor.Green);
                        }
                    }
                    catch { /* Ignore invalid shortcuts */ }
                }
            }

            // Remove Start Menu shortcuts.
            DisplayMessage("Removing Start Menu shortcuts...", ConsoleColor.Cyan);
            string[] startMenuPaths = new string[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Microsoft Edge.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "Microsoft Edge.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Microsoft Edge.lnk")
            };
            foreach (var shortcut in startMenuPaths)
            {
                if (File.Exists(shortcut))
                {
                    DisplayMessage($"Deleting: {shortcut}", ConsoleColor.Cyan);
                    try
                    {
                        File.Delete(shortcut);
                        if (!File.Exists(shortcut))
                        {
                            DisplayMessage($"Successfully deleted: {shortcut}", ConsoleColor.Green);
                        }
                        else
                        {
                            DisplayMessage($"Failed to delete: {shortcut}", ConsoleColor.Red);
                        }
                    }
                    catch (Exception ex)
                    {
                        DisplayMessage($"Error deleting {shortcut}: {ex.Message}", ConsoleColor.Red);
                    }
                }
            }

            // Clean Edge folders.
            DisplayMessage("Cleaning Edge folders...", ConsoleColor.Cyan);
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string[] edgePaths = new string[]
            {
                Path.Combine(localAppData, "Microsoft", "Edge"),
                Path.Combine(programFiles, "Microsoft", "Edge"),
                Path.Combine(programFilesX86, "Microsoft", "Edge"),
                Path.Combine(programFilesX86, "Microsoft", "EdgeUpdate"),
                Path.Combine(programFilesX86, "Microsoft", "EdgeCore"),
                Path.Combine(localAppData, "Microsoft", "EdgeUpdate"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft", "EdgeUpdate")
            };
            foreach (var path in edgePaths)
            {
                if (Directory.Exists(path) || File.Exists(path))
                {
                    DisplayMessage($"Cleaning: {path}", ConsoleColor.Cyan);
                    // Use external commands to take ownership and set permissions.
                    RunCommand("takeown", $"/F \"{path}\" /R /D Y");
                    RunCommand("icacls", $"\"{path}\" /grant administrators:F /T");
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);
                        }
                        else if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                    catch (Exception ex)
                    {
                        DisplayMessage($"Error deleting {path}: {ex.Message}", ConsoleColor.Red);
                    }
                }
            }

            // Clean Edge registry entries.
            DisplayMessage("Cleaning Edge registry entries...", ConsoleColor.Cyan);
            string[] edgeRegKeys = new string[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Edge",
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Edge Update",
                @"SOFTWARE\Microsoft\EdgeUpdate",
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe",
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft EdgeUpdate",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft EdgeUpdate",
                @"SOFTWARE\Microsoft\Edge",
                @"SOFTWARE\WOW6432Node\Microsoft\Edge",
                @"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Edge",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Edge Update"
            };
            foreach (var regKey in edgeRegKeys)
            {
                DeleteRegistryKey(Registry.LocalMachine, regKey);
            }
            // Delete the HKCU key for Edge.
            DeleteRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Edge");

            // Force uninstall EdgeUpdate.
            string edgeUpdatePath = Path.Combine(programFilesX86, "Microsoft", "EdgeUpdate", "MicrosoftEdgeUpdate.exe");
            if (File.Exists(edgeUpdatePath))
            {
                RunCommand(edgeUpdatePath, "/uninstall");
            }

            // Remove EdgeUpdate services.
            string[] services = new string[]
            {
                "edgeupdate",
                "edgeupdatem",
                "MicrosoftEdgeElevationService"
            };
            foreach (var service in services)
            {
                try
                {
                    RunCommand("sc", $"stop {service}");
                    RunCommand("sc", $"delete {service}");
                }
                catch (Exception ex)
                {
                    DisplayMessage($"Error handling service {service}: {ex.Message}", ConsoleColor.Red);
                }
            }

            // Finally force uninstall Edge (if needed).
            try
            {
                var edgeSetupFiles = Directory.GetFiles(Path.Combine(programFilesX86, "Microsoft", "Edge", "Application"), "setup.exe", SearchOption.AllDirectories);
                if (edgeSetupFiles.Length > 0)
                {
                    RunCommand(edgeSetupFiles[0], "--uninstall --system-level --verbose-logging --force-uninstall");
                }
            }
            catch { }

            // Restart Explorer.
            try
            {
                foreach (var proc in Process.GetProcessesByName("explorer"))
                {
                    proc.Kill();
                }
            }
            catch { }
            Thread.Sleep(1000);
            Process.Start("explorer");

            DisplayMessage("Microsoft Edge uninstallation process completed!", ConsoleColor.Green);

            // Create protective Edge folders and set security.
            DisplayMessage("Creating protective Edge folders...", ConsoleColor.Cyan);
            var protectiveFolders = new[]
            {
                new { Base = Path.Combine(programFilesX86, "Microsoft", "Edge"), App = Path.Combine(programFilesX86, "Microsoft", "Edge", "Application"), CreateSubFolder = true },
                new { Base = Path.Combine(programFilesX86, "Microsoft", "EdgeCore"), App = (string)null, CreateSubFolder = false }
            };

            foreach (var folder in protectiveFolders)
            {
                try
                {
                    Directory.CreateDirectory(folder.Base);
                    if (folder.CreateSubFolder && folder.App != null)
                    {
                        Directory.CreateDirectory(folder.App);
                    }
                    DisplayMessage($"Processing protective folder: {folder.Base}", ConsoleColor.Cyan);

                    string currentUser = WindowsIdentity.GetCurrent().Name;
                    // Set ACL for the folder.
                    DirectorySecurity acl = new DirectorySecurity();
                    acl.SetOwner(new System.Security.Principal.NTAccount(currentUser));
                    acl.SetAccessRuleProtection(true, false);
                    acl.AddAccessRule(new FileSystemAccessRule(
                        currentUser,
                        FileSystemRights.FullControl | FileSystemRights.TakeOwnership | FileSystemRights.ChangePermissions,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow));

                    // Deny certain rights for SYSTEM, Administrators, TrustedInstaller, Authenticated Users.
                    acl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier("S-1-5-18"),
                        FileSystemRights.TakeOwnership | FileSystemRights.ChangePermissions,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None, AccessControlType.Deny));
                    acl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier("S-1-5-32-544"),
                        FileSystemRights.TakeOwnership | FileSystemRights.ChangePermissions,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None, AccessControlType.Deny));
                    acl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier("S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464"),
                        FileSystemRights.TakeOwnership | FileSystemRights.ChangePermissions,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None, AccessControlType.Deny));
                    acl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier("S-1-5-11"),
                        FileSystemRights.TakeOwnership | FileSystemRights.ChangePermissions,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None, AccessControlType.Deny));

                    Directory.SetAccessControl(folder.Base, acl);

                    if (folder.CreateSubFolder)
                    {
                        // Recursively apply ACL to subdirectories.
                        foreach (string subDir in Directory.GetDirectories(folder.Base, "*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                Directory.SetAccessControl(subDir, acl);
                                DisplayMessage($"Success: {subDir}", ConsoleColor.Green);
                            }
                            catch (Exception ex)
                            {
                                DisplayMessage($"Error occurred: {subDir} - {ex.Message}", ConsoleColor.Red);
                            }
                        }
                    }
                    else
                    {
                        DisplayMessage($"Success: {folder.Base}", ConsoleColor.Green);
                    }
                }
                catch (Exception ex)
                {
                    DisplayMessage($"Error occurred: {folder.Base} - {ex.Message}", ConsoleColor.Red);
                }
            }
            DisplayMessage("Protective folders created and security settings configured for Edge and EdgeCore.", ConsoleColor.Green);
        }

        // -------------------------------------
        // Script 3: Outlook & OneDrive Cleaner
        // -------------------------------------
        static void CleanOutlookAndOneDrive()
        {
            DisplayMessage("Outlook & OneDrive Cleaner started", ConsoleColor.Yellow);

            // Close Outlook processes.
            var outlookProcesses = Process.GetProcesses()
                .Where(p => p.ProcessName.IndexOf("outlook", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            foreach (var proc in outlookProcesses)
            {
                try
                {
                    DisplayMessage($"Terminating Outlook process: {proc.ProcessName} (PID: {proc.Id})", ConsoleColor.Cyan);
                    proc.Kill();
                }
                catch { }
            }
            Thread.Sleep(2000);

            // Remove Outlook apps (using PowerShell commands).
            RunCommand("powershell", "-Command \"Get-AppxPackage *Microsoft.Office.Outlook* | Remove-AppxPackage\"");
            RunCommand("powershell", "-Command \"Get-AppxProvisionedPackage -Online | Where-Object {$_.PackageName -like '*Microsoft.Office.Outlook*'} | Remove-AppxProvisionedPackage -Online\"");
            RunCommand("powershell", "-Command \"Get-AppxPackage *Microsoft.OutlookForWindows* | Remove-AppxPackage\"");
            RunCommand("powershell", "-Command \"Get-AppxProvisionedPackage -Online | Where-Object {$_.PackageName -like '*Microsoft.OutlookForWindows*'} | Remove-AppxProvisionedPackage -Online\"");

            // Remove Outlook folders.
            string windowsAppsPath = @"C:\Program Files\WindowsApps";
            if (Directory.Exists(windowsAppsPath))
            {
                var outlookFolders = Directory.GetDirectories(windowsAppsPath, "Microsoft.OutlookForWindows*");
                foreach (var folder in outlookFolders)
                {
                    try
                    {
                        RunCommand("takeown", $"/f \"{folder}\" /r /d Y");
                        RunCommand("icacls", $"\"{folder}\" /grant administrators:F /T");
                        Directory.Delete(folder, true);
                        DisplayMessage($"Deleted Outlook folder: {folder}", ConsoleColor.Green);
                    }
                    catch (Exception ex)
                    {
                        DisplayMessage($"Error deleting folder {folder}: {ex.Message}", ConsoleColor.Red);
                    }
                }
            }

            // Remove all shortcut containing outlook.exe and onedrive.exe
            string[] directories =
            {
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)
            };

            foreach (string dir in directories)
            {
                if (!Directory.Exists(dir)) continue;

                foreach (string shortcut in Directory.GetFiles(dir, "*.lnk", SearchOption.AllDirectories))
                {
                    try
                    {
                        var shell = new IWshRuntimeLibrary.WshShell();
                        var link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcut);

                        if (link.TargetPath.EndsWith("outlook.exe", StringComparison.OrdinalIgnoreCase))
                        {
                            File.Delete(shortcut);
                            DisplayMessage($"Deleted: {shortcut}", ConsoleColor.Green);
                        }

                        if (link.TargetPath.EndsWith("onedrive.exe", StringComparison.OrdinalIgnoreCase))
                        {
                            File.Delete(shortcut);
                            DisplayMessage($"Deleted: {shortcut}", ConsoleColor.Green);
                        }
                    }
                    catch { /* Ignore invalid shortcuts */ }
                }
            }

            // Remove Outlook shortcuts.
            string[] outlookShortcutPaths = new string[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Outlook.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "Outlook.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Microsoft Office", "Outlook.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "Microsoft Office", "Outlook.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), "Outlook.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Outlook.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), "Microsoft Outlook.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Microsoft Outlook.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), "Outlook (New).lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Outlook (New).lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Outlook (New).lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "Outlook (New).lnk")
            };
            foreach (var shortcut in outlookShortcutPaths)
            {
                if (File.Exists(shortcut))
                {
                    try
                    {
                        File.Delete(shortcut);
                        DisplayMessage($"Deleted shortcut: {shortcut}", ConsoleColor.Green);
                    }
                    catch (Exception ex)
                    {
                        DisplayMessage($"Error deleting shortcut {shortcut}: {ex.Message}", ConsoleColor.Red);
                    }
                }
            }

            // Taskbar cleanup: remove certain registry values.
            string[] registryPaths = new string[]
            {
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband",
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\TaskbarMRU",
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\TaskBar",
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced"
            };
            foreach (var regPath in registryPaths)
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(regPath, true))
                    {
                        if (key != null)
                        {
                            string[] valueNames = new string[] { "Favorites", "FavoritesResolve", "FavoritesChanges", "FavoritesRemovedChanges", "TaskbarWinXP", "PinnedItems" };
                            foreach (var val in valueNames)
                            {
                                try { key.DeleteValue(val); } catch { }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    DisplayMessage($"Error cleaning registry at {regPath}: {ex.Message}", ConsoleColor.Red);
                }
            }
            // Set ShowTaskViewButton to 0.
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true))
                {
                    if (key != null)
                    {
                        key.SetValue("ShowTaskViewButton", 0, RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error setting ShowTaskViewButton: {ex.Message}", ConsoleColor.Red);
            }

            // Remove LayoutModification.xml and icon/thumbnail caches.
            string localApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string layoutFile = Path.Combine(localApp, "Microsoft", "Windows", "Shell", "LayoutModification.xml");
            if (File.Exists(layoutFile))
            {
                try { File.Delete(layoutFile); } catch { }
            }
            string explorerFolder = Path.Combine(localApp, "Microsoft", "Windows", "Explorer");
            if (Directory.Exists(explorerFolder))
            {
                foreach (var pattern in new string[] { "iconcache*", "thumbcache*" })
                {
                    foreach (var file in Directory.GetFiles(explorerFolder, pattern))
                    {
                        try { File.Delete(file); } catch { }
                    }
                }
            }

            // OneDrive removal.
            var oneDriveProcesses = Process.GetProcesses()
                .Where(p => p.ProcessName.IndexOf("onedrive", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            foreach (var proc in oneDriveProcesses)
            {
                try { proc.Kill(); } catch { }
            }
            string systemRoot = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string oneDriveSetupPath = Path.Combine(systemRoot, "SysWOW64", "OneDriveSetup.exe");
            if (!File.Exists(oneDriveSetupPath))
            {
                oneDriveSetupPath = Path.Combine(systemRoot, "System32", "OneDriveSetup.exe");
            }
            if (File.Exists(oneDriveSetupPath))
            {
                RunCommand(oneDriveSetupPath, "/uninstall");
            }

            string[] oneDrivePaths = new string[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "OneDrive.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "OneDrive.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), "OneDrive.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "OneDrive.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "OneDrive"),
                Path.Combine(localApp, "Microsoft", "OneDrive"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft", "OneDrive"),
                Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "OneDriveTemp")
            };
            foreach (var path in oneDrivePaths)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                        DisplayMessage($"Deleted directory: {path}", ConsoleColor.Green);
                    }
                    else if (File.Exists(path))
                    {
                        File.Delete(path);
                        DisplayMessage($"Deleted file: {path}", ConsoleColor.Green);
                    }
                }
                catch (Exception ex)
                {
                    DisplayMessage($"Error deleting {path}: {ex.Message}", ConsoleColor.Red);
                }
            }

            string[] oneDriveRegKeys = new string[]
            {
                @"CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}",
                @"Wow6432Node\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}",
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace\{018D5C66-4533-4307-9B53-224DE2ED1FE6}"
            };
            foreach (var regKey in oneDriveRegKeys)
            {
                DeleteRegistryKey(Registry.ClassesRoot, regKey);
            }

            // Restart Explorer.
            try
            {
                foreach (var proc in Process.GetProcessesByName("explorer"))
                {
                    proc.Kill();
                }
            }
            catch { }
            Thread.Sleep(2000);
            Process.Start("explorer");

            DisplayMessage("Outlook and OneDrive removal process completed!", ConsoleColor.Green);
        }

        /// <summary>
        /// Downloads a PowerShell script from a URL, saves it to 
        /// the temp directory, runs it with specific parameters.
        /// </summary>
        public static void RunWinConfig()
        {
            DisplayMessage("Starting Windows configuration process...", ConsoleColor.Yellow);
            try
            {
                string scriptUrl = "https://win11debloat.raphi.re/";
                string tempDir = Path.GetTempPath();
                string scriptPath = Path.Combine(tempDir, "Win11Debloat.ps1");

                DisplayMessage("Attempting to download Windows configuration script from: " + scriptUrl, ConsoleColor.Cyan);
                DisplayMessage("Target script path: " + scriptPath, ConsoleColor.Cyan);

                using (WebClient client = new WebClient())
                {
                    // Synchronously download the script.
                    byte[] content = client.DownloadData(scriptUrl);
                    File.WriteAllBytes(scriptPath, content);
                }
                DisplayMessage("Windows configuration script successfully saved to disk", ConsoleColor.Green);

                // Build the PowerShell command string.
                string powershellCommand =
                    "Set-ExecutionPolicy Bypass -Scope Process -Force; " +
                    "& '" + scriptPath + "' -Silent -RemoveApps -RemoveGamingApps -DisableTelemetry " +
                    "-DisableBing -DisableSuggestions -DisableLockscreenTips -RevertContextMenu " +
                    "-TaskbarAlignLeft -HideSearchTb -DisableWidgets -DisableCopilot -ExplorerToThisPC " +
                    "-ClearStartAllUsers -DisableDVR -DisableStartRecommended " +
                    "-DisableMouseAcceleration";

                DisplayMessage("Executing PowerShell command with parameters:", ConsoleColor.Cyan);
                DisplayMessage("Command: " + powershellCommand, ConsoleColor.Cyan);

                ProcessStartInfo psi = new ProcessStartInfo("powershell", "-Command \"" + powershellCommand + "\"")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,  // Required for redirection.
                    CreateNoWindow = true       // We'll output to our console.
                };

                using (Process psProcess = new Process())
                {
                    psProcess.StartInfo = psi;

                    // Handle standard output asynchronously.
                    psProcess.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            // Now you can look in this exact copy of what you've been outputting.
                            DisplayMessage(e.Data, ConsoleColor.Cyan);
                        }
                    };

                    // Handle standard error asynchronously.
                    psProcess.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            DisplayMessage(e.Data, ConsoleColor.Red);
                        }
                    };

                    // Start the process and begin asynchronous read.
                    psProcess.Start();
                    psProcess.BeginOutputReadLine();
                    psProcess.BeginErrorReadLine();

                    // Wait until the process exits.
                    psProcess.WaitForExit();
                }
            }
            catch (Exception e)
            {
                DisplayMessage("Unexpected error during Windows configuration: " + e.Message, ConsoleColor.Red);
            }
        }

        /// <summary>
        /// Runs a PowerShell command that processes the config.
        /// It monitors the process output for a completion message.
        /// </summary>
        static bool shouldExit = false; // Flag to track exit condition
        public static void RunTweaks()
        {
            try
            {
                // Directly use the provided JSON content.
                string jsonConfig = @"
                {
                    ""WPFTweaks"":  [
                      ""WPFTweaksRestorePoint"",
                      ""WPFTweaksWifi"",
                      ""WPFTweaksHome"",
                      ""WPFTweaksRemoveEdge"",
                      ""WPFTweaksRemoveHomeGallery"",
                      ""WPFTweaksDisableLMS1"",
                      ""WPFTweaksIPv46"",
                      ""WPFTweaksDeBloat"",
                      ""WPFTweaksConsumerFeatures"",
                      ""WPFTweaksTele"",
                      ""WPFTweaksDisplay"",
                      ""WPFTweaksAH"",
                      ""WPFTweaksRightClickMenu"",
                      ""WPFTweaksRemoveCopilot"",
                      ""WPFTweaksLoc"",
                      ""WPFTweaksRemoveOnedrive"",
                      ""WPFTweaksServices"",
                      ""WPFTweaksDeleteTempFiles"",
                      ""WPFTweaksRecallOff"",
                      ""WPFTweaksDisableBGapps""
                  ],
                    ""WPFFeature"":  [
                        ""WPFFeaturesSandbox"",
                        ""WPFFeatureshyperv""
                    ]
                }";

                // Write JSON configuration to a temporary file.
                string tempDir = Path.GetTempPath();
                string jsonPath = Path.Combine(tempDir, "custom_config.json");
                File.WriteAllText(jsonPath, jsonConfig);
                string logFile = Path.Combine(tempDir, "cttwinutil.log");

                // Construct the PowerShell command.
                string command = "$ErrorActionPreference = 'SilentlyContinue'; " +
                                 "iex \"& { $(irm christitus.com/win) } -Config '" + jsonPath + "' -Run\" *>&1 | " +
                                 "Tee-Object -FilePath '" + logFile + "'";

                // Encode the command in Unicode and then to Base64.
                byte[] commandBytes = Encoding.Unicode.GetBytes(command);
                string encodedCommand = Convert.ToBase64String(commandBytes);

                // Set up the process to run PowerShell with the encoded command.
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = $"-NoProfile -NonInteractive -EncodedCommand {encodedCommand}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,  // Required for redirection.
                    CreateNoWindow = true       // We'll output to our console.
                };

                using (Process psProcess = new Process())
                {
                    psProcess.StartInfo = psi;

                    // Handle standard output asynchronously.
                    psProcess.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            // Now you can look in this exact copy of what you've been outputting.
                            DisplayMessage(e.Data, ConsoleColor.Cyan);
                            if (e.Data.Contains("Tweaks are Finished"))
                            {
                                DisplayMessage("DEBUG: This is the end!\nProcess will now exit gracefully.", ConsoleColor.Green);
                                shouldExit = true; // Set exit flag
                            }
                        }
                    };

                    // Handle standard error asynchronously.
                    psProcess.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            DisplayMessage(e.Data, ConsoleColor.Red);
                        }
                    };

                    // Start the process and begin asynchronous read.
                    psProcess.Start();
                    psProcess.BeginOutputReadLine();
                    psProcess.BeginErrorReadLine();

                    while (!psProcess.HasExited)
                    {
                        if (shouldExit) // Check flag
                        {
                            DisplayMessage("Stopping process safely...", ConsoleColor.Green);

                            psProcess.CloseMainWindow(); // Try closing first
                            psProcess.WaitForExit(3000);

                            if (!psProcess.HasExited)
                            {
                                psProcess.Kill(); // Force kill if needed
                            }
                            return;
                        }

                        Thread.Sleep(500); // Prevent high CPU usage
                    }

                    // Wait until the process exits.
                    psProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                DisplayMessage("Error: " + ex.Message, ConsoleColor.Red);
            }
        }

        // Helper to delete registry keys recursively.
        static void DeleteRegistryKey(RegistryKey root, string subKeyPath)
        {
            try
            {
                root.DeleteSubKeyTree(subKeyPath, false);
                DisplayMessage($"Successfully deleted registry key: {root.Name}\\{subKeyPath}", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                DisplayMessage($"Failed to delete registry key: {root.Name}\\{subKeyPath} - {ex.Message}", ConsoleColor.Red);
            }
        }

        static void DisplayMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}