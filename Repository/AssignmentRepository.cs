using OneDirect.Models;
using OneDirect.Repository.Interface;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository
{
    public class AssignmentRepository : IAssignmentInterface

    {
        private OneDirectContext context;

        public AssignmentRepository(OneDirectContext context)
        {
            this.context = context;
        }

        public EquipmentAssignment getEquipmentAssignment(string lAssignmentId)
        {
            return (from p in context.EquipmentAssignment
                    where p.AssignmentId == lAssignmentId
                    select p).FirstOrDefault();
        }
        public List<EquipmentAssignment> getEquipmentAssignment()
        {
            
            return null;

        }
        public EquipmentAssignment getEquipmentAssignment(string lPatientID, string lTherspistID)
        {
            return (from p in context.EquipmentAssignment
                    where p.PatientId == Convert.ToInt32(lPatientID) && p.InstallerId == lTherspistID
                    select p).FirstOrDefault();
        }

        public string InsertEquipmentAssignment(EquipmentAssignment pEquipmentAssignment)
        {
            try
            {
                context.EquipmentAssignment.Add(pEquipmentAssignment);
                context.SaveChanges();
                return "success";
            }
            catch (Exception ex)
            {
                return "fail";
            }

        }

        public string UpdateEquipmentAssignment(EquipmentAssignment pEquipmentAssignment)
        {
            try
            {
                context.Entry(pEquipmentAssignment).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                return "success";
            }
            catch (Exception ex)
            {
                return "fail";
            }

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
