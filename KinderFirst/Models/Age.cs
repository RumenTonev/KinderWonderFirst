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
        [RegularExpression(@"^(0?[1-9]|[12]\d|3[01])$", ErrorMessage = "Please enter valid day value qith single digit or preceding 0 for 1 to 9 or 2 digitd for the other.")]
        public string Day { get; set; }
        [MinLength(1), MaxLength(2)]
        [Required(ErrorMessage = "The month is required")]
        [RegularExpression(@"^(0?[1-9]|1[012])$", ErrorMessage = "Please enter valid month value 1 to 12 plus leading 0 for one digit months.")]
        public string Month { get; set; }
        [Range(1940,2010)]
        [Required(ErrorMessage = "The year is required")]
        public int Year { get; set; }
    }
}