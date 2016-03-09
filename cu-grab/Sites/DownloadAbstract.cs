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
        abstract public String ClickDisplayedShow(int selectedIndex);
        abstract public void FillShowsList();
        abstract public String GetSelectedNameShow(int selectedIndex);
        abstract public DownloadObject GetDownloadObject(int selectedIndex);
        abstract public void CleanEpisodes();
        abstract public String GetSubtitles();
        abstract public List<object> GetShowsList();
        abstract public List<object> GetEpisodesList();
    }
}