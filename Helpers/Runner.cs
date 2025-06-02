using DebloaterTool.Logging;
using System;
using System.Diagnostics;

namespace DebloaterTool.Helpers
{
    internal class Runner
    {
        public static string Command(
            string path,
            string arguments = null,
            bool redirect = false,
            bool redirectOutputLogger = false,
            string workingDirectory = null,
            bool waitforexit = true)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = arguments ?? string.Empty,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = redirect || redirectOutputLogger, // ensure redirection is on
                    CreateNoWindow = true,
                    UseShellExecute = false // required for redirection
                };

                if (!string.IsNullOrEmpty(workingDirectory))
                {
                    psi.WorkingDirectory = workingDirectory;
                }

                using (Process process = new Process { StartInfo = psi })
                {
                    string output = string.Empty;

                    if (redirectOutputLogger)
                    {
                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrWhiteSpace(e.Data))
                            {
                                Logger.Log(e.Data, Level.INFO);
                            }
                        };
                    }

                    process.Start();

                    if (redirectOutputLogger)
                    {
                        process.BeginOutputReadLine();
                    }

                    if (waitforexit)
                    {
                        process.WaitForExit();

                        // If redirect is enabled but logging is not, still capture the output
                        if (redirect && !redirectOutputLogger)
                        {
                            output = process.StandardOutput.ReadToEnd();
                        }
                    }

                    return (redirect && !redirectOutputLogger) ? output : null;
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
