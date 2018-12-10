using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class SessionAuditTrail
    {
        public int AuditTrailId { get; set; }
        public string SessionId { get; set; }
        public string SessionType { get; set; }
        public string SessionStatus { get; set; }
        public string LinkedSession { get; set; }
        public DateTime SessionOpenTime { get; set; }
        public DateTime? SessionClosedTime { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string PasswordUsed { get; set; }
    }
}
