using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class PatientRxView
    {

        public string RxId { get; set; }
        public string ProviderId { get; set; }
        public string PatientId { get; set; }
        [Required(ErrorMessage = "Surgery Type is required")]
        public string EquipmentType { get; set; }
        [Required(ErrorMessage = "Surgery Date is required")]
        public string SurgeryDate { get; set; }

        [Required(ErrorMessage = "Rx Start Date is required")]
        public string RxStartDate { get; set; }
        [Required(ErrorMessage = "Rx End Date is required")]
        public string RxEndDate { get; set; }
        [Required(ErrorMessage = "Max Rom up is required")]
        public int MaxRomup { get; set; }
        [Required(ErrorMessage = "Max Rom down is required")]
        public int MaxRomdown { get; set; }
        
        [Required(ErrorMessage = "Rx Sessions Per Week is required")]
        public int RxSessionsPerWeek { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        [Required(ErrorMessage = "Rx Days Perweek is required")]
        public List<checkboxModel> RxDays { get; set; }


        [Required(ErrorMessage = "Date of Birth is required")]
        public string DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"(?:\+\s*\d{2}[\s-]*)?(?:\d[-\s]*){10}", ErrorMessage = "Invalid Mobile Number.")]
        public string Mobile { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "Patient Name is required")]
        public string PatientName { get; set; }

    }

    public class checkboxModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isCheck { get; set; }
    }
}
