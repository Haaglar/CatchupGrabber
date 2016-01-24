using cu_grab.Downloader.RTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Controls;

namespace cu_grab
{
    public class RTE : DownloadAbstract
    {
        private const String EpisodePlaylistUrl = @"https://www.rte.ie/rteavgen/getplaylist/?type=web&format=json&id=";
        private const String CndUrl = @"http://cdn.rasset.ie";
        private List<RTEShows> rteShows;
        private List<Episodes> selectedShowEpisodes = new List<Episodes>();

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="lBoxContent">The ListBox in which the content is displayed in</param>
        public RTE(ListBox lBoxContent) : base(lBoxContent){}
        /// <summary>
        /// Fills the listbox with the JSON from rte search
        /// </summary>
        public override void FillShowsList()
        {
            WebRequest reqSearchJs = HttpWebRequest.Create(@"https://www.rte.ie/player/shows.js?v=2");
            WebResponse resSearchJs = reqSearchJs.GetResponse();

            using (StreamReader srjs = new StreamReader(resSearchJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd(); 
                int jslen = jsonjs.Length;
                jsonjs = jsonjs.Substring(12, jslen - 12); // remove the "var shows = "

                JavaScriptSerializer jss = new JavaScriptSerializer();
                rteShows = jss.Deserialize<List<RTEShows>>(jsonjs);
                rteShows = rteShows.OrderBy(x => x.v).ToList(); //Order By name 
            }
            listBoxContent.ItemsSource = rteShows;
            resSearchJs.Close();
        }
        /// <summary>
        /// Sets the ListBox to RTE
        /// </summary>
        public override void SetActive()
        {
            listBoxContent.ItemsSource = rteShows;
        }
        /// <summary>
        /// Handles the click of a ListBox object
        /// </summary>
        /// <returns></returns>
        public override string ClickDisplayedShow()
        {
            //Get page content
            String pageShow;
            String proxyAddress = Properties.Settings.Default.HTTPProxySettingRTE;
            String url = "http://www.rte.ie/player/lh/show/" + rteShows[listBoxContent.SelectedIndex].id;
            //Use a web client here for the proxy option as you cannot view get the episode list without having a IE address
            //Also Glype or PHProxy proxies do not seem to work either
            using (WebClient webClient = new WebClient())
            {
                if (proxyAddress != "")//If they suplied a proxy
                {
                    WebProxy proxy = new WebProxy(proxyAddress);
                    webClient.Proxy = proxy;
                }
                pageShow = webClient.DownloadString(new System.Uri(url));
            }
           
            //Crop stuff
            int startPoint = pageShow.IndexOf("main-content-box clearfix"); // Start of episode list (two occurances, want first anyway)
            int endPoint = pageShow.IndexOf("main-content-box-container  black");   //recommended rubbish
            pageShow = pageShow.Substring(startPoint, endPoint - startPoint);

            //Regex searches
            Regex regName = new Regex(@"thumbnail-title"">(.*)<");
            Regex regID = new Regex(@"href="".*/([0-9].*)/");

            MatchCollection matchName = regName.Matches(pageShow);
            MatchCollection matchID = regID.Matches(pageShow);

            //Add episodes to list
            foreach (Tuple<Match, Match> match in matchName.Cast<Match>().Zip(matchID.Cast<Match>(), Tuple.Create)) //Join the two in a tuple
            {
                String description = match.Item1.Groups[1].Value;
                String ID = match.Item2.Groups[1].Value;
                selectedShowEpisodes.Add(new Episodes(description, ID));
            }
            String selectedShow = rteShows[listBoxContent.SelectedIndex].v;
            //Clean the name for windows
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                selectedShow = selectedShow.Replace(c, '-');
            }
            listBoxContent.ItemsSource = selectedShowEpisodes;
            return selectedShow;
        }
        /// <summary>
        /// Gets the url for the selected episode
        /// </summary>
        /// <returns>The highest bitrate url for the seleected episode</returns>
        public override string GetUrl()
        {
            //Construct URL
            String urlJson = EpisodePlaylistUrl +  selectedShowEpisodes[listBoxContent.SelectedIndex].EpisodeID;
            String showJsonString;
            WebRequest reqShow = HttpWebRequest.Create(urlJson);
            using (WebResponse resShowUrl = reqShow.GetResponse())
            {
                using (Stream responseStreamUrl = resShowUrl.GetResponseStream())
                {
                    using (StreamReader srShowUrl = new StreamReader(responseStreamUrl, System.Text.Encoding.UTF8))
                    {
                        showJsonString= srShowUrl.ReadToEnd();
                    }
                }
            }
            //Get the hls url
            Regex getHlsUrl = new Regex(@"hls_url""\: ""(.*?)""");
            String urlSuffix = getHlsUrl.Matches(showJsonString)[0].Groups[1].Value;
            String manifestHlsUrl = CndUrl + "/manifest" + urlSuffix;

            return GetHighestBitrate(manifestHlsUrl);
            
        }
        /// <summary>
        /// Gets the highest bitrate from a m3u8 link
        /// </summary>
        /// <param name="url">URL of m3u8</param>
        /// <returns>The highest bitrate redition URL</returns>
        private String GetHighestBitrate(String url)
        {
            WebRequest reqManifest = HttpWebRequest.Create(url);
            using (WebResponse resManifest = reqManifest.GetResponse())
            {
                using (Stream responseStreamManifest = resManifest.GetResponseStream())
                {
                    using (StreamReader srShowManifest = new StreamReader(responseStreamManifest, System.Text.Encoding.UTF8))
                    {
                        String line; // current line 
                        String finalUrl = "";
                        Regex regexBandwidth = new Regex(@"(?<=\bBANDWIDTH=)([0-9]+)"); //Quality Selection
                        int index = 0;
                        int row = -1;
                        long bandwidth = 0;
                        long tmp = 0;
                        //Get the highest quality link
                        while ((line = srShowManifest.ReadLine()) != null)
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
                        return CndUrl + finalUrl; // Its a relative adress
                    }
                }
            }
           
        }
        public override void CleanEpisodes()
        {
            selectedShowEpisodes.Clear();
            listBoxContent.ItemsSource = rteShows;
        }
        public override string GetSelectedName()
        {
            return selectedShowEpisodes[listBoxContent.SelectedIndex].Name;
        }
        public override string GetSubtitles()
        {
            return "";
        }
    }
}
