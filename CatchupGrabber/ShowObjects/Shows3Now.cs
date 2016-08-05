using System.Collections.Generic;

/// <summary>
/// Auto Generated using json2csharp.
/// tostring overide added
/// </summary>
namespace CatchupGrabber.ShowObjects._3Now
{

    public class Channel
    {
        public string slug { get; set; }
        public string displayName { get; set; }
        public string logo { get; set; }
    }

    public class Genre
    {
        public string slug { get; set; }
        public string displayName { get; set; }
    }

    public class Images
    {
        public string dashboardHero { get; set; }
        public string showHero { get; set; }
        public string showTile { get; set; }
    }

    public class Show
    {
        public string showId { get; set; }
        public string name { get; set; }
        public Images images { get; set; }
        public string channel { get; set; }
        public List<object> genres { get; set; }
        public string synopsis { get; set; }
        public bool partial { get; set; }
        public string website { get; set; }
        public object marketingType { get; set; }
        public override string ToString()
        {
            return name;
        }
    }

    public class Shows3Now
    {
        public List<Channel> channels { get; set; }
        public List<Genre> genres { get; set; }
        public List<Show> shows { get; set; }
    }

}
