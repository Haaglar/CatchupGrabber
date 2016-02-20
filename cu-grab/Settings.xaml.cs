using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace cu_grab
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();

            TextBoxSpanishHTTPProxy.Text = Properties.Settings.Default.HTTPSpanish;
            TextBoxSpanishGlypeProxy.Text = Properties.Settings.Default.GlypeSpanish;
            TextBoxIrishHTTPProxy.Text = Properties.Settings.Default.HTTPIrish;
            switch(Properties.Settings.Default.ProxyOptionSpanish)
            {
                case("None"):
                    RadioSpanishUseNone.IsChecked = true;
                    break;
                case("Gylpe"):
                    RadioSpanishUseGlype.IsChecked = true;
                    break;
                case("HTTP"):
                    RadioSpanishUseHTTP.IsChecked = true;
                    break;
            }


            CheckBoxSubtitleDownloadSetting.IsChecked = Properties.Settings.Default.DownloadSubtitlesSetting;
            CheckBoxSubtitleConvertSetting.IsChecked = Properties.Settings.Default.ConvertSubtitle;
            this.PreviewKeyDown += EscExit_PreviewKeyDown;
        }

        private void ButtonSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            //Get Spansih Glype proxy
            String proxyUnfiltered = TextBoxSpanishGlypeProxy.Text;
            if (proxyUnfiltered != "")
            {
                if(!proxyUnfiltered.StartsWith("http://"))
                    proxyUnfiltered = "http://" + proxyUnfiltered;//Add nessesary http
                if(proxyUnfiltered.EndsWith("/"))
                       proxyUnfiltered = proxyUnfiltered.Remove(proxyUnfiltered.Length - 1);//Remove an ending / if it exits
            }
            Properties.Settings.Default.GlypeSpanish = proxyUnfiltered;
            //Get spanish HTTP Proxy
            Properties.Settings.Default.HTTPSpanish = TextBoxSpanishHTTPProxy.Text;
            //Radio group
            //TODO move to enum
            if(RadioSpanishUseGlype.IsChecked ?? false)
            {
                Properties.Settings.Default.ProxyOptionSpanish = "Gylpe";
            }
            else if (RadioSpanishUseHTTP.IsChecked?? false)
            {
                Properties.Settings.Default.ProxyOptionSpanish = "HTTP";
            }
            else
            {
                Properties.Settings.Default.ProxyOptionSpanish = "None";
            }


            //Get RTE proxy
            proxyUnfiltered = TextBoxIrishHTTPProxy.Text;
            Properties.Settings.Default.HTTPIrish = proxyUnfiltered;


            //Checkbox for sub download
            Properties.Settings.Default.DownloadSubtitlesSetting = CheckBoxSubtitleDownloadSetting.IsChecked ?? false;
            Properties.Settings.Default.ConvertSubtitle = CheckBoxSubtitleConvertSetting.IsChecked ?? false;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void ButtonCancelSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EscExit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }
    }
}
