using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;


// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPanel : Page
    {
        bool isReady = false;
        public SettingsPanel()
        {
            this.InitializeComponent();
            SettingsInit();
        }


        private void SettingAppearenceBackgroundLoad(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            String selectedIndex = localSettings.Values["Background"] as string;
            int selectedItem;
            if (selectedIndex == "ms-appdata:///local/Background/UserSetWallpaper.png") 
            { 
                selectedItem = 4; 
            }
            else if (selectedIndex == "ms-appx:///Assets/Background/DefaultHonkaiWallpaper.png") 
            { 
                selectedItem = 1; 
            }
            else
            {
                if (selectedIndex == "ms-appx:///Assets/Background/DefaultStarrailWallpaper.png")
                { 
                    selectedItem = 2; 
                }
                else if (selectedIndex == "ms-appx:///Assets/Background/DefaultGenshinWallpaper.jpg")
                {
                    selectedItem = 3;
                }
                else
                {
                    selectedItem = 0;
                }
            }
            if (storageFolder != null)
            {
                Custombg.Source= new BitmapImage(new Uri("ms-appdata:///local/Background/UserSetWallpaper.png"));
            }
            else
            {
                Custombg.Source = new BitmapImage(new Uri("ms-appx:///Assets/Content/Add_bg_Temp.png"));
            }
            SettingAppearenceBackgroundChooseGrid.SelectedIndex = selectedItem; 
        }

        private void SettingAppearenceBackgroundSet(object sender, SelectionChangedEventArgs e)
        {
            ApplicationDataContainer StatusPanelSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            int selectedIndex = SettingAppearenceBackgroundChooseGrid.SelectedIndex;
            switch (selectedIndex)
            {
                case 0: StatusPanelSettings.Values["Background"] = "ms-appx:///Assets/Background/Wallpaper01.png"; break;
                case 1: StatusPanelSettings.Values["Background"] = "ms-appx:///Assets/Background/DefaultHonkaiWallpaper.png"; break;
                case 2: StatusPanelSettings.Values["Background"] = "ms-appx:///Assets/Background/DefaultStarrailWallpaper.png"; break;
                case 3: StatusPanelSettings.Values["Background"] = "ms-appx:///Assets/Background/DefaultGenshinWallpaper.jpg"; break;
                case 4: StatusPanelSettings.Values["Background"] = "ms-appdata:///local/Background/UserSetWallpaper.png"; break;
            }
        }

        private void SettingsAppearenceBackgroundsSetButton(object sender, RoutedEventArgs e)
        {
            SavePicUpdate();
           
        }

        private async void SavePicUpdate()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                // The user cancelled the picking operation
                return;
            }
            else
            {
                
                var stream = await file.OpenReadAsync();
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var bitmapImage = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                stream.Seek(0);
                await bitmapImage.SetSourceAsync(stream);
                stream.Dispose();
                Custombg.Source = bitmapImage;

                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFolder folder = await storageFolder.CreateFolderAsync("Background", CreationCollisionOption.OpenIfExists);
                var savedPic = await folder.CreateFileAsync("UserSetWallpaper.png", CreationCollisionOption.ReplaceExisting);

                var writeStream = new InMemoryRandomAccessStream();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, writeStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)bitmapImage.PixelWidth, (uint)bitmapImage.PixelHeight, 96, 96, bitmapImage.PixelBuffer.ToArray());
                await encoder.FlushAsync();
                writeStream.Seek(0);
                var imgFileOut = await savedPic.OpenStreamForWriteAsync();
                var fileProxyStream = writeStream.AsStreamForRead();
                await fileProxyStream.CopyToAsync(imgFileOut);
                await imgFileOut.FlushAsync();
                fileProxyStream.Dispose();
                imgFileOut.Dispose();
                
                SettingAppearenceBackgroundChooseGrid.SelectedIndex = -1;
            }
            
        }

        private async void AboutpageOpenDataFolderBtn(object sender, RoutedEventArgs e)
        {
            var t = new FolderLauncherOptions();
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            await Launcher.LaunchFolderAsync(folder, t);
        }

        private async void AppLocation_Genshin_BrowseButton(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".exe");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if(file == null)
            {
                return;
            }
            else
            {
                string filename = file.Name;
                if((filename== "YuanShen.exe")||(filename== "GenshinImpact.exe"))
                {
                    AppLocationGenshin.Text = file.Path;
                    ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    localSettings.Values["AppLocationGenshin"]=file.Path;
                }
                else
                {
                    if(filename== "launcher.exe")
                    {
                        ((Window.Current.Content as Frame).Content as MainPage).NotifyPane_Activated("小心海助手识别到游戏启动器；已更改为游戏本体。");

                    }
                    else
                    {
                        ((Window.Current.Content as Frame).Content as MainPage).NotifyPane_Activated("请选择游戏本体（GenshinImpact.exe/YuanShen.exe）或者启动器（launcher.exe）；小心海助手无法识别当前所选择的文件。");
                        return;
                    }
                }
                
            }
        }

        public void RefreshSettings_Blockwords()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            List<string> BrowsePage_BlockwordList = new List<string>();
            string blockwords = "";
            if (localSettings.Values["BrowseSettings_Blockwords"]!=null) 
                blockwords= localSettings.Values["BrowseSettings_Blockwords"].ToString();
            while(blockwords!=""&&blockwords!=null)
            {
                int sepchar=blockwords.IndexOf(",");
                BrowsePage_BlockwordList.Add(blockwords.Substring(0, sepchar));
                blockwords=blockwords.Substring(sepchar+1);
            }
            BrowsePage_BlockwordsList.Items.Clear();
            int BlockwordNum;
            if (BrowsePage_BlockwordList == null) BlockwordNum = 0; else BlockwordNum = BrowsePage_BlockwordList.Count;
            BrowsePage_BlockwordsListTitle.Text = "已启用关键词 (" + BlockwordNum.ToString() + ")";
            for (int i = 0; i < BlockwordNum; i++)
            {
                Grid blockwordgrid = new Grid();
                AppBarButton BlockWordDeleteButton = new AppBarButton();
                BlockWordDeleteButton.Icon = new SymbolIcon(Symbol.Cancel);
                BlockWordDeleteButton.Width = 40; BlockWordDeleteButton.Height = 40; BlockWordDeleteButton.Margin = new Thickness(-10, -10, -10, -10);
                TextBlock BlockwordBlock = new TextBlock();
                BlockwordBlock.Text = BrowsePage_BlockwordList[i];
                BlockwordBlock.Margin = new Thickness(30, 0, 0, 0); BlockwordBlock.VerticalAlignment = VerticalAlignment.Center;
                blockwordgrid.Children.Add(BlockwordBlock);
                blockwordgrid.Children.Add(BlockWordDeleteButton);
                BrowsePage_BlockwordsList.Items.Add(blockwordgrid);
            }
        }

        public void SettingsInit()
        {
            isReady = true;
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            //外观&主页

            //速览&捷径
                if (localSettings.Values["AppLocationGenshin"] != null && (string)localSettings.Values["AppLocationGenshin"] != "")
                {
                    AppLocationGenshin.Text = localSettings.Values["AppLocationGenshin"].ToString();
                }
                else AppLocationGenshin.Text = "%Application_Path%";
                if (localSettings.Values["AppLocationHonkai3"] != null && (string)localSettings.Values["AppLocationHonkai3"] != "")
                {
                    AppLocationHonkai3.Text = localSettings.Values["AppLocationHonkai3"].ToString();
                }
                else AppLocationHonkai3.Text = "%Application_Path%";
                if (localSettings.Values["AppLocationStarrail"] != null && (string)localSettings.Values["AppLocationStarrail"] != "")
                {
                    AppLocationStarrail.Text = localSettings.Values["AppLocationStarrail"].ToString();
                }
                else AppLocationStarrail.Text = "%Application_Path%";
            //用户&关联

            //浏览&功能
            if (localSettings.Values["BrowseSettings_BlockEnabled"] != null && (bool)localSettings.Values["BrowseSettings_BlockEnabled"] == true)
            {
                BlockSettingsGrid.Opacity = 1;
                BrowsePage_Blocksettingswitch.IsOn = true;
            }
            else
            {
                BlockSettingsGrid.Opacity = 0.5;
                BrowsePage_Blocksettingswitch.IsOn = false;
            }
            RefreshSettings_Blockwords();

            //关于此应用


        }

        private void BrowsePage_BlockSwitch(object sender, RoutedEventArgs e)
        {
            if (!isReady) return;
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch.IsOn)
            {
                BlockSettingsGrid.Opacity = 1;
                localSettings.Values["BrowseSettings_BlockEnabled"] = true;
            }
            else
            {
                BlockSettingsGrid.Opacity = 0.5;
                localSettings.Values["BrowseSettings_BlockEnabled"] = false;
            }
        }

        private void BrowsePage_AddBlockword(object sender, TappedRoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string BrowsePage_BlockwordList;
            if (localSettings.Values["BrowseSettings_Blockwords"] != null)
                BrowsePage_BlockwordList = localSettings.Values["BrowseSettings_Blockwords"].ToString();
            else BrowsePage_BlockwordList = "";
            if(AddblockwordTextbox.Text != null && AddblockwordTextbox.Text != "") 
            {
                BrowsePage_BlockwordList = BrowsePage_BlockwordList + AddblockwordTextbox.Text + ",";
                localSettings.Values["BrowseSettings_Blockwords"] = BrowsePage_BlockwordList;
                AddblockwordTextbox.Text = "";
                RefreshSettings_Blockwords();
            }
            else ((Window.Current.Content as Frame).Content as MainPage).NotifyPane_Activated("请输入一个有效词。");
        }

        private void BrowsePage_ClearBlockword(object sender, TappedRoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["BrowseSettings_Blockwords"] = "";
            RefreshSettings_Blockwords();
        }

        private void Aboutpage_SourceLink(object sender, TappedRoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri(@"https://github.com/copperate/KokomiAssistant"));
        }
    }
}
