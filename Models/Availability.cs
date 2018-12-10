using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Availability
    {
        public string AvailabilityId { get; set; }
        public string UserType { get; set; }
        public string UserId { get; set; }
        public string DayOfWeek { get; set; }
        public string HourOfDay { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string TimeZoneOffset { get; set; }

        public User User { get; set; }
    }
}
