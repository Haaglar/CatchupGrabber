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

        public Super3(ListBox lBoxContent) : base(lBoxContent) { }

        public override void CleanEpisodes()
        {
            throw new NotImplementedException();
        }
        public override string ClickDisplayedShow()
        {
            throw new NotImplementedException();
        }
        public override void FillShowsList()
        {
            String showsXML;
            Regex filter = new Regex(@"<title>(.*?)<\/title>\n<link(.*?)<\/link>");
            using (WebClient wc = new WebClient())
            {
                showsXML = wc.DownloadString(@"http://www.super3.cat/rss/sp3_serie_emissio_rss.xml");
            }
            MatchCollection matches = filter.Matches(showsXML);
            for(int i = 2; i < matches.Count; i++) //First two are useless
            {
                showsS3.Add(new ShowsGeneric(matches[i].Groups[1].Value, matches[i].Groups[2].Value));
            }
            listBoxContent.ItemsSource = showsS3;
        }
        public override string GetSelectedName()
        {
            throw new NotImplementedException();
        }
        public override string GetSubtitles()
        {
            throw new NotImplementedException();
        }
        public override string GetUrl()
        {
            throw new NotImplementedException();
        }
        public override void SetActive()
        {
            throw new NotImplementedException();
        }
    }
}
