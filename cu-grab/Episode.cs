using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Class for the epiosdes of a show on tenplay, plus7 and RTE
namespace cu_grab
{
    public class Episode
    {
        public string Name { get; set; }
        public string EpisodeID { get; set; }

        public Episode(string N, string E)
        {
            Name = N;
            EpisodeID = E;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
