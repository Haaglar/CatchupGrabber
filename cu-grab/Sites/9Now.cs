using cu_grab.EpisodeObjects._9Now;
using cu_grab.Shows._9Now;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace cu_grab
{
    class _9Now : DownloadAbstract
    {
        private Shows9Now shows9N;
        private Episodes9Now episodes9N;
        private string episodeUrlP1 = "https://tv-api.9now.com.au/v1/pages/tv-series/";
        private string episodeUrlP2 = "?device=android";
        public _9Now() { }

        public override void CleanEpisodes()
        {
            episodes9N = null;
        }

        public override string ClickDisplayedShow(int selectedIndex)
        {
            string jsonContentEpisodes;
            string slug = shows9N.items[selectedIndex].slug;
            using (WebClient webClient = new WebClient())
            {
                jsonContentEpisodes = webClient.DownloadString(episodeUrlP1 + slug + episodeUrlP2);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            episodes9N = jss.Deserialize<Episodes9Now>(jsonContentEpisodes);
            return shows9N.items[selectedIndex].name;
        }

        public override void FillShowsList()
        {
            //Device can be anything, but the android app uses so well aswell
            string address = "https://tv-api.9now.com.au/v1/tv-series?device=android&take=99999";
            string jsonContent;
            using (WebClient webClient = new WebClient())
            {
                jsonContent = webClient.DownloadString(address);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            shows9N = jss.Deserialize<Shows9Now>(jsonContent);
        }

        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override List<object> GetEpisodesList()
        {
            return episodes9N.items[0].items.ToList<object>();
        }

        public override string GetSelectedNameEpisode(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override List<object> GetShowsList()
        {
            return shows9N.items.ToList<object>();
        }

        public override string GetSubtitles()
        {
            return "";
        }
    }
}
