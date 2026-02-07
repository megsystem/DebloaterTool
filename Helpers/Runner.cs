using DebloaterTool.Logging;
using System;
using System.Diagnostics;
using System.Threading;

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
            bool waitforexit = true,
            bool skipErrors = false,
            string customExitCheck = null)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = arguments ?? string.Empty,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = redirect || redirectOutputLogger, // ensure redirection is on
                    RedirectStandardError = redirectOutputLogger,
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
                    bool shouldExit = false;

                    if (redirectOutputLogger)
                    {
                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrWhiteSpace(e.Data))
                            {
                                Logger.Log(e.Data, Level.INFO);
                                if (!string.IsNullOrEmpty(customExitCheck) && 
                                    e.Data.Contains(customExitCheck))
                                {
                                    shouldExit = true; // Set exit flag
                                }
                            }
                        };

                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrWhiteSpace(e.Data) && !skipErrors)
                            {
                                Logger.Log(e.Data, Level.ERROR);
                            }
                        };
                    }

                    process.Start();

                    if (redirectOutputLogger)
                    {
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                    }

                    if (waitforexit)
                    {
                        while (!process.HasExited)
                        {
                            if (shouldExit && !string.IsNullOrEmpty(customExitCheck)) // Custom exit
                            {
                                Logger.Log("Stopping process safely for custom exit...", Level.SUCCESS);
                                process.Kill(); // Force kill
                                return null;
                            }

                            Thread.Sleep(500); // Prevent high CPU usage
                        }

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
                if (!skipErrors)
                {
                    Logger.Log($"Error: {ex.Message}", Level.ERROR);
                }
                return null;
            }
        }
    }
}
