using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestReview.Models.PhotoCountFB
{
    public class FBidExample
    {
        public IList<Datum> data { get; set; }
        public Paging paging { get; set; }
    }
}