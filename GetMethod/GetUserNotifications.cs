using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Notifications;
//using Windows.Web.Http;

namespace KokomiAssistant
{
    class GetNotifications
    {
        public async static Task<NotificationsRoot> GetNotification(string last_id, string category)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            int loginmethod = int.Parse(localSettings.Values["LoginMethod"].ToString());
            Uri uri;
            if (loginmethod == 1) uri = new Uri("https://bbs-api.miyoushe.com/notification/api/getUserNotifications?category=" + category + "&page_size=20&last_id=" + last_id + "&uid=" + localSettings.Values["LoginUserID"].ToString());
            else uri = new Uri("https://bbs-api.miyoushe.com/notification/wapi/getUserGameNotifications?category=" + category + "&page_size=20&last_id=" + last_id);
            var handler = new HttpClientHandler() { UseCookies = false };
            HttpClient client = new HttpClient(handler);
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://www.miyoushe.com");
            var message = new HttpRequestMessage(HttpMethod.Get, uri);

            
            List<string> cookies = new List<string>();
            string CookieString = localSettings.Values["LoginCookies"].ToString();
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
            }
            var responce = await client.SendAsync(message);
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(NotificationsRoot));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (NotificationsRoot)serializer.ReadObject(ms);

            return data;
        }

        public async static Task<CountRoot> GetUnreadNotificationCount()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            int loginmethod = int.Parse(localSettings.Values["LoginMethod"].ToString());
            Uri uri;
            if (loginmethod == 1) uri = new Uri("https://bbs-api.miyoushe.com/notification/api/getUserUnreadCount?uid=" + localSettings.Values["LoginUserID"].ToString());
            else uri = new Uri("https://bbs-api.miyoushe.com/notification/wapi/getUserGameUnreadCount");
            var handler = new HttpClientHandler() { UseCookies = false };
            HttpClient client = new HttpClient(handler);

            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://www.miyoushe.com");
            var message = new HttpRequestMessage(HttpMethod.Get, uri);

            
            List<string> cookies = new List<string>();
            string CookieString = localSettings.Values["LoginCookies"].ToString();
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
            }
            var responce = await client.SendAsync(message);
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(CountRoot));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (CountRoot)serializer.ReadObject(ms);

            return data;
        }
        public async static Task<CountRoot> ClearUnreadNotifications()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            int loginmethod = int.Parse(localSettings.Values["LoginMethod"].ToString());
            Uri uri;
            if (loginmethod == 1) uri = new Uri("https://bbs-api.miyoushe.com/notification/api/clearUserUnread");
            else uri = new Uri("https://bbs-api.miyoushe.com/notification/wapi/clearUserGameUnread");
            var handler = new HttpClientHandler() { UseCookies = false };
            HttpClient client = new HttpClient(handler);

            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://www.miyoushe.com");
            var message = new HttpRequestMessage(HttpMethod.Get, uri);

            List<string> cookies = new List<string>();
            string CookieString = localSettings.Values["LoginCookies"].ToString();
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
            }
            var responce = await client.SendAsync(message);
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(CountRoot));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            CountRoot data= new CountRoot();

            return data;
        }
    }

    public class NotificationsData
    {
        public List<NotificationsList> list { get; set; }
        public string last_id { get; set; }
        public bool is_last { get; set; }
    }

    public class NotificationsExt
    {
        public string post_id { get; set; }
        public string reply_id { get; set; }
        public string image { get; set; }
        public string silent_end_time { get; set; }
        public int silent_src { get; set; }
        public bool is_silent_reduce { get; set; }
        public int delete_src { get; set; }
        public bool appeal_adopt { get; set; }
        public int appeal_type { get; set; }
        public string appeal_opinion { get; set; }
        public string del_reason { get; set; }
        public int forum_id { get; set; }
        public string forum_silent_end_time { get; set; }
        public string instant_id { get; set; }
        public string entity_type { get; set; }
        public List<object> forward_list { get; set; }
        public string app_path { get; set; }
        public string web_path { get; set; }
        public int mention_item_type { get; set; }
        public string mention_item_id { get; set; }
        public int refer_types { get; set; }
        public object site_msg_config { get; set; }
        public bool is_from_tip_delete { get; set; }
        public string highlight_string { get; set; }
        public string highlight_url { get; set; }
        public string upvote_content { get; set; }
        public string r_reply_id { get; set; }
        public string appeal_context { get; set; }
        public string recent_follow_at { get; set; }
    }

    public class NotificationsList
    {
        public string notification_id { get; set; }
        public int game_id { get; set; }
        public string uid { get; set; }
        public string op_uid { get; set; }
        public int type { get; set; }
        public string subject { get; set; }
        public string content { get; set; }
        public string detail { get; set; }
        public NotificationsExt ext { get; set; }
        public bool is_read { get; set; }
        public int created_at { get; set; }
        public int updated_at { get; set; }
        public string app_path { get; set; }
        public string web_path { get; set; }
        public Origin origin { get; set; }
        public bool is_content_deleted { get; set; }
        public NotificationsOpUser op_user { get; set; }
        public bool can_appeal { get; set; }
        public string site_title { get; set; }
        public string site_content { get; set; }
        public string button_text { get; set; }
        public bool is_from_focus_user { get; set; }
        public string structured_content { get; set; }
        public bool is_web_jump_disable { get; set; }
        public bool content_is_missing { get; set; }
        public string task_id { get; set; }
        public string site_icon { get; set; }
        public int site_msg_display_type { get; set; }
        public bool user_appeal { get; set; }
        public int schema_version { get; set; }
    }

    public class Origin
    {
        public string text { get; set; }
        public Image image { get; set; }
        public bool is_deleted { get; set; }
        public object instant { get; set; }
        public List<object> forward_list { get; set; }
        public string structured_content { get; set; }
        public bool origin_is_missing { get; set; }
        public string followed_at { get; set; }
    }

    public class NotificationsOpUser
    {
        public string uid { get; set; }
        public string nickname { get; set; }
        public string introduce { get; set; }
        public string avatar { get; set; }
        public int gender { get; set; }
        public object certification { get; set; }
        public object level_exp { get; set; }
        public string avatar_url { get; set; }
        public bool is_follow { get; set; }
        public bool is_followed { get; set; }
        public bool is_following { get; set; }
    }

    public class NotificationsRoot
    {
        public int retcode { get; set; }
        public string message { get; set; }
        public NotificationsData data { get; set; }
    }

    // CountData
    public class ChannelUnreadCount
    {
    }

    public class Count
    {
        public int follow { get; set; }
        public int villa_activity_focus { get; set; }
        public int villa_dby_focus { get; set; }
        public int villa_interaction_focus { get; set; }
        public int active_mention_focus { get; set; }
        public int mention { get; set; }
        public int villa_audit { get; set; }
        public int villa_interaction { get; set; }
        public int comment { get; set; }
        public int active_mention { get; set; }
        public int mentor_invite_focus { get; set; }
        public int villa_audit_focus { get; set; }
        public int mentor_reply { get; set; }
        public int reply { get; set; }
        public int mentor_invite { get; set; }
        public int mentor { get; set; }
        public int comment_focus { get; set; }
        public int system { get; set; }
        public int mention_focus { get; set; }
        public int reply_focus { get; set; }
        public int mentor_reply_focus { get; set; }
        public int villa_activity { get; set; }
        public int recent_follow { get; set; }
        public int recent_follow_focus { get; set; }
        public int mentor_focus { get; set; }
        public int villa_dby { get; set; }
    }

    public class CountData
    {
        public Count count { get; set; }
        public ChannelUnreadCount channel_unread_count { get; set; }
        public string user_channel_unread_count { get; set; }
    }

    public class CountRoot
    {
        public int retcode { get; set; }
        public string message { get; set; }
        public CountData data { get; set; }
    }


}
