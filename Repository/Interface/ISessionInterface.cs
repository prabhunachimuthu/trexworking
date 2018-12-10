using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    public interface ISessionInterface : IDisposable
    {
        Session getSession(string lsessionID);
        void InsertSession(Session pSession);
        List<UserSession> getSessionList();
        UserSession getSessionbySessionId(string lsessionID);
        List<UserSession> getSessionList(string lpatientId, string eenum);
        List<UserSession> getSessionListByTherapistId(string lTherapist);
        List<UserSession> getSessionListByProtocoloId(string lProtocol);
        void UpdateSession(Session pSession);
        string DeleteSessionRecordsWithCasecade(string sessionId);
        List<SessionItem> getSessionList(int lpatientId, string RxId);
    }
}
