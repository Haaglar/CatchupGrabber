﻿using cu_grab.Downloader.RTE;
using cu_grab.NetworkAssister;
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
        private List<ShowsRTE> rteShows;
        private List<EpisodesGeneric> selectedShowEpisodes = new List<EpisodesGeneric>();
        private CUNetworkAssist netAssist = new CUNetworkAssist(); 

        /// <summary>
        /// Standard constructor
        /// </summary>
        public RTE() {}
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
                rteShows = jss.Deserialize<List<ShowsRTE>>(jsonjs);
                rteShows = rteShows.OrderBy(x => x.v).ToList(); //Order By name 
            }
            resSearchJs.Close();
        }

        /// <summary>
        /// Handles the click of a ListBox object
        /// </summary>
        /// <returns></returns>
        public override string ClickDisplayedShow(int selectedIndex)
        {
            //Get page content
            String pageShow;
            String proxyAddress = Properties.Settings.Default.HTTPIrish;
            String url = "http://www.rte.ie/player/lh/show/" + rteShows[selectedIndex].id;
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
                selectedShowEpisodes.Add(new EpisodesGeneric(description, ID));
            }
            String selectedShow = rteShows[selectedIndex].v;
            //Clean the name for windows
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                selectedShow = selectedShow.Replace(c, '-');
            }
            return selectedShow;
        }
        /// <summary>
        /// Gets the url for the selected episode
        /// </summary>
        /// <returns>The highest bitrate url for the seleected episode</returns>
        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            //Construct URL
            String urlJson = EpisodePlaylistUrl +  selectedShowEpisodes[selectedIndex].EpisodeID;
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
            return new DownloadObject(CndUrl + netAssist.GetHighestM3U8Address(manifestHlsUrl), GetSubtitles(), Country.Ireland,DownloadMethod.HLS);
            
        }
        public override void CleanEpisodes()
        {
            selectedShowEpisodes.Clear();
        }
        public override string GetSelectedNameShow(int selectedIndex)
        {
            return selectedShowEpisodes[selectedIndex].Name;
        }
        public override string GetSubtitles()
        {
            return "";
        }
        public override List<object> GetShowsList()
        {
            return rteShows.ToList<object>();
        }
        public override List<object> GetEpisodesList()
        {
            return selectedShowEpisodes.ToList<object>();
        }
    }
}
