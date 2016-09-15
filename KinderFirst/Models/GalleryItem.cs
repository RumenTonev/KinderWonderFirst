using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KinderFirst.Models
{
    public class GalleryItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "owner")]
        public string Owner { get; set; }
        [JsonProperty(PropertyName = "mail")]
        public string Mail { get; set; }
        [JsonProperty(PropertyName = "piclink")]
        public string PicLink { get; set; }
        [JsonProperty(PropertyName = "likes")]
        public int Likes { get; set; }
        [JsonProperty(PropertyName = "isip")]
        public bool IsIp { get; set; }
    }
}