using DebloaterTool.Helpers;
using DebloaterTool.Settings;
using DebloaterTool.Logging;
using System;
using System.Diagnostics;
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
            // Generate modulelist.txt
            if (args.Contains("--generate-module-list"))
            {
                string path = "modulelist.txt";
                var modules = ModuleList.GetAllModules();
                var sb = new StringBuilder();

                sb.AppendLine("// ------------------------------------------------------------------------------");
                sb.AppendLine("// This is an example of the module file");
                sb.AppendLine("// Project by @_giovannigiannone");
                sb.AppendLine("// Uncomment the modules that you want to enable");
                sb.AppendLine("// ------------------------------------------------------------------------------");

                foreach (var module in modules)
                {
                    sb.AppendLine($"// {module.Description} (default: {module.DefaultEnabled.ToString().ToLower()})");
                    sb.AppendLine($"// {module.Action.Method.DeclaringType?.Name}.{module.Action.Method.Name}");
                    sb.AppendLine("// ------------------------------------------------------------------------------");
                }

                File.WriteAllText(path, sb.ToString(), Encoding.Unicode);
                Environment.Exit(0);
            }

            // Run the Welcome Screen and EULA
            Internet.Inizialize();
            Console.Title = $"{(Admins.IsAdministrator() ? "[Administrator]: " : "")}DebloaterTool {Global.Version}";
            foreach (string line in Global.Logo) Display.DisplayMessage(line.CenterInConsole(), ConsoleColor.Magenta);
            Console.WriteLine();
            Display.DisplayMessage("+=============================================================+".CenterInConsole(), ConsoleColor.DarkCyan);
            Display.DisplayMessage("|              End User License Agreement (EULA)              |".CenterInConsole(), ConsoleColor.DarkCyan);
            Display.DisplayMessage("+=============================================================+".CenterInConsole(), ConsoleColor.DarkCyan);
            Display.DisplayMessage("| By using this software, you agree to the following terms:   |".CenterInConsole(), ConsoleColor.DarkCyan);
            Display.DisplayMessage("| 1. This software is open source under the MIT License.      |".CenterInConsole(), ConsoleColor.DarkCyan);
            Display.DisplayMessage("| 2. You may not distribute modified versions without         |".CenterInConsole(), ConsoleColor.DarkCyan);
            Display.DisplayMessage("|    including the original license.                          |".CenterInConsole(), ConsoleColor.DarkCyan);
            Display.DisplayMessage("| 3. The developers are not responsible for any damages.      |".CenterInConsole(), ConsoleColor.Red);
            Display.DisplayMessage("| 4. Please disable your antivirus before proceeding.         |".CenterInConsole(), ConsoleColor.DarkYellow);
            Display.DisplayMessage("| 5. No warranty is provided; use at your own risk.           |".CenterInConsole(), ConsoleColor.DarkCyan);
            Display.DisplayMessage("| 6. Use --help to see all available automation options.      |".CenterInConsole(), ConsoleColor.DarkMagenta);
            Display.DisplayMessage("| 7. Support at https://megsystem.github.io/DebloaterTool/    |".CenterInConsole(), ConsoleColor.DarkCyan);
            Display.DisplayMessage("+=============================================================+".CenterInConsole(), ConsoleColor.DarkCyan);
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------");
            Logger.Log($"Welcome to DebloaterTool Debug Console!", Level.INFO, Save: false);
            Console.WriteLine("--------------------------------------------------------------------------");

            // Parse arguments
            bool skipEULA = args.Contains("--skipEULA");
            bool noURLOpen = args.Contains("--noURLOpen");
            bool autoUAC = args.Contains("--autoUAC");
            bool showHelp = args.Contains("--help");
            var modeArg = args.FirstOrDefault(a => a.StartsWith("--mode="));
            var restartArg = args.FirstOrDefault(a => a.StartsWith("--restart="));
            string modulepath = null;

            // args is your command-line arguments array
            foreach (string arg in args)
            {
                if (File.Exists(arg))
                {
                    modulepath = arg;
                    break;
                }
            }

            // Show help
            if (showHelp)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  --skipEULA        Skips the EULA prompt.");
                Console.WriteLine("  --restart=[Y/N]   Automatically restarts the computer after debloating.");
                Console.WriteLine("  --noURLOpen       Prevents URLs from being opened.");
                Console.WriteLine("  --autoUAC         Automatically elevates privileges if needed.");
                Console.WriteLine("  --mode=[A|M|C]    Sets the installation mode: A (Complete), M (Minimal), C (Custom).");
                Console.WriteLine("  [PATH]            Loads modules listed in the specified text file.");
                Console.WriteLine("  --help            Displays this help message.");
                Environment.Exit(0);
            }

            // Run the debloater github page
            if (!noURLOpen) Process.Start("https://megsystem.github.io/DebloaterTool/");

            // Optionally, you can also provide an argument for selecting the debloating mode.
            char choice = '\0';
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

            // Fallback only if modeArg is null or invalid
            if (choice == '\0' && !string.IsNullOrEmpty(modulepath))
            {
                choice = 'C';
            }

            // EULA Confirmation (skipped if --skipEULA is provided)
            if (!skipEULA)
            {
                if (!Display.RequestYesOrNo("Do you accept the EULA?"))
                {
                    Logger.Log("EULA declined!", Level.CRITICAL);
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                Logger.Log("EULA Accepted!", Level.SUCCESS);
            }

            // Check if the program is runned with administrator rights!
            if (!Admins.IsAdministrator())
            {
                Logger.Log("Not runned as administrator!", Level.CRITICAL);

                // Ask only in interactive mode; quit if they decline.
                if (!autoUAC && !Display.RequestYesOrNo("Do you want to run as administrator?"))
                {
                    Logger.Log("User declined elevation in interactive mode. Exiting application.");
                    Environment.Exit(0);
                }

                // At this point we’re either in silent mode or the user said “yes”
                Logger.Log("Restarting application with administrator privileges.");
                Admins.RestartAsAdmin(args);
            }

            bool restart;
            if (!string.IsNullOrWhiteSpace(restartArg) && restartArg.Contains("="))
            {
                var restartValue = restartArg.Split('=')[1].ToUpper();
                restart = restartValue == "Y"
                    ? true
                    : restartValue == "N"
                        ? false
                        : Display.RequestYesOrNo("Do you want to restart after the process?");
            }
            else
            {
                restart = Display.RequestYesOrNo("Do you want to restart after the process?");
            }

            // If the mode wasn't set via arguments, ask the user interactively.
            if (choice != 'A' && choice != 'M' && choice != 'C')
            {
                do
                {
                    Console.WriteLine("Select the type of debloating:");
                    Console.WriteLine("[A] Complete - Removes all unnecessary apps and services.");
                    Console.WriteLine("[M] Minimal  - Removes only bloatware while keeping essential apps.");
                    Console.WriteLine("[C] Custom   - Choose what to remove manually.");
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
            Directory.CreateDirectory(Global.logsPath);
            Directory.CreateDirectory(Global.themePath);
            Directory.CreateDirectory(Global.bootlogoPath);
            Directory.CreateDirectory(Global.debloatersPath);
            Directory.CreateDirectory(Global.wallpapersPath);

            // Execute based on selection
            switch (choice)
            {
                case 'A': // Complete
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("|    Running Complete Debloating...   |", Level.VERBOSE);
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("| DebloaterTool by @_giovannigiannone |", Level.VERBOSE);
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    foreach (var tweaks in ModuleList.GetAllModules())
                    {
                        tweaks.Action();
                    }
                    break;

                case 'M': // Minimal
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("|    Running Minimal Debloating...    |", Level.VERBOSE);
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("| DebloaterTool by @_giovannigiannone |", Level.VERBOSE);
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    foreach (var tweaks in ModuleList.GetAllModules())
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
                    foreach (var tweaks in ModuleList.GetAllModules())
                    {
                        var method = tweaks.Action.Method;
                        var fullName = $"{method.DeclaringType.Name}.{method.Name}";
                        allModules[fullName] = tweaks;
                    }

                    // Check module args
                    if (!string.IsNullOrEmpty(modulepath))
                    {
                        // Read all lines from the file
                        string[] lines = File.ReadAllLines(modulepath);

                        // Add each line to the list
                        foreach (string line in lines)
                        {
                            if (!string.IsNullOrWhiteSpace(line) && !line.TrimStart().StartsWith("//"))
                            {
                                selectedModules.Add(line.Trim());
                            }
                        }

                        // Skipped modules
                        foreach (var module in allModules.Keys)
                        {
                            if (!selectedModules.Contains(module))
                            {
                                skippedModules.Add(module);
                            }
                        }
                    }
                    
                    // If the module list is empty
                    if (!selectedModules.Any())
                    {
                        // Prompt user with description
                        foreach (var module in allModules)
                        {
                            string prompt =
                                $"Description: {module.Value.Description} " +
                                $"[DEFAULT ENABLED: {module.Value.DefaultEnabled.ToString().ToUpper()}]" +
                                $"\nDo you want to run {module.Key}?";
                            if (Display.RequestYesOrNo(prompt))
                                selectedModules.Add(module.Key);
                            else
                                skippedModules.Add(module.Key);
                        }
                    }

                    // Logging
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("|     Running Custom Debloating...    |", Level.VERBOSE);
                    Logger.Log("+=====================================+", Level.VERBOSE);
                    Logger.Log("ENABLED MODULES:", Level.VERBOSE);
                    selectedModules.ForEach(m => Logger.Log($"[+] {m}", Level.VERBOSE));
                    Logger.Log("SKIPPED MODULES:", Level.VERBOSE);
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

            // Save log
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string logFileName = $"DebloaterTool_{dateTime}.log";
            string destinationPath = Path.Combine(Global.logsPath, logFileName);
            Logger.Log($"Debloating complete!", Level.SUCCESS);
            Logger.Log($"Log file saved on {destinationPath}", Level.SUCCESS);
            Logger.Log($"[DebloaterTool by @_giovannigiannone]", Level.VERBOSE);
            File.Copy(Global.LogFilePath, destinationPath);
            File.Delete(Global.LogFilePath);

            // Delete empty folder in the RootFolder
            DeleteEmptyFolders(Global.InstallPath);

            // Restart
            if (restart)
            {
                // Save Welcome Message to temporary file
                string tempPath = Path.Combine(Global.debloatersPath, "DebloaterWelcome.vbs");
                string dataWelcome = Global.welcome.Replace("[INSTALLPATH]", Global.InstallPath);
                File.WriteAllText(tempPath, dataWelcome, Encoding.Unicode); // Save the script
                Process.Start("wscript.exe", $"\"{tempPath}\"")?.WaitForExit(); // Run the script
                Process.Start("shutdown.exe", "-r -t 0"); // Restart the computer
            }

            // Wait for user to press Enter
            Display.DisplayMessage("+---------------------------------------------------------------------------------------------+", ConsoleColor.DarkYellow);
            Display.DisplayMessage("|  Press ENTER to close this window. Thank you for using our debloater. - @_giovannigiannone  |", ConsoleColor.DarkYellow);
            Display.DisplayMessage("+---------------------------------------------------------------------------------------------+", ConsoleColor.DarkYellow);
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
    }
}