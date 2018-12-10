using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Jobparameter
    {
        public int Id { get; set; }
        public int Jobid { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Updatecount { get; set; }

        public Job Job { get; set; }
    }
}
