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
using OneDirect.Vsee;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    [Route("api/[controller]/[action]")]
    public class AppointmentController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly IRomChangeLogInterface IRomChangeLog;
        private readonly IPatientRxInterface IPatientRx;
        private readonly INewPatient INewPatient;
        private readonly IPatient IPatient;
        private readonly IAppointmentScheduleInterface lIAppointmentScheduleRepository;
        private readonly IProtocolInterface lIProtocolRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public AppointmentController(OneDirectContext context, ILogger<AppointmentController> plogger, IPatientRxInterface lIPatientRx, INewPatient lINewPatient)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
            IRomChangeLog = new RomChangeLogRepository(context);
            IPatientRx = lIPatientRx;
            INewPatient = lINewPatient;
            IPatient = new PatientRepository(context);
            lIAppointmentScheduleRepository = new AppointmentScheduleRepository(context);
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


        //To cancel a booked appointment
        [HttpPost]
        [ActionName("cancelappointment")]
        public JsonResult cancelappointment([FromBody]cancelappointment pcancelappointment, string sessionid)
        {
            string timezoneid = TimeZoneInfo.Local.SupportsDaylightSavingTime ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;//"US Eastern Standard Time";//
            try
            {
                if (!string.IsNullOrEmpty(sessionid) && pcancelappointment != null)
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null)
                    {
                        string _result = lIAppointmentScheduleRepository.CancelAppointments(lpatient.PatientId, pcancelappointment.UserID, pcancelappointment.DateTime);
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = timezoneid });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient is not registered", TimeZone = timezoneid });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "request string is not proper", TimeZone = timezoneid });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = timezoneid });
            }
        }


        //Patient can use this to mark an appointment No Show if the therapist or support did not call.
        [HttpPost]
        [ActionName("noshowappointment")]
        public JsonResult noshowappointment([FromBody]cancelappointment pcancelappointment, string sessionid)
        {
            string timezoneid = TimeZoneInfo.Local.SupportsDaylightSavingTime ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;//"US Eastern Standard Time";//
            try
            {
                if (!string.IsNullOrEmpty(sessionid) && pcancelappointment != null)
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null)
                    {
                        string _result = lIAppointmentScheduleRepository.NoShowAppointments(lpatient.PatientId, pcancelappointment.UserID, pcancelappointment.DateTime);
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = timezoneid });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient is not registered", TimeZone = timezoneid });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "request string is not proper", TimeZone = timezoneid });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = timezoneid });
            }
        }


        //called to get the available slots of appointment of therapist and support .(use code=1 for therapist and code=2 for support
        [HttpGet]
        [ActionName("downloadavailability")]
        public JsonResult downloadavailability(string sessionid, int code)
        {
            string timezoneid1 = TimeZoneInfo.Local.SupportsDaylightSavingTime ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;//"US Eastern Standard Time";//
            try
            {
                if (!string.IsNullOrEmpty(sessionid) && code > 0)
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null)
                    {
                        List<PatientRx> lpatientRx = INewPatient.GetNewPatientRxByPatId(lpatient.PatientId.ToString());
                        if (lpatientRx != null && lpatientRx.Count > 0)
                        {
                            var startdate = lpatientRx.Min(x => x.RxStartDate).Value;
                            var enddate = lpatientRx.Max(x => x.RxEndDate).Value;
                            List<AvailableSlot> AvailableSlotList = lIAppointmentScheduleRepository.GetAvailableSlots(lpatient.Therapistid, startdate, enddate, code);
                            downloadavailability ldownloadavailability = new downloadavailability();
                            ldownloadavailability.availableSlots = AvailableSlotList;
                            string timezoneid = TimeZoneInfo.Local.SupportsDaylightSavingTime ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;//"US Eastern Standard Time";//
                            ldownloadavailability.timeZoneOffset = timezoneid;

                            return Json(new { Status = (int)HttpStatusCode.OK, result = "success", AvailabilityList = ldownloadavailability, TimeZone = timezoneid });
                        }
                        else
                        {
                            return Json(new { Status = (int)HttpStatusCode.OK, result = "PatientRx is not registered", TimeZone = timezoneid1 });
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient is not registered", TimeZone = timezoneid1 });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "request string is not proper", TimeZone = timezoneid1 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = timezoneid1 });
            }
        }



        //To book an available timeslot for Therapist or TREX support.
        [HttpPost]
        [ActionName("bookappointment")]
        public JsonResult bookappointment([FromBody]bookappointment pbookappointment, string sessionid)
        {
            string timezoneid = TimeZoneInfo.Local.SupportsDaylightSavingTime ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;//"US Eastern Standard Time";//
            try
            {
                if (!string.IsNullOrEmpty(sessionid) && pbookappointment != null && !string.IsNullOrEmpty(pbookappointment.UserID) && !string.IsNullOrEmpty(pbookappointment.DateTime))
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null)
                    {
                        User luser = lIUserRepository.getUser(pbookappointment.UserID);
                        if (luser != null)
                        {
                            AppointmentSchedule lbookAppointment = lIAppointmentScheduleRepository.CheckAppointmentSchedule(pbookappointment.UserID, Utilities.getUserType(luser.Type.ToString()), Convert.ToDateTime(pbookappointment.DateTime));
                            if (lbookAppointment == null)
                            {
                                lbookAppointment = new AppointmentSchedule();

                                lbookAppointment.UserType = luser.Type == 1 ? "Support" : "Therapist";
                                lbookAppointment.UserId = luser.UserId;
                                lbookAppointment.Datetime = Convert.ToDateTime(pbookappointment.DateTime);
                                lbookAppointment.PatientId = lpatient.PatientId;
                                lbookAppointment.SlotStatus = "Booked";
                                lbookAppointment.CallStatus = "Open";
                                lbookAppointment.CreateDate = DateTime.UtcNow;
                                lbookAppointment.UpdateDate = DateTime.UtcNow;
                                lbookAppointment.RecordedFile = "";
                            }
                            else if (lbookAppointment != null && lbookAppointment.CallStatus == "Extra")
                            {
                                lbookAppointment.Datetime = Convert.ToDateTime(pbookappointment.DateTime);
                                lbookAppointment.PatientId = lpatient.PatientId;
                                lbookAppointment.SlotStatus = "Booked";
                                lbookAppointment.CallStatus = "Open";
                                lbookAppointment.UpdateDate = DateTime.UtcNow;
                            }

                            User pPatient = lIUserRepository.getUser(lpatient.PatientLoginId);
                            User pTherapistorSupport = lIUserRepository.getUser(luser.UserId);
                            if (pPatient != null && !string.IsNullOrEmpty(pPatient.Vseeid) && pTherapistorSupport != null && !string.IsNullOrEmpty(pTherapistorSupport.Vseeid))
                            {
                                VSeeHelper vsee = new VSeeHelper();
                                dynamic resURI = vsee.GetURI(pTherapistorSupport.Vseeid, pTherapistorSupport.Password, pPatient.Vseeid);
                                if (resURI != null)
                                {
                                    lbookAppointment.VseeUrl = resURI;
                                    int _result = 0;
                                    if (!string.IsNullOrEmpty(lbookAppointment.AppointmentId))
                                    {
                                        _result = lIAppointmentScheduleRepository.UpdateAppointment(lbookAppointment);
                                    }
                                    else
                                    {
                                        lbookAppointment.AppointmentId = Guid.NewGuid().ToString();
                                        _result = lIAppointmentScheduleRepository.InsertAppointment(lbookAppointment);
                                    }
                                    if (_result > 0)
                                    {

                                        string content = "New appointment has booked.<br><a href='" + ConfigVars.NewInstance.url + "?ruserid=" + Utilities.EncryptText(pTherapistorSupport.UserId) + "&rtype=" + Utilities.EncryptText(pTherapistorSupport.Type.ToString()) + "&rpage=" + Utilities.EncryptText("appointment") + "'> Click to view</a>";
                                        Smtp.SendGridEmail(luser.Email, "Appointment", content);
                                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = timezoneid });
                                    }
                                    else
                                        return Json(new { Status = (int)HttpStatusCode.Created, result = "not inserted", TimeZone = timezoneid });
                                }
                                else
                                {
                                    return Json(new { Status = (int)HttpStatusCode.Created, result = "not inserted", TimeZone = timezoneid });
                                }
                            }
                            else
                            {
                                return Json(new { Status = (int)HttpStatusCode.Created, result = "not inserted", TimeZone = timezoneid });
                            }

                        }
                        else
                        {
                            return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "user record is not found", TimeZone = timezoneid });
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient is not registered", TimeZone = timezoneid });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "request string is not proper", TimeZone = timezoneid });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = timezoneid });
            }
        }


        [HttpGet]
        [ActionName("updateappointment")]
        public JsonResult updateappointment(string appointmentid)
        {
            string timezoneid = TimeZoneInfo.Local.SupportsDaylightSavingTime ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;//"US Eastern Standard Time";//
            try
            {
                if (!string.IsNullOrEmpty(appointmentid))
                {

                    AppointmentSchedule lappointment = lIAppointmentScheduleRepository.GetAppointment(appointmentid);
                    if (lappointment != null && lappointment.PatientId.HasValue)
                    {
                        Patient lpatient = IPatient.GetPatientByPatientID(lappointment.PatientId.Value);
                        if (lpatient != null)
                        {
                            User pPatient = lIUserRepository.getUser(lpatient.PatientLoginId);
                            User pTherapistorSupport = lIUserRepository.getUser(lappointment.UserId);
                            if (pPatient != null && !string.IsNullOrEmpty(pPatient.Vseeid) && pTherapistorSupport != null && !string.IsNullOrEmpty(pTherapistorSupport.Vseeid))
                            {
                                VSeeHelper vsee = new VSeeHelper();
                                dynamic resURI = vsee.GetURI(pTherapistorSupport.Vseeid, pTherapistorSupport.Password, pPatient.Vseeid);
                                if (resURI != null)
                                {
                                    lappointment.VseeUrl = resURI;
                                    int _result = lIAppointmentScheduleRepository.UpdateAppointment(lappointment);
                                    if (_result > 0)
                                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", url = resURI, TimeZone = timezoneid });
                                    else
                                        return Json(new { Status = (int)HttpStatusCode.Created, result = "not updated", TimeZone = timezoneid });
                                }
                                else
                                {
                                    return Json(new { Status = (int)HttpStatusCode.Created, result = "not updated", TimeZone = timezoneid });
                                }
                            }
                            else
                            {
                                return Json(new { Status = (int)HttpStatusCode.Created, result = "not updated", TimeZone = timezoneid });
                            }

                        }
                        else
                        {
                            return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient is not registered", TimeZone = timezoneid });
                        }

                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "appointment is not registered", TimeZone = timezoneid });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "request string is not proper", TimeZone = timezoneid });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = timezoneid });
            }
        }


        [HttpGet]
        [ActionName("updatestatus")]
        public JsonResult updatestatus(string appointmentid, string status)
        {

            string timezoneid = TimeZoneInfo.Local.SupportsDaylightSavingTime ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;//"US Eastern Standard Time";//
            try
            {
                if (!string.IsNullOrEmpty(appointmentid) && !string.IsNullOrEmpty(status))
                {

                    AppointmentSchedule lappointment = lIAppointmentScheduleRepository.GetAppointment(appointmentid);
                    if (lappointment != null && lappointment.PatientId.HasValue)
                    {
                        Patient lpatient = IPatient.GetPatientByPatientID(lappointment.PatientId.Value);
                        if (lpatient != null)
                        {
                            lappointment.CallStatus = status;
                            int _result = lIAppointmentScheduleRepository.UpdateAppointment(lappointment);
                            if (_result > 0)
                                return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = timezoneid });
                            else
                                return Json(new { Status = (int)HttpStatusCode.Created, result = "not updated", TimeZone = timezoneid });
                        }
                        else
                        {
                            return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient is not registered", TimeZone = timezoneid });
                        }

                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "appointment is not registered", TimeZone = timezoneid });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "request string is not proper", TimeZone = timezoneid });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = timezoneid });
            }
        }



        //to get a list of past and current appointments with their status for the Patient using PatientID
        [HttpGet]
        [ActionName("getappointments")]
        public JsonResult getappointments(string sessionid)
        {
            string timezoneid = TimeZoneInfo.Local.SupportsDaylightSavingTime ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;//"US Eastern Standard Time";//
            try
            {
                if (!string.IsNullOrEmpty(sessionid))
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null)
                    {
                        List<AppointmentScheduleView> _result = lIAppointmentScheduleRepository.GetAppointments(lpatient.PatientId);
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", appointments = _result, TimeZone = timezoneid });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient is not registered", TimeZone = timezoneid });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "request string is not proper", TimeZone = timezoneid });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = timezoneid });
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
