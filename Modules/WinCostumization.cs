using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace DebloaterTool
{
    internal class WinCostumization
    {
        public static void DisableSnapTools()
        {
            SetRegistryValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "WindowArrangementActive", "0", RegistryValueKind.String);
            SetRegistryValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "JointResize", 0, RegistryValueKind.DWord);
            SetRegistryValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "SnapAssist", 0, RegistryValueKind.DWord);
            SetRegistryValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "SnapFill", 0, RegistryValueKind.DWord);
            SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "EnableSnapAssistFlyout", 0, RegistryValueKind.DWord);
        }

        public static void EnableUltimatePerformance()
        {
            string ultimatePlan = ComFunction.RunCommand("cmd.exe", "/c powercfg -list");
            if (ultimatePlan.Contains("Ultimate Performance"))
            {
                Logger.Log("Ultimate Performance plan is already installed.");
            }
            else
            {
                Logger.Log("Installing Ultimate Performance plan...");
                ComFunction.RunCommand("cmd.exe", "/c powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                Logger.Log("> Ultimate Performance plan installed.");
            }

            string updatedPlanList = ComFunction.RunCommand("cmd.exe", "/c powercfg -list");
            string ultimatePlanGUID = ExtractGUID(updatedPlanList, "Ultimate Performance");
            if (!string.IsNullOrEmpty(ultimatePlanGUID))
            {
                ComFunction.RunCommand("cmd.exe", $"/c powercfg -setactive {ultimatePlanGUID}");
                Logger.Log("Ultimate Performance plan is now active.");
            }
        }

        static string ExtractGUID(string powercfgOutput, string planName)
        {
            foreach (string line in powercfgOutput.Split('\n'))
            {
                if (line.Contains(planName))
                {
                    string[] parts = line.Split(' ');
                    foreach (string part in parts)
                    {
                        if (part.Contains("-"))
                        {
                            return part.Trim();
                        }
                    }
                }
            }
            return string.Empty;
        }

        static void SetRegistryValue(string path, string name, object value, RegistryValueKind valueKind)
        {
            try
            {
                var parts = path.Split(new[] { '\\' }, 2);
                var rootKey = parts[0];
                var subKey = parts.Length > 1 ? parts[1].TrimStart('\\') : string.Empty;

                Logger.Log($"Attempting to set registry value at RootKey: {rootKey}, SubKey: {subKey}");

                RegistryKey baseKey;
                switch (rootKey)
                {
                    case "HKEY_LOCAL_MACHINE":
                        baseKey = Registry.LocalMachine;
                        break;
                    case "HKEY_CURRENT_USER":
                        baseKey = Registry.CurrentUser;
                        break;
                    case "HKEY_CLASSES_ROOT":
                        baseKey = Registry.ClassesRoot;
                        break;
                    case "HKEY_USERS":
                        baseKey = Registry.Users;
                        break;
                    case "HKEY_CURRENT_CONFIG":
                        baseKey = Registry.CurrentConfig;
                        break;
                    default:
                        throw new ArgumentException("Unknown registry root key: " + rootKey);
                }

                using (var key = baseKey.OpenSubKey(subKey, true) ?? baseKey.CreateSubKey(subKey))
                {
                    if (key == null)
                    {
                        Logger.Log($"Failed to create or open registry key: {path}", Level.ERROR);
                        return;
                    }
                    key.SetValue(name, value, valueKind);
                    Logger.Log($"Updated {path} -> {name} = {value}", Level.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message, Level.ERROR);
            }
        }
    }
}
