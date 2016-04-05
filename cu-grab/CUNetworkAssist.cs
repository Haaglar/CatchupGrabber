using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace cu_grab
{
    namespace NetworkAssister
    {
        //Will contain a list of network related classes
        public class CUNetworkAssist
        {
            public CUNetworkAssist() { }

            /// <summary>
            /// Gets the highest bitrate from a m3u8 url
            /// </summary>
            /// <param name="masterUrl">The arrdess to the Master m3u8</param>
            /// <returns>The highest bitrate value</returns>
            public String GetHighestM3U8Address(String masterUrl)
            {
                String finalUrl = "";
                WebRequest reqManifest = HttpWebRequest.Create(masterUrl);
                using (WebResponse resManifest = reqManifest.GetResponse())
                {
                    using (Stream responseStreamManifest = resManifest.GetResponseStream())
                    {
                        using (StreamReader srShowManifest = new StreamReader(responseStreamManifest, System.Text.Encoding.UTF8))
                        {
                            String line; // current line 

                            Regex regexBandwidth = new Regex(@"(?<=\bBANDWIDTH=)([0-9]+)"); //Quality Selection
                            int index = 0;
                            int row = -1;
                            long bandwidth = 0;
                            long tmp = 0;
                            //Get the highest quality link
                            while ((line = srShowManifest.ReadLine()) != null)
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
                            }//End while
                        }//End StreamReader
                    }// End stream
                }//End Webresponse
                return finalUrl;
            }

        }

        /// <summary>
        /// A CookieAwareWebClient, used to store Glype proxy seesion info. Since we cant Async download when we need to post data at the same time.
        /// Thanks to http://stackoverflow.com/questions/4740752/how-do-i-log-into-a-site-with-webclient
        /// </summary>
        public class CookieAwareWebClient : WebClient
        {
            public CookieAwareWebClient()
            {
                CookieContainer = new CookieContainer();
            }
            public CookieContainer CookieContainer { get; private set; }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = (HttpWebRequest)base.GetWebRequest(address);
                request.CookieContainer = CookieContainer;
                return request;
            }
        }
    }
}