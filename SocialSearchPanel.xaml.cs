using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SocialSearchPanel : Page
    {
        bool isReady=false;
        public SocialSearchPanel()
        {
            this.InitializeComponent();
            isReady = true;
        }

        private async void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            ProgressBarA.IsIndeterminate = true;
            string keyword=SearchWordTextBox.Text;
            string areaid;
            if (ListSortAreaCombobox.SelectedIndex == 0) areaid = "";
            else areaid = ListSortAreaCombobox.SelectedIndex.ToString();
            Uri uri = new Uri("https://api-takumi.miyoushe.com/post/api/searchPosts?keyword="+keyword+"&last_id=&gids="+areaid+"&forum_id=&size=50&order_type="+ ListSortTypeCombobox.SelectedIndex);
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(SearchResultRoot));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (SearchResultRoot)serializer.ReadObject(ms);
            int listnum = data.data.posts.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            DateTime dt = new DateTime(1970, 1, 1);
            SearchResultList.Items.Clear();
            for (int i = 0; i < listnum; i++)
            {
                String Gameid; int gamearea;
                try { gamearea = data.data.posts[i].post.game_id; }
                catch (System.NullReferenceException) { gamearea = 0; }
                switch (gamearea)
                {
                    case 1: Gameid = "崩三"; break;
                    case 2: Gameid = "原神"; break;
                    case 3: Gameid = "崩二"; break;
                    case 4: Gameid = "未定"; break;
                    case 5: Gameid = "大别野"; break;
                    case 6: Gameid = "崩铁"; break;
                    case 8: Gameid = "绝区零"; break;
                    case 9981: Gameid = "Jabbr"; break;
                    default: Gameid = "未知"; break;

                }
                string areatext;
                try { areatext = Gameid + "·" + data.data.posts[i].forum.name; }
                catch (System.NullReferenceException) { areatext = Gameid + "(无版区)"; }

                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.posts[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.posts[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.posts[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.MaxLines = 1;
                summeryText.Text = data.data.posts[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.posts[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.posts[i].post.cover));
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
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.posts[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 120;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.posts[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 120;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.posts[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 120;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.posts[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                Grid postareaGrid = new Grid();
                postareaGrid.MinWidth = 120;
                Windows.UI.Xaml.Controls.Image postareaIcon = new Windows.UI.Xaml.Controls.Image();
                postareaIcon.Width = 20; postareaIcon.Height = 20; postareaIcon.HorizontalAlignment = HorizontalAlignment.Left; postareaIcon.VerticalAlignment = VerticalAlignment.Center;
                postareaIcon.Stretch = Stretch.Fill; postareaIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/location.png"));
                TextBlock postareaText = new TextBlock();
                postareaText.TextWrapping = TextWrapping.NoWrap; postareaText.TextTrimming = TextTrimming.CharacterEllipsis;
                postareaText.HorizontalAlignment = HorizontalAlignment.Left; postareaText.VerticalAlignment = VerticalAlignment.Top;
                postareaText.Margin = new Thickness(25, 0, 0, 0); postareaText.Text = areatext;
                postareaGrid.Children.Add(postareaIcon); postareaGrid.Children.Add(postareaText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid);poststatusGV.Items.Add(postareaGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.posts[i].post.post_id;

                SearchResultList.Items.Add(maingrid);
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
                SearchResultList.Items.Add(maingrid);
            }

            SearchResultList.Tag = data.data.last_id;
            ProgressBarA.IsIndeterminate = false;
        }

        private void NavigateDetail(object sender, ItemClickEventArgs e)
        {
            String postID;
            dynamic clickedItem = e.ClickedItem;
            postID = (string)clickedItem.Tag;
            if (postID == "loadmore")
            {
                int pivotselected = SearchPivot.SelectedIndex;
                switch (pivotselected)
                {
                    case 0: PostPage_Loadmore(); break;
                    case 1: break;
                    case 2: break;
                    case 3: break;
                    default: break;
                }
            }
            else
                Frame.Navigate(typeof(PostDetailPanel), postID);
            //}
        }

        private void SearchWordTextbox_Keydown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            { 
                SearchButtonClicked(sender, e);
            }
        }

        private async void PostPage_Loadmore()
        {
            ProgressBarA.IsIndeterminate = true;
            string keyword = SearchWordTextBox.Text;
            string areaid;
            string last_id = SearchResultList.Tag.ToString();
            SearchResultList.Items.RemoveAt(SearchResultList.Items.Count - 1);
            if (ListSortAreaCombobox.SelectedIndex == 0) areaid = "";
            else areaid = ListSortAreaCombobox.SelectedIndex.ToString();
            Uri uri = new Uri("https://api-takumi.miyoushe.com/post/api/searchPosts?keyword=" + keyword + "&last_id="+last_id+"&gids=" + areaid + "&forum_id=&size=50&order_type=" + ListSortTypeCombobox.SelectedIndex);
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(SearchResultRoot));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (SearchResultRoot)serializer.ReadObject(ms);
            int listnum = data.data.posts.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            DateTime dt = new DateTime(1970, 1, 1);
            for (int i = 0; i < listnum; i++)
            {
                String Gameid; int gamearea;
                try { gamearea = data.data.posts[i].post.game_id; }
                catch (System.NullReferenceException) { gamearea = 0; }
                switch (gamearea)
                {
                    case 1: Gameid = "崩三"; break;
                    case 2: Gameid = "原神"; break;
                    case 3: Gameid = "崩二"; break;
                    case 4: Gameid = "未定"; break;
                    case 5: Gameid = "大别野"; break;
                    case 6: Gameid = "崩铁"; break;
                    case 8: Gameid = "绝区零"; break;
                    case 9981: Gameid = "Jabbr"; break;
                    default: Gameid = "未知"; break;

                }
                string areatext;
                try { areatext = Gameid + "·" + data.data.posts[i].forum.name; }
                catch (System.NullReferenceException) { areatext = Gameid + "(无版区)"; }

                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 30; avatarellipse.Width = 30;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(10, 10, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.posts[i].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                maingrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = data.data.posts[i].user.nickname;
                maingrid.Children.Add(usernameText);

                TextBlock posttitleText = new TextBlock();
                posttitleText.VerticalAlignment = VerticalAlignment.Top; posttitleText.HorizontalAlignment = HorizontalAlignment.Stretch;
                posttitleText.Margin = new Thickness(10, 50, 0, 0); posttitleText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                posttitleText.FontSize = 18; posttitleText.FontWeight = Windows.UI.Text.FontWeights.Bold;
                posttitleText.Text = data.data.posts[i].post.subject;
                maingrid.Children.Add(posttitleText);

                TextBlock summeryText = new TextBlock();
                summeryText.VerticalAlignment = VerticalAlignment.Stretch; summeryText.HorizontalAlignment = HorizontalAlignment.Stretch;
                summeryText.Margin = new Thickness(10, 84, 9, 40); summeryText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                summeryText.FontSize = 18; summeryText.TextTrimming = TextTrimming.CharacterEllipsis;
                summeryText.MaxLines = 1;
                summeryText.Text = data.data.posts[i].post.content + blankstring;
                maingrid.Children.Add(summeryText);

                if (!string.IsNullOrEmpty(data.data.posts[i].post.cover))
                {
                    Windows.UI.Xaml.Controls.Image postImg = new Windows.UI.Xaml.Controls.Image();
                    postImg.HorizontalAlignment = HorizontalAlignment.Left; postImg.MaxHeight = 120; postImg.Margin = new Thickness(20, 120, 20, 40);
                    postImg.Source = new BitmapImage(new Uri(data.data.posts[i].post.cover));
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
                viewNumText.Margin = new Thickness(25, 0, 0, 0); viewNumText.Text = data.data.posts[i].stat.view_num.ToString();
                viewnumGrid.Children.Add(viewNumIcon); viewnumGrid.Children.Add(viewNumText);

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 120;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = data.data.posts[i].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 120;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = data.data.posts[i].stat.reply_num.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                Grid posttimeGrid = new Grid();
                posttimeGrid.MinWidth = 120;
                Windows.UI.Xaml.Controls.Image posttimeIcon = new Windows.UI.Xaml.Controls.Image();
                posttimeIcon.Width = 20; posttimeIcon.Height = 20; posttimeIcon.HorizontalAlignment = HorizontalAlignment.Left; posttimeIcon.VerticalAlignment = VerticalAlignment.Center;
                posttimeIcon.Stretch = Stretch.Fill; posttimeIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/time.png"));
                TextBlock posttimeText = new TextBlock();
                posttimeText.TextWrapping = TextWrapping.Wrap; posttimeText.TextTrimming = TextTrimming.CharacterEllipsis;
                posttimeText.HorizontalAlignment = HorizontalAlignment.Left; posttimeText.VerticalAlignment = VerticalAlignment.Top;
                posttimeText.Margin = new Thickness(25, 0, 0, 0); posttimeText.Text = dt.AddSeconds(data.data.posts[i].post.created_at).ToLocalTime().ToString();
                posttimeGrid.Children.Add(posttimeIcon); posttimeGrid.Children.Add(posttimeText);

                Grid postareaGrid = new Grid();
                postareaGrid.MinWidth = 120;
                Windows.UI.Xaml.Controls.Image postareaIcon = new Windows.UI.Xaml.Controls.Image();
                postareaIcon.Width = 20; postareaIcon.Height = 20; postareaIcon.HorizontalAlignment = HorizontalAlignment.Left; postareaIcon.VerticalAlignment = VerticalAlignment.Center;
                postareaIcon.Stretch = Stretch.Fill; postareaIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/location.png"));
                TextBlock postareaText = new TextBlock();
                postareaText.TextWrapping = TextWrapping.NoWrap; postareaText.TextTrimming = TextTrimming.CharacterEllipsis;
                postareaText.HorizontalAlignment = HorizontalAlignment.Left; postareaText.VerticalAlignment = VerticalAlignment.Top;
                postareaText.Margin = new Thickness(25, 0, 0, 0); postareaText.Text = areatext;
                postareaGrid.Children.Add(postareaIcon); postareaGrid.Children.Add(postareaText);

                poststatusGV.Items.Add(viewnumGrid); poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid); poststatusGV.Items.Add(posttimeGrid); poststatusGV.Items.Add(postareaGrid);

                maingrid.Children.Add(poststatusGV);
                maingrid.Tag = data.data.posts[i].post.post_id;

                SearchResultList.Items.Add(maingrid);
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
                SearchResultList.Items.Add(maingrid);
            }

            SearchResultList.Tag = data.data.last_id;
            ProgressBarA.IsIndeterminate = false;

        }
        private void ListSortTypeChange(object sender, SelectionChangedEventArgs e)
        {
            if(isReady)
            {
                SearchButtonClicked(null,null);
            }
        }
    }

    public class SearchResultRoot
    {
        public string message { get; set; }
        public int retcode { get; set; }
        public SearchResultData data { get; set; }
    }
    public class SearchResultData
    {
        public List<List> posts { get; set; }
        public string last_id { get; set; }
        public bool islast { get; set; }
        public List<string> token_list { get; set; }   
        public List<object> databox { get; set; }
    }
    public class ListViewSearchPostGallery
    {
        public string Tag { get; set; }
        public string UserPic { get; set; }
        public string User { get; set; }
        public string PostTitle { get; set; }
        public string PostSummary { get; set; }
        public string PostPic { get; set; }
        public int ViewNum { get; set; }
        public int LikeNum { get; set; }
        public int CommentNum { get; set; }
        public string last_id { get; set; }
        public string PubTime { get; set; }
        public string PubArea {  get; set; }
    }
}
