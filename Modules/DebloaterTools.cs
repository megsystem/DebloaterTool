using DebloaterTool.Helpers;
using DebloaterTool.Logging;
using DebloaterTool.Settings;
using System;
using System.IO;
using System.Text;

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
                    "& '" + scriptPath + $"' {Global.raphiToolArgs}";

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
                string jsonPath = Path.Combine(Global.debloatersPath, "christitus.json");
                File.WriteAllBytes(jsonPath, Global.christitusConfig);

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
                    "& '" + scriptPath + $"' -Config '{jsonPath}' -Run -NoUI";

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
    }
}
