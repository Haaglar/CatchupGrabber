using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CatchupGrabber
{
    class AmebaTV : DownloadAbstract
    {
        private List<ShowsGeneric> showList;
        private List<EpisodesGeneric> episodesList;

        private string addressShows = "http://www.amebatv.com/AmebaProxy?apireq=/catalog/videos/shows?pagesize=500&orderby=name";
        //Gets episodes of show
        private string addressEpisodep1 = "http://www.amebatv.com/AmebaProxy?apireq=/catalog/videos/series/";
        private string addressEpisodep2 = "/episodes?pagesize=52";

        //On episode click
        private string episodeMetap1 = "http://www.amebatv.com/AmebaProxy?apireq=/catalog/videos/index/";
        private string episodeMetap2 = "/streams?lang=en";

        public override void CleanEpisodes()
        {
            episodesList = null;
        }

        public override void ClickDisplayedShow(int selectedIndex)
        {
            string xmlData;
            episodesList = new List<EpisodesGeneric>();
            using (WebClient webClient = new WebClient())
            {
                xmlData = webClient.DownloadString(addressEpisodep1 + showList[selectedIndex].url + addressEpisodep2);
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlData);

            XmlNode root = doc.DocumentElement;
            XmlNodeList episodes = root.SelectNodes("//video");

            foreach (XmlNode node in episodes)
            {
                string name = node["titlefull"].InnerText;
                string desc = node["summaryfull"].InnerText;
                string id = node.Attributes["id"].Value;
                episodesList.Add(new EpisodesGeneric(name,id,desc));
            }

        }

        public override void FillShowsList()
        {
            string xmlData;
            showList = new List<ShowsGeneric>();
            using (WebClient webClient = new WebClient())
            {
                xmlData = webClient.DownloadString(addressShows);
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlData);

            XmlNode root = doc.DocumentElement;
            XmlNodeList series = root.SelectNodes("//series");
            foreach(XmlNode node in series)
            {

                string name = node["titlefull"].InnerText;
                string desc = node["summaryfull"].InnerText;
                string id = node.Attributes["id"].Value;
                showList.Add(new ShowsGeneric(name, id, desc));
            }
            ShowListCacheValid = true;
        }

        public override string GetDescriptionEpisode(int selectedIndex)
        {
            return episodesList[selectedIndex].Description;
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            return showList[selectedIndex].description;
        }

        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            string xmlData;
            using (WebClient webClient = new WebClient())
            {
                string proxy = Properties.Settings.Default.HTTPUSA;
                if(!string.IsNullOrWhiteSpace(proxy))
                {
                    webClient.Proxy = new WebProxy(proxy);
                }
                xmlData = webClient.DownloadString(episodeMetap1 + episodesList[selectedIndex].EpisodeID + episodeMetap2);
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlData);

            XmlNode root = doc.DocumentElement;
            XmlNodeList episodes = root.SelectNodes("//videostream[@deliverymethod=\"httpdownload\"]");
            int bitrate = 0;
            string dlurl = "";
            foreach(XmlNode vs in episodes)
            {
                int curBR = int.Parse(vs.Attributes["videobitrate"].Value);
                if (curBR > bitrate)
                {
                    bitrate = curBR;
                    dlurl = vs.Attributes["href"].Value;
                }
            }

            return new DownloadObject(dlurl,GetSubtitles(), Country.USA, DownloadMethod.HTTP);
        }

        public override List<object> GetEpisodesList()
        {
            return episodesList.ToList<object>();
        }

        public override string GetSelectedEpisodeName(int selectedIndex)
        {
            return episodesList[selectedIndex].Name;
        }

        public override string GetSelectedShowName(int selectedIndex)
        {
            return showList[selectedIndex].name;
        }

        public override List<object> GetShowsList()
        {
            return showList.ToList<object>() ;
        }

        public override string GetSubtitles()
        {
            return "";
        }
    }
}
