using DebloaterTool.Helpers;
using DebloaterTool.Logging;

namespace DebloaterTool.Modules
{
    internal class Compression
    {
        public static void CompressOS()
        {
            string command = @"/c c:\windows\*.* /s /i /exe:lzx";
            Logger.Log($"Starting compression with arguments: {command}", Level.INFO);
            Runner.Command("compact.exe", command);
            Logger.Log("Compression process completed.", Level.SUCCESS);
        }

        public static void CleanupWinSxS()
        {
            string command = "/Online /Cleanup-Image /StartComponentCleanup /ResetBase";
            Logger.Log("Cleaning up WinSxS store...", Level.INFO);
            Runner.Command("dism.exe", command, redirectOutputLogger: true);
            Logger.Log("WinSxS cleanup completed.", Level.SUCCESS);
        }
    }
}
