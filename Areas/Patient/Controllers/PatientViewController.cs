using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OneDirect.Extensions;
using OneDirect.Helper;
using OneDirect.Models;
using OneDirect.Repository;
using OneDirect.Repository.Interface;
using OneDirect.ViewModels;

namespace OneDirect.Areas.Patient.Controllers
{
    [Area("Patient")]

    public class PatientViewController : Controller
    {
        private readonly ILibraryInterface lILibraryRepository;
        private readonly IPatientLibraryInterface lIPatientLibraryRepository;
        private readonly IPatientReviewInterface lIPatientReviewRepository;
        private readonly ISessionAuditTrailInterface lISessionAuditTrailRepository;
        private readonly IEquipmentExerciseInterface lIEquipmentExerciseRepository;
        private readonly IPatientRxInterface lIPatientRxRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly IPatient lIPatient;
        private readonly ILogger logger;
        private OneDirectContext context;

        public PatientViewController(OneDirectContext context, ILogger<PatientViewController> plogger, IEquipmentExerciseInterface IEquipmentExerciseRepository, IPatientRxInterface IPatientRxRepository, IPatient lPatientRepository, ILibraryInterface ILibraryRepository, IPatientLibraryInterface IPatientLibraryRepository)
        {
            logger = plogger;
            this.context = context;
            lIPatientReviewRepository = new PatientReviewRepository(context);
            lIUserRepository = new UserRepository(context);
            lIEquipmentExerciseRepository = IEquipmentExerciseRepository;
            lISessionAuditTrailRepository = new SessionAuditTrailRepository(context);
            lIPatientRxRepository = IPatientRxRepository;
            lIPatient = lPatientRepository;
            lILibraryRepository = ILibraryRepository;
            lIPatientLibraryRepository = IPatientLibraryRepository;
        }

        public string RemoveSpecialChars(string str)
        {
            string[] chars = new string[] { "-", " ", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "(", ")", ":", "|", "[", "]" };
            for (var i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                {
                    str = str.Replace(chars[i], "");
                }
            }
            return str;
        }

