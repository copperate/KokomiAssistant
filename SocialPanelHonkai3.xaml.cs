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
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SocialPanelHonkai3 : Page
    {

        DateTime dt = new DateTime(1970, 1, 1);

        public SocialPanelHonkai3()
        {
            this.InitializeComponent();
        }

        private void NavigateDetail(object sender, ItemClickEventArgs e)
        {
            String postID;
            dynamic clickedItem = e.ClickedItem;
            postID = (string)clickedItem.Tag;
            Frame.Navigate(typeof(PostDetailPanel), postID);
        }

        private void AppbarButton_Refresh_Click(object sender, RoutedEventArgs e)
        {
            int pivotselected = Honkai3Pivot.SelectedIndex;
            ProgressStatus.IsIndeterminate = true;
            switch (pivotselected)
            {
                case 0: Honkai3DiscoverList_Loading(null, e); break;
                case 1: Honkai3OfficalList_Loaded(null, e); break;
                case 2: Honkai3DockList_Loaded(null, e); break;
                case 3: Honkai3WalkthroughList_Loaded(null, e); break;
                case 4: Honkai3FanPicList_Loaded(null, e); break;
                case 5: Honkai3FanDocList_Loaded(null, e); break;
                default: break;
            }
        }

        private void GotoSearchPanel(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SocialSearchPanel), null);
        }

        private async void Honkai3DiscoverList_Loading(FrameworkElement sender, object args)
        {
            ProgressStatus.IsIndeterminate = true;
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            //RootObject data = await PostList.GetPostList(28, 1, "");
            Uri uri = new Uri("https://api-takumi.miyoushe.com/post/api/feeds/posts?fresh_action=1&gids=1&last_id=");
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
                list.Add(new ListViewPostGallery()
                {
                    Tag = data.data.list[i].post.post_id,
                    UserPic = data.data.list[i].user.avatar_url,
                    User = data.data.list[i].user.nickname,
                    PostTitle = data.data.list[i].post.subject,
                    PostSummary = data.data.list[i].post.content + blankstring,
                    PostPic = data.data.list[i].post.cover,
                    ViewNum = data.data.list[i].stat.view_num,
                    LikeNum = data.data.list[i].stat.like_num,
                    CommentNum = data.data.list[i].stat.reply_num,
                    last_id = data.data.last_id,
                    PubTime = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString()
                });
            }
            Honkai3DiscoverList.ItemsSource = list;
            Honkai3DiscoverList.Tag = list;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void Honkai3OfficalList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(6, 1, "");
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
                list.Add(new ListViewPostGallery()
                {
                    Tag = data.data.list[i].post.post_id,
                    UserPic = data.data.list[i].user.avatar_url,
                    User = data.data.list[i].user.nickname,
                    PostTitle = data.data.list[i].post.subject,
                    PostSummary = data.data.list[i].post.content + blankstring,
                    PostPic = data.data.list[i].post.cover,
                    ViewNum = data.data.list[i].stat.view_num,
                    LikeNum = data.data.list[i].stat.like_num,
                    CommentNum = data.data.list[i].stat.reply_num,
                    last_id = data.data.last_id,
                    PubTime = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString()
                });
            }
            Honkai3OfficalList.ItemsSource = list;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void Honkai3DockList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(1, 1, "");
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
                list.Add(new ListViewPostGallery()
                {
                    Tag = data.data.list[i].post.post_id,
                    UserPic = data.data.list[i].user.avatar_url,
                    User = data.data.list[i].user.nickname,
                    PostTitle = data.data.list[i].post.subject,
                    PostSummary = data.data.list[i].post.content + blankstring,
                    PostPic = data.data.list[i].post.cover,
                    ViewNum = data.data.list[i].stat.view_num,
                    LikeNum = data.data.list[i].stat.like_num,
                    CommentNum = data.data.list[i].stat.reply_num,
                    last_id = data.data.last_id,
                    PubTime = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString()
                });
            }
            Honkai3DockList.ItemsSource = list;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void Honkai3WalkthroughList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(14, 1, "");
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
                list.Add(new ListViewPostGallery()
                {
                    Tag = data.data.list[i].post.post_id,
                    UserPic = data.data.list[i].user.avatar_url,
                    User = data.data.list[i].user.nickname,
                    PostTitle = data.data.list[i].post.subject,
                    PostSummary = data.data.list[i].post.content + blankstring,
                    PostPic = data.data.list[i].post.cover,
                    ViewNum = data.data.list[i].stat.view_num,
                    LikeNum = data.data.list[i].stat.like_num,
                    CommentNum = data.data.list[i].stat.reply_num,
                    last_id = data.data.last_id,
                    PubTime = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString()
                });
            }
            Honkai3WalkthroughList.ItemsSource = list;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void Honkai3FanDocList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(41, 1, "");
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
                list.Add(new ListViewPostGallery()
                {
                    Tag = data.data.list[i].post.post_id,
                    UserPic = data.data.list[i].user.avatar_url,
                    User = data.data.list[i].user.nickname,
                    PostTitle = data.data.list[i].post.subject,
                    PostSummary = data.data.list[i].post.content + blankstring,
                    PostPic = data.data.list[i].post.cover,
                    ViewNum = data.data.list[i].stat.view_num,
                    LikeNum = data.data.list[i].stat.like_num,
                    CommentNum = data.data.list[i].stat.reply_num,
                    last_id = data.data.last_id,
                    PubTime = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString()
                });
            }
            Honkai3FanDocList.ItemsSource = list;
            ProgressStatus.IsIndeterminate = false;
        }

        private async void Honkai3FanPicList_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressStatus.IsIndeterminate = true;
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(4, 1, "");
            int listnum = data.data.list.Count;
            string blankstring = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            for (int i = 0; i < listnum; i++)
            {
                list.Add(new ListViewPostGallery()
                {
                    Tag = data.data.list[i].post.post_id,
                    UserPic = data.data.list[i].user.avatar_url,
                    User = data.data.list[i].user.nickname,
                    PostTitle = data.data.list[i].post.subject,
                    PostSummary = data.data.list[i].post.content + blankstring,
                    PostPic = data.data.list[i].post.cover,
                    ViewNum = data.data.list[i].stat.view_num,
                    LikeNum = data.data.list[i].stat.like_num,
                    CommentNum = data.data.list[i].stat.reply_num,
                    last_id = data.data.last_id,
                    PubTime = dt.AddSeconds(data.data.list[i].post.created_at).ToLocalTime().ToString()
                });
            }
            Honkai3FanPicList.ItemsSource = list;
            ProgressStatus.IsIndeterminate = false;
        }
    }
}
