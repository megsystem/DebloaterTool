using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Net;
using System.Linq;

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
            DisplayMessage("3. Please disable your antivirus before proceeding.", ConsoleColor.DarkYellow);
            Console.WriteLine("---------------------------------");

            // Parse arguments to decide which elements to skip.
            bool skipEULA = args.Contains("--skipEULA");
            bool autoRestart = args.Contains("--autoRestart");

            // Optionally, you can also provide an argument for selecting the debloating mode.
            char choice = '\0';
            var modeArg = args.FirstOrDefault(a => a.StartsWith("--mode="));
            if (modeArg != null)
            {
                // Expecting something like --mode=A, --mode=M, or --mode=C
                var modeValue = modeArg.Split('=')[1].ToUpper();
                if (modeValue.Length > 0 && "AMC".Contains(modeValue))
                {
                    choice = modeValue[0];
                    Console.WriteLine("Debloating mode selected via args: " + choice);
                }
            }

            // EULA Confirmation (skipped if --skipEULA is provided)
            if (!skipEULA)
            {
                if (!RequestYesOrNo("Do you accept the EULA?"))
                {
                    Console.WriteLine("EULA declined. Press ENTER to close.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            // Check if the program is runned with administrator rights!
            if (!IsAdministrator())
            {
                DisplayMessage("This application must be run with administrator rights!", ConsoleColor.Red);

                if (RequestYesOrNo("Do you want to run as administrator?"))
                {
                    RestartAsAdmin();
                }
                else
                {
                    DisplayMessage("Please restart the program with administrator rights to continue!", ConsoleColor.Red);
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            // Restart Confirmation (if --autoRestart is not provided, ask the user)
            bool restart = autoRestart || RequestYesOrNo("Do you want to restart after the process?");

            // If the mode wasn't set via arguments, ask the user interactively.
            if (choice != 'A' && choice != 'M' && choice != 'C')
            {
                do
                {
                    Console.WriteLine("Select the type of debloating:");
                    Console.WriteLine("[A] Complete - Removes all unnecessary apps and services.");
                    Console.WriteLine("[M] Minimal - Removes only bloatware while keeping essential apps.");
                    Console.WriteLine("[C] Custom - Choose what to remove manually.");
                    Console.Write("Enter your choice (A/M/C): ");

                    choice = char.ToUpper(Console.ReadKey(true).KeyChar); // Read a single key, convert to uppercase
                    Console.WriteLine(choice); // Display the selected key

                    if (choice != 'A' && choice != 'M' && choice != 'C')
                    {
                        Console.WriteLine("Invalid choice. Please enter A, M, or C.");
                    }

                } while (choice != 'A' && choice != 'M' && choice != 'C');
            }

            // Execute based on selection
            switch (choice)
            {
                case 'A': // Complete
                    Console.WriteLine("Running Complete Debloating...");
                    Console.WriteLine("---------------------------------");
                    WinDefender.Uninstall();
                    WinUpdate.DisableWindowsUpdateV1();
                    WinUpdate.DisableWindowsUpdateV2();
                    DebloaterTools.RunChrisTool();
                    DebloaterTools.RunRaphiTool();
                    RemoveUnnecessary.ApplyRegistryChanges();
                    RemoveUnnecessary.UninstallEdge();
                    RemoveUnnecessary.CleanOutlookAndOneDrive();
                    WinStore.Uninstall();
                    WinCostumization.DisableSnapTools();
                    WinCostumization.EnableUltimatePerformance();
                    Ungoogled.UngoogledInstaller();
                    Ungoogled.ChangeUngoogledHomePage();
                    break;

                case 'M': // Minimal
                    Console.WriteLine("Running Minimal Debloating...");
                    Console.WriteLine("---------------------------------");
                    WinUpdate.DisableWindowsUpdateV1();
                    DebloaterTools.RunChrisTool();
                    DebloaterTools.RunRaphiTool();
                    RemoveUnnecessary.ApplyRegistryChanges();
                    RemoveUnnecessary.UninstallEdge();
                    RemoveUnnecessary.CleanOutlookAndOneDrive();
                    WinCostumization.DisableSnapTools();
                    WinCostumization.EnableUltimatePerformance();
                    Ungoogled.UngoogledInstaller();
                    Ungoogled.ChangeUngoogledHomePage();
                    break;

                case 'C': // Custom
                    bool runDefender = RequestYesOrNo("Do you want to disable Windows Defender?");
                    bool runWindowsUpdate = RequestYesOrNo("Do you want to disable Windows Update?");
                    bool runWindowsStore = RequestYesOrNo("Do you want to remove Windows Store?");
                    bool runDebloater = RequestYesOrNo("Do you want to run Debloater Tools?");
                    bool runRemoveUnnecessary = RequestYesOrNo("Do you want to remove unnecessary components?");
                    bool runWinCostumization = RequestYesOrNo("Do you want to set Windows Costumization?");
                    bool runUngoogled = RequestYesOrNo("Do you want to install Ungoogled Chrome?");
                    Console.WriteLine("Running Custom Debloating...");
                    Console.WriteLine("---------------------------------");

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
                    break;
            }

            // Run Wallpaper
            Wallpaper.SetCustomWallpaper();

            // Process completed
            Logger.Log($"Debloating successfull successed (SUCC)", Level.SUCCESS);
            Logger.Log($"[Debloater by @_giovannigiannone/@fckgiovanni]", Level.WARNING);
            if (restart)
            {
                Process.Start("shutdown.exe", "-r -t 0"); // Restart the computer
            }
            else
            {
                DisplayMessage("Restart skipped. Process completed. Press ENTER to close.", ConsoleColor.Green);
                Console.ReadLine(); // Wait for user to press Enter
            }

            // End
            return;
        }

        static bool RequestYesOrNo(string message)
        {
            while (true)
            {
                Console.Write($"{message} (yes/no): ");
                string response = Console.ReadLine()?.Trim().ToLower();

                if (response == "yes") return true;
                if (response == "no") return false;

                Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
            }
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

        static void DisplayMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}