using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class CreateDeviceCalibrationView
    {
        public string SetupId { get; set; }
        [Required]
        public string ControllerId { get; set; }
        [Required]
        public DateTime ControllerDateTime { get; set; }
        [Required]
        public string MacAddress { get; set; }
        [Required]
        public string ChairId { get; set; }
        [Required]
        public string BoomId1 { get; set; }
        public string BoomId2 { get; set; }
        public string BoomId3 { get; set; }
        [Required]
        public string EquipmentType { get; set; }
        [Required]
        public string DeviceConfiguration { get; set; }
        [Required]
        public string PatientSide { get; set; }
        [Required]
        public int Actuator1RetractedAngle { get; set; }
        [Required]
        public int Actuator1RetractedPulse { get; set; }
        [Required]
        public int Actuator1ExtendedAngle { get; set; }
        [Required]
        public int Actuator1ExtendedPulse { get; set; }
        [Required]
        public int Actuator1NeutralAngle { get; set; }
        [Required]
        public int Actuator1NeutralPulse { get; set; }
        public int? Actuator2RetractedAngle { get; set; }
        public int? Actuator2RetractedPulse { get; set; }
        public int? Actuator2ExtendedAngle { get; set; }
        public int? Actuator2ExtendedPulse { get; set; }
        public int? Actuator2NeutralAngle { get; set; }
        public int? Actuator2NeutralPulse { get; set; }
        public int? Actuator3RetractedAngle { get; set; }
        public int? Actuator3RetractedPulse { get; set; }
        public int? Actuator3ExtendedAngle { get; set; }
        public int? Actuator3ExtendedPulse { get; set; }
        public int? Actuator3NeutralAngle { get; set; }
        public int? Actuator3NeutralPulse { get; set; }
        [Required]
        public string InstallerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatePending { get; set; }
        public string NewControllerId { get; set; }
        [Required]
        public bool InActive { get; set; }
        public string Description { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        [Required]
        public string TabletId { get; set; }
    }

    public class UpdateDeviceCalibrationView
    {
        [Required]
        public string SetupId { get; set; }
        [Required]
        public string ControllerId { get; set; }
        [Required]
        public DateTime ControllerDateTime { get; set; }
        [Required]
        public string MacAddress { get; set; }
        [Required]
        public string ChairId { get; set; }
        [Required]
        public string BoomId1 { get; set; }
        public string BoomId2 { get; set; }
        public string BoomId3 { get; set; }
        [Required]
        public string EquipmentType { get; set; }
        [Required]
        public string DeviceConfiguration { get; set; }
        [Required]
        public string PatientSide { get; set; }
        [Required]
        public int Actuator1RetractedAngle { get; set; }
        [Required]
        public int Actuator1RetractedPulse { get; set; }
        [Required]
        public int Actuator1ExtendedAngle { get; set; }
        [Required]
        public int Actuator1ExtendedPulse { get; set; }
        [Required]
        public int Actuator1NeutralAngle { get; set; }
        [Required]
        public int Actuator1NeutralPulse { get; set; }
        public int? Actuator2RetractedAngle { get; set; }
        public int? Actuator2RetractedPulse { get; set; }
        public int? Actuator2ExtendedAngle { get; set; }
        public int? Actuator2ExtendedPulse { get; set; }
        public int? Actuator2NeutralAngle { get; set; }
        public int? Actuator2NeutralPulse { get; set; }
        public int? Actuator3RetractedAngle { get; set; }
        public int? Actuator3RetractedPulse { get; set; }
        public int? Actuator3ExtendedAngle { get; set; }
        public int? Actuator3ExtendedPulse { get; set; }
        public int? Actuator3NeutralAngle { get; set; }
        public int? Actuator3NeutralPulse { get; set; }
        [Required]
        public string InstallerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatePending { get; set; }
        public string NewControllerId { get; set; }
        [Required]
        public bool InActive { get; set; }
        public string Description { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        [Required]
        public string TabletId { get; set; }
    }
    public class CheckDeviceCalibration
    {
        [Required]
        public string ControllerId { get; set; }
        [Required]
        public string ChairId { get; set; }
        [Required]
        public string BoomId1 { get; set; }
        public string BoomId2 { get; set; }
        public string BoomId3 { get; set; }
        [Required]
        public string EquipmentType { get; set; }
        [Required]
        public string DeviceConfiguration { get; set; }
        [Required]
        public string PatientSide { get; set; }

    }
}
