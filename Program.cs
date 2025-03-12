using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Net;

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
            Console.Title = $"DebloaterTool V{version.Major}.{version.Minor}.{version.Build}";
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

            // EULA Confirmation
            if (!RequestYesOrNo("Do you accept the EULA?"))
            {
                Console.WriteLine("EULA declined. Press ENTER to close.");
                Console.ReadKey();
                Environment.Exit(0);
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

            // Restart Confirmation
            bool restart = RequestYesOrNo("Do you want to restart after the process?");

            // Uninstall Windows Defender
            WinDefender.Uninstall();
            
            // Run DebloaterTools
            DebloaterTools.RunTweaks();
            DebloaterTools.RunWinConfig();

            // Run RemoveUnnecessary
            RemoveUnnecessary.ApplyRegistryChanges();
            RemoveUnnecessary.UninstallEdge();
            RemoveUnnecessary.CleanOutlookAndOneDrive();

            // Run WinUpdate
            WinUpdate.DisableWindowsUpdate();

            // Run Ungoogled
            Ungoogled.UngoogledInstaller();
            Ungoogled.ChangeUngoogledHomePage();

            // Run Wallpaper
            Wallpaper.SetCustomWallpaper();

            // Process completed
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