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
    class TV3Cat : DownloadAbstract
    {
        private List<ShowsGeneric> showList = new List<ShowsGeneric>();
        public TV3Cat (ListBox lBoxContent) : base(lBoxContent) { }
        public override void FillShowsList()
        {
            String websiteShowList;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8; //Cause its system ansi by defualt and that screws up the text
                websiteShowList = webClient.DownloadString("http://www.ccma.cat/tv3/programes/");
            }
            Regex getContents = new Regex(@"a href=""(.*?)"">(.*?)<", RegexOptions.Singleline);
            String cut = websiteShowList.Substring(websiteShowList.IndexOf(@"div class=""span9""")); //Cut the string
            MatchCollection entries = getContents.Matches(cut);
            foreach(Match entry in entries)
            {
                showList.Add(new ShowsGeneric(entry.Groups[2].Value.Trim(), entry.Groups[1].Value));
            }
            listBoxContent.ItemsSource = showList;

        }
        public override string ClickDisplayedShow()
        {
            throw new NotImplementedException();
        }
        public override string GetUrl()
        {
            throw new NotImplementedException();
        }
        public override string GetSelectedName()
        {
            throw new NotImplementedException();
        }
        public override void CleanEpisodes()
        {
            throw new NotImplementedException();
        }
        public override void SetActive()
        {
            throw new NotImplementedException();
        }
        public override string GetSubtitles()
        {
            throw new NotImplementedException();
        }
    }
}
