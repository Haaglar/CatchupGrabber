using CatchupGrabber.MiscObjects;
using CatchupGrabber.NetworkAssister;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace CatchupGrabber
{
    class SVTse : DownloadAbstract
    {
        private static string BaseURL = "http://www.svtplay.se";
        private static string ShowListURL = "/program";
        private static string AdditionalEpisodes = @"?sida=2&tab=helaprogram&embed=true";
        
        private List<ShowsGeneric> showsSVT = new List<ShowsGeneric>();
        private List<EpisodesGeneric> episodesSVT = new List<EpisodesGeneric>();
        private SVTJson episodeData;
        private CUNetworkAssist netAssist = new CUNetworkAssist();

        public SVTse(){ }
        public override void CleanEpisodes()
        {
            episodesSVT.Clear();
        }
        public override void ClickDisplayedShow(int selectedIndex)
        {
            string websiteShowList;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8; //Webpage encoding
                websiteShowList = webClient.DownloadString(showsSVT[selectedIndex].url);
            }
            //Regex the json data
            Regex showsSearch = new Regex(@"""url"":""([^""]*)"",""programVersionId"".*?""title"":""([^""]*)""");
            MatchCollection matchesOne = showsSearch.Matches(websiteShowList);
            foreach (Match match in matchesOne)
            {
                episodesSVT.Add(new EpisodesGeneric( WebUtility.HtmlDecode(match.Groups[2].Value), BaseURL + match.Groups[1].Value));
            }
        }
        public override void FillShowsList()
        {
            string websiteShowList;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8; //Webpage encoding
                websiteShowList = webClient.DownloadString(BaseURL + ShowListURL);
            }
            websiteShowList = WebUtility.HtmlDecode(websiteShowList);// Since its got swedish characterse
            // Theres some json in the page that contains usefull information about the shows
            // But it has wierdly named attributes, and will require regex editing to remove it
            //So i just regex to get the data from it           
            Regex abuseShows = new Regex(@"""title"":""([^""]*)"",""urlFriendlyTitle"":""([^""]*)""");
            MatchCollection showsCol = abuseShows.Matches(websiteShowList);
            foreach(Match match in showsCol)
            {
                showsSVT.Add(new ShowsGeneric( match.Groups[1].Value,BaseURL + "/"+ match.Groups[2].Value));
            }
            RequestedSiteData = true;
        }
        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            string jsonData;
            Regex regRefId = new Regex(@"/([0-9]+)/");
            string value = regRefId.Match(episodesSVT[selectedIndex].EpisodeID).Groups[1].Value;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8; //Webpage encoding Get some Json Data
                string d = BaseURL + @"/video/" + value + "?output=json";
                jsonData = webClient.DownloadString(d);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            episodeData = jss.Deserialize<SVTJson>(jsonData);
            string url = episodeData.video.videoReferences.Single(v => v.playerType.Equals("ios")).url;
            url = url.Substring(0, url.IndexOf("m3u8") + 4);
            string urlnew = netAssist.GetHighestM3U8Address(url);
            url = new Uri(new Uri(url), urlnew).ToString();
            return new DownloadObject(url,GetSubtitles(), Country.Sweden,DownloadMethod.HLS);
        }
        public override string GetSelectedEpisodeName(int selectedIndex)
        {
            return episodesSVT[selectedIndex].Name;
        }

        public override string GetSubtitles()
        {
            if (episodeData.video.subtitleReferences != null)
            {
                string full = episodeData.video.subtitleReferences[0].url;
                if (!string.IsNullOrEmpty(full))
                {
                    int end = full.LastIndexOf("/");
                    full = full.Substring(0, end) + "/all.vtt"; ;
                }
                return full;
            }
            return "";
        }

        public override List<object> GetShowsList()
        {
            return showsSVT.ToList<object>();
        }
        public override List<object> GetEpisodesList()
        {
            return episodesSVT.ToList<object>();
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            return null;
        }

        public override string GetDescriptionEpisode(int selectedIndex)
        {
            return null;
        }

        public override string GetSelectedShowName(int selectedIndex)
        {
            return showsSVT[selectedIndex].name;
        }
    }
}
