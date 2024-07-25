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
using Windows.UI.Notifications;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.Storage;
// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KokomiAssistant
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ToolPanelDev : Page
    {
        public ToolPanelDev()
        {
            this.InitializeComponent();
        }

        private void ButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        private void TestButtonClicked2(object sender, RoutedEventArgs e)
        {
            Check_in();
            
        }
        public async void Check_in()
        {
            Uri uri = new Uri("https://bbs-api.miyoushe.com/apihub/app/api/signIn");
            var handler = new HttpClientHandler() { UseCookies = false };
            HttpClient client = new HttpClient(handler);
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var message = new HttpRequestMessage(HttpMethod.Post, uri);

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            List<string> cookies = new List<string>();
            /*string CookieString = localSettings.Values["LoginCookies"].ToString();
            while (CookieString != "")
            {
                string oneLineCookie;
                if (CookieString.IndexOf("\r") >= 0) oneLineCookie = CookieString.Substring(0, CookieString.IndexOf("\r"));
                else
                {
                    oneLineCookie = CookieString;
                    CookieString = "";
                }
                cookies.Add(oneLineCookie);
                int i = CookieString.IndexOf("\r") + 1;
                if (i <= CookieString.Length) CookieString = CookieString.Substring(i);
            }

            for (int i = 0; i < cookies.Count(); i++)
            {
                message.Headers.Add("Cookie", cookies[i]);
            }*/

            message.Headers.Add("Cookie", "stuid=31084695");
            message.Headers.Add("Cookie", "stoken=v2_uqvHWz-baNPFLlaTSB-HdcD6FG0cdLIReYDeiQuT9j5LXk0Be_f5aunyrceynSVjiDYEhRJtaedoVFCAeopynS4jdBjZ5rETCZNHcLhtQ7eqZVXKbie8Ae9D7NaLZ1k17T2U7fg4xnI4zx4ukA==.CAE=");
            message.Headers.Add("Cookie", "mid=0zjmeftv9a_mhy");

            message.Headers.Add("DS", "1720703419,196442,3734488e2eefeee2c9f6c3002c02db96");
            message.Headers.Add("x-rpc-client_type", "2");
            message.Headers.Add("x-rpc-app_version", "2.70.1");
            message.Headers.Add("x-rpc-sys_version", "10.0");
            message.Headers.Add("x-rpc-channel", "miyousheluodi");
            message.Headers.Add("x-rpc-device_id", "a639550a-eb5c-3726-9512-58874e2c962");
            message.Headers.Add("x-rpc-device_fp", "38d7fab59f6f93");
            message.Headers.Add("x-rpc-device_name", "Microsoft Lumia 950");
            message.Headers.Add("x-rpc-device_model", "Lumia 950");
            message.Headers.Add("x-rpc-verify_key", "bll8iq97cem8");
            message.Headers.Add("x-rpc-csm_source", "discussion");

            message.Content = new ByteArrayContent(Encoding.UTF8.GetBytes("{\"gids\":6}"));
            var responce = await client.SendAsync(message);
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(NotificationsRoot));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (NotificationsRoot)serializer.ReadObject(ms);

            //return data;
        }
    }
}
