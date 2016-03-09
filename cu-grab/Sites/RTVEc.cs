using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Controls;

namespace cu_grab
{
    public class RTVEc : DownloadAbstract
    {
        private ShowsClan value;
        private EpisodesClan episodesClan;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="lBoxContent">The ListBox in which the content is displayed in</param>
        public RTVEc(ListBox lBoxContent) : base(lBoxContent) { }

        /// <summary>
        /// Fills the listbox with the JSON from RTVEClan search 
        /// </summary>
        public override void FillShowsList()
        {
            WebRequest reqSearchJs = HttpWebRequest.Create(@"http://www.rtve.es/infantil/buscador-clan/obtener-datos-programas.json");
            WebResponse resSearchJs = reqSearchJs.GetResponse();

            using (StreamReader srjs = new StreamReader(resSearchJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                value = jss.Deserialize<ShowsClan>(jsonjs);
                value.infoBuscador = value.infoBuscador.OrderBy(x => x.titulo).ToList(); //Maybe change to date published 
            }
            resSearchJs.Close();
        }
        /// <summary>
        /// Sets the object list to the episodes for RTVEClan
        /// </summary>
        public override void SetActive()
        {
            listBoxContent.ItemsSource = value.infoBuscador;
        }

        /// <summary>
        /// Handles clicking on a show and setting the listbox to the episodes for the show. 
        /// </summary>
        /// <returns>The name of the selected show</returns>
        public override String ClickDisplayedShow(int selectedIndex)
        {
            WebRequest reqTematicasJs = HttpWebRequest.Create("http://www.rtve.es/api/tematicas/" + value.infoBuscador[selectedIndex].id + "/videos.json");
            WebResponse resTematicasJs = reqTematicasJs.GetResponse();

            using (StreamReader srjs = new StreamReader(resTematicasJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                episodesClan = jss.Deserialize<EpisodesClan>(jsonjs);
                episodesClan.page.items = episodesClan.page.items.OrderBy(x => x.publicationDate).ToList();
                            
            }

            String selectedShow = value.infoBuscador[selectedIndex].titulo;
            //Clean the name for windows
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                selectedShow = selectedShow.Replace(c, '-');
            }
            listBoxContent.ItemsSource = episodesClan.page.items;
            resTematicasJs.Close();
            return selectedShow;
        }
        /// <summary>
        /// Gets the url for the selected object in the list
        /// Depricated, used generateUrl instead
        /// </summary>
        /// <returns></returns>
        public String GetUrlOld()
        {
            WebRequest reqTematicasJs = HttpWebRequest.Create("http://www.rtve.es/ztnr/movil/thumbnail/default/videos/" + episodesClan.page.items[listBoxContent.SelectedIndex].id + ".png");
            //reqTematicasJs.Headers.Add("Referer", episodesClan.page.items[objectList.SelectedIndex].htmlUrl);
            WebResponse resTematicasJs = reqTematicasJs.GetResponse();
            string base64 = "";
            using (StreamReader srjs = new StreamReader(resTematicasJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                base64 = srjs.ReadToEnd();
            }
            resTematicasJs.Close();
            return GetUrlFromPNGUrl(base64);
        }
        /// <summary>
        /// Genrates a downloadURL for rtve clan
        /// Based off work from rtvealacarta by itorres and personal research
        /// </summary>
        /// <returns>The url to download from</returns>
        public override DownloadObject GetDownloadObject()
        {
            //Create a phase conatining the video id and milliseconds since the unix epoch
            DateTime dt = DateTime.Now;
            DateTime epoch = new DateTime(1970, 1, 1);
            String joined = episodesClan.page.items[listBoxContent.SelectedIndex].id + "_es_" + (dt - epoch).TotalMilliseconds;
            

            RijndaelManaged aesEncrypt = new RijndaelManaged(); 
            //Set up the key and phrase to encrypt
            String passPhrase = "pmku579tg465GDjf1287gDFFED56788C"; // key for the "oceano"/tablet url
            byte[] key = new System.Text.UTF8Encoding().GetBytes(passPhrase);
            byte[] toEncrypt = new System.Text.UTF8Encoding().GetBytes(joined);

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
        /// Depreciate method for grabbing the URL (as it doesnt get the highest quality.)
        /// </summary>
        private String GetUrlFromPNGUrl(String text)
        {    
            Regex rer = new Regex(@"tEXt(.*)#.([0-9]*)"); //Search for the two piece of data that we need
            byte[] data = Convert.FromBase64String(text);
            String decodedString = Encoding.UTF8.GetString(data);
            //remove junk data which messes up regex, well most of it anyway
            decodedString = Regex.Replace(decodedString, @"[^\u0000-\u007F]", string.Empty);
            MatchCollection matchBand = rer.Matches(decodedString);

            //The different sections of the png to decode
            String group1 = matchBand[0].Groups[1].Value;
            String group2 = matchBand[0].Groups[2].Value;
            //Port of the code found in youtube-dl, which its self based off stuff elsewhere.
            String alphabet = "";
            int e = 0, d = 0;
            foreach (char l in group1)
            {
                if (d == 0)
                {
                    alphabet += (l);
                    d = e = (e + 1) % 4;
                }
                else
                    d -= 1;
            }
            String url = "";
            int f = 0, b = 1;
            e = 3;
            int lint = 0;
            foreach (char letter in group2)
            {
                if (f == 0)
                {
                    lint = int.Parse(letter.ToString()) * 10; //Char -> string -> int yep
                    f = 1;
                }

                else
                {
                    if (e == 0)
                    {
                        lint += int.Parse(letter.ToString());
                        url += alphabet[lint];
                        e = (b + 3) % 4;
                        f = 0;
                        b += 1;
                    }
                    else
                    {
                        e -= 1;
                    }
                }
            }

            return url;
        }
        /// <summary>
        /// Handles Clearing the episode list and reseting it back to the show list
        /// </summary>
        public override void CleanEpisodes()
        {
            episodesClan = null;
        }

        public override String GetSelectedName()
        {
            return episodesClan.page.items[listBoxContent.SelectedIndex].ToString();
        }
        public override String GetSubtitles()
        {
            return "";
        }
        public override List<object> GetShowsList()
        {
            return value.infoBuscador.ToList<object>();
        }
    }
}
