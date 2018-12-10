using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class PatientConfiguration
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string SetupId { get; set; }
        public string EquipmentType { get; set; }
        public string DeviceConfiguration { get; set; }
        public string PatientSide { get; set; }
        public int CurrentFlexion { get; set; }
        public int CurrentExtension { get; set; }
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

        public User Installer { get; set; }
        public Patient Patient { get; set; }
        public DeviceCalibration Setup { get; set; }
    }
}
