using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
{
    public class IOSRendition
    {
        public bool audioOnly { get; set; }
        public string defaultURL { get; set; }
        public int encodingRate { get; set; }
        public int frameHeight { get; set; }
        public int frameWidth { get; set; }
        public int mediaDeliveryType { get; set; }
        public bool remote { get; set; }
        public int size { get; set; }
        public string videoCodec { get; set; }
        public int videoContainer { get; set; }
    }

    public class Caption
    {
        public string URL { get; set; }
        public List<object> languages { get; set; }
    }

    public class CuePoint
    {
        public object assetId { get; set; }
        public bool forceStop { get; set; }
        public object id { get; set; }
        public object metadata { get; set; }
        public object metadataString { get; set; }
        public string name { get; set; }
        public double time { get; set; }
        public double timeMs { get; set; }
        public int type { get; set; }
        public int version { get; set; }
        public object videoId { get; set; }
    }

    public class CustomFields
    {
        public string start_date_nt { get; set; }
        public string start_date_qld { get; set; }
        public string end_date_act { get; set; }
        public string end_date_nsw { get; set; }
        public string start_date_act { get; set; }
        public string title { get; set; }
        public string created { get; set; }
        public string start_date_tas { get; set; }
        public string priority { get; set; }
        public string published { get; set; }
        public string end_date_wa { get; set; }
        public string end_date_sa { get; set; }
        public string start_date_nsw { get; set; }
        public string key_number { get; set; }
        public string modified_time { get; set; }
        public string taxonomy_ids { get; set; }
        public string copyright { get; set; }
        public string end_date_qld { get; set; }
        public string start_date_sa { get; set; }
        public string source { get; set; }
        public string start_date_wa { get; set; }
        public string start_date_vic { get; set; }
        public string end_date_nt { get; set; }
        public string rating { get; set; }
        public string end_date_tas { get; set; }
        public string end_date_vic { get; set; }
    }

    public class EndDate
    {
        public string date { get; set; }
        public int day { get; set; }
        public int hour { get; set; }
        public string meridian { get; set; }
        public int milliseconds { get; set; }
        public int minutes { get; set; }
        public int month { get; set; }
        public int seconds { get; set; }
        public string timezone { get; set; }
        public int year { get; set; }
    }

    public class StartDate
    {
        public string date { get; set; }
        public int day { get; set; }
        public int hour { get; set; }
        public string meridian { get; set; }
        public int milliseconds { get; set; }
        public int minutes { get; set; }
        public int month { get; set; }
        public int seconds { get; set; }
        public string timezone { get; set; }
        public int year { get; set; }
    }

    public class Tag
    {
        public object image { get; set; }
        public string name { get; set; }
    }

    public class BCoveJson
    {
        public int FLVFullCodec { get; set; }
        public bool FLVFullLengthRemote { get; set; }
        public bool FLVFullLengthStreamed { get; set; }
        public string FLVFullLengthURL { get; set; }
        public int FLVFullSize { get; set; }
        public int FLVPreBumperControllerType { get; set; }
        public bool FLVPreBumperStreamed { get; set; }
        public object FLVPreBumperURL { get; set; }
        public int FLVPreviewCodec { get; set; }
        public int FLVPreviewSize { get; set; }
        public bool FLVPreviewStreamed { get; set; }
        public object FLVPreviewURL { get; set; }
        public object HDSRenditions { get; set; }
        public List<IOSRendition> IOSRenditions { get; set; }
        public bool SWFVerificationRequired { get; set; }
        public object WMVFullAssetId { get; set; }
        public object WMVFullLengthURL { get; set; }
        public object adCategories { get; set; }
        public object adKeys { get; set; }
        public bool allowViralSyndication { get; set; }
        public List<string> allowedCountries { get; set; }
        public object awards { get; set; }
        public List<Caption> captions { get; set; }
        public List<object> categories { get; set; }
        public object color { get; set; }
        public int controllerType { get; set; }
        public string creationDate { get; set; }
        public List<CuePoint> cuePoints { get; set; }
        public object customFieldValues { get; set; }
        public CustomFields customFields { get; set; }
        public bool dateFiltered { get; set; }
        public string displayName { get; set; }
        public object drmMetadataURL { get; set; }
        public int economics { get; set; }
        public int encodingRate { get; set; }
        public EndDate endDate { get; set; }
        public bool excludeListedCountries { get; set; }
        public object filterEndDate { get; set; }
        public object filterStartDate { get; set; }
        public bool forceAds { get; set; }
        public bool geoRestricted { get; set; }
        public string hdsManifestUrl { get; set; }
        public long id { get; set; }
        public bool isSubmitted { get; set; }
        public object language { get; set; }
        public int length { get; set; }
        public object lineupId { get; set; }
        public object linkText { get; set; }
        public string linkURL { get; set; }
        public object logoOverlay { get; set; }
        public string longDescription { get; set; }
        public object monthlyAmount { get; set; }
        public int numberOfPlays { get; set; }
        public int previewLength { get; set; }
        public string publishedDate { get; set; }
        public long publisherId { get; set; }
        public string publisherName { get; set; }
        public object purchaseAmount { get; set; }
        public object ratingEnum { get; set; }
        public string referenceId { get; set; }
        public List<object> renditions { get; set; }
        public object rentalAmount { get; set; }
        public object rentalPeriod { get; set; }
        public object sharedBy { get; set; }
        public bool sharedByExternalAcct { get; set; }
        public object sharedSourceId { get; set; }
        public bool sharedToExternalAcct { get; set; }
        public string shortDescription { get; set; }
        public StartDate startDate { get; set; }
        public bool submitted { get; set; }
        public List<Tag> tags { get; set; }
        public string thumbnailURL { get; set; }
        public object version { get; set; }
        public string videoStillURL { get; set; }
        public object yearProduced { get; set; }
    }
}
