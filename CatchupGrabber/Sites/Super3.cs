using CatchupGrabber.EpisodeObjects.Super3;
using CatchupGrabber.MiscObjects.Super3;
using CatchupGrabber.Shows.Super3;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace CatchupGrabber
{
    class Super3 : DownloadAbstract
    {
        private ShowsSuper3 showsS3;
        private EpisodesSuper3 episodesS3;
        //Contains the current download urls
        private Super3Json dlJSON;
        //private List<EpisodesGeneric> episodesS3 = new List<EpisodesGeneric>();
        private static string jsonMP4Url = "http://dinamics.ccma.cat/pvideo/media.jsp?media=video&version=0s&idint=";
        //private static string searchUrlP1 = "http://www.super3.cat/searcher/super3/searching.jsp?format=MP4&catBusca=";
        //private static string searchUrlP2 = "&presentacion=xml&pagina=1&itemsPagina=52";
        public Super3() { }

        public override void CleanEpisodes()
        {
            episodesS3 = null;
            dlJSON = null;
        }
        /// <summary>
        /// Gets The displayed episodes for the clicked show
        /// </summary>
        /// <returns>The selected show</returns>
        public override void ClickDisplayedShow(int selectedIndex)
        {
            string showsPage;
            string selectedUrl;
            string selectedName = showsS3.resposta.items.item[selectedIndex].titol;
            if (selectedName.EndsWith("(VO)"))
            {
                selectedUrl = "http://dinamics.ccma.cat/feeds/super3/videos.jsp?idioma=ANGLES&programa_id=" + showsS3.resposta.items.item[selectedIndex].id;
            }
            else
            {
                selectedUrl = "http://dinamics.ccma.cat/feeds/super3/videos.jsp?idioma=CATALA&programa_id=" + showsS3.resposta.items.item[selectedIndex].id;
            }
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.GetEncoding("iso-8859-1");
                showsPage = wc.DownloadString(selectedUrl);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            episodesS3 = jss.Deserialize<EpisodesSuper3>(showsPage);
        }

        public override void FillShowsList()
        {
            string showsJson;
            //Get Catalan
            using (WebClient wc = new WebClient())
            {
                showsJson = wc.DownloadString(@"http://dinamics.ccma.cat/feeds/super3/programes.jsp");
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            showsS3 = jss.Deserialize<ShowsSuper3>(showsJson);

            //Get English
            using (WebClient wc = new WebClient())
            {
                showsJson = wc.DownloadString(@"http://dinamics.ccma.cat/feeds/super3/programes.jsp?filtre=progangles");
            }
            ShowsSuper3 tmp = jss.Deserialize<ShowsSuper3>(showsJson);

            //Temp fix to iding names super3 
            foreach (var value in tmp.resposta.items.item)
            {
                value.titol += " (VO)";
            }

            //Concat and update to one big number
            showsS3.resposta.items.item = showsS3.resposta.items.item.Concat(tmp.resposta.items.item).ToList();
            showsS3.resposta.items.num += tmp.resposta.items.num;

            showsS3.resposta.items.item = showsS3.resposta.items.item.OrderBy(x => x.titol).ToList();
            ShowListCacheValid = true;
        }
        public override string GetSelectedEpisodeName(int selectedIndex)
        {
            return episodesS3.resposta.items.item[selectedIndex].titol;
        }
        public override string GetSubtitles()
        {
            if(dlJSON == null)
            {
                return "";
            }
            else if(dlJSON.subtitols == null || dlJSON.subtitols.url == null)
            {
                return "";
            }
            return dlJSON.subtitols.url;
        }
        /// <summary>
        /// Gets the url
        /// </summary>
        /// <returns></returns>
        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            string jsonMP4;
            using (WebClient wc = new WebClient())
            {
                //Last bit not actually needed but whatever, it uses android on the app
                jsonMP4 = wc.DownloadString(jsonMP4Url + episodesS3.resposta.items.item[selectedIndex].id + "&profile=pc");
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            dlJSON = jss.Deserialize<Super3Json>(jsonMP4);
            return new DownloadObject(dlJSON.media.url, GetSubtitles() , Country.Spain,DownloadMethod.HTTP);
        }

        public override List<object> GetShowsList()
        {
            return showsS3.resposta.items.item.ToList<object>();
        }
        public override List<object> GetEpisodesList()
        {
            return episodesS3.resposta.items.item.ToList<object>();
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            return null;
        }

        public override string GetDescriptionEpisode(int selectedIndex)
        {
            return episodesS3.resposta.items.item[selectedIndex].entradeta;
        }

        public override string GetSelectedShowName(int selectedIndex)
        {
            return showsS3.resposta.items.item[selectedIndex].titol;
        }
    }
}
