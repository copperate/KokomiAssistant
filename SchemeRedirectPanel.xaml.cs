using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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
using Windows.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;

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
            //base.OnNavigatedTo(e);
            if (eventargs.Host == "article") {
                ContentFrameView.Navigate(typeof(PostDetailPanel), localpath);
            }
            if (eventargs.Host == "user") {
                ContentFrameView.Navigate(typeof(UserDetailPanel), localpath); 
            }
            if (eventargs.Host == "home") { ContentFrameView.Navigate(typeof(StatusPanel), localpath); }
            if (eventargs.Host == "webview") { //ContentFrameView.Content = eventargs.Query.Substring(1);
                string text = eventargs.Query.Substring(6);
                string uri = System.Web.HttpUtility.UrlDecode(text, System.Text.Encoding.UTF8);
                ContentFrameView.Navigate(typeof(ToolPanelInsideWebView), uri);
            }
            if (eventargs.Host == "openurl") {
                string text = eventargs.Query.Substring(eventargs.Query.IndexOf("url=") + 4);
                string uri = System.Web.HttpUtility.UrlDecode(text, System.Text.Encoding.UTF8);
                //原神版区的冒险互助专区转为帖子
                if (uri.Contains("qaa.miyoushe.com/ys_help") && uri.Contains("articleDetail"))
                {
                    string postid = uri.Substring(uri.IndexOf("articleDetail/") + 14);
                    ContentFrameView.Navigate(typeof(PostDetailPanel), postid);
                }
                //其它网页仍正常通过浏览器访问
                else ContentFrameView.Navigate(typeof(ToolPanelInsideWebView), uri);
            }


            /*if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            {
                
            }
            else
            {

            }*/
            

        }
        private void NavigateBackToHome(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Navigate(object sender, TappedRoutedEventArgs e)
        {

        }

        private void SchemeRedirectBackbutton_clicked(object sender, RoutedEventArgs e)
        {
            try { ContentFrameView.GoBack(); } catch { Frame.Navigate(typeof(MainPage)); }
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
    }
}