        [AllowAnonymous]
        public IActionResult Index(string id = "", string ruserid = "", string rtype = "", string rpage = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {

                    TempData["msg"] = "<script>Helpers.ShowMessage('Session expired, please login again', 1);</script>";
                }

                if (!string.IsNullOrEmpty(ruserid) && !string.IsNullOrEmpty(rtype))
                {
                    LoginViewModel puser = new LoginViewModel();
                    puser.ruserid = ruserid;
                    puser.rtype = rtype;
                    puser.rpage = rpage;
                    return View(puser);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }

            return View();
        }
        [TypeFilter(typeof(LoginAuthorizeAttribute))]
        public IActionResult Aboutus()
        {
            ViewBag.HeaderName = "Aboutus";
            return View();
        }
        [HttpPost]
        public IActionResult Index(LoginViewModel pUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(pUser.UserId) && !String.IsNullOrEmpty(pUser.Password))
                    {

                        User _users = null;
                        if (pUser.UserId.ToString().Trim() != ConfigVars.NewInstance.AdminUserName)
                        {
                            _users = lIUserRepository.LoginUser(pUser.UserId.ToString().ToLower().Trim(), pUser.Password.ToString().Trim());
                            if (_users == null)
                            {
                                _users = lIUserRepository.LoginUser(pUser.UserId.ToString().ToUpper().Trim(), pUser.Password.ToString().Trim());
                            }
                        }

                        if (_users != null)
                        {

                            if (string.IsNullOrEmpty(_users.LoginSessionId) && string.IsNullOrEmpty(pUser.ruserid) && string.IsNullOrEmpty(pUser.rtype))
                            {
                                _users.LoginSessionId = Guid.NewGuid().ToString();
                                lIUserRepository.UpdateSessionId(_users);

                                //Session Audit Trail
                                int result = lISessionAuditTrailRepository.InsertSessionAuditTrail(_users, "Web", "Open", "");
                                if (result > 0)
                                {
                                    HttpContext.Session.SetString("SessionId", _users.LoginSessionId);
                                    HttpContext.Session.SetString("UserId", _users.UserId);
                                    HttpContext.Session.SetString("UserName", _users.Name);
                                    HttpContext.Session.SetString("UserType", _users.Type.ToString());

                                    HttpContext.SetCookie("UserId", _users.UserId, 5, CookieExpiryIn.Hours);

                                    if (_users.Type.ToString() == "5")
                                        return RedirectToAction("Dashboard", "PatientView", new { area = "Patient", id = _users.UserId });
                                }

                            }
                            else
                            {

                                ViewBag.UserId = _users.UserId;
                                User luser = lIUserRepository.getUser(_users.UserId);
                                string loginsessionid = luser.LoginSessionId;
                                if (luser != null)
                                {
                                    luser.LoginSessionId = Guid.NewGuid().ToString();
                                    lIUserRepository.UpdateSessionId(luser);

                                    //Session Audit Trail
                                    int result = lISessionAuditTrailRepository.InsertSessionAuditTrail(luser, "Web", "Open", loginsessionid);
                                    if (result > 0)
                                    {
                                        HttpContext.Session.SetString("SessionId", luser.LoginSessionId);
                                        HttpContext.Session.SetString("UserId", luser.UserId);
                                        HttpContext.Session.SetString("UserName", luser.Name);
                                        HttpContext.Session.SetString("UserType", luser.Type.ToString());
                                        HttpContext.SetCookie("UserId", luser.UserId, 5, CookieExpiryIn.Hours);
                                        if (luser.Type.ToString() == "5")
                                            return RedirectToAction("Dashboard", "PatientView", new { area = "Patient", id = luser.UserId });
                                    }
                                }
                            }
                        }
                        else
                        {
                            TempData["msg"] = "<script>alert('Invalid Username or Password');</script>";
                            if (!string.IsNullOrEmpty(pUser.ruserid) && !string.IsNullOrEmpty(pUser.rtype))
                                return RedirectToAction("Index", "Login", new { ruserid = pUser.ruserid, rtype = pUser.rtype, rpage = pUser.rpage });

                        }


                    }
                    else
                    {
                        return View();
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [TypeFilter(typeof(LoginAuthorizeAttribute))]
        public IActionResult Dashboard(string id)
        {
            ViewBag.HeaderName = "Progress";
            HttpContext.RemoveCookie("ReviewID");
            JsonSerializerSettings lsetting1 = new JsonSerializerSettings();
            lsetting1.ContractResolver = new CamelCasePropertyNamesContractResolver();
            lsetting1.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            try
            {
                if (!string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                {
                    string patientLoginId = !string.IsNullOrEmpty(id) ? id : HttpContext.Session.GetString("UserId");
                    DashboardView lview = lIPatientRxRepository.getPatientRxByPatientLoginId(patientLoginId);
                    if (lview != null)
                    {
                        //Insert to User Activity Log -Patient
                        JsonSerializerSettings lsetting = new JsonSerializerSettings();
                        lsetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                        List<EquipmentExercise> lExerciseString = lIEquipmentExerciseRepository.GetEquipmentExerciseString();

                        if (!string.IsNullOrEmpty(lview.PatientRx.DeviceConfiguration))
                        {

                            string _uType = HttpContext.Session.GetString("UserType");
                            if (_uType == "5")
                            {

                                ROMChartViewModel ROM = lIPatientRxRepository.getPatientRxROMChart(lview.PatientRx.PatientId, lview.PatientRx.EquipmentType, lview.PatientRx.DeviceConfiguration);
                                if (ROM != null)
                                {
                                    ViewBag.ROM = ROM;
                                }

                                ViewBag.EquipmentType = lview.PatientRx.EquipmentType;



                                //Treatment Calendar
                                List<TreatmentCalendarViewModel> TreatmentCalendarList = lIPatientRxRepository.getTreatmentCalendar(lview.PatientRx.PatientId, lview.PatientRx.EquipmentType, lview.PatientRx.DeviceConfiguration);
                                if (TreatmentCalendarList != null && TreatmentCalendarList.Count > 0)
                                {
                                    ViewBag.TreatmentCalendar = TreatmentCalendarList;
                                }


                            }
                        }
                    }
                    return View(lview);
                }

            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);

            }

            return View(null);
        }

        [TypeFilter(typeof(LoginAuthorizeAttribute))]
        public IActionResult VideoCallPatient()
        {
            ViewBag.HeaderName = "VideoCall";
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionId")))
            //{
            //    int ApiKey = 46201082;
            //    string ApiSecret = "fbef52398143449c9b269e1357d797c64ae945be";
            //    var OpenTok = new OpenTok(ApiKey, ApiSecret);
            //    var session = OpenTok.CreateSession();
            //    // Store this sessionId in the database for later use:
            //    string sessionId = session.Id;
            //    //string token = OpenTok.GenerateToken(sessionId);
            //    double inOneMonth = (DateTime.UtcNow.Add(TimeSpan.FromDays(30)).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            //    string token = session.GenerateToken(role: Role.MODERATOR, expireTime: inOneMonth, data: "name=Prabhu");
            //    HttpContext.Session.SetString("OpenTokSessionId", sessionId);
            //    HttpContext.Session.SetString("Token", token);
            //}
            //ViewBag.PatientName = HttpContext.Session.GetString("UserName");
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                Models.Patient lpatient = lIPatient.GetPatientByPatientLoginId(HttpContext.Session.GetString("UserId"));
                if (lpatient != null)
                {
                    HttpContext.Session.SetString("PatientId", lpatient.PatientLoginId);
                    HttpContext.Session.SetString("PatientName", lpatient.PatientName);
                    HttpContext.Session.SetString("TherapistId", lpatient.Therapistid);

                }
            }
            return View();
        }


