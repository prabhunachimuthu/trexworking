using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface IUserActivityLogInterface : IDisposable
    {
        List<UserActivityLog> UserActivityListByReviewId(string reviewid);
        UserActivityLog GetUserActivityLog(int activityId);
        int InsertUserActivityLog(UserActivityLog pUserActivityLog);
        int UpdateUserActivityLog(UserActivityLog pUserActivityLog);
        string DeleteUserActivityLog(int activityId);

        List<UserActivityLog> UserActivityList(string userid = "");
        List<UserActivityLogView> UserActivityViewList(string userid = "");
        List<UserActivityLog> UserActivityViewList2(string userid = "");
    }
}
