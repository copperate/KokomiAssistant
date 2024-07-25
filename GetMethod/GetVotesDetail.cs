using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using static KokomiAssistant.GetVotesDetail;

namespace KokomiAssistant
{
    class GetVotesDetail
    {
        public async static Task<VoteRoot> GetVotes(string vote_id, string uid)
        {
            Uri uri = new Uri("https://api-takumi.miyoushe.com/apihub/api/getVotes?owner_uid=" + uid + "&vote_ids=" + vote_id);
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);          //TODO:增加离线逻辑
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(VoteRoot));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (VoteRoot)serializer.ReadObject(ms);
            return data;
        }

        public async static Task<VoteRoot> GetVotesResult(string vote_id, string uid)
        {
            Uri uri = new Uri("https://bbs-api.miyoushe.com/apihub/api/getVotesResult?owner_uid=" + uid + "&vote_ids=" + vote_id);
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);          //TODO:增加离线逻辑
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(VoteRoot));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (VoteRoot)serializer.ReadObject(ms);
            return data;
        }
    }


        public class VoteData1
        {
            public List<VoteData> data { get; set; }
        }
        public class VoteData
        {
            
            public string vote_id { get; set; }
            public string uid { get; set; }
            public int vote_limit { get; set; }
            public int end_time { get; set; }
            public string title { get; set; }
            public List<string> vote_option_indexes { get; set; }
            public string created_at { get; set; }

            public bool is_over { get; set; }
            public object option_stats { get; set; } //option_stats 不知为何无法反序列化，即使“按原样传递”也失败
            public int user_cnt { get; set; }
        }

        public class OptionStats
        {
            [JsonProperty(propertyName:"2")]
            public int id2 { get; set; }
           /* [JsonProperty("5")]
            public int _5 { get; set; }
            [JsonProperty("0")]public int _0 { get; set; }
            [JsonProperty("3")]public int _3 { get; set; }
            [JsonProperty("1")]public int _1 { get; set; }
            [JsonProperty("4")]public int _4 { get; set; }

            [JsonProperty("6")] public int _6 { get; set; }
            [JsonProperty("7")] public int _7 { get; set; }
            [JsonProperty("8")] public int _8 { get; set; }
            [JsonProperty("9")] public int _9 { get; set; }
            [JsonProperty("10")] public int _10 { get; set; }
            [JsonProperty("11")] public int _11 { get; set; }
            [JsonProperty("12")] public int _12 { get; set; }
            [JsonProperty("13")] public int _13 { get; set; }
            [JsonProperty("14")] public int _14 { get; set; }
            [JsonProperty("15")] public int _15 { get; set; }
            [JsonProperty("16")] public int _16 { get; set; }
            [JsonProperty("17")] public int _17 { get; set; }
            [JsonProperty("18")] public int _18 { get; set; }
            [JsonProperty("19")] public int _19 { get; set; }
            [JsonProperty("20")] public int _20 { get; set; }
            [JsonProperty("21")] public int _21 { get; set; }
            [JsonProperty("22")] public int _22 { get; set; }
            [JsonProperty("23")] public int _23 { get; set; }
            [JsonProperty("24")] public int _24 { get; set; }
            [JsonProperty("25")] public int _25 { get; set; }
            [JsonProperty("26")] public int _26 { get; set; }
            [JsonProperty("27")] public int _27 { get; set; }
            [JsonProperty("28")] public int _28 { get; set; }
            [JsonProperty("29")] public int _29 { get; set; }
            [JsonProperty("30")] public int _30 { get; set; }
            [JsonProperty("31")] public int _31 { get; set; }
            [JsonProperty("32")] public int _32 { get; set; }
            [JsonProperty("33")] public int _33 { get; set; }
            [JsonProperty("34")] public int _34 { get; set; }
            [JsonProperty("35")] public int _35 { get; set; }
            [JsonProperty("36")] public int _36 { get; set; }
            [JsonProperty("37")] public int _37 { get; set; }
            [JsonProperty("38")] public int _38 { get; set; }
            [JsonProperty("39")] public int _39 { get; set; }
            [JsonProperty("40")] public int _40 { get; set; }
            [JsonProperty("41")] public int _41 { get; set; }
            [JsonProperty("42")] public int _42 { get; set; }
            [JsonProperty("43")] public int _43 { get; set; }
            [JsonProperty("44")] public int _44 { get; set; }
            [JsonProperty("45")] public int _45 { get; set; }
            [JsonProperty("46")] public int _46 { get; set; }
            [JsonProperty("47")] public int _47 { get; set; }
            [JsonProperty("48")] public int _48 { get; set; }
            [JsonProperty("49")] public int _49 { get; set; }
            [JsonProperty("50")] public int _50 { get; set; }*/

        }

        public class VoteRoot
        {
            public int retcode { get; set; }
            public string message { get; set; }
            public VoteData1 data { get; set; }
        }
    
}
