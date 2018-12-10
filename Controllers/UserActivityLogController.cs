using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository.Interface;
using Microsoft.Extensions.Logging;
using OneDirect.Models;
using OneDirect.Repository;
using System.Net;
using OneDirect.Helper;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    public class UserActivityLogController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly IPatient IPatient;
        private readonly ISessionInterface lISessionRepository;
        private readonly IUserActivityLogInterface lIUserActivityLogRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public UserActivityLogController(OneDirectContext context, ILogger<UserActivityLogController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
            IPatient = new PatientRepository(context);
            lISessionRepository = new SessionRepository(context);
            lIUserActivityLogRepository = new UserActivityLogRepository(context);
            lIEquipmentAssignmentRepository = new AssignmentRepository(context);

        }

        //insert the record of user activity
        [HttpPost]
        [ActionName("insertuseractivity")]
        public JsonResult insertuseractivity([FromBody]UserActivityLog Ilog)
        {
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("SessionId")) && Ilog != null)
                {
                    User luser = lIUserRepository.getUserbySessionId(HttpContext.Session.GetString("SessionId"));
                    if (luser != null)
                    {
                        Ilog.SessionId = HttpContext.Session.GetString("SessionId");
                        Ilog.ActivityType = "Review";
                        Ilog.StartTimeStamp = !string.IsNullOrEmpty(HttpContext.Session.GetString("SessionTime")) ? Convert.ToDateTime(HttpContext.Session.GetString("SessionTime")) : DateTime.Now;
                        
                        Ilog.UserId = HttpContext.Session.GetString("UserId");
                        Ilog.UserName = HttpContext.Session.GetString("UserName");
                        Ilog.UserType = HttpContext.Session.GetString("UserType");
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ReviewID")))
                        {
                            Ilog.ReviewId = HttpContext.Session.GetString("ReviewID");
                        }
                        int _result = lIUserActivityLogRepository.InsertUserActivityLog(Ilog);
                        if (_result > 0)
                            return Json(new { result = "success" });

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            return Json(new { result = "failure" });
        }
    }
}
