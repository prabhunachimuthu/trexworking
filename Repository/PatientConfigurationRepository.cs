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
    public class PatientConfigurationRepository : IPatientConfigurationInterface
    {
        private OneDirectContext context;

        public PatientConfigurationRepository(OneDirectContext context)
        {
            this.context = context;
        }

        public PatientConfiguration getPatientConfiguration(int patientId, string setupId)
        {
            var config = (from p in context.PatientConfiguration
                          where p.SetupId == setupId && p.PatientId == patientId
                          select p).FirstOrDefault();
            return config;
        }

        public PatientConfiguration getPatientConfigurationbyRxId(string rxId)
        {
            var config = (from p in context.PatientConfiguration.Include(x => x.Setup)
                          where p.RxId == rxId
                          select p).FirstOrDefault();
            return config;
        }
        //kajal
        public List<PatientConfiguration> getPatientConfigurationByPatientId(int patientId)
        {
            var config = (from p in context.PatientConfiguration
                          where p.PatientId == patientId
                          select p).ToList();
            return config;
        }
        public PatientConfiguration getPatientConfigurationbyPatientId(int patientId, string equipmentType)
        {
            var config = (from p in context.PatientConfiguration
                          where p.PatientId == patientId && p.EquipmentType.ToLower() == equipmentType.ToLower()
                          select p).FirstOrDefault();
            return config;
        }
        public PatientConfiguration getPatientConfigurationbyPatientId(int patientId, string equipmentType, string deviceConfiguration)
        {
            var config = (from p in context.PatientConfiguration
                          where p.PatientId == patientId && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration.ToLower() == deviceConfiguration.ToLower()
                          select p).FirstOrDefault();
            return config;
        }

        public PatientConfiguration getPatientConfiguration(string patientId, string EquipmentType, string DeviceConfiguration, string PatientSide)
        {
            var config = (from p in context.PatientConfiguration
                          where p.PatientId == Convert.ToInt32(patientId) && p.EquipmentType.ToLower() == EquipmentType.Trim().ToLower() && p.DeviceConfiguration == DeviceConfiguration && p.PatientSide == PatientSide
                          select p).FirstOrDefault();
            return config;
        }
        public string DeletePatientConfiguration(int pPatientId)
        {
            try
            {
                var patientConfig = (from p in context.PatientConfiguration where p.PatientId == pPatientId select p).ToList();
                context.PatientConfiguration.RemoveRange(patientConfig);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
        }

        public string DeletePatientConfigurationbyConfigId(int configId)
        {
            try
            {
                var patientConfig = (from p in context.PatientConfiguration where p.Id == configId select p).FirstOrDefault();
                context.PatientConfiguration.Remove(patientConfig);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
        }

        public void InsertPatientConfiguration(PatientConfiguration pPatientConfiguration)
        {
            context.PatientConfiguration.Add(pPatientConfiguration);
            context.SaveChanges();
        }

        public void UpdatePatientConfiguration(PatientConfiguration pPatientConfiguration)
        {
           
            context.Entry(pPatientConfiguration).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
