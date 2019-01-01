using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestReview.Models.data_consistency
{
    public class Prediction
    {
        public string description { get; set; }
        public string id { get; set; }
        public IList<MatchedSubstring> matched_substrings { get; set; }
        public string place_id { get; set; }
        public string reference { get; set; }
        public StructuredFormatting structured_formatting { get; set; }
        public IList<Term> terms { get; set; }
        public IList<string> types { get; set; }
    }
}