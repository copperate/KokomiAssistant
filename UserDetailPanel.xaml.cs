using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;
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
            /*
            if(Data.data.user_info.certifications == null)
            {*/
                DetailUserCertificationLabel.Height = 0;
            /*
            }
            else
            {
                double WidthA = 26 + DetailUserLocationLabel.ActualWidth;
                DetailUserCertificationLabel.Margin= new Thickness(WidthA, 0, 0, 0);    
                DetailUserCertification.Text="认证: " + Data.data.user_info.certifications[0].label;
            }*/
            FollowingNumText.Text=Data.data.user_info.achieve.follow_cnt.ToString();
            FollowerNumText.Text = Data.data.user_info.achieve.followed_cnt.ToString();
            LikeNumText.Text=Data.data.user_info.achieve.like_num.ToString();
            
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
            DateTime dt = new DateTime(1970, 1, 1);
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
                            Tag = data.data.list[qi].entity_id,
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
            Frame.Navigate(typeof(PostDetailPanel), postID);
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
            try
            {
                get1 = subareaid.data.list[currentid].post.forum.name;
            }
            catch
            {
                get1 = "无版区";
            }
            return (get1);
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
