using cu_grab.NetworkAssister;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace cu_grab
{
    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        //Locals
        String url;
        String subtitle;
        DownloadMethod dlMethod;
        Country cnt;
        String fileName;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="passedData">A download object containing information about the show to download</param>
        /// <param name="fName">The filename</param>
        public DownloadWindow(DownloadObject passedData,String fName)
        {
            InitializeComponent();
            this.Show();
            url = passedData.EpisodeUrl;
            subtitle = passedData.SubtitleUrl;
            dlMethod = passedData.DlMethod;
            fileName = fName;
            cnt = passedData.CountryOfOrigin;
            if (!subtitle.Equals("") && Properties.Settings.Default.DownloadSubtitlesSetting)
                DownloadSubtitle();
            DownloadShow();
        }
        /// <summary>
        /// Handles the download options for a Downlod object
        /// </summary>
        private void DownloadShow()
        {
            TextBlockDLInfo.Text = "Downloading: " + fileName;
            switch(dlMethod)
            {
                case DownloadMethod.HLS:
                    RunFFmpeg(url, fileName);
                    break;
                case DownloadMethod.HTTP:
                    ProgressDL.Visibility = System.Windows.Visibility.Visible;
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
                    }
                    break;
            }
        }
        private void DownloadSubtitle()
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(new System.Uri(subtitle), fileName + Path.GetExtension(subtitle));
            }
        }

        //------------------Download methods------------------------//
        /// <summary>
        /// Download HLS stream via ffmpeg
        /// </summary>
        /// <param name="url">The URL to download, (Master or Rendition)</param>
        /// <param name="nameLocation">The file name and location (without file extension)</param>
        /// <returns>Returns FFmpeg's error code</returns>
        public void RunFFmpeg(string url, string nameLocation)
        {
            //ffmpeg
            ProcessStartInfo ffmpeg = new ProcessStartInfo();
            ffmpeg.Arguments = " -i " + url + " -acodec copy -vcodec copy -bsf:a aac_adtstoasc " + '"' + nameLocation + '"' + ".mp4";
            ffmpeg.FileName = "ffmpeg.exe";

            using (Process proc = Process.Start(ffmpeg))
            {
            }
        }

        public void HTTPProxyDownload(string url, string name, string httpProxy)
        {
            using(WebClient webClient = new WebClient())
            {
                WebProxy wp = new WebProxy(httpProxy);
                webClient.Proxy = wp;
                webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                webClient.DownloadFileCompleted += WebClient_AsyncCompletedEventHandler;
                webClient.DownloadFileAsync(new System.Uri(url), name);
            }
        }
        /// <summary>
        /// Standard download for a file, note proxy download will be slow and appear unresponsive for a while
        /// </summary>
        /// <param name="url">The url to download from</param>
        /// <param name="name">File name to save to plus extension</param>
        /// <param name="proxyAddress">A string url to a Glype proxy</param>
        public void StandardDownload(String url, String name, String proxyAddress)
        {
            using (CookieAwareWebClient webClient = new CookieAwareWebClient())
            {
                webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                webClient.DownloadFileCompleted += WebClient_AsyncCompletedEventHandler;
                if (proxyAddress != "")//If they suplied a proxy
                {
                    //Add standard post headers
                    webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    //Referer since it might not like requests from elsewhere
                    webClient.Headers.Add("referer", proxyAddress);
                    //Make a blank request to example.com for cookies
                    webClient.UploadData(proxyAddress + "/includes/process.php?action=update", "POST", System.Text.Encoding.UTF8.GetBytes("u=" + "example.com" + "&allowCookies=on"));
                    //Download the file
                    webClient.DownloadFileAsync(new System.Uri(proxyAddress + "/browse.php?u=" + url + "&b=12&f=norefer"), name);
                }
                else
                {
                    webClient.DownloadFileAsync(new System.Uri(url), name);
                }
            }
        }
        
        void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
           ProgressDL.Value = e.ProgressPercentage;
            TextBlockProgress.Text = (e.BytesReceived / 1024).ToString() + "kB / " + (e.TotalBytesToReceive / 1024).ToString() + "kB"; //Download progress in byte
        }
        void WebClient_AsyncCompletedEventHandler(object sender, AsyncCompletedEventArgs e)
        {
            TextBlockProgress.Text = "Download Complete";
        }

        /// <summary>
        /// Download individual hls segments via webclient
        /// </summary>
        /// <param name="url">The bitrate url</param>
        public void proxiedHls(String url)
        {
            String parent = new Uri(new Uri(url), ".").ToString();
            String m3u8;
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                m3u8 = wc.DownloadString(url);
            }
            List<string> filelist = new List<string>();
            using (StringReader strReader = new StringReader(m3u8))
            {
                String line;
                while ((line = strReader.ReadLine()) != null)
                {
                    if (line.StartsWith("#")) continue;
                    using (WebClient wc = new WebClient())
                    {
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

        //Download methods end
    }
}
