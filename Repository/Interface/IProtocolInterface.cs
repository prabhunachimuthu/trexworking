using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface IProtocolInterface : IDisposable
    {
        List<Protocol> getProtocol();
        Protocol getProtocol(List<SessionImport> lsessionList);
        Protocol getProtocol(string lprotocolID);
        Protocol getProtocol(string lprotocolID, string lAssignmentID);
        string InsertProtocol(Protocol pProtocol);
        string UpdateProtocol(Protocol pProtocol);
        List<Protocol> getMobileProtocol(string lpatientId);
        int getProtocolCount(string lAssignmentID);
        List<Protocol> getProtocolList(string lpatientId);
        List<Protocol> getProtocolListBySessionId(string SessionId);
        string DeleteProtocol(Protocol pProtocol);

    }
}
