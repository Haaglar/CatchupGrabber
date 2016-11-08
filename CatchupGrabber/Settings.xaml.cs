using CatchupGrabber.MiscObjects.EnumValues;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace CatchupGrabber
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private string oldValRegionIrish; //Need to check if change if so clear cache
        public List<Site> cacheClear;     //Return value

        public Settings()
        {
            InitializeComponent();
            cacheClear = new List<Site>();
            //Spanish
            TextBoxSpanishHTTPProxy.Text = Properties.Settings.Default.HTTPSpanish;
            TextBoxSpanishGlypeProxy.Text = Properties.Settings.Default.GlypeSpanish;
            switch (Properties.Settings.Default.ProxyOptionSpanish)
            {
                case ("None"):
                    RadioSpanishUseNone.IsChecked = true;
                    break;
                case ("Glype"):
                    RadioSpanishUseGlype.IsChecked = true;
                    break;
                case ("HTTP"):
                    RadioSpanishUseHTTP.IsChecked = true;
                    break;
            }

            //Irish
            TextBoxIrishHTTPProxy.Text = Properties.Settings.Default.HTTPIrish;
            oldValRegionIrish = Properties.Settings.Default.IrishRegionOption;
            if (!oldValRegionIrish.Equals("ie")) //If not ie then its world wide
            {
                RadioButtonIrishRegionWorldWide.IsChecked = true;
            }
            else
            {
                RadioButtonIrishRegionIrish.IsChecked = true;
            }
            TextBoxSwedishHTTPProxy.Text = Properties.Settings.Default.HTTPSwedish;

            //USA
            TextBoxUSAHTTPProxy.Text = Properties.Settings.Default.HTTPUSA;

            //Misc
            CheckBoxDownloadWindowClose.IsChecked = Properties.Settings.Default.ExitDLOnDownload;
            CheckBoxSpaceReplace.IsChecked = Properties.Settings.Default.SpaceReplace;
            CheckBoxOverwrite.IsChecked = Properties.Settings.Default.OverwriteFile;
            CheckBoxLoadImages.IsChecked = Properties.Settings.Default.LoadImages;
            //Sub
            CheckBoxSubtitleDownloadSetting.IsChecked = Properties.Settings.Default.DownloadSubtitlesSetting;
            if (!(CheckBoxSubtitleConvertSetting.IsChecked = Properties.Settings.Default.ConvertSubtitle) ?? false)
            {
                RadioButtonSRT.IsEnabled = false;
                RadioButtonASS.IsEnabled = false;
            }

            if(Properties.Settings.Default.SubtitleFormat.Equals(".srt"))
            {
                RadioButtonSRT.IsChecked = true;
            }
            else
            {
                RadioButtonASS.IsChecked = true;
            }

            //Setup Key handler
            this.PreviewKeyDown += Exit_PreviewKeyDown;
        }

        /// <summary>
        /// Handles the save button being pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            //Sites options
            SpanishSettingsSave();
            IrishSettingsSave();
            Properties.Settings.Default.HTTPSwedish =  TextBoxSwedishHTTPProxy.Text;
            Properties.Settings.Default.HTTPUSA = TextBoxUSAHTTPProxy.Text;

            SubtitleSettingsSave();
            MiscSettingsSave();
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void ButtonCancelSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Key handler that closes the settings window on key pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
            if (e.Key == Key.S && (e.KeyboardDevice.Modifiers == ModifierKeys.Control))
            {
                ButtonSaveSettings_Click(sender, null);
            }
        }

        /// <summary>
        /// Handles saving settings for the spanish websites
        /// </summary>
        private void SpanishSettingsSave()
        {
            //Get Spansih Glype proxy
            string proxyUnfiltered = TextBoxSpanishGlypeProxy.Text;
            if (proxyUnfiltered != "")
            {
                if (!proxyUnfiltered.StartsWith("http://"))
                    proxyUnfiltered = "http://" + proxyUnfiltered;//Add nessesary http
                if (proxyUnfiltered.EndsWith("/"))
                    proxyUnfiltered = proxyUnfiltered.Remove(proxyUnfiltered.Length - 1);//Remove an ending / if it exits
            }
            Properties.Settings.Default.GlypeSpanish = proxyUnfiltered;
            //Get spanish HTTP Proxy
            Properties.Settings.Default.HTTPSpanish = TextBoxSpanishHTTPProxy.Text;
            //Radio group
            //TODO move to enum
            if (RadioSpanishUseGlype.IsChecked ?? false)
            {
                Properties.Settings.Default.ProxyOptionSpanish = "Glype";
            }
            else if (RadioSpanishUseHTTP.IsChecked ?? false)
            {
                Properties.Settings.Default.ProxyOptionSpanish = "HTTP";
            }
            else
            {
                Properties.Settings.Default.ProxyOptionSpanish = "None";
            }
        }


        /// <summary>
        /// Handles saving settings for the irish websites
        /// </summary>
        private void IrishSettingsSave()
        {
            //Get RTE proxy
            Properties.Settings.Default.HTTPIrish = TextBoxIrishHTTPProxy.Text;
            Properties.Settings.Default.IrishRegionOption = (RadioButtonIrishRegionIrish.IsChecked ?? false) ? "ie" : "us";
            if (!oldValRegionIrish.Equals(Properties.Settings.Default.IrishRegionOption))
            {
                cacheClear.Add(Site.RTE);
            }
        }

        /// <summary>
        /// Handles subtitle options
        /// </summary>
        private void SubtitleSettingsSave()
        {
            Properties.Settings.Default.DownloadSubtitlesSetting = CheckBoxSubtitleDownloadSetting.IsChecked ?? false;
            Properties.Settings.Default.ConvertSubtitle = CheckBoxSubtitleConvertSetting.IsChecked ?? false;
            Properties.Settings.Default.SubtitleFormat = (RadioButtonSRT.IsChecked ?? false) ? ".srt" : ".ass";
        }

        /// <summary>
        /// Handles the misc options
        /// </summary>
        private void MiscSettingsSave()
        {
            Properties.Settings.Default.ExitDLOnDownload = CheckBoxDownloadWindowClose.IsChecked ?? false;
            Properties.Settings.Default.SpaceReplace = CheckBoxSpaceReplace.IsChecked ?? false;
            Properties.Settings.Default.OverwriteFile = CheckBoxOverwrite.IsChecked ?? false;
            Properties.Settings.Default.LoadImages = CheckBoxLoadImages.IsChecked ?? false;
        }

        /// <summary>
        /// Handles checking of the ConvertSettings (Enabling and disabling its sub options)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxSubtitleConvertSetting_Click(object sender, RoutedEventArgs e)
        {
            if(CheckBoxSubtitleConvertSetting.IsChecked ?? false)
            {

                RadioButtonSRT.IsEnabled = true;
                RadioButtonASS.IsEnabled = true;
            }
            else
            {
                RadioButtonSRT.IsEnabled = false;
                RadioButtonASS.IsEnabled = false;
            }
        }
    }
}
