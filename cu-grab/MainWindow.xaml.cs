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
     * Error handling!
     * Make a attractive GUI
     * Use API, for descriptions and stuff instead of crawling
     * Split into seperate classes
     * Other sites
     *      rtvs
     *          search Crawl api
     *          do stuff
     *      p7 maybe?   
     */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Regex regexID = new Regex(@"(?<=\bdata-video-id="")[^""]*");      //ID selection
        Regex regexBandwidth = new Regex(@"(?<=\bBANDWIDTH=)([0-9]+)"); //Quality Selection
        Regex idFind = new Regex(@"<div class=""content-card__image-container"">(.+?)</div>", RegexOptions.Singleline); //Finds all the episodes for the show

        Regex regexIDImage = new Regex(@"(?<=\bsrc="")[^""]*");      //ID from image
        Regex regexIDEpisode = new Regex(@"(?<=\balt="")[^""]*");      //Episode name from image

        Regex IDSplit = new Regex(@"([0-9]+)",RegexOptions.RightToLeft);

        String lcJsonUrl = @"http://www.rtve.es/infantil/buscador-clan/obtener-datos-programas.json";
        String p7JsonUrl = @"https://au.tv.yahoo.com/plus7/data/tv-shows/";

        String BC_URL = "http://c.brightcove.com/services/mobile/streaming/index/master.m3u8?videoId="; //url taken from and m3u8
        String PUB_ID = "&pubId=2199827728001"; //ID taken from any m3u8
        RootObject shows;
        List<Episode> selectedShowEpisodes;
        String tpURL = "http://tenplay.com.au";
        String selectedShow = "";
        enum State {DisplayingNone, DisplayingShows, DisplayingEpisodes};
        State curState = State.DisplayingNone;

        public MainWindow()
        {
            InitializeComponent();
            selectedShowEpisodes = new List<Episode>();
            fillShowsList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Location_Select(object sender, RoutedEventArgs e)
        {
            SaveFileDialog locationToSave = new SaveFileDialog();
            locationToSave.Filter = "MP4 Video|*.mp4";
            locationToSave.Title = "Choose location.";
            locationToSave.ShowDialog();
            file_to_save.Text = locationToSave.FileName;
        }
        /// <summary>
        /// Handle url supplied link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String theUrl = url_location.Text;
            Uri uri;
            String finalUrl = "";
            //Check if valid url and file save location
            if (file_to_save.Text != "" && (Uri.TryCreate(theUrl, UriKind.Absolute, out uri) || Uri.TryCreate("http://" + theUrl, UriKind.Absolute, out uri)))
            {
                WebRequest req = HttpWebRequest.Create(uri);
                WebResponse res = req.GetResponse();
                StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
                string page = sr.ReadToEnd();
                res.Close();
                sr.Close();
                //See if we found the ID
                MatchCollection matchID = regexID.Matches(page);
                if (matchID.Count > 0)
                {     
                    Debug.Print("Getting highes");
                    WebRequest reqm3u8 = HttpWebRequest.Create(BC_URL + matchID[0].Value + PUB_ID);
                    WebResponse resm3u8 = reqm3u8.GetResponse();
                    StreamReader srm3u8 = new StreamReader(resm3u8.GetResponseStream(), System.Text.Encoding.UTF8);
                    String line;
                    // TODO: Write this section better
                    int index = 0;
                    int row = -1;
                    long bandwidth = 0;
                    long tmp=0;
                    //Get the highest quality link
                    while ((line = srm3u8.ReadLine()) != null)
                    {
                        Debug.Print("Getting highes");
                        if(row == index)
                        {
                            finalUrl = line;
                        }
                        index++;
                         MatchCollection matchBand = regexBandwidth.Matches(line);
                         if (matchBand.Count > 0)
                        {
                            tmp = int.Parse(matchBand[0].Value);
                            if (tmp > bandwidth)
                            {
                                row = index;
                                bandwidth = tmp; 
                            }
                        }
                    }
                    errorLabel.Text = finalUrl;
                    resm3u8.Close();
                    srm3u8.Close();
                    
                }  
               //errorLabel.Content 
            }
            runFFmpeg(finalUrl, file_to_save.Text);    
        }
        /// <summary>
        /// Double click of list item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void oL_ItemPressed(object sender, MouseButtonEventArgs e)
        {
            switch (curState)
            {
                case State.DisplayingShows:
                    clickDisplayedShow();
                    break;
                case State.DisplayingEpisodes:
                    clickDisplayedEpisode();
                    break;
            }
        }

        /// <summary>
        /// Fillthe ListBox with the shows currently on Tenplay found from the search JSON.
        /// </summary>
        public void fillShowsList()
        {

            WebRequest reqSearchJs = HttpWebRequest.Create("http://tenplay.com.au/web%20api/showsearchjson");
            WebResponse resSearchJs = reqSearchJs.GetResponse();
            
            using (StreamReader srjs = new StreamReader(resSearchJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                shows = jss.Deserialize<RootObject>(jsonjs);
                //shows = JsonConvert.DeserializeObject<RootObject>(jsonjs);
                shows.Shows = shows.Shows.OrderBy(x => x.Name).ToList();
                objectList.ItemsSource = shows.Shows;
            }
            curState = State.DisplayingShows;
            resSearchJs.Close();
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
        /// Handles the double click of 
        /// </summary>
        public void clickDisplayedShow()
        {
            WebRequest reqShow = HttpWebRequest.Create("http://tenplay.com.au" + shows.Shows[objectList.SelectedIndex].ShowURL);
            WebResponse resShow = reqShow.GetResponse();

            StreamReader srShow = new StreamReader(resShow.GetResponseStream(), System.Text.Encoding.UTF8);
            string pageShow = srShow.ReadToEnd();
            //Get and iterate over the episodes divs
            foreach (Match matchID in idFind.Matches(pageShow))
            {
                //get src ID and episode name
                MatchCollection matchIdImage = regexIDImage.Matches(matchID.Value);
                MatchCollection matchIdName = regexIDEpisode.Matches(matchID.Value);
                if (matchIdImage.Count > 0)
                {
                    String valueFull = matchIdImage[0].Value;
                    String split = valueFull.Split('?')[1];
                    String final = IDSplit.Matches(split)[0].Value;
                    selectedShowEpisodes.Add(new Episode(matchIdName[0].Value, final));
                }
            }
            //Store the current show name for file naming later
            selectedShow = shows.Shows[objectList.SelectedIndex].Name;
            //Clean the name for windows
            foreach (var c in System.IO.Path.GetInvalidFileNameChars()) 
            {
                selectedShow = selectedShow.Replace(c, '-'); 
            }
            //Update list and states
            objectList.ItemsSource = selectedShowEpisodes;
            curState = State.DisplayingEpisodes;

            resShow.Close();
            srShow.Close();
        }
        /// <summary>
        /// Hanlde clicking on an episode
        /// </summary>
        public void clickDisplayedEpisode()
        {
            // Get standard m3u8from
            WebRequest reqm3u8 = HttpWebRequest.Create(BC_URL + selectedShowEpisodes[objectList.SelectedIndex].EpisodeID + PUB_ID);
            WebResponse resm3u8 = reqm3u8.GetResponse();
            StreamReader srm3u8 = new StreamReader(resm3u8.GetResponseStream(), System.Text.Encoding.UTF8);

            String url = getHighestm3u8(srm3u8);
            runFFmpeg(url, selectedShow + " " + selectedShowEpisodes[objectList.SelectedIndex].Name);
            resm3u8.Close();
            srm3u8.Close();

        }
        /// <summary>
        /// Gets the highest rendition from a master m3u8
        /// </summary>
        /// <param name="m3u8"></param>
        /// <returns></returns>
        public String getHighestm3u8(StreamReader m3u8)
        {
            String line; // current line 
            String finalUrl ="";
            // TODO: Write this section better
            int index = 0;
            int row = -1;
            long bandwidth = 0;
            long tmp = 0;
            //Get the highest quality link
            while ((line = m3u8.ReadLine()) != null)
            {
                if (row == index)
                {
                    finalUrl = line;
                }
                index++;
                MatchCollection matchBand = regexBandwidth.Matches(line);
                if (matchBand.Count > 0)
                {
                    tmp = int.Parse(matchBand[0].Value);
                    if (tmp > bandwidth)
                    {
                        row = index;
                        bandwidth = tmp;
                    }
                }
            }
            return finalUrl;

        }
        /// <summary>
        /// Cleanup function for returning to shows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Shows_Pressed(object sender, RoutedEventArgs e)
        {
            selectedShowEpisodes.Clear();
            curState = State.DisplayingShows;
            objectList.ItemsSource = shows.Shows;
            selectedShow = "";
        }
    }
}
