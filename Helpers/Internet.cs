using DebloaterTool.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace DebloaterTool.Helpers
{
    internal class Internet
    {
        public static void Inizialize() 
        {
            // For .NET Framework 4.0, enable TLS 1.2 by casting its numeric value.
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            if (!IsInternetAvailable())
            {
                Logger.Log("No internet connection detected.", Level.ERROR);
                Logger.Log("This program requires an active internet connection to run.", Level.ERROR);
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        private static bool IsInternetAvailable()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("https://github.com/megsystem/DebloaterTool"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool DownloadFile(string url, string outputPath)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    long remoteFileSize = 0;

                    // Get remote file size using HEAD request
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "HEAD";
                    request.UserAgent = "Mozilla/5.0 (compatible; AcmeInc/1.0)";

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        long.TryParse(response.Headers.Get("Content-Length"), out remoteFileSize);
                    }

                    if (File.Exists(outputPath))
                    {
                        long localFileSize = new FileInfo(outputPath).Length;

                        if (remoteFileSize > 0 && localFileSize == remoteFileSize)
                        {
                            Logger.Log($"File already exists at '{outputPath}' and matches remote size, skipping download.", Level.WARNING);
                            return true;
                        }
                    }

                    using (ManualResetEvent waitHandle = new ManualResetEvent(false))
                    {
                        bool success = true;
                        Exception downloadException = null;

                        client.DownloadProgressChanged += (s, e) =>
                        {
                            int totalBlocks = 50;
                            int progressBlocks = (int)(e.ProgressPercentage / 99.0 * totalBlocks);
                            string progressBar = new string('#', progressBlocks) + new string('-', totalBlocks - progressBlocks);
                            if (e.ProgressPercentage != 100)
                            {
                                Logger.Log($"Downloading: [{progressBar}] {e.ProgressPercentage + 1}%   ", Level.DOWNLOAD,
                                    Return: true, Save: false);
                            }
                        };

                        client.DownloadFileCompleted += (s, e) =>
                        {
                            if (e.Error != null)
                            {
                                downloadException = e.Error;
                                success = false;
                            }

                            if (e.Cancelled)
                            {
                                success = false;
                            }

                            waitHandle.Set();
                        };

                        client.DownloadFileAsync(new Uri(url), outputPath);
                        waitHandle.WaitOne(); // Wait until download completes

                        if (!success)
                        {
                            if (File.Exists(outputPath))
                            {
                                File.Delete(outputPath); // Delete partial or empty file
                            }

                            throw downloadException ?? new Exception("Download was cancelled or failed.");
                        }
                    }

                    System.Console.WriteLine();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(
                    $"Download failed. Url: {url}. Exception: {ex}",
                    Level.ERROR
                );
                return false;
            }
        }

        public static string FetchDataUrl(string apiUrl)
        {
            Logger.Log("Fetching data information...", Level.INFO);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.UserAgent = "Mozilla/5.0 (compatible; AcmeInc/1.0)";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    // Check for a successful response status code (e.g., 200 OK)
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return reader.ReadToEnd();
                    }
                    else
                    {
                        Logger.Log($"Error: Received {response.StatusCode} status from the API", Level.ERROR);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"An unexpected error occurred: {ex.Message}", Level.ERROR);
                return null;
            }
        }
    }
}
