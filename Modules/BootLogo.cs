using DebloaterTool.Helpers;
using DebloaterTool.Settings;
using DebloaterTool.Logging;
using System;
using System.IO;

namespace DebloaterTool.Modules
{
    internal class BootLogo
    {
        public static void Install()
        {
            try
            {
                // 1. Prepare temp folder
                string bootlogozip = Path.Combine(Global.bootlogoPath, "bootlogo.zip");

                // 2. Download
                if (!Download.DownloadFile(Global.bootlogo, bootlogozip))
                {
                    Logger.Log("Failed to download bootlogo. Exiting...", Level.ERROR);
                    return;
                }

                // 3. Extract
                Logger.Log("Extracting bootlogo...", Level.INFO);
                Zip.ExtractZipFile(bootlogozip, Global.bootlogoPath);

                // 4. Locate and run install.cmd
                string installCmdPath = Path.Combine(Global.bootlogoPath, "install.cmd");
                if (File.Exists(installCmdPath))
                {
                    Logger.Log("Running install.cmd...", Level.INFO);
                    Runner.Command(installCmdPath, workingDirectory: Global.bootlogoPath, NoWindow: false);
                    Logger.Log("install.cmd finished.", Level.INFO);
                }
                else
                {
                    Logger.Log("install.cmd not found in extracted folder.", Level.ERROR);
                }

                // 5. Delete bootlogo zip
                File.Delete(bootlogozip);
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error in Install: {ex.Message}", Level.ERROR);
            }
        }
    }
}
