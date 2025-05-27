using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using DebloaterTool.Logging;
using System;

namespace DebloaterTool.Helpers
{
    internal class Admins
    {
        public static void RestartAsAdmin(string[] args)
        {
            ProcessStartInfo proc = new ProcessStartInfo()
            {
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true,
                Verb = "runas",
                Arguments = string.Join(" ", args.Select(arg =>
                    arg.Contains(" ") ? $"\"{arg}\"" : arg)) // Quote args with spaces
            };

            try
            {
                Process.Start(proc);
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to start as administrator: {ex.Message}", Level.ERROR);
            }

            Environment.Exit(0);
        }

        // Checks if the current process is running as administrator.
        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
