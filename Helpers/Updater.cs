using DebloaterTool.Logging;
using DebloaterTool.Settings;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace DebloaterTool.Helpers
{
    internal class Updater
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_SHOW = 5;

        private static string exePath = Assembly.GetExecutingAssembly().Location;
        private static string updatedExeName = Path.GetFileNameWithoutExtension(exePath) + ".update.exe";
        private static string tempUpdatedPath = Path.Combine(Path.GetTempPath(), updatedExeName);
        private static string updaterScriptPath = Path.Combine(Path.GetTempPath(), "apply_update.bat");

        public static void CheckUpdateCLI()
        {
#if DEBUG
            return;
#endif
            try
            {
                if (!NeedUpdate())
                {
                    Logger.Log("Application is already up to date.");
                    TryDeleteFile(tempUpdatedPath);
                    Console.Clear();
                    return;
                }

                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_SHOW);

                Logger.Log("New update detected.");

                if (!Display.RequestYesOrNo("A new update is available. Do you want to update now?"))
                {
                    Logger.Log("User chose to skip the update.");
                    TryDeleteFile(tempUpdatedPath);
                    Console.Clear();
                    return;
                }

                InstallUpdate();
            }
            catch (Exception ex)
            {
                Logger.Log($"Updater failed: {ex.Message}", Level.ERROR);
            }

            Console.Clear();
        }

        /// <summary>
        /// Checks if an update is required by downloading latest exe and comparing hashes.
        /// </summary>
        public static bool NeedUpdate()
        {
            try
            {
                // Download updated executable to temp location
                if (!Internet.DownloadFile(Global.lastversionurl, tempUpdatedPath))
                {
                    Logger.Log("Failed to download latest version.", Level.ERROR);
                    return false;
                }

                return !FilesAreEqual(exePath, tempUpdatedPath);
            }
            catch (Exception ex)
            {
                Logger.Log($"NeedUpdate failed: {ex.Message}", Level.ERROR);
                return false;
            }
        }

        /// <summary>
        /// Installs the update by downloading updater script and launching it.
        /// </summary>
        public static void InstallUpdate()
        {
            try
            {
                if (!File.Exists(tempUpdatedPath))
                {
                    Logger.Log("Updated file not found. Run NeedUpdate first.", Level.ERROR);
                    return;
                }

                if (!Internet.DownloadFile(Global.updaterbat, updaterScriptPath))
                {
                    Logger.Log("Failed to download updater script.", Level.ERROR);
                    TryDeleteFile(tempUpdatedPath);
                    return;
                }

                Logger.Log("Launching update script...");
                Runner.Command(updaterScriptPath, $"\"{exePath}\" \"{tempUpdatedPath}\"", waitforexit: false);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Logger.Log($"InstallUpdate failed: {ex.Message}", Level.ERROR);
            }
        }

        private static bool FilesAreEqual(string path1, string path2)
        {
            if (!File.Exists(path1) || !File.Exists(path2)) return false;

            using (var fs1 = File.OpenRead(path1))
            using (var fs2 = File.OpenRead(path2))
            using (var sha = SHA256.Create())
            {
                byte[] hash1 = sha.ComputeHash(fs1);
                byte[] hash2 = sha.ComputeHash(fs2);

                return StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2);
            }
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to delete temporary file '{path}': {ex.Message}", Level.ERROR);
            }
        }
    }
}
