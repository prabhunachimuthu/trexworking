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
    public class LibraryRepository : ILibraryInterface
    {
        private OneDirectContext context;

        public LibraryRepository(OneDirectContext context)
        {
            this.context = context;
        }




        public void InsertLibrary(Library pLibrary)
        {
            context.Library.Add(pLibrary);
            context.SaveChanges();
        }
        public void UpdateLibrary(Library pLibrary)
        {
            context.Entry(pLibrary).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }

        public string DeleteLibrary(int libaryId)
        {
            try
            {
                var library = (from p in context.Library where p.Id == libaryId select p).ToList();
                context.Library.RemoveRange(library);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
        }

        public List<Library> getLibrary()
        {
            return (from p in context.Library
                    select p).ToList();
        }

        public Library getLibraryById(int id)
        {
            return (from p in context.Library
                    where p.Id == id
                    select p).FirstOrDefault();
        }
        public List<Library> getLibrary(string limb, string exercise)
        {
            List<Library> pLibrary = (from p in context.Library
                                      where p.Limb == limb && p.Exercise == exercise
                                      select p).ToList();
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
