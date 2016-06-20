using CatchupGrabber.EpisodeObjects.Prima;
using CatchupGrabber.MiscObjects.Prima;
using CatchupGrabber.NetworkAssister;
using CatchupGrabber.Shows.Prima;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CatchupGrabber
{
    class Prima : DownloadAbstract
    {
        private ShowsPrima showListObj;
        private EpisodesPrima epiListObj;
        public override void CleanEpisodes()
        {
            epiListObj = null;
        }

        public override void ClickDisplayedShow(int selectedIndex)
        {
            string showsJson;
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;
                showsJson = wc.DownloadString(@"https://api.play-backend.iprima.cz/api/v1/lists/carousels/prod-" + showListObj.result[selectedIndex].id);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            epiListObj = jss.Deserialize<EpisodesPrima>(showsJson);
        }

        public override void FillShowsList()
        {
            HandleData().Wait();
            RequestedSiteData = true;
        }

        /// <summary>
        /// Handles treading of the requests and setting the show list
        /// </summary>
        /// <returns></returns>
        private async Task HandleData()
        {

            Task<string> req1 = RequestShowsJson("https://api.play-backend.iprima.cz/api/v1/products/filter?limit=100");
            Task<string> req2 = RequestShowsJson("https://api.play-backend.iprima.cz/api/v1/products/filter?offset=100&limit=100");
            Task<string> req3 = RequestShowsJson("https://api.play-backend.iprima.cz/api/v1/products/filter?offset=200&limit=100");

            string res1 = await req1;
            string res2 = await req2;
            string res3 = await req3;

            CreateEpisodeList(res1);
            CreateEpisodeList(res2);
            CreateEpisodeList(res3);

        }
        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            string url = "http://api.play-backend.iprima.cz/api/v1/products/id-" + epiListObj.result[0].result[selectedIndex].id +"/play/";
            string dlJson;
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;
                dlJson = wc.DownloadString(url);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            PrimaJson episodeInfo = jss.Deserialize<PrimaJson>(dlJson);
            CUNetworkAssist cuna = new CUNetworkAssist();
            string m3u8 = cuna.GetHighestM3U8Address(episodeInfo.streamInfos[0].url);
            return new DownloadObject(m3u8, GetSubtitles(), Country.Czech, DownloadMethod.HLS);
        }

        public override List<object> GetEpisodesList()
        {
            return epiListObj.result[0].result.ToList<object>();
        }

        public override string GetSelectedEpisodeName(int selectedIndex)
        {
            return epiListObj.result[0].result[selectedIndex].localTitle;
        }

        public override List<object> GetShowsList()
        {
            return showListObj.result.ToList<object>();
        }

        public override string GetSubtitles()
        {
            return "";
        }

        /// <summary>
        /// Request a json from the specified url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private Task<string> RequestShowsJson(string url)
        {
            return Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                //?offset=100
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"order\":[\"title\"]}";    //POST data for requesting the shows in a-z order
                    streamWriter.Write(json);
                }
                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string result;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                return result;
            });
        }
        /// <summary>
        /// Creates or appeneds JSON request to the show list
        /// </summary>
        /// <param name="result">The JSON in string form</param>
        private void CreateEpisodeList(string result)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            if (showListObj == null)
            {
                showListObj = jss.Deserialize<ShowsPrima>(result);
            }
            else
            {
                ShowsPrima tmp = jss.Deserialize<ShowsPrima>(result);
                showListObj.result = showListObj.result.Concat(tmp.result).ToList();
            }
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            return showListObj.result[selectedIndex].annotation;
        }

        public override string GetDescriptionEpisode(int selectedIndex)
        {
            return epiListObj.result[0].result[selectedIndex].annotation;
        }

        public override string GetSelectedShowName(int selectedIndex)
        {
            return showListObj.result[selectedIndex].localTitle;
        }
    }
}
