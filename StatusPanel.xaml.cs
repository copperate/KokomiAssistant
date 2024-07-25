using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class StatusPanel : Page
    {
        public StatusPanel()
        {
            this.InitializeComponent();
        }

        private void BackgroundLoad(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String selectedIndex;
            try
            {
                selectedIndex = localSettings.Values["Background"]as string;    
                
            }
            catch
            {
                selectedIndex = "ms-appx:///Assets/Background/Wallpaper01.png";
            }
            if(selectedIndex==null) selectedIndex = "ms-appx:///Assets/Background/Wallpaper01.png";
            StatusPanelBackground.Source = new BitmapImage(new Uri(selectedIndex));
            
        }

        private async void Gamestart_Genshin(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            try
            {
                string startlocation = localSettings.Values["AppLocationGenshin"] as string;
                //await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                //System.Diagnostics.Process.Start("Assets/Plugins/Launcher.exe --run");
            }
            catch
            {
                ((Window.Current.Content as Frame).Content as MainPage2).NotifyPane_Activated("运行失败，请检查设置。");
            }
        }

        private void SampleQuickTest(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PostDetailPanel), "53411892");
        }
    }
}
