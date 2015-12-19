using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Reflection;
using System.Web.Script.Serialization;
using System.ComponentModel;
namespace cu_grab
{
    /* Note: Requires FFmpeg
     * TODO: 
     * More proper error handling
     * A better state handler by creating a Base object for downloader classes to derive from and using Function overloading.
     * Make a attractive GUI
     * Use API, for descriptions and stuff instead of crawling on tenplay
     * p7, Get a good oauth library or something
     *     
     */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        String selectedShow = "";
        enum State {DisplayingNone, DisplayingShows, DisplayingEpisodes};
        enum Site { None, TenP, RTVEC }
        State curState = State.DisplayingNone;
        Site curSite = Site.None;
        Tenp tenPlay;
        RTVEc rtveClan;
        public MainWindow()
        {
            InitializeComponent();
        }
    
        /// <summary>
        /// Double click of list item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void oL_ItemPressed(object sender, MouseButtonEventArgs e)
        {
            //Handle sites
            switch (curSite) 
            { 
                //Tenplay
                case Site.TenP:
                    switch (curState)
                    {
                        // Get episodes for the selected show
                        case State.DisplayingShows:
                            try
                            {
                                selectedShow = tenPlay.clickDisplayedShow();
                                curState = State.DisplayingEpisodes;
                            }
                            catch
                            {
                                errorLabel.Text = "Failed to get episode list for selected show";
                            }
                            break;
                        //Download Selected show
                        case State.DisplayingEpisodes:
                            try
                            {
                                String name = tenPlay.getSelectedName();
                                String dlUrl = tenPlay.getUrl();
                                runFFmpeg(dlUrl, selectedShow + " " + name);
                            }
                            catch
                            {
                                errorLabel.Text = "Failed to download episode";
                            }
                            break;
                    }
                    break;
                //RTVEC
                case Site.RTVEC:
                    switch (curState)
                    {
                        // Get episodes for the selected show
                        case State.DisplayingShows:
                            try
                            {
                                selectedShow = rtveClan.clickDisplayedShow();
                                curState = State.DisplayingEpisodes;
                            }
                            catch
                            {
                                errorLabel.Text = "Failed to get episode list for selected show";
                            }
                            break;

                        //Download Selected show
                        case State.DisplayingEpisodes:
                            try
                            {
                                String name = rtveClan.getSelectedName();
                                String url = rtveClan.generateUrl();
                                standardDownload(url, selectedShow + " " + name + ".mp4");
                            }
                            catch
                            {
                                errorLabel.Text = "Failed to download episode";
                            }
                            break;
                    }
                    break;
            }
        }
    
        /// <summary>
        /// Download HLS stream via ffmpeg
        /// </summary>
        /// <param name="url">The URL to download, (Master or Rendition)</param>
        /// <param name="nameLocation">The file name and location (without file extension)</param>
        /// <returns>Returns FFmpegs error code</returns>
        public int runFFmpeg(string url, string nameLocation)
        {
            //ffmpeg
            ProcessStartInfo ffmpeg = new ProcessStartInfo();
            ffmpeg.Arguments = " -i " + url + " -acodec copy -vcodec copy -bsf:a aac_adtstoasc " + '"' + nameLocation + '"' + ".mp4";
            ffmpeg.FileName = "ffmpeg.exe";
            int exitCode;
            using (Process proc = Process.Start(ffmpeg))
            {
                errorLabel.Text = "Downloading, please wait.";
                proc.WaitForExit();
                exitCode = proc.ExitCode;
            }
            errorLabel.Text = "Download completed";
            return exitCode;
        }
        /// <summary>
        /// Standard download for a file, note proxy download will be slow
        /// </summary>
        /// <param name="url">The url to download from</param>
        /// <param name="name">Name plus extension</param>
        public void standardDownload(String url, String name)
        {
            using (CookieAwareWebClient webClient = new CookieAwareWebClient())
            {
                webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
                webClient.DownloadFileCompleted += webClient_AsyncCompletedEventHandler;
                errorLabel.Text = "Downloading please wait...";
                if (TextBoxProxy.Text != "")//If they spullied a proxy
                {

                    //Add required http
                    String proxyAddress = TextBoxProxy.Text.StartsWith("http://") ? TextBoxProxy.Text : "http://" + TextBoxProxy.Text;
                    webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    webClient.Headers.Add("referer", proxyAddress);
                    //Make a blank request to example.com for cookies
                    webClient.UploadData(proxyAddress + "/includes/process.php?action=update", "POST", System.Text.Encoding.UTF8.GetBytes("u=" + "example.com" + "&allowCookies=on"));
                    //Download the file
                    
                    webClient.DownloadFileAsync(new System.Uri(proxyAddress + "browse.php?u=" + url + "&b=12&f=norefer"), name);
                }
                else
                {
                    webClient.DownloadFileAsync(new System.Uri(url), name);
                }
            }
        }

        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBarDL.Value = e.ProgressPercentage;
        }
        void webClient_AsyncCompletedEventHandler(object sender, AsyncCompletedEventArgs e)
        {
            errorLabel.Text = "Download Complete";
        }
        /// <summary>
        /// Cleanup function for returning to shows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Shows_Pressed(object sender, RoutedEventArgs e)
        {
            switch(curSite)
            {
                case Site.TenP:
                    tenPlay.cleanEpisodes();
                    curState = State.DisplayingShows;  
                    selectedShow = "";
                    break;
                case Site.RTVEC:
                    rtveClan.cleanEpisodes();
                    curState = State.DisplayingShows;
                    selectedShow = "";
                    break;

            }
            
        }

        private void ButtonTenplay_Click(object sender, RoutedEventArgs e)
        {
            //First time selecting site
            if (tenPlay == null)
            {
                try
                {
                    tenPlay = new Tenp(objectList);
                    tenPlay.fillShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get Episode listings for Tenplay.";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.TenP)
            {
                Shows_Pressed(null,null);
                return;
            }
            // other time selecting site
            else
            {
                tenPlay.setTPActive();
            }  
            curState = State.DisplayingShows;
            curSite = Site.TenP;
            selectedShow = "";
        }

        private void ButtonRTVEC_Click(object sender, RoutedEventArgs e)
        {
            //First time selecting site
            if (rtveClan == null)
            {
                try
                {
                    rtveClan = new RTVEc(objectList);
                    rtveClan.fillShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get Episode listings for RTVE Clan";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.RTVEC)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                rtveClan.setRTVEcActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.RTVEC;
            selectedShow = "";
        }

        /// <summary>
        /// A CookieAwareWebClient, used to store Glype proxy seesion info. Since we cant Async download when we need to post data at the same time.
        /// Thanks to http://stackoverflow.com/questions/4740752/how-do-i-log-into-a-site-with-webclient
        /// </summary>
        public class CookieAwareWebClient : WebClient
        {
            public CookieAwareWebClient()
            {
                CookieContainer = new CookieContainer();
            }
            public CookieContainer CookieContainer { get; private set; }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = (HttpWebRequest)base.GetWebRequest(address);
                request.CookieContainer = CookieContainer;
                return request;
            }
        }

    }
}
