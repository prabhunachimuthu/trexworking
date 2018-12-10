using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class PatientConfigurationView
    {
        [Required]
        public string Rxid { get; set; }
        [Required]
        public PatientConfig patientconfiguration { get; set; }
    }

    public class PatientConfigurationDetails
    {
        public string InstallerName { get; set; }
        public PatientConfiguration patientconfiguration { get; set; }
    }
    public class DeviceConfigurationDetails
    {
        public string InstallerName { get; set; }
        public DeviceCalibration devicecalibration { get; set; }
    }

    public class PatientConfigurationResult
    {
        public List<PatientConfigurationDetails> patientconfiguration { get; set; }
        public List<DeviceConfigurationDetails> devicecalibration { get; set; }
    }
    public class checkpatientconfiguration
    {
        [Required]
        public string PatientId { get; set; }
        [Required]
        public string EquipmentType { get; set; }
        [Required]
        public string DeviceConfiguration { get; set; }
        [Required]
        public string PatientSide { get; set; }
    }

    public  class PatientConfig
    {
        [Required]
        public string PatientId { get; set; }
        [Required]
        public string SetupId { get; set; }
        [Required]
        public string EquipmentType { get; set; }
        [Required]
        public string DeviceConfiguration { get; set; }
        [Required]
        public string PatientSide { get; set; }
        [Required]
        public int CurrentFlexion { get; set; }
        [Required]
        public int CurrentExtension { get; set; }
        [Required]
        public int CurrentRestPosition { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string RxId { get; set; }
        public string InstallerId { get; set; }
        public string PatientFirstName { get; set; }
        public int UserMode { get; set; }
        public int? SagittalAngle { get; set; }
        public int? ScapularAngle { get; set; }
        public int? FrontalAngle { get; set; }

    }
}
