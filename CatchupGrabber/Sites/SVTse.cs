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
        
        private List<ShowsGeneric> showsSVT;
        private List<EpisodesGeneric> episodesSVT = new List<EpisodesGeneric>();
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
            showsSVT = new List<ShowsGeneric>();
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
            ShowListCacheValid = true;
        }
        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            string pageData;
           
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8; //Webpage encoding Get some Json Data
                pageData = webClient.DownloadString(episodesSVT[selectedIndex].EpisodeID);
            }
            Regex findm3u8 = new Regex(@"""url"":""(.*?m3u8)""",RegexOptions.RightToLeft);
            string url = findm3u8.Match(pageData).Groups[1].Value;
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
