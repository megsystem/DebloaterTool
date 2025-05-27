using DebloaterTool.Helpers;
using Microsoft.Win32;

namespace DebloaterTool.Modules
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

            Regedit.InstallRegModification(modifications);
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

            Regedit.InstallRegModification(modifications);
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

            Regedit.InstallRegModification(modifications);
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
            Regedit.InstallRegModification(modifications);
        }

        /// <summary>
        /// Applies security-related registry tweaks to enhance system security
        /// </summary>
        public static void ApplySecurityPerformanceTweaks()
        {
            var modifications = new[]
            {
                // Disable automatic sign-on after restart (security measure)
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "DisableAutomaticRestartSignOn", RegistryValueKind.DWord, 1),
                // Disable Find My Device (prevents device tracking)
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\FindMyDevice", "AllowFindMyDevice", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Settings\FindMyDevice", "LocationSyncEnabled", RegistryValueKind.DWord, 0),
                // Disable cross-device clipboard (prevents data leakage)
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "AllowCrossDeviceClipboard", RegistryValueKind.DWord, 0),
                // Disable credential syncing (prevents password exposure)
                new RegistryModification(Registry.LocalMachine, @"Software\Policies\Microsoft\Windows\SettingSync", "DisableCredentialsSettingSync", RegistryValueKind.DWord, 2),
                new RegistryModification(Registry.LocalMachine, @"Software\Policies\Microsoft\Windows\SettingSync", "DisableCredentialsSettingSyncUserOverride", RegistryValueKind.DWord, 1),
                // Disable biometrics (can be exploited)
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Biometrics", "Enabled", RegistryValueKind.DWord, 0),
                // Disable device metadata network access (reduces network attack surface)
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Device Metadata", "PreventDeviceMetadataFromNetwork", RegistryValueKind.DWord, 1),
                // Disable app sync with devices (limits cross-device attack vectors)
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppPrivacy", "LetAppsSyncWithDevices", RegistryValueKind.DWord, 2),
                // Deny device access for loosely coupled apps (improves access control)
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\LooselyCoupled", "Value", RegistryValueKind.String, "Deny"),
                // Disable CDP connections (reduces network exposure)
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\CDP", "CdpSessionUserAuthzPolicy", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\CDP", "NearShareChannelUserAuthzPolicy", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\CDP", "RomeSdkChannelUserAuthzPolicy", RegistryValueKind.DWord, 0),
                // Disable PCA (Program Compatibility Assistant - can be a security risk)
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisablePCA", RegistryValueKind.DWord, 1),
                // Disable SbEnable (SecureBoot compatibility check - can leak info)
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "SbEnable", RegistryValueKind.DWord, 0),
                // Disable location services (prevents location tracking)
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location", "Value", RegistryValueKind.String, "Deny"),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableLocation", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableLocationScripting", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableWindowsLocationProvider", RegistryValueKind.DWord, 1),
                // Disable sensors (prevents unauthorized sensor access)
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}", "SensorPermissionState", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"System\CurrentControlSet\Services\lfsvc\Service\Configuration", "Status", RegistryValueKind.DWord, 0)
            };

            Regedit.InstallRegModification(modifications);
        }
    }
}
