using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestReview.Models.FB
{
    public class OgObject
    {
        public string id { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public DateTime updated_time { get; set; }
    }
}