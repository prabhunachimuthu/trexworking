using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class PatientReview
    {
        public PatientReview()
        {
            UserActivityLog = new HashSet<UserActivityLog>();
        }

        public string ReviewId { get; set; }
        public string UserId { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public string SessionId { get; set; }
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string ActivityType { get; set; }
        public DateTime StartTimeStamp { get; set; }
        public int Duration { get; set; }
        public string AssessmentComment { get; set; }
        public string AssessmentChecklist { get; set; }

        public ICollection<UserActivityLog> UserActivityLog { get; set; }
    }
}
