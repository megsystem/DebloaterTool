using DebloaterTool.Helpers;
using DebloaterTool.Logging;
using DebloaterTool.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

// Created by @_giovannigiannone and ChatGPT
// Inspired from the Talon's Project!
namespace DebloaterTool
{
    class Program
    {
        // Import Win32 functions
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy,
            uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint GetConsoleProcessList(uint[] processList, uint processCount);

        static bool hideme = false;

        static void Main(string[] args)
        {
            bool climode = args.Contains("--cli");
            IntPtr handle = GetConsoleWindow();

            if (handle != IntPtr.Zero)
            {
                // Get the list of processes attached to this console
                uint[] processList = new uint[1];
                uint count = GetConsoleProcessList(processList, 1);

                // If count is 1, it means only THIS process is using the console.
                // This happens when you double-click the .exe (it creates a new console).
                // If count > 1, it means the console was already open (cmd, powershell, etc).
                if (count == 1 && !climode)
                {
                    ShowWindow(handle, SW_HIDE);
                    hideme = true;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Contains("--generate-module-list"))
            {
                GenerateModuleList();
                Environment.Exit(0);
            }

            ApiResponse result = new ApiResponse();
            Internet.Inizialize();
            Updater.CheckForUpdates();
            Console.Title = $"{(Admins.IsAdministrator() ? "[Administrator]: " : "")}DebloaterTool {Global.Version}";
            DisplayWelcomeScreen();

            // Parse arguments
            bool skipEULA = args.Contains("--skipEULA");
            bool autoUAC = args.Contains("--autoUAC");
            result.restart = args.Contains("--autoRestart");
            bool fullDebloat = args.Contains("--fullDebloat");
            string modulePath = args.FirstOrDefault(File.Exists);
            bool showHelp = args.Contains("--help");

            // Show help
            if (showHelp)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  [PATH]            Loads custom modules listed in the specified text file.");
                Console.WriteLine("  --skipEULA        Skips the EULA prompt - only in the console");
                Console.WriteLine("  --autoRestart     Automatically restarts the computer after debloating.");
                Console.WriteLine("  --fullDebloat     Performs a full debloat without selecting modules (all modules will be run).");
                Console.WriteLine("  --help            Displays this help message.");
                Environment.Exit(0);
            }

            // Check if the program is runned with administrator rights!
            if (!Admins.IsAdministrator())
            {
                Logger.Log("Not runned as administrator!", Level.CRITICAL);
                Logger.Log("Restarting application with administrator privileges.");

                if (!hideme)
                {
                    Array.Resize(ref args, args.Length + 1);
                    args[args.Length - 1] = "--cli";
                }

                Admins.RestartAsAdmin(args);
            }

            InitializeFolders();
            var modules = ModuleList.GetAllModules().ToList();

            if (fullDebloat)
            {
                result.status = -1; // set cli mode
                EULAConsole(skipEULA);
                RunFullModules(modules);
            }
            else if (!string.IsNullOrEmpty(modulePath))
            {
                result.status = -1; // set cli mode
                EULAConsole(skipEULA);
                RunModulesFromFile(modulePath, modules);
            }
            else
            {
                try
                {
                    result = StartWebInterface(modules, 8080);
                }
                catch
                {
                    result = StartWebInterface(modules, GetFreePort());
                }
            }

            if (result.status == 1) Environment.Exit(0);

            string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string logFileName = $"DebloaterTool_{dateTime}.log";
            string destinationPath = Path.Combine(Global.logsPath, logFileName);

            Logger.Log($"Debloating complete!", Level.SUCCESS);
            Logger.Log($"Log file saved on {destinationPath}", Level.SUCCESS);
            Logger.Log($"[DebloaterTool by @_giovannigiannone]", Level.VERBOSE);

            File.Copy(Global.LogFilePath, destinationPath, overwrite: true);
            File.Delete(Global.LogFilePath);

            string tempPath = Path.Combine(Global.debloatersPath, "DebloaterWelcome.vbs");
            File.WriteAllText(tempPath, Global.welcome.Replace("[INSTALLPATH]", Global.InstallPath), Encoding.Unicode);
            Process.Start("wscript.exe", $"\"{tempPath}\"")?.WaitForExit();

            bool shouldRestart =
                result.restart ||
                (result.status == -1 && Display.RequestYesOrNo("Do you want to restart to apply changes?"));

            if (shouldRestart)
            {
                Process.Start("shutdown.exe", "-r -t 0");
                Environment.Exit(0);
            }

            // Wait for user to press Enter
            Display.DisplayMessage("+---------------------------------------------------------------------------------------------+", ConsoleColor.DarkYellow);
            Display.DisplayMessage("|  Press ENTER to close this window. Thank you for using our debloater. - @_giovannigiannone  |", ConsoleColor.DarkYellow);
            Display.DisplayMessage("+---------------------------------------------------------------------------------------------+", ConsoleColor.DarkYellow);
            Console.ReadKey();

            // End
            Environment.Exit(0);
        }

        static int GetFreePort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        static void EULAConsole(bool skipEULA)
        {
            // EULA Confirmation
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
        }

