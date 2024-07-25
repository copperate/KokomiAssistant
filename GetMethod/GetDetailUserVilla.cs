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
    class GetDetailUserVilla
    {
        public async static Task<DetailUserVillaRoot> GetUserVillaDetailInfo(int userID)
        {
            Uri uri = new Uri("https://api-takumi.miyoushe.com/vila/api/villaGetSelfCreatedVillas?uid=" + userID);
            HttpClient client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Referrer = new Uri("https://app.mihoyo.com");
            var responce = await client.GetAsync(uri);          //TODO:增加离线逻辑
            var result = await responce.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(DetailUserVillaRoot));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (DetailUserVillaRoot)serializer.ReadObject(ms);
            return data;
        }

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [DataContract]
    public class DetailVillaData
    {
        [DataMember]
        public List<DetailVilla> villas { get; set; }
    }
    [DataContract]
    public class DetailUserVillaRoot
    {
        [DataMember]
        public int retcode { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public DetailVillaData data { get; set; }
    }
    [DataContract]
    public class DetailVilla
    {
        [DataMember]
        public DetailVilla2 villa { get; set; }
        [DataMember]
        public List<object> active_member_avatar_urls { get; set; }
        [DataMember]
        public int member_num { get; set; }
    }
    [DataContract]
    public class DetailVilla2
    {
        [DataMember]
        public string villa_id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string villa_avatar_url { get; set; }
        [DataMember]
        public string owner_uid { get; set; }
        [DataMember]
        public bool is_official { get; set; }
        [DataMember]
        public string introduce { get; set; }
        [DataMember]
        public int category_id { get; set; }
        [DataMember]
        public string villa_cover { get; set; }
        [DataMember]
        public string villa_created_at { get; set; }
    }


}
