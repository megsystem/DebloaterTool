using System.Diagnostics;
using System.IO;

namespace DebloaterTool
{
    internal class ExplorerTheme
    {
        public static void Install()
        {
            string themePath = @"C:\ExplorerTheme";
            Directory.CreateDirectory(themePath);
            string explorerthemezip = Path.Combine(themePath, "ExplorerTheme.zip");

            // Attempt to download the bootlogo file
            if (!HelperGlobal.DownloadFile(ExternalLinks.bootlogo, explorerthemezip))
            {
                Logger.Log("Failed to download ExplorerTheme. Exiting...", Level.ERROR);
                return;
            }

            Logger.Log("Extracting bootlogo...", Level.INFO);
            HelperZip.ExtractZipFile(explorerthemezip, themePath);
            string installCmdPath = Path.Combine(themePath, "register.cmd");

            if (File.Exists(installCmdPath))
            {
                Logger.Log("Running register.cmd...", Level.INFO);

                var process = new Process();
                process.StartInfo.FileName = installCmdPath;
                process.StartInfo.WorkingDirectory = themePath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;

                process.Start();
                process.WaitForExit();

                Logger.Log("register.cmd finished.", Level.INFO);
            }
            else
            {
                Logger.Log("register.cmd not found in extracted folder.", Level.ERROR);
            }
        }
    }
}
