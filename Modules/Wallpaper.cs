using System;
using System.IO;

namespace DebloaterTool
{
    internal class Wallpaper
    {
        /// <summary>
        /// Downloads a custom wallpaper and sets it as the desktop background.
        /// </summary>
        public static void SetCustomWallpaper()
        {
            try
            {
                // Get the path to the Pictures folder
                string picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                string savePath = Path.Combine(picturesFolder, "DebloaterTool.png");

                Logger.Log("Downloading custom wallpaper...");

                // Attempt to download the wallpaper file
                if (!HelperGlobal.DownloadFile(ExternalLinks.wallpaper, savePath))
                {
                    Logger.Log("Failed to download wallpaper. Exiting...", Level.ERROR);
                    return;
                }

                Logger.Log("Wallpaper downloaded successfully.");

                // Apply the downloaded image as the desktop wallpaper
                int result = HelperImports.SystemParametersInfo(
                    HelperImports.SPI_SETDESKWALLPAPER, 
                    0, 
                    savePath, 
                    HelperImports.SPIF_UPDATEINIFILE | HelperImports.SPIF_SENDCHANGE
                );

                // Check if the wallpaper was successfully changed
                if (result != 0)
                {
                    Logger.Log("Wallpaper successfully changed.", Level.SUCCESS);
                }
                else
                {
                    Logger.Log("Wallpaper change failed.", Level.ERROR);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the process
                Logger.Log($"Error setting wallpaper: {ex.Message}", Level.ERROR);
            }
        }
    }
}