        [TypeFilter(typeof(LoginAuthorizeAttribute))]
        public IActionResult PlayVideo(int id)
        {
            try
            {
                ViewBag.HeaderName = "Library";
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                {

                    Library lLibrary = lILibraryRepository.getLibraryById(id);
                    return View(lLibrary);

                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            return View(null);
        }

        public IActionResult Signout(string id = "", string userid = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                switch (id)
                {
                    case "Forced Logout":
                    case "forced logout":
                        if (!string.IsNullOrEmpty(userid))
                        {
                            lISessionAuditTrailRepository.UpdateSessionAuditTrail(userid, "Web", "Forced Logout");
                            HttpContext.RemoveCookie("ReviewID");
                            return RedirectToAction("LoginExisting", "Login", new { userid = userid });
                        }
                        break;
                    case "Clean Logout":
                    case "clean logout":
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                        {
                            User luser = lIUserRepository.getUser(HttpContext.Session.GetString("UserId"));
                            if (luser != null)
                            {
                                if (!string.IsNullOrEmpty(HttpContext.GetCookie("ReviewID")))
                                    UpdateReviewActivityLog(luser.UserId, HttpContext.GetCookie("ReviewID"));

                                luser.LoginSessionId = "";
                                string res = lIUserRepository.UpdateSessionId(luser);
                                if (!string.IsNullOrEmpty(res))
                                {
                                    lISessionAuditTrailRepository.UpdateSessionAuditTrail(luser.UserId, "Web", "Clean Logout");
                                    HttpContext.RemoveCookie("UserId");
                                    HttpContext.RemoveCookie("ReviewID");
                                }

                            }
                        }
                        break;
                    case "Expired":
                    case "expired":
                        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                        {
                            User _luser = lIUserRepository.getUser(HttpContext.GetCookie("UserId"));
                            if (_luser != null)
                            {
                                if (!string.IsNullOrEmpty(HttpContext.GetCookie("ReviewID")))
                                    UpdateReviewActivityLog(_luser.UserId, HttpContext.GetCookie("ReviewID"));

                                _luser.LoginSessionId = "";
                                string res = lIUserRepository.UpdateSessionId(_luser);
                                if (!string.IsNullOrEmpty(res))
                                {
                                    lISessionAuditTrailRepository.UpdateSessionAuditTrail(_luser.UserId, "Web", "Expired");
                                    HttpContext.RemoveCookie("UserId");
                                    HttpContext.RemoveCookie("ReviewID");
                                }
                            }
                            return RedirectToAction("Index", new { id = "Session Expired" });
                        }
                        else
                        {
                            User luser = lIUserRepository.getUser(HttpContext.Session.GetString("UserId"));
                            if (luser != null)
                            {
                                if (!string.IsNullOrEmpty(HttpContext.GetCookie("ReviewID")))
                                    UpdateReviewActivityLog(luser.UserId, HttpContext.GetCookie("ReviewID"));

                                luser.LoginSessionId = "";
                                string res = lIUserRepository.UpdateSessionId(luser);
                                if (!string.IsNullOrEmpty(res))
                                {
                                    lISessionAuditTrailRepository.UpdateSessionAuditTrail(luser.UserId, "Web", "Expired");
                                    HttpContext.RemoveCookie("UserId");
                                    HttpContext.RemoveCookie("ReviewID");
                                }

                            }
                            return RedirectToAction("Index", new { id = "Session Expired" });
                        }


                }
            }


            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public int UpdateReviewActivityLog(string userId, string reviewid)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(reviewid))
                {
                    User luser = lIUserRepository.getUser(userId);
                    if (luser != null)
                    {
                        PatientReview lreview = lIPatientReviewRepository.GetPatientReview(reviewid);
                        if (lreview != null)
                        {
                            lreview.Duration = Convert.ToInt32((DateTime.Now - lreview.StartTimeStamp).TotalSeconds);
                            int _result = lIPatientReviewRepository.UpdatePatientReview(lreview);
                            return _result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }

            return 0;
        }

        public IActionResult PatientLibrary()
        {
            try
            {
                ViewBag.HeaderName = "Library";
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                {
                    Models.Patient lpatient = lIPatient.GetPatientByPatientLoginId(HttpContext.Session.GetString("UserId"));
                    if (lpatient != null)
                    {
                        List<Library> lLibrary = lIPatientLibraryRepository.getPatientLibrary(lpatient.PatientLoginId, lpatient.EquipmentType);
                        return View(lLibrary);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            return View(null);
        }

    }

}