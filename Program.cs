﻿using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Net;
using System.Linq;
using System.IO;
using System.Text;

// Created by @_giovannigiannone and ChatGPT
// Inspired from the Talon's Project!
namespace DebloaterTool
{
    class Program
    {
        static void Main(string[] args)
        {
            // For .NET Framework 4.0, enable TLS 1.2 by casting its numeric value.
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            // Run the Welcome Screen and EULA
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.Title = $"{(IsAdministrator() ? "[Administrator]: " : "")}DebloaterTool V{version.Major}.{version.Minor}.{version.Build}";
            Console.WriteLine("+=================================================================================================================+");
            Console.WriteLine("|                                                                                                                 |");
            Console.WriteLine("|   ██████╗ ███████╗██████╗ ██╗     ██████╗  █████╗ ████████╗███████╗██████╗ ████████╗ ██████╗  ██████╗ ██╗       |");
            Console.WriteLine("|   ██╔══██╗██╔════╝██╔══██╗██║    ██╔═══██╗██╔══██╗╚══██╔══╝██╔════╝██╔══██╗╚══██╔══╝██╔═══██╗██╔═══██╗██║       |");
            Console.WriteLine("|   ██║  ██║█████╗  ██████╔╝██║    ██║   ██║███████║   ██║   █████╗  ██████╔╝   ██║   ██║   ██║██║   ██║██║       |");
            Console.WriteLine("|   ██║  ██║██╔══╝  ██╔══██╗██║    ██║   ██║██╔══██║   ██║   ██╔══╝  ██╔══██╗   ██║   ██║   ██║██║   ██║██║       |");
            Console.WriteLine("|   ██████╔╝███████╗██████╔╝██████╗╚██████╔╝██║  ██║   ██║   ███████╗██║  ██║   ██║   ╚██████╔╝╚██████╔╝██████╗   |");
            Console.WriteLine("|   ╚═════╝ ╚══════╝╚═════╝ ╚═════╝ ╚═════╝ ╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═╝   ╚═╝    ╚═════╝  ╚═════╝ ╚═════╝   |");
            Console.WriteLine("|                                                                                                                 |");
            Console.WriteLine("|   ██████╗ ██╗   ██╗     ███████╗ ██████╗██╗  ██╗ ██████╗ ██╗ ██████╗ ██╗   ██╗ █████╗ ███╗   ██╗███╗   ██╗██╗   |");
            Console.WriteLine("|   ██╔══██╗╚██╗ ██╔╝     ██╔════╝██╔════╝██║ ██╔╝██╔════╝ ██║██╔═══██╗██║   ██║██╔══██╗████╗  ██║████╗  ██║██║   |");
            Console.WriteLine("|   ██████╔╝ ╚████╔╝      █████╗  ██║     █████╔╝ ██║  ███╗██║██║   ██║██║   ██║███████║██╔██╗ ██║██╔██╗ ██║██║   |");
            Console.WriteLine("|   ██╔══██╗  ╚██╔╝       ██╔══╝  ██║     ██╔═██╗ ██║   ██║██║██║   ██║╚██╗ ██╔╝██╔══██║██║╚██╗██║██║╚██╗██║██║   |");
            Console.WriteLine("|   ██████╔╝   ██║        ██║     ╚██████╗██║  ██╗╚██████╔╝██║╚██████╔╝ ╚████╔╝ ██║  ██║██║ ╚████║██║ ╚████║██║   |");
            Console.WriteLine("|   ╚═════╝    ╚═╝        ╚═╝      ╚═════╝╚═╝  ╚═╝ ╚═════╝ ╚═╝ ╚═════╝   ╚═══╝  ╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═══╝╚═╝   |");
            Console.WriteLine("|                                                                                                                 |");
            Console.WriteLine("+=================================================================================================================+");
            Console.WriteLine("End User License Agreement (EULA)");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("By using this software, you agree to the following terms:");
            Console.WriteLine("1. You may not distribute this software without permission.");
            Console.WriteLine("2. The developers are not responsible for any damages.");
            HelperDisplay.DisplayMessage("3. Please disable your antivirus before proceeding.", ConsoleColor.DarkYellow);
            Console.WriteLine("---------------------------------");

            // Parse arguments to decide which elements to skip.
            bool skipEULA = args.Contains("--skipEULA");
            bool autoRestart = args.Contains("--autoRestart");

            // Optionally, you can also provide an argument for selecting the debloating mode.
            char choice = '\0';
            var modeArg = args.FirstOrDefault(a => a.StartsWith("--mode="));
            if (modeArg != null)
            {
                // Expecting something like --mode=A, --mode=M, --mode=C, or --mode=D
                var modeValue = modeArg.Split('=')[1].ToUpper();
                if (modeValue.Length > 0 && "AMCD".Contains(modeValue))
                {
                    choice = modeValue[0];
                    Console.WriteLine("Debloating mode selected via args: " + choice);
                }
            }

            // EULA Confirmation (skipped if --skipEULA is provided)
            if (!skipEULA)
            {
                if (!HelperDisplay.RequestYesOrNo("Do you accept the EULA?"))
                {
                    Logger.Log($"[DebloaterTool by @_giovannigiannone]", Level.VERBOSE);
                    Logger.Log("EULA declined!", Level.CRITICAL);
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            // Check if the program is runned with administrator rights!
            if (!IsAdministrator())
            {
                Logger.Log($"[DebloaterTool by @_giovannigiannone]", Level.VERBOSE);
                Logger.Log("Not runned as administrator!", Level.CRITICAL);

                if (HelperDisplay.RequestYesOrNo("Do you want to run as administrator?"))
                {
                    RestartAsAdmin();
                }
                else
                {
                    Environment.Exit(0);
                }
            }

            // Restart Confirmation (if --autoRestart is not provided, ask the user)
            bool restart = autoRestart || HelperDisplay.RequestYesOrNo("Do you want to restart after the process?");

            // If the mode wasn't set via arguments, ask the user interactively.
            if (choice != 'A' && choice != 'M' && choice != 'C' && choice != 'D')
            {
                do
                {
                    Console.WriteLine("Select the type of debloating:");
                    Console.WriteLine("[A] Complete - Removes all unnecessary apps and services.");
                    Console.WriteLine("[M] Minimal - Removes only bloatware while keeping essential apps.");
                    Console.WriteLine("[C] Custom - Choose what to remove manually.");
                    Console.WriteLine("[D] Debug - Enter the name of the module you want to run.");
                    Console.Write("Enter your choice (A/M/C/D): ");

                    choice = char.ToUpper(Console.ReadKey(true).KeyChar); // Read a single key, convert to uppercase
                    Console.WriteLine(choice); // Display the selected key

                    if (choice != 'A' && choice != 'M' && choice != 'C' && choice != 'D')
                    {
                        Console.WriteLine("Invalid choice. Please enter A, M, C, or D.");
                    }

                } while (choice != 'A' && choice != 'M' && choice != 'C' && choice != 'D');
            }

            // Execute based on selection
            switch (choice)
            {
                case 'A': // Complete
                    Console.WriteLine("Running Complete Debloating...");
                    Console.WriteLine("---------------------------------");
                    Logger.Log($"[DebloaterTool by @_giovannigiannone]", Level.VERBOSE);
                    WinDefender.Uninstall();
                    WinUpdate.DisableWindowsUpdateV1();
                    WinUpdate.DisableWindowsUpdateV2();
                    DebloaterTools.RunChrisTool();
                    DebloaterTools.RunRaphiTool();
                    RemoveUnnecessary.ApplyRegistryChanges();
                    RemoveUnnecessary.UninstallEdge();
                    RemoveUnnecessary.CleanOutlookAndOneDrive();
                    SecurityPerformance.DisableRemAssistAndRemDesk();
                    SecurityPerformance.DisableSpectreAndMeltdown();
                    SecurityPerformance.DisableWinErrorReporting();
                    SecurityPerformance.DisableTelAndDiagnost();
                    SecurityPerformance.DisableSMBv1();
                    WinStore.Uninstall();
                    WinCostumization.DisableSnapTools();
                    WinCostumization.EnableUltimatePerformance();
                    Ungoogled.UngoogledInstaller();
                    Ungoogled.ChangeUngoogledHomePage();
                    WindowsTheme.ExplorerTheme();
                    WindowsTheme.BorderTheme();
                    BootLogo.Install();
                    break;

                case 'M': // Minimal
                    Console.WriteLine("Running Minimal Debloating...");
                    Console.WriteLine("---------------------------------");
                    Logger.Log($"[DebloaterTool by @_giovannigiannone]", Level.VERBOSE);
                    WinUpdate.DisableWindowsUpdateV1();
                    DebloaterTools.RunChrisTool();
                    DebloaterTools.RunRaphiTool();
                    RemoveUnnecessary.ApplyRegistryChanges();
                    RemoveUnnecessary.UninstallEdge();
                    RemoveUnnecessary.CleanOutlookAndOneDrive();
                    SecurityPerformance.DisableRemAssistAndRemDesk();
                    SecurityPerformance.DisableWinErrorReporting();
                    SecurityPerformance.DisableTelAndDiagnost();
                    SecurityPerformance.DisableSMBv1();
                    WinCostumization.DisableSnapTools();
                    WinCostumization.EnableUltimatePerformance();
                    Ungoogled.UngoogledInstaller();
                    Ungoogled.ChangeUngoogledHomePage();
                    BootLogo.Install();
                    break;

                case 'C': // Custom
                    bool runDefender = HelperDisplay.RequestYesOrNo("Do you want to disable Windows Defender?");
                    bool runWindowsUpdate = HelperDisplay.RequestYesOrNo("Do you want to disable Windows Update?");
                    bool runWindowsStore = HelperDisplay.RequestYesOrNo("Do you want to remove Windows Store?");
                    bool runDebloater = HelperDisplay.RequestYesOrNo("Do you want to run Debloater Tools?");
                    bool runRemoveUnnecessary = HelperDisplay.RequestYesOrNo("Do you want to remove unnecessary components?");
                    bool runSecurityPerformance = HelperDisplay.RequestYesOrNo("Do you want to run Security Performance?");
                    bool runWinCostumization = HelperDisplay.RequestYesOrNo("Do you want to set Windows Costumization?");
                    bool runUngoogled = HelperDisplay.RequestYesOrNo("Do you want to install Ungoogled Chrome?");
                    bool runCustomTheme = HelperDisplay.RequestYesOrNo("Do you want to install Custom Theme (explorer and border)?");
                    bool runBootLogo = HelperDisplay.RequestYesOrNo("Do you want to install custom Boot Logo?");
                    Console.WriteLine("Running Custom Debloating...");
                    Console.WriteLine("---------------------------------");
                    Logger.Log($"[DebloaterTool by @_giovannigiannone]", Level.VERBOSE);

                    // Uninstall Windows Defender
                    if (runDefender) { WinDefender.Uninstall(); }

                    // Run WinUpdate
                    if (runWindowsUpdate)
                    {
                        WinUpdate.DisableWindowsUpdateV1();
                        WinUpdate.DisableWindowsUpdateV2();
                    }

                    // Run WinStore
                    if (runWindowsStore) { WinStore.Uninstall(); }

                    // Run DebloaterTools
                    if (runDebloater)
                    {
                        DebloaterTools.RunChrisTool();
                        DebloaterTools.RunRaphiTool();
                    }

                    // Run RemoveUnnecessary
                    if (runRemoveUnnecessary)
                    {
                        RemoveUnnecessary.ApplyRegistryChanges();
                        RemoveUnnecessary.UninstallEdge();
                        RemoveUnnecessary.CleanOutlookAndOneDrive();
                    }

                    // Run SecurityPerformance
                    if (runSecurityPerformance)
                    {
                        SecurityPerformance.DisableRemAssistAndRemDesk();
                        SecurityPerformance.DisableSpectreAndMeltdown();
                        SecurityPerformance.DisableWinErrorReporting();
                        SecurityPerformance.DisableTelAndDiagnost();
                        SecurityPerformance.DisableSMBv1();
                    }

                    // Run WinCostumization
                    if (runWinCostumization)
                    {
                        WinCostumization.DisableSnapTools();
                        WinCostumization.EnableUltimatePerformance();
                    }

                    // Run Ungoogled
                    if (runUngoogled)
                    {
                        Ungoogled.UngoogledInstaller();
                        Ungoogled.ChangeUngoogledHomePage();
                    }

                    // Run Custom Theme
                    if (runCustomTheme) 
                    { 
                        WindowsTheme.ExplorerTheme(); 
                        WindowsTheme.BorderTheme(); 
                    }

                    // Run BootLogo
                    if (runBootLogo) { BootLogo.Install(); }
                    break;

                case 'D': // Tropical
                    HelperModule.ListModule();
                    Console.Write("Enter method to execute (e.g. WinDefender.Uninstall): ");
                    string input = Console.ReadLine();
                    Console.WriteLine("Running DebugMode Debloating...");
                    Console.WriteLine("---------------------------------");
                    Logger.Log($"[DebloaterTool by @_giovannigiannone]", Level.VERBOSE);
                    HelperModule.RunModule(input);
                    break;
            }

            // Run Wallpaper
            Wallpaper.SetCustomWallpaper();

            // Process completed
            Logger.Log($"Debloating successfull successed (SUCC)", Level.SUCCESS);
            Logger.Log($"[DebloaterTool by @_giovannigiannone]", Level.VERBOSE);
            if (restart)
            {
                // Save Welcome Message to temporary file
                string tempPath = Path.Combine(Path.GetTempPath(), "DebloaterWelcome.vbs");
                File.WriteAllText(tempPath, Config.Resource.Welcome, Encoding.Unicode); // Save the script
                Process.Start("wscript.exe", "\"" + tempPath + "\""); // Run the script
                Process.Start("shutdown.exe", "-r -t 0"); // Restart the computer
            }
            else
            {
                Logger.Log("Restart skipped. Process completed. Press ENTER to close.", Level.ALERT);
                Console.ReadKey(); // Wait for user to press Enter
            }

            // End
            return;
        }

        static void RestartAsAdmin()
        {
            ProcessStartInfo proc = new ProcessStartInfo()
            {
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true,
                Verb = "runas"
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
        static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}