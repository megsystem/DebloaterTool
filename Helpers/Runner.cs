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
    }
}
