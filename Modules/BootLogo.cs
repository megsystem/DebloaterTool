﻿using System.Diagnostics;
using System.IO;

namespace DebloaterTool
{
    internal class BootLogo
    {
        public static void Install()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            string bootlogozip = Path.Combine(tempPath, "bootlogo.zip");

            // Attempt to download the bootlogo file
            if (!ComGlobal.DownloadFile(ExternalLinks.bootlogo, bootlogozip))
            {
                Logger.Log("Failed to download bootlogo. Exiting...", Level.ERROR);
                return;
            }

            Logger.Log("Extracting bootlogo...", Level.INFO);
            ComZip.ExtractZipFile(bootlogozip, tempPath);

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
    }
}
