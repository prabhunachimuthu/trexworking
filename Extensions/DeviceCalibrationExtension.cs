using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Extensions
{
    public class DeviceCalibrationExtension
    {
        public static CreateDeviceCalibrationView DeviceCalibrationToDeviceCalibrationView(DeviceCalibration lDeviceCalibration)
        {
            if (lDeviceCalibration == null)
                return null;
            CreateDeviceCalibrationView lDeviceCalibrationView = new CreateDeviceCalibrationView()
            {
                ControllerId = lDeviceCalibration.ControllerId,
                ControllerDateTime = lDeviceCalibration.ControllerDateTime,
                MacAddress = lDeviceCalibration.MacAddress,
                ChairId = lDeviceCalibration.ChairId,
                BoomId1 = lDeviceCalibration.BoomId1,
                BoomId2 = lDeviceCalibration.BoomId2,
                BoomId3 = lDeviceCalibration.BoomId3,
                EquipmentType = lDeviceCalibration.EquipmentType,
                DeviceConfiguration = lDeviceCalibration.DeviceConfiguration,
                PatientSide = lDeviceCalibration.PatientSide,
                Actuator1RetractedAngle = lDeviceCalibration.Actuator1RetractedAngle,
                Actuator1RetractedPulse = lDeviceCalibration.Actuator1RetractedPulse,
                Actuator1ExtendedAngle = lDeviceCalibration.Actuator1ExtendedAngle,
                Actuator1ExtendedPulse = lDeviceCalibration.Actuator1ExtendedPulse,
                Actuator1NeutralAngle = lDeviceCalibration.Actuator1NeutralAngle,
                Actuator1NeutralPulse = lDeviceCalibration.Actuator1NeutralPulse,
                Actuator2RetractedAngle = lDeviceCalibration.Actuator2RetractedAngle,
                Actuator2RetractedPulse = lDeviceCalibration.Actuator2RetractedPulse,
                Actuator2ExtendedAngle = lDeviceCalibration.Actuator2ExtendedAngle,
                Actuator2ExtendedPulse = lDeviceCalibration.Actuator2ExtendedPulse,
                Actuator2NeutralAngle = lDeviceCalibration.Actuator2NeutralAngle,
                Actuator2NeutralPulse = lDeviceCalibration.Actuator2NeutralPulse,
                Actuator3RetractedAngle = lDeviceCalibration.Actuator3RetractedAngle,
                Actuator3RetractedPulse = lDeviceCalibration.Actuator3RetractedPulse,
                Actuator3ExtendedAngle = lDeviceCalibration.Actuator3ExtendedAngle,
                Actuator3ExtendedPulse = lDeviceCalibration.Actuator3ExtendedPulse,
                Actuator3NeutralAngle = lDeviceCalibration.Actuator3NeutralAngle,
                Actuator3NeutralPulse = lDeviceCalibration.Actuator3NeutralPulse,
                InstallerId = lDeviceCalibration.InstallerId,
                InActive = lDeviceCalibration.InActive,
                UpdatePending = lDeviceCalibration.UpdatePending,
                NewControllerId = lDeviceCalibration.NewControllerId,
                Description = lDeviceCalibration.Description,
                Latitude=lDeviceCalibration.Latitude,
                Longitude=lDeviceCalibration.Longitude,
                TabletId=lDeviceCalibration.TabletId
            };
            return lDeviceCalibrationView;
        }

        public static DeviceCalibration DeviceCalibrationViewToDeviceCalibration(CreateDeviceCalibrationView lDeviceCalibration)
        {
            if (lDeviceCalibration == null)
                return null;
            DeviceCalibration ldevice = new DeviceCalibration()
            {
                ControllerId = lDeviceCalibration.ControllerId,
                ControllerDateTime = lDeviceCalibration.ControllerDateTime,
                MacAddress = lDeviceCalibration.MacAddress,
                ChairId = lDeviceCalibration.ChairId,
                BoomId1 = lDeviceCalibration.BoomId1,
                BoomId2 = lDeviceCalibration.BoomId2,
                BoomId3 = lDeviceCalibration.BoomId3,
                EquipmentType = lDeviceCalibration.EquipmentType,
                DeviceConfiguration = lDeviceCalibration.DeviceConfiguration,
                PatientSide = lDeviceCalibration.PatientSide,
                Actuator1RetractedAngle = lDeviceCalibration.Actuator1RetractedAngle,
                Actuator1RetractedPulse = lDeviceCalibration.Actuator1RetractedPulse,
                Actuator1ExtendedAngle = lDeviceCalibration.Actuator1ExtendedAngle,
                Actuator1ExtendedPulse = lDeviceCalibration.Actuator1ExtendedPulse,
                Actuator1NeutralAngle = lDeviceCalibration.Actuator1NeutralAngle,
                Actuator1NeutralPulse = lDeviceCalibration.Actuator1NeutralPulse,
                Actuator2RetractedAngle = lDeviceCalibration.Actuator2RetractedAngle,
                Actuator2RetractedPulse = lDeviceCalibration.Actuator2RetractedPulse,
                Actuator2ExtendedAngle = lDeviceCalibration.Actuator2ExtendedAngle,
                Actuator2ExtendedPulse = lDeviceCalibration.Actuator2ExtendedPulse,
                Actuator2NeutralAngle = lDeviceCalibration.Actuator2NeutralAngle,
                Actuator2NeutralPulse = lDeviceCalibration.Actuator2NeutralPulse,
                Actuator3RetractedAngle = lDeviceCalibration.Actuator3RetractedAngle,
                Actuator3RetractedPulse = lDeviceCalibration.Actuator3RetractedPulse,
                Actuator3ExtendedAngle = lDeviceCalibration.Actuator3ExtendedAngle,
                Actuator3ExtendedPulse = lDeviceCalibration.Actuator3ExtendedPulse,
                Actuator3NeutralAngle = lDeviceCalibration.Actuator3NeutralAngle,
                Actuator3NeutralPulse = lDeviceCalibration.Actuator3NeutralPulse,
                InstallerId = lDeviceCalibration.InstallerId,
                InActive = lDeviceCalibration.InActive,
                UpdatePending = lDeviceCalibration.UpdatePending,
                NewControllerId = lDeviceCalibration.NewControllerId,
                Description = lDeviceCalibration.Description,
                Latitude = lDeviceCalibration.Latitude,
                Longitude = lDeviceCalibration.Longitude,
                TabletId = lDeviceCalibration.TabletId
            };
            return ldevice;
        }
    }
}
