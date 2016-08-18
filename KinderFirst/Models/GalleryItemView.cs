using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KinderFirst.Models
{
    public class GalleryItemView
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}