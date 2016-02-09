using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace cu_grab
{
    class Super3 : DownloadAbstract
    {
        private List<ShowsGeneric> showsS3 = new List<ShowsGeneric>();
        private List<EpisodesGeneric> episodesS3 = new List<EpisodesGeneric>();
        private static string jsonMP4Url = "http://dinamics.ccma.cat/pvideo/media.jsp?media=video&version=0s&idint=";
        public Super3(ListBox lBoxContent) : base(lBoxContent) { }

        public override void CleanEpisodes()
        {
            episodesS3.Clear();
            listBoxContent.ItemsSource = showsS3;
        }
        public override string ClickDisplayedShow()
        {
            //TODO: Replace page crawl with 
            String showsPage;
            String selectedName = showsS3[listBoxContent.SelectedIndex].name;
            //Its an rss feed yay
            Regex getEpisodes = new Regex(@"<h2>(.*?)<\/h2>\s*<span>([0-9]+)<\/span>");
            using (WebClient wc = new WebClient())
            {
                showsPage = wc.DownloadString(showsS3[listBoxContent.SelectedIndex].url);
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
            String showsXML;
            //Its an rss feed yay
            Regex filter = new Regex(@"<title>(.*?)<\/title>\n<link>(.*?)<\/link>");
            using (WebClient wc = new WebClient())
            {
                showsXML = wc.DownloadString(@"http://www.super3.cat/rss/sp3_serie_emissio_rss.xml");
            }
            MatchCollection matches = filter.Matches(showsXML);
            for(int i = 2; i < matches.Count; i++) //First two are useless
            {
                showsS3.Add(new ShowsGeneric(matches[i].Groups[1].Value, WebUtility.HtmlDecode(matches[i].Groups[2].Value)));
            }
            //rss is out of order
            showsS3 = showsS3.OrderBy(x=> x.name).ToList();
            listBoxContent.ItemsSource = showsS3;
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
        public override string GetUrl()
        {

            String jsonMP4;

            using (WebClient wc = new WebClient())
            {
                //Last bit not actually needed but whatever
                //
                jsonMP4 = wc.DownloadString(jsonMP4Url + episodesS3[listBoxContent.SelectedIndex].EpisodeID + "&profile=pc");
            }
            Regex getMp4 = new Regex(@"""(.*?\.mp4)""", RegexOptions.RightToLeft); //Cause this way is the best
            Match mp4 = getMp4.Match(jsonMP4);
            return mp4.Groups[1].Value;
        }
        public override void SetActive()
        {
            listBoxContent.ItemsSource = showsS3;
        }
    }
}
