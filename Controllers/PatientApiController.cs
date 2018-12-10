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

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    public class PatientApiController : Controller
    {
        private readonly IPatientRxInterface lIPatientRxRepository;

        private readonly IAppointmentScheduleInterface lIAppointmentScheduleRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly ISessionAuditTrailInterface lISessionAuditTrailRepository;
        private readonly IPatient IPatient;
        private readonly ILogger logger;
        private OneDirectContext context;

        public PatientApiController(OneDirectContext context, ILogger<PatientApiController> plogger, IPatientRxInterface IPatientRxRepository)
        {
            logger = plogger;
            this.context = context;
            IPatient = new PatientRepository(context);
            lIUserRepository = new UserRepository(context);

            lIPatientRxRepository = IPatientRxRepository;
            lISessionAuditTrailRepository = new SessionAuditTrailRepository(context);
            lIAppointmentScheduleRepository = new AppointmentScheduleRepository(context);
        }


        //Checks the Patient’s first name and Surgery date on Patient table and if found, returns Success or else return failure.
        [HttpGet]
        [Route("claimpatient")]
        public JsonResult claimpatient(string patientloginid, string surgerydate)
        {
            try
            {
                string _result = IPatient.ClaimPatient(patientloginid, surgerydate);
                if (_result == "success")
                {
                    return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                else if (_result == "fail")
                {
                    return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "No such patient record", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });

            }

        }


        //If the Patientloginid, and Surgerydate is found in the Patient table then records the PIN in Patient table. 
        [HttpGet]
        [Route("createpin")]
        public JsonResult createpin(string patientloginid, string surgerydate, string PIN)
        {
            try
            {
                string _result = IPatient.CreatePIN(patientloginid, surgerydate, PIN);
                if (_result == "success")
                {
                    User pUser = lIUserRepository.getUser(patientloginid);
                    if (pUser != null)
                    {
                        pUser.Password = PIN;
                        lIUserRepository.UpdateUser(pUser);
                    }

                    return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                else if (_result == "fail")
                {
                    return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "No such patient record", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });

            }

        }


        //If the Patientloginid, and Surgerydate is found in the Patient table then records the PIN in Patient table. 
        [HttpGet]
        [Route("createencryptpin")]
        public JsonResult createencryptpin(string patientloginid, string surgerydate, string PIN)
        {
            try
            {
                string _result = IPatient.CreateEncryptPIN(patientloginid, surgerydate, PIN);
                if (_result == "success")
                {
                    User pUser = lIUserRepository.getUser(patientloginid);
                    if (pUser != null)
                    {
                        pUser.EncryptPasswrod = PIN;
                        lIUserRepository.UpdateUser(pUser);
                    }

                    return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                else if (_result == "fail")
                {
                    return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "No such patient record", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });

            }

        }


        //This API is called to login a patient. it returns sessionId which is used by other APIs for uplaoding and downloading data from server
        [HttpGet]
        [Route("patientlogin")]
        public JsonResult patientlogin(string patientloginid, string PIN)
        {
            try
            {
                PatientLoginView _result = null;
                User luser = null;
                string loginsessionId = "";
                if (!string.IsNullOrEmpty(patientloginid) && !string.IsNullOrEmpty(PIN))
                {
                    luser = lIUserRepository.getUser(patientloginid, PIN, 5);
                }
                if (luser == null && !string.IsNullOrEmpty(patientloginid))
                {
                    luser = lIUserRepository.getUser(patientloginid);
                }
                if (luser != null && !string.IsNullOrEmpty(luser.Password))
                {

                    _result = IPatient.PatientLoginsReturnPatientLoginViewUsingPatientLoginId(patientloginid.ToLower(), PIN);
                    if (_result != null)
                    {
                        loginsessionId = luser.LoginSessionId;
                        if (!string.IsNullOrEmpty(loginsessionId))
                        {
                            lISessionAuditTrailRepository.UpdateSessionAuditTrail(luser.UserId, "API", "Forced Logout");
                        }
                        luser = lIUserRepository.getUser(patientloginid, PIN, 5);

                        lISessionAuditTrailRepository.InsertSessionAuditTrail(luser, "API", "Open", loginsessionId);

                        _result.PatientFirstName = _result.PatientFirstName.Split(new char[0]).Length > 0 ? _result.PatientFirstName.Split(new char[0])[0] : _result.PatientFirstName;
                        return Json(new { Status = (int)HttpStatusCode.OK, SessionId = _result.LoginSessionId, Patient = _result, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)1, SessionId = "", result = "UserID or Password does not match", TimeZone = DateTime.UtcNow.ToString("s") });
                    }


                }
                else if (luser != null && string.IsNullOrEmpty(luser.Password))
                {
                    if (luser.Type == ConstantsVar.Patient)
                        return Json(new { Status = (int)3, SessionId = "", result = "Patient's PIN has not been set", TimeZone = DateTime.UtcNow.ToString("s") });
                    else
                        return Json(new { Status = (int)3, SessionId = "", result = "User's Password has not been set", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                else
                {
                    return Json(new { Status = (int)2, SessionId = "", result = "Patient Details is not found", TimeZone = DateTime.UtcNow.ToString("s") });

                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, SessionId = "", result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }

        //This API is called to login a patient. it returns sessionId which is used by other APIs for uplaoding and downloading data from server
        [HttpGet]
        [Route("encryptpatientlogin")]
        public JsonResult encryptpatientlogin(string patientloginid, string PIN)
        {
            try
            {
                PatientLoginView _result = null;
                User luser = null;
                string loginsessionId = "";
                if (!string.IsNullOrEmpty(patientloginid) && !string.IsNullOrEmpty(PIN))
                {
                    luser = lIUserRepository.getUserWithEncryptPassword(patientloginid, PIN, 5);
                }
                if (luser == null && !string.IsNullOrEmpty(patientloginid))
                {
                    luser = lIUserRepository.getUser(patientloginid);
                }
                if (luser != null && !string.IsNullOrEmpty(luser.EncryptPasswrod))
                {

                    _result = IPatient.PatientLoginsReturnPatientLoginViewUsingPatientLoginIdWithEncryptPin(patientloginid.ToLower(), PIN);
                    if (_result != null)
                    {
                        loginsessionId = luser.LoginSessionId;
                        if (!string.IsNullOrEmpty(loginsessionId))
                        {
                            lISessionAuditTrailRepository.UpdateSessionAuditTrail(luser.UserId, "API", "Forced Logout");
                        }
                        luser = lIUserRepository.getUserWithEncryptPassword(patientloginid, PIN, 5);

                        lISessionAuditTrailRepository.InsertSessionAuditTrail(luser, "API", "Open", loginsessionId);

                        _result.PatientFirstName = _result.PatientFirstName.Split(new char[0]).Length > 0 ? _result.PatientFirstName.Split(new char[0])[0] : _result.PatientFirstName;
                        return Json(new { Status = (int)HttpStatusCode.OK, SessionId = _result.LoginSessionId, Patient = _result, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)1, SessionId = "", result = "UserID or Password does not match", TimeZone = DateTime.UtcNow.ToString("s") });
                    }


                }
                else if (luser != null && string.IsNullOrEmpty(luser.Password))
                {
                    if (luser.Type == ConstantsVar.Patient)
                        return Json(new { Status = (int)3, SessionId = "", result = "Patient's Encrypt PIN has not been set", TimeZone = DateTime.UtcNow.ToString("s") });
                    else
                        return Json(new { Status = (int)3, SessionId = "", result = "User's Encrypt Password has not been set", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                else
                {
                    return Json(new { Status = (int)2, SessionId = "", result = "Patient Details is not found", TimeZone = DateTime.UtcNow.ToString("s") });

                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, SessionId = "", result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }

        [HttpGet]
        [Route("logout")]
        public JsonResult logout(string sessionid, string status)
        {
            try
            {
                User _result = null;
                if (!string.IsNullOrEmpty(sessionid))
                {
                    _result = lIUserRepository.getUserbySessionId(sessionid);
                }
                if (_result != null)
                {
                    _result.LoginSessionId = "";
                    string res = lIUserRepository.UpdateUser(_result);
                    if (!string.IsNullOrEmpty(res))
                    {
                        string res1 = IPatient.UpdatePatientSessionId(_result.UserId);
                        if (!string.IsNullOrEmpty(res1))
                        {
                            if (!string.IsNullOrEmpty(status) && (status.ToLower() == "clean logout" || status.ToLower() == "expired"))
                            {
                                if (status.ToLower() == "expired")
                                    lISessionAuditTrailRepository.UpdateSessionAuditTrail(_result.UserId, "API", "Expired");
                                else
                                    lISessionAuditTrailRepository.UpdateSessionAuditTrail(_result.UserId, "API", "Clean Logout");
                            }
                            else
                                lISessionAuditTrailRepository.UpdateSessionAuditTrail(_result.UserId, "API", "Clean Logout");
                            return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, SessionId = "", result = "user details is not found", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, SessionId = "", result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }
            return Json(new { Status = (int)HttpStatusCode.InternalServerError, SessionId = "", result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
        }


        [HttpGet]
        [Route("patientsummary")]
        public JsonResult patientsummary(string sessionid, string rxid)
        {
            PatientSummary lsummary = new PatientSummary();
            try
            {

                if (!string.IsNullOrEmpty(sessionid) && !string.IsNullOrEmpty(rxid))
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null)
                    {
                        PatientRx lPatientRx = lIPatientRxRepository.getPatientRxbyRxId(rxid, lpatient.PatientId.ToString());
                        if (lPatientRx != null)
                        {
                            lsummary.RxStartDate = lPatientRx.RxStartDate.Value;
                            lsummary.RxEndDate = lPatientRx.RxEndDate.Value;
                            lsummary.RxDuration = Convert.ToInt32((lsummary.RxEndDate - lsummary.RxStartDate).TotalDays);
                            lsummary.RemainingDays = Convert.ToInt32((lsummary.RxEndDate - DateTime.Now).TotalDays);
                            double requiredSession = (((Convert.ToDateTime(lPatientRx.RxEndDate) - Convert.ToDateTime(lPatientRx.RxStartDate)).TotalDays / 7) * (int)lPatientRx.RxDaysPerweek * (int)lPatientRx.RxSessionsPerWeek);
                            int totalSession = lPatientRx.Session != null ? lPatientRx.Session.ToList().Count : 0;
                            lsummary.SessionSuggested = Convert.ToInt32(requiredSession);
                            lsummary.SessionCompleted = totalSession;


                            PatientRx lPatientRxPain = lIPatientRxRepository.getPatientRxPain(lPatientRx.RxId, lPatientRx.PatientId.ToString());

                            if (lPatientRxPain != null)
                            {
                                List<Session> lSessionList = lPatientRx.Session != null ? lPatientRxPain.Session.ToList() : null;
                                if (lSessionList != null && lSessionList.Count > 0)
                                {
                                    lsummary.MaxPainLevel = lSessionList.Select(x => x.Pain.Max(y => y.PainLevel)).Max().HasValue ? lSessionList.Select(x => x.Pain.Max(y => y.PainLevel)).Max().Value : 0;
                                    lsummary.MaxFlexionAchieved = lSessionList.Max(x => x.MaxFlexion);
                                    lsummary.MaxExtensionAchieved = lSessionList.Max(x => x.MaxExtension);
                                    lsummary.TrexMinutes = lSessionList.Select(x => x.RangeDuration1).Sum().HasValue ? lSessionList.Select(x => x.RangeDuration1).Sum().Value : 0;
                                    lsummary.FlexionExtensionMinutes = lSessionList.Select(x => x.RangeDuration2).Sum().HasValue ? lSessionList.Select(x => x.RangeDuration2).Sum().Value : 0;
                                }
                            }
                        }
                        else
                        {
                            return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "patientrx is not configured", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Forbidden, result = "patient is not configured", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Forbidden, result = "session id not valid", TimeZone = DateTime.UtcNow.ToString("s") });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
            return Json(new { Status = (int)HttpStatusCode.OK, result = "success", Summary = lsummary, TimeZone = DateTime.UtcNow.ToString("s") });
        }



    }

}
