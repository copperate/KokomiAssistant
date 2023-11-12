using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PostDetailPanel : Page
    {
        public PostDetailPanel()
        {
            this.InitializeComponent();
            
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            {
                int postid = int.Parse((string)e.Parameter);
                GetPostDetails(postid);
            }
            else
            {
                
            }
            base.OnNavigatedTo(e);
        }
        public async void GetPostDetails(int i)
        {
            DetailRoot Data = await PostDetail.GetPostList(i);
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
            String area_sub_name;
            try
            {
                area_sub_name = Data.data.post.forum.name;
                AreaText.Text = Gameid + "-" + Data.data.post.forum.name;
            }
            catch (System.NullReferenceException)
            {
                AreaText.Text = Gameid + "(无版区)" ;
                //throw;
            }
            AreaText.Tag = gamearea;
            ViewNumText.Text="浏览量："+Data.data.post.stat.view_num.ToString();
            //Windows.Storage.StorageFolder CacheFolder = ApplicationData.Current.LocalFolder;
            //Windows.Storage.StorageFile CacheDetailFile = await CacheFolder.CreateFileAsync("temp.html",Windows.Storage.CreationCollisionOption.ReplaceExisting);
            String DetailData = "<html><head></head><body>"+Data.data.post.post.content+"</body></html>";
            //await Windows.Storage.FileIO.WriteTextAsync(CacheDetailFile,DetailData);
            //PostContent.Navigate(new Uri("ms-appdata:///local/temp.html"));
            DetailPanelLikeContentButton.Label = "赞("+Data.data.post.stat.like_num.ToString()+")";
            DetailPanelBookmarkContentButton.Label ="收藏("+Data.data.post.stat.bookmark_num.ToString()+")";  
            DetailPanelShareContentButton.Label ="转发("+Data.data.post.stat.forward_num.ToString()+")";  
            CommentPivotItem.Header="评论("+Data.data.post.stat.reply_num.ToString()+")"; 
            UserDetailPane0.Tag=Data.data.post.user.uid.ToString();

            PostContent.Visibility = Visibility.Collapsed;
            PostContentView.Visibility = Visibility.Collapsed;
            switch (Data.data.post.post.view_type)
            {
                case 1: {PostContent.NavigateToString(DetailData);PostContent.Visibility = Visibility.Visible;
                        //PostContentView.Navigate(typeof(PostViewMode.DocumentView),Data.data.post.post.structured_content);

                        }break;
                case 2: { PostContentView.Navigate(typeof(PostViewMode.MultiPicView), Data.data.post.post.content);PostContentView.Visibility = Visibility.Visible;
                    }break;
                default: PostContent.NavigateToString(DetailData); break;
            }
            
            
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

            getComments(i, 2, false);
            
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
                DetailPanelSplit.OpenPaneLength = 350;

            }
        }
        public async void getComments(int postid,int sorttype,bool ismaster)
        {
            RepliesObjectRoot reply_data = await GetPostReplies.GetPostList(postid, sorttype, ismaster);
            //RepliesListView.Items.Clear();
            List<replydetail> reply_detail = new List<replydetail>();
            int reply_data_list_num = reply_data.data.list.Count();
            DateTime dt = new DateTime(1970, 1, 1);
            for (int index = 0; index < reply_data_list_num; index++)
            {
                reply_detail.Add(new replydetail()
                {
                    Tag = reply_data.data.list[index].reply.reply_id,
                    username = reply_data.data.list[index].user.nickname,
                    useravatar = reply_data.data.list[index].user.avatar_url,
                    floor = reply_data.data.list[index].reply.floor_id + "F",
                    pubtime_likenum = dt.AddSeconds(reply_data.data.list[index].reply.created_at).ToLocalTime().ToString() + "　▲" + reply_data.data.list[index].stat.like_num,
                    replycontent = reply_data.data.list[index].reply.content,
                    sub_reply_count = "查看评论详情(" + reply_data.data.list[index].sub_reply_count.ToString() + ")"
                });
            }
            RepliesListView.ItemsSource = reply_detail;

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
            dataPackage.Properties.Title = PostTitle.Text + "　，分享自「小心海助手」";
            dataPackage.Properties.Description = shareLink;
            DataRequest request = args.Request;
            request.Data = dataPackage;
        }

        private void CommentSortChange(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            int selecteditem = comboBox.SelectedIndex;
            if (selecteditem == 0) { selecteditem = 3; }
            try
            {
                getComments((int)PostTitle.Tag, selecteditem,false);
            }
            catch (NullReferenceException)
            {
                //throw;
            }
        }
    }
    public class replydetail
    {
        public string Tag { get; set; }
        public string username { get; set; }
        public string useravatar { get; set; }
        public string floor { get; set; }
        public string pubtime_likenum { get; set; }
        public string replycontent { get; set; }
        public string sub_reply_count { get; set; }

    }
}
