﻿using cu_grab.Shows.DPlay;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace cu_grab
{
    public class DPlay : DownloadAbstract
    {
        private ShowsDPlay showsDPlay;
        private List<Episodes> episodesDPlay = new List<Episodes>();
        private static String EpisodeAjaxAddrp1 = @"http://it.dplay.com/api/v2/ajax/shows/";
        private static String EpisodeAjaxAddrp2 = @"/seasons/?show_id=";
        private static String ShowsUrl = @"http://it.dplay.com/api/v2/ajax/modules?items=400&page_id=32&module_id=26&page=0";


        public DPlay(ListBox lBoxContent) : base(lBoxContent) { }


        public override void FillShowsList()
        {

            String pageContent;
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
            //Yep every time you want to load all shows there its a ~3.6mb download, split into ~555kB chunks, and thats just text.
            //Full of useless information such as image metadata and links for 10 different images for a single show, all linking the same image
            //Like dimensitions and crop, absolute waste of resources
            jss.MaxJsonLength = Int32.MaxValue; 
            showsDPlay = jss.Deserialize<ShowsDPlay>(pageContent);
            //Remove all 0 episode shows, dont work, based on some value but in this area
            //showsDPlay.data.RemoveAll(x => x.taxonomy_items[2].metadata.episodes == "0");
            listBoxContent.ItemsSource = showsDPlay.data;
            resSearchJs.Close();
            rse.Close();
        }

        public override void SetActive()
        {
            listBoxContent.ItemsSource = showsDPlay.data;
        }

        public override string ClickDisplayedShow()
        {
            String output;
            //I would love to use the existing JSON for something other than the URL
            //But its so poorly conveluted that its impossible to do so
            using (WebClient webClient = new WebClient())
            {
                output = webClient.DownloadString(showsDPlay.data[listBoxContent.SelectedIndex].url);
            }
            Regex idFind = new Regex(@"data-show-id=""([0-9]*)");
            Match matches = idFind.Match(output);
            string id = matches.Groups[1].Value;

            //Get json for episodes
            using (WebClient webClient = new WebClient())
            {
                output = webClient.DownloadString(EpisodeAjaxAddrp1 + id + EpisodeAjaxAddrp2);
            }

            //Since i cant seem to parse the json data as its different between shows, Ill just abuse regex again
            Regex regName = new Regex(@"""title"":""([^""]+)"""); // Name
            Regex regID = new Regex(@"""hls"":""(.*?)"""); //Will be the url

            MatchCollection matchName = regName.Matches(output);
            MatchCollection matchID = regID.Matches(output);

            //Add episodes to list
            foreach (var match in matchName.Cast<Match>().Zip(matchID.Cast<Match>(), Tuple.Create)) //Join the two in a tuple
            {
                String description = match.Item1.Groups[1].Value;
                String ID = match.Item2.Groups[1].Value.Replace(@"\",""); //Remove escaped JSON
                episodesDPlay.Add(new Episodes(description, ID));
            }

            String selectedShow = showsDPlay.data[listBoxContent.SelectedIndex].title;


            //Clean the name for windows
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                selectedShow = selectedShow.Replace(c, '-');
            }
            listBoxContent.ItemsSource = episodesDPlay;
            return selectedShow;
        }
        public override string GetUrl()
        {
            throw new NotImplementedException();
        }
        public override void CleanEpisodes()
        {
            episodesDPlay.Clear();
            listBoxContent.ItemsSource = showsDPlay.data;
        }
        public override string GetSelectedName()
        {
            throw new NotImplementedException();
        }
        public override string GetSubtitles()
        {
            throw new NotImplementedException();
        }
    }
}
