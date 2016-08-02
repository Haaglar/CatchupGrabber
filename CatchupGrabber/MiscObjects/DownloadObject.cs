namespace CatchupGrabber
{
    public enum Country { Aus,Ireland,Italy,Spain, Sweden, Czech, USA}
    public enum DownloadMethod {HTTP, HLS}

    /// <summary>
    /// Used to handle download types and methods yadda
    /// </summary>
    public class DownloadObject
    {
        public string EpisodeUrl { get; set; }
        public string SubtitleUrl { get; set; }
        public Country CountryOfOrigin { get; set; }
        public DownloadMethod DlMethod { get; set; }
        public DownloadObject(string eUrl, string sUrl, Country countryO, DownloadMethod downlMethod)
        {
            EpisodeUrl = eUrl;
            SubtitleUrl = sUrl;
            CountryOfOrigin = countryO;
            DlMethod = downlMethod;
        }
    }
}
