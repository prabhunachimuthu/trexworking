using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class NewPatientRx
    {
        public string RxId { get; set; }
        public string ProviderId { get; set; }
        public int PatientId { get; set; }
        public string EquipmentType { get; set; }
        [Required(ErrorMessage = "Rx Start Date is required")]
        public DateTime? RxStartDate { get; set; }
        [Required(ErrorMessage = "Rx End Date is required")]
        public DateTime? RxEndDate { get; set; }
        public int? MaxRomup { get; set; }
        public int? MaxRomdown { get; set; }
        [Required(ErrorMessage = "Rx Days per week is required")]
        public int? RxDaysPerweek { get; set; }
        [Required(ErrorMessage = "Rx Sessions per week is required")]
        public int? RxSessionsPerWeek { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        [Required(ErrorMessage = "Rx Days Perweek is required")]
        public List<checkboxModel> RxDays { get; set; }
        public bool? Active { get; set; }
        [Required(ErrorMessage = "ExcerciseEnum is required")]
        public string DeviceConfiguration { get; set; }
        public int? ProtocolEnum { get; set; }
        [Required(ErrorMessage = "Current Flexion is required")]
        [RegularExpression("^-?\\d*\\.{0,1}\\d+$", ErrorMessage = "Current Flexion must be numeric.")]
        public int? CurrentFlexion { get; set; }
        [Required(ErrorMessage = "Goal Flexion is required")]
        [RegularExpression("^-?\\d*\\.{0,1}\\d+$", ErrorMessage = "Goal Flexion must be numeric.")]
        public int? GoalFlexion { get; set; }
        [Required(ErrorMessage = "Current Extension is required")]
        [RegularExpression("^-?\\d*\\.{0,1}\\d+$", ErrorMessage = "Current Extension must be numeric.")]
        public int? CurrentExtension { get; set; }
        [Required(ErrorMessage = "Goal Extension is required")]
        [RegularExpression("^-?\\d*\\.{0,1}\\d+$", ErrorMessage = "Goal Extension must be numeric.")]
        public int? GoalExtension { get; set; }
        public string TherapyType { get; set; }
        public string HeadingFlexion { get; set; }
        public string HeadingExtension { get; set; }
        public string ProtocolName { get; set; }
        public string Action { get; set; }
        public int CurrentFlex { get; set; }
        public int GoalFlex { get; set; }
        public int CurrentExten { get; set; }
        public int GoalExten { get; set; }
        public string PatientSide { get; set; }
        public int PainThreshold { get; set; }
        public int RateOfChange { get; set; }


        public int Flex_Current_Start { get; set; }
        public int Flex_Current_End { get; set; }
        public int Flex_Goal_Start { get; set; }
        public int Flex_Goal_End { get; set; }

        public int Ext_Current_Start { get; set; }
        public int Ext_Current_End { get; set; }
        public int Ext_Goal_Start { get; set; }
        public int Ext_Goal_End { get; set; }
    }
   
}
