using DebloaterTool.Helpers;
using DebloaterTool.Logging;
using DebloaterTool.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

// Created by @_giovannigiannone and ChatGPT
// Inspired from the Talon's Project!
namespace DebloaterTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("--generate-module-list"))
            {
                GenerateModuleList();
                Environment.Exit(0);
            }

            Internet.Inizialize();
            Updater.CheckForUpdates();
            Console.Title = $"{(Admins.IsAdministrator() ? "[Administrator]: " : "")}DebloaterTool {Global.Version}";
            DisplayWelcomeScreen();

            // Parse arguments
            bool skipEULA = args.Contains("--skipEULA");
            bool autoUAC = args.Contains("--autoUAC");
            bool autoRestart = args.Contains("--autoRestart");
            bool fullDebloat = args.Contains("--fullDebloat");
            string modulePath = args.FirstOrDefault(File.Exists);
            bool showHelp = args.Contains("--help");

            // Show help
            if (showHelp)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  [PATH]            Loads custom modules listed in the specified text file.");
                Console.WriteLine("  --skipEULA        Skips the EULA prompt - only in the console");
                Console.WriteLine("  --autoUAC         Automatically elevates privileges if needed.");
                Console.WriteLine("  --autoRestart     Automatically restarts the computer after debloating.");
                Console.WriteLine("  --fullDebloat     Performs a full debloat without selecting modules (all modules will be run).");
                Console.WriteLine("  --help            Displays this help message.");
                Environment.Exit(0);
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

            InitializeFolders();

            string result = null;
            var modules = ModuleList.GetAllModules().ToList();

            if (fullDebloat)
            {
                EULAConsole(skipEULA);
                RunFullModules(modules);
            }
            else if (!string.IsNullOrEmpty(modulePath))
            {
                EULAConsole(skipEULA);
                RunModulesFromFile(modulePath, modules);
            }
            else
            {
                result = StartWebInterface(modules);
            }

            string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string logFileName = $"DebloaterTool_{dateTime}.log";
            string destinationPath = Path.Combine(Global.logsPath, logFileName);

            Logger.Log($"Debloating complete!", Level.SUCCESS);
            Logger.Log($"Log file saved on {destinationPath}", Level.SUCCESS);
            Logger.Log($"[DebloaterTool by @_giovannigiannone]", Level.VERBOSE);

            File.Copy(Global.LogFilePath, destinationPath, overwrite: true);
            File.Delete(Global.LogFilePath);

            if (result.Contains("finished")) return;

            bool shouldRestart = autoRestart || result.Contains("enabledrestart")
                    || Display.RequestYesOrNo("Do you want to restart to apply changes?");

            if (shouldRestart)
            {
                string tempPath = Path.Combine(Global.debloatersPath, "DebloaterWelcome.vbs");
                File.WriteAllText(tempPath, Global.welcome.Replace("[INSTALLPATH]", Global.InstallPath), Encoding.Unicode);

                Process.Start("wscript.exe", $"\"{tempPath}\"")?.WaitForExit();
                Process.Start("shutdown.exe", "-r -t 0");
                Environment.Exit(0);
            }

            // Wait for user to press Enter
            Display.DisplayMessage("+---------------------------------------------------------------------------------------------+", ConsoleColor.DarkYellow);
            Display.DisplayMessage("|  Press ENTER to close this window. Thank you for using our debloater. - @_giovannigiannone  |", ConsoleColor.DarkYellow);
            Display.DisplayMessage("+---------------------------------------------------------------------------------------------+", ConsoleColor.DarkYellow);
            Console.ReadKey();

            // End
            return;
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

        static string StartWebInterface(List<TweakModule> modules)
        {
            var webServer = new SimpleWebServer("http://localhost:8080/", modules);
            Process.Start("http://localhost:8080/");
            return webServer.Start();
        }

        public class SimpleWebServer
        {
            private readonly HttpListener listener;
            private readonly string url;
            private readonly List<TweakModule> modules;

            public SimpleWebServer(string url, List<TweakModule> modules)
            {
                this.url = url;
                this.modules = modules;
                listener = new HttpListener();
                listener.Prefixes.Add(url);
            }

            public string Start()
            {
                listener.Start();
                Logger.Log("Server running: " + url);
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                while (true)
                {
                    var context = listener.GetContext();
                    var req = context.Request;
                    var resp = context.Response;

                    try
                    {
                        if (req.Url.AbsolutePath == "/")
                        {
                            Respond(resp, GetHtmlEULA(), "text/html");
                        }
                        else if(req.Url.AbsolutePath == "/settings")
                        {
                            Respond(resp, GetHtmlPage(), "text/html");
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
                            Respond(resp, "Finish - Restart Enabled", "text/plain");
                            return "enabledrestart";
                        }
                        else if (req.Url.AbsolutePath == "/finished")
                        {
                            string data = File.ReadAllText(Global.LogFilePath);
                            Respond(resp, $"Finished - Logs:\n{data}", "text/plain");
                            return "finished";
                        }
                        else if (req.Url.AbsolutePath == "/run")
                        {
                            var reader = new StreamReader(req.InputStream, req.ContentEncoding);
                            string body = reader.ReadToEnd();
                            List<string> selected = serializer.Deserialize<List<string>>(body);

                            Logger.Log("=== Running Modules ===");

                            foreach (string name in selected)
                            {
                                var module = modules.Find(m => $"{m.Action.Method.DeclaringType.Name}.{m.Action.Method.Name}" == name);
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

                            Logger.Log("=== Finished ===");

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

            private string GetHtmlPage()
            {
                return @"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<title>DebloaterTool</title>
<style>
body { margin:0; font-family:Arial,sans-serif; background:#121212; color:#e0e0e0; padding:20px; text-align:center;}
img#banner { max-width:100%; height:auto; border-radius:8px; margin-bottom:20px; }
h1 { font-weight:bold; margin-bottom:20px;}
#modules { width:100%; max-width:800px; margin:0 auto 20px auto; text-align:left;}
.module-card { background:#1e1e1e; padding:10px 15px; border-radius:8px; box-shadow:0 3px 10px rgba(0,0,0,0.5); margin-bottom:10px; display:block; cursor:pointer;}
.module-card:hover { background:#2a2a2a; }
input[type='checkbox'] { width:16px; height:16px; margin-right:8px; vertical-align:middle; }
button { background:#4caf50; color:#fff; font-weight:bold; border:none; border-radius:6px; padding:10px 20px; font-size:14px; cursor:pointer; margin:5px;}
button:hover { background:#45a049;}
#button-container { text-align:center; margin-bottom:15px; }

#loading-overlay {
    display:none;
    position:fixed;
    top:0; left:0;
    width:100%; height:100%;
    background: rgba(0,0,0,0.85);
    color:#fff;
    font-size:24px;
    font-weight:bold;
    justify-content:center;
    align-items:center;
    z-index:9999;
}

/* Restart popup */
#restart-popup {
    display:none;
    position:fixed;
    top:0; left:0;
    width:100%; height:100%;
    background: rgba(0,0,0,0.75);
    justify-content:center;
    align-items:center;
    z-index:10000;
}
#restart-box {
    background:#1e1e1e;
    padding:25px;
    border-radius:10px;
    text-align:center;
    width:300px;
    box-shadow:0 0 12px rgba(0,0,0,0.6);
}
#restart-box h2 { margin-top:0; color:#fff; }
.popup-btn {
    margin:10px;
    padding:10px 20px;
    cursor:pointer;
    font-weight:bold;
    border:none;
    border-radius:6px;
    font-size:14px;
}
#yesBtn { background:#4caf50; color:#fff; }
#noBtn { background:#b33a3a; color:#fff; }
</style>
</head>
<body>

<img id='banner' src='https://raw.githubusercontent.com/megsystem/megsystem/refs/heads/main/banner.png' alt='DebloaterTool Banner' />

<h1>DebloaterTool</h1>

<div id='button-container'>
    <button onclick='selectAll()'>Select All</button>
    <button onclick='deselectAll()'>Deselect All</button>
    <button onclick='runSelected()'>Run Selected Modules</button>
</div>

<div id='modules'></div>

<div id='loading-overlay'>Running modules...</div>

<!-- Restart popup -->
<div id='restart-popup'>
    <div id='restart-box'>
        <h2>Do you want to restart?</h2>
        <button class='popup-btn' id='yesBtn'>Yes</button>
        <button class='popup-btn' id='noBtn'>No</button>
    </div>
</div>

<script>
function ajaxGet(url, callback) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if(xhr.readyState==4 && xhr.status==200) callback(xhr.responseText);
    };
    xhr.open('GET', url,true);
    xhr.send();
}

function ajaxPost(url, data, callback) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange=function(){
        if(xhr.readyState==4) callback(xhr.responseText);
    };
    xhr.open('POST', url,true);
    xhr.setRequestHeader('Content-Type','application/json');
    xhr.send(data);
}

function loadModules(){
    ajaxGet('/modules', function(response){
        var modules = JSON.parse(response);
        var container=document.getElementById('modules');
        container.innerHTML='';
        for(var i=0;i<modules.length;i++){
            var m = modules[i];
            var label=document.createElement('label');
            label.className='module-card';
            var checkbox=document.createElement('input');
            checkbox.type='checkbox';
            checkbox.value=m.Name;
            if(m.Default) checkbox.checked=true;
            label.appendChild(checkbox);
            label.appendChild(document.createTextNode(m.Name+' - '+m.Description));
            container.appendChild(label);
        }
    });
}

function selectAll(){
    var checkboxes=document.getElementById('modules').getElementsByTagName('input');
    for(var i=0;i<checkboxes.length;i++){ checkboxes[i].checked=true; }
}

function deselectAll(){
    var checkboxes=document.getElementById('modules').getElementsByTagName('input');
    for(var i=0;i<checkboxes.length;i++){ checkboxes[i].checked=false; }
}

function runSelected(){
    var checkboxes=document.getElementById('modules').getElementsByTagName('input');
    var selected=[];
    for(var i=0;i<checkboxes.length;i++)
        if(checkboxes[i].checked) selected.push(checkboxes[i].value);

    // Show loading overlay
    document.getElementById('loading-overlay').style.display = 'flex';

    ajaxPost('/run', JSON.stringify(selected), function(response){

        // Hide loading overlay
        document.getElementById('loading-overlay').style.display = 'none';

        // Show popup overlay
        document.getElementById('restart-popup').style.display = 'flex';
    });
}

document.getElementById('yesBtn').onclick = function(){
    window.location.href = '/restart';
};
document.getElementById('noBtn').onclick = function(){
    window.location.href = '/finished';
};

loadModules();
</script>

</body>
</html>";
            }

            private string GetHtmlEULA()
            {
                return @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"" />
    <title>EULA</title>
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />

    <style>
        /* Reset */
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        /* IE-friendly centering using table-cell */
        html, body {
            height: 100%;
        }

        body {
            background-color: #1e1e1e;
            font-family: Arial, sans-serif;
            color: #ffffff;
            display: table;
            width: 100%;
        }

        .center-wrapper {
            display: table-cell;
            vertical-align: middle;
            text-align: center;
            padding: 20px;
        }

        .container {
            width: 450px;
            max-width: 90%;
            border: 2px solid #4a4a4a;
            border-radius: 5px;
            padding: 20px;
            background-color: #2c2c2c;
            margin: auto;
            text-align: center;
        }

        .banner {
            max-width: 100%;
            height: auto;
            margin-bottom: 20px;
        }

        h1 {
            font-size: 1.8rem;
            margin-bottom: 10px;
        }

        h2 {
            margin-top: 15px;
            font-size: 1.2rem;
        }

        p {
            margin: 8px 0;
        }

        /* Button */
        .btn-settings {
            background-color: #4caf50;
            border: none;
            padding: 12px 20px;
            color: white;
            cursor: pointer;
            font-size: 15px;
            font-weight: bold;
            border-radius: 5px;
            margin-top: 20px;
        }

        .btn-settings:hover {
            background-color: #45a049;
        }
    </style>
</head>

<body>
    <div class=""center-wrapper"">
        <div class=""container"">

            <img class=""banner""
                 src=""https://raw.githubusercontent.com/megsystem/megsystem/refs/heads/main/banner.png""
                 alt=""Page Banner"" />

            <h1>End User License Agreement (EULA) 📜</h1>
            <p>Last Updated: April 7, 2025</p>

            <h2>1. Introduction 👋</h2>
            <p>
                Thank you for choosing our software! By downloading, installing, or using this application,
                you agree to be bound by the terms of this End User License Agreement (""Agreement"").
                If you do not agree, please do not use the software. 🙅
            </p>

            <h2>2. License Grant 🔑</h2>
            <p>
                We grant you a limited, non-exclusive, non-transferable, revocable license to use the software
                for personal or commercial purposes, in accordance with this EULA.
            </p>

            <button class=""btn-settings"" onclick=""window.location.href='/settings'"">
                Continue to Settings →
            </button>

        </div>
    </div>
</body>
</html>";
            }
        }
    }
}