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
                    case 6: Gameid = "崩坏;星穹铁道"; break;
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

        }

    }
}
