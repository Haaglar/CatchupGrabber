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
    public class Tenp
    {
        private RootObject shows;
        private List<Episode> selectedShowEpisodes = new List<Episode>();
        private ListBox objectList;

        public Tenp(ListBox oList)
        {
            objectList = oList;
        }
        /// <summary>
        /// Fillthe ListBox with the shows currently on Tenplay found from the search JSON.
        /// </summary>
        public void fillShowsList()
        {

            WebRequest reqSearchJs = HttpWebRequest.Create("http://tenplay.com.au/web%20api/showsearchjson");
            WebResponse resSearchJs = reqSearchJs.GetResponse();

            using (StreamReader srjs = new StreamReader(resSearchJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                shows = jss.Deserialize<RootObject>(jsonjs);
                shows.Shows = shows.Shows.OrderBy(x => x.Name).ToList();
                objectList.ItemsSource = shows.Shows;
            }
            resSearchJs.Close();
            
        }
        /// <summary>
        /// Handles clicking of a show
        /// </summary>
        /// <returns>The name to the clicked show</returns>
        public String clickDisplayedShow()
        {
            WebRequest reqShow = HttpWebRequest.Create("http://tenplay.com.au" + shows.Shows[objectList.SelectedIndex].ShowURL);
            WebResponse resShow = reqShow.GetResponse();

            StreamReader srShow = new StreamReader(resShow.GetResponseStream(), System.Text.Encoding.UTF8);
            string pageShow = srShow.ReadToEnd();

            //REGEX
            Regex idFind = new Regex(@"<div class=""content-card__image-container"">(.+?)</div>", RegexOptions.Singleline); //Finds all the episodes for the show
            Regex regexIDImage = new Regex(@"(?<=\bsrc="")[^""]*");      //ID from image
            Regex regexIDEpisode = new Regex(@"(?<=\balt="")[^""]*");      //Episode name from image
            Regex IDSplit = new Regex(@"([0-9]+)", RegexOptions.RightToLeft); //Get the correct id

            //Get and iterate over the episodes divs
            foreach (Match matchID in idFind.Matches(pageShow))
            {
                //get src ID and episode name
                MatchCollection matchIdImage = regexIDImage.Matches(matchID.Value);
                MatchCollection matchIdName = regexIDEpisode.Matches(matchID.Value);
                if (matchIdImage.Count > 0)
                {
                    String valueFull = matchIdImage[0].Value;
                    String split = valueFull.Split('?')[1];
                    String final = IDSplit.Matches(split)[0].Value;
                    selectedShowEpisodes.Add(new Episode(matchIdName[0].Value, final));
                }
            }
            //Store the current show name for file naming later
            String selectedShow = shows.Shows[objectList.SelectedIndex].Name;
            //Clean the name for windows
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                selectedShow = selectedShow.Replace(c, '-');
            }
            resShow.Close();
            srShow.Close();
            //Update list and states
            objectList.ItemsSource = selectedShowEpisodes;
            return selectedShow;
            
        }
        /// <summary>
        /// Get the download URL for FFmpeg
        /// </summary>
        /// <returns>A url</returns>
        public  String getUrl()
        {
            String BC_URL = "http://c.brightcove.com/services/mobile/streaming/index/master.m3u8?videoId="; //url taken from and m3u8
            String PUB_ID = "&pubId=2199827728001"; //ID taken from any m3u8

            // Get standard m3u8from
            WebRequest reqm3u8 = HttpWebRequest.Create(BC_URL + selectedShowEpisodes[objectList.SelectedIndex].EpisodeID + PUB_ID);
            WebResponse resm3u8 = reqm3u8.GetResponse();
            StreamReader srm3u8 = new StreamReader(resm3u8.GetResponseStream(), System.Text.Encoding.UTF8);

            String url = getHighestm3u8(srm3u8);
            resm3u8.Close();
            srm3u8.Close();
            return url;

        }
        /// <summary>
        /// Get the name of the select show
        /// </summary>
        /// <returns>Returns the Name of the selected episode</returns>
        public  String getSelectedName()
        {
            return selectedShowEpisodes[objectList.SelectedIndex].Name;
        }
        /// <summary>
        /// Handles Clearing the episode list and reseting it back to the show list
        /// </summary>
        public void cleanEpisodes()
        {
            selectedShowEpisodes.Clear();
            objectList.ItemsSource = shows.Shows;
        }

        /// <summary>
        /// Gets the highest rendition from a master m3u8 (move to a general class)
        /// </summary>
        /// <param name="m3u8">A stream reader object containing </param>
        /// <returns></returns>
        private String getHighestm3u8(StreamReader m3u8)
        {
            String line; // current line 
            String finalUrl = "";
            Regex regexBandwidth = new Regex(@"(?<=\bBANDWIDTH=)([0-9]+)"); //Quality Selection
            // TODO: Write this section better
            int index = 0;
            int row = -1;
            long bandwidth = 0;
            long tmp = 0;
            //Get the highest quality link
            while ((line = m3u8.ReadLine()) != null)
            {
                if (row == index)
                {
                    finalUrl = line;
                }
                index++;
                MatchCollection matchBand = regexBandwidth.Matches(line);
                if (matchBand.Count > 0)
                {
                    tmp = int.Parse(matchBand[0].Value);
                    if (tmp > bandwidth)
                    {
                        row = index;
                        bandwidth = tmp;
                    }
                }
            }
            return finalUrl;
        }
        /// <summary>
        /// Sets it as the active List
        /// </summary>
        public void setTPActive()
        {
            objectList.ItemsSource = shows.Shows;
        }
    }
}
