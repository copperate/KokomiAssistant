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
using Windows.UI.Xaml.Navigation;
using Windows.Data.Json;
using Windows.Storage;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Contacts;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SocialPanelGenshin : Page
    {
        public SocialPanelGenshin()
        {
            this.InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GetpostList_Recommend(object sender, RoutedEventArgs e)
        {

        }

        private void GetpostList_Offical(object sender, RoutedEventArgs e)
        {

        }
        private void GetpostList_Jiuguan(object sender, RoutedEventArgs e)
        {

        }

        private void NavigateDetail(object sender, ItemClickEventArgs e)
        {
            String postID;
            dynamic clickedItem = e.ClickedItem;
                postID = (string)clickedItem.Tag;
                Frame.Navigate(typeof(PostDetailPanel), postID);
            //}
        }

        private void GetpostList_Offical(FrameworkElement sender, object args)
        {

        }

        private void NavigateViaID(object sender, RoutedEventArgs e)
        {
            String postID = (string)navigateidbox.Text.ToString();
            Frame.Navigate(typeof(PostDetailPanel), postID);
        }
    }
    


}
