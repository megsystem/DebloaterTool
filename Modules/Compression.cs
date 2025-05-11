using System;
using System.Diagnostics;

namespace DebloaterTool
{
    internal class Compression
    {
        public static void CompressOS()
        {
            string command = @"/c c:\windows\*.* /s /i /exe:lzx";
            Logger.Log($"Starting compression with arguments: {command}", Level.INFO);

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "compact.exe",
                    Arguments = command,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process proc = Process.Start(psi);
                proc.WaitForExit();
                Logger.Log("Compression process completed.", Level.SUCCESS);
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception occurred during compression: {ex.Message}", Level.CRITICAL);
            }
        }
    }
}
