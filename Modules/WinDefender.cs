using System.IO;

namespace DebloaterTool
{
    internal class WinDefender
    {
        /// <summary>
        /// Uninstalls Windows Defender components by:
        /// - Downloading a helper executable (PowerRun.exe) to elevate commands.
        /// - Importing a registry file to apply Defender-related configuration changes.
        /// - Deleting specific files and directories associated with Windows Defender.
        /// </summary>
        public static void Uninstall()
        {
            Logger.Log($"Downloading from {Settings.powerRun}...");
            string powerRunPath = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.exe");
            if (!HelperGlobal.DownloadFile(Settings.powerRun, powerRunPath))
            {
                Logger.Log($"Failed to download {Settings.powerRun}. Skipping...", Level.ERROR);
                return;
            }
            Logger.Log($"Download complete to {powerRunPath}");

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

            string tempRegFile = Path.Combine(Path.GetTempPath(), "defenderkiller.reg");
            File.WriteAllText(tempRegFile, Config.Resource.defender);

            // Import the registry file silently using regedit (/s switch).
            HelperGlobal.RunCommand(powerRunPath, $"regedit.exe /s \"{tempRegFile}\"");

            // Optionally, delete the temporary file after execution.
            try { File.Delete(tempRegFile); } catch { /* Ignore errors on deletion */ }

            foreach (var file in filesToDelete)
            {
                HelperGlobal.RunCommand(powerRunPath, $"cmd.exe /c del /f \"{file}\"");
            }

            foreach (var dir in directoriesToDelete)
            {
                HelperGlobal.RunCommand(powerRunPath, $"cmd.exe /c rmdir /s /q \"{dir}\"");
            }
        }
    }
}
