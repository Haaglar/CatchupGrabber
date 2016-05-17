using System.Collections.Generic;

namespace CatchupGrabber.Shows.Prima
{

    public class ProductSubcategory
    {
        public string key { get; set; }
        public string title { get; set; }
    }

    public class Country
    {
        public string key { get; set; }
        public string title { get; set; }
    }

    public class Language
    {
        public string key { get; set; }
        public string title { get; set; }
    }

    public class Genre
    {
        public string key { get; set; }
        public string title { get; set; }
    }

    public class VideoLanguage
    {
        public string key { get; set; }
        public string title { get; set; }
    }

    public class Images
    {
        public string splash169 { get; set; }
        public string splashWeb { get; set; }
        public string splashAWeb { get; set; }
        public string coverWeb { get; set; }
        public string cover169 { get; set; }
    }

    public class AgeRating
    {
        public string certification { get; set; }
        public int age { get; set; }
        public bool inappropriate { get; set; }
        public string key { get; set; }
        public string title { get; set; }
    }

    public class Result
    {
        public string id { get; set; }
        public string externalId { get; set; }
        public string localTitle { get; set; }
        public string originalTitle { get; set; }
        public string requestedStatus { get; set; }
        public string broadcastDate { get; set; }
        public string productCategory { get; set; }
        public List<ProductSubcategory> productSubcategories { get; set; }
        public string seasonNumber { get; set; }
        public string episodeNumber { get; set; }
        public string annotation { get; set; }
        public int year { get; set; }
        public List<Country> countries { get; set; }
        public Language language { get; set; }
        public string aspectRatio { get; set; }
        public string totalDuration { get; set; }
        public List<Genre> genres { get; set; }
        public List<string> productSections { get; set; }
        public string geoLock { get; set; }
        public string slug { get; set; }
        public List<string> status { get; set; }
        public List<VideoLanguage> videoLanguages { get; set; }
        public Images images { get; set; }
        public int avgRating { get; set; }
        public string promoNotice { get; set; }
        public AgeRating ageRating { get; set; }
        public int? seasonsCount { get; set; }
        public int? episodesCount { get; set; }
        public string parentProductId { get; set; }
        public string parentProductExternalId { get; set; }
        public string seriesId { get; set; }
        public string seriesTitle { get; set; }
        public string seasonId { get; set; }
        public string seasonTitle { get; set; }
        public override string ToString()
        {
            return localTitle;
        }
    }

    public class Filter
    {
        public List<string> category { get; set; }
        public List<string> order { get; set; }
    }

    public class ShowsPrima
    {
        public List<Result> result { get; set; }
        public int totalSize { get; set; }
        public Filter filter { get; set; }
    }

}
