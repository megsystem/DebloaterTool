using Microsoft.Win32;

namespace DebloaterTool
{
    internal class WinCostumization
    {
        /// <summary>
        /// Disables various Windows snap tools by modifying registry settings for the current user.
        /// This function turns off window arrangement, joint resizing, Snap Assist, Snap Fill,
        /// and the Snap Assist flyout.
        /// </summary>
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

            HelperRegedit.InstallRegModification(registryModifications);
        }

        /// <summary>
        /// Enables the Ultimate Performance power plan by checking if it is installed,
        /// installing it if necessary, extracting its GUID from the updated plan list, 
        /// and then setting it as the active power plan.
        /// </summary>
        public static void EnableUltimatePerformance()
        {
            // Retrieve the current list of power plans.
            string ultimatePlan = HelperRunner.Command("cmd.exe", "/c powercfg -list", redirect: true);
            if (ultimatePlan.Contains("Ultimate Performance"))
            {
                Logger.Log("Ultimate Performance plan is already installed.");
            }
            else
            {
                Logger.Log("Installing Ultimate Performance plan...");
                HelperRunner.Command("cmd.exe", "/c powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                Logger.Log("> Ultimate Performance plan installed.");
            }

            // Retrieve the updated list of power plans.
            string updatedPlanList = HelperRunner.Command("cmd.exe", "/c powercfg -list", redirect: true);

            // Inline logic to extract the GUID for the Ultimate Performance plan.
            string ultimatePlanGUID = string.Empty;
            foreach (string line in updatedPlanList.Split('\n'))
            {
                if (line.Contains("Ultimate Performance"))
                {
                    string[] parts = line.Split(' ');
                    foreach (string part in parts)
                    {
                        if (part.Contains("-"))
                        {
                            ultimatePlanGUID = part.Trim();
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(ultimatePlanGUID))
                    {
                        break;
                    }
                }
            }

            // Set the Ultimate Performance plan as the active power plan if its GUID was found.
            if (!string.IsNullOrEmpty(ultimatePlanGUID))
            {
                HelperRunner.Command("cmd.exe", $"/c powercfg -setactive {ultimatePlanGUID}");
                Logger.Log("Ultimate Performance plan is now active.");
            }
        }
    }
}
