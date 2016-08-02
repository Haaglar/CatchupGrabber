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

        public override void CleanEpisodes()
        {
            episodesList = null;
        }

        public override void ClickDisplayedShow(int selectedIndex)
        {
            throw new NotImplementedException();
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
                string id =  node.SelectNodes("//link")[1].Attributes["href"].Value;
                showList.Add(new ShowsGeneric(name, id, desc));
            }
            ShowListCacheValid = true;
        }

        public override string GetDescriptionEpisode(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            return showList[selectedIndex].description;
        }

        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            throw new NotImplementedException();
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