        static void GenerateModuleList()
        {
            try
            {
                const string path = "ModuleList.txt";
                var modules = ModuleList.GetAllModules();
                var sb = new StringBuilder();

                sb.AppendLine("// ------------------------------------------------------------------------------");
                sb.AppendLine("//                                Module List File                               ");
                sb.AppendLine("//                  Automatically generated by the application.                  ");
                sb.AppendLine("//                 Uncomment the modules that you want to enable                 ");
                sb.AppendLine("//                         Project by @_giovannigiannone                         ");
                sb.AppendLine("// ------------------------------------------------------------------------------");

                foreach (var module in modules)
                {
                    sb.AppendLine($"// {module.Description} (default: {module.DefaultEnabled.ToString().ToLower()})");
                    sb.AppendLine($"// {module.Action.Method.DeclaringType?.Name}.{module.Action.Method.Name}");
                    sb.AppendLine("// ------------------------------------------------------------------------------");
                }

                File.WriteAllText(path, sb.ToString(), Encoding.Unicode);
                Console.WriteLine($"Module list successfully generated at '{Path.GetFullPath(path)}'.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error while generating module list: {ex.Message}");
                Environment.Exit(1);
            }
        }

        static void DisplayWelcomeScreen()
        {
            foreach (string line in Global.Logo)
                Display.DisplayMessage(line.CenterInConsole(), ConsoleColor.Magenta);

            Console.WriteLine();
            string[] welcomeLines = new string[]
            {
                "+=============================================================+",
                "|              End User License Agreement (EULA)              |",
                "+=============================================================+",
                "| By using this software, you agree to the following terms:   |",
                "| 1. This software is open source under the MIT License.      |",
                "| 2. You may not distribute modified versions without         |",
                "|    including the original license.                          |",
                "| 3. The developers are not responsible for any damages.      |",
                "| 4. Please disable your antivirus before proceeding.         |",
                "| 5. No warranty is provided; use at your own risk.           |",
                "| 6. Use --help to see all available automation options.      |",
                "| 7. Support at https://megsystem.github.io/DebloaterTool/    |",
                "+=============================================================+"
            };

            foreach (var line in welcomeLines)
            {
                ConsoleColor color = ConsoleColor.DarkCyan;
                if (line.Contains("4.")) color = ConsoleColor.DarkYellow;
                if (line.Contains("3.")) color = ConsoleColor.Red;
                if (line.Contains("6.")) color = ConsoleColor.DarkMagenta;

                Display.DisplayMessage(line.CenterInConsole(), color);
            }

            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------");
            Logger.Log($"Welcome to DebloaterTool Debug Console!", Level.INFO, Save: false);
            Console.WriteLine("--------------------------------------------------------------------------");
        }

        static void InitializeFolders()
        {
            Directory.CreateDirectory(Global.logsPath);
            Directory.CreateDirectory(Global.themePath);
            Directory.CreateDirectory(Global.bootlogoPath);
            Directory.CreateDirectory(Global.debloatersPath);
            Directory.CreateDirectory(Global.wallpapersPath);
        }

        static void RunFullModules(List<TweakModule> modules)
        {
            var allModuleNames = modules.ToDictionary(
                m => $"{m.Action.Method.DeclaringType.Name}.{m.Action.Method.Name}",
                m => m
            );

            // Pass all module names to the common runner
            RunSelectedModules(allModuleNames.Keys.ToList(), allModuleNames);
        }

        static void RunModulesFromFile(string modulePath, List<TweakModule> modules)
        {
            var allModuleNames = modules.ToDictionary(
                m => $"{m.Action.Method.DeclaringType.Name}.{m.Action.Method.Name}",
                m => m
            );

            var selectedModules = File.ReadAllLines(modulePath)
                                      .Where(l => !string.IsNullOrWhiteSpace(l) && !l.TrimStart().StartsWith("//"))
                                      .Select(l => l.Trim())
                                      .ToList();

            RunSelectedModules(selectedModules, allModuleNames);
        }

        // Private helper to execute selected modules
        private static void RunSelectedModules(List<string> selectedModules, Dictionary<string, TweakModule> allModules)
        {
            var skippedModules = allModules.Keys.Except(selectedModules).ToList();

            LogModuleSelection(selectedModules, skippedModules);

            foreach (var name in selectedModules)
            {
                if (allModules.TryGetValue(name, out var module))
                {
                    module.Action();
                }
            }
        }

        static void LogModuleSelection(List<string> selected, List<string> skipped)
        {
            Logger.Log("+=====================================+", Level.VERBOSE);
            Logger.Log("|     Running Custom Debloating...    |", Level.VERBOSE);
            Logger.Log("+=====================================+", Level.VERBOSE);
            Logger.Log("ENABLED MODULES:", Level.VERBOSE);
            selected.ForEach(m => Logger.Log($"[+] {m}", Level.VERBOSE));
            Logger.Log("SKIPPED MODULES:", Level.VERBOSE);
            skipped.ForEach(m => Logger.Log($"[-] {m}", Level.VERBOSE));
            Logger.Log("+=====================================+", Level.VERBOSE);
            Logger.Log("| DebloaterTool by @_giovannigiannone |", Level.VERBOSE);
            Logger.Log("+=====================================+", Level.VERBOSE);
        }

