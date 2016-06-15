using CatchupGrabber.NetworkAssister;
using SubCSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;


namespace CatchupGrabber
{
    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        //Locals
        string url;
        string subtitle;
        DownloadMethod dlMethod;
        Country cnt;
        string fileName;
        CookieAwareWebClient cAWebClient;
        //Async temp valuse
        long bytesReceived = 0;

        //Process
        Process proc;
        DWBindings dwBinding = new DWBindings();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="passedData">A download object containing information about the show to download</param>
        /// <param name="fName">The filename</param>
        public DownloadWindow(DownloadObject passedData, string fName)
        {
            InitializeComponent();
            DataContext = dwBinding;
            this.Show();
            this.Closed += DownloadWindow_Closed;
            url = passedData.EpisodeUrl;
            subtitle = passedData.SubtitleUrl;
            dlMethod = passedData.DlMethod;
            fileName = fName;
            cnt = passedData.CountryOfOrigin;
            //Clean the name for windows
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }
            //Replace spaces if required
            if(Properties.Settings.Default.SpaceReplace)
            {
                fileName = fileName.Replace(" ", ".");
            }
            if (!subtitle.Equals("") && Properties.Settings.Default.DownloadSubtitlesSetting)
                Task.Factory.StartNew(() => DownloadSubtitle(subtitle,fileName,Properties.Settings.Default.SubtitleFormat)); //Run the downloadsub in a background thread
            DownloadShow();
        }

        
        /// <summary>
        /// Handles the download options for a Download object
        /// </summary>
        private void DownloadShow()
        {
            dwBinding.DownloadInfo = "Downloading: " + fileName;
            taskBarDownload.ProgressState = TaskbarItemProgressState.Normal;
            switch (dlMethod)
            {
                case DownloadMethod.HLS:
                    RunFFmpeg(url, fileName);
                    break;
                case DownloadMethod.HTTP:
                    ProgressDL.Visibility = Visibility.Visible;
                    switch(cnt)
                    {
                        case Country.Spain:
                            if (Properties.Settings.Default.ProxyOptionSpanish.Equals("HTTP")) 
                                HTTPProxyDownload(url,fileName,Properties.Settings.Default.HTTPSpanish);
                            else if (Properties.Settings.Default.ProxyOptionSpanish.Equals("Glype"))
                                StandardDownload(url, fileName + ".mp4", Properties.Settings.Default.GlypeSpanish);
                            else
                                StandardDownload(url, fileName + ".mp4", "");
                            break;
                        default:
                            StandardDownload(url, fileName + ".mp4", "");
                            break;
                    }
                    break;
            }
        }
        /// <summary>
        /// Downloads a subtitle
        /// </summary>
        /// <param name="subtileUrl">The location of the subtitle</param>
        /// <param name="fileNameSub">File name to save</param>
        /// <param name="fileType">The type to convert to (null or empty to not convert)</param>
        private void DownloadSubtitle(string subtileUrl, string fileNameSub, string fileType)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(new Uri(subtileUrl), fileNameSub + Path.GetExtension(subtileUrl));
            }
            if (Properties.Settings.Default.ConvertSubtitle && !string.IsNullOrEmpty(fileType))
            {
                SubtitleConverter conv = new SubtitleConverter();
                conv.EncodingRead = Encoding.UTF8;
                conv.ConvertSubtitle(fileNameSub + Path.GetExtension(subtileUrl), fileNameSub + fileType);
            }
        }

        //------------------Download methods------------------------//
        /// <summary>
        /// Download HLS stream via ffmpeg
        /// </summary>
        /// <param name="url">The URL to download, (Master or Rendition)</param>
        /// <param name="nameLocation">The file name and location (without file extension)</param>
        public void RunFFmpeg(string url, string nameLocation)
        {
            //ffmpeg
            ProcessStartInfo ffmpeg = new ProcessStartInfo();
            proc = new Process();
            ffmpeg.FileName = "ffmpeg.exe";
            ffmpeg.Arguments = " -i " + url + " -acodec copy -vcodec copy -bsf:a aac_adtstoasc " + '"' + nameLocation + '"' + ".mp4";
            
            proc.StartInfo = ffmpeg;
            proc.EnableRaisingEvents = true;
            proc.Exited += FFmpeg_Exited;
            proc.Start();
            
        }

        /// <summary>
        /// Event handler for when FFmpeg completes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FFmpeg_Exited(object sender, EventArgs e)
        {
            int tmp = proc.ExitCode;
            proc.Dispose();
            if (tmp == 0) //On success
            {
                //Update textblock as it is on a different thread
                dwBinding.DownloadProgress = "Complete";
                //Close the window if they want it
                if (Properties.Settings.Default.ExitDLOnDownload)
                {
                    this.Dispatcher.BeginInvoke((Action)(() => {
                        this.Close();
                    }));
                }
            }
            else //It failed so enable retry
            {
                dwBinding.DownloadProgress = "FFmpeg returned a non 0 exit code";
                this.Dispatcher.BeginInvoke((Action)(() => {
                    ButtonRetry.Visibility = Visibility.Visible;
                }));
            }
        }

        /// <summary>
        /// Downloads the specified mp4 url via a HTTP proxy
        /// </summary>
        /// <param name="url">The url to download</param>
        /// <param name="name">The filename to save as an (no extension, mp4 added)</param>
        /// <param name="httpProxy">The HTTP proxy</param>
        public void HTTPProxyDownload(string url, string name, string httpProxy)
        {
            WebProxy wp = new WebProxy(httpProxy);
            cAWebClient = new CookieAwareWebClient();
            cAWebClient.Proxy = wp;
            cAWebClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            cAWebClient.DownloadFileCompleted += WebClient_AsyncCompletedEventHandler;
            cAWebClient.DownloadFileAsync(new Uri(url), name + ".mp4");
        }
        /// <summary>
        /// Standard download for a file, note proxy download will be slow and appear unresponsive for a while
        /// </summary>
        /// <param name="url">The url to download from</param>
        /// <param name="name">File name to save to plus extension</param>
        /// <param name="proxyAddress">A string url to a Glype proxy</param>
        public async void StandardDownload(string url, string name, string proxyAddress)
        {
            cAWebClient = new CookieAwareWebClient();
            cAWebClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            cAWebClient.DownloadFileCompleted += WebClient_AsyncCompletedEventHandler;
            if (proxyAddress != "")//If they suplied a proxy
            {
                //Add standard post headers
                cAWebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                //Referer since it might not like requests from elsewhere
                cAWebClient.Headers.Add("referer", proxyAddress);
                try
                {
                    //Make a blank request to example.com for cookies
                    await cAWebClient.UploadStringTaskAsync(proxyAddress + "/includes/process.php?action=update", "POST", "u=" + "example.com" + "&allowCookies=on");
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    ButtonRetry.Visibility = Visibility.Visible;
                    taskBarDownload.ProgressState = TaskbarItemProgressState.Error;
                    dwBinding.DownloadProgress = "Download failed, could not find glype proxy";
                    return;
                }
                //Download the file
                cAWebClient.DownloadFileAsync(new Uri(proxyAddress + "/browse.php?u=" + url + "&b=12&f=norefer"), name);
            }
            else // no proxy, download normally
            {
                cAWebClient.DownloadFileAsync(new Uri(url), name);
            }
        }
        
        /// <summary>
        /// Handles updating the visuals when downloading a video though the webclient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            bytesReceived = e.BytesReceived;
            ProgressDL.Value = e.ProgressPercentage;
            dwBinding.DownloadProgress = (bytesReceived / 1024).ToString() + "kB / " + (e.TotalBytesToReceive / 1024).ToString() + "kB"; //Download progress in byte
            if(Environment.OSVersion.Version.Major >= 6) //As xp cant do taskbar things
            {
                taskBarDownload.ProgressValue = e.ProgressPercentage/100.0;
            }
        }
        /// <summary>
        /// Handles aftermath when the application downloads a video though the webclient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebClient_AsyncCompletedEventHandler(object sender, AsyncCompletedEventArgs e)
        {
            DisposeWebClient();
            if (e.Error != null)
            {
                Console.WriteLine(e.Error.StackTrace);
                ButtonRetry.Visibility = Visibility.Visible;
                taskBarDownload.ProgressState = TaskbarItemProgressState.Error;
                dwBinding.DownloadProgress = "Download failed somewhere";
                return;
            }
            dwBinding.DownloadProgress = "Download Complete";
            taskBarDownload.ProgressState = TaskbarItemProgressState.None;
            if (Properties.Settings.Default.ExitDLOnDownload)
                this.Close();
        }

        /// <summary>
        /// Download individual hls segments via webclient
        /// </summary>
        /// <param name="url">The bitrate url</param>
        public void proxiedHls(string url, string proxy)
        {
            string parent = new Uri(new Uri(url), ".").ToString();
            string m3u8;
            using (WebClient wc = new WebClient())
            {
                wc.Proxy = new WebProxy(proxy);
                wc.Encoding = Encoding.UTF8;
                m3u8 = wc.DownloadString(url);
            }
            List<string> filelist = new List<string>();
            using (StringReader strReader = new StringReader(m3u8))
            {
                string line;
                while ((line = strReader.ReadLine()) != null)
                {
                    if (line.StartsWith("#")) continue;
                    using (WebClient wc = new WebClient())
                    {
                        wc.Proxy = new WebProxy(proxy);
                        wc.Encoding = Encoding.UTF8;
                        wc.DownloadFile(new Uri(parent + line), @"hls_temp" + line);
                    }
                    filelist.Add(@"hls_temp" + line);
                }
            }
            //Join and write
            using (var outputStream = File.Create(filelist[0] + "fin.ts"))
            {
                foreach (var file in filelist)
                {
                    using (var inputStream = File.OpenRead(file))
                    {
                        inputStream.CopyTo(outputStream);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the click of the retry button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRetry_Click(object sender, RoutedEventArgs e)
        {
            ButtonRetry.Visibility = Visibility.Hidden;
            dwBinding.DownloadProgress = "Retrying download";
            if (Environment.OSVersion.Version.Major >= 6) 
            {
                taskBarDownload.ProgressValue = 0;
            }
            DownloadShow();
        }
        //Download methods end

        //------------------------------------Cleanup methods
        private void DisposeWebClient()
        {
            try
            {
                cAWebClient.Dispose();
            }
            catch { }//Already disposed or null 
        }

        void DownloadWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                if (cAWebClient != null)
                {
                    cAWebClient.CancelAsync();
                    cAWebClient.Dispose();
                }
            }
            catch { }
        }

    }

    /// <summary>
    /// Bindings for the visual elements
    /// </summary>
    public class DWBindings : INotifyPropertyChanged
    {
        private string _downloadProgress = "";
        public string DownloadProgress
        {
            get
            {
                return _downloadProgress;
            }
            set
            {
                _downloadProgress = value;
                OnPropertyChanged("DownloadProgress");
            }
        }
        private string _downloadInfo = "";
        public string DownloadInfo
        {
            get
            {
                return _downloadInfo;
            }
            set
            {
                _downloadInfo = value;
                OnPropertyChanged("DownloadInfo");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
