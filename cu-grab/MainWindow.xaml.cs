using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        Dictionary<Site, DownloadAbstract> websiteStore = new Dictionary<Site, DownloadAbstract>();

        public MainWindow()
        {
            InitializeComponent();
            if(!File.Exists("FFmpeg.exe"))
            {
                MessageBox.Show("Catch-up Grabber requires FFmpeg to download from certain sites, please copy it into the working directory.");
            }
            textBlockShow.DataContext = sBinds;
            textBlockSite.DataContext = sBinds;
            SetupSites();
        }
        /// <summary>
        /// Setup the dictionary for all the sites
        /// </summary>
        public void SetupSites()
        {
            Tenp tenPlay;
            RTVEc rtveClan;
            Plus7 plus7;
            RTE rteIE;
            DPlay dplay;
            TV3Cat tv3CatCCMA;
            Super3 super3;
            SVTse svtplay;
            websiteStore.Add(Site.TenP, tenPlay = new Tenp());
            websiteStore.Add(Site.Plus7, plus7 = new Plus7());
            websiteStore.Add(Site.RTVEClan, rtveClan = new RTVEc());
            websiteStore.Add(Site.RTE, rteIE = new RTE());
            websiteStore.Add(Site.DPlay, dplay = new DPlay());
            websiteStore.Add(Site.TV3Cat, tv3CatCCMA = new TV3Cat());
            websiteStore.Add(Site.Super3, super3 = new Super3());
            websiteStore.Add(Site.SVTPlay, svtplay = new SVTse());
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
                            sBinds.SelectedShow = websiteStore[curSite].ClickDisplayedShow(objectList.SelectedIndex);
                            objectList.ItemsSource = websiteStore[curSite].GetEpisodesList();
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
                            String name = sBinds.SelectedShow + " " + websiteStore[curSite].GetSelectedNameEpisode(objectList.SelectedIndex);
                            DownloadObject dlUrl = websiteStore[curSite].GetDownloadObject(objectList.SelectedIndex);
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
            if (websiteStore[curSite] != null)
            {
                websiteStore[curSite].CleanEpisodes();
                objectList.ItemsSource = websiteStore[curSite].GetShowsList();
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
            HandleSiteSelection(Site.TenP, "tenplay.com.au");
        }

        /// <summary>
        /// Handles the action for when the RTVEClan button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRTVEC_Click(object sender, RoutedEventArgs e)
        {
            HandleSiteSelection(Site.RTVEClan, "rtve.es/infantil/");
        }
        /// <summary>
        /// Handles the actions for when the Plus7 button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPlus7_Click(object sender, RoutedEventArgs e)
        {
            HandleSiteSelection(Site.Plus7, "au.tv.yahoo.com/plus7");
        }

        /// <summary>
        /// Handles the actions for when the RTE button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRTE_Click(object sender, RoutedEventArgs e)
        {
            HandleSiteSelection(Site.RTE, "RTE.ie");
        }
        /// <summary>
        /// Handles the actions for when the DPlay (ITA) button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDPlay_Click(object sender, RoutedEventArgs e)
        {
            HandleSiteSelection(Site.DPlay, "it.dplay.com");
        }
        /// <summary>
        /// Handles the actions for when the TV3 (CAT) button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCCMA_Click(object sender, RoutedEventArgs e)
        {
             HandleSiteSelection(Site.TV3Cat, "ccma.cat/tv3/");
        }
        /// <summary>
        /// Handles the actions for when the Super3 button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSuper3_Click(object sender, RoutedEventArgs e)
        {
             HandleSiteSelection(Site.Super3, "super3.cat");
        }
        /// <summary>
        /// Handles the actions for when the SVTPlay button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSVTPlay_Click(object sender, RoutedEventArgs e)
        {
             HandleSiteSelection(Site.SVTPlay, "SVTPlay.se");
        }
        /// <summary>
        /// Handles the actions for site show data and switching between. 
        /// </summary>
        /// <param name="site">The selected site identifier</param>
        /// <param name="url">The url for site display</param>
        private void HandleSiteSelection(Site site, string url) 
        {
            sBinds.SelectedSite = url;
            //First time selecting site
            if (!websiteStore[site].RequestedSiteData)
            {
                objectList.ItemsSource = new List<String>{"Loading..." + url};
                DisableButtonsSites();
                try
                {
                    Task.Factory.StartNew(()=>
                    {
                        websiteStore[site].FillShowsList();
                    }).ContinueWith(x=>
                    {
                        objectList.ItemsSource = websiteStore[site].GetShowsList();
                        EnableButtonsSites();
                        curState = State.DisplayingShows;
                        curSite = site;
                        sBinds.SelectedShow = "";
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    
                }
                catch
                {
                    errorLabel.Text = "Failed to get episode listings for "+url;
                    EnableButtonsSites();
                }
            }
            // If they select it while we are currently on it just return to shows
            else if (curSite == site)
            {
                Shows_Pressed(null, null);
                return;
            }
            // other time selecting site
            else
            {
                websiteStore[site].CleanEpisodes();
                objectList.ItemsSource = websiteStore[site].GetShowsList();

                curState = State.DisplayingShows;
                curSite = site;
                sBinds.SelectedShow = "";
            }
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

        private void DisableButtonsSites()
        {
            ButtonSegment.IsEnabled = false;
            toShows.IsEnabled = false;
        }
        private void EnableButtonsSites()
        {
            ButtonSegment.IsEnabled = true;
            toShows.IsEnabled = true;
        }
    }
}
