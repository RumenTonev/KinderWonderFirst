using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KinderFirst.Models
{
    public class UploadAdminPhotoViews:TutoPicture
    {
[NotMapped]
        public HttpPostedFileBase File { get; set; }
    }
}