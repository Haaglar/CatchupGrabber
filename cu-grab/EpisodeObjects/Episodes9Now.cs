using System.Collections.Generic;

namespace cu_grab.EpisodeObjects._9Now
{

    public class Meta
    {
        public string title { get; set; }
        public string description { get; set; }
        public object keywords { get; set; }
    }

    public class Sizes
    {
        public string w320 { get; set; }
        public string w480 { get; set; }
        public string w768 { get; set; }
        public string w1280 { get; set; }
        public string w1920 { get; set; }
    }

    public class Image
    {
        public string alt { get; set; }
        public Sizes sizes { get; set; }
    }

    public class TvSeries
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string countryOfOrigin { get; set; }
        public Image image { get; set; }
    }

    public class Link
    {
        public string type { get; set; }
        public int id { get; set; }
        public string webUrl { get; set; }
    }

    public class Sizes2
    {
        public string w320 { get; set; }
        public string w480 { get; set; }
        public string w768 { get; set; }
        public string w1280 { get; set; }
        public string w1920 { get; set; }
    }

    public class Image2
    {
        public string alt { get; set; }
        public Sizes2 sizes { get; set; }
    }

    public class Sizes3
    {
        public string w320 { get; set; }
        public string w480 { get; set; }
        public string w768 { get; set; }
        public string w1280 { get; set; }
        public string w1920 { get; set; }
    }

    public class Image3
    {
        public string alt { get; set; }
        public Sizes3 sizes { get; set; }
    }

    public class Channel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string colour { get; set; }
        public Image3 image { get; set; }
    }

    public class SocialNetworks
    {
        public object facebook { get; set; }
        public object twitter { get; set; }
        public object instagram { get; set; }
    }

    public class Season
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public object onAirScheduleDate { get; set; }
        public Link link { get; set; }
        public string description { get; set; }
        public string catalogCode { get; set; }
        public Image2 image { get; set; }
        public Channel channel { get; set; }
        public SocialNetworks socialNetworks { get; set; }
        public object extrasUrl { get; set; }
    }

    public class Sizes4
    {
        public object w320 { get; set; }
        public object w480 { get; set; }
        public object w768 { get; set; }
        public object w1280 { get; set; }
        public object w1920 { get; set; }
    }

    public class Image4
    {
        public object alt { get; set; }
        public Sizes4 sizes { get; set; }
    }

    public class Genre
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public Image4 image { get; set; }
    }

    public class Link2
    {
        public string type { get; set; }
        public int id { get; set; }
        public string webUrl { get; set; }
    }

    public class Season2
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public Link2 link { get; set; }
    }

    public class Link3
    {
        public string type { get; set; }
        public int id { get; set; }
        public string webUrl { get; set; }
    }

    public class CallToAction
    {
        public string text { get; set; }
        public Link3 link { get; set; }
    }

    public class ClipTag
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
    }

    public class Link4
    {
        public string type { get; set; }
        public object id { get; set; }
        public string webUrl { get; set; }
    }

    public class CallToAction2
    {
        public string text { get; set; }
        public Link4 link { get; set; }
    }

    public class Link5
    {
        public string type { get; set; }
        public int id { get; set; }
        public string webUrl { get; set; }
    }

    public class Sizes5
    {
        public string w320 { get; set; }
        public string w480 { get; set; }
        public string w768 { get; set; }
        public string w1280 { get; set; }
        public string w1920 { get; set; }
    }

    public class Image5
    {
        public string alt { get; set; }
        public Sizes5 sizes { get; set; }
    }

    public class BrightcoveOnce
    {
        public string onceuxUrl { get; set; }
        public string onceUrl { get; set; }
    }

    public class CuePoint
    {
        public double time { get; set; }
    }

    public class Video
    {
        public int id { get; set; }
        public bool drm { get; set; }
        public string referenceId { get; set; }
        public BrightcoveOnce brightcoveOnce { get; set; }
        public int duration { get; set; }
        public List<CuePoint> cuePoints { get; set; }
    }

    public class PartOfSeries
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
    }

    public class PartOfSeason
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public string catalogCode { get; set; }
    }

    public class Genre2
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
    }

    public class Item2
    {
        public string type { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
        public string slug { get; set; }
        public int episodeNumber { get; set; }
        public Link5 link { get; set; }
        public string description { get; set; }
        public Image5 image { get; set; }
        public string availability { get; set; }
        public string expiry { get; set; }
        public string airDate { get; set; }
        public string classification { get; set; }
        public Video video { get; set; }
        public PartOfSeries partOfSeries { get; set; }
        public PartOfSeason partOfSeason { get; set; }
        public Genre2 genre { get; set; }
        public string oztamMediaId { get; set; }
        public string oztamPublisherId { get; set; }
        public List<object> tags { get; set; }
        public object partOfEpisode { get; set; }
        public object oztamEpisodeRelationship { get; set; }
        public override string ToString()
        {
            return name;
        }
    }

    public class Item
    {
        public string type { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public CallToAction2 callToAction { get; set; }
        public List<Item2> items { get; set; }
    }

    public class Episodes9Now
    {
        public Meta meta { get; set; }
        public TvSeries tvSeries { get; set; }
        public Season season { get; set; }
        public Genre genre { get; set; }
        public List<Season2> seasons { get; set; }
        public CallToAction callToAction { get; set; }
        public List<ClipTag> clipTags { get; set; }
        public List<Item> items { get; set; }
    }

}
