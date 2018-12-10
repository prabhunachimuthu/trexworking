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
using Newtonsoft.Json;
using OneDirect.ViewModels;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    [Route("api/[controller]/[action]")]
    public class SessionController : Controller
    {
        private readonly IRomChangeLogInterface IRomChangeLog;
        private readonly IPatientRxInterface IPatientRx;
        private readonly INewPatient INewPatient;
        private readonly IPatient IPatient;
        private readonly ISessionInterface lISessionRepository;
        private readonly IProtocolInterface lIProtocolRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public SessionController(OneDirectContext context, ILogger<SessionController> plogger, INewPatient lINewPatient, IPatientRxInterface lIPatientRx)
        {
            logger = plogger;
            this.context = context;
            IRomChangeLog = new RomChangeLogRepository(context);
            IPatientRx = lIPatientRx;
            INewPatient = lINewPatient;
            IPatient = new PatientRepository(context);
            lISessionRepository = new SessionRepository(context);
            lIProtocolRepository = new ProtocolRepository(context);
        }


        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("Session Post Start");
                if (id.ToString() != null)
                {
                    
                    logger.LogDebug("Session Post End");
                    return Ok();
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.InternalServerError;
                    error.ErrorMessage = "Session List is null";
                    response.Add("ErrorResponse", error);
                    logger.LogDebug("Session Post End");
                    return new ObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Session Post Error: " + ex);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = ex.ToString();
                response.Add("ErrorResponse", error);
                return new ObjectResult(response);
            }
        }


        //Use to download previous session records for a patient for a particular Rx.
        [HttpGet]
        [ActionName("downloadsessionrecords")]
        public JsonResult downloadsessionrecords(string sessionid, string rxid)
        {
            try
            {
                if (!string.IsNullOrEmpty(sessionid) && !string.IsNullOrEmpty(rxid))
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null)
                    {
                        List<SessionItem> _result = lISessionRepository.getSessionList(lpatient.PatientId, rxid);
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", sessionlist = _result, TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient is not registered", TimeZone = DateTime.UtcNow.ToString("s") });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "request string is not proper", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }


        //It is used to create new seesion records for a particular RX using sessionID of patient
        // POST api/values
        [HttpPost]
        public JsonResult Post([FromBody]List<Session> pSessionList, string sessionid)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("Session Post Start");
                if (pSessionList != null && pSessionList.Count > 0 && !string.IsNullOrEmpty(sessionid))
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null)
                    {
                        foreach (Session pSession in pSessionList)
                        {
                            PatientRx lrx = IPatientRx.getPatientRx(pSession.RxId);
                            if (lrx != null)
                            {
                                Session lSession = lISessionRepository.getSession(pSession.SessionId);
                                if (lSession == null)
                                {
                                    if (pSession.MaxFlexion > lrx.CurrentFlexion && pSession.MaxExtension > lrx.CurrentExtension)
                                    {
                                        int res = INewPatient.ChangeRxCurrent(lrx.RxId, pSession.MaxFlexion, pSession.MaxExtension, "Patient");
                                    }
                                    else if (pSession.MaxFlexion > lrx.CurrentFlexion || pSession.MaxExtension > lrx.CurrentExtension)
                                    {
                                        if (pSession.MaxFlexion > lrx.CurrentFlexion)
                                        {
                                            int res = INewPatient.ChangeRxCurrentFlexion(lrx.RxId, pSession.MaxFlexion, "Patient");
                                        }
                                        if (pSession.MaxExtension > lrx.CurrentExtension)
                                        {
                                            int res = INewPatient.ChangeRxCurrentExtension(lrx.RxId, pSession.MaxExtension, "Patient");
                                        }
                                    }
                                    else
                                    {
                                        RomchangeLog plog = new RomchangeLog();
                                        plog.RxId = lrx.RxId;
                                        plog.PreviousFlexion = lrx.CurrentFlexion.HasValue ? Convert.ToInt32(lrx.CurrentFlexion) : 0;
                                        plog.PreviousExtension = lrx.CurrentExtension.HasValue ? Convert.ToInt32(lrx.CurrentExtension) : 0;
                                        plog.CreatedDate = DateTime.UtcNow;
                                        plog.ChangedBy = "Patient";
                                        IRomChangeLog.InsertRomChangeLog(plog);
                                    }
                                    pSession.PatientId = lpatient.PatientId;
                                    lISessionRepository.InsertSession(pSession);
                                }
                                else
                                {
                                    lSession.Duration = pSession.Duration;
                                    lSession.ExtensionReps = pSession.ExtensionReps;
                                    lSession.FlexionReps = pSession.FlexionReps;
                                    lSession.MaxPain = pSession.MaxPain;
                                    lSession.MaxFlexion = pSession.MaxFlexion;
                                    lSession.MaxExtension = pSession.MaxExtension;
                                    lSession.PatientId = lpatient.PatientId;
                                    lSession.RxId = pSession.RxId;
                                    lSession.ProtocolId = pSession.ProtocolId;
                                    lSession.Reps = pSession.Reps;
                                    lSession.SessionDate = pSession.SessionDate;
                                    lSession.PainCount = pSession.PainCount;
                                    lSession.TimeZoneOffset = pSession.TimeZoneOffset;
                                    lSession.Boom1Position = pSession.Boom1Position;
                                    lSession.Boom2Position = pSession.Boom2Position;
                                    lSession.RangeDuration1 = pSession.RangeDuration1;
                                    lSession.RangeDuration2 = pSession.RangeDuration2;
                                    lSession.GuidedMode = pSession.GuidedMode;
                                    lISessionRepository.UpdateSession(lSession);

                                    if (pSession.MaxFlexion > lrx.CurrentFlexion && pSession.MaxExtension > lrx.CurrentExtension)
                                    {
                                        int res = INewPatient.ChangeRxCurrent(lrx.RxId, pSession.MaxFlexion, pSession.MaxExtension, "Patient");
                                    }
                                    else if (pSession.MaxFlexion > lrx.CurrentFlexion || pSession.MaxExtension > lrx.CurrentExtension)
                                    {
                                        if (pSession.MaxFlexion > lrx.CurrentFlexion)
                                        {
                                            int res = INewPatient.ChangeRxCurrentFlexion(lrx.RxId, pSession.MaxFlexion, "Patient");
                                        }
                                        if (pSession.MaxExtension > lrx.CurrentExtension)
                                        {
                                            int res = INewPatient.ChangeRxCurrentExtension(lrx.RxId, pSession.MaxExtension, "Patient");
                                        }
                                    }
                                    else
                                    {
                                        RomchangeLog plog = new RomchangeLog();
                                        plog.RxId = lrx.RxId;
                                        plog.PreviousFlexion = lrx.CurrentFlexion.HasValue ? Convert.ToInt32(lrx.CurrentFlexion) : 0;
                                        plog.PreviousExtension = lrx.CurrentExtension.HasValue ? Convert.ToInt32(lrx.CurrentExtension) : 0;
                                        plog.CreatedDate = DateTime.UtcNow;
                                        plog.ChangedBy = "Patient";
                                        IRomChangeLog.InsertRomChangeLog(plog);
                                    }
                                }
                            }
                        }
                        logger.LogDebug("Session Post End");
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "Session inserted successfully", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient session is not valid", TimeZone = DateTime.UtcNow.ToString("s") });
                    }


                }
                else
                {
                    
                    logger.LogDebug("Session Post End");
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Session insertion failed", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Session Post Error: " + ex);
                
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Session insertion failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }


        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
