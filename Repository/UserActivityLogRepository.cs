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
    public class UserActivityLogRepository : IUserActivityLogInterface
    {
        private OneDirectContext context;

        public UserActivityLogRepository(OneDirectContext context)
        {
            this.context = context;
        }

        public List<UserActivityLog> UserActivityList(string userid = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    return (from p in context.UserActivityLog where p.UserId == userid select p).ToList();
                }
                else
                {
                    return (from p in context.UserActivityLog select p).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<UserActivityLog> UserActivityListByReviewId(string reviewid)
        {
            try
            {

                return (from p in context.UserActivityLog where p.ReviewId == reviewid select p).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<UserActivityLog> UserActivityViewList2(string userid = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    return (from p in context.UserActivityLog
                            orderby p.StartTimeStamp descending
                            where p.UserId == userid
                            select p).ToList();
                }
                else
                {
                    return (from p in context.UserActivityLog
                            orderby p.StartTimeStamp descending
                            select p).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<UserActivityLogView> UserActivityViewList(string userid = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    return (from p in context.UserActivityLog
                            orderby p.StartTimeStamp descending
                            where p.UserId == userid
                            group p by p.SessionId into grp
                            select new UserActivityLogView { UserActivityLog = grp.FirstOrDefault(), count = grp.Count() }).ToList();
                }
                else
                {
                    return (from p in context.UserActivityLog
                            orderby p.StartTimeStamp descending
                            group p by p.SessionId into grp
                            select new UserActivityLogView { UserActivityLog = grp.FirstOrDefault(), count = grp.Count() }).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string DeleteUserActivityLog(int activityId)
        {
            try
            {
                var activity = (from p in context.UserActivityLog where p.ActivityId == activityId select p).ToList();
                context.UserActivityLog.RemoveRange(activity);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
        }
        public int InsertUserActivityLog(UserActivityLog pUserActivityLog)
        {

            context.UserActivityLog.Add(pUserActivityLog);
            return context.SaveChanges();
        }

        public UserActivityLog GetUserActivityLog(int activityId)
        {
            return context.UserActivityLog.FirstOrDefault(x => x.ActivityId == activityId);
        }

        public int UpdateUserActivityLog(UserActivityLog pUserActivityLog)
        {
            var _activity = (from p in context.UserActivityLog
                             where p.ActivityId == pUserActivityLog.ActivityId
                             select p).FirstOrDefault();
            if (_activity != null)
            {
                context.Entry(pUserActivityLog).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return context.SaveChanges();
            }
            return 0;
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
