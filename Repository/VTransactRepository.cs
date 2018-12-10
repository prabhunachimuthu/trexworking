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
    public class VTransactRepository : VTransactInterface
    {
        private OneDirectContext context;


        public VTransactRepository(OneDirectContext context)
        {
            this.context = context;
        }

        public Vtransact getVTransactDetails(string therapistId, string patientId)
        {
            return (from p in context.Vtransact
                    where p.TherapistId == therapistId && p.PatientId == patientId && p.Status == 0
                    select p).FirstOrDefault();
        }

        public Vtransact getVTransact(string therapistId)
        {
            return (from p in context.Vtransact
                    where p.TherapistId == therapistId && p.Status == 0
                    select p).FirstOrDefault();
        }
        public Vtransact getVTransactbyTherapistAndPatientId(string therapistId, string patientId)
        {
            Vtransact ltransact = (from p in context.Vtransact
                                   where p.TherapistId == therapistId && p.Status == 0
                                   select p).FirstOrDefault();
            if (ltransact != null)
            {
                ltransact.PatientId = patientId;
                context.Entry(ltransact).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                if (context.SaveChanges() > 0)
                    return ltransact;
            }
            return null;
        }
        public Vtransact getVTransact(string sessionId, string token)
        {
            return (from p in context.Vtransact
                    where p.SessionId == sessionId && p.Token == token
                    select p).FirstOrDefault();
        }
        public Vtransact insertVTransact(Vtransact ltransact)
        {
            context.Vtransact.Add(ltransact);
            if (context.SaveChanges() > 0)
            {
                return ltransact;
            }
            else
                return null;
        }

        public void updateStatus(Vtransact ltransact)
        {
            context.Entry(ltransact).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
