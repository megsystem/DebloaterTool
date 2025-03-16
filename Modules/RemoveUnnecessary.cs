using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace DebloaterTool
{
    internal class RemoveUnnecessary
    {
        // ---------------------------
        // Script 1: Registry Tweaker
        // ---------------------------
        public static void ApplyRegistryChanges()
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

            // Install Regedit Modification
            ComRegedit.InstallRegModification(registryModifications);
        }

        // -------------------------
        // Script 2: Edge Vanisher
        // -------------------------
        public static void UninstallEdge()
        {
            Logger.Log("Edge Vanisher started", Level.WARNING);
            Logger.Log("Starting Microsoft Edge uninstallation process...", Level.WARNING);

            // Terminate Edge processes.
            Logger.Log("Terminating Edge processes...", Level.INFO);
            var edgeProcesses = Process.GetProcesses()
                .Where(p => p.ProcessName.IndexOf("edge", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            if (edgeProcesses.Any())
            {
                foreach (var proc in edgeProcesses)
                {
                    try
                    {
                        Logger.Log($"Terminated process: {proc.ProcessName} (PID: {proc.Id})", Level.INFO);
                        proc.Kill();
                    }
                    catch { }
                }
            }
            else
            {
                Logger.Log("No running Edge processes found.", Level.INFO);
            }

            // Uninstall Edge via its setup.exe.
            Logger.Log("Uninstalling Edge with setup...", Level.INFO);
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
                            ComGlobal.RunCommand(setupPath, "--uninstall --system-level --verbose-logging --force-uninstall");
                            break;
                        }
                    }
                }
            }
            catch { }

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
                            Logger.Log($"Deleted: {shortcut}", Level.SUCCESS);
                        }
                    }
                    catch { /* Ignore invalid shortcuts */ }
                }
            }

            // Remove Start Menu shortcuts.
            Logger.Log("Removing Start Menu shortcuts...", Level.INFO);
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
                    Logger.Log($"Deleting: {shortcut}", Level.INFO);
                    try
                    {
                        File.Delete(shortcut);
                        if (!File.Exists(shortcut))
                        {
                            Logger.Log($"Successfully deleted: {shortcut}", Level.SUCCESS);
                        }
                        else
                        {
                            Logger.Log($"Failed to delete: {shortcut}", Level.ERROR);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error deleting {shortcut}: {ex.Message}", Level.ERROR);
                    }
                }
            }

            // Clean Edge folders.
            Logger.Log("Cleaning Edge folders...", Level.INFO);
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
                    Logger.Log($"Cleaning: {path}", Level.INFO);
                    // Use external commands to take ownership and set permissions.
                    ComGlobal.RunCommand("takeown", $"/F \"{path}\" /R /D Y");
                    ComGlobal.RunCommand("icacls", $"\"{path}\" /grant administrators:F /T");
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
                        Logger.Log($"Error deleting {path}: {ex.Message}", Level.ERROR);
                    }
                }
            }

            // Clean Edge registry entries.
            Logger.Log("Cleaning Edge registry entries...", Level.INFO);
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
                ComRegedit.DeleteRegistryKey(Registry.LocalMachine, regKey);
            }
            // Delete the HKCU key for Edge.
            ComRegedit.DeleteRegistryKey(Registry.CurrentUser, @"Software\Microsoft\Edge");

            // Force uninstall EdgeUpdate.
            string edgeUpdatePath = Path.Combine(programFilesX86, "Microsoft", "EdgeUpdate", "MicrosoftEdgeUpdate.exe");
            if (File.Exists(edgeUpdatePath))
            {
                ComGlobal.RunCommand(edgeUpdatePath, "/uninstall");
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
                    ComGlobal.RunCommand("sc", $"stop {service}");
                    ComGlobal.RunCommand("sc", $"delete {service}");
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error handling service {service}: {ex.Message}", Level.ERROR);
                }
            }

            // Finally force uninstall Edge (if needed).
            try
            {
                var edgeSetupFiles = Directory.GetFiles(Path.Combine(programFilesX86, "Microsoft", "Edge", "Application"), "setup.exe", SearchOption.AllDirectories);
                if (edgeSetupFiles.Length > 0)
                {
                    ComGlobal.RunCommand(edgeSetupFiles[0], "--uninstall --system-level --verbose-logging --force-uninstall");
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

            Logger.Log("Microsoft Edge uninstallation process completed!", Level.SUCCESS);

            // Create protective Edge folders and set security.
            Logger.Log("Creating protective Edge folders...", Level.INFO);
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
                    Logger.Log($"Processing protective folder: {folder.Base}", Level.INFO);

                    string currentUser = WindowsIdentity.GetCurrent().Name;
                    // Set ACL for the folder.
                    DirectorySecurity acl = new DirectorySecurity();
                    acl.SetOwner(new NTAccount(currentUser));
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
                                Logger.Log($"Success: {subDir}", Level.SUCCESS);
                            }
                            catch (Exception ex)
                            {
                                Logger.Log($"Error occurred: {subDir} - {ex.Message}", Level.ERROR);
                            }
                        }
                    }
                    else
                    {
                        Logger.Log($"Success: {folder.Base}", Level.SUCCESS);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error occurred: {folder.Base} - {ex.Message}", Level.ERROR);
                }
            }
            Logger.Log("Protective folders created and security settings configured for Edge and EdgeCore.", Level.SUCCESS);
        }

        // -------------------------------------
        // Script 3: Outlook & OneDrive Cleaner
        // -------------------------------------
        public static void CleanOutlookAndOneDrive()
        {
            Logger.Log("Outlook & OneDrive Cleaner started", Level.WARNING);

            // Close Outlook processes.
            var outlookProcesses = Process.GetProcesses()
                .Where(p => p.ProcessName.IndexOf("outlook", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            foreach (var proc in outlookProcesses)
            {
                try
                {
                    Logger.Log($"Terminating Outlook process: {proc.ProcessName} (PID: {proc.Id})", Level.INFO);
                    proc.Kill();
                }
                catch { }
            }
            Thread.Sleep(2000);

            // Remove Outlook apps (using PowerShell commands).
            ComGlobal.RunCommand("powershell", "-Command \"Get-AppxPackage *Microsoft.Office.Outlook* | Remove-AppxPackage\"");
            ComGlobal.RunCommand("powershell", "-Command \"Get-AppxProvisionedPackage -Online | Where-Object {$_.PackageName -like '*Microsoft.Office.Outlook*'} | Remove-AppxProvisionedPackage -Online\"");
            ComGlobal.RunCommand("powershell", "-Command \"Get-AppxPackage *Microsoft.OutlookForWindows* | Remove-AppxPackage\"");
            ComGlobal.RunCommand("powershell", "-Command \"Get-AppxProvisionedPackage -Online | Where-Object {$_.PackageName -like '*Microsoft.OutlookForWindows*'} | Remove-AppxProvisionedPackage -Online\"");

            // Remove Outlook folders.
            string windowsAppsPath = @"C:\Program Files\WindowsApps";
            if (Directory.Exists(windowsAppsPath))
            {
                var outlookFolders = Directory.GetDirectories(windowsAppsPath, "Microsoft.OutlookForWindows*");
                foreach (var folder in outlookFolders)
                {
                    try
                    {
                        ComGlobal.RunCommand("takeown", $"/f \"{folder}\" /r /d Y");
                        ComGlobal.RunCommand("icacls", $"\"{folder}\" /grant administrators:F /T", redirect:false);
                        Directory.Delete(folder, true);
                        Logger.Log($"Deleted Outlook folder: {folder}", Level.SUCCESS);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error deleting folder {folder}: {ex.Message}", Level.ERROR);
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
                            Logger.Log($"Deleted: {shortcut}", Level.SUCCESS);
                        }

                        if (link.TargetPath.EndsWith("onedrive.exe", StringComparison.OrdinalIgnoreCase))
                        {
                            File.Delete(shortcut);
                            Logger.Log($"Deleted: {shortcut}", Level.SUCCESS);
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
                        Logger.Log($"Deleted shortcut: {shortcut}", Level.SUCCESS);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error deleting shortcut {shortcut}: {ex.Message}", Level.ERROR);
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
                    Logger.Log($"Error cleaning registry at {regPath}: {ex.Message}", Level.ERROR);
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
                Logger.Log($"Error setting ShowTaskViewButton: {ex.Message}", Level.ERROR);
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
                ComGlobal.RunCommand(oneDriveSetupPath, "/uninstall");
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
                        Logger.Log($"Deleted directory: {path}", Level.SUCCESS);
                    }
                    else if (File.Exists(path))
                    {
                        File.Delete(path);
                        Logger.Log($"Deleted file: {path}", Level.SUCCESS);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error deleting {path}: {ex.Message}", Level.ERROR);
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
                ComRegedit.DeleteRegistryKey(Registry.ClassesRoot, regKey);
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

            Logger.Log("Outlook and OneDrive removal process completed!", Level.SUCCESS);
        }
    }
}