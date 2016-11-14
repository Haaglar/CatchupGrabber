using CatchupGrabber.MiscObjects.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace CatchupGrabber
{
    //TODO: rewrite to use api
    class TV3Cat : DownloadAbstract
    {
        private List<ShowsGeneric> showList;
        private List<EpisodesGeneric> episodeList;

        private static string EpisodeJsonUrl = @"http://dinamics.ccma.cat/pvideo/media.jsp?media=video&version=0s&idint=";
        private static string ShowListing = @"http://dinamics.ccma.cat/feeds/programes/llistatProgramesPU.jsp?page=1&pageItems=1000&device=and-xh";
        private static string EpisodeURLP1 = @"http://dinamics.ccma.cat/feeds/videos/llistatVideosServeiPU.jsp?type=videosprog&id=";
        private static string EpisodeURLP2 = @"&page=1&pageItems=1000&device=and-xh&range=24h";

        private static string EpisodeJsonUrlGet = @"&profile=pc";

        public TV3Cat ()  { }

        /// <summary>
        /// Fills the listbox with the content on the programes list
        /// </summary>
        public override void FillShowsList()
        {
            showList = new List<ShowsGeneric>();
            string websiteShowList;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.GetEncoding("iso-8859-1"); //Non english encoding so qw use Latin alphabet no. 1
                websiteShowList = webClient.DownloadString(ShowListing);
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(websiteShowList); // Load the XML 

            XmlNodeList titleList = xmlDoc.GetElementsByTagName("item");
            foreach(XmlNode item in titleList)
            {
                string name = item.SelectSingleNode("titol").InnerText;
                string id  = item.Attributes["idint"].Value;
                string url = item.SelectSingleNode("url").InnerText;
                if(url.Contains("/super3/")) // We handle super3 elsewhere
                {
                    continue;
                }
                showList.Add(new ShowsGeneric(name, id));
            }
            ShowListCacheValid = true;
        }

        public override void ClickDisplayedShow(int selectedIndex)
        {
            episodeList = new List<EpisodesGeneric>();
            string showEpisodeList;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.GetEncoding("iso-8859-1"); //Non english encoding so qw use Latin alphabet no. 1
                showEpisodeList = webClient.DownloadString(EpisodeURLP1 + showList[selectedIndex].url +EpisodeURLP2);
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(showEpisodeList); // Load the XML 

            XmlNodeList itemList = xmlDoc.GetElementsByTagName("item");
            foreach (XmlNode item in itemList)
            {
                string name = item.SelectSingleNode("titol").InnerText;
                string id = item.Attributes["idint"].Value;
                string description = item.SelectSingleNode("entradeta").InnerText;
                episodeList.Add(new EpisodesGeneric(name,id,description));
            }
        }

        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            string pageJson;

            string refID = episodeList[selectedIndex].EpisodeID;

            //Download json
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                pageJson = webClient.DownloadString(EpisodeJsonUrl + refID + EpisodeJsonUrlGet);
            }
            Regex getMp4 = new Regex(@"""(.*?\.mp4)""", RegexOptions.RightToLeft); //Cause this way is the best
            Match mp4 = getMp4.Match(pageJson);

            Regex getSubs = new Regex(@"""(.*?\.xml)""", RegexOptions.RightToLeft);
            Match subMatch = getSubs.Match(pageJson);
            string suburl = subMatch.Groups[1].Value ?? "";
            return new DownloadObject(mp4.Groups[1].Value, suburl, Country.Spain,DownloadMethod.HTTP);
        }
        public override string GetSelectedEpisodeName(int selectedIndex)
        {
            return episodeList[selectedIndex].Name;
        }
        public override void CleanEpisodes()
        {
            episodeList.Clear();
        }

        public override string GetSubtitles()
        {
            return "";
        }
        public override List<object> GetShowsList()
        {
            return showList.ToList<object>();
        }
        public override List<object> GetEpisodesList()
        {
            return episodeList.ToList<object>();
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            return null;
        }

        public override string GetDescriptionEpisode(int selectedIndex)
        {
            return episodeList[selectedIndex].Description;
        }

        public override string GetSelectedShowName(int selectedIndex)
        {
            return showList[selectedIndex].name;
        }
    }
}
