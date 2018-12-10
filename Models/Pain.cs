using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Pain
    {
        public int PatientId { get; set; }
        public string RxId { get; set; }
        public string ProtocolId { get; set; }
        public string SessionId { get; set; }
        public string PainId { get; set; }
        public int? Angle { get; set; }
        public int? RepeatNumber { get; set; }
        public int? PainLevel { get; set; }
        public int? FlexionRepNumber { get; set; }
        public int? ExtensionRepNumber { get; set; }

        public Session Session { get; set; }
    }
}
