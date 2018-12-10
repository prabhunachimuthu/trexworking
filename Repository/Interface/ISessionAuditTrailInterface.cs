using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface ISessionAuditTrailInterface : IDisposable
    {
        int InsertSessionAuditTrail(SessionAuditTrail pSessionAuditTrail);
        int UpdateSessionAuditTrail(SessionAuditTrail pSessionAuditTrail);
        string DeleteSessionAuditTrail(int lAuditTrailID);
        List<SessionAuditTrail> GetSessionAuditTrailByUserId(string luserId);
        List<SessionAuditTrail> GetSessionAuditTrail();
        List<SessionAuditTrail> GetSessionAuditTrail(string SortColumn, string OrderBy, string Search, int Skip, int PageSize, ref int TotalCount, ref int TotalResultCount);
        List<SessionAuditTrail> GetSessionAuditTrailByUserId(string luserId, string status);
        List<SessionAuditTrail> GetSessionAuditTrailByUserId(string luserId, string status, string type);
        List<SessionAuditTrail> GetSessionAuditTrailByUserIdAndType(string luserId, string type);
        List<SessionAuditTrail> GetSessionAuditTrailByUserId(string luserId, string status, string type, string loginId);

        int UpdateSessionAuditTrail(string luserId, string type, string status);
        int InsertSessionAuditTrail(User luser, string type, string status, string loginid);
    }
}
