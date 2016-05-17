using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using CatchupGrabber.Shows.DPlay;
using CatchupGrabber.NetworkAssister;

namespace CatchupGrabber
{
    public class DPlay : DownloadAbstract
    {
        private ShowsDPlay showsDPlay;
        private List<EpisodesGeneric> episodesDPlay = new List<EpisodesGeneric>();
        private static string EpisodeAjaxAddrp1 = @"http://it.dplay.com/api/v2/ajax/shows/";
        private static string EpisodeAjaxAddrp2 = @"/seasons/?show_id=";
        private static string EpisodeAjaxAddrp3 = @"&items=52&sort=episode_number_desc&video_types=-clip"; //52 is the average episodes for a show
        private static string ShowsUrl = @"http://it.dplay.com/api/v2/ajax/modules?items=400&page_id=32&module_id=26&page=0"; //Show list
        private CUNetworkAssist netAssist = new CUNetworkAssist(); 

        /// <summary>
        /// Defualt constructor
        /// </summary>
        public DPlay() { }


        public override void FillShowsList()
        {

            string pageContent;
            WebRequest reqShowJs = HttpWebRequest.Create(ShowsUrl);
            reqShowJs.Timeout = 100000;
            WebResponse resSearchJs = reqShowJs.GetResponse();
            var rse = resSearchJs.GetResponseStream();
            using (StreamReader srjs = new StreamReader(resSearchJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                pageContent = srjs.ReadToEnd();
            }
            
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //Cause the resulting json is ~3.6MB we need to set this. 
            //Yep every time you want to load all shows there its a ~3.6MB download, split into ~555kB chunks, and thats just text.
            //Full of useless information such as image metadata and links for 10 different images for a single show, all linking the same image
            //Like dimensitions and crop, absolute waste of resources
            jss.MaxJsonLength = 20097152; 
            showsDPlay = jss.Deserialize<ShowsDPlay>(pageContent);
            //Remove all 0 episode shows, dont work, based on some value but in this area
            //showsDPlay.data.RemoveAll(x => x.taxonomy_items[2].metadata.episodes == "0");
            resSearchJs.Close();
            rse.Close();
            RequestedSiteData = true;
        }


        public override void ClickDisplayedShow(int selectedIndex)
        {
            string output;
            //I would love to use the existing JSON for something other than the URL
            //But its so conveluted that its impossible to do so
            using (WebClient webClient = new WebClient())
            {
                output = webClient.DownloadString(showsDPlay.data[selectedIndex].url);
            }
            Regex idFind = new Regex(@"data-show-id=""([0-9]*)");
            Match matches = idFind.Match(output);
            string id = matches.Groups[1].Value;

            //Get json for episodes
            using (WebClient webClient = new WebClient())
            {
                output = webClient.DownloadString(EpisodeAjaxAddrp1 + id + EpisodeAjaxAddrp2 + id + EpisodeAjaxAddrp3);
            }

            //Since i cant seem to parse the json data as its different between shows, Ill just abuse regex again
            Regex regName = new Regex(@"""title"":""([^""]+)"""); // Name
            Regex regID = new Regex(@"""hls"":""(.*?)"""); //Will be the url

            MatchCollection matchName = regName.Matches(output);
            MatchCollection matchID = regID.Matches(output);

            //Add episodes to list
            foreach (var match in matchName.Cast<Match>().Zip(matchID.Cast<Match>(), Tuple.Create)) //Join the two in a tuple
            {
                string description = match.Item1.Groups[1].Value;
                string ID = match.Item2.Groups[1].Value.Replace(@"\",""); //Remove escaped JSON
                episodesDPlay.Add(new EpisodesGeneric(description, ID));
            }
        }

        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            string m3u8 = netAssist.GetHighestM3U8Address(episodesDPlay[selectedIndex].EpisodeID);
            return new DownloadObject(m3u8, GetSubtitles(), Country.Italy, DownloadMethod.HLS);
        }

        public override void CleanEpisodes()
        {
            episodesDPlay.Clear();
        }
        public override string GetSelectedEpisodeName(int selectedIndex)
        {
            return episodesDPlay[selectedIndex].Name;
        }
        public override string GetSubtitles()
        {
            return "";
        }
        public override List<object> GetShowsList()
        {
            return showsDPlay.data.ToList<object>();
        }
        public override List<object> GetEpisodesList()
        {
            return episodesDPlay.ToList<object>();
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            return WebUtility.HtmlDecode(showsDPlay.data[selectedIndex].description);
        }

        public override string GetDescriptionEpisode(int selectedIndex)
        {
            return null;
        }

        public override string GetSelectedShowName(int selectedIndex)
        {
            return showsDPlay.data[selectedIndex].title;
        }
    }
}
