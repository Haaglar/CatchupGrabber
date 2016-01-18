using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
{
    abstract public class DownloadAbstract
    {
        abstract public String ClickDisplayedShow();
        abstract public void FillShowsList();
        abstract public String GetSelectedName();
        abstract public String GetUrl();
        abstract public void CleanEpisodes();
        abstract public void SetActive();
        abstract public String GetSubtitles();
    }
}
