using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Web;

namespace cu_grab
{
    class TV3Cat : DownloadAbstract
    {
        public List<ShowsGeneric> showList {get;private set;} 
        private List<EpisodesGeneric> episodeList = new List<EpisodesGeneric>();

        private static string EpisodeJsonUrl = @"http://dinamics.ccma.cat/pvideo/media.jsp?media=video&version=0s&idint=";
        private static string EpisodeJsonUrlGet = @"&profile=pc";

        public TV3Cat (ListBox lBoxContent) : base(lBoxContent) { }

        /// <summary>
        /// Fills the listbox with the content on the programes list
        /// </summary>
        public override void FillShowsList()
        {
            showList = new List<ShowsGeneric>();
            String websiteShowList;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8; //Cause its system ansi by defualt and that screws up the text
                websiteShowList = webClient.DownloadString("http://www.ccma.cat/tv3/programes/");
            }
            Regex getContents = new Regex(@"a href=""(.*?)"">(.*?)<", RegexOptions.Singleline);
            //Cut the string from where the show list starts and ends
            //So we can abuse regex
            int beginIndex = websiteShowList.IndexOf(@"div class=""span9""");
            int endIndex = websiteShowList.IndexOf(@"project_id: modul-programesaz") - beginIndex;
            String cut = websiteShowList.Substring(beginIndex, endIndex); 
            MatchCollection entries = getContents.Matches(cut);
            foreach(Match entry in entries)
            {
                //We dont want super3.cat videos, handlethem elsewhere.
                //Cause the super3 videos here is incomplete and harder to handle
                if (entry.Groups[1].Value.StartsWith(@"http://www.super3.cat/")) continue;
                showList.Add(new ShowsGeneric(entry.Groups[2].Value.Trim(), entry.Groups[1].Value));
            }
        }

        public override string ClickDisplayedShow()
        {
            String urlSelectedTmp = showList[listBoxContent.SelectedIndex].url;
            String showName = showList[listBoxContent.SelectedIndex].name;  //Store it cause we swap listbox later
            String showEpisodeList;
            //Its a relative url
            if(urlSelectedTmp.StartsWith("/tv3/")) //TV3 Download
            {
                String urlFull = @"http://www.ccma.cat" + urlSelectedTmp;
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8; //Cause its system ansi by defualt and that screws up the text
                    showEpisodeList = webClient.DownloadString(urlFull);
                }
                //:) so wrong
                Regex episodeSearch = new Regex(@"<div class=""F-itemContenidorIntern C-destacatVideo"">.*?<a title=""(.*?)"" href=""(.*?)""", RegexOptions.Singleline);
                //Cut it so we got episodes segment only
                showEpisodeList = showEpisodeList.Substring(showEpisodeList.IndexOf("F-cos"));
                MatchCollection episodes = episodeSearch.Matches(showEpisodeList);
                foreach (Match entry in episodes)
                {

                    //Decoding cause of &#039; need to be '
                    episodeList.Add(new EpisodesGeneric(WebUtility.HtmlDecode(entry.Groups[1].Value), entry.Groups[2].Value));
                }
                listBoxContent.ItemsSource = episodeList;
                return showName;
            }
            throw new System.ArgumentException("Not supported");        
        }

        public override DownloadObject GetDownloadObject()
        {
            String pageJson;
            
            String episodeUrl = episodeList[listBoxContent.SelectedIndex].EpisodeID;
            Regex regRefId = new Regex(@"/([0-9]+)/");
            String refID = regRefId.Matches(episodeUrl)[0].Groups[1].Value;

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
        public override string GetSelectedName()
        {
            return episodeList[listBoxContent.SelectedIndex].Name;
        }
        public override void CleanEpisodes()
        {
            episodeList.Clear();
        }
        public override void SetActive()
        {
            listBoxContent.ItemsSource = showList;
        }
        public override string GetSubtitles()
        {
            return "";
        }
        public override List<object> GetShowsList()
        {
            return showList.ToList<object>();
        }
    }
}
