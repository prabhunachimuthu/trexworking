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
    public class SessionAuditTrailRepository : ISessionAuditTrailInterface
    {
        private OneDirectContext context;

        public SessionAuditTrailRepository(OneDirectContext context)
        {
            this.context = context;
        }

        public string DeleteSessionAuditTrail(int lAuditTrailID)
        {
            try
            {
                var audittrail = (from p in context.SessionAuditTrail where p.AuditTrailId == lAuditTrailID select p).ToList();
                context.SessionAuditTrail.RemoveRange(audittrail);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
        }


        public int InsertSessionAuditTrail(SessionAuditTrail pSessionAuditTrail)
        {
            context.SessionAuditTrail.Add(pSessionAuditTrail);
            return context.SaveChanges();
        }

        public int UpdateSessionAuditTrail(SessionAuditTrail pSessionAuditTrail)
        {
            var _auditTrail = (from p in context.SessionAuditTrail
                               where p.AuditTrailId == pSessionAuditTrail.AuditTrailId
                               select p).FirstOrDefault();
            if (_auditTrail != null)
            {
                context.Entry(_auditTrail).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return context.SaveChanges();
            }
            return 0;
        }

        public List<SessionAuditTrail> GetSessionAuditTrailByUserIdAndType(string luserId, string type)
        {
            return context.SessionAuditTrail.Where(x => x.UserId == luserId && x.SessionType.ToLower() == type.ToLower() && x.SessionClosedTime == null).ToList();
        }

        public List<SessionAuditTrail> GetSessionAuditTrail()
        {
            return context.SessionAuditTrail.OrderByDescending(x => x.SessionOpenTime).ToList();
        }

        public List<SessionAuditTrail> GetSessionAuditTrail(string SortColumn, string OrderBy, string Search, int Skip, int PageSize, ref int TotalCount, ref int TotalResultCount)
        {
            List<SessionAuditTrail> listtrail = null;
            TotalCount = 0;
            TotalResultCount = 0;

            if (context.SessionAuditTrail.Count() > 0)
            {

                //Set total count
                List<SessionAuditTrail> ltrail = context.SessionAuditTrail.AsNoTracking().ToList();
                TotalCount = ltrail.Count();

                //searching  
                if (!string.IsNullOrEmpty(Search))
                {
                    ltrail = ltrail.Where(p =>
                        p.Name.ToLower().Contains(Search.ToLower())
                        || p.Type.ToLower().ToString().Contains(Search.ToLower())
                        || p.SessionId.ToLower().ToString().Contains(Search.ToLower())
                        || p.SessionType.ToLower().ToString().Contains(Search.ToLower())
                        || p.SessionStatus.ToLower().ToString().Contains(Search.ToLower())
                        || p.LinkedSession.ToLower().ToString().Contains(Search.ToLower())
                        || p.SessionOpenTime.ToString().Contains(Search)
                        || p.SessionClosedTime.ToString().Contains(Search)).ToList();
                }

                //Sorting
                switch (SortColumn)
                {
                    case "0":
                        ltrail = OrderBy.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ltrail.OrderByDescending(p => p.Name).ToList() : ltrail.OrderBy(p => p.Name).ToList();
                        break;
                    case "1":
                        ltrail = OrderBy.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ltrail.OrderByDescending(p => p.Type).ToList() : ltrail.OrderBy(p => p.Type).ToList();
                        break;
                    case "2":
                        ltrail = OrderBy.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ltrail.OrderByDescending(p => p.SessionId).ToList() : ltrail.OrderBy(p => p.SessionId).ToList();
                        break;
                    case "3":
                        ltrail = OrderBy.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ltrail.OrderByDescending(p => p.SessionType).ToList() : ltrail.OrderBy(p => p.SessionType).ToList();
                        break;
                    case "4":
                        ltrail = OrderBy.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ltrail.OrderByDescending(p => p.SessionStatus).ToList() : ltrail.OrderBy(p => p.SessionStatus).ToList();
                        break;
                    case "5":
                        ltrail = OrderBy.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ltrail.OrderByDescending(p => p.LinkedSession).ToList() : ltrail.OrderBy(p => p.LinkedSession).ToList();
                        break;
                    case "6":
                        ltrail = OrderBy.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ltrail.OrderByDescending(p => p.SessionOpenTime).ToList() : ltrail.OrderBy(p => p.SessionOpenTime).ToList();
                        break;
                    case "7":
                        ltrail = OrderBy.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ltrail.OrderByDescending(p => p.SessionClosedTime).ToList() : ltrail.OrderBy(p => p.SessionClosedTime).ToList();
                        break;
                    default:
                        ltrail = ltrail.OrderByDescending(p => p.SessionOpenTime).ToList();
                        break;
                }

                if (ltrail != null && ltrail.Skip(Skip).Take(PageSize).Count() > 0)
                {
                    //Paging
                    listtrail = ltrail.Skip(Skip).Take(PageSize).ToList();

                    //Filter count
                    TotalResultCount = ltrail.Count();
                }

            }

            return listtrail;
        }


        public List<SessionAuditTrail> GetSessionAuditTrailByUserId(string luserId)
        {
            return context.SessionAuditTrail.Where(x => x.UserId == luserId).ToList();
        }

        public List<SessionAuditTrail> GetSessionAuditTrailByUserId(string luserId, string status)
        {
            return context.SessionAuditTrail.Where(x => x.UserId == luserId && x.SessionStatus.ToLower() == status.ToLower()).ToList();
        }

        public List<SessionAuditTrail> GetSessionAuditTrailByUserId(string luserId, string status, string type)
        {
            return context.SessionAuditTrail.Where(x => x.UserId == luserId && x.SessionStatus.ToLower() == status.ToLower() && x.SessionType.ToLower() == type.ToLower()).ToList();
        }

        public List<SessionAuditTrail> GetSessionAuditTrailByUserId(string luserId, string status, string type, string loginId)
        {
            return context.SessionAuditTrail.Where(x => x.UserId == luserId && x.SessionStatus.ToLower() == status.ToLower() && x.SessionType.ToLower() == type.ToLower() && x.SessionId == loginId).ToList();
        }
        public int UpdateSessionAuditTrail(string luserId, string type, string status)
        {
            int result = 0;
            try
            {
                if (!string.IsNullOrEmpty(luserId))
                {
                    List<SessionAuditTrail> ltrailList = GetSessionAuditTrailByUserIdAndType(luserId, type);
                    if (ltrailList != null && ltrailList.Count > 0)
                    {

                        foreach (SessionAuditTrail sTrail in ltrailList)
                        {
                            sTrail.SessionStatus = status;
                            sTrail.SessionClosedTime = DateTime.Now;
                            int lupdateres = UpdateSessionAuditTrail(sTrail);
                            if (lupdateres > 0)
                            {
                                result = result + 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public int InsertSessionAuditTrail(User luser, string type, string status, string loginid)
        {
            int result = 0;
            try
            {
                if (luser != null)
                {
                    SessionAuditTrail ltrail = new SessionAuditTrail();
                    ltrail.SessionId = luser.LoginSessionId;
                    ltrail.SessionType = type;
                    ltrail.SessionStatus = status;
                    ltrail.LinkedSession = "";
                    ltrail.SessionOpenTime = DateTime.Now;
                    ltrail.UserId = luser.UserId;
                    ltrail.Type = Utilities.getUserType(luser.Type.ToString());
                    ltrail.Name = luser.Name;
                    ltrail.EmailId = luser.Email;
                    ltrail.PasswordUsed = luser.Password;
                    int res = InsertSessionAuditTrail(ltrail);
                    if (res > 0 && ltrail.AuditTrailId > 0 && !string.IsNullOrEmpty(loginid))
                    {
                        List<SessionAuditTrail> ltrailList = GetSessionAuditTrailByUserId(ltrail.UserId, "forced logout", type, loginid);
                        if (ltrailList != null && ltrailList.Count > 0)
                        {
                            foreach (SessionAuditTrail sTrail in ltrailList)
                            {
                                sTrail.LinkedSession = ltrail.SessionId;
                                int lupdateres = UpdateSessionAuditTrail(sTrail);
                                if (lupdateres > 0)
                                {
                                    result = result + 1;
                                }
                            }
                        }
                        else
                        {
                            result = res;
                        }
                    }
                    else
                    {
                        result = res;

                    }
                }

            }
            catch (Exception ex)
            {

            }
            return result;
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
