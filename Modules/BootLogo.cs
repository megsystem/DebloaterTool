using System;
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
                string bootlogozip = Path.Combine(Settings.bootlogoPath, "bootlogo.zip");

                // 2. Download
                if (!HelperDonwload.DownloadFile(Settings.bootlogo, bootlogozip))
                {
                    Logger.Log("Failed to download bootlogo. Exiting...", Level.ERROR);
                    return;
                }

                // 3. Extract
                Logger.Log("Extracting bootlogo...", Level.INFO);
                HelperZip.ExtractZipFile(bootlogozip, Settings.bootlogoPath);

                // 4. Locate and run install.cmd
                string installCmdPath = Path.Combine(Settings.bootlogoPath, "install.cmd");
                if (File.Exists(installCmdPath))
                {
                    Logger.Log("Running install.cmd...", Level.INFO);
                    HelperRunner.Command(installCmdPath, workingDirectory: Settings.bootlogoPath, NoWindow: false);
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
