﻿using DebloaterTool.Logging;
using DebloaterTool.Settings;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

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
                string scriptUrl = Global.raphiToolUrl;
                string scriptPath = Path.Combine(Global.debloatersPath, "Win11Debloat.ps1");

                Logger.Log("Attempting to download Windows configuration script from: " + scriptUrl, Level.INFO);
                Logger.Log("Target script path: " + scriptPath, Level.INFO);

                using (WebClient client = new WebClient())
                {
                    // Synchronously download the script.
                    byte[] content = client.DownloadData(scriptUrl);
                    File.WriteAllBytes(scriptPath, content);
                }
                Logger.Log("Windows configuration script successfully saved to disk", Level.SUCCESS);

                // Build the PowerShell command string.
                string powershellCommand =
                    "Set-ExecutionPolicy Bypass -Scope Process -Force; " +
                    "& '" + scriptPath + $"' {Global.raphiToolArgs}";

                Logger.Log("Executing PowerShell command with parameters:", Level.INFO);
                Logger.Log("Command: " + powershellCommand, Level.INFO);

                ProcessStartInfo psi = new ProcessStartInfo("powershell", "-Command \"" + powershellCommand + "\"")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,  // Required for redirection.
                    CreateNoWindow = true       // We'll output to our console.
                };

                using (Process psProcess = new Process())
                {
                    psProcess.StartInfo = psi;

                    // Handle standard output asynchronously.
                    psProcess.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            // Now you can look in this exact copy of what you've been outputting.
                            Logger.Log(e.Data, Level.INFO);
                        }
                    };

                    // Handle standard error asynchronously.
                    psProcess.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Logger.Log(e.Data, Level.ERROR);
                        }
                    };

                    // Start the process and begin asynchronous read.
                    psProcess.Start();
                    psProcess.BeginOutputReadLine();
                    psProcess.BeginErrorReadLine();

                    // Wait until the process exits.
                    psProcess.WaitForExit();
                }
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
        static bool shouldExit = false; // Flag to track exit condition
        public static void RunChrisTool()
        {
            try
            {
                // Write JSON configuration to a temporary file.
                string jsonPath = Path.Combine(Global.debloatersPath, "christitus.json");
                File.WriteAllBytes(jsonPath, Global.christitusConfig);
                string logFile = Path.Combine(Global.debloatersPath, "cttwinutil.log");

                // Construct the PowerShell command.
                string command = "$ErrorActionPreference = 'SilentlyContinue'; " +
                                 $"iex \"& {{ $(irm {Global.christitusUrl}) }} -Config '" + jsonPath + "' -Run\" *>&1 | " +
                                 "Tee-Object -FilePath '" + logFile + "'";

                // Encode the command in Unicode and then to Base64.
                byte[] commandBytes = Encoding.Unicode.GetBytes(command);
                string encodedCommand = Convert.ToBase64String(commandBytes);

                // Set up the process to run PowerShell with the encoded command.
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = $"-NoProfile -NonInteractive -EncodedCommand {encodedCommand}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,  // Required for redirection.
                    CreateNoWindow = true       // We'll output to our console.
                };

                using (Process psProcess = new Process())
                {
                    psProcess.StartInfo = psi;

                    // Handle standard output asynchronously.
                    psProcess.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            // Now you can look in this exact copy of what you've been outputting.
                            Logger.Log(e.Data, Level.INFO);
                            if (e.Data.Contains("Tweaks are Finished"))
                            {
                                Logger.Log("DEBUG: This is the end!", Level.SUCCESS);
                                Logger.Log("Process will now exit gracefully.", Level.WARNING); 
                                shouldExit = true; // Set exit flag
                            }
                        }
                    };

                    // Handle standard error asynchronously.
                    psProcess.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Logger.Log(e.Data, Level.ERROR);
                        }
                    };

                    // Start the process and begin asynchronous read.
                    psProcess.Start();
                    psProcess.BeginOutputReadLine();
                    psProcess.BeginErrorReadLine();

                    while (!psProcess.HasExited)
                    {
                        if (shouldExit) // Check flag
                        {
                            Logger.Log("Stopping process safely...", Level.SUCCESS);

                            psProcess.CloseMainWindow(); // Try closing first
                            psProcess.WaitForExit(3000);

                            if (!psProcess.HasExited)
                            {
                                psProcess.Kill(); // Force kill if needed
                            }
                            return;
                        }

                        Thread.Sleep(500); // Prevent high CPU usage
                    }

                    // Wait until the process exits.
                    psProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message, Level.ERROR);
            }
        }
    }
}
