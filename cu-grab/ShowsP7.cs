using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
{
    public class ShowsP7
    {
        public string title { get; set; }
        public string url { get; set; }
        public override string ToString()
        {
            return title;
        }
    }
}
