using DebloaterTool.Helpers;
using DebloaterTool.Logging;
using DebloaterTool.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace DebloaterTool.Modules
{
    internal class DebloaterTools
    {
        /// <summary>
        /// Downloads a PowerShell script from a URL, saves it to 
        /// the temp directory, runs it with specific parameters.
        /// </summary>
        public static void RunRaphiTool()
        {
            Logger.Log("Starting Windows configuration process...", Level.WARNING);
            try
            {
                // Write JSON configuration to a temporary file.
                string jsonPath = Path.Combine(Global.debloatersPath, "config.json");
                File.WriteAllBytes(jsonPath, Global.config);
                string sectionName = "raphiTool";
                string outputPath = Path.Combine(Global.debloatersPath, $"{sectionName}.json");
                string combinedArgs = ProcessSection(jsonPath, sectionName, outputPath);

                // Install
                string scriptUrl = Global.raphiToolUrl;
                string scriptPath = Path.Combine(Global.debloatersPath, "Win11Debloat.ps1");
                Logger.Log("Attempting to download Windows configuration script from: " + scriptUrl, Level.INFO);
                Logger.Log("Target script path: " + scriptPath, Level.INFO);
                if (!Internet.DownloadFile(scriptUrl, scriptPath)) return;
                Logger.Log("Windows configuration script successfully saved to disk", Level.SUCCESS);

                // Build the PowerShell command string.
                string powershellCommand =
                    "Set-ExecutionPolicy Bypass -Scope Process -Force; " +
                    "& '" + scriptPath + $"' {combinedArgs}";

                Logger.Log("Executing PowerShell command with parameters:", Level.INFO);
                Logger.Log("Command: " + powershellCommand, Level.INFO);
                Runner.Command("powershell", "-Command \"" + powershellCommand + "\"", redirectOutputLogger: true);
            }
            catch (Exception e)
            {
                Logger.Log("Unexpected error during Windows configuration: " + e.Message, Level.ERROR);
            }
        }

        /// <summary>
        /// Runs a PowerShell command that processes the config.
        /// It monitors the process output for a completion message.
        /// </summary>
        public static void RunChrisTool()
        {
            try
            {
                // Write JSON configuration to a temporary file.
                string jsonPath = Path.Combine(Global.debloatersPath, "config.json");
                File.WriteAllBytes(jsonPath, Global.config);
                string sectionName = "ChrisUtils";
                string outputPath = Path.Combine(Global.debloatersPath, $"{sectionName}.json");
                ProcessSection(jsonPath, sectionName, outputPath);

                // Install
                string scriptUrl = Global.christitusUrl;
                string scriptPath = Path.Combine(Global.debloatersPath, "ChrisTool.ps1");
                Logger.Log("Attempting to download Windows configuration script from: " + scriptUrl, Level.INFO);
                Logger.Log("Target script path: " + scriptPath, Level.INFO);
                if (!Internet.DownloadFile(scriptUrl, scriptPath)) return;
                Logger.Log("Windows configuration script successfully saved to disk", Level.SUCCESS);

                // Build the PowerShell command string.
                string powershellCommand =
                    "Set-ExecutionPolicy Bypass -Scope Process -Force; " +
                    "& '" + scriptPath + $"' -Config '{outputPath}' -Run -NoUI";

                Logger.Log("Executing PowerShell command with parameters:", Level.INFO);
                Logger.Log("Command: " + powershellCommand, Level.INFO);
                Runner.Command("powershell", "-Command \"" + powershellCommand + "\"", 
                    redirectOutputLogger: true, customExitCheck: "Tweaks are Finished");
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message, Level.ERROR);
            }
        }

        /// <summary>
        /// Reads a section from JSON, converts arrays to List<string>, builds combined string of all array values, and writes to file.
        /// </summary>
        static string ProcessSection(string jsonPath, string sectionName, string outputPath)
        {
            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(jsonPath));

            if (!config.ContainsKey(sectionName))
                throw new Exception($"Section '{sectionName}' not found in JSON.");

            var section = (Dictionary<string, object>)config[sectionName];
            var outputSection = new Dictionary<string, object>();

            // Collect all array values for combined args string
            var allArgsList = new List<string>();

            foreach (var kvp in section)
            {
                if (kvp.Value is ArrayList arrayList)
                {
                    // Convert ArrayList to List<string>
                    var list = arrayList.Cast<object>().Select(x => x.ToString()).ToList();
                    outputSection[kvp.Key] = list;

                    // Add all array values to combined args string
                    allArgsList.AddRange(list);
                }
                else
                {
                    outputSection[kvp.Key] = kvp.Value;
                }
            }

            // Serialize section to JSON file
            string outputJson = serializer.Serialize(outputSection);
            File.WriteAllText(outputPath, outputJson);

            // Return a single string of all values in all arrays
            return string.Join(" ", allArgsList);
        }
    }
}
