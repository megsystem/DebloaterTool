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
                string wallpaperFinalPath = @"C:\Wallpapers";
                Logger.Log("Downloading custom wallpaper...");

                int i = 0;
                while ( true )
                {
                    string wallpaperUrl = $"{ExternalLinks.wallpaper}/{i}.png";
                    string wallpaperPath = Path.Combine(wallpaperFinalPath, $"{i}.png");

                    // Attempt to download the wallpaper file
                    if (!HelperGlobal.DownloadFile(wallpaperPath, wallpaperPath))
                    {
                        Logger.Log("Failed to download wallpaper. Exiting...", Level.ERROR);
                        break;
                    }
                    Logger.Log($"Wallpaper {wallpaperUrl} downloaded successfully.");

                    i++;
                }

                HelperWallpaper.SetWallpaperSlideshowFromFolder(wallpaperFinalPath);
                Logger.Log("Wallpaper setted successfully.");
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the process
                Logger.Log($"Error setting wallpaper: {ex.Message}", Level.ERROR);
            }
        }
    }
}
