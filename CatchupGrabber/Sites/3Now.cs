using CatchupGrabber.EpisodeObjects._3Now;
using CatchupGrabber.ShowObjects._3Now;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CatchupGrabber
{
    class _3Now : DownloadAbstract
    {
        private static string SHOWS_URL = "http://now-api.mediaworks.nz/now-api/v2/shows";
        private static string EPISODES_URL = "http://now-api.mediaworks.nz/now-api/v2/show/";

        Shows3Now shows;
        Episodes3Now episodes;

        public override void CleanEpisodes()
        {
            throw new NotImplementedException();
        }

        public override void ClickDisplayedShow(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override void FillShowsList()
        {
            //Stock standard request
            string jsonContent;
            using (WebClient webClient = new WebClient())
            {
                jsonContent = webClient.DownloadString(SHOWS_URL);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            shows = jss.Deserialize<Shows3Now>(jsonContent);
            ShowListCacheValid = true;
        }

        public override string GetDescriptionEpisode(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            return shows.shows[selectedIndex].synopsis;
        }

        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override List<object> GetEpisodesList()
        {
            throw new NotImplementedException();
        }

        public override string GetSelectedEpisodeName(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override string GetSelectedShowName(int selectedIndex)
        {
            return shows.shows[selectedIndex].name;
        }

        public override List<object> GetShowsList()
        {
            return shows.shows.ToList<object>();
        }

        public override string GetSubtitles()
        {
            return "";
        }
    }
}
