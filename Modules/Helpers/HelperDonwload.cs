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
                        int progressBlocks = (int)(e.ProgressPercentage / 100.0 * totalBlocks);
                        string progressBar = new string('#', progressBlocks) + new string('-', totalBlocks - progressBlocks);
                        Logger.Log($"Downloading: [{progressBar}] {e.ProgressPercentage}%   ", Level.DOWNLOAD,
                            Return: true, Save: false);
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

                Logger.Log($"Download completate in {outputPath}", Level.SUCCESS, NewLine: true);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Download error: {ex.Message}", Level.ERROR);
                return false;
            }
        }
    }
}
