using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace KinderFirst.Models
{
    public class TutoPicture
    {
        [Key]
        public string Id { get; set; }
        public string Number { get; set; }
        public string TutoId { get; set; }
    }
}
