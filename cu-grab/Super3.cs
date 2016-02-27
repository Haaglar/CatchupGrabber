using cu_grab.Shows.Super3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Controls;

namespace cu_grab
{
    class Super3 : DownloadAbstract
    {
        private ShowsSuper3 showsS3;
        private List<EpisodesGeneric> episodesS3 = new List<EpisodesGeneric>();
        private static string jsonMP4Url = "http://dinamics.ccma.cat/pvideo/media.jsp?media=video&version=0s&idint=";
        public Super3(ListBox lBoxContent) : base(lBoxContent) { }

        public override void CleanEpisodes()
        {
            episodesS3.Clear();
            listBoxContent.ItemsSource = showsS3.resposta.items.item;
        }
        public override string ClickDisplayedShow()
        {
            //TODO: Replace page crawl with 
            String showsPage;
            String selectedName = showsS3.resposta.items.item[listBoxContent.SelectedIndex].titol;
            //Its an rss feed yay
            Regex getEpisodes = new Regex(@"<h2>(.*?)<\/h2>\s*<span>([0-9]+)<\/span>");
            using (WebClient wc = new WebClient())
            {
                showsPage = wc.DownloadString(showsS3.resposta.items.item[listBoxContent.SelectedIndex].url);
            }
            MatchCollection matches = getEpisodes.Matches(showsPage);
            for (int i = 0; i < matches.Count; i++) //First two are useless
            {
                episodesS3.Add(new EpisodesGeneric(matches[i].Groups[1].Value, matches[i].Groups[2].Value));
            }
            listBoxContent.ItemsSource = episodesS3;
            return selectedName;
        }
        public override void FillShowsList()
        {
            String showsJson;
            //Regex filter = new Regex(@"<title>(.*?)<\/title>\n<link>(.*?)<\/link>");
            using (WebClient wc = new WebClient())
            {
                showsJson = wc.DownloadString(@"http://dinamics.ccma.cat/feeds/super3/programes.jsp");
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            showsS3 = jss.Deserialize<ShowsSuper3>(showsJson);
            showsS3.resposta.items.item = showsS3.resposta.items.item.OrderBy(x => x.titol).ToList();
            listBoxContent.ItemsSource = showsS3.resposta.items.item;
        }
        public override string GetSelectedName()
        {
            return episodesS3[listBoxContent.SelectedIndex].Name;
        }
        public override string GetSubtitles()
        {
            return "";
        }
        /// <summary>
        /// Gets the url
        /// </summary>
        /// <returns></returns>
        public override DownloadObject GetDownloadObject()
        {
            String jsonMP4;
            using (WebClient wc = new WebClient())
            {
                //Last bit not actually needed but whatever
                jsonMP4 = wc.DownloadString(jsonMP4Url + episodesS3[listBoxContent.SelectedIndex].EpisodeID + "&profile=pc");
            }
            Regex getMp4 = new Regex(@"""(.*?\.mp4)""", RegexOptions.RightToLeft); //Cause this way is the best
            Match mp4 = getMp4.Match(jsonMP4);
            return new DownloadObject( mp4.Groups[1].Value, GetSubtitles(), Country.Spain,DownloadMethod.HTTP);
        }
        public override void SetActive()
        {
            listBoxContent.ItemsSource = showsS3.resposta.items.item;
        }
    }
}
