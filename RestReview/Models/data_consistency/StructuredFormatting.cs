using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestReview.Models.data_consistency
{
    public class StructuredFormatting
    {
        public string main_text { get; set; }
        public IList<MainTextMatchedSubstring> main_text_matched_substrings { get; set; }
        public string secondary_text { get; set; }
    }
}