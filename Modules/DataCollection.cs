﻿using DebloaterTool.Helpers;
using Microsoft.Win32;

namespace DebloaterTool.Modules
{
    internal class DataCollection
    {
        /// <summary>
        /// Disables Windows telemetry and diagnostic services by setting their start type to disabled
        /// </summary>
        public static void DisableTelemetryServices()
        {
            var modifications = new[]
            {
                new TweakRegistry(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DiagTrack", "Start", RegistryValueKind.DWord, 4),
                new TweakRegistry(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\diagnosticshub.standardcollector.service", "Start", RegistryValueKind.DWord, 4),
                new TweakRegistry(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\dmwappushservice", "Start", RegistryValueKind.DWord, 4),
                new TweakRegistry(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DcpSvc", "Start", RegistryValueKind.DWord, 4),
                new TweakRegistry(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\WMI\AutoLogger\SQMLogger", "Start", RegistryValueKind.DWord, 0)
            };

            Regedit.InstallRegModification(modifications);
        }

        /// <summary>
        /// Disables various data collection policies and feedback mechanisms
        /// </summary>
        public static void DisableDataCollectionPolicies()
        {
            var modifications = new[]
            {
                // App compatibility and inventory features
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisableEngine", RegistryValueKind.DWord, 1),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "AITEnable", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisableInventory", RegistryValueKind.DWord, 1),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisableUAR", RegistryValueKind.DWord, 1),
                // User activity and data collection
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "UploadUserActivities", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "EnableActivityFeed", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "EnableCdp", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\SQMClient\Windows", "CEIPEnable", RegistryValueKind.DWord, 0),
                // Experimentation
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Microsoft\PolicyManager\current\device\System", "AllowExperimentation", RegistryValueKind.DWord, 0),
                // Feedback and diagnostics
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "DoNotShowFeedbackNotifications", RegistryValueKind.DWord, 1),
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.Users, @".DEFAULT\Software\Microsoft\Windows\CurrentVersion\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack", "ShowedToastAtLevel", RegistryValueKind.DWord, 1),
                new TweakRegistry(Registry.Users, @".DEFAULT\SOFTWARE\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack", "ShowedToastAtLevel", RegistryValueKind.DWord, 1),
                // Speech and input settings
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Speech_OneCore\Settings\OnlineSpeechPrivacy", "HasAccepted", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\TabletPC", "PreventHandwritingDataSharing", RegistryValueKind.DWord, 1),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\TextInput", "AllowLinguisticDataCollection", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\InputPersonalization", "AllowInputPersonalization", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\InputPersonalization", "RestrictImplicitTextCollection", RegistryValueKind.DWord, 1),
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\InputPersonalization", "RestrictImplicitInkCollection", RegistryValueKind.DWord, 1),
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\InputPersonalization\TrainedDataStore", "HarvestContacts", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.CurrentUser, @"SOFTWARE\Microsoft\Input\TIPC", "Enabled", RegistryValueKind.DWord, 0),
                // Sync settings
                new TweakRegistry(Registry.LocalMachine, @"Software\Policies\Microsoft\Windows\Messaging", "AllowMessageSync", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.LocalMachine, @"Software\Policies\Microsoft\Windows\SettingSync", "DisableApplicationSettingSync", RegistryValueKind.DWord, 2),
                new TweakRegistry(Registry.LocalMachine, @"Software\Policies\Microsoft\Windows\SettingSync", "DisableApplicationSettingSyncUserOverride", RegistryValueKind.DWord, 1),
                // Voice features
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppPrivacy", "LetAppsActivateWithVoice", RegistryValueKind.DWord, 2),
                // Web content settings
                new TweakRegistry(Registry.CurrentUser, @"Control Panel\International\User Profile", "HttpAcceptLanguageOptOut", RegistryValueKind.DWord, 1),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\SmartGlass", "UserAuthPolicy", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Personalization\Settings", "AcceptedPrivacyPolicy", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\SettingSync\Groups\Language", "Enabled", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\SearchSettings", "SafeSearchMode", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\CDP", "EnableWebContentEvaluation", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\AppHost", "EnableWebContentEvaluation", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\AppHost\EnableWebContentEvaluation", "Enabled", RegistryValueKind.DWord, 0)
            };

            Regedit.InstallRegModification(modifications);
        }

        /// <summary>
        /// Disables advertising features and content delivery
        /// </summary>
        public static void DisableAdvertisingAndContentDelivery()
        {
            var modifications = new[]
            {
                // Content delivery
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "RotatingLockScreenOverlayEnabled", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "RotatingLockScreenEnabled", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "DisableWindowsSpotlightFeatures", RegistryValueKind.DWord, 1),
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "DisableTailoredExperiencesWithDiagnosticData", RegistryValueKind.DWord, 1),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\CloudContent", "DisableCloudOptimizedContent", RegistryValueKind.DWord, 1),
                // Advertising
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo", "DisabledByGroupPolicy", RegistryValueKind.DWord, 1),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Microsoft\PolicyManager\current\device\Bluetooth", "AllowAdvertising", RegistryValueKind.DWord, 0),
                new TweakRegistry(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", RegistryValueKind.DWord, 0),
                // Feeds
                new TweakRegistry(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Feeds", "ShellFeedsTaskbarOpenOnHover", RegistryValueKind.DWord, 0)
            };

            Regedit.InstallRegModification(modifications);
        }
    }
}
