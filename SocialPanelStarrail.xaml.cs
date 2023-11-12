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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SocialPanelStarrail : Page
    {
        public SocialPanelStarrail()
        {
            this.InitializeComponent();
        }

        private void NavigateDetail(object sender, ItemClickEventArgs e)
        {
            String postID;
            dynamic clickedItem = e.ClickedItem;
            postID = (string)clickedItem.Tag;
            Frame.Navigate(typeof(PostDetailPanel), postID);
            //}
        }

        private async void StarrailOfficalList_Loaded(object sender, RoutedEventArgs e)
        {
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(53, 1, 1);
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
                    PostPic = data.data.list[i].post.cover
                });
            }
            StarrailOfficalList.ItemsSource = list;
        }

        private async void StarrailWaitingRoomList_Loaded(object sender, RoutedEventArgs e)
        {
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(52, 1, 1);
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
                    PostPic = data.data.list[i].post.cover
                });
            }
            StarrailWaitingRoomList.ItemsSource = list;
        }

        private async void StarrailWalkthroughList_Loaded(object sender, RoutedEventArgs e)
        {
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(61, 1, 1);
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
                    PostPic = data.data.list[i].post.cover
                });
            }
            StarrailWalkthroughList.ItemsSource = list;
        }

        private async void StarrailFanPicList_Loaded(object sender, RoutedEventArgs e)
        {
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(56, 1, 1);
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
                    PostPic = data.data.list[i].post.cover
                });
            }
            StarrailFanPicList.ItemsSource = list;
        }

        private async void StarrailCosplayList_Loaded(object sender, RoutedEventArgs e)
        {
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(62, 1, 1);
            int listnum = data.data.list.Count;
            string blankstring = "                                                                         ";
            for (int i = 0; i < listnum; i++)
            {
                list.Add(new ListViewPostGallery()
                {
                    Tag = data.data.list[i].post.post_id,
                    UserPic = data.data.list[i].user.avatar_url,
                    User = data.data.list[i].user.nickname,
                    PostTitle = data.data.list[i].post.subject,
                    PostSummary = data.data.list[i].post.content + blankstring,
                    PostPic = data.data.list[i].post.cover
                });
            }
            StarrailCosplayList.ItemsSource = list;
        }
    }
    
}
