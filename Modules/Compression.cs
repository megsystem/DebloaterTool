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
            HelperRunner.Command("compact.exe", command);
            Logger.Log("Compression process completed.", Level.SUCCESS);
        }
    }
}
