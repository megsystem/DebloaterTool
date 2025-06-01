using DebloaterTool.Helpers;
using DebloaterTool.Settings;
using DebloaterTool.Logging;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;

namespace DebloaterTool.Modules
{
    internal class WindowsTheme
    {
        /// <summary>
        /// Installs and registers a Windows Explorer theme by downloading a ZIP, extracting it, running register.cmd, and cleaning up.
        /// </summary>
        public static void ExplorerTheme()
        {
            try
            {
                string zipPath = Path.Combine(Global.themePath, "ExplorerTheme.zip");
                string zipUrl = Global.explorertheme; // URL to the ZIP

                if (!DownloadFile(zipUrl, zipPath)) return;
                Logger.Log($"Extracting ExplorerTheme to '{Global.themePath}'...", Level.INFO);
                Zip.ExtractZipFile(zipPath, Global.themePath);

                string registerCmd = Path.Combine(Global.themePath, "register.cmd");
                if (File.Exists(registerCmd))
                {
                    Logger.Log("Running register.cmd...", Level.INFO);
                    Runner.Command(registerCmd, workingDirectory: Global.themePath, waitforexit: false);
                    Logger.Log("register.cmd finished.", Level.INFO);
                }
                else
                {
                    Logger.Log("register.cmd not found in extracted folder.", Level.ERROR);
                }

                // Clean up the ZIP file
                try
                {
                    File.Delete(zipPath);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to delete '{zipPath}': {ex.Message}", Level.WARNING);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in ExplorerTheme: {ex.Message}", Level.ERROR);
            }
        }

        /// <summary>
        /// Downloads a border‐themed executable, schedules it at logon, launches it, waits for its config, and patches border radius if running on Windows 10.
        /// </summary>
        public static void BorderTheme()
        {
            try
            {
                string exeName = "tacky-borders.exe";
                string exePath = Path.Combine(Global.themePath, exeName);
                string exeUrl = Global.bordertheme; // URL to the EXE
                string taskName = "BorderThemeStartup";

                if (!DownloadFile(exeUrl, exePath)) return;
                Logger.Log($"Installed '{exeName}' in '{Global.themePath}'.", Level.SUCCESS);
                CreateLogonTask(taskName, exePath);
                Runner.LaunchIfNotRunning(exePath);

                // Wait for the config file to appear
                string configPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".config", "tacky-borders", "config.yaml"
                );

                int waited = 0;
                while (!File.Exists(configPath))
                {
                    if (waited >= 30)
                    {
                        Logger.Log($"Timeout waiting for file: {configPath}", Level.ERROR);
                        return;
                    }

                    Logger.Log($"Waiting for config file to appear... ({waited + 1}s)", Level.INFO);
                    Thread.Sleep(1000);
                    waited++;
                }

                // Only patch the config if not running on Windows 11
                if (!IsWindows11())
                {
                    string content = File.ReadAllText(configPath);

                    if (content.Contains("border_radius: Auto"))
                    {
                        content = content.Replace("border_radius: Auto", "border_radius: RoundSmall");
                        File.WriteAllText(configPath, content);
                        Logger.Log("Config updated: border_radius set to RoundSmall.", Level.INFO);
                    }
                    else
                    {
                        Logger.Log("No matching 'border_radius: Auto' entry found.", Level.ERROR);
                    }
                }
                else
                {
                    Logger.Log("Detected Windows 11; no config changes applied.", Level.WARNING);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in BorderTheme: {ex.Message}", Level.ERROR);
            }
        }

        /// <summary>
        /// Downloads an AlwaysOnTop executable, schedules it at logon, and launches it immediately.
        /// </summary>
        public static void AlwaysOnTop()
        {
            try
            {
                string exeName = "AlwaysOnTop.exe";
                string exePath = Path.Combine(Global.themePath, exeName);
                string exeUrl = Global.alwaysontop;
                string taskName = "AlwaysOnTopStartup";

                if (!DownloadFile(exeUrl, exePath)) return;
                Logger.Log($"Installed '{exeName}' in '{Global.themePath}'.", Level.SUCCESS);
                CreateLogonTask(taskName, exePath);
                Runner.LaunchIfNotRunning(exePath);
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in AlwaysOnTop: {ex.Message}", Level.ERROR);
            }
        }

