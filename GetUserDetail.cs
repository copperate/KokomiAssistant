using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace KokomiAssistant
{
    class GetUserDetail
    {
        public async static Task<UserDetailRoot> GetUserDetailInfo(int userID)
        {
            Uri uri = new Uri("https://api-takumi.miyoushe.com/user/api/getUserFullInfo?uid=" + userID);
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);          //TODO:增加离线逻辑
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(UserDetailRoot));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (UserDetailRoot)serializer.ReadObject(ms);
            return data;
        }
    }

    //UserDetail
    [DataContract]
    public class UserAchieve
    {
        [DataMember]
        public string like_num { get; set; }
        [DataMember]
        public string post_num { get; set; }
        [DataMember]
        public string replypost_num { get; set; }
        [DataMember]
        public string follow_cnt { get; set; }
        [DataMember]
        public string followed_cnt { get; set; }
        [DataMember]
        public string topic_cnt { get; set; }
        [DataMember]
        public string new_follower_num { get; set; }
        [DataMember]
        public string good_post_num { get; set; }
        [DataMember]
        public string follow_collection_cnt { get; set; }
        [DataMember]
        public string silent_num { get; set; }
    }

    [DataContract]
    public class UserAuditInfo
    {
        [DataMember]
        public bool is_nickname_in_audit { get; set; }
        [DataMember]
        public string nickname { get; set; }
        [DataMember]
        public bool is_introduce_in_audit { get; set; }
        [DataMember]
        public string introduce { get; set; }
        [DataMember]
        public int nickname_status { get; set; }
    }

    [DataContract]
    public class UserCertification
    {
        [DataMember]
        public int type { get; set; }
        [DataMember]
        public string label { get; set; }
    }

    [DataContract]
    public class UserCertification2
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string certification_id { get; set; }
        [DataMember]
        public int type { get; set; }
        [DataMember]
        public string label { get; set; }
    }

    [DataContract]
    public class UserCommunityInfo
    {
        [DataMember]
        public bool is_realname { get; set; }
        [DataMember]
        public bool agree_status { get; set; }
        [DataMember]
        public int silent_end_time { get; set; }
        [DataMember]
        public int forbid_end_time { get; set; }
        [DataMember]
        public int info_upd_time { get; set; }
        [DataMember]
        public UserPrivacyInvisible privacy_invisible { get; set; }
        [DataMember]
        public UserNotifyDisable notify_disable { get; set; }
        [DataMember]
        public bool has_initialized { get; set; }
        [DataMember]
        public UserFuncStatus user_func_status { get; set; }
        [DataMember]
        public List<object> forum_silent_info { get; set; }
        [DataMember]
        public string last_login_ip { get; set; }
        [DataMember]
        public int last_login_time { get; set; }
        [DataMember]
        public int created_at { get; set; }
    }

    [DataContract]
    public class UserCustomerService
    {
        [DataMember]
        public bool is_customer_service_staff { get; set; }
        [DataMember]
        public int game_id { get; set; }
    }

    [DataContract]
    public class UserDetailData
    {
        [DataMember]
        public UserInfo user_info { get; set; }
        [DataMember]
        public object follow_relation { get; set; }
        [DataMember]
        public List<object> auth_relations { get; set; }
        [DataMember]
        public bool is_in_blacklist { get; set; }
        [DataMember]
        public bool is_has_collection { get; set; }
        [DataMember]
        public bool is_creator { get; set; }
        [DataMember]
        public UserCustomerService customer_service { get; set; }
        [DataMember]
        public UserAuditInfo audit_info { get; set; }
    }

    [DataContract]
    public class UserLevelExp
    {
        [DataMember]
        public int level { get; set; }
        [DataMember]
        public int exp { get; set; }
        [DataMember]
        public int game_id { get; set; }
    }

    [DataContract]
    public class UserNotifyDisable
    {
        [DataMember]
        public bool reply { get; set; }
        [DataMember]
        public bool upvote { get; set; }
        [DataMember]
        public bool follow { get; set; }
        [DataMember]
        public bool system { get; set; }
        [DataMember]
        public bool chat { get; set; }
    }

    [DataContract]
    public class UserPrivacyInvisible
    {
        [DataMember]
        public bool post { get; set; }
        [DataMember]
        public bool collect { get; set; }
        [DataMember]
        public bool watermark { get; set; }
        [DataMember]
        public bool reply { get; set; }
        [DataMember]
        public bool post_and_instant { get; set; }
    }

    [DataContract]
    public class UserDetailRoot
    {
        [DataMember]
        public int retcode { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public UserDetailData data { get; set; }
    }

    [DataContract]
    public class UserFuncStatus
    {
        [DataMember]
        public bool enable_history_view { get; set; }
        [DataMember]
        public bool enable_recommend { get; set; }
        [DataMember]
        public bool enable_mention { get; set; }
        [DataMember]
        public int user_center_view { get; set; }
        [DataMember]
        public string show_self_created_villa { get; set; }
    }

    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public string uid { get; set; }
        [DataMember]
        public string nickname { get; set; }
        [DataMember]
        public string introduce { get; set; }
        [DataMember]
        public string avatar { get; set; }
        [DataMember]
        public int gender { get; set; }
        [DataMember]
        public UserCertification certification { get; set; }
        [DataMember]
        public List<UserLevelExp> level_exps { get; set; }
        [DataMember]
        public UserAchieve achieve { get; set; }
        [DataMember]
        public UserCommunityInfo community_info { get; set; }
        [DataMember]
        public string avatar_url { get; set; }
        [DataMember]
        public List<UserCertification2> certifications { get; set; }
        [DataMember]
        public object level_exp { get; set; }
        [DataMember]
        public string pendant { get; set; }
        [DataMember]
        public bool is_logoff { get; set; }
        [DataMember]
        public string ip_region { get; set; }
    }


}
