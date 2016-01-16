using cu_grab.Downloader.RTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Controls;

namespace cu_grab
{
    public class RTE : DownloadAbstract
    {
        private ListBox objectList;
        private List<RTEShows> rteShows;
        public RTE(ListBox oList)
        {
            objectList = oList;
        }
        public override void fillShowsList()
        {
            WebRequest reqSearchJs = HttpWebRequest.Create(@"https://www.rte.ie/player/shows.js?v=2");
            WebResponse resSearchJs = reqSearchJs.GetResponse();

            using (StreamReader srjs = new StreamReader(resSearchJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd(); 
                int jslen = jsonjs.Length;
                jsonjs = jsonjs.Substring(12, jslen - 12); // remove the "var shows = ["  at start and "]" at end

                JavaScriptSerializer jss = new JavaScriptSerializer();
                rteShows = jss.Deserialize<List<RTEShows>>(jsonjs);
                rteShows = rteShows.OrderBy(x => x.v).ToList(); //Order By name 
            }
            objectList.ItemsSource = rteShows;
            resSearchJs.Close();
        }
        public override void setActive()
        {
            throw new NotImplementedException();
        }
        public override string clickDisplayedShow()
        {
            throw new NotImplementedException();
        }
        public override string getUrl()
        {
            throw new NotImplementedException();
        }
        public override void cleanEpisodes()
        {
            throw new NotImplementedException();
        }
        public override string getSelectedName()
        {
            throw new NotImplementedException();
        }
        public override string getSubtitles()
        {
            throw new NotImplementedException();
        }
    }
}
