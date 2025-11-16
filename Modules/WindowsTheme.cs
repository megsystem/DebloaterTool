using DebloaterTool.Helpers;
using DebloaterTool.Settings;
using DebloaterTool.Logging;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace DebloaterTool.Modules
{
    internal class WindowsTheme
    {
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

                // Check if is not running and run
                string processName = Path.GetFileNameWithoutExtension(exePath);
                if (Process.GetProcessesByName(processName).Length == 0)
                {
                    try
                    {
                        Runner.Command(exePath, waitforexit: false);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Failed to launch {exePath}: {ex.Message}", Level.ERROR);
                    }
                }
                else
                {
                    Logger.Log($"Process '{processName}' is already running. Skipping launch.", Level.WARNING);
                }

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
                Runner.Command(exePath, waitforexit: false);
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
                string exeName = "WindhawkInstaller.exe";
                string exePath = Path.Combine(Global.themePath, exeName);
                string exeUrl = Global.windhawkinstaller;

                if (!DownloadFile(exeUrl, exePath)) return;
                Logger.Log($"Downloaded '{exeName}' in '{Global.themePath}'.", Level.SUCCESS);
                Runner.Command(exePath, "/S");
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in WindhawkInstaller: {ex.Message}", Level.ERROR);
            }
        }

        public static void SevenZipInstaller()
        {
            try
            {
                string scriptName = "Install7Zip.ps1";
                string scriptPath = Path.Combine(Global.themePath, scriptName);
                string scriptUrl = Global.sevenzipscript;

                if (!DownloadFile(scriptUrl, scriptPath)) return;
                Logger.Log($"Downloaded '{scriptName}' in '{Global.themePath}'.", Level.SUCCESS);
                Runner.Command("powershell.exe", $"-ExecutionPolicy Bypass -File {scriptPath}", redirectOutputLogger: true);
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in SevenZipInstaller: {ex.Message}", Level.ERROR);
            }
        }

        public static void TakeOwnershipMenu()
        {
            try
            {
                string regName = "TakeOwnershipMenu.reg";
                string regPath = Path.Combine(Global.themePath, regName);
                string regUrl = Global.takeownershipmenu;

                if (!DownloadFile(regUrl, regPath)) return;
                Logger.Log($"Downloaded '{regName}' in '{Global.themePath}'.", Level.SUCCESS);
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
            string ps = $@"
$action = New-ScheduledTaskAction -Execute '{exePath}'
$trigger = New-ScheduledTaskTrigger -AtLogon
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries
Register-ScheduledTask -Action $action -Trigger $trigger -TaskName '{taskName}' -Description 'Auto-run task' -Settings $settings -RunLevel Highest -Force
";

            string output = Runner.Command("powershell", ps, redirect: true);

            if (!string.IsNullOrWhiteSpace(output) && output.Contains("TaskPath"))
            {
                Logger.Log($"{taskName} created successfully and runs on battery.", Level.SUCCESS);
                return true;
            }
            else
            {
                Logger.Log($"Failed to create task '{taskName}'. Output: {output}", Level.ERROR);
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
