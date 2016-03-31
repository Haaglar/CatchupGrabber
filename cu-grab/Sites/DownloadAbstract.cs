﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace cu_grab
{
    abstract public class DownloadAbstract
    {
        public bool RequestedSiteData = false; //true after FillShowsList() is called
        abstract public String ClickDisplayedShow(int selectedIndex);
        abstract public void FillShowsList();
        abstract public String GetSelectedNameEpisode(int selectedIndex);
        abstract public DownloadObject GetDownloadObject(int selectedIndex);
        abstract public void CleanEpisodes();
        abstract public String GetSubtitles();
        abstract public List<object> GetShowsList();
        abstract public List<object> GetEpisodesList();
    }
}
