using System.Collections.Generic;

//A class for the Shows that are on letsclan
namespace CatchupGrabber
{
    public class InfoBuscador
    {
        public string id { get; set; }
        public string titulo { get; set; }
        public string personajes { get; set; }
        public string url { get; set; }
        public bool hasActiveGames { get; set; }
        public bool hasActiveAct { get; set; }
        public bool hasActiveQuiz { get; set; }
        public bool hasVideos { get; set; }
        public bool hasForeignVideos { get; set; }
        public string permalink { get; set; }
        public List<object> recommendAgesForChilds { get; set; }
        public override string ToString()
        {
            return titulo;
        }
    }

    public class ShowsClan
    {
        public string generado { get; set; }
        public string status { get; set; }
        public List<InfoBuscador> infoBuscador { get; set; }
    }
}
