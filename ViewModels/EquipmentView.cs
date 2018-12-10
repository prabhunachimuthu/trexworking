using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class EquipmentView
    {
        public string AssignmentId { get; set; }
        [Required(ErrorMessage = "Patient name is required")]
        public string PatientId { get; set; }
        [Required(ErrorMessage = "Therapist name is required")]
        public string TherapistId { get; set; }
        public DateTime? DateTime { get; set; }
        public string EquipmentId { get; set; }
        [Required(ErrorMessage = "Equipment Type is required")]
        public string EquipmentType { get; set; }
        public bool? IsActive { get; set; }
        [Required(ErrorMessage = "Latitude is required")]
        public string Latitude { get; set; }
        [Required(ErrorMessage = "Longitude is required")]
        public string Longitude { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

    }
}
