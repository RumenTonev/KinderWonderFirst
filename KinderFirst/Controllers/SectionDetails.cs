using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinderFirst.Controllers
{
    public class SectionDetails
    {
        public string Id { get; set; }
        public SectionDetails(string id)
        {
            this.Id = id;
        }
    }
}
