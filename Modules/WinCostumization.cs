using Microsoft.Win32;

namespace DebloaterTool
{
    internal class WinCostumization
    {
        public static void DisableSnapTools()
        {
            RegistryModification[] registryModifications = new RegistryModification[]
            {
                new RegistryModification(Registry.CurrentUser, @"Control Panel\Desktop", "WindowArrangementActive", RegistryValueKind.String, "0"),
                new RegistryModification(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "JointResize", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "SnapAssist", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "SnapFill", RegistryValueKind.DWord, 0),
                new RegistryModification(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "EnableSnapAssistFlyout", RegistryValueKind.DWord, 0)
            };

            ComRegedit.InstallRegModification(registryModifications);
        }

        public static void EnableUltimatePerformance()
        {
            string ultimatePlan = ComGlobal.RunCommand("cmd.exe", "/c powercfg -list", redirect: true);
            if (ultimatePlan.Contains("Ultimate Performance"))
            {
                Logger.Log("Ultimate Performance plan is already installed.");
            }
            else
            {
                Logger.Log("Installing Ultimate Performance plan...");
                ComGlobal.RunCommand("cmd.exe", "/c powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                Logger.Log("> Ultimate Performance plan installed.");
            }

            string updatedPlanList = ComGlobal.RunCommand("cmd.exe", "/c powercfg -list", redirect: true);
            string ultimatePlanGUID = ExtractGUID(updatedPlanList, "Ultimate Performance");
            if (!string.IsNullOrEmpty(ultimatePlanGUID))
            {
                ComGlobal.RunCommand("cmd.exe", $"/c powercfg -setactive {ultimatePlanGUID}");
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
    }
}
