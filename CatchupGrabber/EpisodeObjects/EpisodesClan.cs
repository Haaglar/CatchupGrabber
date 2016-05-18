using System.Collections.Generic;

//A class for the episode of a show that is on letsclan
namespace CatchupGrabber.EpisodeObjects.RTVEc
{
    public class PubState
    {
        public string code { get; set; }
        public string description { get; set; }
    }

    public class Statistics
    {
        public int numComentarios { get; set; }
        public int numCompartidas { get; set; }
    }

    public class Quality
    {
        public int identifier { get; set; }
        public string filePath { get; set; }
        public string preset { get; set; }
        public int filesize { get; set; }
        public string type { get; set; }
        public int duration { get; set; }
        public int bitRate { get; set; }
        public string bitRateUnit { get; set; }
        public string language { get; set; }
        public string previewPath { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }

    public class ProgramInfo
    {
        public string title { get; set; }
        public string htmlUrl { get; set; }
        public string channelPermalink { get; set; }
        public string ageRangeUid { get; set; }
        public string ageRange { get; set; }
    }

    public class Type
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Sign
    {
        public object ctvId { get; set; }
        public object name { get; set; }
        public object photo { get; set; }
        public string firma { get; set; }
        public object twitter { get; set; }
        public object facebook { get; set; }
        public object googlePlus { get; set; }
    }

    public class Item
    {
        public string uri { get; set; }
        public string htmlUrl { get; set; }
        public string htmlShortUrl { get; set; }
        public string id { get; set; }
        public string language { get; set; }
        public string longTitle { get; set; }
        public string shortTitle { get; set; }
        public string mainCategoryRef { get; set; }
        public string popularity { get; set; }
        public string popHistoric { get; set; }
        public string numVisits { get; set; }
        public string publicationDate { get; set; }
        public string expirationDate { get; set; }
        public string modificationDate { get; set; }
        public PubState pubState { get; set; }
        public string breadCrumbRef { get; set; }
        public string imageSEO { get; set; }
        public object publicationDateTimestamp { get; set; }
        public string contentType { get; set; }
        public Statistics statistics { get; set; }
        public string alt { get; set; }
        public string foot { get; set; }
        public string shortDescription { get; set; }
        public string description { get; set; }
        public string otherTopicsRefs { get; set; }
        public List<Quality> qualities { get; set; }
        public string qualitiesRef { get; set; }
        public string mainTopic { get; set; }
        public List<object> topicsName { get; set; }
        public int duration { get; set; }
        public string consumption { get; set; }
        public string dateOfEmission { get; set; }
        public string thumbnail { get; set; }
        public ProgramInfo programInfo { get; set; }
        public object sgce { get; set; }
        public object commentOptions { get; set; }
        public string cuePointsRef { get; set; }
        public string configPlayerRef { get; set; }
        public string transcriptionRef { get; set; }
        public string temporadasRef { get; set; }
        public Type type { get; set; }
        public string programRef { get; set; }
        public string relacionadosRef { get; set; }
        public string relManualesRef { get; set; }
        public string publicidadRef { get; set; }
        public string comentariosRef { get; set; }
        public string relatedByLangRef { get; set; }
        public Sign sign { get; set; }
        public string estadisticasRef { get; set; }
        public object ageRangeUid { get; set; }
        public object ageRange { get; set; }
        public int episode { get; set; }
        public string subtitleRef { get; set; }
        public string assetType { get; set; }
        public string title { get; set; }
        public override string ToString()
        {
            return longTitle;
        }
    }

    public class Page
    {
        public List<Item> items { get; set; }
        public int number { get; set; }
        public int size { get; set; }
        public int offset { get; set; }
        public int total { get; set; }
        public int totalPages { get; set; }
        public int numElements { get; set; }
    }

    public class EpisodesClan
    {
        public Page page { get; set; }
    }
}
