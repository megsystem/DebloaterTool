using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace DebloaterTool
{
    internal class WindowsTheme
    {
        public static void ExplorerTheme()
        {
            try
            {
                string explorerthemezip = Path.Combine(Settings.themePath, "ExplorerTheme.zip");

                // Attempt to download the explorertheme file
                if (!HelperDonwload.DownloadFile(Settings.explorertheme, explorerthemezip))
                {
                    Logger.Log("Failed to download ExplorerTheme. Exiting...", Level.ERROR);
                    return;
                }

                Logger.Log($"Extracting ExplorerTheme in {Settings.themePath}...", Level.INFO);
                HelperZip.ExtractZipFile(explorerthemezip, Settings.themePath);
                string installCmdPath = Path.Combine(Settings.themePath, "register.cmd");

                if (File.Exists(installCmdPath))
                {
                    Logger.Log("Running register.cmd...", Level.INFO);
                    HelperRunner.Command(installCmdPath, workingDirectory: Settings.themePath, waitforexit: false);
                    Logger.Log("register.cmd finished.", Level.INFO);
                }
                else
                {
                    Logger.Log("register.cmd not found in extracted folder.", Level.ERROR);
                }

                // Clean up the zip
                File.Delete(explorerthemezip);
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in ExplorerTheme: {ex.Message}", Level.ERROR);
            }
        }

        public static void BorderTheme()
        {
            try
            {
                string borderthemepath = Path.Combine(Settings.themePath, "tacky-borders.exe");
                string processName = Path.GetFileNameWithoutExtension(borderthemepath);

                // Attempt to download the BorderTheme file
                if (!HelperDonwload.DownloadFile(Settings.bordertheme, borderthemepath))
                {
                    Logger.Log("Failed to download BorderTheme. Exiting...", Level.ERROR);
                    return;
                }

                Logger.Log($"Installed in {Settings.themePath} folder!", Level.SUCCESS);

                // Create a scheduled task to run the file at logon with highest privileges
                string taskName = "BorderThemeStartup";
                string arguments = $"/Create /F /RL HIGHEST /SC ONLOGON /TN \"{taskName}\" /TR \"\\\"{borderthemepath}\\\"\"";
                string output = HelperRunner.Command("schtasks", arguments, true);

                if (!string.IsNullOrWhiteSpace(output) && output.Contains("SUCCESS"))
                {
                    Logger.Log("BorderTheme task successfully added to startup!", Level.SUCCESS);
                }
                else
                {
                    Logger.Log($"Failed to create scheduled task for BorderTheme. Output: {output}", Level.ERROR);
                }

                // Launch immediately
                if (Process.GetProcessesByName(processName).Length == 0)
                {
                    Process.Start(borderthemepath);
                }
                else
                {
                    Logger.Log($"Process '{processName}' is already running. Skipping launch.", Level.WARNING);
                }

                // Set confuration
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
                    Logger.Log("Not running on Windows 10. No changes made.", Level.WARNING);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in BorderTheme: {ex.Message}", Level.ERROR);
            }
        }

        public static void AlwaysOnTop()
        {
            try
            {
                string alwaysontoppath = Path.Combine(Settings.themePath, "AlwaysOnTop.exe");
                string processName = Path.GetFileNameWithoutExtension(alwaysontoppath);

                // Download the file
                if (!HelperDonwload.DownloadFile(Settings.alwaysontop, alwaysontoppath))
                {
                    Logger.Log("Failed to download AlwaysOnTop. Exiting...", Level.ERROR);
                    return;
                }

                Logger.Log($"Installed in {Settings.themePath} folder!", Level.SUCCESS);

                // Create a scheduled task to run the file at logon with highest privileges
                string taskName = "AlwaysOnTopStartup";
                string arguments = $"/Create /F /RL HIGHEST /SC ONLOGON /TN \"{taskName}\" /TR \"\\\"{alwaysontoppath}\\\"\"";
                string output = HelperRunner.Command("schtasks", arguments, true);

                if (!string.IsNullOrWhiteSpace(output) && output.Contains("SUCCESS"))
                {
                    Logger.Log("AlwaysOnTop task successfully added to startup!", Level.SUCCESS);
                }
                else
                {
                    Logger.Log($"Failed to create scheduled task for AlwaysOnTop. Output: {output}", Level.ERROR);
                }

                // Launch immediately
                if (Process.GetProcessesByName(processName).Length == 0)
                {
                    Process.Start(alwaysontoppath);
                }
                else
                {
                    Logger.Log($"Process '{processName}' is already running. Skipping launch.", Level.WARNING);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in AlwaysOnTop: {ex.Message}", Level.ERROR);
            }
        }

        public static void WindhawkInstaller()
        {
            try
            {
                string windhawkpath = Path.Combine(Settings.themePath, "WindhawkInstaller.exe");
                string processName = Path.GetFileNameWithoutExtension(windhawkpath);

                // Download the file
                if (!HelperDonwload.DownloadFile(Settings.windhawkinstaller, windhawkpath))
                {
                    Logger.Log("Failed to download Windhawk. Exiting...", Level.ERROR);
                    return;
                }

                Logger.Log($"Installed in {Settings.themePath} folder!", Level.SUCCESS);

                // Launch immediately
                if (Process.GetProcessesByName(processName).Length == 0)
                {
                    HelperRunner.Command(windhawkpath, "/S");
                }
                else
                {
                    Logger.Log($"Process '{processName}' is already running. Skipping launch.", Level.WARNING);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in AlwaysOnTop: {ex.Message}", Level.ERROR);
            }
        }

        public static bool IsWindows11()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var currentBuildStr = (string)reg.GetValue("CurrentBuild");
            var currentBuild = int.Parse(currentBuildStr);
            return currentBuild >= 22000;
        }

        public static void ApplyThemeTweaks()
        {
            RegistryModification[] themeTweaks = new RegistryModification[]
            {
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAl", RegistryValueKind.DWord, 0), // Align taskbar to the left
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", RegistryValueKind.DWord, 0), // Set Windows to dark theme
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", RegistryValueKind.DWord, 0), // Set Windows to dark theme
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent", "AccentColorMenu", RegistryValueKind.DWord, 1), // Use accent color for taskbar/start menu
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", RegistryValueKind.DWord, 1), // Use accent color for taskbar/start menu
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\DWM", "AccentColorInStartAndTaskbar", RegistryValueKind.DWord, 1), // Use accent color for taskbar/start menu
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent", "AccentPalette", RegistryValueKind.Binary, new byte[32]), // Makes the taskbar black
                new RegistryModification(Registry.CurrentUser, @"Control Panel\Colors", "Hilight", RegistryValueKind.String, "0 0 0"), // Sets highlight color to black
                new RegistryModification(Registry.CurrentUser, @"Control Panel\Colors", "HotTrackingColor", RegistryValueKind.String, "0 0 0") // Sets click-and-drag box color to black
            };

            HelperRegedit.InstallRegModification(themeTweaks);
        }
    }
}
