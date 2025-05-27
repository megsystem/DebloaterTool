using System;
using System.IO;

namespace DebloaterTool.Helpers
{
    internal class Zip
    {
        // Make sure that any method calling this runs on an STA thread.
        public static void ExtractZipFile(string zipFilePath, string extractPath)
        {
            // Create the extraction directory if it doesn't exist.
            if (!Directory.Exists(extractPath))
            {
                Directory.CreateDirectory(extractPath);
            }

            // Use COM interop to access the Shell object.
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            dynamic shell = Activator.CreateInstance(shellAppType);

            // Get the ZIP folder and the destination folder.
            dynamic zipFolder = shell.NameSpace(zipFilePath);
            dynamic destinationFolder = shell.NameSpace(extractPath);

            // The flags (4 | 16) mean:
            // 4: Do not display a progress dialog box.
            // 16: Respond "Yes to All" if a dialog box is displayed.
            destinationFolder.CopyHere(zipFolder.Items(), 4 | 16);

            // Optionally, you might want to wait a bit to ensure extraction is complete,
            // because CopyHere is asynchronous.
            System.Threading.Thread.Sleep(1000);
        }
    }
}
