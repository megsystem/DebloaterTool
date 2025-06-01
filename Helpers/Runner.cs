using DebloaterTool.Logging;
using System;
using System.Diagnostics;
using System.IO;

namespace DebloaterTool.Helpers
{
    internal class Runner
    {
        public static string Command(
            string path, 
            string arguments = null, 
            bool redirect = false, 
            string workingDirectory = null,
            bool NoWindow = true,
            bool waitforexit = true)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = arguments ?? string.Empty, // ensure it's not null
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = redirect,
                    CreateNoWindow = NoWindow,
                    UseShellExecute = false // required for redirection
                };

                if (!string.IsNullOrEmpty(workingDirectory))
                {
                    psi.WorkingDirectory = workingDirectory;
                }

                using (Process process = Process.Start(psi))
                {
                    if (waitforexit) process.WaitForExit();
                    return (redirect) ? process.StandardOutput.ReadToEnd() : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error: {ex.Message}", Level.ERROR);
                return null;
            }
        }

        public static void LaunchIfNotRunning(
            string exePath,
            string argument = null)
        {
            string processName = Path.GetFileNameWithoutExtension(exePath);
            if (Process.GetProcessesByName(processName).Length == 0)
            {
                try
                {
                    if (string.IsNullOrEmpty(argument))
                    {
                        Process.Start(exePath);
                    }
                    else
                    {
                        Process.Start(exePath, argument);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to launch {exePath} {(argument != null ? $"with arguments '{argument}'" : "")}: {ex.Message}", Level.ERROR);
                }
            }
            else
            {
                Logger.Log($"Process '{processName}' is already running. Skipping launch.", Level.WARNING);
            }
        }
    }
}
