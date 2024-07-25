using KokomiAssistant.PostViewMode;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
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
    
    public sealed partial class PostDetailPanel : Page
    {
        public static PostDetailPanel PostDetailPage;
        bool isReady = false;

        public PostDetailPanel()
        {
            this.InitializeComponent();
            PostDetailPage = this;
            isReady=true;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            {
                ProgressStatus.IsIndeterminate = true;
                int postid = int.Parse((string)e.Parameter);
                LoadTask(postid);
                ProgressStatus.IsIndeterminate = false;
                ProgressStatus.Visibility = Visibility.Collapsed;
            }
            else
            {
                
            }
            base.OnNavigatedTo(e);
        }
        public async void LoadTask(int postid)
        {
            var results = await GetPostDetails(postid);
        }
        public async Task<string> GetPostDetails(int i)
        {

            DetailRoot Data = await PostDetail.GetPostDetail(i);
            if (Data.retcode != 0)
            {
                PostTitle.Text = "帖子不存在。";
                DetailPanelSplit.OpenPaneLength = 0;
                PostContent.NavigateToString("<html><head></head><body><p>帖子无法访问，服务端未返回数据，可能已被删除或尚未通过审核。</p></body></html>");
                PostContent.Visibility = Visibility.Visible;
                PostContentView.Visibility = Visibility.Collapsed;
                ProgressStatus.Value = 100;
                return "";
            }
            PostTitle.Text = Data.data.post.post.subject;
            PostTitle.Tag = i;
            DetailUserImage.ImageSource = new BitmapImage(new Uri(Data.data.post.user.avatar_url));
            DetailUserNickname.Text = Data.data.post.user.nickname;
            String Gameid;int gamearea;
            try
            {
                gamearea = Data.data.post.post.game_id;
            }
            catch (System.NullReferenceException)
            {
                gamearea = 0;
                //throw;
            }

            switch (gamearea)
            {
                case 1: Gameid = "崩坏3"; break;
                case 2: Gameid = "原神"; break;
                case 3: Gameid = "崩坏学园2"; break;
                case 4: Gameid = "未定事件簿"; break;
                case 5: Gameid = "大别野"; break;
                case 6: Gameid = "崩坏:星穹铁道"; break;
                case 8: Gameid = "绝区零"; break;
                case 9981: Gameid = "Jabbr"; break;
                default:Gameid = "未知";break;

            }
            DetailUserLevel.Text = Gameid+" Lv."+Data.data.post.user.level_exp.level.ToString();
            DetailUserIntroduce.Text = Data.data.post.user.introduce;
            if (Data.data.post.forum == null) AreaText.Text = Gameid + "(无版区)";
            else AreaText.Text = Gameid + " - " + Data.data.post.forum.name;
           
            AreaText.Tag = gamearea;
            ViewNumText.Text="浏览量："+Data.data.post.stat.view_num.ToString();
            String DetailData = "<html><head></head><body>"+Data.data.post.post.content+"</body></html>";
            DetailPanelLikeContentButton.Label = "赞("+Data.data.post.stat.like_num.ToString()+")";
            DetailPanelBookmarkContentButton.Label ="收藏("+Data.data.post.stat.bookmark_num.ToString()+")";  
            DetailPanelShareContentButton.Label ="转发("+Data.data.post.stat.forward_num.ToString()+")";  
            CommentPivotItem.Header="评论("+Data.data.post.stat.reply_num.ToString()+")"; 
            UserDetailPane0.Tag=Data.data.post.user.uid.ToString();

            PostContent.Visibility = Visibility.Collapsed;
            PostContentView.Visibility = Visibility.Collapsed;
            switch (Data.data.post.post.view_type)
            {
                case 1:
                    {//PostContent.NavigateToString(DetailData);PostContent.Visibility = Visibility.Visible;
                        PostContentView.Navigate(typeof(PostViewMode.DocumentView), Data.data.post.post.structured_content);
                        PostContentView.Visibility = Visibility.Visible;
                    }
                    break;
                case 2:
                    {
                        PostContentView.Navigate(typeof(PostViewMode.MultiPicView), Data.data.post.post.content); PostContentView.Visibility = Visibility.Visible;
                    }
                    break;
                case 5:
                    {
                        PostTitle.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                        DetailPanelBG.Background = new SolidColorBrush(Windows.UI.Colors.Black);
                        PostContentView.Navigate(typeof(VideoView), Data.data.post.post.structured_content); PostContentView.Visibility = Visibility.Visible;
                    }
                    break;
                default: PostContent.NavigateToString(DetailData); break;
            }
            ProgressStatus.Value = 80;

            int topicsid = 0; String topicsText = "";
            try
            {
                while (topicsid >= 0)
                {
                    topicsText = topicsText + Data.data.post.topics[topicsid].name.ToString()+" ";
                    topicsid++;
                }
            }
            catch (System.ArgumentOutOfRangeException)
            {
                if (topicsid == 0) { topicsText = "未加入话题"; }
            }
            PostTagsText.Text = "话题：" + topicsText;
            double publish_time = Data.data.post.post.created_at;
            DateTime dt = new DateTime(1970, 1, 1);
            DateTime dt2 = dt.AddSeconds(publish_time).ToLocalTime();
            PubTimeText.Text = "发布/修改时间：" + dt2.ToShortDateString() + " " + dt2.ToLongTimeString();

            getComments(i, 2, false, "");

            
            return "";
        }

        private void NavigateUserDetail(object sender, TappedRoutedEventArgs e)
        {
            String userID = UserDetailPane0.Tag.ToString();
            Frame.Navigate(typeof(UserDetailPanel), userID);
        }

        private void DetailPanelControl(object sender, RoutedEventArgs e)
        {
            if (DetailPanelControlButton.Label == "收起侧栏")
            {
                DetailPanelControlButton.Icon = new SymbolIcon(Symbol.Back);
                DetailPanelControlButton.Label = "展开侧栏";
                DetailPanelSplit.OpenPaneLength = 0;
            }
            else
            {
                DetailPanelControlButton.Icon = new SymbolIcon(Symbol.Forward);
                DetailPanelControlButton.Label = "收起侧栏";
                DetailPanelSplit.OpenPaneLength = 400;

            }
        }
        public async void getComments(int postid,int sorttype,bool ismaster,string lastid)
        {
            CommentLoadProgressBar.IsIndeterminate = true;
            if (lastid == "") RepliesListView.Items.Clear();
            RepliesObjectRoot reply_data = await GetPostReplies.GetPostList(postid, sorttype, ismaster, lastid);
            if (reply_data.data == null) return;
            int reply_data_list_num = reply_data.data.list.Count();
            DateTime dt = new DateTime(1970, 1, 1);
            for (int index = 0; index < reply_data_list_num; index++)
            {
                Grid grid=new Grid();
                ListView listView = new ListView();
                listView.HorizontalContentAlignment = HorizontalAlignment.Left;listView.VerticalContentAlignment = VerticalAlignment.Top;
                listView.VerticalAlignment = VerticalAlignment.Top;listView.IsDoubleTapEnabled = false;listView.IsHoldingEnabled = false;listView.IsTapEnabled = false;
                listView.IsItemClickEnabled = false; listView.IsRightTapEnabled = false; listView.IsHitTestVisible = false; listView.IsMultiSelectCheckBoxEnabled = false;

                Grid statusGrid= new Grid();
                statusGrid.VerticalAlignment = VerticalAlignment.Top;statusGrid.Width = 355;

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 40; avatarellipse.Width = 40;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(0, 5, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(reply_data.data.list[index].user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                statusGrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 8, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 16;
                usernameText.Text = reply_data.data.list[index].user.nickname;
                statusGrid.Children.Add(usernameText);

                TextBlock pubtimeText= new TextBlock();
                pubtimeText.HorizontalAlignment = HorizontalAlignment.Left;pubtimeText.VerticalAlignment = VerticalAlignment.Top;
                pubtimeText.Margin = new Thickness(45, 30, 0, 0);
                pubtimeText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 113, 113, 113));
                pubtimeText.Text = "发布于 " + dt.AddSeconds(reply_data.data.list[index].reply.created_at).ToLocalTime().ToString();
                statusGrid.Children.Add(pubtimeText);

                TextBlock floorText= new TextBlock();
                floorText.HorizontalAlignment = HorizontalAlignment.Right;floorText.VerticalAlignment= VerticalAlignment.Top;
                floorText.Margin = new Thickness(0, 5, 5, 0);
                floorText.Text = reply_data.data.list[index].reply.floor_id + "F";
                statusGrid.Children.Add(floorText);

                if(reply_data.data.list[index].reply.reply_id == reply_data.data.pin_reply_id)
                {
                    Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                    BitmapImage bitmapImage2 = new BitmapImage(new Uri("ms-appx:///Assets/Content/settop.png"));
                    image.Source = bitmapImage2;
                    image.Width = 15;image.Height = 15;image.VerticalAlignment = VerticalAlignment.Bottom;image.HorizontalAlignment = HorizontalAlignment.Right;
                    image.Margin = new Thickness(0, 0, 6, 2);
                    statusGrid.Children.Add(image);
                }

                listView.Items.Add(statusGrid);

                string str = reply_data.data.list[index].reply.struct_content.ToString();
                JArray json = JArray.Parse(str);
                List<DocumentViewContent> Contentlist = json.ToObject<List<DocumentViewContent>>();
                InsertObject insertObject;
                bool isInsert;
                Run run = new Run();
                TextBlock textBlock = new TextBlock();
                for (int i = 0; i < Contentlist.Count; i++)
                {
                    try
                    {
                        insertObject = JObject.Parse(Contentlist[i].insert.ToString()).ToObject<InsertObject>();
                        isInsert = true;
                    }
                    catch (Exception)
                    {
                        insertObject = null;
                        isInsert = false;
                    }
                    if (isInsert)
                    {
                        textBlock.TextWrapping = TextWrapping.Wrap;
                        listView.Items.Add(textBlock);
                        textBlock = new TextBlock();
                        if (insertObject.image != null)
                        {
                            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                            BitmapImage bitmapImage2 = new BitmapImage(new Uri(insertObject.image));
                            //if (bitmapImage.PixelWidth <= 600) { image.Width = bitmapImage.PixelWidth; }
                            image.Source = bitmapImage2;
                            image.Width = 100;image.Height = 100;
                            image.Stretch = Stretch.UniformToFill;
                            image.Tag = bitmapImage;
                            listView.Items.Add(image);
                        }
                        if (insertObject.villa_card != null)
                        {
                            Button button = new Button();
                            Grid grid3 = new Grid();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 0, 108, 190));
                            button.Width = 310; button.Height = 100; button.Padding = new Thickness(0, 0, 0, 0);
                            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                            TextBlock block1 = new TextBlock();
                            TextBlock block2 = new TextBlock();
                            image.Source = new BitmapImage(new Uri(insertObject.villa_card.villa_cover));
                            image.Stretch = Stretch.UniformToFill;
                            image.Opacity = 0.5;
                            block1.Text = "别野"; block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Left;
                            block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); block1.FontSize = 18;
                            block2.Text = insertObject.villa_card.villa_name + "(" + insertObject.villa_card.villa_member_num + ")";
                            block2.VerticalAlignment = VerticalAlignment.Bottom; block2.HorizontalAlignment = HorizontalAlignment.Left;
                            block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); block2.FontSize = 18;
                            grid3.Children.Add(image);
                            grid3.Children.Add(block2);
                            grid3.Children.Add(block1);
                            button.Content = grid3;
                            button.Tag = insertObject.villa_card.villa_id;
                            listView.Items.Add(button);
                        }
                        if (insertObject.mention != null)
                        {
                            Button button = new Button();
                            Grid grid4 = new Grid();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 199, 217, 255));
                            //button.Width = double.NaN; button.Height = 50; button.MinWidth = 300; 
                            button.Padding = new Thickness(0, 0, 0, 0); button.CornerRadius = new CornerRadius(5, 5, 5, 5);
                            button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;

                            Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                            ImageBrush image1 = new ImageBrush();
                            UserDetailRoot Data = await GetUserDetail.GetUserDetailInfo(insertObject.mention.uid);
                            image1.ImageSource = new BitmapImage(new Uri(Data.data.user_info.avatar_url));
                            image1.Stretch = Stretch.UniformToFill;

                            ellipse.Width = 20; ellipse.Height = 20; ellipse.Fill = image1;
                            ellipse.Margin = new Thickness(0, 0, 0, 0); ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left;

                            TextBlock block1 = new TextBlock();
                            block1.Text = insertObject.mention.nickname;
                            block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Left;
                            block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 14;
                            block1.Margin = new Thickness(25, 0, 5, 0);

                            grid4.Children.Add(ellipse);
                            grid4.Children.Add(block1);
                            button.Content = grid4;
                            button.Tag = insertObject.mention.uid;
                            //button.Click += new RoutedEventHandler(UserButtonPressed);
                            listView.Items.Add(button);
                        }
                        if (insertObject.link_card != null)
                        {
                            Button button = new Button();
                            Grid grid5 = new Grid();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                            button.Width = double.NaN; button.Height = 90; ; button.Padding = new Thickness(0, 0, 0, 0);
                            button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                            Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                            ImageBrush image1 = new ImageBrush();
                            image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                            image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                            ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                            TextBlock block1 = new TextBlock();
                            TextBlock block2 = new TextBlock();
                            block1.Text = "[链接卡片]" + insertObject.link_card.title;
                            block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                            block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                            block1.Margin = new Thickness(45, 13, 10, 0);
                            block2.Text = insertObject.link_card.origin_url;
                            block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                            block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                            block2.Margin = new Thickness(10, 50, 10, 0);

                            grid5.Children.Add(ellipse);
                            grid5.Children.Add(block1);
                            grid5.Children.Add(block2);
                            button.Content = grid5;
                            button.Tag = insertObject.link_card.landing_url;
                            //button.Click += new RoutedEventHandler(LinkButtonPressed);
                            listView.Items.Add(button);
                        }
                        if (insertObject.villa_forward_card != null)
                        {
                            TextBlock textBlock1 = new TextBlock();
                            TextBlock textBlock2 = new TextBlock();
                            textBlock1.Text = "从 " + insertObject.villa_forward_card.room_name + "转发的聊天记录 [小心海助手暂不支持]";
                            textBlock1.Margin = new Thickness(50, 10, 10, 0);
                            textBlock1.FontSize = 16; textBlock1.VerticalAlignment = VerticalAlignment.Top;
                            textBlock2.Text = "来自别野 " + insertObject.villa_forward_card.villa_name;
                            textBlock2.Margin = new Thickness(50, 35, 10, 10);
                            textBlock2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 127, 127, 127));
                            textBlock2.VerticalAlignment = VerticalAlignment.Top; textBlock2.HorizontalAlignment = HorizontalAlignment.Left;
                            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                            image.Source = new BitmapImage(new Uri(insertObject.villa_forward_card.villa_avatar_url));
                            image.Width = 40; image.Height = 40; image.HorizontalAlignment = HorizontalAlignment.Left;
                            image.VerticalAlignment = VerticalAlignment.Center;

                            Grid grid6 = new Grid();
                            grid6.Children.Add(textBlock1);
                            grid6.Children.Add(textBlock2);
                            grid6.Children.Add(image);
                            Button button = new Button();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(52, 255, 108, 196));
                            button.HorizontalAlignment = HorizontalAlignment.Left; button.VerticalAlignment = VerticalAlignment.Top;
                            button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                            button.Content = grid6;
                            button.Tag = insertObject.villa_forward_card;
                            listView.Items.Add(button);
                        }
                        if (insertObject.villa_avatar_action != null) 
                        {
                            Grid grid6 = new Grid();

                            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                            BitmapImage bitmapImage2 = new BitmapImage(new Uri(insertObject.villa_avatar_action.url));
                            image.Source = bitmapImage2;
                            image.Width = 200; image.Height = 200;
                            image.Stretch = Stretch.UniformToFill;
                            grid6.Children.Add(image);

                            TextBlock actionText = new TextBlock();
                            actionText.HorizontalAlignment = HorizontalAlignment.Left; actionText.VerticalAlignment = VerticalAlignment.Top;
                            actionText.Margin = new Thickness(10, 210, 0, 0);
                            actionText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 113, 113, 113));
                            actionText.Text = "发起了 " + insertObject.villa_avatar_action.action_name;
                            grid6.Children.Add(actionText);

                            listView.Items.Add(grid6);
                        }
                    }
                    else
                    {
                        run.Text = Contentlist[i].insert.ToString();
                        run.FontSize = 16;
                        if (Contentlist[i].attributes != null)
                        {
                            if (Contentlist[i].attributes.bold != null) { run.FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                            if (Contentlist[i].attributes.italic != null) { run.FontStyle = Windows.UI.Text.FontStyle.Italic; }
                            if (!string.IsNullOrEmpty(Contentlist[i].attributes.color) && Contentlist[i].attributes.color.Substring(0, 1) == "#")
                            {
                                string colorcodeR = Contentlist[i].attributes.color.Substring(1, 2);
                                string colorcodeG = Contentlist[i].attributes.color.Substring(3, 2);
                                string colorcodeB = Contentlist[i].attributes.color.Substring(5, 2);
                                run.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)System.Convert.ToInt32("0x" + colorcodeR, 16), (byte)System.Convert.ToInt32("0x" + colorcodeG, 16), (byte)System.Convert.ToInt32("0x" + colorcodeB, 16)));
                            }
                            if (!string.IsNullOrEmpty(Contentlist[i].attributes.link))
                            {
                                Button button = new Button();
                                Grid grid7 = new Grid();
                                button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                                button.Width = double.NaN; button.Height = 90; ; button.Padding = new Thickness(0, 0, 0, 0);
                                button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                                Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                                ImageBrush image1 = new ImageBrush();
                                image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                                image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                                ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                                TextBlock block1 = new TextBlock();
                                TextBlock block2 = new TextBlock();
                                block1.Text = "[链接]" + Contentlist[i].insert.ToString();
                                block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                                block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                                block1.Margin = new Thickness(45, 13, 10, 0);
                                block2.Text = Contentlist[i].attributes.link;
                                block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                                block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                                block2.Margin = new Thickness(10, 50, 10, 0);

                                grid7.Children.Add(ellipse);
                                grid7.Children.Add(block1);
                                grid7.Children.Add(block2);
                                button.Content = grid7;
                                button.Tag = Contentlist[i].attributes.link;
                                //button.Click += new RoutedEventHandler(LinkButtonPressed);
                                listView.Items.Add(button);
                            }
                        }
                        textBlock.Inlines.Add(run);
                        run = new Run();
                    }
                    if (i + 1 == Contentlist.Count)
                    {
                        textBlock.TextWrapping = TextWrapping.Wrap;
                        textBlock.MaxHeight = 290;
                        listView.Items.Add(textBlock);
                    }
                }

                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = reply_data.data.list[index].stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                Grid commentnumGrid = new Grid();
                commentnumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image commentnumIcon = new Windows.UI.Xaml.Controls.Image();
                commentnumIcon.Width = 20; commentnumIcon.Height = 20; commentnumIcon.HorizontalAlignment = HorizontalAlignment.Left; commentnumIcon.VerticalAlignment = VerticalAlignment.Top;
                commentnumIcon.Stretch = Stretch.Fill; commentnumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/comment.png"));
                TextBlock commentnumText = new TextBlock();
                commentnumText.HorizontalAlignment = HorizontalAlignment.Left; commentnumText.VerticalAlignment = VerticalAlignment.Top;
                commentnumText.Margin = new Thickness(25, 0, 0, 0); commentnumText.Text = reply_data.data.list[index].sub_reply_count.ToString();
                commentnumGrid.Children.Add(commentnumIcon); commentnumGrid.Children.Add(commentnumText);

                poststatusGV.Items.Add(likenumGrid); poststatusGV.Items.Add(commentnumGrid);
                listView.Items.Add(poststatusGV);
                grid.Children.Add(listView);

                grid.Tag = reply_data.data.list[index].reply.reply_id;
                RepliesListView.Items.Add(grid);
                
            }
            Grid grid_lm= new Grid();
            grid_lm.VerticalAlignment = VerticalAlignment.Top; grid_lm.Width = 320;
            Windows.UI.Xaml.Controls.Image loadmoreIcon = new Windows.UI.Xaml.Controls.Image();
            loadmoreIcon.Height = 20; loadmoreIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/loadmore.png"));
            grid_lm.Children.Add(loadmoreIcon);
            grid_lm.Tag = "loadmore";
            RepliesListView.Items.Add(grid_lm);

            RepliesListView.Tag = reply_data.data.last_id;
            CommentLoadProgressBar.IsIndeterminate = false;

        }

        private void DetailPageImageView(object sender, TappedRoutedEventArgs e)
        {
            Windows.UI.Xaml.Controls.Image imagea = sender as Windows.UI.Xaml.Controls.Image;
            PanelPicView.Source = imagea.Tag as BitmapImage;
            PanelPicViewGrid.Visibility = Visibility.Visible;
        }
        private void MentionButtonNavigateUser(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
            String userID = button.Tag.ToString();
            Frame.Navigate(typeof(UserDetailPanel), userID);
        }
        private void DetailPanelSharePostButton(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

         void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            string gamearea;
            switch (AreaText.Tag)
            {
                case 1: gamearea = "bh3"; break;
                case 2: gamearea = "ys"; break;
                case 3: gamearea = "bh2"; break;
                case 4: gamearea = "wd"; break;
                case 5: gamearea = "dby"; break;
                case 6: gamearea = "sr"; break;
                case 8: gamearea = "zzz"; break;
                default: gamearea = "dby"; break;
            }
            string shareLink = "https://www.miyoushe.com/"+gamearea+"/article/"+PostTitle.Tag;　　
            DataPackage dataPackage = new DataPackage();  
            dataPackage.SetWebLink(new Uri(shareLink));
            dataPackage.Properties.Title = PostTitle.Text + "　——分享自「小心海助手」";
            dataPackage.Properties.Description = shareLink;
            DataRequest request = args.Request;
            request.Data = dataPackage;
        }

        private void CommentSortChange(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            int selecteditem = comboBox.SelectedIndex;
            if(isReady)
            {
                if (selecteditem == 0) { selecteditem = 0; }
                getComments((int)PostTitle.Tag, selecteditem, false, "");
            }
            
        }

        private void PanelPicViewGrid_Click(object sender, RoutedEventArgs e)
        {
            PanelPicViewGrid.Visibility = Visibility.Collapsed;
        }

        private void PanelPicViewGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PanelPicViewGrid.Visibility = Visibility.Collapsed;
        }

        private void NotifyPanel_ButtonClick(object sender, RoutedEventArgs e)
        {
            NotifyPane.Visibility = Visibility.Collapsed;
        }
        public async void NotifyPane_Activated(string message)
        {
            NotifyPane.Visibility = Visibility.Visible;
            NotifyDetail.Text = message;
            var result = await PaneClose();
            NotifyPane.Visibility = Visibility.Collapsed;
        }
        public async Task<string> PaneClose()
        {
            return await Task.Run(() => {
                Thread.Sleep(5000); return "";
            });
        }

        private void PanelPicViewGrid_DownloadButton_Clicked(object sender, RoutedEventArgs e)
        {
            NotifyPane_Activated("尝试下载中……");
            try
            {
                SavePicToDownload();
                NotifyPane_Activated("下载成功，已保存到/Download文件夹。");
                
            }
            catch
            {
                NotifyPane_Activated("下载失败，请重试。");
            }

        }

        public async void SavePicToDownload()
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFolder folder = await storageFolder.CreateFolderAsync("Download", CreationCollisionOption.OpenIfExists);
            string filename = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff") + ".png";
            Windows.Storage.StorageFile savedPic = await folder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            RenderTargetBitmap bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(PanelPicView);
            var pixelBuffer = await bitmap.GetPixelsAsync();
            using (var fileStream = await savedPic.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                    BitmapAlphaMode.Ignore,
                                    (uint)bitmap.PixelWidth,
                                    (uint)bitmap.PixelHeight,
                                    DisplayInformation.GetForCurrentView().LogicalDpi,
                                    DisplayInformation.GetForCurrentView().LogicalDpi,
                                    pixelBuffer.ToArray());
                await encoder.FlushAsync();
            }
        }

        private void ShowCommentDetail(object sender, ItemClickEventArgs e)
        {
            dynamic clickedItem = e.ClickedItem;
            string replyID = (string)clickedItem.Tag;
            if (replyID == "loadmore")
            {
                RepliesListView.Items.RemoveAt(RepliesListView.Items.Count - 1);
                getComments((int)PostTitle.Tag, CommentSortCombo.SelectedIndex, false, RepliesListView.Tag.ToString());
            }
            else
            {
                CommentDetailPage.Visibility = Visibility.Visible;
                LoadSubRepliesPage(replyID);
            }
        }

        private void CloseCommentDetailPage(object sender, RoutedEventArgs e)
        {
            CommentDetailPage.Visibility = Visibility.Collapsed;
        }

        public async void LoadSubRepliesPage(string replyID)
        {
            SubReplyProgress.IsIndeterminate = true;
            SubRepliesListView.Items.Clear();
            RootReplyData rootReplyData = await getRootReplyInfoAsync(replyID);
            RepliesObjectRoot SubRepliesList = await getSubRepliesAsync(rootReplyData.data.reply.reply.floor_id.ToString());

            DateTime dt = new DateTime(1970, 1, 1);
            RootReplyFloor.Text = rootReplyData.data.reply.reply.floor_id + "F";
            {
                Grid grid = new Grid();
                ListView listView = new ListView();
                listView.HorizontalContentAlignment = HorizontalAlignment.Left; listView.VerticalContentAlignment = VerticalAlignment.Top;
                listView.VerticalAlignment = VerticalAlignment.Top; listView.IsDoubleTapEnabled = false; listView.IsTapEnabled = false;
                //listView.IsItemClickEnabled = false; 
                listView.IsRightTapEnabled = false; listView.IsMultiSelectCheckBoxEnabled = false;listView.SelectionMode = ListViewSelectionMode.None;
                //用户头像、名称、发布时间、楼层数
                Grid statusGrid = new Grid();
                statusGrid.VerticalAlignment = VerticalAlignment.Top; statusGrid.Width = 305;

                Ellipse avatarellipse = new Ellipse();
                avatarellipse.Height = 40; avatarellipse.Width = 40;
                avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                avatarellipse.Margin = new Thickness(0, 5, 0, 0);
                ImageBrush bitmapImage = new ImageBrush();
                bitmapImage.ImageSource = new BitmapImage(new Uri(rootReplyData.data.reply.user.avatar_url));
                bitmapImage.Stretch = Stretch.UniformToFill;
                avatarellipse.Fill = bitmapImage;
                statusGrid.Children.Add(avatarellipse);

                TextBlock usernameText = new TextBlock();
                usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                usernameText.Margin = new Thickness(45, 8, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                usernameText.FontSize = 16;
                usernameText.Text = rootReplyData.data.reply.user.nickname;
                statusGrid.Children.Add(usernameText);

                TextBlock pubtimeText = new TextBlock();
                pubtimeText.HorizontalAlignment = HorizontalAlignment.Left; pubtimeText.VerticalAlignment = VerticalAlignment.Top;
                pubtimeText.Margin = new Thickness(45, 30, 0, 0);
                pubtimeText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 113, 113, 113));
                pubtimeText.Text = "发布于 " + dt.AddSeconds(rootReplyData.data.reply.reply.created_at).ToLocalTime().ToString();
                statusGrid.Children.Add(pubtimeText);


                listView.Items.Add(statusGrid);
                //评论主体
                string str = rootReplyData.data.reply.reply.struct_content.ToString();
                JArray json = JArray.Parse(str);
                List<DocumentViewContent> Contentlist = json.ToObject<List<DocumentViewContent>>();
                InsertObject insertObject;
                bool isInsert;
                Run run = new Run();

                RichTextBlock MainCommentDetailTextBlock = new RichTextBlock();
                Paragraph paragraph = new Paragraph();
                List<InlineUIContainer> CommentFloorInlineUIContainers = new List<InlineUIContainer>();
                for (int i = 0; i < Contentlist.Count; i++)
                {
                    try
                    {
                        insertObject = JObject.Parse(Contentlist[i].insert.ToString()).ToObject<InsertObject>();
                        isInsert = true;
                    }
                    catch (Exception)
                    {
                        insertObject = null;
                        isInsert = false;
                    }
                    if (isInsert)
                    {
                        if (insertObject.image != null)
                        {
                            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                            BitmapImage bitmapImage2 = new BitmapImage(new Uri(insertObject.image));
                            image.Source = bitmapImage2;
                            image.Stretch = Stretch.UniformToFill;
                            image.Tag = bitmapImage2;
                            image.Tapped += new TappedEventHandler(DetailPageImageView);
                            CommentFloorInlineUIContainers.Add(new InlineUIContainer());
                            CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count-1].Child = image;
                            paragraph.Inlines.Add(CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1]);
                        }
                        if (insertObject.villa_card != null)
                        {
                            Button button = new Button();
                            Grid grid3 = new Grid();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 0, 108, 190));
                            button.Width = 310; button.Height = 100; button.Padding = new Thickness(0, 0, 0, 0);
                            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                            TextBlock block1 = new TextBlock();
                            TextBlock block2 = new TextBlock();
                            image.Source = new BitmapImage(new Uri(insertObject.villa_card.villa_cover));
                            image.Stretch = Stretch.UniformToFill;
                            image.Opacity = 0.5;
                            block1.Text = "别野"; block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Left;
                            block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); block1.FontSize = 18;
                            block2.Text = insertObject.villa_card.villa_name + "(" + insertObject.villa_card.villa_member_num + ")";
                            block2.VerticalAlignment = VerticalAlignment.Bottom; block2.HorizontalAlignment = HorizontalAlignment.Left;
                            block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); block2.FontSize = 18;
                            grid3.Children.Add(image);
                            grid3.Children.Add(block2);
                            grid3.Children.Add(block1);
                            button.Content = grid3;
                            button.Tag = insertObject.villa_card.villa_id;

                            CommentFloorInlineUIContainers.Add(new InlineUIContainer());
                            CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1].Child = button;
                            paragraph.Inlines.Add(CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1]);
                        }
                        if (insertObject.mention != null)
                        {
                            Button button = new Button();
                            Grid grid4 = new Grid();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 199, 217, 255));
                            //button.Width = double.NaN; button.Height = 50; button.MinWidth = 300; 
                            button.Padding = new Thickness(0, 0, 0, 0); button.CornerRadius = new CornerRadius(5, 5, 5, 5);
                            button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;

                            Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                            ImageBrush image1 = new ImageBrush();
                            UserDetailRoot Data = await GetUserDetail.GetUserDetailInfo(insertObject.mention.uid);
                            image1.ImageSource = new BitmapImage(new Uri(Data.data.user_info.avatar_url));
                            image1.Stretch = Stretch.UniformToFill;

                            ellipse.Width = 20; ellipse.Height = 20; ellipse.Fill = image1;
                            ellipse.Margin = new Thickness(0, 0, 0, 0); ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left;

                            TextBlock block1 = new TextBlock();
                            block1.Text = insertObject.mention.nickname;
                            block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Left;
                            block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 14;
                            block1.Margin = new Thickness(25, 0, 5, 0);

                            grid4.Children.Add(ellipse);
                            grid4.Children.Add(block1);
                            button.Content = grid4;
                            button.Tag = insertObject.mention.uid;
                            button.Tapped += new TappedEventHandler(MentionButtonNavigateUser);

                            CommentFloorInlineUIContainers.Add(new InlineUIContainer());
                            CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1].Child = button;
                            paragraph.Inlines.Add(CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1]);
                        }
                        if (insertObject.link_card != null)
                        {
                            Button button = new Button();
                            Grid grid5 = new Grid();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                            button.Width = double.NaN; button.Height = 90; ; button.Padding = new Thickness(0, 0, 0, 0);
                            button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                            Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                            ImageBrush image1 = new ImageBrush();
                            image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                            image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                            ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                            TextBlock block1 = new TextBlock();
                            TextBlock block2 = new TextBlock();
                            block1.Text = "[链接卡片]" + insertObject.link_card.title;
                            block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                            block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                            block1.Margin = new Thickness(45, 13, 10, 0);
                            block2.Text = insertObject.link_card.origin_url;
                            block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                            block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                            block2.Margin = new Thickness(10, 50, 10, 0);

                            grid5.Children.Add(ellipse);
                            grid5.Children.Add(block1);
                            grid5.Children.Add(block2);
                            button.Content = grid5;
                            button.Tag = insertObject.link_card.landing_url;

                            CommentFloorInlineUIContainers.Add(new InlineUIContainer());
                            CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1].Child = button;
                            paragraph.Inlines.Add(CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1]);
                        }
                        if (insertObject.villa_forward_card != null)
                        {
                            TextBlock textBlock1 = new TextBlock();
                            TextBlock textBlock2 = new TextBlock();
                            textBlock1.Text = "从 " + insertObject.villa_forward_card.room_name + "转发的聊天记录 [小心海助手暂不支持]";
                            textBlock1.Margin = new Thickness(50, 10, 10, 0);
                            textBlock1.FontSize = 16; textBlock1.VerticalAlignment = VerticalAlignment.Top;
                            textBlock2.Text = "来自别野 " + insertObject.villa_forward_card.villa_name;
                            textBlock2.Margin = new Thickness(50, 35, 10, 10);
                            textBlock2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 127, 127, 127));
                            textBlock2.VerticalAlignment = VerticalAlignment.Top; textBlock2.HorizontalAlignment = HorizontalAlignment.Left;
                            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                            image.Source = new BitmapImage(new Uri(insertObject.villa_forward_card.villa_avatar_url));
                            image.Width = 40; image.Height = 40; image.HorizontalAlignment = HorizontalAlignment.Left;
                            image.VerticalAlignment = VerticalAlignment.Center;

                            Grid grid6 = new Grid();
                            grid6.Children.Add(textBlock1);
                            grid6.Children.Add(textBlock2);
                            grid6.Children.Add(image);
                            Button button = new Button();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(52, 255, 108, 196));
                            button.HorizontalAlignment = HorizontalAlignment.Left; button.VerticalAlignment = VerticalAlignment.Top;
                            button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                            button.Content = grid6;
                            button.Tag = insertObject.villa_forward_card;

                            CommentFloorInlineUIContainers.Add(new InlineUIContainer());
                            CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1].Child = button;
                            paragraph.Inlines.Add(CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1]);
                        }
                        if (insertObject.villa_avatar_action != null)
                        {
                            Grid grid6 = new Grid();

                            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                            BitmapImage bitmapImage2 = new BitmapImage(new Uri(insertObject.villa_avatar_action.url));
                            image.Source = bitmapImage2;
                            image.Width = 200; image.Height = 200;
                            image.Stretch = Stretch.UniformToFill;
                            grid6.Children.Add(image);

                            TextBlock actionText = new TextBlock();
                            actionText.HorizontalAlignment = HorizontalAlignment.Left; actionText.VerticalAlignment = VerticalAlignment.Top;
                            actionText.Margin = new Thickness(10, 210, 0, 0);
                            actionText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 113, 113, 113));
                            actionText.Text = "发起了 " + insertObject.villa_avatar_action.action_name;
                            grid6.Children.Add(actionText);

                            CommentFloorInlineUIContainers.Add(new InlineUIContainer());
                            CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1].Child = grid6;
                            paragraph.Inlines.Add(CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1]);
                        }
                    }
                    else
                    {
                        run.Text = Contentlist[i].insert.ToString();
                        run.FontSize = 16;
                        if (Contentlist[i].attributes != null)
                        {
                            if (Contentlist[i].attributes.bold != null) { run.FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                            if (Contentlist[i].attributes.italic != null) { run.FontStyle = Windows.UI.Text.FontStyle.Italic; }
                            if (!string.IsNullOrEmpty(Contentlist[i].attributes.color) && Contentlist[i].attributes.color.Substring(0, 1) == "#")
                            {
                                string colorcodeR = Contentlist[i].attributes.color.Substring(1, 2);
                                string colorcodeG = Contentlist[i].attributes.color.Substring(3, 2);
                                string colorcodeB = Contentlist[i].attributes.color.Substring(5, 2);
                                run.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)System.Convert.ToInt32("0x" + colorcodeR, 16), (byte)System.Convert.ToInt32("0x" + colorcodeG, 16), (byte)System.Convert.ToInt32("0x" + colorcodeB, 16)));
                            }
                            if (!string.IsNullOrEmpty(Contentlist[i].attributes.link))
                            {
                                Button button = new Button();
                                Grid grid7 = new Grid();
                                button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                                button.Width = double.NaN; button.Height = 90; ; button.Padding = new Thickness(0, 0, 0, 0);
                                button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                                Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                                ImageBrush image1 = new ImageBrush();
                                image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                                image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                                ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                                TextBlock block1 = new TextBlock();
                                TextBlock block2 = new TextBlock();
                                block1.Text = "[链接]" + Contentlist[i].insert.ToString();
                                block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                                block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                                block1.Margin = new Thickness(45, 13, 10, 0);
                                block2.Text = Contentlist[i].attributes.link;
                                block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                                block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                                block2.Margin = new Thickness(10, 50, 10, 0);

                                grid7.Children.Add(ellipse);
                                grid7.Children.Add(block1);
                                grid7.Children.Add(block2);
                                button.Content = grid7;
                                button.Tag = Contentlist[i].attributes.link;

                                CommentFloorInlineUIContainers.Add(new InlineUIContainer());
                                CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1].Child = button;
                                paragraph.Inlines.Add(CommentFloorInlineUIContainers[CommentFloorInlineUIContainers.Count - 1]);
                            }
                        }
                        paragraph.Inlines.Add(run);
                        run = new Run();
                    }
                    if (i + 1 == Contentlist.Count)
                    {
                        MainCommentDetailTextBlock.Blocks.Add(paragraph);
                        listView.Items.Add(MainCommentDetailTextBlock);
                    }
                }
                /* 原显示评论主体代码
                  string str = rootReplyData.data.reply.reply.struct_content.ToString();
                JArray json = JArray.Parse(str);
                List<DocumentViewContent> Contentlist = json.ToObject<List<DocumentViewContent>>();
                InsertObject insertObject;
                bool isInsert;
                Run run = new Run();
                TextBlock textBlock = new TextBlock();
                for (int i = 0; i < Contentlist.Count; i++)
                {
                    try
                    {
                        insertObject = JObject.Parse(Contentlist[i].insert.ToString()).ToObject<InsertObject>();
                        isInsert = true;
                    }
                    catch (Exception)
                    {
                        insertObject = null;
                        isInsert = false;
                    }
                    if (isInsert)
                    {
                        textBlock.TextWrapping = TextWrapping.Wrap;
                        listView.Items.Add(textBlock);
                        textBlock = new TextBlock();
                        if (insertObject.image != null)
                        {
                            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                            BitmapImage bitmapImage2 = new BitmapImage(new Uri(insertObject.image));
                            //if (bitmapImage.PixelWidth <= 600) { image.Width = bitmapImage.PixelWidth; }
                            image.Source = bitmapImage2;
                            image.Stretch = Stretch.UniformToFill;
                            image.Tag = bitmapImage2;
                            image.Tapped += new TappedEventHandler(DetailPageImageView);
                            listView.Items.Add(image);
                        }
                        if (insertObject.villa_card != null)
                        {
                            Button button = new Button();
                            Grid grid3 = new Grid();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 0, 108, 190));
                            button.Width = 310; button.Height = 100; button.Padding = new Thickness(0, 0, 0, 0);
                            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                            TextBlock block1 = new TextBlock();
                            TextBlock block2 = new TextBlock();
                            image.Source = new BitmapImage(new Uri(insertObject.villa_card.villa_cover));
                            image.Stretch = Stretch.UniformToFill;
                            image.Opacity = 0.5;
                            block1.Text = "别野"; block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Left;
                            block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); block1.FontSize = 18;
                            block2.Text = insertObject.villa_card.villa_name + "(" + insertObject.villa_card.villa_member_num + ")";
                            block2.VerticalAlignment = VerticalAlignment.Bottom; block2.HorizontalAlignment = HorizontalAlignment.Left;
                            block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); block2.FontSize = 18;
                            grid3.Children.Add(image);
                            grid3.Children.Add(block2);
                            grid3.Children.Add(block1);
                            button.Content = grid3;
                            button.Tag = insertObject.villa_card.villa_id;
                            listView.Items.Add(button);
                        }
                        if (insertObject.mention != null)
                        {
                            Button button = new Button();
                            Grid grid4 = new Grid();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 199, 217, 255));
                            button.Width = double.NaN; button.Height = 50; button.MinWidth = 300; button.Padding = new Thickness(0, 0, 0, 0);
                            button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;

                            Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                            ImageBrush image1 = new ImageBrush();
                            UserDetailRoot Data = await GetUserDetail.GetUserDetailInfo(insertObject.mention.uid);
                            image1.ImageSource = new BitmapImage(new Uri(Data.data.user_info.avatar_url));
                            image1.Stretch = Stretch.UniformToFill;

                            ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1;
                            ellipse.Margin = new Thickness(10, 10, 0, 0); ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left;

                            TextBlock block1 = new TextBlock();
                            block1.Text = "@ " + insertObject.mention.nickname;
                            block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Left;
                            block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                            block1.Margin = new Thickness(45, 13, 10, 0);

                            grid4.Children.Add(ellipse);
                            grid4.Children.Add(block1);
                            button.Content = grid4;
                            button.Tag = insertObject.mention.uid;
                            //button.Click += new RoutedEventHandler(UserButtonPressed);
                            listView.Items.Add(button);
                        }
                        if (insertObject.link_card != null)
                        {
                            Button button = new Button();
                            Grid grid5 = new Grid();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                            button.Width = double.NaN; button.Height = 90; ; button.Padding = new Thickness(0, 0, 0, 0);
                            button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                            Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                            ImageBrush image1 = new ImageBrush();
                            image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                            image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                            ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                            TextBlock block1 = new TextBlock();
                            TextBlock block2 = new TextBlock();
                            block1.Text = "[链接卡片]" + insertObject.link_card.title;
                            block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                            block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                            block1.Margin = new Thickness(45, 13, 10, 0);
                            block2.Text = insertObject.link_card.origin_url;
                            block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                            block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                            block2.Margin = new Thickness(10, 50, 10, 0);

                            grid5.Children.Add(ellipse);
                            grid5.Children.Add(block1);
                            grid5.Children.Add(block2);
                            button.Content = grid5;
                            button.Tag = insertObject.link_card.landing_url;
                            //button.Click += new RoutedEventHandler(LinkButtonPressed);
                            listView.Items.Add(button);
                        }
                        if (insertObject.villa_forward_card != null)
                        {
                            TextBlock textBlock1 = new TextBlock();
                            TextBlock textBlock2 = new TextBlock();
                            textBlock1.Text = "从 " + insertObject.villa_forward_card.room_name + "转发的聊天记录 [小心海助手暂不支持]";
                            textBlock1.Margin = new Thickness(50, 10, 10, 0);
                            textBlock1.FontSize = 16; textBlock1.VerticalAlignment = VerticalAlignment.Top;
                            textBlock2.Text = "来自别野 " + insertObject.villa_forward_card.villa_name;
                            textBlock2.Margin = new Thickness(50, 35, 10, 10);
                            textBlock2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 127, 127, 127));
                            textBlock2.VerticalAlignment = VerticalAlignment.Top; textBlock2.HorizontalAlignment = HorizontalAlignment.Left;
                            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                            image.Source = new BitmapImage(new Uri(insertObject.villa_forward_card.villa_avatar_url));
                            image.Width = 40; image.Height = 40; image.HorizontalAlignment = HorizontalAlignment.Left;
                            image.VerticalAlignment = VerticalAlignment.Center;

                            Grid grid6 = new Grid();
                            grid6.Children.Add(textBlock1);
                            grid6.Children.Add(textBlock2);
                            grid6.Children.Add(image);
                            Button button = new Button();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(52, 255, 108, 196));
                            button.HorizontalAlignment = HorizontalAlignment.Left; button.VerticalAlignment = VerticalAlignment.Top;
                            button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                            button.Content = grid6;
                            button.Tag = insertObject.villa_forward_card;
                            listView.Items.Add(button);
                        }
                        if (insertObject.villa_avatar_action != null)
                        {
                            Grid grid6 = new Grid();

                            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                            BitmapImage bitmapImage2 = new BitmapImage(new Uri(insertObject.villa_avatar_action.url));
                            image.Source = bitmapImage2;
                            image.Width = 200; image.Height = 200;
                            image.Stretch = Stretch.UniformToFill;
                            grid6.Children.Add(image);

                            TextBlock actionText = new TextBlock();
                            actionText.HorizontalAlignment = HorizontalAlignment.Left; actionText.VerticalAlignment = VerticalAlignment.Top;
                            actionText.Margin = new Thickness(10, 210, 0, 0);
                            actionText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 113, 113, 113));
                            actionText.Text = "发起了 " + insertObject.villa_avatar_action.action_name;
                            grid6.Children.Add(actionText);

                            listView.Items.Add(grid6);
                        }
                    }
                    else
                    {
                        run.Text = Contentlist[i].insert.ToString();
                        run.FontSize = 16;
                        if (Contentlist[i].attributes != null)
                        {
                            if (Contentlist[i].attributes.bold != null) { run.FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                            if (Contentlist[i].attributes.italic != null) { run.FontStyle = Windows.UI.Text.FontStyle.Italic; }
                            if (!string.IsNullOrEmpty(Contentlist[i].attributes.color) && Contentlist[i].attributes.color.Substring(0, 1) == "#")
                            {
                                string colorcodeR = Contentlist[i].attributes.color.Substring(1, 2);
                                string colorcodeG = Contentlist[i].attributes.color.Substring(3, 2);
                                string colorcodeB = Contentlist[i].attributes.color.Substring(5, 2);
                                run.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)System.Convert.ToInt32("0x" + colorcodeR, 16), (byte)System.Convert.ToInt32("0x" + colorcodeG, 16), (byte)System.Convert.ToInt32("0x" + colorcodeB, 16)));
                            }
                            if (!string.IsNullOrEmpty(Contentlist[i].attributes.link))
                            {
                                Button button = new Button();
                                Grid grid7 = new Grid();
                                button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                                button.Width = double.NaN; button.Height = 90; ; button.Padding = new Thickness(0, 0, 0, 0);
                                button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                                Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                                ImageBrush image1 = new ImageBrush();
                                image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                                image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                                ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                                TextBlock block1 = new TextBlock();
                                TextBlock block2 = new TextBlock();
                                block1.Text = "[链接]" + Contentlist[i].insert.ToString();
                                block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                                block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                                block1.Margin = new Thickness(45, 13, 10, 0);
                                block2.Text = Contentlist[i].attributes.link;
                                block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                                block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                                block2.Margin = new Thickness(10, 50, 10, 0);

                                grid7.Children.Add(ellipse);
                                grid7.Children.Add(block1);
                                grid7.Children.Add(block2);
                                button.Content = grid7;
                                button.Tag = Contentlist[i].attributes.link;
                                //button.Click += new RoutedEventHandler(LinkButtonPressed);
                                listView.Items.Add(button);
                            }
                        }
                        textBlock.Inlines.Add(run);
                        run = new Run();
                    }
                    if (i + 1 == Contentlist.Count)
                    {
                        textBlock.TextWrapping = TextWrapping.Wrap;
                        listView.Items.Add(textBlock);
                    }
                }*/
                //点赞与评论状态
                GridView poststatusGV = new GridView();
                poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                Grid likenumGrid = new Grid();
                likenumGrid.MinWidth = 100;
                Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                TextBlock likenumText = new TextBlock();
                likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = rootReplyData.data.reply.stat.like_num.ToString();
                likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);


                poststatusGV.Items.Add(likenumGrid); 
                listView.Items.Add(poststatusGV);
                grid.Children.Add(listView);

                grid.Tag = rootReplyData.data.reply.reply.reply_id;
                SubRepliesListView.Items.Add(grid);
            }

            TextBlock subreplystitle = new TextBlock();subreplystitle.FontSize = 18;
            subreplystitle.HorizontalAlignment = HorizontalAlignment.Left;subreplystitle.VerticalAlignment = VerticalAlignment.Top;
            subreplystitle.Text = "回复(" + rootReplyData.data.reply.sub_reply_count + ")";
            SubRepliesListView.Items.Add(subreplystitle);

            {
                int reply_data_list_num = SubRepliesList.data.list.Count();
                for (int index = 0; index < reply_data_list_num; index++)
                {
                    Grid grid = new Grid();
                    ListView listView = new ListView();
                    listView.HorizontalContentAlignment = HorizontalAlignment.Left; listView.VerticalContentAlignment = VerticalAlignment.Top;
                    listView.VerticalAlignment = VerticalAlignment.Top; listView.IsDoubleTapEnabled = false; listView.IsHoldingEnabled = false; listView.IsTapEnabled = false;
                    listView.IsItemClickEnabled = false; listView.IsRightTapEnabled = false; listView.IsHitTestVisible = false; listView.IsMultiSelectCheckBoxEnabled = false;

                    Grid statusGrid = new Grid();
                    statusGrid.VerticalAlignment = VerticalAlignment.Top; statusGrid.Width = 305;

                    Ellipse avatarellipse = new Ellipse();
                    avatarellipse.Height = 40; avatarellipse.Width = 40;
                    avatarellipse.HorizontalAlignment = HorizontalAlignment.Left; avatarellipse.VerticalAlignment = VerticalAlignment.Top;
                    avatarellipse.Margin = new Thickness(0, 5, 0, 0);
                    ImageBrush bitmapImage = new ImageBrush();
                    bitmapImage.ImageSource = new BitmapImage(new Uri(SubRepliesList.data.list[index].user.avatar_url));
                    bitmapImage.Stretch = Stretch.UniformToFill;
                    avatarellipse.Fill = bitmapImage;
                    statusGrid.Children.Add(avatarellipse);

                    TextBlock usernameText = new TextBlock();
                    usernameText.VerticalAlignment = VerticalAlignment.Top; usernameText.HorizontalAlignment = HorizontalAlignment.Left;
                    usernameText.Margin = new Thickness(45, 8, 0, 0); usernameText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                    usernameText.FontSize = 16;
                    usernameText.Text = SubRepliesList.data.list[index].user.nickname;
                    statusGrid.Children.Add(usernameText);

                    TextBlock pubtimeText = new TextBlock();
                    pubtimeText.HorizontalAlignment = HorizontalAlignment.Left; pubtimeText.VerticalAlignment = VerticalAlignment.Top;
                    pubtimeText.Margin = new Thickness(45, 30, 0, 0);
                    pubtimeText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 113, 113, 113));
                    pubtimeText.Text = "发布于 " + dt.AddSeconds(SubRepliesList.data.list[index].reply.created_at).ToLocalTime().ToString();
                    statusGrid.Children.Add(pubtimeText);


                    if (SubRepliesList.data.list[index].reply.reply_id == SubRepliesList.data.pin_reply_id)
                    {
                        Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                        BitmapImage bitmapImage2 = new BitmapImage(new Uri("ms-appx:///Assets/Content/settop.png"));
                        image.Source = bitmapImage2;
                        image.Width = 15; image.Height = 15; image.VerticalAlignment = VerticalAlignment.Bottom; image.HorizontalAlignment = HorizontalAlignment.Right;
                        image.Margin = new Thickness(0, 0, 6, 2);
                        statusGrid.Children.Add(image);
                    }

                    listView.Items.Add(statusGrid);
                    if (SubRepliesList.data.list[index].r_user != null)
                    {
                        TextBlock replystatusBlock = new TextBlock();
                        Run run1 = new Run();
                        run1.Text = "回复 ";
                        replystatusBlock.Inlines.Add(run1);
                        run1 = new Run();
                        run1.Text = SubRepliesList.data.list[index].r_user.nickname.ToString();
                        run1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 108, 190));
                        replystatusBlock.Inlines.Add(run1);
                        listView.Items.Add(replystatusBlock);
                    }
                    string str = SubRepliesList.data.list[index].reply.struct_content.ToString();
                    if (str == "") str = "[{\"insert\":\"" + SubRepliesList.data.list[index].reply.content.ToString() + "\"}]";
                    JArray json = JArray.Parse(str);
                    List<DocumentViewContent> Contentlist = json.ToObject<List<DocumentViewContent>>();
                    InsertObject insertObject;
                    bool isInsert;
                    Run run = new Run();
                    TextBlock textBlock = new TextBlock();
                    for (int i = 0; i < Contentlist.Count; i++)
                    {
                        try
                        {
                            insertObject = JObject.Parse(Contentlist[i].insert.ToString()).ToObject<InsertObject>();
                            isInsert = true;
                        }
                        catch (Exception)
                        {
                            insertObject = null;
                            isInsert = false;
                        }
                        if (isInsert)
                        {
                            textBlock.TextWrapping = TextWrapping.Wrap;
                            listView.Items.Add(textBlock);
                            textBlock = new TextBlock();
                            if (insertObject.image != null)
                            {
                                Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                                BitmapImage bitmapImage2 = new BitmapImage(new Uri(insertObject.image));
                                //if (bitmapImage.PixelWidth <= 600) { image.Width = bitmapImage.PixelWidth; }
                                image.Source = bitmapImage2;
                                image.Width = 100; image.Height = 100;
                                image.Stretch = Stretch.UniformToFill;
                                image.Tag = bitmapImage;
                                listView.Items.Add(image);
                            }
                            if (insertObject.villa_card != null)
                            {
                                Button button = new Button();
                                Grid grid3 = new Grid();
                                button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 0, 108, 190));
                                button.Width = 310; button.Height = 100; button.Padding = new Thickness(0, 0, 0, 0);
                                Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                                TextBlock block1 = new TextBlock();
                                TextBlock block2 = new TextBlock();
                                image.Source = new BitmapImage(new Uri(insertObject.villa_card.villa_cover));
                                image.Stretch = Stretch.UniformToFill;
                                image.Opacity = 0.5;
                                block1.Text = "别野"; block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Left;
                                block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); block1.FontSize = 18;
                                block2.Text = insertObject.villa_card.villa_name + "(" + insertObject.villa_card.villa_member_num + ")";
                                block2.VerticalAlignment = VerticalAlignment.Bottom; block2.HorizontalAlignment = HorizontalAlignment.Left;
                                block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); block2.FontSize = 18;
                                grid3.Children.Add(image);
                                grid3.Children.Add(block2);
                                grid3.Children.Add(block1);
                                button.Content = grid3;
                                button.Tag = insertObject.villa_card.villa_id;
                                listView.Items.Add(button);
                            }
                            if (insertObject.mention != null)
                            {
                                Button button = new Button();
                                Grid grid4 = new Grid();
                                button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 199, 217, 255));
                                button.Width = double.NaN; button.Height = 50; button.MinWidth = 300; button.Padding = new Thickness(0, 0, 0, 0);
                                button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;

                                Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                                ImageBrush image1 = new ImageBrush();
                                UserDetailRoot Data = await GetUserDetail.GetUserDetailInfo(insertObject.mention.uid);
                                image1.ImageSource = new BitmapImage(new Uri(Data.data.user_info.avatar_url));
                                image1.Stretch = Stretch.UniformToFill;

                                ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1;
                                ellipse.Margin = new Thickness(10, 10, 0, 0); ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left;

                                TextBlock block1 = new TextBlock();
                                block1.Text = "@ " + insertObject.mention.nickname;
                                block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Left;
                                block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                                block1.Margin = new Thickness(45, 13, 10, 0);

                                grid4.Children.Add(ellipse);
                                grid4.Children.Add(block1);
                                button.Content = grid4;
                                button.Tag = insertObject.mention.uid;
                                //button.Click += new RoutedEventHandler(UserButtonPressed);
                                listView.Items.Add(button);
                            }
                            if (insertObject.link_card != null)
                            {
                                Button button = new Button();
                                Grid grid5 = new Grid();
                                button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                                button.Width = double.NaN; button.Height = 90; ; button.Padding = new Thickness(0, 0, 0, 0);
                                button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                                Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                                ImageBrush image1 = new ImageBrush();
                                image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                                image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                                ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                                TextBlock block1 = new TextBlock();
                                TextBlock block2 = new TextBlock();
                                block1.Text = "[链接卡片]" + insertObject.link_card.title;
                                block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                                block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                                block1.Margin = new Thickness(45, 13, 10, 0);
                                block2.Text = insertObject.link_card.origin_url;
                                block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                                block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                                block2.Margin = new Thickness(10, 50, 10, 0);

                                grid5.Children.Add(ellipse);
                                grid5.Children.Add(block1);
                                grid5.Children.Add(block2);
                                button.Content = grid5;
                                button.Tag = insertObject.link_card.landing_url;
                                //button.Click += new RoutedEventHandler(LinkButtonPressed);
                                listView.Items.Add(button);
                            }
                            if (insertObject.villa_forward_card != null)
                            {
                                TextBlock textBlock1 = new TextBlock();
                                TextBlock textBlock2 = new TextBlock();
                                textBlock1.Text = "从 " + insertObject.villa_forward_card.room_name + "转发的聊天记录 [小心海助手暂不支持]";
                                textBlock1.Margin = new Thickness(50, 10, 10, 0);
                                textBlock1.FontSize = 16; textBlock1.VerticalAlignment = VerticalAlignment.Top;
                                textBlock2.Text = "来自别野 " + insertObject.villa_forward_card.villa_name;
                                textBlock2.Margin = new Thickness(50, 35, 10, 10);
                                textBlock2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 127, 127, 127));
                                textBlock2.VerticalAlignment = VerticalAlignment.Top; textBlock2.HorizontalAlignment = HorizontalAlignment.Left;
                                Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                                image.Source = new BitmapImage(new Uri(insertObject.villa_forward_card.villa_avatar_url));
                                image.Width = 40; image.Height = 40; image.HorizontalAlignment = HorizontalAlignment.Left;
                                image.VerticalAlignment = VerticalAlignment.Center;

                                Grid grid6 = new Grid();
                                grid6.Children.Add(textBlock1);
                                grid6.Children.Add(textBlock2);
                                grid6.Children.Add(image);
                                Button button = new Button();
                                button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(52, 255, 108, 196));
                                button.HorizontalAlignment = HorizontalAlignment.Left; button.VerticalAlignment = VerticalAlignment.Top;
                                button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                                button.Content = grid6;
                                button.Tag = insertObject.villa_forward_card;
                                listView.Items.Add(button);
                            }
                            if (insertObject.villa_avatar_action != null)
                            {
                                Grid grid6 = new Grid();

                                Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                                BitmapImage bitmapImage2 = new BitmapImage(new Uri(insertObject.villa_avatar_action.url));
                                image.Source = bitmapImage2;
                                image.Width = 200; image.Height = 200;
                                image.Stretch = Stretch.UniformToFill;
                                grid6.Children.Add(image);

                                TextBlock actionText = new TextBlock();
                                actionText.HorizontalAlignment = HorizontalAlignment.Left; actionText.VerticalAlignment = VerticalAlignment.Top;
                                actionText.Margin = new Thickness(10, 210, 0, 0);
                                actionText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 113, 113, 113));
                                actionText.Text = "发起了 " + insertObject.villa_avatar_action.action_name;
                                grid6.Children.Add(actionText);

                                listView.Items.Add(grid6);
                            }
                        }
                        else
                        {
                            run.Text = Contentlist[i].insert.ToString();
                            run.FontSize = 16;
                            if (Contentlist[i].attributes != null)
                            {
                                if (Contentlist[i].attributes.bold != null) { run.FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                                if (Contentlist[i].attributes.italic != null) { run.FontStyle = Windows.UI.Text.FontStyle.Italic; }
                                if (!string.IsNullOrEmpty(Contentlist[i].attributes.color) && Contentlist[i].attributes.color.Substring(0, 1) == "#")
                                {
                                    string colorcodeR = Contentlist[i].attributes.color.Substring(1, 2);
                                    string colorcodeG = Contentlist[i].attributes.color.Substring(3, 2);
                                    string colorcodeB = Contentlist[i].attributes.color.Substring(5, 2);
                                    run.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)System.Convert.ToInt32("0x" + colorcodeR, 16), (byte)System.Convert.ToInt32("0x" + colorcodeG, 16), (byte)System.Convert.ToInt32("0x" + colorcodeB, 16)));
                                }
                                if (!string.IsNullOrEmpty(Contentlist[i].attributes.link))
                                {
                                    Button button = new Button();
                                    Grid grid7 = new Grid();
                                    button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                                    button.Width = double.NaN; button.Height = 90; ; button.Padding = new Thickness(0, 0, 0, 0);
                                    button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                                    Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                                    ImageBrush image1 = new ImageBrush();
                                    image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                                    image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                                    ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                                    TextBlock block1 = new TextBlock();
                                    TextBlock block2 = new TextBlock();
                                    block1.Text = "[链接]" + Contentlist[i].insert.ToString();
                                    block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                                    block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                                    block1.Margin = new Thickness(45, 13, 10, 0);
                                    block2.Text = Contentlist[i].attributes.link;
                                    block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                                    block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                                    block2.Margin = new Thickness(10, 50, 10, 0);

                                    grid7.Children.Add(ellipse);
                                    grid7.Children.Add(block1);
                                    grid7.Children.Add(block2);
                                    button.Content = grid7;
                                    button.Tag = Contentlist[i].attributes.link;
                                    //button.Click += new RoutedEventHandler(LinkButtonPressed);
                                    listView.Items.Add(button);
                                }
                            }
                            textBlock.Inlines.Add(run);
                            run = new Run();
                        }
                        if (i + 1 == Contentlist.Count)
                        {
                            textBlock.TextWrapping = TextWrapping.Wrap;
                            listView.Items.Add(textBlock);
                        }
                    }

                    GridView poststatusGV = new GridView();
                    poststatusGV.HorizontalAlignment = HorizontalAlignment.Stretch; poststatusGV.VerticalAlignment = VerticalAlignment.Bottom;
                    poststatusGV.Height = 40; poststatusGV.Margin = new Thickness(10, 0, 0, 0); poststatusGV.Padding = new Thickness(0, 0, 0, 0);
                    poststatusGV.VerticalContentAlignment = VerticalAlignment.Top; poststatusGV.HorizontalContentAlignment = HorizontalAlignment.Left;
                    poststatusGV.IsSwipeEnabled = false; poststatusGV.IsHitTestVisible = false; poststatusGV.IsEnabled = false; poststatusGV.IsZoomedInView = false;

                    Grid likenumGrid = new Grid();
                    likenumGrid.MinWidth = 100;
                    Windows.UI.Xaml.Controls.Image likenumIcon = new Windows.UI.Xaml.Controls.Image();
                    likenumIcon.Width = 20; likenumIcon.Height = 20; likenumIcon.HorizontalAlignment = HorizontalAlignment.Left; likenumIcon.VerticalAlignment = VerticalAlignment.Top;
                    likenumIcon.Stretch = Stretch.Fill; likenumIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/like.png"));
                    TextBlock likenumText = new TextBlock();
                    likenumText.HorizontalAlignment = HorizontalAlignment.Left; likenumText.VerticalAlignment = VerticalAlignment.Top;
                    likenumText.Margin = new Thickness(25, 0, 0, 0); likenumText.Text = SubRepliesList.data.list[index].stat.like_num.ToString();
                    likenumGrid.Children.Add(likenumIcon); likenumGrid.Children.Add(likenumText);

                    poststatusGV.Items.Add(likenumGrid);
                    listView.Items.Add(poststatusGV);
                    grid.Children.Add(listView);

                    grid.Tag = SubRepliesList.data.list[index].reply.reply_id;
                    SubRepliesListView.Items.Add(grid);

                }
                Grid grid_lm = new Grid();
                grid_lm.VerticalAlignment = VerticalAlignment.Top; grid_lm.Width = 320;
                Windows.UI.Xaml.Controls.Image loadmoreIcon = new Windows.UI.Xaml.Controls.Image();
                loadmoreIcon.Height = 20; loadmoreIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/loadmore.png"));
                grid_lm.Children.Add(loadmoreIcon);
                grid_lm.Tag = "loadmore";
                SubRepliesListView.Items.Add(grid_lm);
                SubReplyProgress.IsIndeterminate = false;
                SubRepliesListView.Tag = SubRepliesList.data.last_id;
            }


        }
        public async Task<RootReplyData> getRootReplyInfoAsync(string replyID)
        {
            Uri uri = new Uri("https://bbs-api.miyoushe.com/post/api/getRootReplyInfo?post_id="+PostTitle.Tag.ToString()+"&reply_id="+replyID);
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);          //TODO:增加离线逻辑
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RootReplyData));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (RootReplyData)serializer.ReadObject(ms);

            return data;
        }
        public async Task<RepliesObjectRoot> getSubRepliesAsync(string floor_id)
        {
        
            Uri uri = new Uri("https://bbs-api.miyoushe.com/post/api/getSubReplies?floor_id="+floor_id+"&post_id=" + PostTitle.Tag.ToString() + "&size=50");
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);          //TODO:增加离线逻辑
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RepliesObjectRoot));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (RepliesObjectRoot)serializer.ReadObject(ms);

            return data;
        }
    }
    public class RootReply
    {
        public RepliesList reply {  get; set; }
        public List post {  get; set; }
    }

    public class RootReplyData
    {
        public int retcode {  get; set; }
        public string message {  get; set; }
        public RootReply data { get; set; }
    }
}
