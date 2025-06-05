using DebloaterTool.Logging;
using DebloaterTool.Settings;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace DebloaterTool.Helpers
{
    internal class Updater
    {
        public static void CheckForUpdates()
        {
#if DEBUG
            return;
#endif
            try
            {
                string exePath = Assembly.GetExecutingAssembly().Location;
                string updatedExeName = Path.GetFileNameWithoutExtension(exePath) + ".update.exe";
                string tempUpdatedPath = Path.Combine(Path.GetTempPath(), updatedExeName);
                string updaterScriptPath = Path.Combine(Path.GetTempPath(), "apply_update.bat");

                // Download updated executable to temp location
                if (!Internet.DownloadFile(Global.lastversionurl, tempUpdatedPath))
                {
                    Logger.Log("Failed to download latest version.", Level.ERROR);
                    return;
                }

                // Compare hashes
                if (!FilesAreEqual(exePath, tempUpdatedPath))
                {
                    Logger.Log("New update detected.");

                    if (!Internet.DownloadFile(Global.updaterbat, updaterScriptPath))
                    {
                        Logger.Log("Failed to download updater script.", Level.ERROR);
                        return;
                    }

                    Logger.Log("Launching update script...");
                    Runner.Command(updaterScriptPath, $"\"{exePath}\" \"{tempUpdatedPath}\"", waitforexit: false);
                    Environment.Exit(0);
                }
                else
                {
                    Logger.Log("Application is already up to date.");
                    TryDeleteFile(tempUpdatedPath); // Clean up unused download
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Updater failed: {ex.Message}", Level.ERROR);
            }

            // Cleanup console
            Console.Clear();
        }

        private static bool FilesAreEqual(string path1, string path2)
        {
            if (!File.Exists(path1) || !File.Exists(path2)) return false;

            FileStream fs1 = null;
            FileStream fs2 = null;
            SHA256 sha = null;
            try
            {
                fs1 = File.OpenRead(path1);
                fs2 = File.OpenRead(path2);
                sha = SHA256.Create();

                byte[] hash1 = sha.ComputeHash(fs1);
                byte[] hash2 = sha.ComputeHash(fs2);

                return StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2);
            }
            finally
            {
                if (fs1 != null) fs1.Dispose();
                if (fs2 != null) fs2.Dispose();
                if (sha != null) sha.Dispose();
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
