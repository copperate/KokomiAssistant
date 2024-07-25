using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
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
    public sealed partial class ToolPanelGame : Page
    {
        Frame rootFrame = Window.Current.Content as Frame;

        public ToolPanelGame()
        {
            this.InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ToolPanelGameViewed();
            base.OnNavigatedTo(e);
        }

        public async void ToolPanelGameViewed()
        {
            ProgressStatus.IsIndeterminate = true;
            GameToolRootObject GenshinData = await GetGameTools(2);
            GameToolRootObject Honkai3Data = await GetGameTools(1);
            GameToolRootObject StarrailData = await GetGameTools(6);
            GameToolRootObject ZenZeroData = await GetGameTools(8);
            

            List<GameToolsView> list= new List<GameToolsView>();
            for (int i = 0;i<GenshinData.data.navigator.Count; i++)
            {
                list.Add(new GameToolsView()
                {
                    Tag = GenshinData.data.navigator[i].app_path,
                    ToolCover = GenshinData.data.navigator[i].icon,
                    ToolName = GenshinData.data.navigator[i].name
                });
            }
            GenshinToolPanelGrid.ItemsSource = list;


            List<GameToolsView> list2 = new List<GameToolsView>();
            for (int i = 0; i < Honkai3Data.data.navigator.Count; i++)
            {
                list2.Add(new GameToolsView()
                {
                    Tag = Honkai3Data.data.navigator[i].app_path,
                    ToolCover = Honkai3Data.data.navigator[i].icon,
                    ToolName = Honkai3Data.data.navigator[i].name
                });
            }
            Honkai3ToolPanelGrid.ItemsSource = list2;


            List<GameToolsView> list3 = new List<GameToolsView>();
            for (int i = 0; i < StarrailData.data.navigator.Count; i++)
            {
                list3.Add(new GameToolsView()
                {
                    Tag = StarrailData.data.navigator[i].app_path,
                    ToolCover = StarrailData.data.navigator[i].icon,
                    ToolName = StarrailData.data.navigator[i].name
                });
            }
            StarrailToolPanelGrid.ItemsSource = list3;

            List<GameToolsView> list4 = new List<GameToolsView>();
            for (int i = 0; i < ZenZeroData.data.navigator.Count; i++)
            {
                list4.Add(new GameToolsView()
                {
                    Tag = ZenZeroData.data.navigator[i].app_path,
                    ToolCover = ZenZeroData.data.navigator[i].icon,
                    ToolName = ZenZeroData.data.navigator[i].name
                });
            }
            ZenZeroToolPanelGrid.ItemsSource = list4;

            ProgressStatus.IsIndeterminate = false;
        }

        public async static Task<GameToolRootObject> GetGameTools(int channelID)
        {
            Uri uri = new Uri("https://api-takumi.miyoushe.com/apihub/api/home/new?gids=" + channelID);
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(GameToolRootObject));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (GameToolRootObject)serializer.ReadObject(ms);
            return data;
        }

        private void ToolPanel_ItemClicked(object sender, ItemClickEventArgs e)
        {
            dynamic clickedItem = e.ClickedItem;
            string uri = (string)clickedItem.Tag;
            Uri local=new Uri(uri);
            string schemehost = local.Scheme;
            bool nflag = true;
            if (schemehost == "mihoyobbs")
            {
                if (local.Host == "article") { rootFrame.Navigate(typeof(PostDetailPanel), local.AbsolutePath.Substring(1)); nflag = false; }
                if (local.Host == "user") { rootFrame.Navigate(typeof(UserDetailPanel), local.AbsolutePath.Substring(1)); nflag = false; }
                if (nflag) { rootFrame.Navigate(typeof(SchemeRedirectPanel), local); }
            }
            else
            {
                rootFrame.Navigate(typeof(ToolPanelInsideWebView), local);
            }
        }
    }

    public class GameToolsView
    { 
        public string Tag { get; set; } 
        public string ToolCover { get; set; }
        public string ToolName { get; set; }
    }

    //Get Homepage Tools Data
    public class HomeBackground
    {
        public string image { get; set; }
        public string color { get; set; }
    }

    public class HomeCarousels
    {
        public int position { get; set; }
        public List<GameToolData> data { get; set; }
    }

    public class Config
    {
        public int id { get; set; }
        public int game_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string status { get; set; }
        public HomeRules rules { get; set; }
        public Questionnaire questionnaire { get; set; }
        public Pkg pkg { get; set; }
        public DetailServlet detail_servlet { get; set; }
        public PreRegisterCount pre_register_count { get; set; }
        public bool is_top { get; set; }
        public string last_update_time { get; set; }
        public int app_store_switch { get; set; }
        public int download_switch { get; set; }
        public string developer { get; set; }
    }

    public class TopicCount
    {
        public int view { get; set; }
        public int discuss { get; set; }
    }

    public class GameToolData
    {
        //Top Data
        public List<HomeNavigator> navigator { get; set; }
        public HomeDiscussion discussion { get; set; }
        public HomeBackground background { get; set; }
        public HomeOfficial official { get; set; }
        public HomeCarousels carousels { get; set; }
        public HotTopics hot_topics { get; set; }
        public List<GameReception> game_receptions { get; set; }
        public List<object> posts { get; set; }
        public List<object> lives { get; set; }
        public object recommend_villa { get; set; }

        //game_reception_data, current useless
        public Config config { get; set; }
        public object user_status { get; set; }

        //offical post recommend data here
        public string post_id { get; set; }
        public string title { get; set; }
        public string date { get; set; }
        public string label { get; set; }
        public bool is_top { get; set; }
        public int view_type { get; set; }
        public string image_url { get; set; }

        //carousels data
        public string cover { get; set; }
        public string app_path { get; set; }

        //topic data
        public int topic_id { get; set; }
        public string image { get; set; }
        public string topic_name { get; set; }
        public string post_name { get; set; }
        public TopicCount count { get; set; }
    }

    public class DetailServlet
    {
        public int type { get; set; }
        public object normal_servlet { get; set; }
        public object customize_detail { get; set; }
    }

    public class HomeDiscussion
    {
        public string icon { get; set; }
        public string title { get; set; }
        public string prompt { get; set; }
    }

    public class GameReception
    {
        public int position { get; set; }
        public GameToolData data { get; set; }
    }

    public class HotTopics
    {
        public int position { get; set; }
        public List<GameToolData> data { get; set; }
    }

    public class HomeNavigator
    {
        public int id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public string app_path { get; set; }
        public int reddot_online_time { get; set; }
    }

    public class HomeOfficial
    {
        public int position { get; set; }
        public int forum_id { get; set; }
        public List<GameToolData> data { get; set; }
    }

    public class Pkg
    {
        public string android_url { get; set; }
        public string pkg_name { get; set; }
        public string pkg_version { get; set; }
        public string ios_url { get; set; }
        public string pkg_length { get; set; }
        public string pkg_md5 { get; set; }
        public string pkg_version_code { get; set; }
        public string ios_version { get; set; }
        public string new_filename { get; set; }
        public string filename { get; set; }
        public string pkg_version_name { get; set; }
    }

    public class PreRegisterCount
    {
        public bool show_count { get; set; }
        public string count { get; set; }
    }

    public class Questionnaire
    {
        public int questionnaire_type { get; set; }
        public string questionnaire_url { get; set; }
        public int questionnaire_status { get; set; }
    }

    public class GameToolRootObject
    {
        public int retcode { get; set; }
        public string message { get; set; }
        public GameToolData data { get; set; }
    }

    public class HomeRules
    {
        public int game_level { get; set; }
        public int community_level { get; set; }
    }


}
