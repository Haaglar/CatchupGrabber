﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatchupGrabber
{
    class _3Now : DownloadAbstract
    {
        private static string SHOWS_URL = "http://now-api.mediaworks.nz/now-api/v2/shows";
        private static string EPISODEs_URL = "http://now-api.mediaworks.nz/now-api/v2/show/";
        public override void CleanEpisodes()
        {
            throw new NotImplementedException();
        }

        public override void ClickDisplayedShow(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override void FillShowsList()
        {
            throw new NotImplementedException();
        }

        public override string GetDescriptionEpisode(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override string GetDescriptionShow(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override DownloadObject GetDownloadObject(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override List<object> GetEpisodesList()
        {
            throw new NotImplementedException();
        }

        public override string GetSelectedEpisodeName(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override string GetSelectedShowName(int selectedIndex)
        {
            throw new NotImplementedException();
        }

        public override List<object> GetShowsList()
        {
            throw new NotImplementedException();
        }

        public override string GetSubtitles()
        {
            throw new NotImplementedException();
        }
    }
}
