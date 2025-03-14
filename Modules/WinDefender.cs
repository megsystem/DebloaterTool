using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace DebloaterTool
{
    internal class WinDefender
    {
        public static void Uninstall()
        {
            string powerRunUrl = "https://github.com/megsystem/DebloaterTool/raw/refs/heads/main/External/PowerRun.exe";
            string powerRunPath = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.exe");

            Logger.Log("Downloading...");
            if (!ComFunction.DownloadFile(powerRunUrl, powerRunPath))
            {
                Logger.Log("Failed to download PowerRun.exe. Exiting...", Level.ERROR);
                return;
            }
            Logger.Log("Download complete.");

            string[] filesToDelete =
            {
                "C:\\Windows\\WinSxS\\FileMaps\\wow64_windows-defender*.manifest",
                "C:\\Windows\\WinSxS\\FileMaps\\x86_windows-defender*.manifest",
                "C:\\Windows\\WinSxS\\FileMaps\\amd64_windows-defender*.manifest",
                "C:\\Windows\\System32\\SecurityHealthSystray.exe",
                "C:\\Windows\\System32\\SecurityHealthService.exe",
                "C:\\Windows\\System32\\SecurityHealthHost.exe",
                "C:\\Windows\\System32\\drivers\\WdDevFlt.sys",
                "C:\\Windows\\System32\\drivers\\WdBoot.sys",
                "C:\\Windows\\System32\\drivers\\WdFilter.sys",
                "C:\\Windows\\System32\\wscsvc.dll",
                "C:\\Windows\\System32\\smartscreen.dll",
                "C:\\Windows\\SysWOW64\\smartscreen.dll",
                "C:\\Windows\\System32\\DWWIN.EXE"
            };

            string[] directoriesToDelete =
            {
                "C:\\ProgramData\\Microsoft\\Windows Defender",
                "C:\\Program Files (x86)\\Windows Defender",
                "C:\\Program Files\\Windows Defender",
                "C:\\Windows\\System32\\SecurityHealth",
                "C:\\Windows\\System32\\WebThreatDefSvc"
            };

            string tempRegFile = Path.Combine(Path.GetTempPath(), "temp_reg_file.reg");
            File.WriteAllText(tempRegFile, Config.Resource.defender);

            // Import the registry file silently using regedit (/s switch).
            ComFunction.RunCommand(powerRunPath, $"regedit.exe /s \"{tempRegFile}\"");

            // Optionally, delete the temporary file after execution.
            try { File.Delete(tempRegFile); } catch { /* Ignore errors on deletion */ }

            foreach (var file in filesToDelete)
            {
                ComFunction.RunCommand(powerRunPath, $"cmd.exe /c del /f \"{file}\"");
            }

            foreach (var dir in directoriesToDelete)
            {
                ComFunction.RunCommand(powerRunPath, $"cmd.exe /c rmdir /s /q \"{dir}\"");
            }
        }
    }
}
