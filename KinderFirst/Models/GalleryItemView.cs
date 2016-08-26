using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KinderFirst.Models
{
    public class GalleryItemView
    {
        [MinLength(3),MaxLength(20)]
        [Required(ErrorMessage = "The First Name is required")]
        public string FirstName { get; set; }
        [MinLength(3), MaxLength(20)]
        [Required(ErrorMessage = "The Lat Name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [MinLength(3), MaxLength(20)]
        public string Mail { get; set; }
        public string PicWidth { get; set; }
        public string PicHeight { get; set; }
        public string PicTop { get; set; }
        public string PicLeft { get; set; }
        public string PicName { get; set; }
        public string PicRight { get; set; }
        public string PicBottom { get; set; }
    }
}