using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Extensions
{
    public class PatientConfigurationExtension
    {
        public static PatientConfig PatientConfigurationToPatientConfigurationView(PatientConfiguration lPatientConfiguration)
        {
            if (lPatientConfiguration == null)
                return null;
            PatientConfig lPatientConfigurationView = new PatientConfig()
            {
                PatientId = lPatientConfiguration.PatientId.ToString(),
                SetupId = lPatientConfiguration.SetupId,
                EquipmentType = lPatientConfiguration.EquipmentType,
                DeviceConfiguration = lPatientConfiguration.DeviceConfiguration,
                PatientSide = lPatientConfiguration.PatientSide,
                CurrentFlexion = lPatientConfiguration.CurrentFlexion,
                CurrentExtension = lPatientConfiguration.CurrentExtension,
                CurrentRestPosition = lPatientConfiguration.CurrentRestPosition,
                CreatedDate = lPatientConfiguration.CreatedDate,
                UpdatedDate = lPatientConfiguration.UpdatedDate,
                RxId = lPatientConfiguration.RxId,
                InstallerId = lPatientConfiguration.InstallerId,
                PatientFirstName = lPatientConfiguration.PatientFirstName,
                UserMode = lPatientConfiguration.UserMode,
                SagittalAngle = lPatientConfiguration.SagittalAngle,
                ScapularAngle = lPatientConfiguration.ScapularAngle,
                FrontalAngle = lPatientConfiguration.FrontalAngle

            };
            return lPatientConfigurationView;
        }

        public static PatientConfiguration PatientConfigurationViewToPatientConfiguration(PatientConfig lPatientConfiguration)
        {
            if (lPatientConfiguration == null)
                return null;
            PatientConfiguration pPatientConfiguration = new PatientConfiguration()
            {
                PatientId = Convert.ToInt32(lPatientConfiguration.PatientId),
                SetupId = lPatientConfiguration.SetupId,
                EquipmentType = lPatientConfiguration.EquipmentType,
                DeviceConfiguration = lPatientConfiguration.DeviceConfiguration,
                PatientSide = lPatientConfiguration.PatientSide,
                CurrentFlexion = lPatientConfiguration.CurrentFlexion,
                CurrentExtension = lPatientConfiguration.CurrentExtension,
                CurrentRestPosition = lPatientConfiguration.CurrentRestPosition,
                CreatedDate = lPatientConfiguration.CreatedDate,
                UpdatedDate = lPatientConfiguration.UpdatedDate,
                RxId = lPatientConfiguration.RxId,
                InstallerId = lPatientConfiguration.InstallerId,
                PatientFirstName = lPatientConfiguration.PatientFirstName,
                UserMode = lPatientConfiguration.UserMode,
                SagittalAngle = lPatientConfiguration.SagittalAngle,
                ScapularAngle = lPatientConfiguration.ScapularAngle,
                FrontalAngle = lPatientConfiguration.FrontalAngle
            };
            return pPatientConfiguration;
        }

    }
}
