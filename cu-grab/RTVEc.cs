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
        private static ShowsClan value;
        private static EpisodesClan episodesClan;
        public static bool requested = false;
        //idividual http://www.rtve.es/api/videos/2787795.json
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectList"></param>
        public static void fillShowsList(ListBox objectList)
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
            requested = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectList"></param>
        public static void setRTVEcActive(ListBox objectList)
        {
            objectList.ItemsSource = value.infoBuscador;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectList"></param>
        public static String clickDisplayedShow(ListBox objectList)
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
        /// TODO Replace this with time method
        /// </summary>
        public static void getUrlFromPNGUrl()
        {
            Regex rer = new Regex(@"tEXt(.*)#.([0-9]*)");
            String text = File.ReadAllText(@"C:\Users\");
            byte[] data = Convert.FromBase64String(text);
            String decodedString = Encoding.UTF8.GetString(data);
            //remove junk data which messes up regex
            decodedString = Regex.Replace(decodedString, @"[^\u0000-\u007F]", string.Empty);
            MatchCollection matchBand = rer.Matches(decodedString);

            //The different sections of the png to decode
            String group1 = matchBand[0].Groups[1].Value;
            String group2 = matchBand[0].Groups[2].Value;
            //Port of the code found in youtube-dl, which its self based of stuff elsewhere.
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
                    lint = int.Parse(letter.ToString()) * 10;
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
        }
    }
}
