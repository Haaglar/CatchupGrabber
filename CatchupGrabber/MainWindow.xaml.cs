using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CatchupGrabber.MiscObjects.EnumValues;

namespace CatchupGrabber
{
    /* Note: Requires FFmpeg
     * TODO: 
     * More sites :)
     * Make a attractive GUI
     * Use API, for descriptions and stuff instead of crawling on tenplay, p7 (OAuth) and possibly RTE(OAuth)
     * Implement an alternative hls downloader inorder to use a proxy when downloading through that method
     */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StringBindings sBinds = new StringBindings();
        State curState = State.DisplayingNone;
        Site curSite = Site.None;
        Dictionary<Site, DownloadAbstract> websiteStore = new Dictionary<Site, DownloadAbstract>();
        Dictionary<Site, string> addressStore = new Dictionary<Site, string>();
        public MainWindow()
        {
            InitializeComponent();
            if(!File.Exists("FFmpeg.exe"))
            {
                MessageBox.Show("Catch-up Grabber requires FFmpeg to download from certain sites, please copy it into the working directory.", "Warning");
            }
            DataContext = sBinds;
            SetupSites();
            SetupAddress();
        }
        /// <summary>
        /// Setup the dictionary for all the sites
        /// </summary>
        private void SetupSites()
        {
            Tenp tenPlay;
            RTVEc rtveClan;
            Plus7 plus7;
            RTE rteIE;
            DPlay dplay;
            TV3Cat tv3CatCCMA;
            Super3 super3;
            SVTse svtplay;
            _9Now nNow;
            Prima prima;
            websiteStore.Add(Site.TenP, tenPlay = new Tenp());
            websiteStore.Add(Site.Plus7, plus7 = new Plus7());
            websiteStore.Add(Site.RTVEClan, rtveClan = new RTVEc());
            websiteStore.Add(Site.RTE, rteIE = new RTE());
            websiteStore.Add(Site.DPlay, dplay = new DPlay());
            websiteStore.Add(Site.TV3Cat, tv3CatCCMA = new TV3Cat());
            websiteStore.Add(Site.Super3, super3 = new Super3());
            websiteStore.Add(Site.SVTPlay, svtplay = new SVTse());
            websiteStore.Add(Site._9Now, nNow = new _9Now());
            websiteStore.Add(Site.Prima, prima = new Prima());
        }
        /// <summary>
        /// Setup the dictionary for all the site's addresses
        /// </summary>
        private void SetupAddress()
        {
            addressStore.Add(Site.None, "");
            addressStore.Add(Site.TenP, "tenplay.com.au");
            addressStore.Add(Site.Plus7, "au.tv.yahoo.com/plus7");
            addressStore.Add(Site.RTVEClan, "rtve.es/infantil/");
            addressStore.Add(Site.RTE, "RTE.ie");
            addressStore.Add(Site.DPlay, "it.dplay.com");
            addressStore.Add(Site.TV3Cat, "ccma.cat/tv3/");
            addressStore.Add(Site.Super3, "super3.cat");
            addressStore.Add(Site.SVTPlay, "SVTPlay.se");
            addressStore.Add(Site._9Now, "9now.com.au");
            addressStore.Add(Site.Prima, "play.iprima.cz");
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
                int selected = objectList.SelectedIndex;
                sBinds.Error = ""; //Clear the error
                switch (curState)
                {
                    // Get episodes for the selected show
                    case State.DisplayingShows:
                        DisableButtonsSites();
                        sBinds.SelectedShow = websiteStore[curSite].GetSelectedShowName(selected);
                        objectList.ItemsSource = new List<string> { "Loading..." + sBinds.SelectedShow};
                        Task.Factory.StartNew(() =>
                        {
                            websiteStore[curSite].ClickDisplayedShow(selected);
                        }).ContinueWith(x=>
                        {
                            if (x.Exception == null)
                            {
                                objectList.ItemsSource = websiteStore[curSite].GetEpisodesList();
                                curState = State.DisplayingEpisodes;
                                ResetView();
                            }
                            else
                            {
                                Console.WriteLine(x.Exception.ToString());
                                sBinds.Error = "Failed to get episode list for selected show";
                                Shows_Pressed(null, null);
                            }
                            EnableButtonsSites();
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                        break;
                    //Download Selected show
                    case State.DisplayingEpisodes:
                        try
                        {
                            string name = sBinds.SelectedShow + " " + websiteStore[curSite].GetSelectedEpisodeName(objectList.SelectedIndex);
                            DownloadObject dlUrl = websiteStore[curSite].GetDownloadObject(objectList.SelectedIndex);
                            DownloadWindow dlWindow = new DownloadWindow(dlUrl, name);
                        }
                        catch(Exception eDl)
                        {
                            Console.WriteLine(eDl.ToString());
                            sBinds.Error = "Failed to download episode";
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
            if(curSite == Site.None)
            {
                return;
            }
            bool check = websiteStore[curSite].ShowListCacheValid;
            if (check)
            {
                websiteStore[curSite].CleanEpisodes();
                objectList.ItemsSource = websiteStore[curSite].GetShowsList();
                curState = State.DisplayingShows;
                sBinds.SelectedShow = "";
            }
            else
            {
                MakeRequest(curSite);
            }
            ResetView();
        }
        /// <summary>
        /// Handles the action for when the Tenplay button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonTenplay_Click(object sender, RoutedEventArgs e)
        {
            HandleSiteSelection(Site.TenP);
        }

        /// <summary>
        /// Handles the action for when the Clan RTVE button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRTVEC_Click(object sender, RoutedEventArgs e)
        {
            HandleSiteSelection(Site.RTVEClan);
        }
        /// <summary>
        /// Handles the actions for when the Plus7 button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPlus7_Click(object sender, RoutedEventArgs e)
        {
            HandleSiteSelection(Site.Plus7);
        }

        /// <summary>
        /// Handles the actions for when the RTE button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRTE_Click(object sender, RoutedEventArgs e)
        {
            HandleSiteSelection(Site.RTE);
        }
        /// <summary>
        /// Handles the actions for when the DPlay (ITA) button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDPlay_Click(object sender, RoutedEventArgs e)
        {
            HandleSiteSelection(Site.DPlay);
        }
        /// <summary>
        /// Handles the actions for when the TV3 (CAT) button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCCMA_Click(object sender, RoutedEventArgs e)
        {
             HandleSiteSelection(Site.TV3Cat);
        }
        /// <summary>
        /// Handles the actions for when the Super3 button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSuper3_Click(object sender, RoutedEventArgs e)
        {
             HandleSiteSelection(Site.Super3);
        }
        /// <summary>
        /// Handles the actions for when the SVTPlay button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSVTPlay_Click(object sender, RoutedEventArgs e)
        {
             HandleSiteSelection(Site.SVTPlay);
        }

        /// <summary>
        /// Handles the actions for when the 9Now button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button9Now_Click(object sender, RoutedEventArgs e)
        {
            HandleSiteSelection(Site._9Now);
        }
        /// <summary>
        /// Handles the actions for when the Prima button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPrima_Click(object sender, RoutedEventArgs e)
        {
            HandleSiteSelection(Site.Prima);
        }

        /// <summary>
        /// Handles the actions for site show data and switching between. 
        /// </summary>
        /// <param name="site">The selected site identifier</param>
        /// <param name="url">The url for site display</param>
        private void HandleSiteSelection(Site site) 
        {
            sBinds.SelectedShow = "";
            sBinds.SelectedSite = addressStore[site];
            sBinds.SelectedDescription = "";
            sBinds.Error = "";
            //First time selecting site
            if (!websiteStore[site].ShowListCacheValid)
            {
                MakeRequest(site);
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
            ResetView();
        }
        /// <summary>
        /// Handles the action for when the Setting button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            settingsWindow.ShowDialog();
            //Clear cache if changed
            foreach(Site s in settingsWindow.cacheClear)
            {
                websiteStore[s].InvalidateShowListCache();
            }
        }

        /// <summary>
        /// Handles the about buttons being pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void About_Pressed(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Catchup Grabber v" + typeof(MainWindow).Assembly.GetName().Version +  " by Haaglar 2015-2016\r\nDownload video from certain TV-Catchup services for offline viewing\r\nProject page: https://github.com/Haaglar/CatchupGrabber \r\nReport issues and request features here https://github.com/Haaglar/CatchupGrabber/issues", "About");
        }

        /// <summary>
        /// Disables conflicting visual options while the application in requesting data
        /// </summary>
        private void DisableButtonsSites()
        {
            ButtonSegment.IsEnabled = false;
            objectList.IsEnabled = false;
            toShows.IsEnabled = false;
        }
        /// <summary>
        /// Reenables the visual options
        /// </summary>
        private void EnableButtonsSites()
        {
            ButtonSegment.IsEnabled = true;
            objectList.IsEnabled = true;
            toShows.IsEnabled = true;
        }

        /// <summary>
        /// Makes the request to the specifed site
        /// </summary>
        /// <param name="site"></param>
        private void MakeRequest(Site site)
        {
            objectList.ItemsSource = new List<string> { "Loading..." + addressStore[site] };
            DisableButtonsSites();
            //Fetch and handle the result on a background thread/task
            Task.Factory.StartNew(() =>
            {
                websiteStore[site].FillShowsList();
            }).ContinueWith(x =>
            {
                if (x.IsFaulted || x.IsCanceled)
                {
                    sBinds.Error = "Failed to get episode listings for " + addressStore[site];
                    curState = State.DisplayingNone;
                    objectList.ItemsSource = null;
                    curSite = Site.None;
                    sBinds.SelectedSite = "";
                }
                else
                {
                    objectList.ItemsSource = websiteStore[site].GetShowsList();
                    curState = State.DisplayingShows;
                    curSite = site;
                }
                EnableButtonsSites();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Resets the main listbox to the top of the screen
        /// </summary>
        private void ResetView()
        {
            if (objectList.Items != null && objectList.Items.Count > 0)
            {
                objectList.ScrollIntoView(objectList.Items[0]);
            }
            objectList.SelectedIndex = -1;
        }

        /// <summary>
        /// Handles the selection of an item in the shows/episodes list(updates the description)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(objectList.SelectedIndex != -1)
            {
                if (curState == State.DisplayingShows)
                {
                    string tmp = websiteStore[curSite].GetDescriptionShow(objectList.SelectedIndex);
                    if (tmp != null)
                    {
                        sBinds.SelectedDescription = tmp;
                    }
                }
                else if(curState == State.DisplayingEpisodes)
                {
                    string tmp = websiteStore[curSite].GetDescriptionEpisode(objectList.SelectedIndex);
                    if (tmp != null)
                    {
                        sBinds.SelectedDescription = tmp;
                    }
                }
            }
        }
    }
}