        static ApiResponse StartWebInterface(List<TweakModule> modules, int port)
        {
            var webServer = new ApplicationWebServer($"http://localhost:{port}/", modules);
            return webServer.Start();
        }

        public class ApiResponse
        {
            public int status { get; set; }
            public bool restart { get; set; }
            public static int percetage;
        }

        public class ApplicationWebServer
        {
            private readonly HttpListener listener;
            private readonly string url;
            private readonly List<TweakModule> modules;

            public ApplicationWebServer(string url, List<TweakModule> modules)
            {
                this.url = url;
                this.modules = modules;
                listener = new HttpListener();
                listener.Prefixes.Add(url);
            }

            public ApiResponse Start()
            {
                listener.Start();
                Logger.Log("Server running: " + url);
                Thread thread = new Thread(() =>
                {
                    var form = new WebBrowser(url);
                    Application.Run(form);
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ApiResponse responseObj = new ApiResponse();
                bool eula = true;

                while (true)
                {
                    var context = listener.GetContext();
                    var req = context.Request;
                    var resp = context.Response;

                    try
                    {
                        if (req.Url.AbsolutePath == "/")
                        {
                            if (eula)
                                Respond(resp, Properties.Resources.EULA, "text/html");
                            else
                                Respond(resp, Properties.Resources.SETTINGS, "text/html");
                        }
                        else if (req.Url.AbsolutePath == "/disableeula")
                        {
                            eula = false;
                        }
                        else if (req.Url.AbsolutePath == "/showcli")
                        {
                            var handle = GetConsoleWindow();
                            ShowWindow(handle, SW_SHOW);
                            hideme = false;
                        }
                        else if (req.Url.AbsolutePath == "/hidecli")
                        {
                            var handle = GetConsoleWindow();
                            ShowWindow(handle, SW_HIDE);
                            hideme = true;
                        }
                        else if (req.Url.AbsolutePath == "/modules")
                        {
                            var moduleList = modules.Select(m => new
                            {
                                Name = $"{m.Action.Method.DeclaringType.Name}.{m.Action.Method.Name}",
                                m.Description,
                                Default = m.DefaultEnabled
                            }).ToList();

                            Respond(resp, serializer.Serialize(moduleList), "application/json");
                        }
                        else if (req.Url.AbsolutePath == "/restart")
                        {
                            responseObj.status = 0;
                            responseObj.restart = true;
                            return responseObj;
                        }
                        else if (req.Url.AbsolutePath == "/log")
                        {
                            string data = File.ReadAllText(Global.LogFilePath);
                            Respond(resp, data, "text/plain");
                            responseObj.status = 0;
                            responseObj.restart = false;
                            return responseObj;
                        }
                        else if (req.Url.AbsolutePath == "/kill")
                        {
                            responseObj.status = 1;
                            return responseObj;
                        }
                        else if (req.Url.AbsolutePath == "/run")
                        {
                            var reader = new StreamReader(req.InputStream, req.ContentEncoding);
                            string body = reader.ReadToEnd();
                            List<string> selected = serializer.Deserialize<List<string>>(body);

                            Logger.Log("=== Running Modules ===");
                            IntPtr consoleHandle = GetConsoleWindow();
                            if (!hideme) SetWindowPos(consoleHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

                            ApiResponse.percetage = 0;
                            int total = selected.Count;
                            int current = 0;

                            foreach (string name in selected)
                            {
                                current++; // complete add +1
                                ApiResponse.percetage = (int)Math.Round(((double)current / total * 100) -1);
                                Logger.Log($"Progress: {ApiResponse.percetage}%");

                                var module = modules.Find(m =>
                                    $"{m.Action.Method.DeclaringType.Name}.{m.Action.Method.Name}" == name);

                                if (module != null)
                                {
                                    Logger.Log("[+] Running: " + name);
                                    try { module.Action(); }
                                    catch (Exception ex) { Logger.Log("Error: " + ex.Message, Level.ERROR); }
                                }
                                else
                                {
                                    Logger.Log("[-] Module not found: " + name);
                                }
                            }

                            ApiResponse.percetage = 100;
                            Logger.Log("=== Finished ===");
                            if (!hideme) SetWindowPos(consoleHandle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

                            // Risposta al browser
                            Respond(resp, serializer.Serialize(new { status = "done" }), "application/json");
                        }
                        else
                        {
                            resp.StatusCode = 404;
                            Respond(resp, "Not Found", "text/plain");
                        }
                    }
                    catch (Exception ex)
                    {
                        Respond(resp, "Error: " + ex.Message, "text/plain");
                    }
                    finally
                    {
                        resp.OutputStream.Close();
                    }
                }
            }

            private void Respond(HttpListenerResponse resp, string content, string contentType)
            {
                byte[] data = Encoding.UTF8.GetBytes(content);
                resp.ContentType = contentType;
                resp.ContentLength64 = data.Length;
                resp.OutputStream.Write(data, 0, data.Length);
            }
        }
    }
}