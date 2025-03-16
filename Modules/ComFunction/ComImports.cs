using System.Runtime.InteropServices;

namespace DebloaterTool
{
    internal class ComImports
    {
        // Import SystemParametersInfo from user32.dll to change the desktop wallpaper
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        // Constants for setting the wallpaper
        public const int SPI_SETDESKWALLPAPER = 20; // Action to change the wallpaper
        public const int SPIF_UPDATEINIFILE = 0x01; // Update user profile settings
        public const int SPIF_SENDCHANGE = 0x02;   // Broadcast change to all windows
    }
}
