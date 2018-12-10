using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Jobqueue
    {
        public int Id { get; set; }
        public int Jobid { get; set; }
        public string Queue { get; set; }
        public DateTime? Fetchedat { get; set; }
        public int Updatecount { get; set; }
    }
}
