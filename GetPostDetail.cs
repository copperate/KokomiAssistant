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
    class PostDetail
    {
        public async static Task<DetailRoot> GetPostDetail(int postID)
        {
            Uri uri = new Uri("https://bbs-api.miyoushe.com/post/api/getPostFull?post_id=" + postID);
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);          //TODO:增加离线逻辑
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(DetailRoot));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (DetailRoot)serializer.ReadObject(ms);

            return data;
        }
    }

    [DataContract]
    public class DetailCertification
    {
        [DataMember]
        public int type { get; set; }
        [DataMember]
        public string label { get; set; }
    }

    [DataContract]
    public class DetailCover
    {
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public int height { get; set; }
        [DataMember]
        public int width { get; set; }
        [DataMember]
        public string format { get; set; }
        [DataMember]
        public string size { get; set; }
        [DataMember]
        public DetailCrop crop { get; set; }
        [DataMember]
        public bool is_user_set_cover { get; set; }
        [DataMember]
        public string image_id { get; set; }
        [DataMember]
        public string entity_type { get; set; }
        [DataMember]
        public string entity_id { get; set; }
        [DataMember]
        public bool is_deleted { get; set; }
    }

    [DataContract]
    public class DetailCrop
    {
        [DataMember]
        public int x { get; set; }
        [DataMember]
        public int y { get; set; }
        [DataMember]
        public int w { get; set; }
        [DataMember]
        public int h { get; set; }
        [DataMember]
        public string url { get; set; }
    }

    [DataContract]
    public class DetailData
    {
        [DataMember]
        public DetailPostA post { get; set; }
    }

    [DataContract]
    public class DetailForum
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public int game_id { get; set; }
        [DataMember]
        public object forum_cate { get; set; }
    }

    [DataContract]
    public class DetailImageList
    {
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public int height { get; set; }
        [DataMember]
        public int width { get; set; }
        [DataMember]
        public string format { get; set; }
        [DataMember]
        public string size { get; set; }
        [DataMember]
        public object crop { get; set; }
        [DataMember]
        public bool is_user_set_cover { get; set; }
        [DataMember]
        public string image_id { get; set; }
        [DataMember]
        public string entity_type { get; set; }
        [DataMember]
        public string entity_id { get; set; }
        [DataMember]
        public bool is_deleted { get; set; }
    }

    [DataContract]
    public class DetailLevelExp
    {
        [DataMember]
        public int level { get; set; }
        [DataMember]
        public int exp { get; set; }
    }

    [DataContract]
    public class DetailPostA
    {
        [DataMember]
        public DetailPostB post { get; set; }
        [DataMember]
        public DetailForum forum { get; set; }
        [DataMember]
        public List<DetailTopic> topics { get; set; }
        [DataMember]
        public DetailUser user { get; set; }
        [DataMember]
        public DetailSelfOperation self_operation { get; set; }
        [DataMember]
        public DetailStat stat { get; set; }
        [DataMember]
        public object help_sys { get; set; }
        [DataMember]
        public DetailCover cover { get; set; }
        [DataMember]
        public List<DetailImageList> image_list { get; set; }
        [DataMember]
        public bool is_official_master { get; set; }
        [DataMember]
        public bool is_user_master { get; set; }
        [DataMember]
        public bool hot_reply_exist { get; set; }
        [DataMember]
        public int vote_count { get; set; }
        [DataMember]
        public int last_modify_time { get; set; }
        [DataMember]
        public string recommend_type { get; set; }
        [DataMember]
        public object collection { get; set; }
        [DataMember]
        public List<object> vod_list { get; set; }
        [DataMember]
        public bool is_block_on { get; set; }
        [DataMember]
        public object forum_rank_info { get; set; }
        [DataMember]
        public List<object> link_card_list { get; set; }
        [DataMember]
        public object news_meta { get; set; }
    }
    [DataContract]
    public class DetailPostB
    { 
        [DataMember]
        public int game_id { get; set; }
        [DataMember]
        public string post_id { get; set; }
        [DataMember]
        public int f_forum_id { get; set; }
        [DataMember]
        public string uid { get; set; }
        [DataMember]
        public string subject { get; set; }
        [DataMember]
        public string content { get; set; }
        [DataMember]
        public int view_type { get; set; }
        [DataMember]
        public int created_at { get; set; }
        [DataMember]
        public List<string> images { get; set; }
        [DataMember]
        public DetailPostStatus post_status { get; set; }
        [DataMember]
        public List<int> topic_ids { get; set; }
        [DataMember]
        public int view_status { get; set; }
        [DataMember]
        public int max_floor { get; set; }
        [DataMember]
        public int is_original { get; set; }
        [DataMember]
        public int republish_authorization { get; set; }
        [DataMember]
        public string reply_time { get; set; }
        [DataMember]
        public int is_deleted { get; set; }
        [DataMember]
        public bool is_interactive { get; set; }
        [DataMember]
        public string structured_content { get; set; }
        [DataMember]
        public List<object> structured_content_rows { get; set; }
        [DataMember]
        public int review_id { get; set; }
        [DataMember]
        public bool is_profit { get; set; }
        [DataMember]
        public bool is_in_profit { get; set; }
        [DataMember]
        public int updated_at { get; set; }
        [DataMember]
        public int deleted_at { get; set; }
        [DataMember]
        public int pre_pub_status { get; set; }
        [DataMember]
        public int cate_id { get; set; }
        [DataMember]
        public int profit_post_status { get; set; }
        [DataMember]
        public int audit_status { get; set; }
        [DataMember]
        public string meta_content { get; set; }
        [DataMember]
        public bool is_missing { get; set; }
        [DataMember]
        public int block_reply_img { get; set; }
        [DataMember]
        public bool is_showing_missing { get; set; }
        [DataMember]
        public int block_latest_reply_time { get; set; }
        [DataMember]
        public int selected_comment { get; set; }
    }

    [DataContract]
    public class DetailPostStatus
    {
        [DataMember]
        public bool is_top { get; set; }
        [DataMember]
        public bool is_good { get; set; }
        [DataMember]
        public bool is_official { get; set; }
    }

    

    [DataContract]
    public class DetailSelfOperation
    {
        [DataMember]
        public int attitude { get; set; }
        [DataMember]
        public bool is_collected { get; set; }
    }

    [DataContract]
    public class DetailStat
    {
        [DataMember]
        public int view_num { get; set; }
        [DataMember]
        public int reply_num { get; set; }
        [DataMember]
        public int like_num { get; set; }
        [DataMember]
        public int bookmark_num { get; set; }
        [DataMember]
        public int forward_num { get; set; }
    }

    [DataContract]
    public class DetailTopic
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string cover { get; set; }
        [DataMember]
        public bool is_top { get; set; }
        [DataMember]
        public bool is_good { get; set; }
        [DataMember]
        public bool is_interactive { get; set; }
        [DataMember]
        public int game_id { get; set; }
        [DataMember]
        public int content_type { get; set; }
    }


    [DataContract]
    public class DetailUser
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
        public DetailCertification certification { get; set; }
        [DataMember]
        public DetailLevelExp level_exp { get; set; }
        [DataMember]
        public bool is_following { get; set; }
        [DataMember]
        public bool is_followed { get; set; }
        [DataMember]
        public string avatar_url { get; set; }
        [DataMember]
        public string pendant { get; set; }
    }

    [DataContract]
    public class DetailRoot
    {
        [DataMember]
        public int retcode { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public DetailData data { get; set; }
    }
}
