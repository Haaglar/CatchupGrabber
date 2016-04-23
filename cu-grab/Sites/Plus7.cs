using cu_grab.MiscObjects.Plus7;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;


namespace cu_grab
{
    public class Plus7 : DownloadAbstract
    {
        private string tvShowsUrl = @"https://au.tv.yahoo.com/plus7/data/tv-shows/"; //Json object used to provide search suggestions
        private List<ShowsP7> showsP7;
        private List<EpisodesGeneric> selectedShowEpisodes = new List<EpisodesGeneric>();
        private BCoveJson bCoveJson; //Json from the api request 

        //Stuff for downloading
        private string apiUrl = "http://c.brightcove.com/services/json/player/media/?command=find_media_by_reference_id";
        private string publisherIdMain = "2376984108001";
        private string publisherIdAlt = "2376984109001"; //Alternate for extras and TV Snax

        /// <summary>
        /// Standard constructor
        /// </summary>
        public Plus7() {}
       
        /// <summary>
        /// Fills showsP7 with data taken from the search feature on the P7 website
        /// </summary>
        public override void FillShowsList()
        {
            WebRequest reqSearchJs = HttpWebRequest.Create(tvShowsUrl);
            WebResponse resSearchJs = reqSearchJs.GetResponse();

            using (StreamReader srjs = new StreamReader(resSearchJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                showsP7 = jss.Deserialize<List<ShowsP7>>(jsonjs);
                showsP7 = showsP7.OrderBy(x => x.title).ToList(); 
            }
            resSearchJs.Close();
            RequestedSiteData = true;
        }
        /// <summary>
        /// Fills selectedShowEpisodes with episdes from the selected show
        /// </summary>
        /// <param name="selectedIndex">Index of the selected show in the list</param>
        /// <returns>The name of the selected Show</returns>
        public override string ClickDisplayedShow(int selectedIndex)
        {
            string pageShow;
            WebRequest reqShow = HttpWebRequest.Create(showsP7[selectedIndex].url);
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

            //Honestly I dont want to do this, but I dont want to use external libraries and HTMLDocument doesnt like me so screw it.  
            //string regexHeadline = @"""headline"">([0-9].)"; 
            Regex regexLoadmore = new Regex(@"data-url=""(.*)"" rel=""next""");
            MatchCollection matchLoadMore = regexLoadmore.Matches(pageShow);
            if (matchLoadMore.Count != 0)//If it hasn't loaded all of the episodes make another request
            {
                string updatedUrl = matchLoadMore[0].Groups[1].Value;
                updatedUrl = updatedUrl.Replace("&amp;", @"&");            //Fix ampersands
                updatedUrl = updatedUrl.Replace("2/?pp=10", @"1/?pp=70"); //Get the first 70 results for the show (Make this dynamic)
                updatedUrl = @"http://au.tv.yahoo.com" + updatedUrl + "&bucket=exclusiveBkt"; //Add on additional GET value and prefix
                Uri uri = new Uri(updatedUrl);
                WebRequest reqShowAll = HttpWebRequest.Create(uri);
                WebResponse resShowAll = reqShowAll.GetResponse();
                StreamReader srShowAll = new StreamReader(resShowAll.GetResponseStream(), System.Text.Encoding.UTF8);
                pageShow = srShowAll.ReadToEnd();
                resShowAll.Close();
                srShowAll.Close();
            
            }
            else
            {
                //If we dont need to load, remove the stuff off the side (recommended crap) so its cleaned for regex
                int startPoint = pageShow.IndexOf("class=\"g-col-8 g-xl-30-40 g-l-20-30 g-m-row g-main"); //Main content for shows
                int endPoint = pageShow.IndexOf("class=\"g-col-4 g-xl-10-40 g-l-10-30 g-m-row g-rail");   //Side reccommendations div
                pageShow = pageShow.Substring(startPoint, endPoint - startPoint); //cut excess rubbish
            }

            Regex regexLinks = new Regex(@"href=""//(.*/)"""); 
            Regex regexDesc = new Regex(@"collection-title-link-inner"">.*\n(.*)"); 

            MatchCollection matchDesc = regexDesc.Matches(pageShow);
            MatchCollection matchLinks = regexLinks.Matches(pageShow);
            int i = 0;
            foreach (Match match in matchDesc)
            {
                string url = matchLinks[i].Groups[1].Value;
                string description = match.Groups[1].Value.Trim(); // Trim excess whitespace cause otherwise itll look like rubbish 
                selectedShowEpisodes.Add(new EpisodesGeneric(description, url));
                i += 2;//Skip every second as theres a href on both the image and the content
            }

            //Store the current show name for file naming later
            string selectedShow = showsP7[selectedIndex].title;
            //Clean the name for windows
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                selectedShow = selectedShow.Replace(c, '-');
            }
            //Update list
            return selectedShow;
        }
        /// <summary>
        /// Gets the name of the current selected show
        /// </summary>
        /// <param name="selectedIndex">Index of the selected show in the list</param>
        /// <returns>The selected show's name</returns>
        public override string GetSelectedNameEpisode(int selectedIndex)
        {
            return selectedShowEpisodes[selectedIndex].Name;
        }
        /// <summary>
        /// Grabs the page, does some stuff (important part ported from p7-hls) and gets the URL
        /// </summary>
        /// <param name="selectedIndex">Index of the selected show in the list</param>
        /// <returns>An object containing the download instructions</returns>
        public override DownloadObject GetDownloadObject(int selectedIndex) 
        {
            //Get episode page data
            string pageContent;
            string url = selectedShowEpisodes[selectedIndex].EpisodeID;
            WebRequest reqShow = HttpWebRequest.Create("https://" + url);
            using (WebResponse resShowUrl = reqShow.GetResponse())
            {
                using (Stream responseStreamUrl = resShowUrl.GetResponseStream())
                {
                    using (StreamReader srShowUrl = new StreamReader(responseStreamUrl, System.Text.Encoding.UTF8))
                    {
                        pageContent = srShowUrl.ReadToEnd();
                    }
                }
            }
            //Get Id from Url
            Regex regRefId = new Regex(@"/([0-9]+)/");
            string refID = regRefId.Matches(url)[0].Groups[1].Value;
            // Get playerkey from page
            Regex regPlayerKey = new Regex(@"rKey"" value=""(.*)""");
            string playerKey = regPlayerKey.Matches(pageContent)[0].Groups[1].Value;
            string jsonUrl = apiUrl + "&playerKey=" + playerKey + "&pubId=" + publisherIdMain + "&refId=" + refID;

            //Get and store the json data   
            using(WebClient wc = new WebClient())
            {
                string showsJson = wc.DownloadString(jsonUrl);
                if(showsJson.Equals("null"))//Bad id
                {
                    showsJson = wc.DownloadString(jsonUrl = apiUrl + "&playerKey=" + playerKey + "&pubId=" + publisherIdAlt + "&refId=" + refID);
                }
                JavaScriptSerializer jss = new JavaScriptSerializer();
                bCoveJson = jss.Deserialize<BCoveJson>(showsJson);
            }

            //Get highest quality
            int defaultQual = bCoveJson.FLVFullSize;
            string fullLengthURL = bCoveJson.FLVFullLengthURL;
            int oldSize = 0;
            foreach(IOSRendition redition in bCoveJson.IOSRenditions)
            {
                if(oldSize < redition.size)
                {
                    fullLengthURL = redition.defaultURL;   
                }
            }
            return new DownloadObject(fullLengthURL, GetSubtitles(), Country.Aus, DownloadMethod.HLS);
        }
        /// <summary>
        /// Handles Clearing the episode list and reseting it back to the show list
        /// </summary>
        public override void CleanEpisodes() 
        {
            selectedShowEpisodes.Clear();
        }

        /// <summary>
        /// Gets the URL of the subtitles for the selected episode.
        /// </summary>
        /// <returns>The URL for the captions, returns a blank String if no Subtitles exist </returns>
        public override string GetSubtitles()
        {
            if(bCoveJson.captions != null)
            {
                return bCoveJson.captions[0].URL;
            }
            return "";
        }
        /// <summary>
        /// Gets the list that contains the shows
        /// </summary>
        /// <returns>The show list</returns>
        public override List<object> GetShowsList()
        {
            return showsP7.ToList<object>();
        }
        /// <summary>
        /// Gets the list that contains the episodes 
        /// </summary>
        /// <returns></returns>
        public override List<object> GetEpisodesList()
        {
            return selectedShowEpisodes.ToList<object>();
        }
    }
}
