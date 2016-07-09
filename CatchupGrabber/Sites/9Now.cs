using CatchupGrabber.EpisodeObjects._9Now;
using CatchupGrabber.MiscObjects._9Now;
using CatchupGrabber.NetworkAssister;
using CatchupGrabber.Shows._9Now;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace CatchupGrabber
{
    class _9Now : DownloadAbstract
    {
        private Shows9Now shows9N;
        private Episodes9Now episodes9N;
        private string episodeUrlP1 = "https://tv-api.9now.com.au/v1/pages/tv-series/";
        private string episodeUrlP2 = "?device=android";
        private string apiAddressP1 = "http://api.brightcove.com/services/library?media_delivery=http&reference_id=";
        private string apiAddressP2 = "&command=find_video_by_reference_id&token=7jHYJN84oHHRRin6N6JpuDmm3ghgxP4o3GGXsatxe5aDKZ4MGOztLw..&video_fields=accountId%2CshortDescription%2CiOSRenditions%2CWVMRenditions%2CHLSURL%2CvideoFullLength";
        private CUNetworkAssist netAssist = new CUNetworkAssist();
        public _9Now() { }

        public override void CleanEpisodes()
        {
            episodes9N = null;
        }

        public override void ClickDisplayedShow(int selectedIndex)
        {
            string jsonContentEpisodes;
            string slug = shows9N.items[selectedIndex].slug;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = System.Text.Encoding.UTF8;
                jsonContentEpisodes = webClient.DownloadString(episodeUrlP1 + slug + episodeUrlP2);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            episodes9N = jss.Deserialize<Episodes9Now>(jsonContentEpisodes);
        }

        public override void FillShowsList()
        {
            //Device can be anything, but the android app uses android so we'll aswell, 99999 is also what the android app does
            string address = "https://tv-api.9now.com.au/v1/tv-series?device=android&take=99999";
            string jsonContent;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = System.Text.Encoding.UTF8;
                jsonContent = webClient.DownloadString(address);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            shows9N = jss.Deserialize<Shows9Now>(jsonContent);
            ShowListCacheValid = true;
        }

        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            string brightCoveRefId = episodes9N.items[0].items[selectedIndex].video.referenceId;
            string jsonRequest;
            using (WebClient webClient = new WebClient())
            {
                jsonRequest = webClient.DownloadString(apiAddressP1 + brightCoveRefId + apiAddressP2);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            BCoveJson9N json = jss.Deserialize<BCoveJson9N>(jsonRequest);

            string fullLengthURL = json.HLSURL;
            int oldSize = 0;

            foreach (IOSRendition redition in json.IOSRenditions)
            {
                if (oldSize < redition.encodingRate)
                {
                    fullLengthURL = redition.url;
                    oldSize = redition.encodingRate;
                }
            }
            return new DownloadObject(fullLengthURL, GetSubtitles(), Country.Aus, DownloadMethod.HLS);
        }

        public override List<object> GetEpisodesList()
        {
            return episodes9N.items[0].items.ToList<object>();
        }

        public override string GetSelectedEpisodeName(int selectedIndex)
        {
            return episodes9N.items[0].items[selectedIndex].name;
        }

        public override List<object> GetShowsList()
        {
            return shows9N.items.ToList<object>();
        }

        public override string GetSubtitles()
        {
            return "";
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            return shows9N.items[selectedIndex].description;
        }

        public override string GetDescriptionEpisode(int selectedIndex)
        {
            return episodes9N.items[0].items[selectedIndex].description;
        }

        public override string GetSelectedShowName(int selectedIndex)
        {
            return shows9N.items[selectedIndex].name;
        }
    }
}
