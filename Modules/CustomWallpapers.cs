﻿using DebloaterTool.Helpers;
using DebloaterTool.Settings;
using DebloaterTool.Logging;
using System;
using System.IO;

namespace DebloaterTool.Modules
{
    internal class CustomWallpapers
    {
        /// <summary>
        /// Downloads a custom wallpaper and sets it as the desktop background.
        /// </summary>
        public static void InstallWallpapers()
        {
            try
            {
                // Now download the lockscreen
                string lockscreenName = "Lockscreen.png";
                string lockscreenUrl = $"{Global.wallpaper}/{lockscreenName}";
                string lockscreenLocalPath = Path.Combine(Global.wallpapersPath, lockscreenName);
                DownloadAndLog(lockscreenUrl, lockscreenLocalPath, "Lockscreen");

                // Set Wallpaper Lockscreen
                Wallpaper.SetLockScreenWallpaper(lockscreenLocalPath);
                Logger.Log("Wallpaper Lockscreen setted successfully.", Level.SUCCESS);

                // Now download the desktop wallpapers
                int i = 1;
                while (true)
                {
                    string fileName = $"{i}.png";
                    string fileUrl = $"{Global.wallpaper}/{fileName}";
                    string fileLocalPath = Path.Combine(Global.wallpapersPath, fileName);

                    if (!DownloadAndLog(fileUrl, fileLocalPath, $"Wallpaper #{i}"))
                        break;

                    i++;
                }

                // Set Wallpaper Desktop
                Wallpaper.SetWallpaperSlideshowFromFolder(Global.wallpapersPath);
                Logger.Log("Wallpaper Slideshow setted successfully.", Level.SUCCESS);
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
            if (!Internet.DownloadFile(url, path))
            {
                Logger.Log($"Unable to download the {description} from \"{url}\". Skipping...", Level.WARNING);
                return false;
            }

            Logger.Log($"{description} downloaded successfully to \"{path}\".", Level.SUCCESS);
            return true;
        }
    }
}
