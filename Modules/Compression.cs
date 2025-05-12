namespace DebloaterTool
{
    internal class Compression
    {
        public static void CompressOS()
        {
            string command = @"/c c:\windows\*.* /s /i /exe:lzx";
            Logger.Log($"Starting compression with arguments: {command}", Level.INFO);
            HelperRunner.Command("compact.exe", command);
            Logger.Log("Compression process completed.", Level.SUCCESS);
        }

        public static void CleanupWinSxS()
        {
            string command = "/Online /Cleanup-Image /StartComponentCleanup /ResetBase";
            Logger.Log("Cleaning up WinSxS store...", Level.INFO);
            HelperRunner.Command("dism.exe", command, NoWindow: false);
            Logger.Log("WinSxS cleanup completed.", Level.SUCCESS);
        }
    }
}
