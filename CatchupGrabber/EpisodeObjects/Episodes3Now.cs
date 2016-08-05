using System.Collections.Generic;

namespace CatchupGrabber.EpisodeObjects._3Now
{

    public class Images
    {
        public string dashboardHero { get; set; }
        public string showHero { get; set; }
        public string showTile { get; set; }
    }

    public class Once
    {
        public string url { get; set; }
        public object drm { get; set; }
        public string brightcoveId { get; set; }
    }

    public class VideoAdKeys
    {
        public string slotname { get; set; }
        public string sz { get; set; }
        public int cmsid { get; set; }
        public string vid { get; set; }
        public string genre { get; set; }
        public string cust_params { get; set; }
    }

    public class VideoCloud
    {
        public string brightcoveId { get; set; }
        public string drm { get; set; }
        public VideoAdKeys videoAdKeys { get; set; }
        public string hlsUrl { get; set; }
    }

    public class VideoRenditions
    {
        public Once once { get; set; }
        public VideoCloud videoCloud { get; set; }
    }

    public class Images2
    {
        public string videoTile { get; set; }
    }

    public class CuePoint
    {
        public double time { get; set; }
    }

    public class Episode
    {
        public string showId { get; set; }
        public string showTitle { get; set; }
        public string videoId { get; set; }
        public string name { get; set; }
        public string episode { get; set; }
        public string season { get; set; }
        public string type { get; set; }
        public string synopsis { get; set; }
        public VideoRenditions videoRenditions { get; set; }
        public string airedDate { get; set; }
        public string availableDate { get; set; }
        public string expiryDate { get; set; }
        public double duration { get; set; }
        public bool geoblock { get; set; }
        public Images2 images { get; set; }
        public string externalMediaId { get; set; }
        public List<CuePoint> cuePoints { get; set; }
        public override string ToString()
        {
            return name;
        }
    }

    public class HeroEpisode
    {
        public string title { get; set; }
        public string videoId { get; set; }
    }

    public class Show
    {
        public string showId { get; set; }
        public string name { get; set; }
        public Images images { get; set; }
        public string channel { get; set; }
        public List<string> genres { get; set; }
        public string synopsis { get; set; }
        public bool partial { get; set; }
        public string website { get; set; }
        public object marketingType { get; set; }
        public List<Episode> episodes { get; set; }
        public List<object> extras { get; set; }
        public HeroEpisode heroEpisode { get; set; }
    }

    public class Images3
    {
        public string dashboardHero { get; set; }
        public string showHero { get; set; }
        public string showTile { get; set; }
    }

    public class Item
    {
        public string showId { get; set; }
        public string name { get; set; }
        public Images3 images { get; set; }
        public string channel { get; set; }
        public List<string> genres { get; set; }
        public string synopsis { get; set; }
        public bool partial { get; set; }
        public string website { get; set; }
        public object marketingType { get; set; }
    }

    public class Section
    {
        public string title { get; set; }
        public List<Item> items { get; set; }
        public string presentationStyle { get; set; }
    }

    public class StaticAdParams
    {
        public string sz { get; set; }
        public string show { get; set; }
        public string sect1 { get; set; }
    }

    public class Configuration
    {
        public StaticAdParams staticAdParams { get; set; }
    }

    public class Episodes3Now
    {
        public Show show { get; set; }
        public List<Section> sections { get; set; }
        public Configuration configuration { get; set; }
    }

}
