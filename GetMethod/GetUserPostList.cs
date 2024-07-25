using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace KokomiAssistant
{
    class GetUserPostList
    {
        public async static Task<UserPostListObjectRoot> GetPostList_of_User(int userid)
        {
            Uri uri = new Uri("https://api-takumi.miyoushe.com/painter/api/user_instant/list?uid=" + userid+"&offset=0&size=50");
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);          //TODO:增加离线逻辑
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(UserPostListObjectRoot));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (UserPostListObjectRoot)serializer.ReadObject(ms);

            return data;
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AvatarExt
    {
        public int avatar_type { get; set; }
        public string avatar_assets_id { get; set; }
        public List<object> resources { get; set; }
        public List<object> hd_resources { get; set; }
    }

    public class UserPostListData
    {
        public bool is_last { get; set; }
        public int next_offset { get; set; }
        public List<UserPostList> list { get; set; }
        public object top_post { get; set; }
    }

    public class Instant
    {
        public Instant instant { get; set; }
        public List<object> forward_list { get; set; }
        public List<ImageList> image_list { get; set; }
        public List<TopicList> topic_list { get; set; }
        public UserPostListStat stat { get; set; }
        public UserPostListUser user { get; set; }
        public List<object> vod_list { get; set; }
        public ReferObject refer_object { get; set; }
        public object resource_info { get; set; }
        public Status status { get; set; }
        public UserPLSelfOperation self_operation { get; set; }
        public bool hot_reply_exist { get; set; }
        public bool is_block_on { get; set; }
        public List<object> link_card_list { get; set; }
        public string id { get; set; }
        public string structured_content { get; set; }
        public List<object> structured_content_rows { get; set; }
        public List<string> image_id_list { get; set; }
        public List<object> vod_id_list { get; set; }
        public List<object> forward_instant_id { get; set; }
        public List<string> topic_id_list { get; set; }
        public int refer_types { get; set; }
        public string refer_id { get; set; }
        public string uid { get; set; }
        public bool is_deleted { get; set; }
        public string created_at { get; set; }
        public string content { get; set; }
        public string review_id { get; set; }
        public int view_status { get; set; }
        public int delete_src { get; set; }
        public string deleted_at { get; set; }
        public List<object> link_card_id_list { get; set; }
        public string summary { get; set; }
        public bool is_missing { get; set; }
        public bool is_showing_missing { get; set; }
        public bool is_mentor { get; set; }
    }

    public class InstantUpvoteStat
    {
        public int upvote_type { get; set; }
        public int upvote_cnt { get; set; }
    }


    public class UserPostList
    {
        public string id { get; set; }
        public string uid { get; set; }
        public string entity_id { get; set; }
        public int entity_type { get; set; }
        public string publish_at { get; set; }
        public string game_id { get; set; }
        public UserPostListPost post { get; set; }
        public Instant instant { get; set; }
    }

    public class UserPostListPost
    {
        public UserPostListPost2 post { get; set; }
        public Forum forum { get; set; }
        public List<Topic> topics { get; set; }
        public UserPostListUser user { get; set; }
        public UserPLSelfOperation self_operation { get; set; }
        public UserPostListStat stat { get; set; }
        public object help_sys { get; set; }
        public object cover { get; set; }
        public List<ImageList> image_list { get; set; }
        public bool is_official_master { get; set; }
        public bool is_user_master { get; set; }
        public bool hot_reply_exist { get; set; }
        public int vote_count { get; set; }
        public int last_modify_time { get; set; }
        public string recommend_type { get; set; }
        public object collection { get; set; }
        public List<object> vod_list { get; set; }
        public bool is_block_on { get; set; }
        public object forum_rank_info { get; set; }
        public bool is_mentor { get; set; }
    }
    public class UserPostListPost2
    { 
        public int game_id { get; set; }
        public string post_id { get; set; }
        public int f_forum_id { get; set; }
        public string uid { get; set; }
        public string subject { get; set; }
        public string content { get; set; }
        public string cover { get; set; }
        public int view_type { get; set; }
        public int created_at { get; set; }
        public List<string> images { get; set; }
        public PostStatus post_status { get; set; }
        public List<int> topic_ids { get; set; }
        public int view_status { get; set; }
        public int max_floor { get; set; }
        public int is_original { get; set; }
        public int republish_authorization { get; set; }
        public string reply_time { get; set; }
        public int is_deleted { get; set; }
        public bool is_interactive { get; set; }
        public string structured_content { get; set; }
        public List<object> structured_content_rows { get; set; }
        public int review_id { get; set; }
        public bool is_profit { get; set; }
        public bool is_in_profit { get; set; }
        public string summary { get; set; }
        public bool is_missing { get; set; }
        public int pre_pub_status { get; set; }
        public int profit_post_status { get; set; }
        public bool is_showing_missing { get; set; }
        public int block_reply_img { get; set; }
        public bool is_mentor { get; set; }
    }

    public class PostUpvoteStat
    {
        public int upvote_type { get; set; }
        public int upvote_cnt { get; set; }
    }

    public class ReferObject
    {
        public string refer_id { get; set; }
        public int refer_types { get; set; }
        public string title { get; set; }
        public string structured_content { get; set; }
        public List<object> structured_content_rows { get; set; }
        public List<ImageList> image_list { get; set; }
        public List<TopicList> topic_list { get; set; }
        public UserPostListStat stat { get; set; }
        public UserPostListUser user { get; set; }
        public List<object> vod_list { get; set; }
        public object cover { get; set; }
        public int game_id { get; set; }
        public object resource_info { get; set; }
        public bool is_deleted { get; set; }
        public Status status { get; set; }
        public string review_id { get; set; }
        public string created_at { get; set; }
        public string content { get; set; }
        public int is_original { get; set; }
        public int republish_authorization { get; set; }
        public bool is_missing { get; set; }
        public string summary { get; set; }
        public List<object> link_card_list { get; set; }
        public bool is_showing_missing { get; set; }
        public int block_reply_img { get; set; }
        public bool is_mentor { get; set; }
    }

    public class UserPostListObjectRoot
    {
        public int retcode { get; set; }
        public string message { get; set; }
        public UserPostListData data { get; set; }
    }

    public class UserPLSelfOperation
    {
        public int attitude { get; set; }
        public bool is_collected { get; set; }
        public int upvote_type { get; set; }
        public bool is_like { get; set; }
    }

    public class UserPostListStat
    {
        public int view_num { get; set; }
        public int reply_num { get; set; }
        public int like_num { get; set; }
        public int bookmark_num { get; set; }
        public int forward_num { get; set; }
        public List<PostUpvoteStat> post_upvote_stat { get; set; }
        public List<InstantUpvoteStat> instant_upvote_stat { get; set; }
    }

    public class Status
    {
        public bool is_official { get; set; }
        public bool is_good { get; set; }
    }

    public class TopicList
    {
        public int id { get; set; }
        public string name { get; set; }
        public string cover { get; set; }
        public bool is_top { get; set; }
        public bool is_good { get; set; }
        public bool is_interactive { get; set; }
        public int game_id { get; set; }
        public int content_type { get; set; }
    }

    public class UserPostListUser
    {
        public string uid { get; set; }
        public string nickname { get; set; }
        public string introduce { get; set; }
        public string avatar { get; set; }
        public int gender { get; set; }
        public Certification certification { get; set; }
        public LevelExp level_exp { get; set; }
        public bool is_following { get; set; }
        public bool is_followed { get; set; }
        public string avatar_url { get; set; }
        public string pendant { get; set; }
        public object avatar_ext { get; set; }
    }


}
