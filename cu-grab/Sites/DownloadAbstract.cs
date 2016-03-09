using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace cu_grab
{
    abstract public class DownloadAbstract
    {
        protected ListBox listBoxContent;
        protected DownloadAbstract(ListBox lBoxContent)
        {
            listBoxContent = lBoxContent;
        }
        abstract public String ClickDisplayedShow(int selectedIndex);
        abstract public void FillShowsList();
        abstract public String GetSelectedNameShow(int selectedIndex);
        abstract public DownloadObject GetDownloadObject();
        abstract public void CleanEpisodes();
        abstract public void SetActive();
        abstract public String GetSubtitles();
        abstract public List<object> GetShowsList();
    }
}
