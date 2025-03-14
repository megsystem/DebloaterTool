using System;
using System.Diagnostics;
using System.Net;

namespace DebloaterTool
{
    internal class ComFunction
    {
        public static bool DownloadFile(string url, string outputPath)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, outputPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Download error: {ex.Message}", Level.ERROR);
                return false;
            }
        }

        public static string RunCommand(string path, string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = arguments,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false // required for redirection
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    return process.StandardOutput.ReadToEnd();
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
