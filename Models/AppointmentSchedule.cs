using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class AppointmentSchedule
    {
        public string AppointmentId { get; set; }
        public string UserType { get; set; }
        public string UserId { get; set; }
        public DateTime Datetime { get; set; }
        public int? PatientId { get; set; }
        public string SlotStatus { get; set; }
        public string CallStatus { get; set; }
        public string RecordedFile { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string VseeUrl { get; set; }

        public Patient Patient { get; set; }
        public User User { get; set; }
    }
}
