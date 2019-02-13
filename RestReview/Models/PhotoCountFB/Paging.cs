using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestReview.Models.PhotoCountFB
{
    public class Paging
    {
        public Cursors cursors { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
    }
}