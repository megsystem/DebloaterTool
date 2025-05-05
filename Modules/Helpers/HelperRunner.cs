using System;
using System.Diagnostics;

namespace DebloaterTool
{
    internal class HelperRunner
    {
        public static string Command(string path, string arguments, bool redirect = false)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = arguments,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = redirect,
                    CreateNoWindow = true,
                    UseShellExecute = false // required for redirection
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
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
