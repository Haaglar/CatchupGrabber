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
        private List<ShowsGeneric> showsSVT = new List<ShowsGeneric>();
        public SVTse(ListBox lBoxContent){ }
        public override void CleanEpisodes()
        {
            throw new NotImplementedException();
        }
        public override string ClickDisplayedShow(int selectedIndex)
        {
            throw new NotImplementedException();
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
        public override string GetSelectedNameShow(int selectedIndex)
        {
            throw new NotImplementedException();
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
            return null;
        }
    }
}
