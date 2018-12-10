using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    public interface VTransactInterface : IDisposable
    {
        void updateStatus(Vtransact ltransact);
        Vtransact getVTransact(string therapistId);
        Vtransact insertVTransact(Vtransact ltransact);
        Vtransact getVTransact(string sessionId, string token);
        Vtransact getVTransactbyTherapistAndPatientId(string therapistId, string patientId);
        Vtransact getVTransactDetails(string therapistId, string patientId);
    }
}
