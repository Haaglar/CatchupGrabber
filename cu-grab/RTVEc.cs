using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Controls;

namespace cu_grab
{
    class RTVEc
    {
        private static ShowsClan value;
        public static bool requested = false;

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
                value.infoBuscador = value.infoBuscador.OrderBy(x => x.titulo).ToList();
                objectList.ItemsSource = value.infoBuscador;
            }
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
    }
}
