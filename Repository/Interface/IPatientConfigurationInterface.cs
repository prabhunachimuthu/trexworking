using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface IPatientConfigurationInterface : IDisposable
    {
        PatientConfiguration getPatientConfigurationbyRxId(string rxId);
        void InsertPatientConfiguration(PatientConfiguration pPatientConfiguration);
        void UpdatePatientConfiguration(PatientConfiguration pPatientConfiguration);
        string DeletePatientConfiguration(int pPatientId);
        PatientConfiguration getPatientConfiguration(int patientId, string setupId);
        PatientConfiguration getPatientConfigurationbyPatientId(int patientId, string equipmentType);
        PatientConfiguration getPatientConfiguration(string patientId, string EquipmentType, string DeviceConfiguration, string PatientSide);
        PatientConfiguration getPatientConfigurationbyPatientId(int patientId, string equipmentType, string deviceConfiguration);
        string DeletePatientConfigurationbyConfigId(int configId);
        //kajal
        List<PatientConfiguration> getPatientConfigurationByPatientId(int patientId);
    }
}
