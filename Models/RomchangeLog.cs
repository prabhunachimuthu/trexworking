using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class RomchangeLog
    {
        public int Id { get; set; }
        public string RxId { get; set; }
        public string ChangedBy { get; set; }
        public int PreviousFlexion { get; set; }
        public int PreviousExtension { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
