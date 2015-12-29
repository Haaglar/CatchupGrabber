using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Controls;


namespace cu_grab
{
    public class Plus7 : DownloadAbstract
    {

       private String tvShowsUrl = @"https://au.tv.yahoo.com/plus7/data/tv-shows/"; //Json object used to provide search suggestions
       private List<ShowsP7> showsP7;
       private List<Episode> selectedShowEpisodes = new List<Episode>();
       private BCoveJson bCoveJson; //Json from the api request 
       private ListBox objectList;
       //Stuff for downloading
       private String apiUrl = "http://c.brightcove.com/services/json/player/media/?command=find_media_by_reference_id";
       private String publisherId = "2376984108001";

        public Plus7(ListBox oList)
        {
            objectList = oList;
        }
        public override void fillShowsList()
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
            objectList.ItemsSource = showsP7;
            resSearchJs.Close();
        }
        public override String clickDisplayedShow()
        {
            String pageShow;
            WebRequest reqShow = HttpWebRequest.Create(showsP7[objectList.SelectedIndex].url);
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
                String updatedUrl = matchLoadMore[0].Groups[1].Value;
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
                //If we dont need to load, remove the shit off the side (recommended crap) 
                int startPoint = pageShow.IndexOf("class=\"g-xl-30-40 g-l-20-30 g-m-row g-main"); //Main content for shows
                int endPoint = pageShow.IndexOf("class=\"g-xl-10-40 g-l-10-30 g-m-row g-rail");
                pageShow = pageShow.Substring(startPoint, endPoint - startPoint); //cut excess rubbish
            }

            Regex regexLinks = new Regex(@"href=""//(.*/)"""); 
            Regex regexDesc = new Regex(@"collection-title-link-inner"">.*\n(.*)"); 

            MatchCollection matchDesc = regexDesc.Matches(pageShow);
            MatchCollection matchLinks = regexLinks.Matches(pageShow);
            int i = 0;
            foreach (Match match in matchDesc)
            {
                String url = matchLinks[i].Groups[1].Value;
                String description = match.Groups[1].Value.Trim(); // Trim excess whitespace cause otherwise itll look like rubbish 
                selectedShowEpisodes.Add(new Episode(description, url));
                i += 2;//Skip every second as theres a href on both the image and the content
            }
        
            //Store the current show name for file naming later
            String selectedShow = showsP7[objectList.SelectedIndex].title;
            //Clean the name for windows
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                selectedShow = selectedShow.Replace(c, '-');
            }
            //Update list and states
            objectList.ItemsSource = selectedShowEpisodes;
            return selectedShow;
        }
        public override String getSelectedName()
        {
            return selectedShowEpisodes[objectList.SelectedIndex].Name;
        }
        /// <summary>
        /// Grabs the page, does some stuff (important part ported from p7-hls) and gets the URL
        /// </summary>
        /// <returns>The m3u8 url</returns>
        public override String getUrl() 
        {
            //Get episode page data
            String pageContent;
            String url = selectedShowEpisodes[objectList.SelectedIndex].EpisodeID;
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
            String refID = regRefId.Matches(url)[0].Groups[1].Value;
            // Get playerkey from page
            Regex regPlayerKey = new Regex(@"rKey"" value=""(.*)""");
            String playerKey = regPlayerKey.Matches(pageContent)[0].Groups[1].Value;
            String jsonUrl = apiUrl + "&playerKey=" + playerKey + "&pubId=" + publisherId + "&refId=" + refID;

            //Get and store the json data   
            WebRequest reqShowJson = HttpWebRequest.Create(jsonUrl);
            using (WebResponse resShowJson = reqShowJson.GetResponse())
            {
                using (Stream responseStreamJson = resShowJson.GetResponseStream())
                {
                    using (StreamReader srShowJson = new StreamReader(responseStreamJson, System.Text.Encoding.UTF8))
                    {
                        String showJson = srShowJson.ReadToEnd();
                        JavaScriptSerializer jss = new JavaScriptSerializer();
                        bCoveJson = jss.Deserialize<BCoveJson>(showJson);   
                    }
                }
            }
            //Get highest quality
            int size = bCoveJson.FLVFullSize;
            foreach(IOSRendition redition in bCoveJson.IOSRenditions)
            {
                if(redition.size == size)
                {
                    return redition.defaultURL;
                }
            }
            //If we don't get the highest quality, return the master URL
            return bCoveJson.FLVFullLengthURL;
        }
        
        public override void cleanEpisodes() 
        {
            selectedShowEpisodes.Clear();
            objectList.ItemsSource = showsP7;
        }
        public override void setActive()
        {
            objectList.ItemsSource = showsP7;
        }
        public override String getSubtitles()
        {
            if(bCoveJson.captions != null)
            {
                return bCoveJson.captions[0].URL;
            }
            return "";
        }
    }
}
