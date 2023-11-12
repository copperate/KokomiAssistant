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
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Contacts;

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
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GetpostList_Recommend(object sender, RoutedEventArgs e)
        {

        }

        private async void GetpostList_Offical(object sender, RoutedEventArgs e)
        {
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(28, 1, 1);
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
                                PostSummary = data.data.list[i].post.content+blankstring,
                                PostPic = data.data.list[i].post.cover
                            }) ;
            }
            
            GenshinOfficalList.ItemsSource = list;
        }
        private async void GetpostList_Bar(object sender, RoutedEventArgs e)
        {
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(26, 1, 1);
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
            GenshinBarList.ItemsSource = list;
        }

        private void NavigateDetail(object sender, ItemClickEventArgs e)
        {
            String postID;
            dynamic clickedItem = e.ClickedItem;
                postID = (string)clickedItem.Tag;
                Frame.Navigate(typeof(PostDetailPanel), postID);
            //}
        }


        private void NavigateViaID(object sender, RoutedEventArgs e)
        {
            String postID = (string)navigateidbox.Text.ToString();
            Frame.Navigate(typeof(PostDetailPanel), postID);
        }

        private async void GenshinGonglveList_Loaded(object sender, RoutedEventArgs e)
        {
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(43, 1, 1);
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
            GenshinGonglveList.ItemsSource = list;
        }

        private async void GenshinFanPicList_Loaded(object sender, RoutedEventArgs e)
        {
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(29, 1, 1);
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
            GenshinFanPicList.ItemsSource = list;
        }

        private async void GenshinCosplayList_Loaded(object sender, RoutedEventArgs e)
        {
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(49, 1, 1);
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
            GenshinCosplayList.ItemsSource = list;
        }

        private async void GenshinHardcoreList_Loaded(object sender, RoutedEventArgs e)
        {
            List<ListViewPostGallery> list = new List<ListViewPostGallery>();
            RootObject data = await PostList.GetPostList(50, 1, 1);
            int listnum = data.data.list.Count; 
            for (int i = 0; i < listnum; i++)
            {
                list.Add(new ListViewPostGallery()
                {
                    Tag = data.data.list[i].post.post_id,
                    UserPic = data.data.list[i].user.avatar_url,
                    User = data.data.list[i].user.nickname,
                    PostTitle = data.data.list[i].post.subject,
                    PostSummary = data.data.list[i].post.content,
                    PostPic = data.data.list[i].post.cover
                });
            }
            GenshinHardcoreList.ItemsSource = list;
        }

        private async void AppbarButton_Refresh_Click(object sender, RoutedEventArgs e)
        {
            int pivotselected = GenshinPivot.SelectedIndex;
            List<ListViewPostGallery> listA = new List<ListViewPostGallery>();
            RootObject dataA; int listnumA; string blankstringA = "　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　";
            switch (pivotselected)
            {
                case 0:break;
                case 2:
                    dataA = await PostList.GetPostList(26, 1, 1);
                    listnumA = dataA.data.list.Count;
                    
                    for (int i = 0; i < listnumA; i++)
                    {
                        listA.Add(new ListViewPostGallery()
                        {
                            Tag = dataA.data.list[i].post.post_id,
                            UserPic = dataA.data.list[i].user.avatar_url,
                            User = dataA.data.list[i].user.nickname,
                            PostTitle = dataA.data.list[i].post.subject,
                            PostSummary = dataA.data.list[i].post.content + blankstringA,
                            PostPic = dataA.data.list[i].post.cover
                        });
                    }
                    GenshinBarList.ItemsSource = listA; break;
                case 1:
                    dataA = await PostList.GetPostList(28, 1, 1);
                    listnumA = dataA.data.list.Count;
                    for (int i = 0; i < listnumA; i++)
                    {
                        listA.Add(new ListViewPostGallery()
                        {
                            Tag = dataA.data.list[i].post.post_id,
                            UserPic = dataA.data.list[i].user.avatar_url,
                            User = dataA.data.list[i].user.nickname,
                            PostTitle = dataA.data.list[i].post.subject,
                            PostSummary = dataA.data.list[i].post.content + blankstringA,
                            PostPic = dataA.data.list[i].post.cover
                        });
                    }
                    GenshinOfficalList.ItemsSource = listA; break;
                case 3:
                    dataA = await PostList.GetPostList(43, 1, 1);
                    listnumA = dataA.data.list.Count;
                    for (int i = 0; i < listnumA; i++)
                    {
                        listA.Add(new ListViewPostGallery()
                        {
                            Tag = dataA.data.list[i].post.post_id,
                            UserPic = dataA.data.list[i].user.avatar_url,
                            User = dataA.data.list[i].user.nickname,
                            PostTitle = dataA.data.list[i].post.subject,
                            PostSummary = dataA.data.list[i].post.content + blankstringA,
                            PostPic = dataA.data.list[i].post.cover
                        });
                    }
                    GenshinGonglveList.ItemsSource = listA; break;
                case 4:dataA = await PostList.GetPostList(29, 1, 1);
                    listnumA = dataA.data.list.Count;
                    for (int i = 0; i < listnumA; i++)
                    {
                        listA.Add(new ListViewPostGallery()
                        {
                            Tag = dataA.data.list[i].post.post_id,
                            UserPic = dataA.data.list[i].user.avatar_url,
                            User = dataA.data.list[i].user.nickname,
                            PostTitle = dataA.data.list[i].post.subject,
                            PostSummary = dataA.data.list[i].post.content + blankstringA,
                            PostPic = dataA.data.list[i].post.cover
                        });
                    }
                    GenshinFanPicList.ItemsSource = listA; break; 
                case 5:dataA = await PostList.GetPostList(49, 1, 1);
                    listnumA = dataA.data.list.Count;
                    for (int i = 0; i < listnumA; i++)
                    {
                        listA.Add(new ListViewPostGallery()
                        {
                            Tag = dataA.data.list[i].post.post_id,
                            UserPic = dataA.data.list[i].user.avatar_url,
                            User = dataA.data.list[i].user.nickname,
                            PostTitle = dataA.data.list[i].post.subject,
                            PostSummary = dataA.data.list[i].post.content + blankstringA,
                            PostPic = dataA.data.list[i].post.cover
                        });
                    }
                    GenshinCosplayList.ItemsSource = listA; break;
                case 6:dataA = await PostList.GetPostList(50, 1, 1);
                    listnumA = dataA.data.list.Count;
                    for (int i = 0; i < listnumA; i++)
                    {
                        listA.Add(new ListViewPostGallery()
                        {
                            Tag = dataA.data.list[i].post.post_id,
                            UserPic = dataA.data.list[i].user.avatar_url,
                            User = dataA.data.list[i].user.nickname,
                            PostTitle = dataA.data.list[i].post.subject,
                            PostSummary = dataA.data.list[i].post.content,
                            PostPic = dataA.data.list[i].post.cover
                        });
                    }
                    GenshinHardcoreList.ItemsSource = listA; break;
                default:
                    break;
            }
        }
    }

    public class ListViewPostGallery
    {
        public string Tag { get; set; }
        public string UserPic { get; set; }
        public string User { get; set; }
        public string PostTitle { get; set; }
        public string PostSummary { get; set; }
        public string PostPic { get; set; }

    }

}
