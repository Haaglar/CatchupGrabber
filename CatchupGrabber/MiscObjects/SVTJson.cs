using System.Collections.Generic;

namespace CatchupGrabber.MiscObjects
{
    public class VideoReference
    {
        public string url { get; set; }
        public int bitrate { get; set; }
        public string playerType { get; set; }
    }

    public class SubtitleReference
    {
        public string url { get; set; }
    }

    public class Video
    {
        public List<VideoReference> videoReferences { get; set; }
        public List<SubtitleReference> subtitleReferences { get; set; }
        public int position { get; set; }
        public int materialLength { get; set; }
        public bool live { get; set; }
        public bool availableOnMobile { get; set; }
    }

    public class Statistics
    {
        public string client { get; set; }
        public string mmsClientNr { get; set; }
        public string context { get; set; }
        public string programId { get; set; }
        public string mmsCategory { get; set; }
        public string broadcastDate { get; set; }
        public string broadcastTime { get; set; }
        public string category { get; set; }
        public string statisticsUrl { get; set; }
        public string title { get; set; }
        public string folderStructure { get; set; }
    }

    public class Context
    {
        public string title { get; set; }
        public string programTitle { get; set; }
        public string thumbnailImage { get; set; }
        public string posterImage { get; set; }
        public string popoutUrl { get; set; }
        public string programVersionId { get; set; }
    }

    public class SVTJson
    {
        public int videoId { get; set; }
        public Video video { get; set; }
        public Statistics statistics { get; set; }
        public Context context { get; set; }
    }
}
