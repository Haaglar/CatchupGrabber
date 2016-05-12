using cu_grab.EpisodeObjects.RTVEc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace cu_grab
{
    public class RTVEc : DownloadAbstract
    {
        private ShowsClan value;
        private EpisodesClan episodesClan;

        private Regex regexAnnotaions = new Regex(@"<[^>]*>");

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="lBoxContent">The ListBox in which the content is displayed in</param>
        public RTVEc() { }

        /// <summary>
        /// Fills the listbox with the JSON from RTVEClan search 
        /// </summary>
        public override void FillShowsList()
        {
            WebRequest reqSearchJs = HttpWebRequest.Create(@"http://www.rtve.es/infantil/buscador-clan/obtener-datos-programas.json");
            WebResponse resSearchJs = reqSearchJs.GetResponse();

            using (StreamReader srjs = new StreamReader(resSearchJs.GetResponseStream(), Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                value = jss.Deserialize<ShowsClan>(jsonjs);
                value.infoBuscador = value.infoBuscador.OrderBy(x => x.titulo).ToList(); //Maybe change to date published 
            }
            resSearchJs.Close();
            RequestedSiteData = true;
        }

        /// <summary>
        /// Handles clicking on a show and setting the listbox to the episodes for the show. 
        /// </summary>
        /// <returns>The name of the selected show</returns>
        public override string ClickDisplayedShow(int selectedIndex)
        {
            WebRequest reqTematicasJs = HttpWebRequest.Create("http://www.rtve.es/api/tematicas/" + value.infoBuscador[selectedIndex].id + "/videos.json");
            WebResponse resTematicasJs = reqTematicasJs.GetResponse();

            using (StreamReader srjs = new StreamReader(resTematicasJs.GetResponseStream(), Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                episodesClan = jss.Deserialize<EpisodesClan>(jsonjs);
                episodesClan.page.items = episodesClan.page.items.OrderBy(x => x.publicationDate).ToList();
                            
            }

            string selectedShow = value.infoBuscador[selectedIndex].titulo;
            resTematicasJs.Close();
            return selectedShow;
        }

        /// <summary>
        /// Genrates a downloadURL for rtve clan
        /// Based off work from rtvealacarta by itorres and personal research
        /// </summary>
        /// <returns>The url to download from</returns>
        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            //Create a phase conatining the video id and milliseconds since the unix epoch
            DateTime dt = DateTime.Now;
            DateTime epoch = new DateTime(1970, 1, 1);
            string joined = episodesClan.page.items[selectedIndex].id + "_es_" + (dt - epoch).TotalMilliseconds;
            

            RijndaelManaged aesEncrypt = new RijndaelManaged();
            //Set up the key and phrase to encrypt
            string passPhrase = "pmku579tg465GDjf1287gDFFED56788C"; // key for the "oceano"/tablet url
            byte[] key = new UTF8Encoding().GetBytes(passPhrase);
            byte[] toEncrypt = new UTF8Encoding().GetBytes(joined);

            //Set up aes settings
            aesEncrypt.BlockSize = 128;
            aesEncrypt.IV = new byte[16];
            aesEncrypt.Padding = PaddingMode.PKCS7;
            aesEncrypt.Mode = CipherMode.CBC;
            aesEncrypt.Key = key;
            
            // encrypt it
            ICryptoTransform transform = aesEncrypt.CreateEncryptor();
            byte[] encryptedText = transform.TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);
            return new DownloadObject("http://www.rtve.es/ztnr/consumer/oceano/video/" + Convert.ToBase64String(encryptedText), GetSubtitles(), Country.Spain, DownloadMethod.HTTP);
        }
        
        /// <summary>
        /// Handles Clearing the episode list and reseting it back to the show list
        /// </summary>
        public override void CleanEpisodes()
        {
            episodesClan = null;
        }

        public override string GetSelectedNameEpisode(int selectedIndex)
        {
            return episodesClan.page.items[selectedIndex].ToString();
        }
        public override string GetSubtitles()
        {
            return "";
        }
        public override List<object> GetShowsList()
        {
            return value.infoBuscador.ToList<object>();
        }
        public override List<object> GetEpisodesList()
        {
            return episodesClan.page.items.ToList<object>();
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            return null;
        }

        public override string GetDescriptionEpisode(int selectedIndex)
        {
            string desc = episodesClan.page.items[selectedIndex].description;
            if (string.IsNullOrEmpty(desc))
            {
                return null;
            }

            desc = WebUtility.HtmlDecode(regexAnnotaions.Replace(desc, "").Replace("\\n", " "));
            return desc;
        }
    }
}
