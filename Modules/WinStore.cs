namespace DebloaterTool
{
    internal class WinStore
    {
        public static void UninstallWindowsStore()
        {
            // Remove Microsoft Store for the current user.
            ComGlobal.RunCommand("powershell", "-NoProfile -Command \"Get-AppxPackage *WindowsStore* | Remove-AppxPackage\"");

            // Remove Microsoft Store for all users.
            ComGlobal.RunCommand("powershell", "-NoProfile -Command \"Get-AppxPackage -AllUsers *WindowsStore* | Remove-AppxPackage\"");
        }
    }
}
