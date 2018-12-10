using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Job
    {
        public Job()
        {
            Jobparameter = new HashSet<Jobparameter>();
            State = new HashSet<State>();
        }

        public int Id { get; set; }
        public int? Stateid { get; set; }
        public string Statename { get; set; }
        public string Invocationdata { get; set; }
        public string Arguments { get; set; }
        public DateTime Createdat { get; set; }
        public DateTime? Expireat { get; set; }
        public int Updatecount { get; set; }

        public ICollection<Jobparameter> Jobparameter { get; set; }
        public ICollection<State> State { get; set; }
    }
}
