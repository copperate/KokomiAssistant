using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Data.Json;
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
    public sealed partial class DocumentView : Page
    {
        public DocumentView()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string str=e.Parameter.ToString();
            JArray json = JArray.Parse(str) ;
            List<DocumentViewContent> list = json.ToObject<List<DocumentViewContent>>();
            
            //base.OnNavigatedTo(e);
        }
    }

    public class DocumentViewContent
    {
        public object insert { get; set; }
        public Attributes attributes { get; set; }
    }
    public class Attributes
    {
        public int height { get; set; }
        public int width { get; set; }
        public int size { get; set; }
        public string ext { get; set; }
        public int? header { get; set; }
        public bool? italic { get; set; }
        public string link { get; set; }
    }
    public class ImageViewer
    {
        public string image;
    }
    public class VideoViewer
    {
        public string id { get; set; }
        public int duration { get; set; }
        public string cover { get; set; }
        public List<Resolution> resolutions { get; set; }
        public int view_num { get; set; }
        public int transcoding_status { get; set; }
        public int review_status { get; set; }
    }
}
