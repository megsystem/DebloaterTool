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
                Directory.CreateDirectory(wallpaperFinalPath);

                int i = 1;
                while ( true )
                {
                    string wallpaperUrl = $"{ExternalLinks.wallpaper}/{i}.png";
                    string wallpaperPath = Path.Combine(wallpaperFinalPath, $"{i}.png");

                    // Attempt to download the wallpaper file
                    Logger.Log($"Downloading \"{wallpaperUrl}\" wallpaper...");
                    if (!HelperGlobal.DownloadFile(wallpaperUrl, wallpaperPath))
                    {
                        Logger.Log($"Wallpaper \"{wallpaperUrl}\" not found! Skipping...", Level.ERROR);
                        break;
                    }
                    Logger.Log($"Wallpaper \"{wallpaperUrl}\" downloaded successfully to \"{wallpaperPath}\".", Level.SUCCESS);

                    //
                    i++;
                }

                HelperWallpaper.SetWallpaperSlideshowFromFolder(wallpaperFinalPath);
                Logger.Log("Wallpaper SlideShow setted successfully.", Level.SUCCESS);
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the process
                Logger.Log($"Error setting wallpaper: {ex.Message}", Level.ERROR);
            }
        }
    }
}
