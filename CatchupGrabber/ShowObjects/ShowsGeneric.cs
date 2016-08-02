namespace CatchupGrabber
{
    /// <summary>
    /// Generic Show list entry
    /// </summary>
    class ShowsGeneric
    {
        public string name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public override string ToString()
        {
            return name;
        }
        public ShowsGeneric(string N, string U)
        {
            name = N;
            url = U;
            description = "";
        }
        public ShowsGeneric(string N, string U, string D)
        {
            name = N;
            url = U;
            description = D;
        }
    }
}
