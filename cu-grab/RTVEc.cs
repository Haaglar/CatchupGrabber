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
    class RTVEc
    {
        private ShowsClan value;
        private EpisodesClan episodesClan;
        private ListBox objectList;
        public RTVEc(ListBox oList)
        {
            objectList = oList;
        }

        /// <summary>
        /// Fills the listbox with the JSON from RTVEClan search 
        /// </summary>
        /// <param name="objectList"></param>
        public void fillShowsList()
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
            objectList.ItemsSource = value.infoBuscador;
            resSearchJs.Close();
        }
        /// <summary>
        /// Sets the object list to the episodes for RTVEClan
        /// </summary>
        /// <param name="objectList"></param>
        public void setRTVEcActive()
        {
            objectList.ItemsSource = value.infoBuscador;
        }
        /// <summary>
        /// Handles clicking on a show and setting the listbox to the episodes for the show. 
        /// </summary>
        /// <param name="objectList"></param>
        public String clickDisplayedShow()
        {
            WebRequest reqTematicasJs = HttpWebRequest.Create("http://www.rtve.es/api/tematicas/" + value.infoBuscador[objectList.SelectedIndex].id + "/videos.json");
            WebResponse resTematicasJs = reqTematicasJs.GetResponse();

            using (StreamReader srjs = new StreamReader(resTematicasJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                string jsonjs = srjs.ReadToEnd();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                episodesClan = jss.Deserialize<EpisodesClan>(jsonjs);
                episodesClan.page.items = episodesClan.page.items.OrderBy(x => x.publicationDate).ToList();
                            
            }

            String selectedShow = value.infoBuscador[objectList.SelectedIndex].titulo;
            //Clean the name for windows
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                selectedShow = selectedShow.Replace(c, '-');
            }
            objectList.ItemsSource = episodesClan.page.items;
            resTematicasJs.Close();
            return selectedShow;
        }
        /// <summary>
        /// Gets the url for the selected object in the list
        /// </summary>
        /// <param name="objectList"></param>
        /// <returns></returns>
        public String getUrl()
        {
            WebRequest reqTematicasJs = HttpWebRequest.Create("http://www.rtve.es/ztnr/movil/thumbnail/default/videos/" + episodesClan.page.items[objectList.SelectedIndex].id + ".png");
            //reqTematicasJs.Headers.Add("Referer", episodesClan.page.items[objectList.SelectedIndex].htmlUrl);
            WebResponse resTematicasJs = reqTematicasJs.GetResponse();
            string base64 = "";
            using (StreamReader srjs = new StreamReader(resTematicasJs.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                base64 = srjs.ReadToEnd();
            }
            resTematicasJs.Close();
            return getUrlFromPNGUrl(base64);
        }

        /// <summary>
        /// TODO Replace this with time method
        /// </summary>
        private String getUrlFromPNGUrl(String text)
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
        /// <param name="objectList"></param>
        public void cleanEpisodes()
        {
            objectList.ItemsSource = value.infoBuscador;
            episodesClan = null;
        }

        public String getSelectedName()
        {
            return episodesClan.page.items[objectList.SelectedIndex].ToString();
        }
    }
}
