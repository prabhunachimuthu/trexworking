using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface IDeviceCalibrationInterface : IDisposable
    {
        DeviceCalibration getDeviceCalibrationbyEDPCB1(string equipmentType, string deviceConfiguration, string patientSide, string charId, string boomId1);
        DeviceCalibration getDeviceCalibrationbyCEDPCB1(string controllerId, string equipmentType, string deviceConfiguration, string patientSide, string charId, string boomId1);
        DeviceCalibration getDeviceCalibrationbyControllerId(string ControllerId);
        DeviceCalibration getDeviceCalibration(string SetupId, string InstallerId);
        DeviceConfigurationDetails getDeviceCalibrationbySetupId(string setupId);
        List<DeviceConfigurationDetails> getDeviceCalibration();
        List<DeviceCalibration> getDeviceCalibrationListByInstallerId(string InstallerId);
        DeviceCalibration getDeviceCalibrationByRxID(string RxId);
        List<DeviceConfigurationDetails> getDeviceCalibrationByInstallerId(string installerid);
        int deleteDeviceCalibrationCascade(string setupId);
        List<DeviceCalibration> getAllDeviceCalibration();
        List<PatientConfigurationDetails> getAllPatientDeviceCalibration();
        List<PatientConfigurationDetails> getPatientDeviceCalibrationByInstallerId(string installerId);
        DeviceCalibration getDeviceCalibration(string lSetupId);
        void InsertDeviceCalibration(DeviceCalibration pDeviceCalibration);
        void UpdateDeviceCalibration(DeviceCalibration pDeviceCalibration);
        string DeleteDeviceCalibration(string pSetupID);
        List<DeviceCalibration> getDeviceCalibrationbyInstallerId(string InstallerId);
        DeviceCalibration getDeviceCalibrationbyControllerId(string ControllerId, string installerId);
        DeviceCalibration getDeviceCalibrationbyEDPC(string equipmentType, string deviceConfiguration, string patientSide, string charId);
        DeviceCalibration getDeviceCalibrationbyEDPB1(string equipmentType, string deviceConfiguration, string patientSide, string boomId1);
        DeviceCalibration getDeviceCalibrationbyEDPB2(string equipmentType, string deviceConfiguration, string patientSide, string boomId2);
        DeviceCalibration getDeviceCalibrationbyEDPB3(string equipmentType, string deviceConfiguration, string patientSide, string boomId3);
        DeviceCalibration getDeviceCalibrationbyEDPCB1B2B3(string equipmentType, string deviceConfiguration, string patientSide, string charId, string boomId1, string boomId2, string boomId3);
        DeviceCalibration getDeviceCalibrationbyEDPCB1B2(string equipmentType, string deviceConfiguration, string patientSide, string charId, string boomId1, string boomId2);
        DeviceCalibration getDeviceCalibrationbyCEDPCB1B2(string controllerId, string equipmentType, string deviceConfiguration, string patientSide, string charId, string boomId1, string boomId2);
        DeviceCalibration getDeviceCalibrationbyCEDPCB1B2B3(string controllerId, string equipmentType, string deviceConfiguration, string patientSide, string charId, string boomId1, string boomId2, string boomId3);
        DeviceCalibration getDeviceCalibrationbyCEDP(string controllerId, string equipmentType, string deviceConfiguration, string patientSide);
    }
}
