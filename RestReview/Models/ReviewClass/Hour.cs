using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestReview.Models.ReviewClass
{
    public class Hour
    {
        public IList<Open> open { get; set; }
        public string hours_type { get; set; }
        public bool is_open_now { get; set; }
    }
}