using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
{
    //Will contain a list of network related classes
    public class CUNetworkAssist
    {

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
