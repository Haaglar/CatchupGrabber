using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
{
    public class LastestEpisode
    {
        public string ScId { get; set; }
        public string BcId { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public object ExtraInfo { get; set; }
    }

    public class Show
    {
        public string ScId { get; set; }
        public string PgId { get; set; }
        public string Name { get; set; }
        public string LogoURL { get; set; }
        public string ShowURL { get; set; }
        public int NumberOfEpisodes { get; set; }
        public LastestEpisode LastestEpisode { get; set; }
        public object ExtraInfo { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class RootObject
    {
        public List<Show> Shows { get; set; }
    }  
}
