using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Protocol
    {
        public Protocol()
        {
            Session = new HashSet<Session>();
        }

        public string ProtocolId { get; set; }
        public string RxId { get; set; }
        public int PatientId { get; set; }
        public string ProtocolName { get; set; }
        public string EquipmentType { get; set; }
        public string DeviceConfiguration { get; set; }
        public int ProtocolEnum { get; set; }
        public int RestPosition { get; set; }
        public int? MaxUpLimit { get; set; }
        public int? StretchUpLimit { get; set; }
        public int? MaxDownLimit { get; set; }
        public int? StretchDownLimit { get; set; }
        public int? FlexUpLimit { get; set; }
        public int? FlexUpHoldtime { get; set; }
        public int? StretchUpHoldtime { get; set; }
        public int? FlexDownLimit { get; set; }
        public int? FlexDownHoldtime { get; set; }
        public int? StretchDownHoldtime { get; set; }
        public int? UpReps { get; set; }
        public int? DownReps { get; set; }
        public int? Reps { get; set; }
        public int? Time { get; set; }
        public int? RestTime { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public int RestAt { get; set; }
        public int RepsAt { get; set; }
        public int Speed { get; set; }
        public string LaymanName { get; set; }
        public int? Plane { get; set; }

        public Patient Patient { get; set; }
        public PatientRx Rx { get; set; }
        public ICollection<Session> Session { get; set; }
    }
}
