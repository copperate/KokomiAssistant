using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ToolPanelChat : Page
    {
        public ToolPanelChat()
        {
            this.InitializeComponent();
            if(true)
            {
                VillaPcVersion.Visibility = Visibility.Collapsed;
            }
            else
            {
                VillaPcVersion.Visibility = Visibility.Visible;
                //useBrowserAsync();
            }
        }

        public async Task useBrowserAsync()
        {
            await InsideWebView.EnsureCoreWebView2Async();
            InsideWebView.CoreWebView2.Navigate("https://dby.miyoushe.com/");
        }


        private void ChannelPaneControl(object sender, TappedRoutedEventArgs e)
        {
            if ((string)ChannelPaneControlButton.Tag == "expand")
            {
                ChannelPaneControlButton.Tag = "collpsed";
                ChannelPaneControlButtonImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/Collpsed.png"));
                ChannelPane.OpenPaneLength = 300;
                ChannelPaneControlButton.Width = 40;
            }
            else
            {
                ChannelPaneControlButton.Tag = "expand";
                ChannelPaneControlButtonImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/Expand.png"));
                ChannelPane.OpenPaneLength = 60;
                ChannelPaneControlButton.Width = 60;
                
            }

        }

        private void VillaPaneControl(object sender, TappedRoutedEventArgs e)
        {
            if ((string)VillaPaneControlButton.Tag == "collpsed")
            {
                
                VillaPaneControlButton.Tag = "expand";
                VillaPaneControlButtonImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/Expand.png"));
                VillaPane.OpenPaneLength = 60;
                VillaPaneControlButton.Width = 60;
            }
            else
            {
                VillaPaneControlButton.Tag = "collpsed";
                VillaPaneControlButtonImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/Collpsed.png"));
                VillaPane.OpenPaneLength = 300;
                VillaPaneControlButton.Width = 40;

            }

        }
    }
}
