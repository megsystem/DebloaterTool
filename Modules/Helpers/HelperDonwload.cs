using System;
using System.IO;
using System.Net;
using System.Threading;

namespace DebloaterTool
{
    internal class HelperDonwload
    {
        public static bool DownloadFile(string url, string outputPath)
        {
            try
            {
                if (File.Exists(outputPath))
                {
                    Logger.Log($"File already exists at '{outputPath}', skipping download.", Level.WARNING);
                    return true;
                }

                using (WebClient client = new WebClient())
                using (ManualResetEvent waitHandle = new ManualResetEvent(false))
                {
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; AcmeInc/1.0)");

                    bool success = true;
                    Exception downloadException = null;

                    client.DownloadProgressChanged += (s, e) =>
                    {
                        int totalBlocks = 50;
                        int progressBlocks = (int)(e.ProgressPercentage / 99.0 * totalBlocks);
                        string progressBar = new string('#', progressBlocks) + new string('-', totalBlocks - progressBlocks);
                        if (e.ProgressPercentage != 100)
                        {
                            Logger.Log($"Downloading: [{progressBar}] {e.ProgressPercentage+1}%   ", Level.DOWNLOAD,
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
                    waitHandle.WaitOne(); // Wait until the download completes

                    if (!success)
                    {
                        if (File.Exists(outputPath))
                        {
                            File.Delete(outputPath); // Delete partial or empty file
                        }

                        throw downloadException ?? new Exception("Download was cancelled or failed.");
                    }
                }

                Console.WriteLine();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Download error: {ex.Message}", Level.ERROR);
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
