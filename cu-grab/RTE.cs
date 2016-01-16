using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace cu_grab
{
    public class RTE : DownloadAbstract
    {
        private ListBox objectList;
        public RTE(ListBox oList)
        {
            objectList = oList;
        }
        public override void fillShowsList()
        {
            
        }
        public override void setActive()
        {
            throw new NotImplementedException();
        }
        public override string clickDisplayedShow()
        {
            throw new NotImplementedException();
        }
        public override string getUrl()
        {
            throw new NotImplementedException();
        }
        public override void cleanEpisodes()
        {
            throw new NotImplementedException();
        }
        public override string getSelectedName()
        {
            throw new NotImplementedException();
        }
        public override string getSubtitles()
        {
            throw new NotImplementedException();
        }
    }
}
