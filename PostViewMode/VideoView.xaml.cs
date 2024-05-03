using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KokomiAssistant.PostViewMode
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VideoView : Page
    {
        public VideoView()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string str = e.Parameter.ToString();
            JArray json = JArray.Parse(str);
            List<VodPlayStructedContent> VodContent = json.ToObject<List<VodPlayStructedContent>>();
            InsertVod vod;
            try { vod = JObject.Parse(VodContent[1].insert.ToString()).ToObject<InsertVod>();
                ViewDescribe.Text = "视频简介：\n" + VodContent[0].insert.ToString();
            }
            catch { vod = JObject.Parse(VodContent[0].insert.ToString()).ToObject<InsertVod>();
                ViewDescribe.Text = "视频简介：无\n";
            }
            VideoViewPlayer2.PosterSource = new BitmapImage(new Uri(vod.vod.cover));
            int max_rezid = vod.vod.resolutions.Count - 1;
            RezChoose.SelectedIndex = max_rezid;
            VideoViewPlayer2.Source = new Uri(vod.vod.resolutions[max_rezid].url);
            switch (max_rezid)
            {
                case 3:Rez_1440.Tag = vod.vod.resolutions[3].url; Rez_1080.Tag = vod.vod.resolutions[2].url; Rez_720.Tag = vod.vod.resolutions[1].url; Rez_480.Tag = vod.vod.resolutions[0].url;
                    break;
                case 2:Rez_1080.Tag= vod.vod.resolutions[2].url; Rez_720.Tag = vod.vod.resolutions[1].url; Rez_480.Tag = vod.vod.resolutions[0].url;
                    Rez_1440.Visibility = Visibility.Collapsed;
                    break;
                case 1: Rez_720.Tag = vod.vod.resolutions[1].url; Rez_480.Tag = vod.vod.resolutions[0].url;
                    Rez_1440.Visibility = Visibility.Collapsed; Rez_1080.Visibility = Visibility.Collapsed;
                    break;
                default: Rez_480.Tag = vod.vod.resolutions[0].url;
                    Rez_1440.Visibility = Visibility.Collapsed; Rez_1080.Visibility = Visibility.Collapsed; Rez_720.Visibility = Visibility.Collapsed;
                    break;
            }
            ViewNum.Text = "浏览量：" + vod.vod.view_num.ToString();
            
        }

        private void VideoDetail_Click(object sender, RoutedEventArgs e)
        {
            if((string)VideoDetailPanelControl.Tag=="0")
            {
                VideoViewVideoDetail.Width = 400;
                VideoViewVideoDetail.Height = double.NaN;
                VideoViewVideoDetail.Background=new SolidColorBrush(Windows.UI.Color.FromArgb(127,142,142,142));
                VideoViewVideoDetail.VerticalAlignment = VerticalAlignment.Stretch;
                VideoDetailPanelControl.Tag = "1";
            }
            else
            {
                VideoViewVideoDetail.Width = 40;
                VideoViewVideoDetail.Height = 40;
                VideoViewVideoDetail.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 142, 142, 142));
                VideoViewVideoDetail.VerticalAlignment = VerticalAlignment.Top;
                VideoDetailPanelControl.Tag = "0";
            }
        }

        private void RezChoose_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            int selecteditem = comboBox.SelectedIndex;
            try {
                switch(selecteditem)
                {
                    case 0: VideoViewPlayer2.Source = new Uri((string)Rez_480.Tag);break;
                    case 1: VideoViewPlayer2.Source = new Uri((string)Rez_720.Tag); break;
                    case 2: VideoViewPlayer2.Source = new Uri((string)Rez_1080.Tag); break;
                    case 3: VideoViewPlayer2.Source = new Uri((string)Rez_1440.Tag); break;
                    default: break;
                }
            }
            catch { }
        }
    }

    public class VodPlayStructedContent
    {
        public object insert { get; set; }
        
    }
    public class InsertVod
    {
        public Vodplay vod { get; set; }
    }

    public class Vodplay
    {
        public string id { get; set; }
        public int duration { get; set; }
        public string cover { get; set; }
        public List<Resolution> resolutions { get; set; }
        public int view_num { get; set; }
        public int transcoding_status { get; set; }
        public int review_status { get; set; }
    }
}
