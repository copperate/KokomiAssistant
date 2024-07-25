using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
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
    public sealed partial class MainPage2 : Page
    {
        public static MainPage2 navPage2;
        bool isReady=false;
        public MainPage2()
        {
            this.InitializeComponent();
            navPage2 = this;
            isReady = true;
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            //localSettings.Values["LoginUser"] = 0;
            if (localSettings.Values["LoginUser"] != null && (int)localSettings.Values["LoginUser"] != 0)
            {
                ImageBrush image1 = new ImageBrush();
                image1.ImageSource = new BitmapImage(new Uri(localSettings.Values["LoginUserAvatar"].ToString()));
                UserLoginAvatar.Fill = image1;
                UserLoginName.Text = localSettings.Values["LoginUsername"].ToString();
            }
        }


        private void MainPage2_NaviBar_ItemClicked(object sender, SelectionChangedEventArgs e)
        {
            int PageID = MainPage2_NaviBar.SelectedIndex;
            if (isReady)
            {
                MainPage2_RightBar.SelectedIndex = -1;
            }
            switch (PageID)
            {
                case 0: MainPageFrame.Margin = new Thickness(0, 40, 0, 0); 
                    MainPageFrame.Navigate(typeof(StatusPanel));
                    if (isReady)
                    {
                        MainPage2_MiddleBar.Visibility = Visibility.Collapsed;
                        MainPage2_ToolListBar.Visibility = Visibility.Collapsed;
                    }
                    
                    break;
                case 1: 
                    MainPage2_MiddleBar.Visibility = Visibility.Visible;
                    MainPage2_ToolListBar.Visibility = Visibility.Collapsed;
                    MainPageFrame.Margin = new Thickness(40, 40, 0, 0);
                    if (false) { }
                    else 
                    { 
                        MainPage2_MiddleBar.SelectedIndex = -1; 
                        MainPage2_MiddleBar.SelectedIndex = 0; 
                    }
                    break;
                case 2: 
                    MainPageFrame.Margin = new Thickness(40, 40, 0, 0);
                    MainPage2_ToolListBar.Visibility = Visibility.Visible;
                    MainPage2_MiddleBar.Visibility = Visibility.Collapsed;
                    MainPage2_ToolListBar.SelectedIndex = -1;
                    MainPage2_ToolListBar.SelectedIndex = 0;
                    break;
                default: break;
            }
        }
        private void MainPage2_RightBar_ItemClicked(object sender, SelectionChangedEventArgs e)
        {
            int PageID = MainPage2_RightBar.SelectedIndex;
            MainPageFrame.Margin = new Thickness(0, 40, 0, 0);
            MainPage2_MiddleBar.Visibility = Visibility.Collapsed; 
            MainPage2_ToolListBar.Visibility = Visibility.Collapsed;
            if (isReady) MainPage2_NaviBar.SelectedIndex = -1;
            switch (PageID)
            {
                case 0: MainPageFrame.Navigate(typeof(UserMsgPage));  break;
                case 1: MainPageFrame.Navigate(typeof(SettingsPanel));  break;
                default: break; 
            }
        }

        private void MainPage2_MiddleBar_ItemClicked(object sender, SelectionChangedEventArgs e)
        {
            int PageID = MainPage2_MiddleBar.SelectedIndex;
            MainPageFrame.Margin = new Thickness(40, 40, 0, 0); 
            switch (PageID)
            {
                case 0: MainPageFrame.Navigate(typeof(SocialPanelGenshin)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                case 1: MainPageFrame.Navigate(typeof(SocialPanelStarrail)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                case 2: MainPageFrame.Navigate(typeof(SocialPanelHonkai3)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                case 3: MainPageFrame.Navigate(typeof(SocialPagePanel.SocialPanelZenless)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                case 4: MainPageFrame.Navigate(typeof(SocialPagePanel.SocialPanelWd)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                case 5: MainPageFrame.Navigate(typeof(SocialPagePanel.SocialPanelDby)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                case 6: MainPageFrame.Navigate(typeof(SocialPagePanel.SocialPanelBh2)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                case 8: MainPageFrame.Navigate(typeof(SocialSearchPanel)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                default: break;
            }
        }

        private void NotifyPanel_ButtonClick(object sender, RoutedEventArgs e)
        {
            NotifyPane.Visibility = Visibility.Collapsed;
        }
        public async void NotifyPane_Activated(string message)
        {
            //NotifyPane.Height = 0;
            NotifyPane.Visibility = Visibility.Visible;
            NotifyDetail.Text = message;
            //for (int i = 1; i <= 1200; i++)if (i % 30 == 0) NotifyPane.Height++;
            var result = await PaneClose();
            NotifyPane.Visibility = Visibility.Collapsed;
        }
        public async Task<string> PaneClose()
        {
            return await Task.Run(() => {
                Thread.Sleep(5000); return "";
            });
        }

        private void MainPage2_ToolListBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int PageID = MainPage2_ToolListBar.SelectedIndex;
            MainPageFrame.Margin = new Thickness(40, 40, 0, 0);
            switch (PageID)
            {
                case 0: MainPageFrame.Navigate(typeof(ToolPanelGame)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                case 1: MainPageFrame.Navigate(typeof(ToolPanelAssistant)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                case 2: MainPageFrame.Navigate(typeof(ToolPanelPhoneLink)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                case 3: MainPageFrame.Navigate(typeof(ToolPanelChat)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                case 4: MainPageFrame.Navigate(typeof(ToolPanelDev)); if (isReady) MainPage2_RightBar.SelectedIndex = -1; break;
                
                default: break;
            }
        }
    }

}
