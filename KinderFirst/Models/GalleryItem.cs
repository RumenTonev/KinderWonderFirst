using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KinderFirst.Models
{
    public class GalleryItem
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public string Mail { get; set; }
        public string PicLink { get; set; }
        public int Likes { get; set; }
    }
}