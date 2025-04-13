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
        /// Disables telemetry, diagnostic, and related services/settings.
        /// </summary>
        public static void DisableTelAndDiagnost()
        {
            var modifications = new[]
            {
                new RegistryModification(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DiagTrack", "Start", RegistryValueKind.DWord, 4),
                new RegistryModification(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\diagnosticshub.standardcollector.service", "Start", RegistryValueKind.DWord, 4),
                new RegistryModification(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\dmwappushservice", "Start", RegistryValueKind.DWord, 4),
                new RegistryModification(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DcpSvc", "Start", RegistryValueKind.DWord, 4),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisableEngine", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "SbEnable", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\SQMClient\Windows", "CEIPEnable", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "AITEnable", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisableInventory", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisablePCA", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisableUAR", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Device Metadata", "PreventDeviceMetadataFromNetwork", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\WMI\AutoLogger\SQMLogger", "Start", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\PolicyManager\current\device\System", "AllowExperimentation", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "RotatingLockScreenOverlayEnabled", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "RotatingLockScreenEnabled", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "DisableWindowsSpotlightFeatures", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "DisableTailoredExperiencesWithDiagnosticData", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\CloudContent", "DisableCloudOptimizedContent", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "DoNotShowFeedbackNotifications", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo", "DisabledByGroupPolicy", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\PolicyManager\current\device\Bluetooth", "AllowAdvertising", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "DisableAutomaticRestartSignOn", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\TabletPC", "PreventHandwritingDataSharing", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\TextInput", "AllowLinguisticDataCollection", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\InputPersonalization", "AllowInputPersonalization", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\SearchSettings", "SafeSearchMode", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "UploadUserActivities", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "AllowCrossDeviceClipboard", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"Software\Policies\Microsoft\Windows\Messaging", "AllowMessageSync", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"Software\Policies\Microsoft\Windows\SettingSync", "DisableCredentialsSettingSync", RegistryValueKind.DWord, 2),
                new RegistryModification(Registry.LocalMachine, @"Software\Policies\Microsoft\Windows\SettingSync", "DisableCredentialsSettingSyncUserOverride", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"Software\Policies\Microsoft\Windows\SettingSync", "DisableApplicationSettingSync", RegistryValueKind.DWord, 2),
                new RegistryModification(Registry.LocalMachine, @"Software\Policies\Microsoft\Windows\SettingSync", "DisableApplicationSettingSyncUserOverride", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppPrivacy", "LetAppsActivateWithVoice", RegistryValueKind.DWord, 2),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\FindMyDevice", "AllowFindMyDevice", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Settings\FindMyDevice", "LocationSyncEnabled", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "EnableActivityFeed", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "EnableCdp", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.Users, @".DEFAULT\Software\Microsoft\Windows\CurrentVersion\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack", "ShowedToastAtLevel", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.Users, @".DEFAULT\SOFTWARE\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack", "ShowedToastAtLevel", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Speech_OneCore\Settings\OnlineSpeechPrivacy", "HasAccepted", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location", "Value", RegistryValueKind.String, "Deny"),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Settings\FindMyDevice", "LocationSyncEnabled", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableLocation", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableLocationScripting", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableWindowsLocationProvider", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}", "SensorPermissionState", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"System\CurrentControlSet\Services\lfsvc\Service\Configuration", "Status", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Biometrics", "Enabled", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Feeds", "ShellFeedsTaskbarOpenOnHover", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\CDP", "CdpSessionUserAuthzPolicy", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\CDP", "NearShareChannelUserAuthzPolicy", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\CDP", "RomeSdkChannelUserAuthzPolicy", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\CDP", "EnableWebContentEvaluation", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Control Panel\International\User Profile", "HttpAcceptLanguageOptOut", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\SmartGlass", "UserAuthPolicy", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Personalization\Settings", "AcceptedPrivacyPolicy", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\SettingSync\Groups\Language", "Enabled", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\InputPersonalization", "RestrictImplicitTextCollection", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\InputPersonalization", "RestrictImplicitInkCollection", RegistryValueKind.DWord, 1),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\InputPersonalization\TrainedDataStore", "HarvestContacts", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"SOFTWARE\Microsoft\Input\TIPC", "Enabled", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppPrivacy", "LetAppsSyncWithDevices", RegistryValueKind.DWord, 2),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\LooselyCoupled", "Value", RegistryValueKind.String, "Deny"),
                new RegistryModification(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\AppHost", "EnableWebContentEvaluation", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\AppHost\EnableWebContentEvaluation", "Enabled", RegistryValueKind.DWord, 0)
            };

            // Apply all modifications.
            HelperRegedit.InstallRegModification(modifications);
        }
    }
}
