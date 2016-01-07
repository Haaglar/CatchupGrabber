using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
{

    public class Channel
    {
        public string uri { get; set; }
        public string htmlUrl { get; set; }
        public string htmlShortUrl { get; set; }
        public string id { get; set; }
        public string medioRef { get; set; }
        public string uid { get; set; }
        public string title { get; set; }
        public string permalink { get; set; }
        public string programsRef { get; set; }
        public string videosRef { get; set; }
        public string audiosRef { get; set; }
        public string multimediasRef { get; set; }
        public string directosAhoraRef { get; set; }
        public string directosEnvivoAhoraRef { get; set; }
        public string directosTodosAhoraRef { get; set; }
        public string directosProximosRef { get; set; }
        public string directosEnvivoProximosRef { get; set; }
        public string directosTodosProximosRef { get; set; }
        public string agrupadoresRef { get; set; }
        public string videosVistosRef { get; set; }
        public string audiosVistosRef { get; set; }
        public string multimediasVistosRef { get; set; }
        public string videosPopularesRef { get; set; }
        public string audiosPopularesRef { get; set; }
        public string multimediasPopularesRef { get; set; }
    }

    public class Item
    {
        public string uri { get; set; }
        public string htmlUrl { get; set; }
        public string htmlShortUrl { get; set; }
        public string id { get; set; }
        public int ctvId { get; set; }
        public string uid { get; set; }
        public string name { get; set; }
        public string permalink { get; set; }
        public string language { get; set; }
        public string longTitle { get; set; }
        public string shortTitle { get; set; }
        public string description { get; set; }
        public string longDescription { get; set; }
        public string showMan { get; set; }
        public string director { get; set; }
        public object contact { get; set; }
        public string emission { get; set; }
        public string publicationDate { get; set; }
        public object expirationDate { get; set; }
        public object orden { get; set; }
        public string thumbnail { get; set; }
        public string logo { get; set; }
        public string imgBackground { get; set; }
        public string imgPoster { get; set; }
        public string imgPortada { get; set; }
        public bool outOfEmission { get; set; }
        public Channel channel { get; set; }
        public string fanBoxID { get; set; }
        public string ageRangeUid { get; set; }
        public string ageRange { get; set; }
        public List<object> recommendAgesForChilds { get; set; }
        public string seccionesRef { get; set; }
        public string temporadasRef { get; set; }
        public string agrupadoresRef { get; set; }
        public string videosRef { get; set; }
        public string audiosRef { get; set; }
        public string multimediasRef { get; set; }
        public string relacionadosRef { get; set; }
        public string otherChannelsRef { get; set; }
        public string childrenInfoRef { get; set; }
        public object filtro { get; set; }
        public string webRtve { get; set; }
        public string webOficial { get; set; }
        public string relatedByLangRef { get; set; }
        public object publicationDateTimestamp { get; set; }
        public string contentType { get; set; }
        public string imageSEO { get; set; }
        public string imgPinta { get; set; }
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

    public class ShowsRTVE
    {
        public Page page { get; set; }
    }

}
