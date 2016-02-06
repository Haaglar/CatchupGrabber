using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
{
    /// <summary>
    /// Generic Show list entry
    /// </summary>
    class ShowsGeneric
    {
        public string name { get; set; }
        public string url { get; set; }
        public override string ToString()
        {
            return name;
        }
        public ShowsGeneric(string N, string U)
        {
            name = N;
            url = U;
        }
    }
}
