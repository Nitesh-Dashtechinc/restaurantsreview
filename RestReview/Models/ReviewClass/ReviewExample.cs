using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestReview.Models.ReviewClass
{
    public class ReviewExample
    {
        public string id { get; set; }
        public string alias { get; set; }
        public string name { get; set; }
        public string image_url { get; set; }
        public bool is_claimed { get; set; }
        public bool is_closed { get; set; }
        public string url { get; set; }
        public string phone { get; set; }
        public string display_phone { get; set; }
        public int review_count { get; set; }
        public IList<Category> categories { get; set; }
        public double rating { get; set; }
        public Location location { get; set; }
        public Coordinates coordinates { get; set; }
        public IList<string> photos { get; set; }
        public string price { get; set; }
        public IList<Hour> hours { get; set; }
        public IList<string> transactions { get; set; }
    }
}