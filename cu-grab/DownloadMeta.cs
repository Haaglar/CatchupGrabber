using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
{
    enum Country { Aus,Ireland,Spain}
    enum DownloadMethod {HTTP, HLS}
    public class DownloadMeta
    {
        String episodeUrl;
        String subtitleUrl;
        Country country;
        DownloadMethod dlMethod;
        public DownloadMeta(String eUrl, String sUrl, Country countryO, DownloadMethod downlMethod)
        {
            episodeUrl = eUrl;
            subtitleUrl = sUrl;
            country = countryO;
            dlMethod = downlMethod;
        }
    }
}
