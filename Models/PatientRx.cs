using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class PatientRx
    {
        public PatientRx()
        {
            Protocol = new HashSet<Protocol>();
            Session = new HashSet<Session>();
        }

        public string RxId { get; set; }
        public string ProviderId { get; set; }
        public int PatientId { get; set; }
        public string EquipmentType { get; set; }
        public DateTime? RxStartDate { get; set; }
        public DateTime? RxEndDate { get; set; }
        public int? CurrentFlexion { get; set; }
        public int? CurrentExtension { get; set; }
        public int? GoalFlexion { get; set; }
        public int? GoalExtension { get; set; }
        public int? RxDaysPerweek { get; set; }
        public int? RxSessionsPerWeek { get; set; }
        public string RxDays { get; set; }
        public bool? Active { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string PatientSide { get; set; }
        public int PainThreshold { get; set; }
        public int RateOfChange { get; set; }
        public string DeviceConfiguration { get; set; }

        public Patient Patient { get; set; }
        public User Provider { get; set; }
        public ICollection<Protocol> Protocol { get; set; }
        public ICollection<Session> Session { get; set; }
    }
}
