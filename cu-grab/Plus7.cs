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

       String tvShowsUrl = @"https://au.tv.yahoo.com/plus7/data/tv-shows/"; //Json object used to provide search suggestions
       private List<ShowsP7> showsP7;
       private List<Episode> selectedShowEpisodes = new List<Episode>();
       private ListBox objectList;
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
            using (WebResponse resShow = reqShow.GetResponse())
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

            Regex regexLinks = new Regex(@"href=""//(.*/)"""); //Skip every second
            Regex regexDesc = new Regex(@"collection-title-link-inner"">.*\n(.*)"); // Make sure to trim excess whitespace

            MatchCollection matchDesc = regexDesc.Matches(pageShow);
            MatchCollection matchLinks = regexLinks.Matches(pageShow);
            int i = 0;
            foreach (Match match in matchDesc)
            {
                String url = matchLinks[i].Groups[1].Value;
                String description = match.Groups[1].Value.Trim();
                selectedShowEpisodes.Add(new Episode(description, url));
                i += 2;
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
        public override String getUrl() 
        {
            return "";
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
    }
}
