using System.Collections.Generic;

namespace cu_grab.Shows._9Now
{

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

    public class Genre
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
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

    public class PartOfSeries
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class Genre2
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
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
        public string slug { get; set; }
        public string name { get; set; }
        public string colour { get; set; }
        public Image3 image { get; set; }
    }

    public class ContainsSeason
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string catalogCode { get; set; }
        public Image2 image { get; set; }
        public int seasonNumber { get; set; }
        public string onAirScheduleDate { get; set; }
        public string updatedAt { get; set; }
        public PartOfSeries partOfSeries { get; set; }
        public Genre2 genre { get; set; }
        public Channel channel { get; set; }
    }

    public class Item
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Image image { get; set; }
        public Genre genre { get; set; }
        public string countryOfOrigin { get; set; }
        public string updatedAt { get; set; }
        public List<ContainsSeason> containsSeason { get; set; }
        public override string ToString()
        {
            return name;
        }
    }

    public class Shows9Now
    {
        public object sort { get; set; }
        public int take { get; set; }
        public int skip { get; set; }
        public int count { get; set; }
        public List<Item> items { get; set; }
    }

}
