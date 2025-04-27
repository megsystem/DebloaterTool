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
                // Prepare the destination folder
                string wallpaperFinalPath = @"C:\DebloaterTool\Wallpapers";
                Directory.CreateDirectory(wallpaperFinalPath);

                // Now download the desktop wallpapers
                int i = 1;
                while (true)
                {
                    string fileName = $"{i}.png";
                    string fileUrl = $"{ExternalLinks.wallpaper}/{fileName}";
                    string fileLocalPath = Path.Combine(wallpaperFinalPath, fileName);

                    if (!DownloadAndLog(fileUrl, fileLocalPath, $"Wallpaper #{i}"))
                        break;

                    i++;
                }

                // Now download the lockscreen
                string lockscreenName = "Lockscreen.png";
                string lockscreenUrl = $"{ExternalLinks.wallpaper}/{lockscreenName}";
                string lockscreenLocalPath = Path.Combine(wallpaperFinalPath, lockscreenName);
                DownloadAndLog(lockscreenUrl, lockscreenLocalPath, "Lockscreen");

                // Set wallpapers
                HelperWallpaper.SetWallpaperSlideshowFromFolder(wallpaperFinalPath);
                HelperWallpaper.SetLockScreenWallpaper(lockscreenLocalPath);
                Logger.Log("Wallpaper SlideShow setted successfully.", Level.SUCCESS);
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the process
                Logger.Log($"Error setting wallpaper: {ex.Message}", Level.ERROR);
            }
        }

        /// <summary>
        /// Attempts to download from <paramref name="url"/> to <paramref name="path"/>,
        /// logging both the attempt and the result with a friendly description.
        /// </summary>
        static bool DownloadAndLog(string url, string path, string description)
        {
            Logger.Log($"Downloading {description} from \"{url}\"...");
            if (!HelperGlobal.DownloadFile(url, path))
            {
                Logger.Log($"Unable to download the {description} from \"{url}\". Skipping...", Level.WARNING);
                return false;
            }

            Logger.Log($"{description} downloaded successfully to \"{path}\".", Level.SUCCESS);
            return true;
        }
    }
}
