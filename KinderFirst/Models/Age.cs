using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KinderFirst.Models
{
    public class Age
    {
        [MinLength(1), MaxLength(2)]
        [Required(ErrorMessage = "The day is required")]
        [RegularExpression(@"^(0?[1-9]|[12]\d|3[01])$", ErrorMessage = "Валидни дати 1-31 като едноцифрените 1-9 може да имат 0 отпред")]
        public string Day { get; set; }
        [MinLength(1), MaxLength(2)]
        [Required(ErrorMessage = "The month is required")]
        [RegularExpression(@"^(0?[1-9]|1[012])$", ErrorMessage = "Валидни месеци 1-12 като едноцифрените 1-9 може да имат 0 отпред")]
        public string Month { get; set; }
      
        [Required(ErrorMessage = "The year is required")]
        [RegularExpression(@"^(200)[0-4]|((19)\d{2})$", ErrorMessage = "Валидни години  1900-2004")]
        public string Year { get; set; }
    }
}