using System.Collections.Generic;

namespace CatchupGrabber.MiscObjects.Prima
{

    public class Lang
    {
        public string key { get; set; }
        public string title { get; set; }
    }

    public class StreamInfo
    {
        public string type { get; set; }
        public object drm { get; set; }
        public Lang lang { get; set; }
        public string url { get; set; }
        public List<string> profiles { get; set; }
    }

    public class ThumbnailInfo
    {
        public string url { get; set; }
    }

    public class PrimaJson
    {
        public bool adsEnabled { get; set; }
        public string adSection { get; set; }
        public List<string> adKeywords { get; set; }
        public int videoAdsPreRoll { get; set; }
        public int videoAdsMidRoll { get; set; }
        public List<int> videoAdsMidRollPositions { get; set; }
        public int videoAdsPostRoll { get; set; }
        public List<object> subInfos { get; set; }
        public List<StreamInfo> streamInfos { get; set; }
        public ThumbnailInfo thumbnailInfo { get; set; }
        public List<string> preRolls { get; set; }
        public string preFullscreen { get; set; }
        public string preOverlay { get; set; }
        public string midFullscreen { get; set; }
        public string midOverlay { get; set; }
        public List<string> postRolls { get; set; }
    }

}
