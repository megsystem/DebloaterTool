using System;
using System.Diagnostics;
using System.IO;

namespace DebloaterTool
{
    internal class WindowsTheme
    {
        static string themePath = @"C:\DebloaterTool\WinTheme";

        public static void ExplorerTheme()
        {
            try
            {
                Directory.CreateDirectory(themePath);
                string explorerthemezip = Path.Combine(themePath, "ExplorerTheme.zip");

                // Attempt to download the explorertheme file
                if (!HelperGlobal.DownloadFile(ExternalLinks.explorertheme, explorerthemezip))
                {
                    Logger.Log("Failed to download ExplorerTheme. Exiting...", Level.ERROR);
                    return;
                }

                Logger.Log($"Extracting ExplorerTheme in {themePath}...", Level.INFO);
                HelperZip.ExtractZipFile(explorerthemezip, themePath);
                string installCmdPath = Path.Combine(themePath, "register.cmd");

                if (File.Exists(installCmdPath))
                {
                    Logger.Log("Running register.cmd...", Level.INFO);

                    var process = new Process();
                    process.StartInfo.FileName = installCmdPath;
                    process.StartInfo.WorkingDirectory = themePath;
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    Logger.Log("register.cmd finished.", Level.INFO);
                }
                else
                {
                    Logger.Log("register.cmd not found in extracted folder.", Level.ERROR);
                }

                // Clean up the zip
                try
                {
                    File.Delete(explorerthemezip);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Could not delete temporary zip: {ex.Message}", Level.WARNING);
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
                Directory.CreateDirectory(themePath);
                string borderthemepath = Path.Combine(themePath, "tacky-borders.exe");

                // Attempt to download the BorderTheme file
                if (!HelperGlobal.DownloadFile(ExternalLinks.bordertheme, borderthemepath))
                {
                    Logger.Log("Failed to download BorderTheme. Exiting...", Level.ERROR);
                    return;
                }

                Logger.Log($"Installed in {themePath} folder!", Level.SUCCESS);

                // Create a scheduled task to run the file at logon with highest privileges
                string taskName = "BorderThemeStartup";
                string arguments = $"/Create /F /RL HIGHEST /SC ONLOGON /TN \"{taskName}\" /TR \"\\\"{borderthemepath}\\\"\"";
                string output = HelperGlobal.RunCommand("schtasks", arguments, true);

                if (!string.IsNullOrWhiteSpace(output) && output.Contains("SUCCESS"))
                {
                    Logger.Log("BorderTheme task successfully added to startup!", Level.SUCCESS);
                }
                else
                {
                    Logger.Log($"Failed to create scheduled task for BorderTheme. Output: {output}", Level.ERROR);
                }

                // Launch immediately
                try
                {
                    Process.Start(borderthemepath);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to launch BorderTheme executable: {ex.Message}", Level.ERROR);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in BorderTheme: {ex.Message}", Level.ERROR);
            }
        }
    }
}
