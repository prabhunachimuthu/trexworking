using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class SessionAuditTrailView
    {
        public int AuditTrailId { get; set; }
        public string SessionId { get; set; }
        public string SessionType { get; set; }
        public string SessionStatus { get; set; }
        public string LinkedSession { get; set; }
        public string SessionOpenTime { get; set; }
        public string SessionClosedTime { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string PasswordUsed { get; set; }
    }
}
