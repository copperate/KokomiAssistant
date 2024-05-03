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
    class GetPostReplies
    {
        public async static Task<RepliesObjectRoot> GetPostList(int postID,int ordertype, bool masteronly,string lastid)
        {
            Uri uri;
            if (ordertype == 0) 
            {
                uri = new Uri("https://api-takumi.miyoushe.com/post/api/getPostReplies?post_id=" + postID + "&size=50&only_master=false&last_id="+lastid+"&is_hot=true&from_external_link=false");
            }
            else
            {
                uri = new Uri("https://api-takumi.miyoushe.com/post/api/getPostReplies?post_id="+postID+"&order_type="+ordertype+ "&size=50&only_master=false&last_id="+lastid+"&is_hot=false&from_external_link=false");
            }
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);          //TODO:增加离线逻辑
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RepliesObjectRoot));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (RepliesObjectRoot)serializer.ReadObject(ms);

            return data;
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class RepliesData
    {
        public List<RepliesList> list { get; set; }
        public string last_id { get; set; }
        public bool is_last { get; set; }
        public string post_owner_uid { get; set; }
        public string pin_reply_id { get; set; }
        public int fold_reply_num { get; set; }
    }

    public class Image
    {
        public string url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string format { get; set; }
        public string size { get; set; }
        public string image_id { get; set; }
        public string entity_type { get; set; }
        public string entity_id { get; set; }
        public bool is_deleted { get; set; }
    }


    public class RepliesList
    {
        public Reply reply { get; set; }
        public ReplyUser user { get; set; }
        public ReplyStat stat { get; set; }
        public ReplySelfOperation self_operation { get; set; }
        public MasterStatus master_status { get; set; }
        public List<Image> images { get; set; }
        public List<SubReply> sub_replies { get; set; }
        public bool is_lz { get; set; }
        public int sub_reply_count { get; set; }
        public RUser r_user { get; set; }
        public object r_reply { get; set; }
        public object r_post { get; set; }
    }

    public class MasterStatus
    {
        public bool is_official_master { get; set; }
        public bool is_user_master { get; set; }
    }

    public class Reply
    {
        public int game_id { get; set; }
        public string post_id { get; set; }
        public string reply_id { get; set; }
        public string uid { get; set; }
        public string r_uid { get; set; }
        public string content { get; set; }
        public int f_forum_id { get; set; }
        public string f_reply_id { get; set; }
        public int floor_id { get; set; }
        public int is_deleted { get; set; }
        public int delete_src { get; set; }
        public int created_at { get; set; }
        public int updated_at { get; set; }
        public Int64 deleted_at { get; set; }
        public string struct_content { get; set; }
        public List<object> structured_content_rows { get; set; }
        public bool is_top { get; set; }
        public bool has_block_word { get; set; }
        public int overt_status { get; set; }
        public bool is_showing_missing { get; set; }
        public int selected_comment_time { get; set; }
        public bool is_mentor { get; set; }
        public int view_status { get; set; }
    }

    public class RepliesObjectRoot
    {
        public int retcode { get; set; }
        public string message { get; set; }
        public RepliesData data { get; set; }
    }

    public class RUser
    {
        public string uid { get; set; }
        public string nickname { get; set; }
        public string introduce { get; set; }
        public string avatar { get; set; }
        public int gender { get; set; }
        public Certification certification { get; set; }
        public LevelExp level_exp { get; set; }
        public string avatar_url { get; set; }
        public string pendant { get; set; }
        public string ip_region { get; set; }
        public bool is_following { get; set; }
        public bool is_followed { get; set; }
        public object avatar_ext { get; set; }
    }

    public class ReplySelfOperation
    {
        public int attitude { get; set; }
        public int reply_vote_attitude { get; set; }
    }

    public class ReplyStat
    {
        public int reply_num { get; set; }
        public int like_num { get; set; }
        public int sub_num { get; set; }
        public int dislike_num { get; set; }
    }

    public class SubReply
    {
        public Reply reply { get; set; }
        public ReplyUser user { get; set; }
        public ReplyStat stat { get; set; }
        public ReplySelfOperation self_operation { get; set; }
        public MasterStatus master_status { get; set; }
        public List<object> images { get; set; }
        public List<object> sub_replies { get; set; }
        public bool is_lz { get; set; }
        public int sub_reply_count { get; set; }
        public RUser r_user { get; set; }
        public object r_reply { get; set; }
        public object r_post { get; set; }
    }

    public class ReplyUser
    {
        public string uid { get; set; }
        public string nickname { get; set; }
        public string introduce { get; set; }
        public string avatar { get; set; }
        public int gender { get; set; }
        public Certification certification { get; set; }
        public LevelExp level_exp { get; set; }
        public string avatar_url { get; set; }
        public string pendant { get; set; }
        public string ip_region { get; set; }
        public bool is_following { get; set; }
        public bool is_followed { get; set; }
        public AvatarExt avatar_ext { get; set; }
    }


}
