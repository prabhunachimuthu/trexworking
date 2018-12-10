using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class State
    {
        public int Id { get; set; }
        public int Jobid { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
        public DateTime Createdat { get; set; }
        public string Data { get; set; }
        public int Updatecount { get; set; }

        public Job Job { get; set; }
    }
}
