using cu_grab.Shows.Prima;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace cu_grab
{
    class Prima : DownloadAbstract
    {
        private ShowsPrima showListObj;
        public override void CleanEpisodes()
        {
            throw new NotImplementedException();
        }

        public override string ClickDisplayedShow(int selectedIndex)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override List<object> GetEpisodesList()
        {
            throw new NotImplementedException();
        }

        public override string GetSelectedNameEpisode(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override List<object> GetShowsList()
        {
            return showListObj.result.ToList<object>();
        }

        public override string GetSubtitles()
        {
            throw new NotImplementedException();
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
    }
}
