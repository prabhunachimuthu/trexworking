using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class SessionView
    {
        public int PatientId { get; set; }
        public string RxId { get; set; }

        [Required]
        public string ProtocolId { get; set; }
        public string SessionId { get; set; }
        [Required]
        public int? Reps { get; set; }
        [Required]
        public int? Duration { get; set; }
        public int? FlexionReps { get; set; }
        public int? ExtensionReps { get; set; }
        [Required]
        public DateTime? SessionDate { get; set; }
        [Required]
        public int? PainCount { get; set; }
        [Required]
        public int MaxFlexion { get; set; }
        [Required]
        public int MaxExtension { get; set; }
        [Required]
        public int MaxPain { get; set; }
        public string Patname { get; set; }
        public string EType { get; set; }
        public string EEnum { get; set; }
        public int? Boom1Position { get; set; }
        public int? Boom2Position { get; set; }
        public int? RangeDuration1 { get; set; }
        public int? RangeDuration2 { get; set; }
        public int? GuidedMode { get; set; }

        public string returnView { get; set; }

    }

    public class SessionImport
    {
        public string ProtocolId { get; set; }
        public string SessionDate { get; set; }
        public string Reps { get; set; }
        public string Duration { get; set; }
        public string MaxFlexion { get; set; }
        public string FlexionReps { get; set; }
        public string MaxExtension { get; set; }
        public string ExtensionReps { get; set; }
        public string MaxPain { get; set; }
        public string Boom1Position { get; set; }
        public string Boom2Position { get; set; }
        public string RangeDuration1 { get; set; }
        public string RangeDuration2 { get; set; }
        public string GuidedMode { get; set; }
    }
}