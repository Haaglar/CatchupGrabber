using System;
using System.Collections.Generic;

namespace cu_grab
{
    abstract public class DownloadAbstract
    {
        public bool RequestedSiteData = false; //true after FillShowsList() is called
        abstract public string ClickDisplayedShow(int selectedIndex);
        abstract public void FillShowsList();
        abstract public string GetSelectedNameEpisode(int selectedIndex);
        abstract public DownloadObject GetDownloadObject(int selectedIndex);
        abstract public void CleanEpisodes();
        abstract public string GetSubtitles();
        abstract public List<object> GetShowsList();
        abstract public List<object> GetEpisodesList();
        abstract public string GetDescriptionShow();
        abstract public string GetDescriptionEpisode(int selectedIndex);
    }
}
