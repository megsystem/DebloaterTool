using System;
using System.Diagnostics;
using System.IO;

namespace DebloaterTool
{
    internal class WindowsTheme
    {
        public static void ExplorerTheme()
        {
            string themePath = @"C:\ExplorerTheme";
            Directory.CreateDirectory(themePath);
            string explorerthemezip = Path.Combine(themePath, "ExplorerTheme.zip");

            // Attempt to download the explorertheme file
            if (!HelperGlobal.DownloadFile(ExternalLinks.explorertheme, explorerthemezip))
            {
                Logger.Log("Failed to download ExplorerTheme. Exiting...", Level.ERROR);
                return;
            }

            Logger.Log("Extracting ExplorerTheme...", Level.INFO);
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

            try { File.Delete(explorerthemezip); } catch { }
        }

        public static void BorderTheme()
        {
            string startupfolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string borderthemepath = Path.Combine(startupfolder, "tacky-borders.exe");

            // Attempt to download the BorderTheme file
            if (!HelperGlobal.DownloadFile(ExternalLinks.bordertheme, borderthemepath))
            {
                Logger.Log("Failed to download BorderTheme. Exiting...", Level.ERROR);
                return;
            }

            Logger.Log("Installed in startup folder!", Level.SUCCESS);
        }
    }
}
