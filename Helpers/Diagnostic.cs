using Microsoft.Win32;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace DebloaterTool.Helpers
{
    internal class Diagnostic
    {
        public static bool enabled = false;
        private static string webhook = "https://discord.com/api/webhooks/1469759095312748758/kgh8jU74MhKtoaMlFADwRZrY_905EbShY22j9D3wx9TeCIW0Nnpf3euDo4IHw0p6zcXa";

        public static void Send(string content)
        {
            if (!enabled || string.IsNullOrEmpty(content)) return;

            try
            {
                string id = GetHardwareId();

                // 1. Force TLS 1.2
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                using (WebClient client = new WebClient())
                {
                    // 2. Set headers
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Encoding = Encoding.UTF8; // Ensure UTF8 for special characters

                    // 3. Robust JSON escaping
                    // We must escape backslashes first, then quotes, then newlines.
                    string escapedContent = content
                        .Replace("\\", "\\\\") // Escape backslashes
                        .Replace("\"", "\\\"") // Escape quotes
                        .Replace("\n", "\\n") // Escape newlines
                        .Replace("\r", "\\r"); // Escape carriage returns

                    string payload = string.Format("{{\"content\": \"**[{0}]**\\n{1}\"}}", id, escapedContent);

                    // 4. Upload as string
                    client.UploadString(webhook, "POST", payload);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending webhook: " + ex.Message);
            }
        }

        public static string GetHardwareId()
        {
            // Combine hardware constants
            string rawData = Environment.MachineName +
                             Environment.ProcessorCount +
                             Environment.UserName +
                             Environment.OSVersion;

            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes) sb.Append(b.ToString("X2"));
                return sb.ToString(); // Returns a 32-char hex string
            }
        }
    }
}
