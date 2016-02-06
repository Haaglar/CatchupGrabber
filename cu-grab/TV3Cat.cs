using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace cu_grab
{
    class TV3Cat : DownloadAbstract
    {
        public TV3Cat (ListBox lBoxContent) : base(lBoxContent) { }
        public override void FillShowsList()
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
        public override string GetSelectedName()
        {
            throw new NotImplementedException();
        }
        public override void CleanEpisodes()
        {
            throw new NotImplementedException();
        }
        public override void SetActive()
        {
            throw new NotImplementedException();
        }
        public override string GetSubtitles()
        {
            throw new NotImplementedException();
        }
    }
}
