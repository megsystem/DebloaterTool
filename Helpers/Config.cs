using DebloaterTool.Logging;
using DebloaterTool.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Script.Serialization;

namespace DebloaterTool.Helpers
{
    internal class Config
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void InizializeConfig()
        {
            string configPath = Global.configFilePath;

            if (File.Exists(configPath))
            {
                Logger.Log("Config file already exists. Skipping creation.");
                return;
            }

            string defaultConfig = @"{
  ""DEBUG"": ""false"",
  ""DIAGNOSTIC"": ""false""
}";

            File.WriteAllText(configPath, defaultConfig, new UTF8Encoding(false));

            Logger.Log("Config file created.", Level.SUCCESS);
        }

        public static void CheckConfig()
        {
            if (!File.Exists(Global.configFilePath)) return;

            string json = File.ReadAllText(Global.configFilePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                Logger.Log("Config file is empty!", Level.ERROR);
                return;
            }

            try
            {
                var serializer = new JavaScriptSerializer();
                var dict = serializer.Deserialize<Dictionary<string, object>>(json);

                if (Convert.ToBoolean(dict["DEBUG"]))
                {
                    Logger.Log("DEBUG IS TRUE");
                    IntPtr handle = GetConsoleWindow();
                    ShowWindow(handle, SW_SHOW);
                }

                if (Convert.ToBoolean(dict["DIAGNOSTIC"]))
                {
                    Logger.Log("DIAGNOSTIC IS TRUE");
                    Diagnostic.enabled = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error parsing JSON: " + ex.Message, Level.ERROR);
            }
        }

        public static void UpdateConfigValue(string name, bool value)
        {
            if (!File.Exists(Global.configFilePath))
            {
                Logger.Log("Config file not found.", Level.ERROR);
                return;
            }

            try
            {
                string json = File.ReadAllText(Global.configFilePath);

                var serializer = new JavaScriptSerializer();
                var dict = serializer.Deserialize<Dictionary<string, object>>(json);

                if (!dict.ContainsKey(name))
                {
                    Logger.Log($"Config key '{name}' not found.", Level.ERROR);
                    return;
                }

                dict[name] = value;

                string updatedJson = serializer.Serialize(dict);
                File.WriteAllText(Global.configFilePath, updatedJson, new UTF8Encoding(false));

                Logger.Log($"Config '{name}' updated to {value}.", Level.SUCCESS);
            }
            catch (Exception ex)
            {
                Logger.Log("Error updating config: " + ex.Message, Level.ERROR);
            }
        }

        public static bool GetConfigValue(string name, bool defaultValue = false)
        {
            if (!File.Exists(Global.configFilePath))
            {
                Logger.Log("Config file not found.", Level.ERROR);
                return defaultValue;
            }

            try
            {
                string json = File.ReadAllText(Global.configFilePath);

                var serializer = new JavaScriptSerializer();
                var dict = serializer.Deserialize<Dictionary<string, object>>(json);

                if (!dict.ContainsKey(name))
                {
                    Logger.Log($"Config key '{name}' not found. Using default.", Level.WARNING);
                    return defaultValue;
                }

                return Convert.ToBoolean(dict[name]);
            }
            catch (Exception ex)
            {
                Logger.Log("Error reading config: " + ex.Message, Level.ERROR);
                return defaultValue;
            }
        }
    }
}
