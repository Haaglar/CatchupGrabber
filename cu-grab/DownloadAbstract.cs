using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
{
    abstract public class DownloadAbstract
    {
        abstract public String clickDisplayedShow();
        abstract public void fillShowsList();
        abstract public String getSelectedName();
        abstract public String getUrl();
        abstract public void cleanEpisodes();
        abstract public void setActive();
    }
}
