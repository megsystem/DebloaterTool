using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace DebloaterTool
{
    internal class Wallpaper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        public static void SetCustomWallpaper()
        {
            string picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string savePath = Path.Combine(picturesFolder, "DebloaterTool.png");

            DownloadImage(ExternalLinks.wallpaper, savePath);
            SetWallpaper(savePath);
        }

        static void DownloadImage(string url, string savePath)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(url, savePath);
                    Logger.Log($"Desktop background downloaded to: {savePath}", Level.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to download desktop background: {ex.Message}", Level.ERROR);
            }
        }

        static void SetWallpaper(string imagePath)
        {
            try
            {
                int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
                if (result != 0)
                {
                    Logger.Log("Wallpaper change completed.", Level.SUCCESS);
                }
                else
                {
                    Logger.Log("Wallpaper change failed.", Level.ERROR);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error setting wallpaper: {ex.Message}", Level.ERROR);
            }
        }
    }
}
