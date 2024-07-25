using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;
using Windows.Networking.NetworkOperators;
using Windows.System;
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
    public sealed partial class UserDetailPanel : Page
    {
        public UserDetailPanel()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            {
                int userid = int.Parse((string)e.Parameter);
                GetUserDetails(userid);
            }
            else
            {

            }
            base.OnNavigatedTo(e);
        }
        public async void GetUserDetails(int i)
        {
            UserDetailRoot Data = await GetUserDetail.GetUserDetailInfo(i);

            DetailUserNickname.Text=Data.data.user_info.nickname;
            DetailUserIntroduce.Text = Data.data.user_info.introduce;
            DetailUserImage.ImageSource = new BitmapImage(new Uri(Data.data.user_info.avatar_url));
            DetailUserID.Text = "ID:"+Data.data.user_info.uid.ToString();
            DetailUserID.Tag = Data.data.user_info.uid;
            if (Data.data.user_info.pendant != "")
            {
                DetailPendantImage.Source = new BitmapImage(new Uri(Data.data.user_info.pendant));
            }
            else DetailPendantImage.Source = null;

            DetailUserLocation.Text = "位置: " + Data.data.user_info.ip_region.ToString();
            DetailUserCertificationLabel.Height = 0;
            
            FollowingNumText.Text=Data.data.user_info.achieve.follow_cnt.ToString();
            FollowerNumText.Text = Data.data.user_info.achieve.followed_cnt.ToString();
            LikeNumText.Text=Data.data.user_info.achieve.like_num.ToString();
            if (Data.data.user_silence_status != 0)
            {
                UserStatus.Visibility = Visibility.Visible;
                UserStatus.Tag=Data.data.user_silence_reason.ToString();
            }
            else UserStatus.Visibility = Visibility.Collapsed;
            
            if(Data.data.user_info.community_info.user_func_status.show_self_created_villa== "SHOW_SELF_CREATED_VILLA_STATUS_PUBLIC")
            {
                DetailUserVillaRoot villadata = await GetDetailUserVilla.GetUserVillaDetailInfo(i);
                try
                {
                    UserVillaButtonText.Text = villadata.data.villas[0].villa.name.ToString()+"("+ villadata.data.villas[0].member_num.ToString()+")";
                    UserVillaButtonBg.Source = new BitmapImage(new Uri(villadata.data.villas[0].villa.villa_cover));
                }
                catch
                {
                    UserVillaButton.Height = 0;
                    GameStatusPanelGrid.Margin = new Thickness(10, 176, 0, 0);
                }
            }
            else
            {
                UserVillaButton.Height = 0;
                GameStatusPanelGrid.Margin = new Thickness(10, 176, 0, 0);
            }
            String Gameid;
            String UserLevelString="";
            int channelID = 0; ;
            int channelIDs = 0; ;
            while (channelIDs != 99)
            {
                try {
                    channelID = Data.data.user_info.level_exps[channelIDs].game_id;
                } catch {
                    channelID = 99;
                }
                switch (channelID)
                {
                    case 1: Gameid = "崩坏3"; break;
                    case 2: Gameid = "原神"; break;
                    case 3: Gameid = "崩坏学园2"; break;
                    case 4: Gameid = "未定事件簿"; break;
                    case 5: Gameid = "大别野"; break;
                    case 6: Gameid = "崩坏:星穹铁道"; break;
                    case 8: Gameid = "绝区零"; break;
                    case 9981: Gameid = "Jabbr"; break;
                    default: Gameid = "未知"; break;
                }
                if (Gameid != "未知")
                {
                    if (channelIDs == 0)
                    {
                        UserLevelString = UserLevelString + Gameid + " Lv." + Data.data.user_info.level_exps[channelIDs].level.ToString();
                    }
                    else UserLevelString = UserLevelString + " | " + Gameid + " Lv." + Data.data.user_info.level_exps[channelIDs].level.ToString();
                    channelIDs = channelIDs + 1;
                }
                else channelIDs = 99;
            }
            DetailUserLevel.Text = UserLevelString;
            DetailUserLevel.Tag = UserLevelString;
            UserPostListObjectRoot ListData = await GetUserPostList.GetPostList_of_User(i);
            
            UserListHideHint.Visibility = Visibility.Collapsed;
            UserPostScrollView.Visibility = Visibility.Collapsed;
            if (ListData.retcode== 0)
            {
                UserPostScrollView.Visibility = Visibility.Visible;
                UserPostListShow(ListData);
            }
            else
            {
                UserListHideHint.Visibility = Visibility.Visible;//UserPostListShow(ListData);
            }
        }
        public void UserPostListShow(UserPostListObjectRoot data)
        {
            List<UserPostListViewContentPost> listdata = new List<UserPostListViewContentPost>();
            int list_num = data.data.list.Count();
            DateTime dt = new DateTime(1970, 1, 1, 8, 0, 0);
            for(int qi = 0;qi < list_num;qi++)
            {
                switch(data.data.list[qi].entity_type)
                {
                    case 1:
                        listdata.Add(new UserPostListViewContentPost()
                        {
                            Tag = data.data.list[qi].entity_id,
                            PostTitle = data.data.list[qi].post.post.subject,
                            PostSummery = data.data.list[qi].post.post.summary,
                            PostArea = areaget(data.data.list[qi].game_id)+" · " + subareaget(data,qi),
                            PublicDate_Day = dt.AddSeconds(data.data.list[qi].post.post.created_at).Day.ToString(),
                            PublicDate_Month= dt.AddSeconds(data.data.list[qi].post.post.created_at).Month.ToString(),
                            PublicDate_Year_Time= dt.AddSeconds(data.data.list[qi].post.post.created_at).ToShortTimeString()+" · "+ dt.AddSeconds(data.data.list[qi].post.post.created_at).Year.ToString(),
                            UserPic = data.data.list[qi].post.post.cover.ToString()
                        });break;
                    case 2:
                        listdata.Add(new UserPostListViewContentPost()
                        {
                            Tag = "t"+data.data.list[qi].entity_id,
                            PostTitle = "发布了一条动态",
                            PostSummery = data.data.list[qi].instant.instant.content,
                            PostArea = "动态[当前版本不支持查看详情]",
                            PublicDate_Day = dt.AddSeconds(double.Parse(data.data.list[qi].instant.instant.created_at)).Day.ToString(),
                            PublicDate_Month = dt.AddSeconds(double.Parse(data.data.list[qi].instant.instant.created_at)).Month.ToString(),
                            PublicDate_Year_Time = dt.AddSeconds(double.Parse(data.data.list[qi].instant.instant.created_at)).ToShortTimeString() + " · " + dt.AddSeconds(double.Parse(data.data.list[qi].instant.instant.created_at)).Year.ToString(),
                            //UserPic = data.data.list[qi].post.post.cover.ToString(),
                        }); break;
                    default:break;
                }
               // try{
                    
              //  }catch{continue;}
                
            }
            UserPostListView.ItemsSource = listdata;
        }
        private void NavigateDetail(object sender, ItemClickEventArgs e)
        {
            String postID;
            dynamic clickedItem = e.ClickedItem;
            postID = (string)clickedItem.Tag;
            if (String.Equals(postID[0],'t'))
            {
                
            }
            else Frame.Navigate(typeof(PostDetailPanel), postID);
            //}
        }
        public string areaget(string areaid)
        {
            string Gameid;
            switch (int.Parse(areaid))
            {
                case 1: Gameid = "崩坏3"; break;
                case 2: Gameid = "原神"; break;
                case 3: Gameid = "崩坏学园2"; break;
                case 4: Gameid = "未定事件簿"; break;
                case 5: Gameid = "大别野"; break;
                case 6: Gameid = "崩坏:星穹铁道"; break;
                case 8: Gameid = "绝区零"; break;
                case 9981: Gameid = "Jabbr"; break;
                default: Gameid = "未知"; break;
            }
            return (Gameid);
        }
        public string subareaget(UserPostListObjectRoot subareaid,int currentid)
        {
            string get1;
            if(subareaid.data.list[currentid].post.forum != null)
            {
                get1 = subareaid.data.list[currentid].post.forum.name;

            }
            else get1 = "无版区";
            
            return (get1);
        }

        private async void UserCommentShow(object sender, RoutedEventArgs e)
        {
            string userID = DetailUserID.Tag.ToString(); string nextOffset = "";
            if (UserCommentList.Tag != null) nextOffset = UserCommentList.Tag.ToString();

            Uri uri = new Uri("https://api-takumi.miyoushe.com/post/api/userReply?offset=" + nextOffset + "&size=20&uid=" + userID);
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);          //TODO:增加离线逻辑
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RepliesObjectRoot));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            RepliesObjectRoot userReplies = (RepliesObjectRoot)serializer.ReadObject(ms);

            if (userReplies.retcode != 0) 
            {
                CommentListHideHint.Visibility = Visibility.Visible;
                UserCommentScrollView.Visibility = Visibility.Collapsed;
            }
            else
            {
                UserCommentList.Tag = userReplies.data.next_offset;
                DateTime dt = new DateTime(1970, 1, 1, 8, 0, 0);
                for (int i = 0; i < userReplies.data.list.Count; i++) 
                {
                    Grid grid = new Grid(); Grid grid2 = new Grid();
                    TextBlock forward = new TextBlock();
                    grid2.Width = 5;grid2.Height = 30;grid2.CornerRadius = new CornerRadius(2.5);grid2.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 86, 212, 255));
                    grid2.HorizontalAlignment = HorizontalAlignment.Left;grid.Children.Add(grid2);

                    forward.Margin = new Thickness(10, 0, 0, 0);
                    forward.VerticalAlignment = VerticalAlignment.Top;forward.HorizontalAlignment = HorizontalAlignment.Left;forward.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 124, 124, 124));
                    if (userReplies.data.list[i].r_reply != null)
                    {
                        forward.Text = dt.AddSeconds(userReplies.data.list[i].reply.created_at).ToLocalTime().ToString() + " · 回复评论 " + userReplies.data.list[i].r_reply.content;
                    }
                    else 
                    if (userReplies.data.list[i].r_post != null)
                    {
                        forward.Text = dt.AddSeconds(userReplies.data.list[i].reply.created_at).ToLocalTime().ToString() + " · 在帖子<" + userReplies.data.list[i].r_post.subject + ">下的回复";
                    }
                    grid.Children.Add(forward);

                    TextBlock detail = new TextBlock();
                    detail.VerticalAlignment = VerticalAlignment.Top; detail.HorizontalAlignment = HorizontalAlignment.Left;
                    detail.FontSize = 16;detail.Margin = new Thickness(10, 23, 0, 0);
                    detail.Text = userReplies.data.list[i].reply.content;
                    grid.Children.Add(detail);

                    grid.Tag = userReplies.data.list[i].reply.post_id;
                    UserCommentList.Items.Add(grid);
                }
            }
        }

        private void NavigateCommentDetail(object sender, ItemClickEventArgs e)
        {
            String postID;
            dynamic clickedItem = e.ClickedItem;
            postID = (string)clickedItem.Tag;
            if (String.Equals(postID[0], 't'))
            {

            }
            else Frame.Navigate(typeof(PostDetailPanel), postID);
        }
    }
    public class UserPostListViewContentPost 
    { 
        public string Tag { get; set; }
        public string PostTitle { get; set; }
        public string PostSummery { get; set; }
        public string PostArea { get; set; }
        public string UserPic { get; set; }
        public string PublicDate_Day { get; set; }
        public string PublicDate_Month { get; set; }
        public string PublicDate_Year_Time { get; set; }
    }
}
