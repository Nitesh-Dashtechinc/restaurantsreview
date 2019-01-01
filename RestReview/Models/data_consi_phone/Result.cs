using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestReview.Models.data_consi_phone
{
    public class Result
    {
        public string formatted_phone_number { get; set; }
        public string international_phone_number { get; set; }
        public string name { get; set; }
        public double rating { get; set; }
        public string website { get; set; }
    }
}