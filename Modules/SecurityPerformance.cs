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

            HelperRegedit.InstallRegModification(modifications);
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

            HelperRegedit.InstallRegModification(modifications);
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

            HelperRegedit.InstallRegModification(modifications);
        }

        /// <summary>
        /// Disables Windows Error Reporting and stops related services by modifying the registry.
        /// </summary>
        public static void DisableWinErrorReporting()
        {
            var modifications = new[]
            {
                // Disable Windows Error Reporting via policies.
                new RegistryModification(
                    Registry.LocalMachine,
                    @"SOFTWARE\Policies\Microsoft\Windows\Windows Error Reporting",
                    "Disabled",
                    RegistryValueKind.DWord,
                    1),

                // Disable error reporting for PC Health.
                new RegistryModification(
                    Registry.LocalMachine,
                    @"SOFTWARE\Policies\Microsoft\PCHealth\ErrorReporting",
                    "DoReport",
                    RegistryValueKind.DWord,
                    0),

                // Set WerSvc (Windows Error Reporting Service) to disabled.
                new RegistryModification(
                    Registry.LocalMachine,
                    @"SYSTEM\CurrentControlSet\Services\WerSvc",
                    "Start",
                    RegistryValueKind.DWord,
                    4),

                // Set wercplsupport (WER Control Panel Support Service) to disabled.
                new RegistryModification(
                    Registry.LocalMachine,
                    @"SYSTEM\CurrentControlSet\Services\wercplsupport",
                    "Start",
                    RegistryValueKind.DWord,
                    4)
            };

            // Apply registry changes.
            HelperRegedit.InstallRegModification(modifications);
        }

        /// <summary>
        /// Applies security-and-performance registry tweaks:
        ///   • Disables telemetry/diagnostic services
        ///   • Turns off data-collection policies and feedback mechanisms
        ///   • Disables location sensors and biometrics
        /// </summary>
        public static void ApplySecurityPerformanceTweaks()
        {
            var modifications = new[]
            {
                // Stop telemetry/diagnostic services
                new RegistryModification(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DiagTrack", "Start", RegistryValueKind.DWord, 4),
                new RegistryModification(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\diagnosticshub.standardcollector.service", "Start", RegistryValueKind.DWord, 4),
                new RegistryModification(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\dmwappushservice", "Start", RegistryValueKind.DWord, 4),
                new RegistryModification(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DcpSvc", "Start", RegistryValueKind.DWord, 4),
                new RegistryModification(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\WMI\AutoLogger\SQMLogger", "Start", RegistryValueKind.DWord, 0),
                // Disable AppCompat and feedback/data-collection policies
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisableEngine", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "SbEnable", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "AITEnable", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisableInventory", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisablePCA", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisableUAR", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\SQMClient\Windows", "CEIPEnable", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\CloudContent", "DisableCloudOptimizedContent", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "DoNotShowFeedbackNotifications", RegistryValueKind.DWord, 1),
                // PolicyManager experimentation toggle
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\PolicyManager\current\device\System", "AllowExperimentation", RegistryValueKind.DWord, 0),
                // Disable AdvertisingInfo (group-policy lock)
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo", "DisabledByGroupPolicy", RegistryValueKind.DWord, 1),
                // Disable location sensors
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableLocation", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableLocationScripting", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableWindowsLocationProvider", RegistryValueKind.DWord, 1),
                // Sensor permission override
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}", "SensorPermissionState", RegistryValueKind.DWord, 0),
                // Disable biometric authentication
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Biometrics", "Enabled", RegistryValueKind.DWord, 0),
            };

            HelperRegedit.InstallRegModification(modifications);
        }
    }
}
