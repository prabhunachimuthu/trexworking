using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository.Interface;
using OneDirect.Models;
using Microsoft.Extensions.Logging;
using OneDirect.Repository;
using System.Net;
using System.Data;
using OneDirect.Helper;
using OneDirect.ViewModels;
using Newtonsoft.Json;
using OneDirect.Vsee;
using OpenTokSDK;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    public class VideoCallController : Controller
    {
        private readonly IPatientRxInterface lIPatientRxRepository;

        private readonly VTransactInterface lVTransactRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly ISessionAuditTrailInterface lISessionAuditTrailRepository;
        private readonly IPatient IPatient;
        private readonly ILogger logger;
        private OneDirectContext context;

        public VideoCallController(OneDirectContext context, ILogger<PatientApiController> plogger, IPatientRxInterface IPatientRxRepository, VTransactInterface IVTransactInterface)
        {
            logger = plogger;
            this.context = context;
            IPatient = new PatientRepository(context);
            lIUserRepository = new UserRepository(context);

            lIPatientRxRepository = IPatientRxRepository;
            lISessionAuditTrailRepository = new SessionAuditTrailRepository(context);
            lVTransactRepository = IVTransactInterface;
        }


        [HttpGet]
        [Route("GetSessionIDAndToken")]
        public JsonResult GetSessionIDAndToken(string therapistId, string patientId)
        {
            try
            {
                if (!string.IsNullOrEmpty(therapistId) && !string.IsNullOrEmpty(patientId))
                {
                    Patient lpatient = IPatient.GetPatientByPatientLoginId(patientId);
                    if (lpatient != null)
                    {
                        Vtransact _result = lVTransactRepository.getVTransactbyTherapistAndPatientId(therapistId, patientId);
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", VTransact = _result, PatientName = lpatient.PatientName.Replace(' ', '_'), TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }

        }

        [HttpPost]
        [Route("CreateSessionIDAndToken")]
        public JsonResult CreateSessionIDAndToken(string therapistId)
        {
            try
            {
                int ApiKey = ConfigVars.NewInstance.OpenTokAPIKey;//46201082
                string ApiSecret = ConfigVars.NewInstance.OpenTokAPISecretKey;// "fbef52398143449c9b269e1357d797c64ae945be";
                var OpenTok = new OpenTok(ApiKey, ApiSecret);
                var session = OpenTok.CreateSession(mediaMode: MediaMode.ROUTED);
                //var session=OpenTok.CreateSession();
                // Store this sessionId in the database for later use:
                string sessionId = session.Id;
                //string token = OpenTok.GenerateToken(sessionId);
                double inOneMonth = (DateTime.UtcNow.Add(TimeSpan.FromDays(30)).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                string token = session.GenerateToken(role: Role.MODERATOR, expireTime: inOneMonth);
                Vtransact ltranact = new Vtransact();
                ltranact.SessionId = sessionId;
                ltranact.TherapistId = therapistId;
                ltranact.Status = 0;
                ltranact.Token = token;
                Vtransact _result = lVTransactRepository.insertVTransact(ltranact);

                return Json(new { Status = (int)HttpStatusCode.OK, result = "success", VTransact = _result, TimeZone = DateTime.UtcNow.ToString("s") });
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });

            }

        }

        [HttpPost]
        [Route("UpdateStatus")]
        public JsonResult UpdateStatus([FromBody]Vtransact ptranact)
        {
            try
            {
                if (ptranact != null)
                {
                    Vtransact ltransact = lVTransactRepository.getVTransact(ptranact.SessionId, ptranact.Token);
                    if (ltransact != null)
                    {
                        ltransact.PatientId = ptranact.PatientId;
                        ltransact.Status = ptranact.Status;
                        ltransact.Duration = ptranact.Duration;
                        lVTransactRepository.updateStatus(ltransact);
                    }
                }
                return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });

            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });

            }

        }


    }

}
