﻿using cu_grab.Downloader.RTE;
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
        private List<Episode> selectedShowEpisodes = new List<Episode>();
        public RTE(ListBox oList)
        {
            objectList = oList;
        }
        /// <summary>
        /// Fills the listbox with the JSON from rte search
        /// </summary>
        public override void fillShowsList()
        {
            WebRequest reqSearchJs = HttpWebRequest.Create(@"https://www.rte.ie/player/shows.js?v=2");
            WebResponse resSearchJs = reqSearchJs.GetResponse();

            using (StreamReader srjs = new StreamReader(resSearchJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd(); 
                int jslen = jsonjs.Length;
                jsonjs = jsonjs.Substring(12, jslen - 12); // remove the "var shows = "

                JavaScriptSerializer jss = new JavaScriptSerializer();
                rteShows = jss.Deserialize<List<RTEShows>>(jsonjs);
                rteShows = rteShows.OrderBy(x => x.v).ToList(); //Order By name 
            }
            objectList.ItemsSource = rteShows;
            resSearchJs.Close();
        }
        /// <summary>
        /// Sets the ListBox to RTE
        /// </summary>
        public override void setActive()
        {
            objectList.ItemsSource = rteShows;
        }
        public override string clickDisplayedShow()
        {
            //Get page content
            String pageShow;
            WebRequest reqShow = HttpWebRequest.Create("https://www.rte.ie/player/lh/show/" + rteShows[objectList.SelectedIndex].id);
            using (WebResponse resShow = reqShow.GetResponse()) //>using
            {
                using (Stream responseStream = resShow.GetResponseStream())
                {
                    using (StreamReader srShow = new StreamReader(responseStream, System.Text.Encoding.UTF8))
                    {
                        pageShow = srShow.ReadToEnd();
                    }
                }
            }
            //

            return pageShow;
        }
        public override string getUrl()
        {
            throw new NotImplementedException();
        }
        public override void cleanEpisodes()
        {
            selectedShowEpisodes.Clear();
            objectList.ItemsSource = rteShows;
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
