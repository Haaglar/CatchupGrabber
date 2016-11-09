using CatchupGrabber.EpisodeObjects._3Now;
using CatchupGrabber.MiscObjects.EnumValues;
using CatchupGrabber.NetworkAssister;
using CatchupGrabber.ShowObjects._3Now;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            episodes = null;
        }

        public override void ClickDisplayedShow(int selectedIndex)
        {
            //Stock standard request
            string jsonContent;
            using (WebClient webClient = new WebClient())
            {
                jsonContent = webClient.DownloadString(EPISODES_URL + shows.shows[selectedIndex].showId);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            episodes = jss.Deserialize<Episodes3Now>(jsonContent);
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
            return episodes.show.episodes[selectedIndex].synopsis;
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            return shows.shows[selectedIndex].synopsis;
        }

        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            CUNetworkAssist cuA = new CUNetworkAssist(); 
            return new DownloadObject(cuA.GetHighestM3U8Address(episodes.show.episodes[selectedIndex].videoRenditions.videoCloud.hlsUrl),GetSubtitles(),Country.NewZealand, DownloadMethod.HLS);
        }

        public override List<object> GetEpisodesList()
        {
            return episodes.show.episodes.ToList<object>();
        }

        public override string GetSelectedEpisodeName(int selectedIndex)
        {
            return episodes.show.episodes[selectedIndex].name;
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

        public override string GetImageURLShow(int index)
        {
            try
            {
                return shows.shows[index].images.dashboardHero.Replace("[width]", "381").Replace("[height]", "286"); //Res of whats on the site
            }
            catch
            {
                return null;
            }
        }
        public override string GetImageURLEpisosde(int index)
        {
            try
            {
                return episodes.show.episodes[index].images.videoTile.Replace("[width]", "602").Replace("[height]", "340"); //Res of episodes
            }
            catch
            {
                return null;
            }
        }
    }
}
