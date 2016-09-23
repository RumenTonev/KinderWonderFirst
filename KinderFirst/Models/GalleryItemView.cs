using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KinderFirst.Models
{
    public class GalleryItemView
    {

        [MinLength(3, ErrorMessage = "Минимална дължина 3"), MaxLength(20, ErrorMessage = "Максимална дължина 20")]
        [Required(ErrorMessage = "Собственото име е задължително")]
        public string FirstName { get; set; }
        [MinLength(3, ErrorMessage = "Минимална дължина 3"), MaxLength(20, ErrorMessage = "Максимална дължина 20")]
        [Required(ErrorMessage = "Фамилията е задължителна")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Имайлът е задължителен")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        [MinLength(3, ErrorMessage = "Минимална дължина 3"), MaxLength(20, ErrorMessage = "Максимална дължина 20")]
        public string Mail { get; set; }
        [Required(ErrorMessage = "Телефонният номер е задължителен")]
        [RegularExpression("^([0-9]){4,10}$", ErrorMessage = "Моля въведете само цифри, от 4 до 10 символа дължина.")]
        public string Phone { get; set; }
        public string PicWidth { get; set; }
        public string PicHeight { get; set; }
        public string PicTop { get; set; }
        public string PicLeft { get; set; }
        public string PicName { get; set; }
        public string PicRight { get; set; }
        public string PicBottom { get; set; }
    }
}