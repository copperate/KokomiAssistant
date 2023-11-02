using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        //String iks = "KokomiAssistant.";
        public MainPage()
        {
            this.InitializeComponent();
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

        private void NaviView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
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
        private bool TryGoBack()
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
        
    }
}
