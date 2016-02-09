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
using SubCSharp;
using cu_grab.NetworkAssister;
namespace cu_grab
{
    /* Note: Requires FFmpeg
     * TODO: 
     * More sites :)
     * More proper error handling
     * Make a attractive GUI
     * Use API, for descriptions and stuff instead of crawling on tenplay, p7 (OAuth) and possibly RTE(OAuth)
     */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        String selectedShow = "";
        enum State {DisplayingNone, DisplayingShows, DisplayingEpisodes};
        enum Site {None, TenP, Plus7, RTVEClan, RTE, DPlay, TV3Cat, Super3}
        State curState = State.DisplayingNone;
        Site curSite = Site.None;

        //--Downloaderbstract Objects
        DownloadAbstract dlAbs; //What will be currently selected
        Tenp tenPlay;
        RTVEc rtveClan;
        Plus7 plus7;
        RTE rteIE;
        DPlay dplay;
        TV3Cat tv3CatCCMA;
        Super3 super3;

        SubtitleConverter subConv;
        public MainWindow()
        {
            InitializeComponent();
            if(!File.Exists("FFmpeg.exe"))
            {
                MessageBox.Show("Catch-up Grabber requires FFmpeg to download from certain sites, please copy it into the working directory.");
            }
        }
    
        /// <summary>
        /// Double click of list item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OL_ItemPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock || e.OriginalSource is Border) //Make sure that we double click an item not the scrollbar
            {
                switch (curState)
                {
                    // Get episodes for the selected show
                    case State.DisplayingShows:
                        try
                        {
                            selectedShow = dlAbs.ClickDisplayedShow();
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
                            errorLabel.Text = "Downloading episode, please wait...";
                            String name = dlAbs.GetSelectedName();
                            String dlUrl = dlAbs.GetUrl();
                            switch (curSite) //Handle the correct download method for the option selected
                            {
                                case Site.Plus7:
                                    if (Properties.Settings.Default.DownloadSubtitlesSetting)
                                    {
                                        String subUrl = dlAbs.GetSubtitles();
                                        if (subUrl != "")
                                        {
                                            StandardDownloadSub(subUrl, selectedShow + " " + name + ".dfxp", ""); //Thread locked cause yeah
                                            subConv = new SubtitleConverter();
                                            subConv.ConvertSubtitle(selectedShow + " " + name + ".dfxp", selectedShow + " " + name + ".srt");
                                        }
                                    }
                                    goto case Site.TenP;
                                case Site.TenP: case Site.RTE: case Site.DPlay:
                                    RunFFmpeg(dlUrl, selectedShow + " " + name);
                                    break;
                                case Site.RTVEClan: case Site.TV3Cat: case Site.Super3:
                                    StandardDownload(dlUrl, selectedShow + " " + name + ".mp4", Properties.Settings.Default.GlypeProxySettingRTVE);
                                    break;
                            }
                        }
                        catch
                        {
                            errorLabel.Text = "Failed to download episode";
                        }
                        break;
                }
            }
        }
        

        //------------------Download methods------------------------//
        /// <summary>
        /// Download HLS stream via ffmpeg
        /// </summary>
        /// <param name="url">The URL to download, (Master or Rendition)</param>
        /// <param name="nameLocation">The file name and location (without file extension)</param>
        /// <returns>Returns FFmpeg's error code</returns>
        public int RunFFmpeg(string url, string nameLocation)
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
        /// Standard download for a file, note proxy download will be slow and appear unresponsive for a while
        /// </summary>
        /// <param name="url">The url to download from</param>
        /// <param name="name">File name to save to plus extension</param>
        /// <param name="proxyAddress">A string url to a Glype proxy</param>
        public void StandardDownload(String url, String name, String proxyAddress)
        {
            using (CookieAwareWebClient webClient = new CookieAwareWebClient())
            {
                webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
                webClient.DownloadFileCompleted += webClient_AsyncCompletedEventHandler;
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

        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBarDL.Value = e.ProgressPercentage;
            textBlockDownloadStatus.Text = (e.BytesReceived / 1024).ToString() + "kB / " + (e.TotalBytesToReceive / 1024).ToString() + "kB"; //Download progress in byte
        }
        void webClient_AsyncCompletedEventHandler(object sender, AsyncCompletedEventArgs e)
        {
            errorLabel.Text = "Download Complete";
        }


        public void StandardDownloadSub(String url, String name, String proxyAddress)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(new System.Uri(url), name);
            }
        }


        /// <summary>
        /// Download individual hls segments via webclient
        /// </summary>
        /// <param name="url">The bitrate url</param>
        public void proxiedHls(String url)
        {
            String parent = new Uri(new Uri(url), ".").ToString();
            String m3u8;
            using(WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                m3u8 = wc.DownloadString(url);
            }
            List<string> filelist= new List<string>();
            using(StringReader strReader = new StringReader(m3u8))
            {
                String line;
                while((line = strReader.ReadLine()) != null)
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

        //-----------------BUTTONS START---------------//
        /// <summary>
        /// Cleanup function for returning to shows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Shows_Pressed(object sender, RoutedEventArgs e)
        {
            if (dlAbs != null)
            {
                dlAbs.CleanEpisodes();
                curState = State.DisplayingShows;
                selectedShow = "";
            }
        }
        /// <summary>
        /// Handles the action for when the Tenplay button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonTenplay_Click(object sender, RoutedEventArgs e)
        {
            //First time selecting site
            if (tenPlay == null)
            {
                try
                {
                    tenPlay = new Tenp(objectList);
                    tenPlay.FillShowsList();
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
                dlAbs.CleanEpisodes();
                tenPlay.SetActive();
            }  
            curState = State.DisplayingShows;
            curSite = Site.TenP;
            selectedShow = "";
            dlAbs = tenPlay;
        }

        /// <summary>
        /// Handles the action for when the RTVEClan button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRTVEC_Click(object sender, RoutedEventArgs e)
        {
            //First time selecting site
            if (rtveClan == null)
            {
                try
                {
                    rtveClan = new RTVEc(objectList);
                    rtveClan.FillShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get Episode listings for RTVE Clan";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.RTVEClan)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                rtveClan.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.RTVEClan;
            selectedShow = "";
            dlAbs = rtveClan;
        }
        /// <summary>
        /// Handles the actions for when the Plus7 button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPlus7_Click(object sender, RoutedEventArgs e)
        {
            //First time selecting site
            if (plus7 == null)
            {
                try
                {
                    plus7 = new Plus7(objectList);
                    plus7.FillShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get episode listings for Plus7";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.Plus7)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                plus7.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.Plus7;
            selectedShow = "";
            dlAbs = plus7;
        }

        /// <summary>
        /// Handles the actions for when the RTE button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRTE_Click(object sender, RoutedEventArgs e)
        {
            //First time selecting site
            if (rteIE == null)
            {
                try
                {
                    rteIE = new RTE(objectList);
                    rteIE.FillShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get episode listings for RTE";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.RTE)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                rteIE.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.RTE;
            selectedShow = "";
            dlAbs = rteIE;
        }

        private void ButtonDPlay_Click(object sender, RoutedEventArgs e)
        {
            //First time selecting site
            if (dplay == null)
            {
                try
                {
                    dplay = new DPlay(objectList);
                    dplay.FillShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get episode listings for DPlay";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.DPlay)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                dplay.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.DPlay;
            selectedShow = "";
            dlAbs = dplay;
        }

        private void ButtonCCMA_Click(object sender, RoutedEventArgs e)
        {
            //First time selecting site
            if (tv3CatCCMA == null)
            {
                try
                {
                    tv3CatCCMA = new TV3Cat(objectList);
                    tv3CatCCMA.FillShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get episode listings for CCMA (TV3)";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.TV3Cat)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                tv3CatCCMA.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.TV3Cat;
            selectedShow = "";
            dlAbs = tv3CatCCMA;
        }

        private void ButtonSuper3_Click(object sender, RoutedEventArgs e)
        {
            //First time selecting site
            if (super3 == null)
            {
                try
                {
                    super3 = new Super3(objectList);
                    super3.FillShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get episode listings for Super3";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.Super3)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                super3.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.Super3;
            selectedShow = "";
            dlAbs = super3;
        }

        /// <summary>
        /// Handles the action for when the Setting button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            settingsWindow.Show();
        }

        

        
    }
}
