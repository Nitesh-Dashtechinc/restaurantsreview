using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestReview.Models
{
    public class business_data
    {
        public string id { get; set; }
        public string name { get; set; }
        public float rate { get; set; }
        public int ratecount { get; set; }
        public string imageurl { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public string Url { get; set; }
    }
}