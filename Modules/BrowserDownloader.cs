using DebloaterTool.Helpers;
using DebloaterTool.Logging;
using DebloaterTool.Settings;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace DebloaterTool.Modules
{
    internal class BrowserDownloader
    {
        public static void Installer()
        {
            if (TweakBrowser.browsers.ContainsKey(TweakBrowser.requestBrowser))
            {
                var browser = TweakBrowser.browsers[TweakBrowser.requestBrowser];
                Logger.Log("Selected: " + browser.Name);
                browser.InstallAction(); // Run the action
            }
        }

        public static void FireFox()
        {
            bool is64Bit = Environment.Is64BitOperatingSystem;
            string firefoxUrl;

            if (is64Bit)
            {
                // Direct link to 64-bit installer
                firefoxUrl = "https://download.mozilla.org/?product=firefox-latest-ssl&os=win64&lang=en-US";
            }
            else
            {
                // Direct link to 32-bit installer
                firefoxUrl = "https://download.mozilla.org/?product=firefox-latest-ssl&os=win&lang=en-US";
            }

            Logger.Log("Firefox download URL: " + firefoxUrl);
            string installerPath = Path.Combine(Path.GetTempPath(), "FirefoxInstaller.exe");
            string homepageUrl = Global.tabLink;

            Logger.Log("Downloading Firefox...");
            if (!Internet.DownloadFile(firefoxUrl, installerPath)) return;

            Logger.Log("Installing Firefox silently...");
            Runner.Command(installerPath, "/S");

            Logger.Log("Setting custom Firefox homepage...");
            var tweaks = new List<TweakRegistry>
            {
                new TweakRegistry(
                    Registry.LocalMachine,
                    @"SOFTWARE\Policies\Mozilla\Firefox",
                    "ShowHomeButton",
                    RegistryValueKind.DWord,
                    1
                ),
                new TweakRegistry(
                    Registry.LocalMachine,
                    @"SOFTWARE\Policies\Mozilla\Firefox\Homepage",
                    "URL",
                    RegistryValueKind.String,
                    homepageUrl
                ),
                new TweakRegistry(
                    Registry.LocalMachine,
                    @"SOFTWARE\Policies\Mozilla\Firefox\Homepage",
                    "StartPage",
                    RegistryValueKind.String,
                    "homepage"
                ),
                new TweakRegistry(
                    Registry.LocalMachine,
                    @"SOFTWARE\Policies\Mozilla\Firefox\Homepage",
                    "Locked",
                    RegistryValueKind.DWord,
                    0
                )
            };

            Regedit.InstallRegModification(tweaks.ToArray());
            Logger.Log("Done.");
        }

        public static void Ungoogled()
        {
            UngoogledInstaller();
            ChangeUngoogledHomePage();
        }

        private static void UngoogledInstaller()
        {
            try
            {
                string json = Internet.FetchDataUrl("https://api.github.com/repos/ungoogled-software/ungoogled-chromium-windows/releases/latest");
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
                    Logger.Log("Installer asset not found for pattern: " + searchPattern, Level.ERROR);
                    return;
                }

                Logger.Log("Latest installer found: " + assetName, Level.INFO);
                Logger.Log("Download URL: " + downloadUrl, Level.INFO);

                string tempFile = Path.Combine(Path.GetTempPath(), assetName);
                Logger.Log("Downloading installer to " + tempFile + "...", Level.INFO);
                if (!Internet.DownloadFile(downloadUrl, tempFile))
                {
                    Logger.Log($"Failed to download {downloadUrl}. Exiting...", Level.ERROR);
                    return;
                }
                Logger.Log("Download completed.", Level.SUCCESS);

                Logger.Log("Starting installer...", Level.SUCCESS);
                Runner.Command(tempFile);
                Logger.Log("Installer process completed.", Level.SUCCESS);

                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                Logger.Log("An error occurred: " + ex.Message, Level.ERROR);
            }
        }

        private static void ChangeUngoogledHomePage()
        {
            string argToAdd = $"--custom-ntp={Global.tabLink}";
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
                                    Logger.Log("Updated: " + file, Level.SUCCESS);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log("Error processing shortcut " + file + ": " + ex.Message, Level.ERROR);
                        }
                    }
                    foreach (string sub in Directory.GetDirectories(path))
                        ProcessDirectory(sub);
                }
                catch (Exception ex)
                {
                    Logger.Log("Error processing directory " + path + ": " + ex.Message, Level.ERROR);
                }
            };

            foreach (string dir in dirs)
            {
                if (Directory.Exists(dir))
                    ProcessDirectory(dir);
            }
            Logger.Log("Shortcut update complete.", Level.SUCCESS);
        }
    }
}
