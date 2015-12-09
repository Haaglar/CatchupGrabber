using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Reflection;
using System.Web.Script.Serialization;
namespace cu_grab
{
    /* Note: Requires FFmpeg
     * TODO: 
     * Error handling!
     * Make a attractive GUI
     * Use API, for descriptions and stuff instead of crawling
     * Other sites
     *      rtvs
     *          search Crawl api
     *          do stuff
     *      p7 maybe?   
     */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        String lcJsonUrl = @"http://www.rtve.es/infantil/buscador-clan/obtener-datos-programas.json";
        String p7JsonUrl = @"https://au.tv.yahoo.com/plus7/data/tv-shows/";

           
        String tpURL = "http://tenplay.com.au";
        String selectedShow = "";
        enum State {DisplayingNone, DisplayingShows, DisplayingEpisodes};
        enum Site {None, TenP, RTVEC }
        State curState = State.DisplayingNone;


        public MainWindow()
        {
            InitializeComponent();
            Tenp.fillShowsList(objectList);
            curState = State.DisplayingShows;
        }
    
        /// <summary>
        /// Double click of list item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void oL_ItemPressed(object sender, MouseButtonEventArgs e)
        {
            switch (curState)
            {
                case State.DisplayingShows:
                    selectedShow = Tenp.clickDisplayedShow(objectList);
                    curState = State.DisplayingEpisodes;
                    break;
                case State.DisplayingEpisodes:
                    String name = Tenp.getSelectedName(objectList);
                    String dlUrl = Tenp.getUrl(objectList);
                    runFFmpeg(dlUrl, selectedShow + " " + name);
                    break;
            }
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">The rendition URL to download</param>
        /// <param name="nameLocation">The file name and location (without file extension)</param>
        /// <returns>Returns FFmpegs error code</returns>
        public int runFFmpeg(string url, string nameLocation)
        {
            //ffmpeg
            ProcessStartInfo ffmpeg = new ProcessStartInfo();
            ffmpeg.Arguments = " -i " + url + " -acodec copy -vcodec copy -bsf:a aac_adtstoasc " + '"' + nameLocation + '"' + ".mp4";
            ffmpeg.FileName = "ffmpeg.exe";
            int exitCode;
            using (Process proc = Process.Start(ffmpeg))
            {
                proc.WaitForExit();
                exitCode = proc.ExitCode;
            }
            return exitCode;
        }

        
        /// <summary>
        /// Cleanup function for returning to shows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Shows_Pressed(object sender, RoutedEventArgs e)
        {
            Tenp.cleanEpisodes(objectList);
            curState = State.DisplayingShows;  
            selectedShow = "";
        }
    }
}
