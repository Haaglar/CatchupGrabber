using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
{
    public enum Country { Aus,Ireland,Spain}
    public enum DownloadMethod {HTTP, HLS}

    /// <summary>
    /// Used to handle download types and methods yadda
    /// </summary>
    public class DownloadObject
    {
        public String EpisodeUrl { get; set; }
        public String SubtitleUrl { get; set; }
        public Country CountryOfOrigin { get; set; }
        public DownloadMethod DlMethod { get; set; }
        public DownloadObject(String eUrl, String sUrl, Country countryO, DownloadMethod downlMethod)
        {
            EpisodeUrl = eUrl;
            SubtitleUrl = sUrl;
            CountryOfOrigin = countryO;
            DlMethod = downlMethod;
        }
    }
}
