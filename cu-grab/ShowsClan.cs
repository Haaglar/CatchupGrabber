using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
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
    }

    public class ShowsClan
    {
        public string generado { get; set; }
        public string status { get; set; }
        public List<InfoBuscador> infoBuscador { get; set; }
    }
}
