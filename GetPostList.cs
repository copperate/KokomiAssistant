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
    class PostList
    {
        public async static Task<RootObject> GetPostList(int channelID,int pageNum,int sortType)
        {
            Uri uri;
            if (sortType == 3)
            {
                uri = new Uri("https://api-takumi.miyoushe.com/post/api/getForumPostList?forum_id=" + channelID + "&is_good=false&is_hot=true&page_size=20&sort_type=&page=" + pageNum);
            }
            else
            {
                uri = new Uri("https://api-takumi.miyoushe.com/post/api/getForumPostList?forum_id=" + channelID + "&is_good=false&is_hot=false&page_size=20&sort_type=" + sortType + "&page=" + pageNum);
            }
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce =await client.GetAsync(uri);
            var result=await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RootObject));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data=(RootObject)serializer.ReadObject(ms);

            return data;
        }
    }
    [DataContract]
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Certification
    {
        [DataMember]
        public int type { get; set; }
        [DataMember]
        public string label { get; set; }
    }
    [DataContract]
    public class Cover
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
    public class Data
    {
        [DataMember]
        public List<List> list { get; set; }
        [DataMember]
        public string last_id { get; set; }
        [DataMember]
        public bool is_last { get; set; }
        [DataMember]
        public bool is_origin { get; set; }
    }
    [DataContract]
    public class Forum
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
    public class HelpSys
    {
        [DataMember]
        public object top_up { get; set; }
        [DataMember]
        public List<object> top_n { get; set; }
        [DataMember]
        public int answer_num { get; set; }
    }
    [DataContract]
    public class ImageList
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
    public class LevelExp
    {
        [DataMember]
        public int level { get; set; }
        [DataMember]
        public int exp { get; set; }
    }
    [DataContract]
    public class List
    {
        [DataMember]
        public Post post { get; set; }
        [DataMember]
        public Forum forum { get; set; }
        [DataMember]
        public List<Topic> topics { get; set; }
        [DataMember]
        public User user { get; set; }
        [DataMember]
        public SelfOperation self_operation { get; set; }
        [DataMember]
        public Stat stat { get; set; }
        [DataMember]
        public HelpSys help_sys { get; set; }
        [DataMember]
        public Cover cover { get; set; }
        [DataMember]
        public List<ImageList> image_list { get; set; }
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
        public List<VodList> vod_list { get; set; }
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
    public class Post
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
        public string cover { get; set; }
        [DataMember]
        public int view_type { get; set; }
        [DataMember]
        public int created_at { get; set; }
        [DataMember]
        public List<string> images { get; set; }
        [DataMember]
        public PostStatus post_status { get; set; }
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
    public class PostStatus
    {
        [DataMember]
        public bool is_top { get; set; }
        [DataMember]
        public bool is_good { get; set; }
        [DataMember]
        public bool is_official { get; set; }
    }
    [DataContract]
    public class Resolution
    {
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public string definition { get; set; }
        [DataMember]
        public int height { get; set; }
        [DataMember]
        public int width { get; set; }
        [DataMember]
        public int bitrate { get; set; }
        [DataMember]
        public string size { get; set; }
        [DataMember]
        public string format { get; set; }
        [DataMember]
        public string label { get; set; }
    }
    [DataContract]
    public class RootObject
    {
        [DataMember]
        public int retcode { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public Data data { get; set; }
    }
    [DataContract]
    public class SelfOperation
    {
        [DataMember]
        public int attitude { get; set; }
        [DataMember]
        public bool is_collected { get; set; }
    }
    [DataContract]
    public class Stat
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
    public class Topic
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
    public class User
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
        public Certification certification { get; set; }
        [DataMember]
        public LevelExp level_exp { get; set; }
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
    public class VodList
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public int duration { get; set; }
        [DataMember]
        public string cover { get; set; }
        [DataMember]
        public List<Resolution> resolutions { get; set; }
        [DataMember]
        public int view_num { get; set; }
        [DataMember]
        public int transcoding_status { get; set; }
        [DataMember]
        public int review_status { get; set; }
        [DataMember]
        public string brief_intro { get; set; }
    }


}
