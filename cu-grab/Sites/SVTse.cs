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
    class SVTse : DownloadAbstract
    {
        private static string BaseURL = "http://www.svtplay.se";
        private static string ShowListURL = "/program";
        private static string AdditionalEpisodes = @"?sida=2&tab=helaprogram&embed=true";
        private List<ShowsGeneric> showsSVT = new List<ShowsGeneric>();
        private List<EpisodesGeneric> episodesSVT = new List<EpisodesGeneric>();

        public SVTse(){ }
        public override void CleanEpisodes()
        {
            episodesSVT.Clear();
        }
        public override string ClickDisplayedShow(int selectedIndex)
        {
            String websiteShowList;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8; //Webpage encoding
                websiteShowList = webClient.DownloadString(showsSVT[selectedIndex].url);
            }
            Regex showsSearch = new Regex(@"<a href=""([^""].*)"" class=""play_vertical-list__header-link"">([^<]*)</a>");
            MatchCollection matchesOne = showsSearch.Matches(websiteShowList);
            foreach (Match match in matchesOne)
            {
                episodesSVT.Add(new EpisodesGeneric( WebUtility.HtmlDecode(match.Groups[2].Value), BaseURL + match.Groups[1].Value));
            }
            //If theres more we have to do it again
            if (websiteShowList.IndexOf("play_title-page__pagination-button-thin-labe") != -1)
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8; //Webpage encoding
                    websiteShowList = webClient.DownloadString(showsSVT[selectedIndex].url + AdditionalEpisodes);
                    MatchCollection matchesTwo = showsSearch.Matches(websiteShowList);
                    foreach (Match match in matchesTwo)
                    {
                        episodesSVT.Add(new EpisodesGeneric( WebUtility.HtmlDecode(match.Groups[2].Value), BaseURL + match.Groups[1].Value));
                    }
                }
            }
            return showsSVT[selectedIndex].name;
            
        }
        public override void FillShowsList()
        {
            String websiteShowList;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8; //Webpage encoding
                websiteShowList = webClient.DownloadString(BaseURL + ShowListURL);
            }
            websiteShowList = WebUtility.HtmlDecode(websiteShowList);// Since its got swedish characterse
            Regex abuseShows = new Regex(@"<a href=""([^""]*)"" .* class=""play_link-list__link play_link-list__link--with-padding"">([^<]*)</a>");
            MatchCollection showsCol = abuseShows.Matches(websiteShowList);
            foreach(Match match in showsCol)
            {
                showsSVT.Add(new ShowsGeneric( match.Groups[2].Value,BaseURL + match.Groups[1].Value));
            }
        }
        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            throw new NotImplementedException();
        }
        public override string GetSelectedNameEpisode(int selectedIndex)
        {
            return episodesSVT[selectedIndex].Name;
        }
        public override string GetSubtitles()
        {
            throw new NotImplementedException();
        }

        public override List<object> GetShowsList()
        {
            return showsSVT.ToList<object>();
        }
        public override List<object> GetEpisodesList()
        {
            return episodesSVT.ToList<object>();
        }
    }
}
