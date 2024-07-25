using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
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
    public sealed partial class UserMsgPage : Page
    {
        Frame rootFrame = Window.Current.Content as Frame;
        bool isLogin = false;
        bool isReady = false;
        public UserMsgPage()
        {
            this.InitializeComponent();
            isReady = true;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["LoginUser"] != null && (int)localSettings.Values["LoginUser"] != 0)
            {
                ImageBrush image1 = new ImageBrush();
                image1.ImageSource = new BitmapImage(new Uri(localSettings.Values["LoginUserAvatar"].ToString()));
                DetailUserImage.Fill = image1;
                DetailUserNickname.Text = localSettings.Values["LoginUsername"].ToString();
                if (localSettings.Values["LoginUserPendant"].ToString() != null && localSettings.Values["LoginUserPendant"].ToString() != "") 
                    DetailPendantImage.Source= new BitmapImage(new Uri(localSettings.Values["LoginUserPendant"].ToString()));
                else DetailPendantImage.Visibility = Visibility.Collapsed;
                isLogin = true;

                GetUserNotificationsUnreadCount();
                GetUserNotifications();
            }
            
        }
        public async void GetUserNotificationsUnreadCount()
        {
            CountRoot countRoot = await GetNotifications.GetUnreadNotificationCount();
            if(countRoot.data.count.system != 0) 
            {
                NotificationsCountText.Text = "通知与推送 · " + countRoot.data.count.system.ToString();
            }
            if (countRoot.data.count.reply != 0)
            {
                ReplysCountText.Text = "评论与回复 · " + countRoot.data.count.reply.ToString();
            }
            if (countRoot.data.count.comment != 0 || countRoot.data.count.mention != 0)
            {
                int count = countRoot.data.count.comment + countRoot.data.count.mention;
                LikeMetionCountText.Text = "获赞与提及 · " + count;
            }
            countRoot = await GetNotifications.ClearUnreadNotifications();

        }
        public async void GetUserNotifications()
        {
            string last_id;
            if (SystemNotificationsView.Tag != null && SystemNotificationsView.Tag.ToString() != "") 
            {
                last_id = SystemNotificationsView.Tag.ToString();
                SystemNotificationsView.Items.RemoveAt(SystemNotificationsView.Items.Count - 1);
            }
            else last_id = "";
            NotificationsRoot data = await GetNotifications.GetNotification(last_id, "system");
            DateTime dt = new DateTime(1970, 1, 1);
           
            for (int i = 0; i < data.data.list.Count(); i++) 
            {
                Grid grid = new Grid();
                grid.CornerRadius = new CornerRadius(20);grid.Margin = new Thickness(0, 10, 0, 0);
                grid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 234, 234, 234));
                TextBlock AreaTimeBlock = new TextBlock();
                switch (data.data.list[i].game_id)
                {
                    case 1: AreaTimeBlock.Text = "崩坏3"; break;
                    case 2: AreaTimeBlock.Text = "原神"; break;
                    case 3: AreaTimeBlock.Text = "崩坏学园2"; break;
                    case 4: AreaTimeBlock.Text = "未定事件簿"; break;
                    case 5: AreaTimeBlock.Text = "大别野"; break;
                    case 6: AreaTimeBlock.Text = "崩坏:星穹铁道"; break;
                    case 8: AreaTimeBlock.Text = "绝区零"; break;
                    case 9981: AreaTimeBlock.Text = "Jabbr"; break;
                    default: AreaTimeBlock.Text = "推送"; break;
                }
                AreaTimeBlock.Text= AreaTimeBlock.Text+ " · "+ dt.AddSeconds(data.data.list[i].created_at).ToLocalTime().ToString();
                AreaTimeBlock.VerticalAlignment = VerticalAlignment.Top;AreaTimeBlock.HorizontalAlignment = HorizontalAlignment.Left;
                AreaTimeBlock.Margin = new Thickness(226, 10, 0, 0);AreaTimeBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 111, 111, 111));
                AreaTimeBlock.FontSize = 12;
                grid.Children.Add(AreaTimeBlock);

                TextBlock titleBlock = new TextBlock();
                titleBlock.Text = data.data.list[i].subject;
                titleBlock.VerticalAlignment= VerticalAlignment.Top;titleBlock.HorizontalAlignment = HorizontalAlignment.Left;titleBlock.Margin = new Thickness(226, 30, 10, 0);
                titleBlock.FontSize = 18;titleBlock.FontWeight = Windows.UI.Text.FontWeights.Bold;
                grid.Children.Add(titleBlock);

                TextBlock SummaryBlock = new TextBlock();
                SummaryBlock.Text = data.data.list[i].content;
                SummaryBlock.VerticalAlignment= VerticalAlignment.Top;SummaryBlock.HorizontalAlignment = HorizontalAlignment.Left;SummaryBlock.Margin = new Thickness(226, 59, 10, 0);
                SummaryBlock.FontSize = 16;
                grid.Children.Add(SummaryBlock);
                if (data.data.list[i].ext.image != null && data.data.list[i].ext.image != "") 
                {
                    Windows.UI.Xaml.Controls.Image NotificationImage = new Windows.UI.Xaml.Controls.Image();
                    NotificationImage.HorizontalAlignment= HorizontalAlignment.Left;NotificationImage.VerticalAlignment = VerticalAlignment.Top; NotificationImage.Margin = new Thickness(0, 0, 0, 1);
                    NotificationImage.Source= new BitmapImage(new Uri(data.data.list[i].ext.image));
                    NotificationImage.Height = 100; NotificationImage.Width = 216;
                    grid.Children.Add(NotificationImage);
                }
                

                grid.Tag = data.data.list[i].app_path;
                SystemNotificationsView.Items.Add(grid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 13, 0, 13); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 18;
                usernameText.Text = "载入过往通知……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                SystemNotificationsView.Items.Add(maingrid);
            }
            if (!data.data.is_last) SystemNotificationsView.Tag = data.data.last_id;
            else SystemNotificationsView.Tag = data.data.is_last;
        }

        public async void GetLikeMentionNotications()
        {
            string last_id;
            if (LikeMentionNotificationsView.Tag != null && LikeMentionNotificationsView.Tag.ToString() != "")
            {
                last_id = LikeMentionNotificationsView.Tag.ToString();
                LikeMentionNotificationsView.Items.RemoveAt(LikeMentionNotificationsView.Items.Count - 1);
            }
            else last_id = "";
            NotificationsRoot data = await GetNotifications.GetNotification(last_id, "comment");
            DateTime dt = new DateTime(1970, 1, 1);

            for (int i = 0; i < data.data.list.Count(); i++)
            {
                Grid grid = new Grid();
                grid.Margin = new Thickness(0, 10, 0, 0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 40; avatarellipse.Width = 40;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(5, 5, 0, 5);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].op_user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                grid.Children.Add(avatarellipse);

                TextBlock textBlock = new TextBlock();
                textBlock.VerticalAlignment = VerticalAlignment.Top; textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock.Margin = new Thickness(50, 4, 0, 21); textBlock.FontSize = 16;
                textBlock.Text = data.data.list[i].op_user.nickname;
                grid.Children.Add(textBlock);

                TextBlock textBlock1 = new TextBlock();
                textBlock1.VerticalAlignment = VerticalAlignment.Top; textBlock1.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock1.Margin = new Thickness(50, 27, 0, 0); textBlock1.FontSize = 12; textBlock1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128));
                if (data.data.list[i].type == 22)
                {
                    textBlock1.Text = dt.AddSeconds(data.data.list[i].created_at).ToLocalTime().ToString() + " · 给评论点了赞";
                }
                else textBlock1.Text = dt.AddSeconds(data.data.list[i].created_at).ToLocalTime().ToString() + " · 给帖子点了赞";
                grid.Children.Add(textBlock1);

                TextBlock textBlock2 = new TextBlock();
                textBlock2.VerticalAlignment = VerticalAlignment.Top; textBlock2.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock2.Margin = new Thickness(50, 50, 10, 10); textBlock2.FontSize = 16;
                textBlock2.Text = data.data.list[i].origin.text;
                grid.Children.Add(textBlock2);

                grid.Tag = data.data.list[i].app_path;
                LikeMentionNotificationsView.Items.Add(grid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(0, 0, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 16;
                usernameText.Text = "载入过往通知……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                LikeMentionNotificationsView.Items.Add(maingrid);
            }
            if (!data.data.is_last) LikeMentionNotificationsView.Tag = data.data.last_id;
            else LikeMentionNotificationsView.Tag = data.data.is_last;

        }

        public async void GetRepliesNotications()
        {
            string last_id;
            if (CommentNotificationsView.Tag != null && CommentNotificationsView.Tag.ToString() != "")
            {
                last_id = CommentNotificationsView.Tag.ToString();
                CommentNotificationsView.Items.RemoveAt(CommentNotificationsView.Items.Count - 1);
            }
            else last_id = "";
            NotificationsRoot data = await GetNotifications.GetNotification(last_id, "reply");
            DateTime dt = new DateTime(1970, 1, 1);

            for (int i = 0; i < data.data.list.Count(); i++)
            {
                Grid grid = new Grid();
                grid.Margin=new Thickness(0,10,0,0);

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 40; avatarellipse.Width = 40;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(5, 5, 0, 5);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(data.data.list[i].op_user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                grid.Children.Add(avatarellipse);

                TextBlock textBlock = new TextBlock();
                textBlock.VerticalAlignment = VerticalAlignment.Top;textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock.Margin = new Thickness(50, 4, 0, 21);textBlock.FontSize = 16;
                textBlock.Text = data.data.list[i].op_user.nickname;
                grid.Children.Add(textBlock);

                TextBlock textBlock1 = new TextBlock();
                textBlock1.VerticalAlignment = VerticalAlignment.Top; textBlock1.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock1.Margin = new Thickness(50, 27, 0, 0); textBlock1.FontSize = 12;textBlock1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128));
                if (data.data.list[i].type == 2)
                {
                    textBlock1.Text = dt.AddSeconds(data.data.list[i].created_at).ToLocalTime().ToString() + " · 回复了评论“" + data.data.list[i].origin.text + "”";
                }
                else textBlock1.Text = dt.AddSeconds(data.data.list[i].created_at).ToLocalTime().ToString() + " · 评论了帖子“" + data.data.list[i].origin.text + "”";
                grid.Children.Add(textBlock1);

                TextBlock textBlock2 = new TextBlock();
                textBlock2.VerticalAlignment = VerticalAlignment.Top;textBlock2.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock2.Margin = new Thickness(50, 50, 0, 0);textBlock2.FontSize = 16;
                textBlock2.Text = data.data.list[i].content;
                grid.Children.Add(textBlock2);

                grid.Tag = data.data.list[i].app_path;
                CommentNotificationsView.Items.Add(grid);
            }
            {
                Grid maingrid = new Grid();
                maingrid.Margin = new Thickness(0, 10, 10, 0); maingrid.Padding = new Thickness(0, 0, 0, 0); maingrid.BorderThickness = new Thickness(0, 0, 0, 0);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(0,0,0,0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 16;
                usernameText.Text = "载入过往通知……";
                maingrid.Children.Add(usernameText);
                maingrid.Tag = "loadmore";
                CommentNotificationsView.Items.Add(maingrid);
            }
            if (!data.data.is_last) CommentNotificationsView.Tag = data.data.last_id;
            else CommentNotificationsView.Tag = data.data.is_last;
        }

        private void NotificationGo(object sender, ItemClickEventArgs e)
        {
            string AppPath;
            dynamic clickedItem = e.ClickedItem;
            AppPath = (string)clickedItem.Tag;
            if (AppPath == "loadmore")
            {
                int channelID = NotificationSelectPane.SelectedIndex;
                switch(channelID)
                {
                    case 0: GetUserNotifications();break;
                    case 1: GetLikeMentionNotications();break; 
                    case 2: GetRepliesNotications();break;
                }
            }
            else
            {
                if (AppPath.Contains("mihoyobbs://"))
                {
                    if (AppPath.Contains("article/"))
                    {
                        AppPath = AppPath.Substring(AppPath.IndexOf("article/") + 8);
                        if (AppPath.Contains("?")) AppPath = AppPath.Substring(0, AppPath.IndexOf("?"));
                        rootFrame.Navigate(typeof(PostDetailPanel), AppPath);
                    }
                    if (AppPath.Contains("user/"))
                    {
                        AppPath = AppPath.Substring(AppPath.IndexOf("user/") + 5);
                        if (AppPath.Contains("?")) AppPath = AppPath.Substring(0, AppPath.IndexOf("?"));
                        rootFrame.Navigate(typeof(UserDetailPanel), AppPath);
                    }
                    if (AppPath.Contains("reply?"))
                    {
                        AppPath = AppPath.Substring(AppPath.IndexOf("reply?") + 6);
                        if (AppPath.Contains("&reply")) AppPath = AppPath.Substring(7, AppPath.IndexOf("reply") - 8);
                        rootFrame.Navigate(typeof(PostDetailPanel), AppPath);
                    }
                }
                else rootFrame.Navigate(typeof(ToolPanelInsideWebView), AppPath);

            }
        }

        private void GotoUserDetailPage(object sender, TappedRoutedEventArgs e)
        {
            if(isLogin)
            {
                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                rootFrame.Navigate(typeof(UserDetailPanel), localSettings.Values["LoginUserID"].ToString());
            }
        }

        private void DMSelectPane_Clicked(object sender, ItemClickEventArgs e)
        {
            NotificationSelectPane.SelectedIndex = -1;
            int channelID = DMSelectPane.SelectedIndex;
            dynamic clickedItem = e.ClickedItem;
            SystemView.Visibility = Visibility.Collapsed; LikeMentionView.Visibility = Visibility.Collapsed; ReplyView.Visibility = Visibility.Collapsed; 
            DMView.Visibility = Visibility.Visible;
        }   

        private void NotificationSelectPane_Clicked(object sender, ItemClickEventArgs e)
        {
            DMSelectPane.SelectedIndex = -1;
            dynamic clickedItem = e.ClickedItem;
            SystemView.Visibility = Visibility.Collapsed; LikeMentionView.Visibility = Visibility.Collapsed; ReplyView.Visibility = Visibility.Collapsed; DMView.Visibility = Visibility.Collapsed;
            switch (clickedItem.Tag)
            {
                case "system":
                    SystemView.Visibility = Visibility.Visible; 
                    SystemNotificationsView.Tag = ""; 
                    SystemNotificationsView.Items.Clear();
                    GetUserNotifications();
                    break;
                case "like":
                    LikeMentionView.Visibility = Visibility.Visible;
                    LikeMentionNotificationsView.Tag = "";
                    LikeMentionNotificationsView.Items.Clear();
                    GetLikeMentionNotications();
                    break; 
                case "reply":
                    ReplyView.Visibility = Visibility.Visible;
                    CommentNotificationsView.Tag = "";
                    CommentNotificationsView.Items.Clear();
                    GetRepliesNotications();
                    break;
            }
        }
    }
}
