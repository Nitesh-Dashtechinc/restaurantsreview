using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestReview.Models.ReviewClass
{
    public class Open
    {
        public bool is_overnight { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public int day { get; set; }
    }
}