using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;
using cu_grab.series.rtve;
using System.Windows.Controls;

namespace cu_grab
{
    public class RTVE : DownloadAbstract
    {
        private ShowsRTVE showsRTVE;
        private ListBox objectList;

        public RTVE(ListBox oList)
        {
            objectList = oList;
        }
        public override void fillShowsList()
        {
            WebRequest reqSearchJs = HttpWebRequest.Create(@"http://www.rtve.es/api/programas.json");
            WebResponse resSearchJs = reqSearchJs.GetResponse();

            using (StreamReader srjs = new StreamReader(resSearchJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                showsRTVE = jss.Deserialize<ShowsRTVE>(jsonjs);
                showsRTVE.page.items = showsRTVE.page.items.OrderBy(x => x.name).ToList(); //Maybe change to date published 
            }
            objectList.ItemsSource = showsRTVE.page.items;
            resSearchJs.Close();
        }

        public override void cleanEpisodes()
        {

        }
        public override string clickDisplayedShow()
        {
            return "";
        }
        
        
        
        public override string getSelectedName()
        {
            return "";
        }
        public override string getSubtitles()
        {
            return "";
        }
        public override string getUrl()
        {
            return "";
        }
        public override void setActive()
        {
            
        }
        

    }
}
