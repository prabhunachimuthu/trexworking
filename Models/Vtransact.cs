using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Vtransact
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string Token { get; set; }
        public string ExpiryTime { get; set; }
        public string TherapistId { get; set; }
        public string PatientId { get; set; }
        public int? Duration { get; set; }
        public int? Status { get; set; }
    }
}
