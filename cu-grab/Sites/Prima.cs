using cu_grab.EpisodeObjects.Prima;
using cu_grab.MiscObjects.Prima;
using cu_grab.NetworkAssister;
using cu_grab.Shows.Prima;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System;

namespace cu_grab
{
    class Prima : DownloadAbstract
    {
        private ShowsPrima showListObj;
        private EpisodesPrima epiListObj;
        public override void CleanEpisodes()
        {
            epiListObj = null;
        }

        public override string ClickDisplayedShow(int selectedIndex)
        {
            string showsJson;
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;
                showsJson = wc.DownloadString(@"https://api.play-backend.iprima.cz/api/v1/lists/carousels/prod-" + showListObj.result[selectedIndex].id);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            epiListObj = jss.Deserialize<EpisodesPrima>(showsJson);
            return showListObj.result[selectedIndex].localTitle;
        }

        public override void FillShowsList()
        {
            requestShowsJson("https://api.play-backend.iprima.cz/api/v1/products/filter?limit=100");
            requestShowsJson("https://api.play-backend.iprima.cz/api/v1/products/filter?offset=100&limit=100");
            requestShowsJson("https://api.play-backend.iprima.cz/api/v1/products/filter?offset=200&limit=100");
            RequestedSiteData = true;
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

        public override string GetSelectedNameEpisode(int selectedIndex)
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

        private void requestShowsJson(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            //?offset=100
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"order\":[\"title\"]}";
                streamWriter.Write(json);
            }
            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            if(showListObj == null)
            {
                showListObj = jss.Deserialize<ShowsPrima>(result);
            }
            else
            {
                ShowsPrima tmp = jss.Deserialize<ShowsPrima>(result);
                showListObj.result = showListObj.result.Concat(tmp.result).ToList();
            }

        }

        public override string GetDescriptionShow()
        {
            return "";
        }
    }
}
