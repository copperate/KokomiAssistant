using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
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
        public SettingsPanel()
        {
            this.InitializeComponent();
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
                BitmapImage bitmapImage = new BitmapImage();
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                await bitmapImage.SetSourceAsync(stream);
                Custombg.Source = bitmapImage;

                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFolder folder = await storageFolder.CreateFolderAsync("Background", CreationCollisionOption.OpenIfExists);
                Windows.Storage.StorageFile savedPic = await folder.CreateFileAsync("UserSetWallpaper.png", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                
                RenderTargetBitmap bitmap = new RenderTargetBitmap();
                await bitmap.RenderAsync(Custombg);
                var pixelBuffer = await bitmap.GetPixelsAsync();
                using (var fileStream = await savedPic.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                        BitmapAlphaMode.Ignore,
                                        (uint)bitmap.PixelWidth,
                                        (uint)bitmap.PixelHeight,
                                        DisplayInformation.GetForCurrentView().LogicalDpi,
                                        DisplayInformation.GetForCurrentView().LogicalDpi,
                                        pixelBuffer.ToArray());
                    await encoder.FlushAsync();
                }

                SettingAppearenceBackgroundChooseGrid.SelectedIndex = -1;
            }
            
        }

    }
}
