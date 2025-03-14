namespace DebloaterTool
{
    internal class WinStore
    {
        public static void UninstallWindowsStore()
        {
            // Remove Microsoft Store for the current user.
            ComFunction.RunCommand("powershell", "-NoProfile -Command \"Get-AppxPackage *WindowsStore* | Remove-AppxPackage\"");

            // Remove Microsoft Store for all users.
            ComFunction.RunCommand("powershell", "-NoProfile -Command \"Get-AppxPackage -AllUsers *WindowsStore* | Remove-AppxPackage\"");
        }
    }
}
