using System;
using System.Collections.Generic;
using System.Drawing;
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
    public sealed partial class ToolPanelInsideWebView : Page
    {
        bool isReady = false;
        public ToolPanelInsideWebView()
        {
            this.InitializeComponent();
            isReady = true;
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if ((e.Parameter is Uri || e.Parameter is string) && !string.IsNullOrWhiteSpace(e.Parameter.ToString()))
            {
                LoadProgress.IsIndeterminate = true;
                string uri=e.Parameter.ToString();
                await InsideWebView.EnsureCoreWebView2Async();

                InsideWebView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows Phone 10.0; Android 6.0.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.5845.179/180 Mobile Safari/537.36 miHoYoBBS/2.59.1 KokomiAssistant/0.1.3";
                InsideWebView.CoreWebView2.Navigate(uri);
                InsideViewAddressBar.Text = uri;
                LoadProgress.IsIndeterminate = false;
            }
            else
            {

            }
        }
        
        private void InsideViewButton_refresh(object sender, RoutedEventArgs e)
        {
            InsideWebView.CoreWebView2.Reload();
        }

        private void InsideViewButton_back(object sender, RoutedEventArgs e)
        {
            InsideWebView.CoreWebView2.GoBack();
        }

        private void InsideViewButton_forward(object sender, RoutedEventArgs e)
        {
            InsideWebView.CoreWebView2.Resume();
        }

        private void BrowserViewmodeChanged(object sender, RoutedEventArgs e)
        {
            if(!isReady) return;
            ToggleSwitch toggleSwitch=sender as ToggleSwitch;
            if(toggleSwitch.IsOn)
            {
                InsideWebView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows Phone 10.0; Android 6.0.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.5845.179/180 Mobile Safari/537.36 miHoYoBBS/2.59.1 KokomiAssistant/0.1.3";
            }
            else
            {
                InsideWebView.CoreWebView2.Settings.UserAgent = "KokomiAssistant/0.1.3";
            }
            InsideWebView.CoreWebView2.Reload();
        }
    }
}
