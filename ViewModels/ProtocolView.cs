using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class ProtocolView
    {
        public string ProtocolId { get; set; }
        [Required(ErrorMessage = "Protocol name is required")]
        public string ProtocolName { get; set; }
        public int RestPosition { get; set; }
        public int? MaxUpLimit { get; set; }
        public int? StretchUpLimit { get; set; }
        public int? MaxDownLimit { get; set; }
        public int? StretchDownLimit { get; set; }
        public int RestAt { get; set; }
        public int RepsAt { get; set; }
        public int Speed { get; set; }
        public int? Reps { get; set; }
        [Required(ErrorMessage = "Patient name is required")]
        public string PatientId { get; set; }
        [Required(ErrorMessage = "Therapist name is required")]
        public string TherapistId { get; set; }
        [Required(ErrorMessage = "Equipment Type is required")]
        public int? EquipmentType { get; set; }
        public string EquipmentName { get; set; }
        public int? FlexUpLimit { get; set; }
        public int? FlexUpHoldtime { get; set; }
        public int? StretchUpHoldtime { get; set; }
        public int? FlexDownLimit { get; set; }
        public int? FlexDownHoldtime { get; set; }
        public int? StretchDownHoldtime { get; set; }
        public int? UpReps { get; set; }
        public int? DownReps { get; set; }
        public DateTime? Time { get; set; }
        public int Actuator { get; set; }
        public string RxId { get; set; }
       
    }
}
