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
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Documents;
using KokomiAssistant.PostViewMode;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SocialPanelGenshin : Page
    {
        Frame rootFrame = Window.Current.Content as Frame;
        DateTime dt = new DateTime(1970, 1, 1);
        bool isReady= false;
        List<string> BlockwordList = new List<string>();

        public SocialPanelGenshin()
        {
            this.InitializeComponent();
            isReady = true;
            refreshblockwords();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        public void refreshblockwords()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["BrowseSettings_BlockEnabled"] == null || !(bool)localSettings.Values["BrowseSettings_BlockEnabled"]) return;
            string blockwords = "";
            if (localSettings.Values["BrowseSettings_Blockwords"] != null)
                blockwords = localSettings.Values["BrowseSettings_Blockwords"].ToString();
            while (blockwords != "" && blockwords != null)
            {
                int sepchar = blockwords.IndexOf(",");
                BlockwordList.Add(blockwords.Substring(0, sepchar));
                blockwords = blockwords.Substring(sepchar + 1);
            }
        }
        
        private async void GetpostListA_Recommend(FrameworkElement sender, object args)
        {
            ProgressStatus.IsIndeterminate = true;
            Uri uri = new Uri("https://api-takumi.miyoushe.com/post/api/feeds/posts?fresh_action=1&gids=2&last_id=");
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RootObject));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (RootObject)serializer.ReadObject(ms);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";

            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0);maingrid.Padding = new Thickness(0, 0, 0, 0);maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30;avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left;avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource=new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText= new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top;usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0);usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText= new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top;posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0);posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18;posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText= new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left;postImg.MaxHeight = 120;postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source=new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }
                

                GridView poststatusGV= new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40;poststatusGV.Margin = new Thickness(10, 0, 0, 0);poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top;poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false;poststatusGV.IsHitTestVisible = false;poststatusGV.IsEnabled = false;poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20;viewNumIcon.Height = 20;viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left;viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill;viewNumIcon.Source= new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText= new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left;viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0);viewNumText.Text =data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid);poststatusGV.Items.Add(likenumGrid);poststatusGV.Items.Add(commentnumGrid);poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinDiscoverList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinDiscoverList.Items.Add(maingrid);
            }

            GenshinDiscoverList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
            
           // catch { ((Window.Current.Content as Frame).Content as MainPage2).NotifyPane_Activated("页面加载失败，请检查网路连接。"); }
        }

        private async void GetpostList_Extend_Recommend(object sender, PointerRoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id = GenshinDiscoverList.Tag.ToString();
            Uri uri = new Uri("https://api-takumi.miyoushe.com/post/api/feeds/posts?fresh_action=1&gids=2&last_id="+last_id);
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RootObject));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (RootObject)serializer.ReadObject(ms);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            GenshinDiscoverList.Items.RemoveAt(GenshinDiscoverList.Items.Count - 1);
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Top;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinDiscoverList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinDiscoverList.Items.Add(maingrid);
            }
            GenshinDiscoverList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void GetpostList_Offical(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            RootObject data = await PostList.GetPostList(28, ListSortTypeCombobox.SelectedIndex, "");
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinOfficalList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinOfficalList.Items.Add(maingrid);
            }
            GenshinOfficalList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void GetpostList_Offical_Extend(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id = GenshinOfficalList.Tag.ToString();
            RootObject data = await PostList.GetPostList(28, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            GenshinOfficalList.Items.RemoveAt(GenshinOfficalList.Items.Count - 1);
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinOfficalList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinOfficalList.Items.Add(maingrid);
            }
            GenshinOfficalList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void GetpostList_Bar(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            RootObject data = await PostList.GetPostList(26, ListSortTypeCombobox.SelectedIndex, "");
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for(int j = 0;j<BlockwordList.Count;j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;
                
                GenshinBarList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinBarList.Items.Add(maingrid);
            }
            GenshinBarList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void GetpostList_Bar_Extend(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id = GenshinBarList.Tag.ToString();
            RootObject data = await PostList.GetPostList(26, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            GenshinBarList.Items.RemoveAt(GenshinBarList.Items.Count - 1);
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinBarList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinBarList.Items.Add(maingrid);
            }
            GenshinBarList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private void NavigateDetail(object sender, ItemClickEventArgs e)
        {
            String postID;
            dynamic clickedItem = e.ClickedItem;
            postID = (string)clickedItem.Tag;
            if (postID == "loadmore")
            { 
                int pivotselected = GenshinPivot.SelectedIndex;
                switch (pivotselected)
                {
                    case 0:GetpostList_Extend_Recommend(null, null);break;
                    case 2:GetpostList_Bar_Extend(null, null); break;
                    case 1:GetpostList_Offical_Extend(null, null); break;
                    case 3:GenshinGonglveList_Extend(null,null); break;
                    case 4:GenshinFanPicList_Extend(null, null); break; 
                    case 5:GenshinCosplayList_Extend(null, null); break;
                    case 6:GenshinHardcoreList_Extend(null, null); break;
                    default:break;
                }
            }
            else
                rootFrame.Navigate(typeof(PostDetailPanel), postID);
            
        }

        private async void GenshinGonglveList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            RootObject data = await PostList.GetPostList(43, ListSortTypeCombobox.SelectedIndex, "");
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinGonglveList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinGonglveList.Items.Add(maingrid);
            }
            GenshinGonglveList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void GenshinGonglveList_Extend(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id = GenshinGonglveList.Tag.ToString();
            RootObject data = await PostList.GetPostList(43, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            GenshinGonglveList.Items.RemoveAt(GenshinDiscoverList.Items.Count - 1);
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinGonglveList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinGonglveList.Items.Add(maingrid);
            }
            GenshinGonglveList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void GenshinFanPicList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            RootObject data = await PostList.GetPostList(29, ListSortTypeCombobox.SelectedIndex, "");
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinFanPicList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinFanPicList.Items.Add(maingrid);
            }
            GenshinFanPicList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void GenshinFanPicList_Extend(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id = GenshinFanPicList.Tag.ToString();
            RootObject data = await PostList.GetPostList(29, ListSortTypeCombobox.SelectedIndex, "");
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            GenshinFanPicList.Items.RemoveAt(GenshinFanPicList.Items.Count - 1);
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinFanPicList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinFanPicList.Items.Add(maingrid);
            }
            GenshinFanPicList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void GenshinCosplayList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            RootObject data = await PostList.GetPostList(49, ListSortTypeCombobox.SelectedIndex, "");
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinCosplayList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinCosplayList.Items.Add(maingrid);
            }
            GenshinCosplayList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void GenshinCosplayList_Extend(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id = GenshinCosplayList.Tag.ToString();
            RootObject data = await PostList.GetPostList(49, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            GenshinCosplayList.Items.RemoveAt(GenshinCosplayList.Items.Count - 1);
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinCosplayList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinCosplayList.Items.Add(maingrid);
            }
            GenshinCosplayList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void GenshinHardcoreList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            RootObject data = await PostList.GetPostList(50, ListSortTypeCombobox.SelectedIndex, "");
            int listnum = data.data.list.Count; 
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinHardcoreList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinHardcoreList.Items.Add(maingrid);
            }
            GenshinHardcoreList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void GenshinHardcoreList_Extend(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id = GenshinHardcoreList.Tag.ToString();
            RootObject data = await PostList.GetPostList(50, ListSortTypeCombobox.SelectedIndex, "");
            GenshinHardcoreList.Items.RemoveAt(GenshinHardcoreList.Items.Count - 1);
            int listnum = data.data.list.Count; 
            for (int i = 0; i < listnum; i++)
            {
                //关键词检查
                bool containword = false;
                for (int j = 0; j < BlockwordList.Count; j++)
                {
                    if (data.data.list[i].user.nickname.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.subject.Contains(BlockwordList[j])) containword = true;
                    if (data.data.list[i].post.content.Contains(BlockwordList[j])) containword = true;
                }
                if (containword) continue;
                //加载列表
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.list[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.list[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.Text = data.data.list[i].post.content;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.list[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.list[i].post.cover));
                    maingrid.Children.Add(postImg);
                }


                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid viewnumGrid = new Grid();
                viewnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image viewNumIcon = new Windows.UI.Xaml.Controls.Image();
                viewNumIcon.Width = 20; viewNumIcon.Height = 20; viewNumIcon.HorizontalAlignment = HorizontalAlignment.Left; viewNumIcon.VerticalAlignment = VerticalAlignment.Top;
                viewNumIcon.Stretch = Stretch.Fill; viewNumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/view.png"));
                TextBlock viewNumText = new TextBlock();
                viewNumText.HorizontalAlignment = HorizontalAlignment.Left; viewNumText.VerticalAlignment = VerticalAlignment.Top;
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.list[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.list[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.list[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.list[i].post.post_id;

                GenshinHardcoreList.Items.Add(maingrid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "加载更多……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                GenshinHardcoreList.Items.Add(maingrid);
            }
            GenshinHardcoreList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private void AppbarButton_Refresh_Click(object sender, RoutedEventArgs e)
        {
            int pivotselected = GenshinPivot.SelectedIndex;
            ProgressStatus.IsIndeterminate = true;
            switch (pivotselected)
            {
                case 0:GenshinDiscoverList.Items.Clear();GetpostListA_Recommend(null,null);break;
                case 2: GenshinBarList.Items.Clear(); GetpostList_Bar(null, null); break;
                case 1:GenshinOfficalList.Items.Clear(); GetpostList_Offical(null, null); break;
                case 3: GenshinGonglveList.Items.Clear(); GenshinGonglveList_Loaded(null,null); break;
                case 4:GenshinFanPicList.Items.Clear(); GenshinFanPicList_Loaded(null, null); break; 
                case 5:GenshinCosplayList.Items.Clear(); GenshinCosplayList_Loaded(null, null); break;
                case 6:GenshinHardcoreList.Items.Clear(); GenshinHardcoreList_Loaded(null, null); break;
                default:
                    break;
            }
            ProgressStatus.IsIndeterminate = false;
        }

        private void GotoSearchPanel(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SocialSearchPanel), null);
        }

        private void ListSortTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            int pivotselected = GenshinPivot.SelectedIndex;

            if (isReady)
            {
                ProgressStatus.IsIndeterminate = true;
                switch (pivotselected)
                {
                    case 0: break;
                    case 2: GenshinBarList.Items.Clear(); GetpostList_Bar(null, null); break;
                    case 1: GenshinOfficalList.Items.Clear(); GetpostList_Offical(null, null); break;
                    case 3: GenshinGonglveList.Items.Clear(); GenshinGonglveList_Loaded(null, null); break;
                    case 4: GenshinFanPicList.Items.Clear(); GenshinFanPicList_Loaded(null, null); break;
                    case 5: GenshinCosplayList.Items.Clear(); GenshinCosplayList_Loaded(null, null); break;
                    case 6: GenshinHardcoreList.Items.Clear(); GenshinHardcoreList_Loaded(null, null); break;
                    default:
                        break;
                }
                ProgressStatus.IsIndeterminate = false;
            }

           
        }
    }

    public class ListViewPostGallery
    {
        public string Tag { get; set; }
        public string UserPic { get; set; }
        public string User { get; set; }
        public string PostTitle { get; set; }
        public string PostSummary { get; set; }
        public string PostPic { get; set; }
        public int ViewNum { get; set; }
        public int LikeNum { get;set; }
        public int CommentNum { get; set; }
        public string last_id { get; set; }
        public string PubTime { get; set; }
    }

}
