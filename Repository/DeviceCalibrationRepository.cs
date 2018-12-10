using Microsoft.EntityFrameworkCore;
using OneDirect.Helper;
using OneDirect.Models;
using OneDirect.Repository.Interface;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository
{
    public class DeviceCalibrationRepository : IDeviceCalibrationInterface
    {
        private OneDirectContext context;

        public DeviceCalibrationRepository(OneDirectContext context)
        {
            this.context = context;
        }


        public DeviceCalibration getDeviceCalibration(string SetupId)
        {
            return (from p in context.DeviceCalibration
                    where p.SetupId == SetupId
                    select p).FirstOrDefault();
        }

        public DeviceCalibration getDeviceCalibrationByRxID(string RxId)
        {
            PatientConfiguration lpatConfig = context.PatientConfiguration.FirstOrDefault(x => x.RxId == RxId);
            if (lpatConfig != null)
            {
                return context.DeviceCalibration.FirstOrDefault(x => x.SetupId == lpatConfig.SetupId);
            }
            return null;
        }

        public DeviceCalibration getDeviceCalibration(string SetupId, string InstallerId)
        {
            return (from p in context.DeviceCalibration
                    where p.SetupId == SetupId && p.InstallerId == InstallerId
                    select p).FirstOrDefault();
        }

        public List<DeviceCalibration> getDeviceCalibrationListByInstallerId(string InstallerId)
        {
            return (from p in context.DeviceCalibration
                    where p.InstallerId == InstallerId
                    select p).ToList();
        }
        public List<DeviceCalibration> getDeviceCalibrationbyInstallerId(string InstallerId)
        {
            return (from p in context.DeviceCalibration
                    where p.InstallerId == InstallerId
                    select p).ToList();
        }

        public List<DeviceCalibration> getAllDeviceCalibration()
        {
            return (from p in context.DeviceCalibration
                    select p).ToList();
        }
        public List<PatientConfigurationDetails> getAllPatientDeviceCalibration()
        {
            return (from p in context.PatientConfiguration.Include(x => x.Setup).Include(x => x.Patient)
                    join _user in context.User on p.Setup.InstallerId equals _user.UserId
                    select new PatientConfigurationDetails
                    {
                        patientconfiguration = p,
                        InstallerName = _user.Name
                    }).ToList();
        }

        public List<PatientConfigurationDetails> getPatientDeviceCalibrationByInstallerId(string installerId)
        {
            return (from p in context.PatientConfiguration.Include(x => x.Setup).Include(x => x.Patient)
                    join _user in context.User on p.Setup.InstallerId equals _user.UserId
                    where p.Setup.InstallerId == installerId
                    select new PatientConfigurationDetails
                    {
                        patientconfiguration = p,
                        InstallerName = _user.Name
                    }).ToList();
        }

        public List<DeviceConfigurationDetails> getDeviceCalibrationByInstallerId(string installerid)
        {
            List<PatientConfiguration> lpatconf = (from p in context.PatientConfiguration
                                                   select p).ToList();
            List<DeviceConfigurationDetails> ldevicelist = (from p in context.DeviceCalibration
                                                            join _user in context.User on p.InstallerId equals _user.UserId
                                                            where p.InstallerId == installerid
                                                            select new DeviceConfigurationDetails
                                                            {
                                                                devicecalibration = p,
                                                                InstallerName = _user.Name
                                                            }).ToList();
            ldevicelist = ldevicelist.Where(p => !lpatconf.Any(p2 => p2.SetupId == p.devicecalibration.SetupId)).ToList();
            return ldevicelist;
        }
        public List<DeviceConfigurationDetails> getDeviceCalibration()
        {
            List<PatientConfiguration> lpatconf = (from p in context.PatientConfiguration
                                                   select p).ToList();
            List<DeviceConfigurationDetails> ldevicelist = (from p in context.DeviceCalibration
                                                            join _user in context.User on p.InstallerId equals _user.UserId
                                                            select new DeviceConfigurationDetails
                                                            {
                                                                devicecalibration = p,
                                                                InstallerName = _user.Name
                                                            }).ToList();
            ldevicelist = ldevicelist.Where(p => !lpatconf.Any(p2 => p2.SetupId == p.devicecalibration.SetupId)).ToList();
            return ldevicelist;
        }

        public DeviceConfigurationDetails getDeviceCalibrationbySetupId(string setupId)
        {
            DeviceConfigurationDetails ldevice = (from p in context.DeviceCalibration
                                                  join _user in context.User on p.InstallerId equals _user.UserId
                                                  where p.SetupId == setupId
                                                  select new DeviceConfigurationDetails
                                                  {
                                                      devicecalibration = p,
                                                      InstallerName = _user.Name
                                                  }).FirstOrDefault();
            return ldevice;
        }

        public DeviceCalibration getDeviceCalibrationbyControllerId(string ControllerId)
        {
            return (from p in context.DeviceCalibration
                    where p.ControllerId == ControllerId
                    select p).OrderByDescending(x => x.UpdatedDate).FirstOrDefault();
        }
        public DeviceCalibration getDeviceCalibrationbyControllerId(string ControllerId, string installerId)
        {
            return (from p in context.DeviceCalibration
                    where p.ControllerId == ControllerId && p.InstallerId == installerId
                    select p).OrderByDescending(x => x.UpdatedDate).FirstOrDefault();
        }

        public int deleteDeviceCalibrationCascade(string setupId)
        {
            DeviceCalibration ldevice = (from p in context.DeviceCalibration
                                         where p.SetupId == setupId
                                         select p).FirstOrDefault();
            if (ldevice != null)
            {
                List<PatientConfiguration> lpatientCofig = (from p in context.PatientConfiguration
                                                            where p.SetupId == ldevice.SetupId
                                                            select p).ToList();
                if (lpatientCofig != null)
                {
                    foreach (PatientConfiguration lpat in lpatientCofig)
                    {
                        List<Protocol> lprotocolList = (from p in context.Protocol where p.RxId == lpat.RxId select p).ToList();
                        foreach (Protocol lprotocol in lprotocolList)
                        {
                            var pain = (from p in context.Pain where p.ProtocolId == lprotocol.ProtocolId select p).ToList();
                            context.Pain.RemoveRange(pain);
                            context.SaveChanges();
                            var session = (from p in context.Session where p.ProtocolId == lprotocol.ProtocolId select p).ToList();
                            context.Session.RemoveRange(session);
                            context.SaveChanges();
                            var protocol = (from p in context.Protocol where p.ProtocolId == lprotocol.ProtocolId select p).ToList();
                            context.Protocol.RemoveRange(protocol);
                            context.SaveChanges();
                        }
                    }
                    context.PatientConfiguration.RemoveRange(lpatientCofig);
                    context.SaveChanges();
                }
                context.DeviceCalibration.RemoveRange(ldevice);
                return context.SaveChanges();
            }

            return 0;
        }
        public DeviceCalibration getDeviceCalibrationbyCEDP(string controllerId, string equipmentType, string deviceConfiguration, string patientSide)
        {
            return (from p in context.DeviceCalibration
                    where p.ControllerId == controllerId && p.EquipmentType == equipmentType && p.DeviceConfiguration == deviceConfiguration && p.PatientSide == patientSide
                    select p).FirstOrDefault();
        }
        public DeviceCalibration getDeviceCalibrationbyEDPC(string equipmentType, string deviceConfiguration, string patientSide, string charId)
        {
            return (from p in context.DeviceCalibration
                    where p.EquipmentType == equipmentType && p.DeviceConfiguration == deviceConfiguration && p.PatientSide == patientSide && p.ChairId == charId
                    select p).FirstOrDefault();
        }

        public DeviceCalibration getDeviceCalibrationbyEDPB1(string equipmentType, string deviceConfiguration, string patientSide, string boomId1)
        {
            return (from p in context.DeviceCalibration
                    where p.EquipmentType == equipmentType && p.DeviceConfiguration == deviceConfiguration && p.PatientSide == patientSide && p.BoomId1 == boomId1
                    select p).FirstOrDefault();
        }
        public DeviceCalibration getDeviceCalibrationbyEDPB2(string equipmentType, string deviceConfiguration, string patientSide, string boomId2)
        {
            return (from p in context.DeviceCalibration
                    where p.EquipmentType == equipmentType && p.DeviceConfiguration == deviceConfiguration && p.PatientSide == patientSide && p.BoomId2 == boomId2
                    select p).FirstOrDefault();
        }
        public DeviceCalibration getDeviceCalibrationbyEDPB3(string equipmentType, string deviceConfiguration, string patientSide, string boomId3)
        {
            return (from p in context.DeviceCalibration
                    where p.EquipmentType == equipmentType && p.DeviceConfiguration == deviceConfiguration && p.PatientSide == patientSide && p.BoomId3 == boomId3
                    select p).FirstOrDefault();
        }

        public DeviceCalibration getDeviceCalibrationbyEDPCB1B2B3(string equipmentType, string deviceConfiguration, string patientSide, string charId, string boomId1, string boomId2, string boomId3)
        {
            return (from p in context.DeviceCalibration
                    where p.EquipmentType == equipmentType && p.DeviceConfiguration == deviceConfiguration && p.PatientSide == patientSide && p.ChairId == charId && p.BoomId1 == boomId1 && p.BoomId2 == boomId2 && p.BoomId3 == boomId3
                    select p).FirstOrDefault();
        }

        public DeviceCalibration getDeviceCalibrationbyEDPCB1B2(string equipmentType, string deviceConfiguration, string patientSide, string charId, string boomId1, string boomId2)
        {
            return (from p in context.DeviceCalibration
                    where p.EquipmentType == equipmentType && p.DeviceConfiguration == deviceConfiguration && p.PatientSide == patientSide && p.ChairId == charId && p.BoomId1 == boomId1 && p.BoomId2 == boomId2
                    select p).FirstOrDefault();
        }

        public DeviceCalibration getDeviceCalibrationbyEDPCB1(string equipmentType, string deviceConfiguration, string patientSide, string charId, string boomId1)
        {
            return (from p in context.DeviceCalibration
                    where p.EquipmentType == equipmentType && p.DeviceConfiguration == deviceConfiguration && p.PatientSide == patientSide && p.ChairId == charId && p.BoomId1 == boomId1
                    select p).FirstOrDefault();
        }
        public DeviceCalibration getDeviceCalibrationbyCEDPCB1B2(string controllerId, string equipmentType, string deviceConfiguration, string patientSide, string charId, string boomId1, string boomId2)
        {
            return (from p in context.DeviceCalibration
                    where p.ControllerId == controllerId && p.EquipmentType == equipmentType && p.DeviceConfiguration == deviceConfiguration && p.PatientSide == patientSide && p.ChairId == charId && p.BoomId1 == boomId1 && p.BoomId2 == boomId2
                    select p).FirstOrDefault();
        }

        public DeviceCalibration getDeviceCalibrationbyCEDPCB1(string controllerId, string equipmentType, string deviceConfiguration, string patientSide, string charId, string boomId1)
        {
            return (from p in context.DeviceCalibration
                    where p.ControllerId == controllerId && p.EquipmentType == equipmentType && p.DeviceConfiguration == deviceConfiguration && p.PatientSide == patientSide && p.ChairId == charId && p.BoomId1 == boomId1
                    select p).FirstOrDefault();
        }

        public DeviceCalibration getDeviceCalibrationbyCEDPCB1B2B3(string controllerId, string equipmentType, string deviceConfiguration, string patientSide, string charId, string boomId1, string boomId2, string boomId3)
        {
            return (from p in context.DeviceCalibration
                    where p.ControllerId == controllerId && p.EquipmentType == equipmentType && p.DeviceConfiguration == deviceConfiguration && p.PatientSide == patientSide && p.ChairId == charId && p.BoomId1 == boomId1 && p.BoomId2 == boomId2 && p.BoomId3 == boomId3
                    select p).FirstOrDefault();
        }
        public string DeleteDeviceCalibration(string pSetupID)
        {
            try
            {
                var device = (from p in context.DeviceCalibration where p.SetupId == pSetupID select p).ToList();
                context.DeviceCalibration.RemoveRange(device);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
        }

        public void InsertDeviceCalibration(DeviceCalibration pDeviceCalibration)
        {
            context.DeviceCalibration.Add(pDeviceCalibration);
            context.SaveChanges();
        }

        public void UpdateDeviceCalibration(DeviceCalibration pDeviceCalibration)
        {
           
            context.Entry(pDeviceCalibration).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
