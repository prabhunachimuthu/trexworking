using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Library
    {
        public int Id { get; set; }
        public string Limb { get; set; }
        public string Side { get; set; }
        public string Exercise { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }
}
