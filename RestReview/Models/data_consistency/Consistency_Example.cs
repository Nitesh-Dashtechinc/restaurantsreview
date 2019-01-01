using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestReview.Models.data_consistency
{
    public class Consistency_Example
    {
        public IList<Prediction> predictions { get; set; }
        public string status { get; set; }
    }
}