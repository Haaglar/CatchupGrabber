using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab.Downloader.RTE
{
    public class RTEShows
    {
        public string i { get; set; }//?
        public int n { get; set; }// Number of episodes
        public string id { get; set; }// Int show id, 
        public string v { get; set; }   //Name
        public override string ToString()
        {
            return v;
        }
    }
}
