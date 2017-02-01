using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
namespace ClientRaw
{
   public partial class ExtendedWebClient : WebClient
    {
        private Timer downloadingProgressTimer = new Timer();
        private Stopwatch timeoutStopwatch = new Stopwatch();

        /// <summary>
        ///  Gets or Sets the length of time, in milliseconds, before the request time out
        /// </summary>
        public int timeout
        {
            get;
            set;
        }

        public bool stoppedByUser
        {
            get;
            set;
        }
        public ExtendedWebClient()
        {
            downloadingProgressTimer.Tick += timer_tick;
            downloadingProgressTimer.Interval = 10000;
        }


        private void timer_tick(object sender, EventArgs e)
        {
            if (timeoutStopwatch.ElapsedMilliseconds > this.timeout)
            {
                stoppedByUser = false;
                this.CancelAsync();
            }
        }

        public void stop()
        {
            stoppedByUser = true;
            this.CancelAsync();
        }
        protected override void OnDownloadFileCompleted(System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // done with downloads

            this.downloadingProgressTimer.Stop();
            this.timeoutStopwatch.Stop();
            base.OnDownloadFileCompleted(e);


        }



        protected override void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            timeoutStopwatch.Restart();

            base.OnDownloadProgressChanged(e);
        }


       
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = base.GetWebRequest(address); ;
            webRequest.Timeout = this.timeout;
            return webRequest;
        } 
    }


   public partial class ExtendedWebClient : WebClient
    {
       public string downloadedFileName = "";

       public void startDownloadingFile(string fileURI)
       {
           string saveFileName = "";

           saveFileName = this.getFileName(fileURI);
           downloadFile(fileURI, saveFileName);

       }

        private string getFileName(string fileURI)
        {
            string saveFileName = "";
            
                if (fileURI.Contains("Auto"))
                {
                    string tempFileName = Path.GetRandomFileName();
                    saveFileName = "AutoData" + Path.GetFileName(tempFileName) + ".txt";

                }
                else
                {
                    saveFileName = Path.GetFileName(fileURI);
                }

                if ((String)this.Headers["Accept-Encoding"] == "gzip")
                {
                    saveFileName = saveFileName.Replace(".txt", "compressed.txt");
                }
                return saveFileName;            
            

        }

        private string getDownloadsFolder()
        {
            DirectoryInfo downloadsFolder;
            var currentDirectory = Directory.GetCurrentDirectory();
            string downloadsFolderPath = Path.Combine(currentDirectory, "Downloads");
            if (!Directory.Exists(downloadsFolderPath))
            {
                downloadsFolder = Directory.CreateDirectory(downloadsFolderPath);
            }
            else
            {
                downloadsFolder = new DirectoryInfo(downloadsFolderPath);
            }

            return downloadsFolder.FullName;
        }


        // core function to download file from a url
        private void downloadFile(string fileURI, string saveFileName)
        {
            try
            {
                string downloadsFolder = this.getDownloadsFolder();

                var downloadFileName = Path.Combine(downloadsFolder , saveFileName);
                Uri fileUrl = new Uri(fileURI);
                this.DownloadFileAsync(fileUrl, downloadFileName);
                this.downloadingProgressTimer.Start();
                this.downloadedFileName = downloadFileName;
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 404)
                {
                    Console.WriteLine("cant no find the file");
                    return;
                }
                else
                {
                    string errorMessage = ex.Message;
                    return;


                }


            }
            catch (IOException ex)
            {
                Console.WriteLine("no such filename " + ex.Message);
                return;

            }
            catch (WebException ex)
            {
                Console.WriteLine("error has occourd " + ex.Message);
                return;

            }


        }

    }
}
