using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

namespace KokomiAssistant.SocialPagePanel
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SocialPanelDby : Page
    {
        Frame rootFrame = Window.Current.Content as Frame;
        DateTime dt = new DateTime(1970, 1, 1);
        bool isReady = false;

        public SocialPanelDby()
        {
            this.InitializeComponent();
            isReady = true;

        }
        private void NavigateDetail(object sender, ItemClickEventArgs e)
        {
            String postID;
            dynamic clickedItem = e.ClickedItem;
            postID = (string)clickedItem.Tag;
            if (postID == "loadmore")
            {
                int pivotselected = DbyPivot.SelectedIndex;
                switch (pivotselected)
                {
                    case 0: DiscoverListLoad(null, e); break;
                    case 1: Area1lList_Loaded(null, e); break;
                    case 2: Area2List_Loaded(null, e); break;
                    case 3: Area3List_Loaded(null, e); break;
                    case 4: Area4List_Loaded(null, e); break;
                    case 5: Area5List_Loaded(null, e); break;
                    case 6: Area6List_Loaded(null, e); break;
                    case 7: Area7List_Loaded(null, e); break;
                    case 8: Area8List_Loaded(null, e); break;
                    default: break;
                }
            }
            else
                rootFrame.Navigate(typeof(PostDetailPanel), postID);

        }
        private void ListSortTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            int pivotselected = DbyPivot.SelectedIndex;

            if (isReady)
            {
                ProgressStatus.IsIndeterminate = true;
                switch (pivotselected)
                {
                    case 0: break;
                    case 1: Area1lList.Items.Clear();Area1lList.Tag = ""; Area1lList_Loaded(null, e); break;
                    case 2: Area2List.Items.Clear(); Area2List.Tag = ""; Area2List_Loaded(null, e); break;
                    case 3: Area3List.Items.Clear(); Area3List.Tag = ""; Area3List_Loaded(null, e); break;
                    case 4: Area4List.Items.Clear(); Area4List.Tag = ""; Area4List_Loaded(null, e); break;
                    case 5: Area5List.Items.Clear(); Area5List.Tag = ""; Area5List_Loaded(null, e); break;
                    case 6: Area6List.Items.Clear(); Area6List.Tag = ""; Area6List_Loaded(null, e); break;
                    case 7: Area7List.Items.Clear(); Area7List.Tag = ""; Area7List_Loaded(null, e); break;
                    case 8: Area8List.Items.Clear(); Area8List.Tag = ""; Area8List_Loaded(null, e); break;
                    default: break;
                }
                ProgressStatus.IsIndeterminate = false;
            }
        }
        private void AppbarButton_Refresh_Click(object sender, RoutedEventArgs e)
        {
            int pivotselected = DbyPivot.SelectedIndex;

            if (isReady)
            {
                ProgressStatus.IsIndeterminate = true;
                switch (pivotselected)
                {
                    case 0: break;
                    case 1: Area1lList.Items.Clear(); Area1lList.Tag = ""; Area1lList_Loaded(null, e); break;
                    case 2: Area2List.Items.Clear(); Area2List.Tag = ""; Area2List_Loaded(null, e); break;
                    case 3: Area3List.Items.Clear(); Area3List.Tag = ""; Area3List_Loaded(null, e); break;
                    case 4: Area4List.Items.Clear(); Area4List.Tag = ""; Area4List_Loaded(null, e); break;
                    case 5: Area5List.Items.Clear(); Area5List.Tag = ""; Area5List_Loaded(null, e); break;
                    case 6: Area6List.Items.Clear(); Area6List.Tag = ""; Area6List_Loaded(null, e); break;
                    case 7: Area7List.Items.Clear(); Area7List.Tag = ""; Area7List_Loaded(null, e); break;
                    case 8: Area8List.Items.Clear(); Area8List.Tag = ""; Area8List_Loaded(null, e); break;
                    default: break;
                }
                ProgressStatus.IsIndeterminate = false;
            }
        }
    
        private void GotoSearchPanel(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SocialSearchPanel), null);
        }
        private async void DiscoverListLoad(FrameworkElement sender, object args)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (DiscoverList.Tag != null && (string)DiscoverList.Tag != "")
            {
                last_id = DiscoverList.Tag.ToString();
                DiscoverList.Items.RemoveAt(DiscoverList.Items.Count - 1);
            }
            else last_id = "";
            Uri uri = new Uri("https://api-takumi.miyoushe.com/post/api/feeds/posts?fresh_action=1&gids=5&last_id=" + last_id);
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

                DiscoverList.Items.Add(maingrid);
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
                DiscoverList.Items.Add(maingrid);
            }

            DiscoverList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;

        }

        private async void Area2List_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (Area2List.Tag != null && (string)Area2List.Tag != "")
            {
                last_id = Area2List.Tag.ToString();
                Area2List.Items.RemoveAt(Area2List.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(35, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
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

                Area2List.Items.Add(maingrid);
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
                Area2List.Items.Add(maingrid);
            }

            Area2List.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void Area1lList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (Area1lList.Tag != null && (string)Area1lList.Tag != "")
            {
                last_id = Area1lList.Tag.ToString();
                Area1lList.Items.RemoveAt(Area1lList.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(36, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
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

                Area1lList.Items.Add(maingrid);
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
                Area1lList.Items.Add(maingrid);
            }

            Area1lList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void Area3List_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (Area3List.Tag != null && (string)Area3List.Tag != "")
            {
                last_id = Area3List.Tag.ToString();
                Area3List.Items.RemoveAt(Area3List.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(34, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
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

                Area3List.Items.Add(maingrid);
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
                Area3List.Items.Add(maingrid);
            }

            Area3List.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void Area4List_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (Area4List.Tag != null && (string)Area4List.Tag != "")
            {
                last_id = Area4List.Tag.ToString();
                Area4List.Items.RemoveAt(Area4List.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(54, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
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

                Area4List.Items.Add(maingrid);
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
                Area4List.Items.Add(maingrid);
            }

            Area4List.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void Area5List_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (Area5List.Tag != null && (string)Area5List.Tag != "")
            {
                last_id = Area5List.Tag.ToString();
                Area5List.Items.RemoveAt(Area5List.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(47, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
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

                Area5List.Items.Add(maingrid);
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
                Area5List.Items.Add(maingrid);
            }

            Area5List.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void Area6List_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (Area6List.Tag != null && (string)Area6List.Tag != "")
            {
                last_id = Area6List.Tag.ToString();
                Area6List.Items.RemoveAt(Area6List.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(48, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
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

                Area6List.Items.Add(maingrid);
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
                Area6List.Items.Add(maingrid);
            }

            Area6List.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void Area7List_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (Area7List.Tag != null && (string)Area7List.Tag != "")
            {
                last_id = Area7List.Tag.ToString();
                Area7List.Items.RemoveAt(Area7List.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(39, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
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

                Area7List.Items.Add(maingrid);
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
                Area7List.Items.Add(maingrid);
            }

            Area7List.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void Area8List_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (Area8List.Tag != null && (string)Area8List.Tag != "")
            {
                last_id = Area8List.Tag.ToString();
                Area8List.Items.RemoveAt(Area8List.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(55, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
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

                Area8List.Items.Add(maingrid);
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
                Area8List.Items.Add(maingrid);
            }

            Area8List.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }
    }
}
