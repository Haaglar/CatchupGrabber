//Class for the epiosdes of a show on tenplay, plus7 and RTE
namespace cu_grab
{
    public class EpisodesGeneric
    {
        public string Name { get; set; }
        public string EpisodeID { get; set; }

        public EpisodesGeneric(string name, string episodeID)
        {
            Name = name;
            EpisodeID = episodeID;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
