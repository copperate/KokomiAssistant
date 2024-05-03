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

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SocialPanelStarrail : Page
    {
        Frame rootFrame = Window.Current.Content as Frame;
        DateTime dt = new DateTime(1970, 1, 1);
        bool isReady = false;
        public SocialPanelStarrail()
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
                int pivotselected = StarrailPivot.SelectedIndex;
                switch (pivotselected)
                {
                    case 0: GetpostList_Recommend(null, e); break;
                    case 1: StarrailOfficalList_Loaded(null, e); break;
                    case 2: StarrailWaitingRoomList_Loaded(null, e); break;
                    case 3: StarrailWalkthroughList_Loaded(null, e); break;
                    case 4: StarrailFanPicList_Loaded(null, e); break;
                    case 5: StarrailCosplayList_Loaded(null, e); break;
                    default: break;
                }
            }
            else
                rootFrame.Navigate(typeof(PostDetailPanel), postID);
            //}
        }

        private void ListSortTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            int pivotselected = StarrailPivot.SelectedIndex;

            if (isReady)
            {
                ProgressStatus.IsIndeterminate = true;
                switch (pivotselected)
                {
                    case 0: break;
                    case 1: StarrailOfficalList.Items.Clear(); StarrailOfficalList.Tag = ""; StarrailOfficalList_Loaded(null, e); break;
                    case 2: StarrailWaitingRoomList.Items.Clear(); StarrailWaitingRoomList.Tag = ""; StarrailWaitingRoomList_Loaded(null, e); break;
                    case 3: StarrailWalkthroughList.Items.Clear(); StarrailWalkthroughList.Tag = ""; StarrailWalkthroughList_Loaded(null, e); break;
                    case 4: StarrailFanPicList.Items.Clear(); StarrailFanPicList.Tag = ""; StarrailFanPicList_Loaded(null, e); break;
                    case 5: StarrailCosplayList.Items.Clear(); StarrailCosplayList.Tag = ""; StarrailCosplayList_Loaded(null, e); break;
                    default:
                        break;
                }
                ProgressStatus.IsIndeterminate = false;
            }


        }

        private async void StarrailOfficalList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (StarrailOfficalList.Tag != null && (string)StarrailOfficalList.Tag != "")
            {
                last_id = StarrailOfficalList.Tag.ToString();
                StarrailOfficalList.Items.RemoveAt(StarrailOfficalList.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(53, ListSortTypeCombobox.SelectedIndex, last_id);
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

                StarrailOfficalList.Items.Add(maingrid);
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
                StarrailOfficalList.Items.Add(maingrid);
            }

            StarrailOfficalList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void StarrailWaitingRoomList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (StarrailWaitingRoomList.Tag != null && (string)StarrailWaitingRoomList.Tag != "")
            {
                last_id = StarrailWaitingRoomList.Tag.ToString();
                StarrailWaitingRoomList.Items.RemoveAt(StarrailWaitingRoomList.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(52, ListSortTypeCombobox.SelectedIndex, last_id);
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

                StarrailWaitingRoomList.Items.Add(maingrid);
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
                StarrailWaitingRoomList.Items.Add(maingrid);
            }

            StarrailWaitingRoomList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void StarrailWalkthroughList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (StarrailWalkthroughList.Tag != null && (string)StarrailWalkthroughList.Tag != "")
            {
                last_id = StarrailWalkthroughList.Tag.ToString();
                StarrailWalkthroughList.Items.RemoveAt(StarrailWalkthroughList.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(61, ListSortTypeCombobox.SelectedIndex, last_id);
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

                StarrailWalkthroughList.Items.Add(maingrid);
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
                StarrailWalkthroughList.Items.Add(maingrid);
            }

            StarrailWalkthroughList.Tag = data.data.last_id;

            ProgressStatus.IsIndeterminate = false;
        }

        private async void StarrailFanPicList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (StarrailFanPicList.Tag != null && (string)StarrailFanPicList.Tag != "")
            {
                last_id = StarrailFanPicList.Tag.ToString();
                StarrailFanPicList.Items.RemoveAt(StarrailFanPicList.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(56, ListSortTypeCombobox.SelectedIndex, last_id);
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

                StarrailFanPicList.Items.Add(maingrid);
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
                StarrailFanPicList.Items.Add(maingrid);
            }

            StarrailFanPicList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void StarrailCosplayList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (StarrailCosplayList.Tag != null && (string)StarrailCosplayList.Tag != "")
            {
                last_id = StarrailCosplayList.Tag.ToString();
                StarrailCosplayList.Items.RemoveAt(StarrailCosplayList.Items.Count - 1);
            }
            else last_id = "";
            RootObject data = await PostList.GetPostList(62, ListSortTypeCombobox.SelectedIndex, last_id);
            int listnum = data.data.list.Count;
            string blankstring = "                                                                         ";
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

                StarrailCosplayList.Items.Add(maingrid);
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
                StarrailCosplayList.Items.Add(maingrid);
            }

            StarrailCosplayList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void GetpostList_Recommend(FrameworkElement sender, object args)
        {
            ProgressStatus.IsIndeterminate = true;
            string last_id;
            if (StarrailDiscoverList.Tag != null && (string)StarrailDiscoverList.Tag != "") { 
                last_id = StarrailDiscoverList.Tag.ToString();
                StarrailDiscoverList.Items.RemoveAt(StarrailDiscoverList.Items.Count - 1);
            }
            else last_id = "";
            Uri uri = new Uri("https://api-takumi.miyoushe.com/post/api/feeds/posts?fresh_action=1&gids=6&last_id="+last_id);
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

                StarrailDiscoverList.Items.Add(maingrid);
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
                StarrailDiscoverList.Items.Add(maingrid);
            }

            StarrailDiscoverList.Tag = data.data.last_id;
            ProgressStatus.IsIndeterminate = false;
        }
        private void AppbarButton_Refresh_Click(object sender, RoutedEventArgs e)
        {
            int pivotselected = StarrailPivot.SelectedIndex;
            ProgressStatus.IsIndeterminate = true;
            switch (pivotselected)
            {
                case 0: StarrailDiscoverList.Items.Clear(); StarrailDiscoverList.Tag = ""; GetpostList_Recommend(null, e); break;
                case 1: StarrailOfficalList.Items.Clear(); StarrailOfficalList.Tag = ""; StarrailOfficalList_Loaded(null, e); break;
                case 2: StarrailWaitingRoomList.Items.Clear(); StarrailWaitingRoomList.Tag = ""; StarrailWaitingRoomList_Loaded(null, e); break;
                case 3: StarrailWalkthroughList.Items.Clear(); StarrailWalkthroughList.Tag = ""; StarrailWalkthroughList_Loaded(null, e); break;
                case 4: StarrailFanPicList.Items.Clear(); StarrailFanPicList.Tag = ""; StarrailFanPicList_Loaded(null, e); break;
                case 5: StarrailCosplayList.Items.Clear(); StarrailCosplayList.Tag = ""; StarrailCosplayList_Loaded(null, e); break;
                default: break;
            }
        }

        private void GotoSearchPanel(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SocialSearchPanel), null);
        }
    }
    
}
