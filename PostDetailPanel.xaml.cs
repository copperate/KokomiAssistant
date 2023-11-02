using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
            DetailUserImage.ImageSource = new BitmapImage(new Uri(Data.data.post.user.avatar_url));
            DetailUserNickname.Text = Data.data.post.user.nickname;
            String Gameid;int gamearea;
            try
            {
                gamearea = Data.data.post.forum.game_id;
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
                case 6: Gameid = "崩坏;星穹铁道"; break;
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
            PostContent.NavigateToString(DetailData);

            double publish_time = Data.data.post.post.created_at;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddSeconds(publish_time).ToLocalTime();
            PubTimeText.Text = "发布/修改时间：" + dt.ToShortDateString() + " " + dt.ToLongTimeString();
        }
        public static Image get_Fill_image(string url)
        {
            var image = new Image();
            image.Source = new BitmapImage(new Uri(url, UriKind.Absolute));
            image.Stretch = Stretch.Fill;
            return image;
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
                DetailPanelSplit.OpenPaneLength = 300;

            }
        }
    }
}
