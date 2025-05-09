﻿using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Net;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;

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
            Console.Title = $"{(IsAdministrator() ? "[Administrator]: " : "")}DebloaterTool {Settings.Version}";
            foreach (string line in Settings.Logo) HelperDisplay.DisplayMessage(line.CenterInConsole(), ConsoleColor.Magenta);
            Console.WriteLine();
            HelperDisplay.DisplayMessage("+=============================================================+".CenterInConsole(), ConsoleColor.DarkCyan);
            HelperDisplay.DisplayMessage("|              End User License Agreement (EULA)              |".CenterInConsole(), ConsoleColor.DarkCyan);
            HelperDisplay.DisplayMessage("+=============================================================+".CenterInConsole(), ConsoleColor.DarkCyan);
            HelperDisplay.DisplayMessage("| By using this software, you agree to the following terms:   |".CenterInConsole(), ConsoleColor.DarkCyan);
            HelperDisplay.DisplayMessage("| 1. This software is open source under the MIT License.      |".CenterInConsole(), ConsoleColor.DarkCyan);
            HelperDisplay.DisplayMessage("| 2. You may not distribute modified versions without         |".CenterInConsole(), ConsoleColor.DarkCyan);
            HelperDisplay.DisplayMessage("|    including the original license.                          |".CenterInConsole(), ConsoleColor.DarkCyan);
            HelperDisplay.DisplayMessage("| 3. The developers are not responsible for any damages.      |".CenterInConsole(), ConsoleColor.Red);
            HelperDisplay.DisplayMessage("| 4. Please disable your antivirus before proceeding.         |".CenterInConsole(), ConsoleColor.DarkYellow);
            HelperDisplay.DisplayMessage("| 5. No warranty is provided; use at your own risk.           |".CenterInConsole(), ConsoleColor.DarkCyan);
            HelperDisplay.DisplayMessage("| 6. Support at https://megsystem.github.io/DebloaterTool/    |".CenterInConsole(), ConsoleColor.DarkCyan);
            HelperDisplay.DisplayMessage("+=============================================================+".CenterInConsole(), ConsoleColor.DarkCyan);
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------");
            Logger.Log($"Welcome to DebloaterTool Debug Console!", Level.INFO, Save: false);
            Console.WriteLine("--------------------------------------------------------------------------");

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
                    Logger.Log("EULA declined!", Level.CRITICAL);
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                Logger.Log("EULA Accepted!", Level.SUCCESS);
            }

            // Check if the program is runned with administrator rights!
            if (!IsAdministrator())
            {
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

            // Configure Folders
            Directory.CreateDirectory(Settings.logsPath);
            Directory.CreateDirectory(Settings.themePath);
            Directory.CreateDirectory(Settings.bootlogoPath);
            Directory.CreateDirectory(Settings.debloatersPath);
            Directory.CreateDirectory(Settings.wallpapersPath);

            // Execute based on selection
            switch (choice)
            {
                case 'A': // Complete
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("|    Running Complete Debloating...   |", Level.VERBOSE);
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("| DebloaterTool by @_giovannigiannone |", Level.VERBOSE);
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    foreach (var tweaks in ModuleRegistry.GetAllModules())
                    {
                        tweaks.Action();
                    }
                    break;

                case 'M': // Minimal
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("|    Running Minimal Debloating.. .   |", Level.VERBOSE);
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("| DebloaterTool by @_giovannigiannone |", Level.VERBOSE);
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    foreach (var tweaks in ModuleRegistry.GetAllModules())
                    {
                        if (!tweaks.DefaultEnabled) continue; // Skip if not enabled
                        tweaks.Action(); // Run only enabled tweaks
                    }
                    break;

                case 'C': // Custom
                    var allModules = new Dictionary<string, TweakModule>();
                    var selectedModules = new List<string>();
                    var skippedModules = new List<string>();

                    // Build dictionary with method names and corresponding TweakModule
                    foreach (var tweaks in ModuleRegistry.GetAllModules())
                    {
                        var method = tweaks.Action.Method;
                        var fullName = $"{method.DeclaringType.Name}.{method.Name}";
                        allModules[fullName] = tweaks;
                    }

                    // Prompt user with description
                    foreach (var module in allModules)
                    {
                        string prompt = 
                            $"Description: {module.Value.Description} " +
                            $"[DEFAULT ENABLED: {module.Value.DefaultEnabled.ToString().ToUpper()}]" +
                            $"\nDo you want to run {module.Key}?";
                        if (HelperDisplay.RequestYesOrNo(prompt))
                            selectedModules.Add(module.Key);
                        else
                            skippedModules.Add(module.Key);
                    }

                    // Logging
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("|   Running Custom Debloating...      |", Level.VERBOSE);
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("ENABLED Modules:", Level.VERBOSE);
                    selectedModules.ForEach(m => Logger.Log($"[+] {m}", Level.VERBOSE));
                    Logger.Log("SKIPPED Modules:", Level.VERBOSE);
                    skippedModules.ForEach(m => Logger.Log($"[-] {m}", Level.VERBOSE));
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("| DebloaterTool by @_giovannigiannone |", Level.VERBOSE);
                    Logger.Log("+=====================================+", Level.VERBOSE);

                    // Execute selected modules
                    foreach (var name in selectedModules)
                    {
                        if (allModules.TryGetValue(name, out var module))
                            module.Action();
                    }
                    break;
            }

            // Run Wallpaper
            Wallpaper.SetCustomWallpaper();

            // Save log
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string logFileName = $"DebloaterTool_{dateTime}.log";
            string destinationPath = Path.Combine(Settings.logsPath, logFileName);
            Logger.Log($"Debloating complete!", Level.SUCCESS);
            Logger.Log($"Log file saved on {destinationPath}", Level.SUCCESS);
            Logger.Log($"[DebloaterTool by @_giovannigiannone]", Level.VERBOSE);
            File.Copy(Settings.LogFilePath, destinationPath);
            File.Delete(Settings.LogFilePath);

            // Delete empty folder in the RootFolder
            DeleteEmptyFolders(Settings.InstallPath);

            // Restart
            if (restart)
            {
                // Save Welcome Message to temporary file
                string tempPath = Path.Combine(Settings.debloatersPath, "DebloaterWelcome.vbs");
                string dataWelcome = Config.Resource.Welcome.Replace("[INSTALLPATH]", Settings.InstallPath);
                File.WriteAllText(tempPath, dataWelcome, Encoding.Unicode); // Save the script
                Process.Start("wscript.exe", $"\"{tempPath}\"")?.WaitForExit(); // Run the script
                Process.Start("shutdown.exe", "-r -t 0"); // Restart the computer
            }

            // Wait for user to press Enter
            HelperDisplay.DisplayMessage("+---------------------------------------------------------------------------------------------+", ConsoleColor.DarkYellow);
            HelperDisplay.DisplayMessage("|  Press ENTER to close this window. Thank you for using our debloater. - @_giovannigiannone  |", ConsoleColor.DarkYellow);
            HelperDisplay.DisplayMessage("+---------------------------------------------------------------------------------------------+", ConsoleColor.DarkYellow);
            Console.ReadKey(); 

            // End
            return;
        }

        static void DeleteEmptyFolders(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                // Recursively delete empty subfolders
                DeleteEmptyFolders(directory);

                // If the directory is now empty, delete it
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory);
                }
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
    }
}