using CatchupGrabber.MiscObjects.EnumValues;
using CatchupGrabber.MiscObjects.LiveStreamSites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace CatchupGrabber
{
    /// <summary>
    /// Interaction logic for LiveStream.xaml
    /// </summary>
    public partial class LiveStream : Window
    {
        List<LSSites> sites;
        public LiveStream()
        {
            InitializeComponent();
            sites = new List<LSSites>();
            sites.Add(new LSSites() { Name = "Channel 9 Australia", URL = "https://9nowch9livehls-i.akamaihd.net/hls/live/250962/melbourne/master1.m3u8", RequiresInitialRequest = false, Method = DownloadMethod.HLS });
            sites.Add(new LSSites() { Name = "Channel 9 Gem Australia", URL = "https://9nowgemlivehls-i.akamaihd.net/hls/live/250972/melbourne/master1.m3u8", RequiresInitialRequest = false, Method = DownloadMethod.HLS });
            sites.Add(new LSSites() { Name = "Channel 9 GO Australia", URL = "https://9nowgolivehls-i.akamaihd.net/hls/live/250977/melbourne/master1.m3u8", RequiresInitialRequest = false, Method = DownloadMethod.HLS });
            sites.Add(new LSSites() { Name = "Channel 9 Life Australia", URL = "https://9nowlifelivehls-i.akamaihd.net/hls/live/250992/melbourne/master1.m3u8", RequiresInitialRequest = false, Method = DownloadMethod.HLS });
            sites.Add(new LSSites() { Name = "Channel 7 Australia", URL = "https://sevenwestmedia01-i.akamaihd.net/hls/live/224813/MEL1/master_high.m3u8", RequiresInitialRequest = false, Method = DownloadMethod.HLS });
            sites.Add(new LSSites() { Name = "Channel 7 Two Australia", URL = "https://sevenwestmedia01-i.akamaihd.net/hls/live/224826/MEL2/master_high.m3u8", RequiresInitialRequest = false, Method = DownloadMethod.HLS });
            sites.Add(new LSSites() { Name = "Channel 7 Mate Australia", URL = "https://sevenwestmedia01-i.akamaihd.net/hls/live/224839/MEL3/master_high.m3u8", RequiresInitialRequest = false, Method = DownloadMethod.HLS });
            sites.Add(new LSSites() { Name = "Channel 7 Flix Australia", URL = "https://sevenwestmedia01-i.akamaihd.net/hls/live/224856/MEL6/master_high.m3u8", RequiresInitialRequest = false, Method = DownloadMethod.HLS });
            sites.Add(new LSSites() { Name = "TV3 Catalunya", URL = "http://ccma-tva-int-abertis-live.hls.adaptive.level3.net/int/ngrp:tv3_web/chunklist_b798000.m3u8", RequiresInitialRequest = false, Method = DownloadMethod.HLS });
            sites.Add(new LSSites() { Name = "3/24 Catalunya", URL = "http://ccma-tva-int-abertis-live.hls.adaptive.level3.net/int/ngrp:324_web/chunklist_b798000.m3u8", RequiresInitialRequest = false, Method = DownloadMethod.HLS });
            sites.Add(new LSSites() { Name = "Rai Gulp", URL = "http://b2everyrai-lh.akamaihd.net/i/raigulp_1@66350/index_1800_av-b.m3u8?sd=10&rebase=on", RequiresInitialRequest = false, Method = DownloadMethod.HLS });
            comboBoxSites.ItemsSource = sites;
            comboBoxSites.SelectedIndex = 0;
        }

        private void buttonLogStream_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("FFmpeg.exe"))
            {
                string filename = ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds) + ".mp4";

                ProcessStartInfo ffmpeg = new ProcessStartInfo();
                using (Process proc = new Process())
                {
                    ffmpeg.FileName = "ffmpeg.exe";
                    if (Properties.Settings.Default.OverwriteFile)
                    {
                        ffmpeg.Arguments = " -i " + sites[comboBoxSites.SelectedIndex].URL + " -acodec copy -vcodec copy -bsf:a aac_adtstoasc " + filename;
                    }
                    proc.StartInfo = ffmpeg;
                    proc.Start();
                }
            }
        }
    }
}
