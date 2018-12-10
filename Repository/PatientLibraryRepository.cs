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
    public class PatientLibraryRepository : IPatientLibraryInterface
    {
        private OneDirectContext context;

        public PatientLibraryRepository(OneDirectContext context)
        {
            this.context = context;
        }

        public void InsertPatientLibrary(PatientLibrary pLibrary)
        {
            context.PatientLibrary.Add(pLibrary);
            context.SaveChanges();
        }
        public void UpdatePatientLibrary(PatientLibrary pLibrary)
        {
            context.Entry(pLibrary).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }

        public string DeletePatientLibrary(int id)
        {
            try
            {
                var library = (from p in context.PatientLibrary where p.Id == id select p).ToList();
                context.PatientLibrary.RemoveRange(library);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
        }

        public List<PatientLibrary> getPatientLibrary()
        {
            return (from p in context.PatientLibrary
                    select p).ToList();
        }
        public PatientLibrary getPatientLibraryById(int id)
        {
            return (from p in context.PatientLibrary
                    where p.Id == id
                    select p).FirstOrDefault();
        }
        public List<PatientLibrary> getPatientLibraryByPatientId(string patientid)
        {
            return (from p in context.PatientLibrary
                    where p.Patient == patientid
                    select p).ToList();
        }
        public List<Library> getPatientLibrary(string patientId, string limb, string exercise)
        {
            List<Library> pLibrary = (from p in context.Library
                                      where p.Limb == limb && p.Exercise == exercise
                                      select p).ToList();
            if (pLibrary != null && pLibrary.Count > 0)
            {
                List<PatientLibrary> lpatientLibrary = (from p in context.PatientLibrary
                                                        where p.Patient == patientId && p.Status == 1
                                                        select p).ToList();
                if (lpatientLibrary != null && lpatientLibrary.Count > 0)
                {
                    pLibrary = pLibrary.Where(x => !lpatientLibrary.Any(m => m.LibraryId == x.Id)).ToList();
                }
            }
            return pLibrary;
        }

        public List<Library> getPatientLibrary(string patientId, string limb)
        {
            List<Library> pLibrary = (from p in context.Library
                                      where p.Limb == limb
                                      select p).ToList();
            if (pLibrary != null && pLibrary.Count > 0)
            {
                List<PatientLibrary> lpatientLibrary = (from p in context.PatientLibrary
                                                        where p.Patient == patientId && p.Status == 1
                                                        select p).ToList();
                if (lpatientLibrary != null && lpatientLibrary.Count > 0)
                {
                    pLibrary = pLibrary.Where(x => !lpatientLibrary.Any(m => m.LibraryId == x.Id)).ToList();
                }
            }
            return pLibrary;
        }

        private string getExcercise(string limb, string exenum, List<EquipmentExcercise> EquipmentExcercise)
        {
            if (EquipmentExcercise != null && EquipmentExcercise.Count > 0)
            {
                EquipmentExcercise _EquipmentExcercise = EquipmentExcercise.Where(p => p.Limb.ToLower() == limb.ToLower() && p.ExcerciseEnum == exenum).FirstOrDefault();
                return _EquipmentExcercise.ExcerciseName;
            }
            return "";
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
