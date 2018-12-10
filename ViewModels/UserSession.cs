using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class UserSession
    {
        public string SessionId { get; set; }
        public string ProtocolId { get; set; }
        public int? Reps { get; set; }
        public DateTime? Date { get; set; }
        public string PatientId { get; set; }
        public string TherapistId { get; set; }
        public int? Duration { get; set; }
        public int? UpReps { get; set; }
        public int? DownReps { get; set; }
        public int PainCount { get; set; }
        public string Time { get; set; }
        public int? Pain { get; set; }
        public string Name { get; set; }
        public int? FlexionArchieved { get; set; }
        public int? FlexionGoal { get; set; }
        public int? ExtensionArchieved { get; set; }
        public int? ExtensionGoal { get; set; }
        public int? ProtocolEnum { get; set; }
        public string Npi { get; set; }
        public int? Pro_reps { get; set; }
        public string EquipmentType { get; set; }
        public int? GuidedMode { get; set; }
        public string DeviceConfiguration { get; set; }
    }

    public class SessionItem
    {
        public int PatientId { get; set; }
        public string RxId { get; set; }
        public string ProtocolId { get; set; }
        public string SessionId { get; set; }
        public int? Reps { get; set; }
        public int? Duration { get; set; }
        public int? FlexionReps { get; set; }
        public int? ExtensionReps { get; set; }
        public DateTime? SessionDate { get; set; }
        public int? PainCount { get; set; }
        public int MaxFlexion { get; set; }
        public int MaxExtension { get; set; }
        public int MaxPain { get; set; }
        public int? Boom1Position { get; set; }
        public int? Boom2Position { get; set; }
        public int? RangeDuration1 { get; set; }
        public int? RangeDuration2 { get; set; }
        public int? GuidedMode { get; set; }

        

    }
}
