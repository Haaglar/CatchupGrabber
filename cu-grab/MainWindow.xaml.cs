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
namespace cu_grab
{
    /* Note: Requires FFmpeg
     * TODO: 
     * More proper error handling
     * Async on proxy
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
                                selectedShow = RTVEc.clickDisplayedShow(objectList);
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
                                String name = RTVEc.getSelectedName(objectList);
                                String url = RTVEc.getUrl(objectList);
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
        /// 
        /// </summary>
        /// <param name="url">The rendition URL to download</param>
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
                proc.WaitForExit();
                exitCode = proc.ExitCode;
            }
            return exitCode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">The url to download from</param>
        /// <param name="name">Name plus extension</param>
        public void standardDownload(String url, String name)
        {
            using (WebClient webClient = new WebClient())
            {
                /*if (TextBoxProxy.Text != "")
                {
                    webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    webClient.Headers.Add("referer", TextBoxProxy.Text);
                    byte[] video = webClient.UploadData(TextBoxProxy.Text + "/includes/process.php?action=update", "POST", System.Text.Encoding.UTF8.GetBytes("u=" + url + "&allowCookies=on"));
                    File.WriteAllBytes(name, video);
                }
                else
                {*/
                webClient.DownloadProgressChanged += wc_DownloadProgressChanged;
                webClient.DownloadFileAsync(new System.Uri(url),name);
                
                //}
            }
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBarDL.Value = e.ProgressPercentage;
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
                    tenPlay.cleanEpisodes(objectList);
                    curState = State.DisplayingShows;  
                    selectedShow = "";
                    break;
                case Site.RTVEC:
                    RTVEc.cleanEpisodes(objectList);
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
                    errorLabel.Text = "Failed to get Episode listings";
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
            if (!RTVEc.requested)
            {
                try
                {
                    RTVEc.fillShowsList(objectList);
                }
                catch
                {
                    errorLabel.Text = "Failed to get Episode listings";
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
                RTVEc.setRTVEcActive(objectList);
            }
            curState = State.DisplayingShows;
            curSite = Site.RTVEC;
            selectedShow = "";
        }
    }
}
