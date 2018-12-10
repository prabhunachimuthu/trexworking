using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository;
using OneDirect.Models;
using Microsoft.Extensions.Logging;
using OneDirect.Repository.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneDirect.ViewModels;
using OneDirect.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OneDirect.Helper;
using OneDirect.Vsee;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class AppointmentsController : Controller
    {
        private readonly INewPatient INewPatient;
        private readonly IPatient IPatient;
        private readonly IAppointmentScheduleInterface lIAppointmentScheduleRepository;
       
        private readonly IUserInterface lIUserRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public AppointmentsController(OneDirectContext context, ILogger<AppointmentsController> plogger, INewPatient lINewPatient)
        {
            logger = plogger;
            this.context = context;
            IPatient = new PatientRepository(context);
            INewPatient = lINewPatient;
            lIUserRepository = new UserRepository(context);
            
            lIAppointmentScheduleRepository = new AppointmentScheduleRepository(context);
        }

        //get output according to ids(avalability, history or calendar and patientId), show all available slots, block slots and history list of the appointments.
        
        // GET: /<controller>/
        public IActionResult Index(string id = "", string patId = "")
        {
            string timezoneid = TimeZoneInfo.Local.Id;

            Console.Write("TREX Server TimeZone offset :" + timezoneid);

            Console.Write("TREX Browser TimeZone offset :" + HttpContext.Session.GetString("timezoneid"));

            Console.Write("TREX Browser TimeZone :" + HttpContext.Session.GetString("timezone"));
           
            if (!string.IsNullOrEmpty(patId))
            {
                ViewBag.PatId = patId;
                Patient lpat = IPatient.GetPatientByPatientID(Convert.ToInt32(patId));
                if (lpat != null)
                {
                    ViewBag.UserName = lpat.PatientName;
                }
                else
                {
                    ViewBag.UserName = patId;
                }
            }
            else
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
            }
            ViewBag.availability = null;
            ViewBag.appointment = null;
            ViewBag.history = null;
            if (!string.IsNullOrEmpty(id) && id == "Availability")
            {
                List<availabilityView> AvailabilityList = new List<availabilityView>();
                if (ConfigVars.NewInstance.slots.SlotDuration == "30")
                {
                    AvailabilityList = lIAppointmentScheduleRepository.GetAvailabilityInMinutes(HttpContext.Session.GetString("UserId"), HttpContext.Session.GetString("timezoneid"));
                }
                else
                {
                    AvailabilityList = lIAppointmentScheduleRepository.GetAvailability(HttpContext.Session.GetString("UserId"), HttpContext.Session.GetString("timezoneid"));
                }
                ViewBag.availability = AvailabilityList;
                ViewBag.Page = "Availability";
            }
            else if (!string.IsNullOrEmpty(id) && id == "History")
            {
                ViewBag.Page = "History";

                if (!string.IsNullOrEmpty(patId))
                {

                    ViewBag.Patients = lIAppointmentScheduleRepository.getAppointmentPatientListByUserId(HttpContext.Session.GetString("UserId"));

                    ViewBag.History = lIAppointmentScheduleRepository.getAppointmentListByPatientId(Convert.ToInt32(patId), HttpContext.Session.GetString("timezoneid"));


                    Patient lpat = IPatient.GetPatientByPatientID(Convert.ToInt32(patId));
                    if (lpat != null)
                    {
                        ViewBag.SelectedPatient = lpat.PatientName;
                    }
                    else
                    {
                        ViewBag.SelectedPatient = patId;
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") == ConstantsVar.Admin.ToString())
                    {
                        ViewBag.Patients = lIAppointmentScheduleRepository.getAppointmentPatientList();

                        ViewBag.History = lIAppointmentScheduleRepository.getAppointmentList(HttpContext.Session.GetString("timezoneid"));
                    }
                    else
                    {
                        ViewBag.Patients = lIAppointmentScheduleRepository.getAppointmentPatientListByUserId(HttpContext.Session.GetString("UserId"));

                        ViewBag.History = lIAppointmentScheduleRepository.getAppointmentListByUserId(HttpContext.Session.GetString("UserId"), HttpContext.Session.GetString("timezoneid"));

                    }

                    ViewBag.SelectedPatient = "All";
                }
            }
            else
            {
                List<appointmentView> AvailabilityList = new List<appointmentView>();
                if (ConfigVars.NewInstance.slots.SlotDuration == "30")
                {
                    AvailabilityList = lIAppointmentScheduleRepository.GetAvailableSlotsForAppointmentCalendarInMinutes(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), DateTime.Now.Date, DateTime.Now.Date.AddDays(60), HttpContext.Session.GetString("timezoneid"));
                }
                else
                {
                    AvailabilityList = lIAppointmentScheduleRepository.GetAvailableSlotsForAppointmentCalendar(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), DateTime.Now.Date, DateTime.Now.Date.AddDays(60), HttpContext.Session.GetString("timezoneid"));
                }


                ViewBag.appointment = AvailabilityList;

               
                ViewBag.Page = "Calendar";
            }

            return View();
        }

        //delete the history record of appointment of patients
        public IActionResult deleteappointment(string appointmentid)
        {
            try
            {
                if (!string.IsNullOrEmpty(appointmentid))
                {
                    lIAppointmentScheduleRepository.DeleteAppointmentSchedule(appointmentid);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            return RedirectToAction("Index", "Appointments", new { id = "History" });
        }


        //kajal
        public IActionResult deleteappointmentTab(string patId, string patName, string eType, string appointmentid, string returnView = "")
        {
            string deviceConfiguration = string.Empty;
            List<PatientRx> deviceconfig = context.PatientRx.Where(x => x.PatientId.ToString() == patId).ToList();
            if (deviceconfig.Count > 0)
            {
                deviceConfiguration = deviceconfig[0].DeviceConfiguration;
            }
            try
            {
                if (!string.IsNullOrEmpty(appointmentid))
                {

                    lIAppointmentScheduleRepository.DeleteAppointmentSchedule(appointmentid);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }

            return RedirectToAction("Index", "Review", new { id = patId, Username = patName, EquipmentType = eType, actuator = deviceConfiguration, tab = "History" });


        }



        //at the time of calling it is generating uri and connecting to patient
        [HttpGet]
        [ActionName("generateuri")]
        public JsonResult generateuri(string id = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {

                    AppointmentSchedule lappointment = lIAppointmentScheduleRepository.GetAppointment(id);
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
                                        return Json(new { result = "success", url = resURI });
                                    else
                                        return Json("");
                                }
                                else
                                {
                                    return Json("");
                                }
                            }
                            else
                            {
                                return Json("");
                            }

                        }
                        else
                        {
                            return Json("");
                        }

                    }
                    else
                    {
                        return Json("");
                    }
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        //refresh the page and get all available slots for appointment Calendar
        [HttpPost]
        public JsonResult refresh(string day = "", string hour = "", string timezoneoffset = "")
        {
            try
            {

                if (ConfigVars.NewInstance.slots.SlotDuration == "30")
                {
                    List<appointmentView> appointmentlist = lIAppointmentScheduleRepository.GetAvailableSlotsForAppointmentCalendarInMinutes(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), DateTime.Now.Date, DateTime.Now.Date.AddDays(60), HttpContext.Session.GetString("timezoneid"));
                    if (appointmentlist != null && appointmentlist.Count > 0)
                        return Json(new { result = "success", appointmentlist = appointmentlist });
                }
                else
                {
                    List<appointmentView> appointmentlist = lIAppointmentScheduleRepository.GetAvailableSlotsForAppointmentCalendar(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), DateTime.Now.Date, DateTime.Now.Date.AddDays(60), HttpContext.Session.GetString("timezoneid"));
                    if (appointmentlist != null && appointmentlist.Count > 0)
                        return Json(new { result = "success", appointmentlist = appointmentlist });
                }

            }
            catch (Exception ex)
            {
                return Json("");
            }
            return Json("");
        }


        //remove the existing available slot and to add a new slot available for generally available page
        [HttpPost]
        public JsonResult insert(string day = "", string hour = "", string minute = "", string timezoneoffset = "")
        {
            try
            {
               

                if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(hour) && !string.IsNullOrEmpty(timezoneoffset) && ((ConfigVars.NewInstance.slots.SlotDuration == "30" && !string.IsNullOrEmpty(minute)) || ConfigVars.NewInstance.slots.SlotDuration != "30"))
                {
                    string result = "";
                    if (ConfigVars.NewInstance.slots.SlotDuration == "30")
                    {
                        result = lIAppointmentScheduleRepository.GetAvailability(HttpContext.Session.GetString("UserId"), day, hour, minute, timezoneoffset);
                    }
                    else
                    {
                        result = lIAppointmentScheduleRepository.GetAvailability(HttpContext.Session.GetString("UserId"), day, hour, timezoneoffset);
                    }
                    if (string.IsNullOrEmpty(result))
                    {
                        if (ConfigVars.NewInstance.slots.SlotDuration == "30")
                        {
                            string res = lIAppointmentScheduleRepository.InsertAvailability(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour, minute);
                            return Json(new { result = res });
                        }
                        else
                        {
                            string res = lIAppointmentScheduleRepository.InsertAvailability(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour);
                            return Json(new { result = res });
                        }
                    }
                    else
                    {
                        if (ConfigVars.NewInstance.slots.SlotDuration == "30")
                        {
                            List<AppointmentSchedule> lappointments = lIAppointmentScheduleRepository.CheckAppointmentSchedule(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour, minute);
                            if (lappointments == null || (lappointments != null && lappointments.Count == 0))
                            {
                                string res = lIAppointmentScheduleRepository.RemoveAvailability(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour, minute);
                                return Json(new { result = res });
                            }
                            else
                            {
                                return Json(new { result = "failure", appointments = lappointments });
                            }
                        }
                        else
                        {
                            List<AppointmentSchedule> lappointments = lIAppointmentScheduleRepository.CheckAppointmentSchedule(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour);
                            if (lappointments == null || (lappointments != null && lappointments.Count == 0))
                            {
                                string res = lIAppointmentScheduleRepository.RemoveAvailability(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour);
                                return Json(new { result = res });
                            }
                            else
                            {
                                return Json(new { result = "failure", appointments = lappointments });
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                return Json("");
            }
            return Json("");
        }
        //make a slot available, unavailable for appointment calendar page
        [HttpPost]
        public JsonResult insertschedule(string date = "", string timezoneoffset = "", string color = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(timezoneoffset) && !string.IsNullOrEmpty(color))
                {
                    if (color == "green")
                    {
                        DateTime datevalue = Convert.ToDateTime(Utilities.ConverTimetoServerTimeZone(Convert.ToDateTime(date), timezoneoffset));
                        AppointmentSchedule lappointment = lIAppointmentScheduleRepository.CheckAppointmentSchedule(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), datevalue);
                        if (lappointment == null)
                        {
                            AppointmentSchedule lbookAppointment = new AppointmentSchedule();
                            lbookAppointment.AppointmentId = Guid.NewGuid().ToString();
                            lbookAppointment.UserType = Utilities.getUserType(HttpContext.Session.GetString("UserType"));
                            lbookAppointment.UserId = HttpContext.Session.GetString("UserId");
                            lbookAppointment.Datetime = Convert.ToDateTime(Utilities.ConverTimetoServerTimeZone(Convert.ToDateTime(date), timezoneoffset));
                            lbookAppointment.SlotStatus = "Blocked";
                            lbookAppointment.CallStatus = "Caller No Show";
                            lbookAppointment.CreateDate = DateTime.UtcNow;
                            lbookAppointment.UpdateDate = DateTime.UtcNow;
                            lbookAppointment.RecordedFile = "";

                            int _result = lIAppointmentScheduleRepository.InsertAppointment(lbookAppointment);

                            return Json(new { result = _result > 0 ? "success" : "" });
                        }
                        else
                        {
                            int _result = lIAppointmentScheduleRepository.DeleteAppointmentSchedule(lappointment.AppointmentId);
                            return Json(new { result = _result > 0 ? "success" : "" });
                        }
                    }
                    else if (color == "white")
                    {
                        AppointmentSchedule lbookAppointment = new AppointmentSchedule();
                        lbookAppointment.AppointmentId = Guid.NewGuid().ToString();
                        lbookAppointment.UserType = Utilities.getUserType(HttpContext.Session.GetString("UserType"));
                        lbookAppointment.UserId = HttpContext.Session.GetString("UserId");
                        lbookAppointment.Datetime = Convert.ToDateTime(Utilities.ConverTimetoServerTimeZone(Convert.ToDateTime(date), timezoneoffset));
                        lbookAppointment.SlotStatus = "Extra";
                        lbookAppointment.CallStatus = "Extra";
                        lbookAppointment.CreateDate = DateTime.UtcNow;
                        lbookAppointment.UpdateDate = DateTime.UtcNow;
                        lbookAppointment.RecordedFile = "";

                        int _result = lIAppointmentScheduleRepository.InsertAppointment(lbookAppointment);

                        return Json(new { result = _result > 0 ? "success" : "" });
                    }
                    else if (color == "grey")
                    {
                        DateTime datevalue = Convert.ToDateTime(Utilities.ConverTimetoServerTimeZone(Convert.ToDateTime(date), timezoneoffset));
                        AppointmentSchedule lappointment = lIAppointmentScheduleRepository.CheckAppointmentSchedule(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), datevalue);
                        if (lappointment != null)
                        {
                            int _result = lIAppointmentScheduleRepository.DeleteAppointmentSchedule(lappointment.AppointmentId);
                            return Json(new { result = _result > 0 ? "success" : "" });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json("");
            }
            return Json("");
        }


        //remove the availability and update AppointmentSchedule
        [HttpPost]
        public JsonResult delete(string day = "", string hour = "", string minute = "", string timezoneoffset = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(hour) && !string.IsNullOrEmpty(timezoneoffset) && ((ConfigVars.NewInstance.slots.SlotDuration == "30" && !string.IsNullOrEmpty(minute)) || ConfigVars.NewInstance.slots.SlotDuration != "30"))
                
                {
                    if (ConfigVars.NewInstance.slots.SlotDuration == "30")
                    {
                        string res = lIAppointmentScheduleRepository.UpdateAppointmentSchedule(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour, minute);
                        if (!string.IsNullOrEmpty(res))
                        {
                            string result = lIAppointmentScheduleRepository.RemoveAvailability(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour, minute);
                            return Json(new { result = result });
                        }
                        else
                        {
                            return Json(new { result = "" });
                        }
                    }
                    else
                    {
                        string res = lIAppointmentScheduleRepository.UpdateAppointmentSchedule(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour);
                        if (!string.IsNullOrEmpty(res))
                        {
                            string result = lIAppointmentScheduleRepository.RemoveAvailability(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour);
                            return Json(new { result = result });
                        }
                        else
                        {
                            return Json(new { result = "" });
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return Json("");
            }
            return Json("");
        }

    }
}




