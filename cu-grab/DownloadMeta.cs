using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
{
    public enum Country { Aus,Ireland,Spain}
    public enum DownloadMethod {HTTP, HLS}
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
