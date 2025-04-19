using System;
using System.Diagnostics;
using System.IO;

namespace DebloaterTool
{
    internal class BootLogo
    {
        public static void Install()
        {
            try
            {
                // 1. Prepare temp folder
                string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tempPath);
                string bootlogozip = Path.Combine(tempPath, "bootlogo.zip");

                // 2. Download
                if (!HelperGlobal.DownloadFile(ExternalLinks.bootlogo, bootlogozip))
                {
                    Logger.Log("Failed to download bootlogo. Exiting...", Level.ERROR);
                    return;
                }

                // 3. Extract
                Logger.Log("Extracting bootlogo...", Level.INFO);
                HelperZip.ExtractZipFile(bootlogozip, tempPath);

                // 4. Locate and run install.cmd
                string installCmdPath = Path.Combine(tempPath, "install.cmd");
                if (File.Exists(installCmdPath))
                {
                    Logger.Log("Running install.cmd...", Level.INFO);

                    var process = new Process();
                    process.StartInfo.FileName = installCmdPath;
                    process.StartInfo.WorkingDirectory = tempPath;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = false;
                    process.Start();
                    process.WaitForExit();

                    Logger.Log("install.cmd finished.", Level.INFO);
                }
                else
                {
                    Logger.Log("install.cmd not found in extracted folder.", Level.ERROR);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in Install: {ex.Message}", Level.ERROR);
            }
        }
    }
}
