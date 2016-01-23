using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace cu_grab
{
    public class DPlay : DownloadAbstract
    {
        public DPlay(ListBox lBoxContent) : base(lBoxContent) { }

        //http://it.dplay.com/api/v2/ajax/shows/ #### /seasons/?show_id= ####
        //http://it.dplay.com/api/v2/ajax/modules?items=400&page_id=32&module_id=26&page=0
        public override void FillShowsList()
        {
            throw new NotImplementedException();
        }
        public override void SetActive()
        {
            throw new NotImplementedException();
        }
        public override string ClickDisplayedShow()
        {
            throw new NotImplementedException();
        }
        public override string GetUrl()
        {
            throw new NotImplementedException();
        }
        public override void CleanEpisodes()
        {
            throw new NotImplementedException();
        }
        public override string GetSelectedName()
        {
            throw new NotImplementedException();
        }
        public override string GetSubtitles()
        {
            throw new NotImplementedException();
        }
    }
}
