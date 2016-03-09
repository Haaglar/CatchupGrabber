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
using System.ComponentModel;
using SubCSharp;
using cu_grab.NetworkAssister;
namespace cu_grab
{
    /* Note: Requires FFmpeg
     * TODO: 
     * More sites :)
     * More proper error handling
     * Make a attractive GUI
     * Use API, for descriptions and stuff instead of crawling on tenplay, p7 (OAuth) and possibly RTE(OAuth)
     */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StringBindings sBinds = new StringBindings();
        enum State {DisplayingNone, DisplayingShows, DisplayingEpisodes};
        enum Site {None, TenP, Plus7, RTVEClan, RTE, DPlay, TV3Cat, Super3, SVTPlay}
        State curState = State.DisplayingNone;
        Site curSite = Site.None;

        //--Downloaderbstract Objects
        DownloadAbstract dlAbs; //What will be currently selected
        Tenp tenPlay;
        RTVEc rtveClan;
        Plus7 plus7;
        RTE rteIE;
        DPlay dplay;
        TV3Cat tv3CatCCMA;
        Super3 super3;
        SVTse svtplay;

        public MainWindow()
        {
            InitializeComponent();
            if(!File.Exists("FFmpeg.exe"))
            {
                MessageBox.Show("Catch-up Grabber requires FFmpeg to download from certain sites, please copy it into the working directory.");
            }
            textBlockShow.DataContext = sBinds;
            textBlockSite.DataContext = sBinds;
        }
    
