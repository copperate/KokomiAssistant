using Newtonsoft.Json.Linq;
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

namespace KokomiAssistant.PostViewMode
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MultiPicView : Page
    {
        public MultiPicView()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string str = e.Parameter.ToString();
            JObject json = JObject.Parse(str);
            PicViewAllContent picViewContent = json.ToObject<PicViewAllContent>();
            try
            {
                PicViewDescribe.Text = picViewContent.describe;
            }
            catch (ArgumentNullException)
            {
                PicViewDescribe.Text = "";
                //throw;
            }
            List <PicViewContent> list = new List<PicViewContent>();
            int listnum = picViewContent.imgs.Count;
            for(int i = 0; i < listnum; i++)
            {
                list.Add(new PicViewContent() { ImageSource = picViewContent.imgs[i] });
            }
            ImglistView.ItemsSource = list;
        }
    }
    public class PicViewAllContent
    {
        public string describe { get; set; }
        public List<string> imgs { get; set; }
    }
    public class PicViewContent
    {
        public string ImageSource { get; set; }
    }

}
