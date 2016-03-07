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
using System.Xml.Linq;

namespace cu_grab
{
    class Super3 : DownloadAbstract
    {
        private ShowsSuper3 showsS3;
        private List<EpisodesGeneric> episodesS3 = new List<EpisodesGeneric>();
        private static string jsonMP4Url = "http://dinamics.ccma.cat/pvideo/media.jsp?media=video&version=0s&idint=";
        private static string searchUrlP1 = "http://www.super3.cat/searcher/super3/searching.jsp?format=MP4&catBusca=";
        private static string searchUrlP2 = "&presentacion=xml&pagina=1&itemsPagina=36";
        public Super3(ListBox lBoxContent) : base(lBoxContent) { }

        public override void CleanEpisodes()
        {
            episodesS3.Clear();
            listBoxContent.ItemsSource = showsS3.resposta.items.item;
        }
        /// <summary>
        /// Gets The displayed episodes for the clicked show
        /// Based off itorres info
        /// </summary>
        /// <returns>The selected show</returns>
        public override string ClickDisplayedShow()
        {
            String showsPage;
            String selectedName = showsS3.resposta.items.item[listBoxContent.SelectedIndex].titol;
            String selectedUrl = searchUrlP1 + showsS3.resposta.items.item[listBoxContent.SelectedIndex].bband.id + searchUrlP2; 
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.GetEncoding("iso-8859-1");
                showsPage = wc.DownloadString(selectedUrl);
            }
            XElement doc = XElement.Parse(showsPage);
            foreach (XElement element in doc.Element("resultats").Elements("item"))
            {
                episodesS3.Add(new EpisodesGeneric(element.Element("titol").Value, element.Attribute("idint").Value));
            }
            listBoxContent.ItemsSource = episodesS3;
            return selectedName;
        }

        public override void FillShowsList()
        {
            String showsJson;
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
            //Concat and update to one big number
            showsS3.resposta.items.item = showsS3.resposta.items.item.Concat(tmp.resposta.items.item).ToList();
            showsS3.resposta.items.num += tmp.resposta.items.num;

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
