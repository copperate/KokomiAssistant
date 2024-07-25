using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;
using Windows.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Controls;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.Media.Casting;
using Windows.Media.Core;

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
            LoadPageDefault(e.Parameter.ToString());
        }
        public async void LoadPageDefault(string e)
        {
            string str = e;
            JArray json = JArray.Parse(str) ;
            List<DocumentViewContent> Contentlist = json.ToObject<List<DocumentViewContent>>();
            InsertObject insertObject;
            bool isInsert;
            Run run = new Run(); 
            TextBlock textBlock = new TextBlock();

            List<Paragraph> paragraphs = new List<Paragraph>();
            List<InlineUIContainer> InlineUIContainers = new List<InlineUIContainer>();
            paragraphs.Add(new Paragraph());

            for (int i = 0; i < Contentlist.Count; i++)
            {
                try
                {
                    insertObject = JObject.Parse(Contentlist[i].insert.ToString()).ToObject<InsertObject>();
                    isInsert = true;
                }
                catch (Exception)
                {
                    insertObject = null;
                    isInsert = false;
                }
                
                if (isInsert)
                {
                    if (insertObject.image != null)
                    {
                        Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                        BitmapImage bitmapImage = new BitmapImage(new Uri(insertObject.image));
                        //if (bitmapImage.PixelWidth <= 600) { image.Width = bitmapImage.PixelWidth; }
                        image.Source = bitmapImage;
                        image.Stretch = Stretch.UniformToFill;
                        image.Tapped += new TappedEventHandler(DetailPageImageView);
                        image.Tag = bitmapImage;
                        InlineUIContainers.Add(new InlineUIContainer());
                        InlineUIContainers[InlineUIContainers.Count - 1].Child = image;
                        paragraphs[paragraphs.Count - 1].Inlines.Add(InlineUIContainers[InlineUIContainers.Count - 1]);
                    }
                    if (insertObject.vod != null)
                    {
                        Grid videogrid = new Grid();videogrid.CornerRadius = new CornerRadius(10);
                        
                        MediaPlayerElement mediaElement = new MediaPlayerElement();
                        mediaElement.Source = MediaSource.CreateFromUri(new Uri(insertObject.vod.resolutions[insertObject.vod.resolutions.Count - 1].url));
                        mediaElement.PosterSource = new BitmapImage(new Uri(insertObject.vod.cover));
                        mediaElement.AreTransportControlsEnabled = true;
                        mediaElement.AutoPlay = false;
                        videogrid.Children.Add(mediaElement);

                        InlineUIContainers.Add(new InlineUIContainer()); 
                        InlineUIContainers[InlineUIContainers.Count-1].AllowFocusOnInteraction = true;
                        
                        paragraphs.Add(new Paragraph());
                        InlineUIContainers[InlineUIContainers.Count - 1].Child = videogrid;
                        paragraphs[paragraphs.Count - 1].Inlines.Add(InlineUIContainers[InlineUIContainers.Count - 1]);
                        paragraphs.Add(new Paragraph());
                    }
                    if (insertObject.villa_card != null)
                    {
                        Button button = new Button();
                        Grid grid = new Grid();
                        button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 0, 108, 190));
                        button.Width = 310; button.Height = 100; button.Padding = new Thickness(0, 0, 0, 0);
                        Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                        TextBlock block1 = new TextBlock();
                        TextBlock block2 = new TextBlock();
                        image.Source = new BitmapImage(new Uri(insertObject.villa_card.villa_cover));
                        image.Stretch = Stretch.UniformToFill;
                        image.Opacity = 0.5;
                        block1.Text = "别野"; block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Left;
                        block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); block1.FontSize = 18;
                        block2.Text = insertObject.villa_card.villa_name + "(" + insertObject.villa_card.villa_member_num + ")";
                        block2.VerticalAlignment = VerticalAlignment.Bottom; block2.HorizontalAlignment = HorizontalAlignment.Left;
                        block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); block2.FontSize = 18;
                        grid.Children.Add(image);
                        grid.Children.Add(block2);
                        grid.Children.Add(block1);
                        button.Content = grid;
                        button.Tag = insertObject.villa_card.villa_id;
                        InlineUIContainers.Add(new InlineUIContainer());
                        InlineUIContainers[InlineUIContainers.Count - 1].Child = button;
                        paragraphs[paragraphs.Count - 1].Inlines.Add(InlineUIContainers[InlineUIContainers.Count - 1]);
                    }
                    if (insertObject.lottery != null)
                    {
                        TextBlock textBlock1 = new TextBlock();
                        textBlock1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 79, 165, 216));
                        textBlock1.Text = "[抽奖]" + insertObject.lottery.toast + " [小心海助手暂不支持]";
                        textBlock1.FontSize = 16;
                        InlineUIContainers.Add(new InlineUIContainer());
                        InlineUIContainers[InlineUIContainers.Count - 1].Child = textBlock1;
                        paragraphs[paragraphs.Count - 1].Inlines.Add(InlineUIContainers[InlineUIContainers.Count - 1]);
                    }
                    if (insertObject.divider != null)
                    {
                        Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                        switch (insertObject.divider)
                        {
                            case "line_1": image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_1.png")); break;
                            case "line_2": image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_2.png")); break;
                            case "line_3": image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_3.png")); break;
                            case "line_4": image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_4.png")); break;
                            default: image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_2.png")); break;
                        }
                        image.Stretch = Stretch.None;
                        InlineUIContainers.Add(new InlineUIContainer());
                        paragraphs.Add(new Paragraph());
                        InlineUIContainers[InlineUIContainers.Count - 1].Child = image;
                        paragraphs[paragraphs.Count - 1].Inlines.Add(InlineUIContainers[InlineUIContainers.Count - 1]);
                        paragraphs[paragraphs.Count-1].TextAlignment = TextAlignment.Center;
                        paragraphs.Add(new Paragraph());
                    }
                    if (insertObject.mention != null)
                    {
                        Button button = new Button();
                        Grid grid4 = new Grid();
                        button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 199, 217, 255));
                        //button.Width = double.NaN; button.Height = 50; button.MinWidth = 300; 
                        button.Padding = new Thickness(0, 0, 0, 0); button.CornerRadius = new CornerRadius(5, 5, 5, 5);
                        button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;

                        Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                        ImageBrush image1 = new ImageBrush();
                        UserDetailRoot Data = await GetUserDetail.GetUserDetailInfo(insertObject.mention.uid);
                        image1.ImageSource = new BitmapImage(new Uri(Data.data.user_info.avatar_url));
                        image1.Stretch = Stretch.UniformToFill;

                        ellipse.Width = 20; ellipse.Height = 20; ellipse.Fill = image1;
                        ellipse.Margin = new Thickness(0, 0, 0, 0); ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left;

                        TextBlock block1 = new TextBlock();
                        block1.Text = "@" + insertObject.mention.nickname;
                        block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Left;
                        block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 14;
                        block1.Margin = new Thickness(25, 0, 5, 0);

                        grid4.Children.Add(ellipse);
                        grid4.Children.Add(block1);
                        button.Content = grid4;
                        button.Tag = insertObject.mention.uid;
                        button.Click += new RoutedEventHandler(UserButtonPressed);

                        InlineUIContainers.Add(new InlineUIContainer());
                        InlineUIContainers[InlineUIContainers.Count - 1].Child = button;
                        paragraphs[paragraphs.Count - 1].Inlines.Add(InlineUIContainers[InlineUIContainers.Count - 1]);
                    }
                    if (insertObject.link_card != null)
                    {
                        Button button = new Button();
                        Grid grid = new Grid();
                        button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                        button.Width = double.NaN; button.Height = 90; ; button.Padding = new Thickness(0, 0, 0, 0);
                        button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                        Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                        ImageBrush image1 = new ImageBrush();
                        image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                        image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                        ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                        TextBlock block1 = new TextBlock();
                        TextBlock block2 = new TextBlock();
                        block1.Text = "[链接卡片]" + insertObject.link_card.title;
                        block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                        block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                        block1.Margin = new Thickness(45, 13, 10, 0);
                        block2.Text = insertObject.link_card.origin_url;
                        block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                        block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                        block2.Margin = new Thickness(10, 50, 10, 0);

                        grid.Children.Add(ellipse);
                        grid.Children.Add(block1);
                        grid.Children.Add(block2);
                        button.Content = grid;
                        button.Tag = insertObject.link_card.landing_url;
                        button.Click += new RoutedEventHandler(LinkButtonPressed);

                        InlineUIContainers.Add(new InlineUIContainer());
                        InlineUIContainers[InlineUIContainers.Count - 1].Child = button;
                        paragraphs[paragraphs.Count - 1].Inlines.Add(InlineUIContainers[InlineUIContainers.Count - 1]);
                    }
                    if (insertObject.fold != null)
                    {
                        Grid foldGrid = new Grid();
                        foldGrid.BorderThickness = new Thickness(1, 1, 1, 1);foldGrid.CornerRadius = new CornerRadius(5, 5, 5, 5);foldGrid.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); 
                        //折叠标题
                        TextBlock foldTitle = new TextBlock();
                        foldTitle = foldTitleExtract(insertObject.fold.title);foldTitle.Margin = new Thickness(10, 6, 0, 0);
                        foldGrid.Children.Add(foldTitle);
                        //分界线
                        Line line=new Line();line.X1 = 0; line.X2 = 400;line.Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));line.VerticalAlignment = VerticalAlignment.Top;line.Margin= new Thickness(0,40,0,0);
                        foldGrid.Children.Add(line);
                        //折叠内容
                        RichTextBlock foldContent = new RichTextBlock();
                        foldContent= foldContentExtract(insertObject.fold.content);
                        foldContent.Margin=new Thickness(5,41,5,0);
                        foldGrid.Children.Add(foldContent);

                        InlineUIContainers.Add(new InlineUIContainer());
                        InlineUIContainers[InlineUIContainers.Count - 1].Child = foldGrid;
                        paragraphs[paragraphs.Count - 1].Inlines.Add(InlineUIContainers[InlineUIContainers.Count - 1]);
                    }
                    if (insertObject.vote != null)
                    {
                        VoteRoot votedata = await GetVotesDetail.GetVotes(insertObject.vote.id, insertObject.vote.uid);
                        VoteRoot votedetail = await GetVotesDetail.GetVotesResult(insertObject.vote.id, insertObject.vote.uid);

                        ListView listView = new ListView();
                        listView.HorizontalContentAlignment = HorizontalAlignment.Left;
                        listView.VerticalContentAlignment = VerticalAlignment.Top;
                        listView.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 200, 200, 200));
                        TextBlock textBlock1 = new TextBlock(); textBlock1.FontSize = 18;
                        textBlock1.Text = votedata.data.data[0].title + " (共" + votedetail.data.data[0].user_cnt + "人参与)";
                        listView.Items.Add(textBlock1);

                        int[] voteCount = new int[51];
                        voteCount =await GetVotesResult2(insertObject.vote.id, insertObject.vote.uid);

                        for (int votelist = 0; votelist < votedata.data.data[0].vote_option_indexes.Count; votelist++)
                        {
                            Grid grid = new Grid();
                            Windows.UI.Xaml.Controls.ProgressBar progressBar = new Windows.UI.Xaml.Controls.ProgressBar();
                            progressBar.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 249, 202, 188));
                            progressBar.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(180, 255, 255, 255));
                            progressBar.HorizontalAlignment = HorizontalAlignment.Left;
                            progressBar.VerticalAlignment = VerticalAlignment.Top;
                            progressBar.Width = 400; progressBar.Height = 40; progressBar.Maximum = votedetail.data.data[0].user_cnt;

                            TextBlock textBlock2 = new TextBlock();
                            textBlock2.FontSize = 18; textBlock2.VerticalAlignment = VerticalAlignment.Center; textBlock2.Margin = new Thickness(10, 0, 10, 0);

                            textBlock2.Text = votedata.data.data[0].vote_option_indexes[votelist] + " (" + voteCount[votelist] + ")";
                            progressBar.Value = voteCount[votelist];
                            
                            grid.Children.Add(progressBar);
                            grid.Children.Add(textBlock2);

                            listView.Items.Add(grid);
                        }

                        InlineUIContainers.Add(new InlineUIContainer());
                        InlineUIContainers[InlineUIContainers.Count - 1].Child = listView;
                        paragraphs[paragraphs.Count - 1].Inlines.Add(InlineUIContainers[InlineUIContainers.Count - 1]);
                    }
                    if (insertObject.villa_forward_card != null)
                    {
                        TextBlock textBlock1 = new TextBlock();
                        TextBlock textBlock2 = new TextBlock();
                        textBlock1.Text = "从 " + insertObject.villa_forward_card.room_name + "转发的聊天记录 [小心海助手暂不支持]";
                        textBlock1.Margin = new Thickness(50, 10, 10, 0);
                        textBlock1.FontSize = 16; textBlock1.VerticalAlignment = VerticalAlignment.Top;
                        textBlock2.Text = "来自别野 " + insertObject.villa_forward_card.villa_name;
                        textBlock2.Margin = new Thickness(50, 35, 10, 10);
                        textBlock2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 127, 127, 127));
                        textBlock2.VerticalAlignment = VerticalAlignment.Top; textBlock2.HorizontalAlignment = HorizontalAlignment.Left;
                        Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                        image.Source = new BitmapImage(new Uri(insertObject.villa_forward_card.villa_avatar_url));
                        image.Width = 40; image.Height = 40; image.HorizontalAlignment = HorizontalAlignment.Left;
                        image.VerticalAlignment = VerticalAlignment.Center;

                        Grid grid = new Grid();
                        grid.Children.Add(textBlock1);
                        grid.Children.Add(textBlock2);
                        grid.Children.Add(image);
                        Button button = new Button();
                        button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(52, 255, 108, 196));
                        button.HorizontalAlignment = HorizontalAlignment.Left; button.VerticalAlignment = VerticalAlignment.Top;
                        button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                        button.Content = grid;
                        button.Tag = insertObject.villa_forward_card;

                        InlineUIContainers.Add(new InlineUIContainer());
                        InlineUIContainers[InlineUIContainers.Count - 1].Child = button;
                        paragraphs[paragraphs.Count - 1].Inlines.Add(InlineUIContainers[InlineUIContainers.Count - 1]);
                    }
                }
                else
                {
                    run.Text = Contentlist[i].insert.ToString();
                    run.FontSize = 16;
                    if (Contentlist[i].attributes != null)
                    {
                        if (Contentlist[i].attributes.bold != null) { run.FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                        if (Contentlist[i].attributes.italic != null) { run.FontStyle = Windows.UI.Text.FontStyle.Italic; }
                        
                        if (!string.IsNullOrEmpty(Contentlist[i].attributes.color) && Contentlist[i].attributes.color.Substring(0,1)=="#")
                        {
                            string colorcodeR = Contentlist[i].attributes.color.Substring(1, 2);
                            string colorcodeG = Contentlist[i].attributes.color.Substring(3, 2);
                            string colorcodeB = Contentlist[i].attributes.color.Substring(5, 2);
                            run.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)System.Convert.ToInt32("0x" + colorcodeR, 16), (byte)System.Convert.ToInt32("0x" + colorcodeG, 16), (byte)System.Convert.ToInt32("0x" + colorcodeB, 16)));
                        }
                        if (!string.IsNullOrEmpty(Contentlist[i].attributes.link))
                        {
                            Button button = new Button();
                            Grid grid = new Grid();
                            Grid grid1 = new Grid();

                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                            button.Width = double.NaN; button.Height = 54; ; button.Padding = new Thickness(0, 0, 0, 0);
                            button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                            Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                            ImageBrush image1 = new ImageBrush();
                            image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                            image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                            ellipse.Width = 20; ellipse.Height = 20; ellipse.Fill = image1; 
                            ellipse.VerticalAlignment = VerticalAlignment.Top; 
                            ellipse.HorizontalAlignment = HorizontalAlignment.Left; 
                            ellipse.Margin = new Thickness(10, 5, 0, 0);

                            TextBlock block1 = new TextBlock();
                            TextBlock block2 = new TextBlock();
                            run.Text = "[链接]" + run.Text;
                            block1.Inlines.Add(run);
                            block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                            block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 11;
                            block1.Margin = new Thickness(30, 4, 20, 0);
                            Line line = new Line();line.Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 134, 134, 134));line.X2 = 100;
                            line.HorizontalAlignment=HorizontalAlignment.Left;line.VerticalAlignment = VerticalAlignment.Top;line.Margin = new Thickness(0, 29, 0, 0);
                            block2.Text = Contentlist[i].attributes.link;
                            block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                            block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block2.FontSize = 11;
                            block2.Margin = new Thickness(10, 30, 10, 0);

                            grid.Children.Add(ellipse);
                            grid.Children.Add(block1);
                            grid.Children.Add(line);
                            grid.Children.Add(block2);
                            button.Content = grid;
                            button.Tag = Contentlist[i].attributes.link;
                            button.Click += new RoutedEventHandler(LinkButtonPressed);

                            grid1.Children.Add(button);
                            grid1.HorizontalAlignment = HorizontalAlignment.Left;grid1.VerticalAlignment = VerticalAlignment.Top;
                            grid1.CornerRadius = new CornerRadius(10, 10, 10, 10);

                            InlineUIContainers.Add(new InlineUIContainer());
                            InlineUIContainers[InlineUIContainers.Count - 1].Child = grid1;
                            paragraphs[paragraphs.Count - 1].Inlines.Add(InlineUIContainers[InlineUIContainers.Count - 1]);
                            run = new Run();//已显示按钮，因此文本隐藏
                        }
                    }
                    if (Contentlist[i].insert.ToString() == "\n")
                    {
                        paragraphs[paragraphs.Count - 1].Inlines.Add(run);
                        //检查标题
                        if (Contentlist[i].attributes != null)
                        {
                            if (Contentlist[i].attributes.header == 1)
                            {
                                paragraphs[paragraphs.Count - 1].FontSize = 20;
                                paragraphs[paragraphs.Count - 1].FontWeight = Windows.UI.Text.FontWeights.SemiBold;
                            }
                            else
                            {
                                paragraphs[paragraphs.Count - 1].FontSize = 18;
                                paragraphs[paragraphs.Count - 1].FontWeight = Windows.UI.Text.FontWeights.SemiBold;
                            }
                        }
                        paragraphs.Add(new Paragraph());
                    }
                    else paragraphs[paragraphs.Count - 1].Inlines.Add(run);
                    run = new Run();
                }
                
            }
            for (int i = 0; i < paragraphs.Count(); i++)
            {
                DocumentViewBlock.Blocks.Add(paragraphs[i]);
            }

            /* 原加载页面代码
            for (int i = 0; i < Contentlist.Count; i++)
            {
                try
                {
                    insertObject = JObject.Parse(Contentlist[i].insert.ToString()).ToObject<InsertObject>();
                    isInsert = true;
                }
                catch (Exception)
                {
                    insertObject = null;
                    isInsert = false;
                }
                
                if (isInsert)
                {
                    textBlock.TextWrapping = TextWrapping.Wrap;
                    DocumentViewList.Items.Add(textBlock);
                    textBlock = new TextBlock();
                    if (insertObject.image != null)
                    {
                        Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                        BitmapImage bitmapImage = new BitmapImage(new Uri(insertObject.image));
                        //if (bitmapImage.PixelWidth <= 600) { image.Width = bitmapImage.PixelWidth; }
                        image.Source = bitmapImage;
                        image.Stretch = Stretch.UniformToFill;
                        image.Tapped += new TappedEventHandler(DetailPageImageView);
                        image.Tag = bitmapImage;
                        DocumentViewList.Items.Add(image);
                    }
                    if (insertObject.vod != null)
                    {
                        MediaElement mediaElement = new MediaElement();
                        mediaElement.Source = new Uri(insertObject.vod.resolutions[insertObject.vod.resolutions.Count - 1].url);
                        mediaElement.AreTransportControlsEnabled = true;
                        mediaElement.AutoPlay = false;
                        DocumentViewList.Items.Add((MediaElement)mediaElement);
                    }
                    if (insertObject.villa_card != null)
                    {
                        Button button = new Button();
                        Grid grid = new Grid();
                        button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 0, 108, 190));
                        button.Width = 310; button.Height = 100; button.Padding = new Thickness(0, 0, 0, 0);
                        Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                        TextBlock block1 = new TextBlock();
                        TextBlock block2 = new TextBlock();
                        image.Source = new BitmapImage(new Uri(insertObject.villa_card.villa_cover));
                        image.Stretch = Stretch.UniformToFill;
                        image.Opacity = 0.5;
                        block1.Text = "别野"; block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Left;
                        block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); block1.FontSize = 18;
                        block2.Text = insertObject.villa_card.villa_name + "(" + insertObject.villa_card.villa_member_num + ")";
                        block2.VerticalAlignment = VerticalAlignment.Bottom; block2.HorizontalAlignment = HorizontalAlignment.Left;
                        block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)); block2.FontSize = 18;
                        grid.Children.Add(image);
                        grid.Children.Add(block2);
                        grid.Children.Add(block1);
                        button.Content = grid;
                        button.Tag = insertObject.villa_card.villa_id;
                        DocumentViewList.Items.Add(button);
                    }
                    if (insertObject.lottery != null)
                    {
                        TextBlock textBlock1 = new TextBlock();
                        textBlock1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 79, 165, 216));
                        textBlock1.Text = "[抽奖]" + insertObject.lottery.toast + " [小心海助手暂不支持]";
                        textBlock1.FontSize = 16;
                        DocumentViewList.Items.Add(textBlock1);
                    }
                    if (insertObject.divider != null)
                    {
                        Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                        switch (insertObject.divider)
                        {
                            case "line_1": image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_1.png")); break;
                            case "line_2": image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_2.png")); break;
                            case "line_3": image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_3.png")); break;
                            case "line_4": image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_4.png")); break;
                            default: image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_2.png")); break;
                        }
                        image.Stretch = Stretch.None;
                        DocumentViewList.Items.Add(image);
                    }
                    if (insertObject.mention != null)
                    {
                        Button button = new Button();
                        Grid grid4 = new Grid();
                        button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 199, 217, 255));
                        //button.Width = double.NaN; button.Height = 50; button.MinWidth = 300; 
                        button.Padding = new Thickness(0, 0, 0, 0); button.CornerRadius = new CornerRadius(5, 5, 5, 5);
                        button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;

                        Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                        ImageBrush image1 = new ImageBrush();
                        UserDetailRoot Data = await GetUserDetail.GetUserDetailInfo(insertObject.mention.uid);
                        image1.ImageSource = new BitmapImage(new Uri(Data.data.user_info.avatar_url));
                        image1.Stretch = Stretch.UniformToFill;

                        ellipse.Width = 20; ellipse.Height = 20; ellipse.Fill = image1;
                        ellipse.Margin = new Thickness(0, 0, 0, 0); ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left;

                        TextBlock block1 = new TextBlock();
                        block1.Text = "@" + insertObject.mention.nickname;
                        block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Left;
                        block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 14;
                        block1.Margin = new Thickness(25, 0, 5, 0);

                        grid4.Children.Add(ellipse);
                        grid4.Children.Add(block1);
                        button.Content = grid4;
                        button.Tag = insertObject.mention.uid;
                        button.Click += new RoutedEventHandler(UserButtonPressed);

                        DocumentViewList.Items.Add(button);
                    }
                    if (insertObject.link_card != null)
                    {
                        Button button = new Button();
                        Grid grid = new Grid();
                        button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                        button.Width = double.NaN; button.Height = 90; ; button.Padding = new Thickness(0, 0, 0, 0);
                        button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                        Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                        ImageBrush image1 = new ImageBrush();
                        image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                        image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                        ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                        TextBlock block1 = new TextBlock();
                        TextBlock block2 = new TextBlock();
                        block1.Text = "[链接卡片]" + insertObject.link_card.title;
                        block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                        block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                        block1.Margin = new Thickness(45, 13, 10, 0);
                        block2.Text = insertObject.link_card.origin_url;
                        block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                        block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                        block2.Margin = new Thickness(10, 50, 10, 0);

                        grid.Children.Add(ellipse);
                        grid.Children.Add(block1);
                        grid.Children.Add(block2);
                        button.Content = grid;
                        button.Tag = insertObject.link_card.landing_url;
                        button.Click += new RoutedEventHandler(LinkButtonPressed);
                        DocumentViewList.Items.Add(button);
                    }
                    if (insertObject.fold != null)
                    {
                        Grid foldGrid = new Grid();
                        foldGrid.BorderThickness = new Thickness(1, 1, 1, 1);foldGrid.CornerRadius = new CornerRadius(5, 5, 5, 5);foldGrid.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); 
                        //折叠标题
                        TextBlock foldTitle = new TextBlock();
                        foldTitle = foldTitleExtract(insertObject.fold.title);foldTitle.Margin = new Thickness(10, 6, 0, 0);
                        foldGrid.Children.Add(foldTitle);
                        //分界线
                        Line line=new Line();line.X1 = 0; line.X2 = 400;line.Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));line.VerticalAlignment = VerticalAlignment.Top;line.Margin= new Thickness(0,40,0,0);
                        foldGrid.Children.Add(line);
                        //折叠内容
                        RichTextBlock foldContent = new RichTextBlock();
                        foldContent= foldContentExtract(insertObject.fold.content);
                        foldContent.Margin=new Thickness(5,41,5,0);
                        foldGrid.Children.Add(foldContent); 
                        DocumentViewList.Items.Add(foldGrid);
                    }
                    if (insertObject.vote != null)
                    {
                        VoteRoot votedata = await GetVotesDetail.GetVotes(insertObject.vote.id, insertObject.vote.uid);
                        VoteRoot votedetail = await GetVotesDetail.GetVotesResult(insertObject.vote.id, insertObject.vote.uid);

                        ListView listView = new ListView();
                        listView.HorizontalContentAlignment = HorizontalAlignment.Left;
                        listView.VerticalContentAlignment = VerticalAlignment.Top;
                        listView.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 200, 200, 200));
                        TextBlock textBlock1 = new TextBlock(); textBlock1.FontSize = 18;
                        textBlock1.Text = votedata.data.data[0].title + " (共" + votedetail.data.data[0].user_cnt + "人参与)";
                        listView.Items.Add(textBlock1);

                        int[] voteCount = new int[51];
                        voteCount =await GetVotesResult2(insertObject.vote.id, insertObject.vote.uid);

                        for (int votelist = 0; votelist < votedata.data.data[0].vote_option_indexes.Count; votelist++)
                        {
                            Grid grid = new Grid();
                            Windows.UI.Xaml.Controls.ProgressBar progressBar = new Windows.UI.Xaml.Controls.ProgressBar();
                            progressBar.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 249, 202, 188));
                            progressBar.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(180, 255, 255, 255));
                            progressBar.HorizontalAlignment = HorizontalAlignment.Left;
                            progressBar.VerticalAlignment = VerticalAlignment.Top;
                            progressBar.Width = 400; progressBar.Height = 40; progressBar.Maximum = votedetail.data.data[0].user_cnt;

                            TextBlock textBlock2 = new TextBlock();
                            textBlock2.FontSize = 18; textBlock2.VerticalAlignment = VerticalAlignment.Center; textBlock2.Margin = new Thickness(10, 0, 10, 0);

                            textBlock2.Text = votedata.data.data[0].vote_option_indexes[votelist] + " (" + voteCount[votelist] + ")";
                            progressBar.Value = voteCount[votelist];
                            
                            grid.Children.Add(progressBar);
                            grid.Children.Add(textBlock2);

                            listView.Items.Add(grid);
                        }
                        DocumentViewList.Items.Add(listView);
                    }
                    if (insertObject.villa_forward_card != null)
                    {
                        TextBlock textBlock1 = new TextBlock();
                        TextBlock textBlock2 = new TextBlock();
                        textBlock1.Text = "从 " + insertObject.villa_forward_card.room_name + "转发的聊天记录 [小心海助手暂不支持]";
                        textBlock1.Margin = new Thickness(50, 10, 10, 0);
                        textBlock1.FontSize = 16; textBlock1.VerticalAlignment = VerticalAlignment.Top;
                        textBlock2.Text = "来自别野 " + insertObject.villa_forward_card.villa_name;
                        textBlock2.Margin = new Thickness(50, 35, 10, 10);
                        textBlock2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 127, 127, 127));
                        textBlock2.VerticalAlignment = VerticalAlignment.Top; textBlock2.HorizontalAlignment = HorizontalAlignment.Left;
                        Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
                        image.Source = new BitmapImage(new Uri(insertObject.villa_forward_card.villa_avatar_url));
                        image.Width = 40; image.Height = 40; image.HorizontalAlignment = HorizontalAlignment.Left;
                        image.VerticalAlignment = VerticalAlignment.Center;

                        Grid grid = new Grid();
                        grid.Children.Add(textBlock1);
                        grid.Children.Add(textBlock2);
                        grid.Children.Add(image);
                        Button button = new Button();
                        button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(52, 255, 108, 196));
                        button.HorizontalAlignment = HorizontalAlignment.Left; button.VerticalAlignment = VerticalAlignment.Top;
                        button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                        button.Content = grid;
                        button.Tag = insertObject.villa_forward_card;
                        DocumentViewList.Items.Add(button);
                    }
                }
                else
                {
                    run.Text = Contentlist[i].insert.ToString();
                    run.FontSize = 16;
                    if (Contentlist[i].attributes != null)
                    {
                        if (Contentlist[i].attributes.bold != null) { run.FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                        if (Contentlist[i].attributes.italic != null) { run.FontStyle = Windows.UI.Text.FontStyle.Italic; }
                        
                        if (!string.IsNullOrEmpty(Contentlist[i].attributes.color) && Contentlist[i].attributes.color.Substring(0,1)=="#")
                        {
                            string colorcodeR = Contentlist[i].attributes.color.Substring(1, 2);
                            string colorcodeG = Contentlist[i].attributes.color.Substring(3, 2);
                            string colorcodeB = Contentlist[i].attributes.color.Substring(5, 2);
                            run.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)System.Convert.ToInt32("0x" + colorcodeR, 16), (byte)System.Convert.ToInt32("0x" + colorcodeG, 16), (byte)System.Convert.ToInt32("0x" + colorcodeB, 16)));
                        }
                        if (!string.IsNullOrEmpty(Contentlist[i].attributes.link))
                        {
                            Button button = new Button();
                            Grid grid = new Grid();
                            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                            button.Width = double.NaN; button.Height = 90; ; button.Padding = new Thickness(0, 0, 0, 0);
                            button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                            Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                            ImageBrush image1 = new ImageBrush();
                            image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                            image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                            ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                            TextBlock block1 = new TextBlock();
                            TextBlock block2 = new TextBlock();
                            block1.Text = "[链接]" + Contentlist[i].insert.ToString();
                            block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                            block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                            block1.Margin = new Thickness(45, 13, 10, 0);
                            block2.Text = Contentlist[i].attributes.link;
                            block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                            block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                            block2.Margin = new Thickness(10, 50, 10, 0);

                            grid.Children.Add(ellipse);
                            grid.Children.Add(block1);
                            grid.Children.Add(block2);
                            button.Content = grid;
                            button.Tag = Contentlist[i].attributes.link;
                            button.Click += new RoutedEventHandler(LinkButtonPressed);
                            DocumentViewList.Items.Add(button);
                        }
                    }
                    //检查标题
                    try
                    {
                        int headerid = 0; string headertext = Contentlist[i + 1].insert.ToString();
                        if (headertext == "\n" && Contentlist[i + 1].attributes != null) headerid = Contentlist[i + 1].attributes.header;
                        if (headerid == 1) { run.FontSize = 20; run.FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                        if (headerid == 2) { run.FontSize = 18; run.FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                    }
                    catch (Exception) { }
                    textBlock.Inlines.Add(run);
                    run = new Run();
                }
                if (i + 1 == Contentlist.Count)
                {
                    textBlock.TextWrapping = TextWrapping.Wrap;
                    DocumentViewList.Items.Add(textBlock);
                }
            }
            */
        }

        private void DetailPageImageView(object sender, TappedRoutedEventArgs e)
        {
            Windows.UI.Xaml.Controls.Image image = sender as Windows.UI.Xaml.Controls.Image;
            Grid grid = PostDetailPanel.PostDetailPage.PanelPicViewGrid;
            Windows.UI.Xaml.Controls.Image image1 = PostDetailPanel.PostDetailPage.PanelPicView;
            image1.Source = image.Tag as BitmapImage;
            grid.Visibility = Visibility.Visible;
        }
        public TextBlock foldTitleExtract(string objectTitle)
        {
            TextBlock textBlock = new TextBlock();
            JArray json = JArray.Parse(objectTitle);
            List<DocumentViewContent> Contentlist = json.ToObject<List<DocumentViewContent>>();
            Run run = new Run();
            for (int i = 0; i < Contentlist.Count; i++)
            {
                run.Text = Contentlist[i].insert.ToString();
                run.FontSize = 16;
                if (Contentlist[i].attributes != null)
                {
                    if (Contentlist[i].attributes.bold != null) { run.FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                    if (Contentlist[i].attributes.italic != null) { run.FontStyle = Windows.UI.Text.FontStyle.Italic; }

                    if (!string.IsNullOrEmpty(Contentlist[i].attributes.color) && Contentlist[i].attributes.color.Substring(0, 1) == "#")
                    {
                        string colorcodeR = Contentlist[i].attributes.color.Substring(1, 2);
                        string colorcodeG = Contentlist[i].attributes.color.Substring(3, 2);
                        string colorcodeB = Contentlist[i].attributes.color.Substring(5, 2);
                        run.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)System.Convert.ToInt32("0x" + colorcodeR, 16), (byte)System.Convert.ToInt32("0x" + colorcodeG, 16), (byte)System.Convert.ToInt32("0x" + colorcodeB, 16)));
                    }
                }
                //检查标题
                try
                {
                    int headerid = 0; string headertext = Contentlist[i + 1].insert.ToString();
                    if (headertext == "\n" && Contentlist[i + 1].attributes != null) headerid = Contentlist[i + 1].attributes.header;
                    if (headerid == 1) { run.FontSize = 20; run.FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                    if (headerid == 2) { run.FontSize = 18; run.FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                }
                catch (Exception) { }
                textBlock.Inlines.Add(run);
                run = new Run();
            }
            return (textBlock);
        }

        public RichTextBlock foldContentExtract(string objectContent)
        {
            Run foldrun = new Run();

            RichTextBlock foldtextBlock = new RichTextBlock();
            List<Paragraph> foldParagraphs = new List<Paragraph>();
            List<InlineUIContainer> inlineUIContainerList = new List<InlineUIContainer>();
            List<Windows.UI.Xaml.Controls.Image> imageList = new List<Windows.UI.Xaml.Controls.Image>();
            foldParagraphs.Add(new Paragraph());

            JArray json2 = JArray.Parse(objectContent);
            List<DocumentViewContent> Contentlist2 = json2.ToObject<List<DocumentViewContent>>();
            InsertObject insertObject2;
            bool isInsert2;
            for (int i = 0; i < Contentlist2.Count; i++)
            {
                try
                {
                     insertObject2 = JObject.Parse(Contentlist2[i].insert.ToString()).ToObject<InsertObject>();
                    isInsert2 = true;
                }
                catch (Exception)
                {
                    insertObject2 = null;
                    isInsert2 = false;
                }

                if (isInsert2)
                {
                    //foldtextBlock.TextWrapping = TextWrapping.Wrap;
                    //foldContentlistView.Items.Add(foldtextBlock);
                    if (insertObject2.image != null)
                    {
                        Windows.UI.Xaml.Controls.Image image2 = new Windows.UI.Xaml.Controls.Image();
                        BitmapImage bitmapImage = new BitmapImage(new Uri(insertObject2.image));
                        image2.Source = bitmapImage;
                        image2.Stretch = Stretch.UniformToFill;
                        image2.Tapped += new TappedEventHandler(DetailPageImageView);
                        image2.Tag = bitmapImage;
                        //foldContentlistView.Items.Add(image2);
                        inlineUIContainerList.Add(new InlineUIContainer());
                        inlineUIContainerList[inlineUIContainerList.Count - 1].Child= image2;
                        foldParagraphs[foldParagraphs.Count - 1].Inlines.Add(inlineUIContainerList[inlineUIContainerList.Count - 1]);foldParagraphs.Add(new Paragraph());

                    }
                    if (insertObject2.vod != null)
                    {
                        MediaElement foldmediaElement = new MediaElement();
                        foldmediaElement.Source = new Uri(insertObject2.vod.resolutions[insertObject2.vod.resolutions.Count - 1].url);
                        foldmediaElement.AreTransportControlsEnabled = true;
                        foldmediaElement.AutoPlay = false;
                        //foldContentlistView.Items.Add(foldmediaElement);
                        inlineUIContainerList.Add(new InlineUIContainer());
                        inlineUIContainerList[inlineUIContainerList.Count - 1].Child = foldmediaElement;
                        foldParagraphs[foldParagraphs.Count - 1].Inlines.Add(inlineUIContainerList[inlineUIContainerList.Count - 1]);foldParagraphs.Add(new Paragraph()); 
                    }

                    if (insertObject2.divider != null)
                    {
                        Windows.UI.Xaml.Controls.Image dividerimage = new Windows.UI.Xaml.Controls.Image();
                        switch (insertObject2.divider)
                        {
                            case "line_1": dividerimage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_1.png")); break;
                            case "line_2": dividerimage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_2.png")); break;
                            case "line_3": dividerimage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_3.png")); break;
                            case "line_4": dividerimage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_4.png")); break;
                            default: dividerimage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/divider_line_2.png")); break;
                        }
                        dividerimage.Stretch = Stretch.None;

                        inlineUIContainerList.Add(new InlineUIContainer());
                        inlineUIContainerList[inlineUIContainerList.Count - 1].Child = dividerimage;
                        foldParagraphs[foldParagraphs.Count - 1].Inlines.Add(inlineUIContainerList[inlineUIContainerList.Count - 1]);foldParagraphs.Add(new Paragraph()); 
                    }
                    if (insertObject2.mention != null)
                    {
                        Button mentionbutton = new Button();
                        Grid mentionbtngrid = new Grid();
                        mentionbutton.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 199, 217, 255));
                        mentionbutton.Width = double.NaN; mentionbutton.Height = 50; mentionbutton.MinWidth = 300; mentionbutton.Padding = new Thickness(0, 0, 0, 0);
                        mentionbutton.HorizontalContentAlignment = HorizontalAlignment.Left; mentionbutton.VerticalContentAlignment = VerticalAlignment.Top;
                        TextBlock mentionbtnblock1 = new TextBlock();
                        mentionbtnblock1.Text = "@" + insertObject2.mention.nickname;
                        mentionbtnblock1.VerticalAlignment = VerticalAlignment.Top; mentionbtnblock1.HorizontalAlignment = HorizontalAlignment.Left;
                        mentionbtnblock1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); mentionbtnblock1.FontSize = 18;
                        mentionbtnblock1.Margin = new Thickness(45, 13, 10, 0);

                        //grid.Children.Add(ellipse);
                        mentionbtngrid.Children.Add(mentionbtnblock1);
                        mentionbutton.Content = mentionbtngrid;
                        mentionbutton.Tag = insertObject2.mention.uid;
                        mentionbutton.Click += new RoutedEventHandler(UserButtonPressed);
                        //foldContentlistView.Items.Add(mentionbutton);

                        inlineUIContainerList.Add(new InlineUIContainer());
                        inlineUIContainerList[inlineUIContainerList.Count - 1].Child = mentionbutton;
                        foldParagraphs[foldParagraphs.Count - 1].Inlines.Add(inlineUIContainerList[inlineUIContainerList.Count - 1]);foldParagraphs.Add(new Paragraph()); 
                    }
                    if (insertObject2.link_card != null)
                    {
                        Button button = new Button();
                        Grid grid = new Grid();
                        button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                        button.Width = double.NaN; button.Height = 90; ; button.Padding = new Thickness(0, 0, 0, 0);
                        button.HorizontalContentAlignment = HorizontalAlignment.Left; button.VerticalContentAlignment = VerticalAlignment.Top;
                        Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                        ImageBrush image1 = new ImageBrush();
                        image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                        image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                        ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                        TextBlock block1 = new TextBlock();
                        TextBlock block2 = new TextBlock();
                        block1.Text = "[链接卡片]" + insertObject2.link_card.title;
                        block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                        block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                        block1.Margin = new Thickness(45, 13, 10, 0);
                        block2.Text = insertObject2.link_card.origin_url;
                        block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                        block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                        block2.Margin = new Thickness(10, 50, 10, 0);

                        grid.Children.Add(ellipse);
                        grid.Children.Add(block1);
                        grid.Children.Add(block2);
                        button.Content = grid;
                        button.Tag = insertObject2.link_card.landing_url;
                        button.Click += new RoutedEventHandler(LinkButtonPressed);
                        //foldContentlistView.Items.Add(button);

                        inlineUIContainerList.Add(new InlineUIContainer());
                        inlineUIContainerList[inlineUIContainerList.Count - 1].Child = button;
                        foldParagraphs[foldParagraphs.Count - 1].Inlines.Add(inlineUIContainerList[inlineUIContainerList.Count - 1]);foldParagraphs.Add(new Paragraph()); 
                    }
                }
                else
                {
                    foldrun.Text = Contentlist2[i].insert.ToString();
                    foldrun.FontSize = 16;
                    if (Contentlist2[i].attributes != null)
                    {
                        if (Contentlist2[i].attributes.bold != null) { foldrun.FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                        if (Contentlist2[i].attributes.italic != null) { foldrun.FontStyle = Windows.UI.Text.FontStyle.Italic; }

                        if (!string.IsNullOrEmpty(Contentlist2[i].attributes.color) && Contentlist2[i].attributes.color.Substring(0, 1) == "#")
                        {
                            string colorcodeR = Contentlist2[i].attributes.color.Substring(1, 2);
                            string colorcodeG = Contentlist2[i].attributes.color.Substring(3, 2);
                            string colorcodeB = Contentlist2[i].attributes.color.Substring(5, 2);
                            foldrun.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)System.Convert.ToInt32("0x" + colorcodeR, 16), (byte)System.Convert.ToInt32("0x" + colorcodeG, 16), (byte)System.Convert.ToInt32("0x" + colorcodeB, 16)));
                        }
                        if (!string.IsNullOrEmpty(Contentlist2[i].attributes.link))
                        {
                            Button linkbtnbutton = new Button();
                            Grid linkbtngrid = new Grid();
                            linkbtnbutton.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(178, 255, 232, 199));
                            linkbtnbutton.Width = double.NaN; linkbtnbutton.Height = 90; ; linkbtnbutton.Padding = new Thickness(0, 0, 0, 0);
                            linkbtnbutton.HorizontalContentAlignment = HorizontalAlignment.Left; linkbtnbutton.VerticalContentAlignment = VerticalAlignment.Top;
                            Windows.UI.Xaml.Shapes.Ellipse ellipse = new Ellipse();
                            ImageBrush image1 = new ImageBrush();
                            image1.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Content/type_link.png"));
                            image1.Stretch = Stretch.UniformToFill; image1.Opacity = 0.6;
                            ellipse.Width = 30; ellipse.Height = 30; ellipse.Fill = image1; ellipse.VerticalAlignment = VerticalAlignment.Top; ellipse.HorizontalAlignment = HorizontalAlignment.Left; ellipse.Margin = new Thickness(10, 10, 0, 0);

                            TextBlock block1 = new TextBlock();
                            TextBlock block2 = new TextBlock();
                            block1.Text = "[链接]" + Contentlist2[i].insert.ToString();
                            block1.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                            block1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); block1.FontSize = 18;
                            block1.Margin = new Thickness(45, 13, 10, 0);
                            block2.Text = Contentlist2[i].attributes.link;
                            block2.VerticalAlignment = VerticalAlignment.Top; block1.HorizontalAlignment = HorizontalAlignment.Stretch;
                            block2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 117, 117, 117)); block1.FontSize = 18;
                            block2.Margin = new Thickness(10, 50, 10, 0);

                            linkbtngrid.Children.Add(ellipse);
                            linkbtngrid.Children.Add(block1);
                            linkbtngrid.Children.Add(block2);
                            linkbtnbutton.Content = linkbtngrid;
                            linkbtnbutton.Tag = Contentlist2[i].attributes.link;
                            linkbtnbutton.Click += new RoutedEventHandler(LinkButtonPressed);
                            //foldContentlistView.Items.Add(linkbtnbutton);

                            foldParagraphs.Add(new Paragraph()); inlineUIContainerList.Add(new InlineUIContainer());
                            inlineUIContainerList[inlineUIContainerList.Count - 1].Child = linkbtnbutton;
                            foldParagraphs[foldParagraphs.Count - 1].Inlines.Add(inlineUIContainerList[inlineUIContainerList.Count - 1]);
                        }
                    }               
                    if (Contentlist2[i].insert.ToString() == "\n")
                    {
                        foldParagraphs[foldParagraphs.Count - 1].Inlines.Add(foldrun); foldParagraphs.Add(new Paragraph());
                        //检查标题
                        if (Contentlist2[i].attributes != null)
                        {
                            if (Contentlist2[i].attributes.header == 1) { foldParagraphs[foldParagraphs.Count - 1].FontSize = 20; foldParagraphs[foldParagraphs.Count - 1].FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                            else { foldParagraphs[foldParagraphs.Count - 1].FontSize = 18; foldParagraphs[foldParagraphs.Count - 1].FontWeight = Windows.UI.Text.FontWeights.SemiBold; }
                        }
                    }
                    else
                        foldParagraphs[foldParagraphs.Count - 1].Inlines.Add(foldrun);
                    foldrun = new Run();
                }
            }
            for(int i = 0;i<foldParagraphs.Count();i++)
            {
                foldtextBlock.Blocks.Add(foldParagraphs[i]);
            }
            return (foldtextBlock);
        }
        public void UserButtonPressed(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;      
            Frame page = PostDetailPanel.PostDetailPage.Frame;
            page.Navigate(typeof(UserDetailPanel), button.Tag.ToString());
        }
        public void LinkButtonPressed(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Frame page = PostDetailPanel.PostDetailPage.Frame;
            string uri = button.Tag.ToString();
            bool isLinkResponded = false;
            if (uri.Contains("bbs.mihoyo.com/") || uri.Contains("miyoushe.com")) //判断是否从小心海助手内部打开
            {
                if (uri.Contains("article/")) {
                    uri = uri.Substring(uri.IndexOf("article/") + 8);
                    if(uri.Contains("?"))uri = uri.Substring(0, uri.IndexOf("?"));
                    page.Navigate(typeof(PostDetailPanel), uri); isLinkResponded = true; 
                }
                if (uri.Contains("accountCenter/")) { page.Navigate(typeof(UserDetailPanel), uri.Substring(uri.IndexOf("id=") + 3)); isLinkResponded = true; }
                if (!isLinkResponded) page.Navigate(typeof(ToolPanelInsideWebView), button.Tag.ToString());
            }
            else page.Navigate(typeof(ToolPanelInsideWebView), button.Tag.ToString());
        }

        public async static Task<int[]> GetVotesResult2(string vote_id, string uid) //拿纯数字做变量名的真的是天才，反序列化死都不生效，此函数用于手动反序列化
        {
            Uri uri = new Uri("https://bbs-api.miyoushe.com/apihub/api/getVotesResult?owner_uid=" + uid + "&vote_ids=" + vote_id);
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);         
            var result = await responce.Content.ReadAsStringAsync();
            int[] votes= new int[51];

            int charA = result.IndexOf("option_stats") + 15;
            int charB = result.IndexOf("}", charA);
            string jsonObj = result.Substring(charA, charB - charA)+",";

            while (jsonObj != "")
            {
                charA = jsonObj.IndexOf("\"") + 1;
                charB = jsonObj.IndexOf("\"", charA);
                int numData = int.Parse(jsonObj.Substring(charA, charB - charA));
                charA = jsonObj.IndexOf(":", charB) + 1;
                charB = jsonObj.IndexOf(",", charA);
                int numCount = int.Parse(jsonObj.Substring(charA, charB - charA));
                votes[numData] = numCount;
                jsonObj = jsonObj.Substring(charB + 1);
            }

            return votes;
        }
    }



    public class DocumentViewContent
    {
        public object insert { get; set; }
        public Attributes attributes { get; set; }
    }
    public class Attributes
    {
        //when insert images
        public int height { get; set; }
        public int width { get; set; }
        public int size { get; set; }
        public string ext { get; set; }
        //when insert texts
        public bool? bold { get; set; }
        public bool? italic { get; set; }
        public string color { get; set; }
        public int header { get; set; }
        public string link { get; set; }
    }
    public class VoteDetail
    {
        public string id { get; set; }
        public string uid { get; set; }
    }
    public class VillaCard
    {
        public string villa_id {  get; set; }
        public string villa_name { get; set; }
        public string villa_avatar_url { get; set; }
        public string villa_cover {  get; set; }
        public string owner_uid {  get; set; }
        public string owner_nickname { get; set;}
        public string owner_avatar_url { get;set; }
        public string villa_introduce {  get; set; }
        public List<string> tag_list { get; set; }
        public string villa_member_num {  get; set; }
        public bool is_available {  get; set; }
    }
    public class Lottery
    {
        public string id { get; set; }
        public string toast { get; set; }
    }
    public class LinkCard
    {
        public int link_type { get; set; }
        public string origin_url {  get; set; }
        public string landing_url {  get; set; }
        public string cover {  get; set; }
        public string title {  get; set; }
        public string card_id {  get; set; }
        public int card_status {  get; set; }
        public int landing_url_type {  get; set; }
    }
    public class Mention
    {
        public int uid { get; set; }
        public string nickname { get; set; }
    }
    public class FoldObject
    {
        public string title { get; set; }
        public string content { get; set; }
        public string id { set; get; }
        public string size { set; get; }
    }
    public class VillaForwardCardObject
    {
        public string room_id { get; set; }
        public string room_name { get; set; }
        public string room_type { get; set; }
        public string villa_id { get; set; }
        public string villa_name { get; set; }
        public string villa_avatar_url { get; set; }
        public string forward_id { get; set; }
        public string active_member_num { get; set; }
        public List<string> active_user_avatar { get; set; }
    }
    public class VillaAvatarAction
    {
        public string url { get; set; }
        public string text { get; set; }
        public string target_user_id { get; set; }
        public string action_id { get; set; }
        public int participate_num { get; set; }
        public string target_user_nickname { get; set; }
        public string action_name { get; set; }
    }
    public class InsertObject
    {
        public string divider {  get; set; }//insert divider
        public string image { get; set; }//insert images
        public Vodplay vod { get; set; } //insert video
        public VoteDetail vote { get; set; } //insert vote
        public VillaCard villa_card { get; set; }//insert villa card (not support yet)
        public string backup_text {  get; set; }
        public Lottery lottery { get; set; }
        public LinkCard link_card {  get; set; }
        public Mention mention { get; set; }
        public FoldObject fold { get; set; }
        public VillaForwardCardObject villa_forward_card { get; set; }
        public VillaAvatarAction villa_avatar_action { get; set; }
    }


}
