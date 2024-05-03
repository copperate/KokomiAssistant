using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage navPage;
        public MainPage()
        {
            this.InitializeComponent();
            navPage = this;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void NaviView_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigated += On_Navigated;
            NaviView.SelectedItem = NaviView.MenuItems[0];
            NaviView_Navigate(typeof(StatusPanel),new EntranceNavigationTransitionInfo());
            
        }

        private void NaviView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true) 
            {
                NaviView_Navigate(typeof(SettingsPanel), args.RecommendedNavigationTransitionInfo);
            }
            else
            {
                if(args.InvokedItemContainer!=null)
                {
                    Type navPagetype = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                    NaviView_Navigate(navPagetype, args.RecommendedNavigationTransitionInfo);
                }
            }
        }

        public void NaviView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }
        private void NaviView_Navigate(Type navPageType,NavigationTransitionInfo transitionInfo)
        {
            Type preNavPageType = ContentFrame.CurrentSourcePageType;
            if(navPageType != null && !Type.Equals(preNavPageType,navPageType))
            {
                ContentFrame.Navigate(navPageType, null, transitionInfo);
            }
        }
        public bool TryGoBack()
        {
            if(!ContentFrame.CanGoBack)
            {
                return false;
            }
            if(NaviView.IsPaneOpen&&(NaviView.DisplayMode==NavigationViewDisplayMode.Compact|| NaviView.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;
            ContentFrame.GoBack();
            return true;
        }
        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NaviView.IsBackEnabled = ContentFrame.CanGoBack;
            if(ContentFrame.SourcePageType==typeof(SettingsPanel))
            {
                NaviView.SelectedItem=(NavigationViewItem)NaviView.SelectedItem;

            }
            else if(ContentFrame.SourcePageType!=null)
            {
                NaviView.SelectedItem = NaviView.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(i => i.Tag.Equals(ContentFrame.SourcePageType.FullName.ToString()));
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
                Thread.Sleep(5000);  return ""; 
            });
        }

    }
}
