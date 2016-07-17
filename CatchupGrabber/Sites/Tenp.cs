using CatchupGrabber.NetworkAssister;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace CatchupGrabber
{
    public class Tenp : DownloadAbstract
    {
        private ShowsTenPlays shows;
        private List<EpisodesGeneric> selectedShowEpisodes = new List<EpisodesGeneric>();
        private CUNetworkAssist netAssist = new CUNetworkAssist(); 

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="lBoxContent">The ListBox in which the content is displayed in</param>
        public Tenp() {}
        
        /// <summary>
        /// Fillthe ListBox with the shows currently on Tenplay found from the search JSON.
        /// </summary>
        public override void FillShowsList()
        {

            WebRequest reqSearchJs = HttpWebRequest.Create("http://tenplay.com.au/web%20api/showsearchjson");
            WebResponse resSearchJs = reqSearchJs.GetResponse();

            using (StreamReader srjs = new StreamReader(resSearchJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                shows = jss.Deserialize<ShowsTenPlays>(jsonjs);
                shows.Shows = shows.Shows.OrderBy(x => x.Name).ToList();    
            }
            resSearchJs.Close();
            ShowListCacheValid = true;
        }
        /// <summary>
        /// Handles clicking of a show
        /// </summary>
        /// <returns>The name to the clicked show</returns>
        public override void ClickDisplayedShow(int selectedIndex)
        {
            WebRequest reqShow = HttpWebRequest.Create("http://tenplay.com.au/handlers/Render.ashx?path=/UserControls/Content/ContentBody.ascx&providername=Episode&datasourceid=" + shows.Shows[selectedIndex].ScId);
            WebResponse resShow = reqShow.GetResponse();

            StreamReader srShow = new StreamReader(resShow.GetResponseStream(), System.Text.Encoding.UTF8);
            string pageShow = srShow.ReadToEnd();

            //REGEX
            Regex regexID = new Regex(@"data-alt-id=""(\d*)""");      
            Regex regexIDEpisode = new Regex(@"alt=""([^""]*)""");      //Episode name from image
            Regex regexDesc = new Regex(@"<span class=""content-card__sub-heading"">(.*?)</span>", RegexOptions.Singleline);
            MatchCollection matchId = regexID.Matches(pageShow);
            MatchCollection matchIdName = regexIDEpisode.Matches(pageShow);
            MatchCollection matchDesc = regexDesc.Matches(pageShow);
            //Get and iterate over the episodes divs
            var results = matchIdName.Cast<Match>().Zip(matchId.Cast<Match>().Zip(matchDesc.Cast<Match>(), (b, c) => new { b, c }), (a, b) => new { Value = a, Value2 = b.b, Value3 = b.c });
            foreach (var match in results)
            {
                selectedShowEpisodes.Add(new EpisodesGeneric(match.Value.Groups[1].Value, match.Value2.Groups[1].Value, match.Value3.Groups[1].Value.Trim()));
            }
            resShow.Close();
            srShow.Close();
        }
        /// <summary>
        /// Get the download URL for FFmpeg
        /// </summary>
        /// <returns>A url</returns>
        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            string BC_URL = "http://c.brightcove.com/services/mobile/streaming/index/master.m3u8?videoId="; //url taken from and m3u8
            string PUB_ID = "&pubId=2199827728001";
            // Get standard m3u8 from
            string url = netAssist.GetHighestM3U8Address(BC_URL + selectedShowEpisodes[selectedIndex].EpisodeID + PUB_ID);
            return new DownloadObject(url, GetSubtitles(), Country.Aus, DownloadMethod.HLS);
        }
        /// <summary>
        /// Get the name of the select show
        /// </summary>
        /// <returns>Returns the Name of the selected episode</returns>
        public override string GetSelectedEpisodeName(int selectedIndex)
        {
            return selectedShowEpisodes[selectedIndex].Name;
        }
        /// <summary>
        /// Handles Clearing the episode list and reseting it back to the show list
        /// </summary>
        public override void CleanEpisodes()
        {
            selectedShowEpisodes.Clear();
        }
        /// <summary>
        /// Sets it as the active List
        /// </summary>

        public override string GetSubtitles()
        {
            return "";
        }
        public override List<object> GetShowsList()
        {
            return shows.Shows.ToList<object>();
        }
        public override List<object> GetEpisodesList()
        {
            return selectedShowEpisodes.ToList<object>();
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            return null;
        }

        public override string GetDescriptionEpisode(int selectedIndex)
        {
            return selectedShowEpisodes[selectedIndex].Description;
        }

        public override string GetSelectedShowName(int selectedIndex)
        {
            return shows.Shows[selectedIndex].Name;
        }
    }
}