        /// <summary>
        /// Double click of list item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OL_ItemPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock || e.OriginalSource is Border) //Make sure that we double click an item not the scrollbar
            {
                switch (curState)
                {
                    // Get episodes for the selected show
                    case State.DisplayingShows:
                        try
                        {
                            sBinds.SelectedShow = dlAbs.ClickDisplayedShow(objectList.SelectedIndex);
                            objectList.ItemsSource = dlAbs.GetEpisodesList();
                            curState = State.DisplayingEpisodes;
                        }
                        catch(Exception eDl)
                        {
                            Console.WriteLine(eDl.ToString());
                            errorLabel.Text = "Failed to get episode list for selected show";
                        }
                        break;
                    //Download Selected show
                    case State.DisplayingEpisodes:
                        try
                        {
                            String name = sBinds.SelectedShow + " " + dlAbs.GetSelectedNameShow(objectList.SelectedIndex);
                            DownloadObject dlUrl = dlAbs.GetDownloadObject(objectList.SelectedIndex);
                            DownloadWindow dlWindow = new DownloadWindow(dlUrl, name);
                        }
                        catch(Exception eDl)
                        {
                            Console.WriteLine(eDl.ToString());
                            errorLabel.Text = "Failed to download episode";
                        }
                        break;
                }
            }
        }
        //-----------------BUTTONS START---------------//
        /// <summary>
        /// Cleanup function for returning to shows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Shows_Pressed(object sender, RoutedEventArgs e)
        {
            if (dlAbs != null)
            {
                dlAbs.CleanEpisodes();
                objectList.ItemsSource = dlAbs.GetShowsList();
                curState = State.DisplayingShows;
                sBinds.SelectedShow = "";
            }
        }
        /// <summary>
        /// Handles the action for when the Tenplay button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonTenplay_Click(object sender, RoutedEventArgs e)
        {
            sBinds.SelectedSite = "tenplay.com.au";
            //First time selecting site
            if (tenPlay == null)
            {
                try
                {
                    tenPlay = new Tenp(objectList);
                    tenPlay.FillShowsList();
                    objectList.ItemsSource = tenPlay.GetShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get Episode listings for Tenplay.";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.TenP)
            {
                Shows_Pressed(null,null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                tenPlay.SetActive();
            }  
            curState = State.DisplayingShows;
            curSite = Site.TenP;
            sBinds.SelectedShow = "";
            dlAbs = tenPlay;
        }

        /// <summary>
        /// Handles the action for when the RTVEClan button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRTVEC_Click(object sender, RoutedEventArgs e)
        {
            sBinds.SelectedSite = "rtve.es/infantil/";
            //First time selecting site
            if (rtveClan == null)
            {
                try
                {
                    rtveClan = new RTVEc(objectList);
                    rtveClan.FillShowsList();
                    objectList.ItemsSource = rtveClan.GetShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get Episode listings for RTVE Clan";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.RTVEClan)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                rtveClan.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.RTVEClan;
            sBinds.SelectedShow = "";
            dlAbs = rtveClan;
        }
        /// <summary>
        /// Handles the actions for when the Plus7 button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPlus7_Click(object sender, RoutedEventArgs e)
        {
            sBinds.SelectedSite = "au.tv.yahoo.com/plus7";
            //First time selecting site
            if (plus7 == null)
            {
                try
                {
                    plus7 = new Plus7(objectList);
                    plus7.FillShowsList();
                    objectList.ItemsSource = plus7.GetShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get episode listings for Plus7";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.Plus7)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                plus7.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.Plus7;
            sBinds.SelectedShow = "";
            dlAbs = plus7;
        }

        /// <summary>
        /// Handles the actions for when the RTE button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRTE_Click(object sender, RoutedEventArgs e)
        {
            sBinds.SelectedSite = "RTE.ie";
            //First time selecting site
            if (rteIE == null)
            {
                try
                {
                    rteIE = new RTE(objectList);
                    rteIE.FillShowsList();
                    objectList.ItemsSource = rteIE.GetShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get episode listings for RTE";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.RTE)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                rteIE.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.RTE;
            sBinds.SelectedShow = "";
            dlAbs = rteIE;
        }

        private void ButtonDPlay_Click(object sender, RoutedEventArgs e)
        {
            sBinds.SelectedSite = "it.dplay.com";
            //First time selecting site
            if (dplay == null)
            {
                try
                {
                    dplay = new DPlay(objectList);
                    dplay.FillShowsList();
                    objectList.ItemsSource = dplay.GetShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get episode listings for DPlay";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.DPlay)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                dplay.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.DPlay;
            sBinds.SelectedShow = "";
            dlAbs = dplay;
        }

        private void ButtonCCMA_Click(object sender, RoutedEventArgs e)
        {
            sBinds.SelectedSite = "ccma.cat/tv3/";
            //First time selecting site
            if (tv3CatCCMA == null)
            {
                try
                {
                    tv3CatCCMA = new TV3Cat(objectList);
                    tv3CatCCMA.FillShowsList();
                    objectList.ItemsSource = tv3CatCCMA.GetShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get episode listings for CCMA (TV3)";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.TV3Cat)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                tv3CatCCMA.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.TV3Cat;
            sBinds.SelectedShow = "";
            dlAbs = tv3CatCCMA;
        }

        private void ButtonSuper3_Click(object sender, RoutedEventArgs e)
        {
            sBinds.SelectedSite = "super3.cat";
            //First time selecting site
            if (super3 == null)
            {
                try
                {
                    super3 = new Super3(objectList);
                    super3.FillShowsList();
                    objectList.ItemsSource = super3.GetShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get episode listings for Super3";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.Super3)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                super3.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.Super3;
            sBinds.SelectedShow = "";
            dlAbs = super3;
        }

        private void ButtonSVTPlay_Click(object sender, RoutedEventArgs e)
        {
            sBinds.SelectedSite = "SVTPlay.se";
            //First time selecting site
            if (svtplay == null)
            {
                try
                {
                    svtplay = new SVTse(objectList);
                    svtplay.FillShowsList();
                    objectList.ItemsSource = svtplay.GetShowsList();
                }
                catch
                {
                    errorLabel.Text = "Failed to get episode listings for SVTPlay";
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == Site.SVTPlay)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                dlAbs.CleanEpisodes();
                svtplay.SetActive();
            }
            curState = State.DisplayingShows;
            curSite = Site.SVTPlay;
            sBinds.SelectedShow = "";
            dlAbs = svtplay;
        }

        /// <summary>
        /// Handles the action for when the Setting button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            settingsWindow.Show();
        }
    }
}
