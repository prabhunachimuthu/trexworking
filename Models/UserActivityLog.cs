using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class UserActivityLog
    {
        public int ActivityId { get; set; }
        public string SessionId { get; set; }
        public string ActivityType { get; set; }
        public DateTime StartTimeStamp { get; set; }
        public int Duration { get; set; }
        public string Comment { get; set; }
        public string RecordChangeType { get; set; }
        public string RecordType { get; set; }
        public string RecordJson { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string RecordExistingJson { get; set; }
        public string ReviewId { get; set; }

        public PatientReview Review { get; set; }
    }
}