        /// <summary>
        /// Downloads WindhawkInstaller and runs it silently if it isn’t already running.
        /// </summary>
        public static void WindhawkInstaller()
        {
            try
            {
                string exeName = "WindhawkInstaller.exe";
                string exePath = Path.Combine(Global.themePath, exeName);
                string exeUrl = Global.windhawkinstaller;

                if (!DownloadFile(exeUrl, exePath)) return;
                Logger.Log($"Installed '{exeName}' in '{Global.themePath}'.", Level.SUCCESS);
                Runner.LaunchIfNotRunning(exePath, "/S");
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in WindhawkInstaller: {ex.Message}", Level.ERROR);
            }
        }

        /// <summary>
        /// Downloads a .reg file and imports it via regedit.
        /// </summary>
        public static void TakeOwnershipMenu()
        {
            try
            {
                string regName = "TakeOwnershipMenu.reg";
                string regPath = Path.Combine(Global.themePath, regName);
                string regUrl = Global.takeownershipmenu;

                if (!DownloadFile(regUrl, regPath)) return;
                Logger.Log($"Installed '{regName}' in '{Global.themePath}'.", Level.SUCCESS);
                Runner.Command("regedit.exe", $"/s \"{regPath}\"");
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in TakeOwnershipMenu: {ex.Message}", Level.ERROR);
            }
        }

        public static void ApplyThemeTweaks()
        {
            TweakRegistry[] themeTweaks = new TweakRegistry[]
            {
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAl", RegistryValueKind.DWord, 0), // Align taskbar to the left
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", RegistryValueKind.DWord, 0), // Set Windows to dark theme
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", RegistryValueKind.DWord, 0), // Set Windows to dark theme
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent", "AccentColorMenu", RegistryValueKind.DWord, 1), // Use accent color for taskbar/start menu
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", RegistryValueKind.DWord, 1), // Use accent color for taskbar/start menu
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\DWM", "AccentColorInStartAndTaskbar", RegistryValueKind.DWord, 1), // Use accent color for taskbar/start menu
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent", "AccentPalette", RegistryValueKind.Binary, new byte[32]), // Makes the taskbar black
                new TweakRegistry(Registry.CurrentUser, @"Control Panel\Colors", "Hilight", RegistryValueKind.String, "0 0 0"), // Sets highlight color to black
                new TweakRegistry(Registry.CurrentUser, @"Control Panel\Colors", "HotTrackingColor", RegistryValueKind.String, "0 0 0") // Sets click-and-drag box color to black
            };

            Regedit.InstallRegModification(themeTweaks);
        }

        private static bool DownloadFile(string url, string destinationPath)
        {
            if (!Internet.DownloadFile(url, destinationPath))
            {
                Logger.Log($"Failed to download from {url}. Exiting...", Level.ERROR);
                return false;
            }
            return true;
        }

        private static bool CreateLogonTask(string taskName, string exePath)
        {
            // Escape quotes around the exe path
            string quotedPath = $"\"{exePath}\"";
            string arguments = $"/Create /F /RL HIGHEST /SC ONLOGON /TN \"{taskName}\" /TR {quotedPath}";
            string output = Runner.Command("schtasks", arguments, redirect: true);

            if (!string.IsNullOrWhiteSpace(output) && output.Contains("SUCCESS"))
            {
                Logger.Log($"{taskName} task successfully added to startup!", Level.SUCCESS);
                return true;
            }
            else
            {
                Logger.Log($"Failed to create scheduled task '{taskName}'. Output: {output}", Level.ERROR);
                return false;
            }
        }

        private static bool IsWindows11()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var currentBuildStr = (string)reg.GetValue("CurrentBuild");
            var currentBuild = int.Parse(currentBuildStr);
            return currentBuild >= 22000;
        }
    }
}
