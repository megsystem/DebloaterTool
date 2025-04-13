namespace DebloaterTool
{
    internal class WinStore
    {
        /// <summary>
        /// Uninstalls Microsoft Store by executing PowerShell commands to remove the Windows Store
        /// package for the current user as well as for all users on the system.
        /// </summary>
        public static void Uninstall()
        {
            // Remove Microsoft Store for the current user.
            HelperGlobal.RunCommand("powershell", "-NoProfile -Command \"Get-AppxPackage *WindowsStore* | Remove-AppxPackage\"");

            // Remove Microsoft Store for all users.
            HelperGlobal.RunCommand("powershell", "-NoProfile -Command \"Get-AppxPackage -AllUsers *WindowsStore* | Remove-AppxPackage\"");
        }
    }
}
