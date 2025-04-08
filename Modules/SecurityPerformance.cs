using Microsoft.Win32;

namespace DebloaterTool
{
    internal class SecurityPerformance
    {
        /// <summary>
        /// Disables SMBv1 by applying registry modifications via the ComRegedit library.
        /// It sets the SMB1 value for the server parameters to 0 and disables the legacy SMB driver.
        /// </summary>
        public static void DisableSMBv1()
        {
            var modifications = new[]
            {
                // Disable SMBv1 Server: set "SMB1" key to 0.
                new RegistryModification(
                    Registry.LocalMachine,
                    @"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters",
                    "SMB1",
                    RegistryValueKind.DWord,
                    0),
                    
                // Disable the legacy SMB driver: set "Start" to 4 (disabled).
                new RegistryModification(
                    Registry.LocalMachine,
                    @"SYSTEM\CurrentControlSet\Services\mrxsmb10",
                    "Start",
                    RegistryValueKind.DWord,
                    4)
            };

            ComRegedit.InstallRegModification(modifications);
        }

        /// <summary>
        /// Disables Remote Desktop and Remote Assistance by applying registry modifications 
        /// via the ComRegedit library. It updates the respective registry keys to disable both features.
        /// </summary>
        public static void DisableRemAssistAndRemDesk()
        {
            var modifications = new[]
            {
                // Disable Remote Desktop: set fDenyTSConnections to 1.
                new RegistryModification(
                    Registry.LocalMachine,
                    @"SYSTEM\CurrentControlSet\Control\Terminal Server",
                    "fDenyTSConnections",
                    RegistryValueKind.DWord,
                    1),
                    
                // Disable Remote Assistance: set fAllowToGetHelp to 0.
                new RegistryModification(
                    Registry.LocalMachine,
                    @"SYSTEM\CurrentControlSet\Control\Remote Assistance",
                    "fAllowToGetHelp",
                    RegistryValueKind.DWord,
                    0)
            };

            ComRegedit.InstallRegModification(modifications);
        }

        /// <summary>
        /// Disables Spectre and Meltdown mitigations in Windows for increased performance.
        /// </summary>
        public static void DisableSpectreAndMeltdown()
        {
            var modifications = new[]
{
                new RegistryModification(
                    Registry.LocalMachine,
                    @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management",
                    "FeatureSettingsOverride",
                    RegistryValueKind.DWord,
                    3),

                new RegistryModification(
                    Registry.LocalMachine,
                    @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management",
                    "FeatureSettingsOverrideMask",
                    RegistryValueKind.DWord,
                    3)
            };

            ComRegedit.InstallRegModification(modifications);
        }

        /// <summary>
        /// Disables Windows Error Reporting to reduce background overhead and logging.
        /// This sets the "Disabled" value in the Windows Error Reporting key to 1.
        /// </summary>
        public static void DisableWinErrorReporting()
        {
            ComRegedit.InstallRegModification(
                new RegistryModification(
                    Registry.LocalMachine,
                    @"SOFTWARE\Microsoft\Windows\Windows Error Reporting",
                    "Disabled",
                    RegistryValueKind.DWord,
                    1));
        }

        /// <summary>
        /// Disables Telemetry and Diagnostic Data Collection to reduce resource usage and background data transmission.
        /// This sets the "AllowTelemetry" value in the DataCollection policies to 0.
        /// Note: Value settings may vary by Windows version; verify this is correct for your system.
        /// </summary>
        public static void DisableTelAndDiagnost()
        {
            ComRegedit.InstallRegModification(
                new RegistryModification(
                    Registry.LocalMachine,
                    @"SOFTWARE\Policies\Microsoft\Windows\DataCollection",
                    "AllowTelemetry",
                    RegistryValueKind.DWord,
                    0));
        }
    }
}
