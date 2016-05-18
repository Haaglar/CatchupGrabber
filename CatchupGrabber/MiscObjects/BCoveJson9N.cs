using System.Collections.Generic;

namespace CatchupGrabber.MiscObjects._9Now
{

    public class IOSRendition
    {
        public bool audioOnly { get; set; }
        public string controllerType { get; set; }
        public string displayName { get; set; }
        public int encodingRate { get; set; }
        public int frameHeight { get; set; }
        public int frameWidth { get; set; }
        public object id { get; set; }
        public object referenceId { get; set; }
        public object remoteStreamName { get; set; }
        public object remoteUrl { get; set; }
        public int size { get; set; }
        public object uploadTimestampMillis { get; set; }
        public string url { get; set; }
        public string videoCodec { get; set; }
        public string videoContainer { get; set; }
        public int videoDuration { get; set; }
    }

    public class VideoFullLength
    {
        public bool audioOnly { get; set; }
        public string controllerType { get; set; }
        public string displayName { get; set; }
        public int encodingRate { get; set; }
        public int frameHeight { get; set; }
        public int frameWidth { get; set; }
        public long id { get; set; }
        public object referenceId { get; set; }
        public object remoteStreamName { get; set; }
        public object remoteUrl { get; set; }
        public int size { get; set; }
        public long uploadTimestampMillis { get; set; }
        public string url { get; set; }
        public string videoCodec { get; set; }
        public string videoContainer { get; set; }
        public int videoDuration { get; set; }
    }

    public class BCoveJson9N
    {
        public object shortDescription { get; set; }
        public string HLSURL { get; set; }
        public List<IOSRendition> IOSRenditions { get; set; }
        public long accountId { get; set; }
        public VideoFullLength videoFullLength { get; set; }
        public List<object> WVMRenditions { get; set; }
    }

}
