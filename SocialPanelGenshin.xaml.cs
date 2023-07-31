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
using Windows.UI.Xaml.Navigation;
using Windows.Data.Json;
using Windows.Storage;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SocialPanelGenshin : Page
    {
        public SocialPanelGenshin()
        {
            this.InitializeComponent();
        }

        public class PostList
        {
            public String ChannelID { get; set; }
            public String PostID { get; set; }
            public String UserHeaderURL { get; set; }
            public String UserName { get; set; }
            public String PostTitle { get; set; }
            public String PostPreview { get; set; }
            public String PostPreviewImg { get; set; }

            public PostList(String channelid,String postid, String userheaderurl, String username,String posttitle,String postpreview,String postpreviewimg)
            {
                this.ChannelID = channelid;
                this.PostID = postid;
                this.UserHeaderURL = userheaderurl;
                this.UserName = username;   
                this.PostTitle = posttitle;
                this.PostPreview = postpreview;
                this.PostPreviewImg = postpreviewimg;
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GetRecommendList(object sender, RoutedEventArgs e)
        {

        }

        private void GetOfficalpostList(object sender, RoutedEventArgs e)
        {
            
        }
        private void GetpostList_jiuguan(object sender, RoutedEventArgs e)
        {

            Uri datauri = new Uri("https://api-takumi.miyoushe.com/post/wapi/getForumPostList?forum_id=26&gids=2&is_good=false&is_hot=false&page_size=20&sort_type=2");
            
        }
    }
    
}
