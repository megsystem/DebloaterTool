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
            Console.Title = "DebloaterTool by ig:@_giovannigiannone tg:fckgiovanni";
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
            Console.ForegroundColor = ConsoleColor.Red; // Set text color to red
            Console.WriteLine("3. Please disable your antivirus before proceeding.");
            Console.ResetColor(); // Reset to default color
            Console.WriteLine("---------------------------------");
            Console.Write("Do you accept the EULA? (yes/no): ");

            string response = Console.ReadLine()?.Trim().ToLower();

            if (response == "yes")
            {
                Console.WriteLine("EULA accepted. You may proceed.");
            }
            else if (response == "no")
            {
                Console.WriteLine("EULA declined. Exiting application.");
                Console.ReadKey();
                return;
            }
            else
            {
                Console.WriteLine("Invalid response. Please restart and enter 'yes' or 'no'.");
                Console.ReadKey();
                return;
            }

            if (!IsAdministrator())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This application must be run with administrator rights!");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            bool restart = false;
            while (true)
            {
                Console.Write("Do you want to restart after the process? (yes/no): ");
                string restartResponse = Console.ReadLine()?.Trim().ToLower();

                if (restartResponse == "yes")
                {
                    restart = true;
                    break;
                }
                else if (restartResponse == "no")
                {
                    restart = false;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
                }
            }

            RunTweaks();
            RunWinConfig();
            ApplyRegistryChanges();
            UninstallEdge();
            CleanOutlookAndOneDrive();
            SetCustomWallpaper();

            if (restart)
            {
                Process.Start("shutdown.exe", "-r -t 0"); // Restart the computer
            }
            else
            {
                Console.WriteLine("Restart skipped. Process completed.");
                Console.ReadLine(); // Wait for user to press Enter
            }

            return;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        static void SetCustomWallpaper()
        {
            string url = "https://i.imgur.com/p4oQOQM.jpeg";
            string picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string savePath = Path.Combine(picturesFolder, "DesktopBackground.png");

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
                    Console.WriteLine($"Desktop background downloaded to: {savePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to download desktop background: {ex.Message}");
            }
        }

        static void SetWallpaper(string imagePath)
        {
            try
            {
                int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
                if (result != 0)
                {
                    Console.WriteLine("Task completed.");
                }
                else
                {
                    Console.WriteLine("Task failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting background: {ex.Message}");
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error running command {command} {arguments}: {ex.Message}");
                Console.ResetColor();
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
            Console.WriteLine("Applying registry changes...");
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
                                Console.WriteLine($"Applied {mod.ValueName} to {mod.SubKey}");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to open registry key: {mod.SubKey}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to modify {mod.ValueName} in {mod.SubKey}: {ex.Message}");
                    }
                }
                Console.WriteLine("Registry changes applied successfully.");

                // Kill Explorer and restart it.
                ProcessStartInfo psiKill = new ProcessStartInfo("taskkill", "/F /IM explorer.exe")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psiKill)?.WaitForExit();
                Process.Start("explorer.exe");
                Console.WriteLine("Explorer restarted to apply registry changes.");

                // Call the Edge Vanisher routine.
                Console.WriteLine("Edge Vanisher started successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying registry changes: {ex.Message}");
            }
        }

        // -------------------------
        // Script 2: Edge Vanisher
        // -------------------------
        static void UninstallEdge()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Edge Vanisher started");
            Console.WriteLine("Starting Microsoft Edge uninstallation process...");
            Console.ResetColor();

            // Terminate Edge processes.
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Terminating Edge processes...");
            Console.ResetColor();
            var edgeProcesses = Process.GetProcesses()
                .Where(p => p.ProcessName.IndexOf("edge", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            if (edgeProcesses.Any())
            {
                foreach (var proc in edgeProcesses)
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Terminated process: {proc.ProcessName} (PID: {proc.Id})");
                        Console.ResetColor();
                        proc.Kill();
                    }
                    catch { }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("No running Edge processes found.");
                Console.ResetColor();
            }

            // Uninstall Edge via its setup.exe.
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Uninstalling Edge with setup...");
            Console.ResetColor();
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

            // Remove Start Menu shortcuts.
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Removing Start Menu shortcuts...");
            Console.ResetColor();
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
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Deleting: {shortcut}");
                    Console.ResetColor();
                    try
                    {
                        File.Delete(shortcut);
                        if (!File.Exists(shortcut))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Successfully deleted: {shortcut}");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Failed to delete: {shortcut}");
                            Console.ResetColor();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error deleting {shortcut}: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }

            // Clean Edge folders.
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Cleaning Edge folders...");
            Console.ResetColor();
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
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Cleaning: {path}");
                    Console.ResetColor();
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
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error deleting {path}: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }

            // Clean Edge registry entries.
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Cleaning Edge registry entries...");
            Console.ResetColor();
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error handling service {service}: {ex.Message}");
                    Console.ResetColor();
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

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nMicrosoft Edge uninstallation process completed!");
            Console.ResetColor();

            // Create protective Edge folders and set security.
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Creating protective Edge folders...");
            Console.ResetColor();
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
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Processing protective folder: {folder.Base}");
                    Console.ResetColor();

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
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Success: {subDir}");
                                Console.ResetColor();
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Error occurred: {subDir} - {ex.Message}");
                                Console.ResetColor();
                            }
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Success: {folder.Base}");
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error occurred: {folder.Base} - {ex.Message}");
                    Console.ResetColor();
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Protective folders created and security settings configured for Edge and EdgeCore.");
            Console.ResetColor();
        }

        // -------------------------------------
        // Script 3: Outlook & OneDrive Cleaner
        // -------------------------------------
        static void CleanOutlookAndOneDrive()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Outlook & OneDrive Cleaner started");
            Console.ResetColor();

            // Close Outlook processes.
            var outlookProcesses = Process.GetProcesses()
                .Where(p => p.ProcessName.IndexOf("outlook", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            foreach (var proc in outlookProcesses)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Terminating Outlook process: {proc.ProcessName} (PID: {proc.Id})");
                    Console.ResetColor();
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
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Deleted Outlook folder: {folder}");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error deleting folder {folder}: {ex.Message}");
                        Console.ResetColor();
                    }
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
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Deleted shortcut: {shortcut}");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error deleting shortcut {shortcut}: {ex.Message}");
                        Console.ResetColor();
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error cleaning registry at {regPath}: {ex.Message}");
                    Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error setting ShowTaskViewButton: {ex.Message}");
                Console.ResetColor();
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
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Deleted directory: {path}");
                        Console.ResetColor();
                    }
                    else if (File.Exists(path))
                    {
                        File.Delete(path);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Deleted file: {path}");
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error deleting {path}: {ex.Message}");
                    Console.ResetColor();
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

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Outlook and OneDrive removal process completed!");
            Console.ResetColor();
        }

        /// <summary>
        /// Downloads a PowerShell script from a URL, saves it to 
        /// the temp directory, runs it with specific parameters.
        /// </summary>
        public static void RunWinConfig()
        {
            Console.WriteLine("Starting Windows configuration process...");
            try
            {
                string scriptUrl = "https://win11debloat.raphi.re/";
                string tempDir = Path.GetTempPath();
                string scriptPath = Path.Combine(tempDir, "Win11Debloat.ps1");

                Console.WriteLine("Attempting to download Windows configuration script from: " + scriptUrl);
                Console.WriteLine("Target script path: " + scriptPath);

                using (WebClient client = new WebClient())
                {
                    // Synchronously download the script.
                    byte[] content = client.DownloadData(scriptUrl);
                    File.WriteAllBytes(scriptPath, content);
                }
                Console.WriteLine("Windows configuration script successfully saved to disk");

                // Build the PowerShell command string.
                string powershellCommand =
                    "Set-ExecutionPolicy Bypass -Scope Process -Force; " +
                    "& '" + scriptPath + "' -Silent -RemoveApps -RemoveGamingApps -DisableTelemetry " +
                    "-DisableBing -DisableSuggestions -DisableLockscreenTips -RevertContextMenu " +
                    "-TaskbarAlignLeft -HideSearchTb -DisableWidgets -DisableCopilot -ExplorerToThisPC " +
                    "-ClearStartAllUsers -DisableDVR -DisableStartRecommended " +
                    "-DisableMouseAcceleration";

                Console.WriteLine("Executing PowerShell command with parameters:");
                Console.WriteLine("Command: " + powershellCommand);

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
                            Console.WriteLine(e.Data);
                        }
                    };

                    // Handle standard error asynchronously.
                    psProcess.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.Error.WriteLine(e.Data);
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
                Console.WriteLine("Unexpected error during Windows configuration: " + e.Message);
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
                            Console.WriteLine(e.Data);
                            if (e.Data.Contains("Tweaks are Finished"))
                            {
                                Console.WriteLine("DEBUG: This is the end!\nProcess will now exit gracefully.");
                                shouldExit = true; // Set exit flag
                            }
                        }
                    };

                    // Handle standard error asynchronously.
                    psProcess.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.Error.WriteLine(e.Data);
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
                            Console.WriteLine("Stopping process safely...");

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
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // Helper to delete registry keys recursively.
        static void DeleteRegistryKey(RegistryKey root, string subKeyPath)
        {
            try
            {
                root.DeleteSubKeyTree(subKeyPath, false);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Successfully deleted registry key: {root.Name}\\{subKeyPath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to delete registry key: {root.Name}\\{subKeyPath} - {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}