using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    public interface IPainInterface : IDisposable
    {
        SessionPain getPainByPainId(string lPainId);
        Pain getPain(string painId);
        SessionPain getSessionDetailsForPainAdd(string lSessionID);
        List<SessionPain> getPainBySessionId(string lSessionID);
        void InsertPain(Pain pPain);
        void UpdatePain(Pain pPain);
        string DeletePain(string painId);
        List<Pain> getPainbyPatientIdAndRxId(int patientId, string RxId);
    }
}
