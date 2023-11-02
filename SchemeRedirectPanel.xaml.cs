using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SchemeRedirectPanel : Page
    {
        public SchemeRedirectPanel()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Uri eventargs=e.Parameter as Uri;
            string schemehost = eventargs.Scheme;
            string localpath = eventargs.AbsolutePath;
            localpath = localpath.Substring(1);
            if (eventargs.Host == "article") { ContentFrameView.Navigate(typeof(PostDetailPanel), localpath); }
            if (eventargs.Host == "user") { ContentFrameView.Navigate(typeof(UserDetailPanel), localpath); }
            if (eventargs.Host == "home") { ContentFrameView.Navigate(typeof(StatusPanel), localpath); }
            if (eventargs.Host == "webview") { ContentFrameView.Content = eventargs.Query.Substring(1); }
            

            /*if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            {
                
            }
            else
            {

            }*/
            //base.OnNavigatedTo(e);
            
        }
        private void NavigateBackToHome(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Navigate(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
