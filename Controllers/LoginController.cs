using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository.Interface;
using Microsoft.Extensions.Logging;
using OneDirect.Models;
using OneDirect.Repository;
using Microsoft.AspNetCore.Http;
using OneDirect.Helper;
using OneDirect.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using OneDirect.Extensions;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Rollbar;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Serialization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    public class LoginController : Controller
    {
        private readonly IPatientReviewInterface lIPatientReviewRepository;
        private readonly IUserActivityLogInterface lIUserActivityLogRepository;
        private readonly IDeviceCalibrationInterface lIDeviceCalibrationRepository;
        private readonly IPatientConfigurationInterface lIPatientConfigurationRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly ISessionAuditTrailInterface lISessionAuditTrailRepository;
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        private OneDirectContext context;
        private readonly IHttpContextAccessor _httpContextAccessor = null;

        public LoginController(OneDirectContext context, ILogger<LoginController> plogger, IHttpContextAccessor httpContextAccessor)
        {
            lIPatientReviewRepository = new PatientReviewRepository(context);
            lIUserActivityLogRepository = new UserActivityLogRepository(context);
            lIDeviceCalibrationRepository = new DeviceCalibrationRepository(context);
            lIPatientConfigurationRepository = new PatientConfigurationRepository(context);
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
            lISessionAuditTrailRepository = new SessionAuditTrailRepository(context);
            this._httpContextAccessor = httpContextAccessor;


        }

        //load the login page
        // GET: /<controller>/
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

        //logout the user with updating the status of session audit trial according to the type of logout 
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


        public IActionResult LoginExisting(string userid = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    User luser = lIUserRepository.getUser(userid);
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

                            if (luser.Type.ToString() == "0")
                                return RedirectToAction("Index", "Patient");
                            if (luser.Type.ToString() == "1")
                                return RedirectToAction("Dashboard", "Support");
                            if (luser.Type.ToString() == "2")
                                return RedirectToAction("Dashboard", "Therapist");
                            if (luser.Type.ToString() == "3")
                                return RedirectToAction("Dashboard", "Provider");
                            if (luser.Type.ToString() == "6")
                                return RedirectToAction("Dashboard", "PatientAdministrator");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            return RedirectToAction("Index", "Login");
        }

        //login the user using valid userId and password and update the session audit trial
        //then redirect to the dashboard according to user
        [HttpPost]
        public IActionResult Index(LoginViewModel pUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(pUser.UserId) && !String.IsNullOrEmpty(pUser.Password))
                    {

                        {
                            User _users = null;
                            if (pUser.UserId.ToString().Trim() == ConfigVars.NewInstance.AdminUserName && pUser.Password.ToString().Trim() == ConfigVars.NewInstance.AdminPassword)
                            {
                                _users = lIUserRepository.getUser(ConfigVars.NewInstance.AdminUserName);
                            }
                            else if (pUser.UserId.ToString().Trim() != ConfigVars.NewInstance.AdminUserName)
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

                                        if (_users.Type.ToString() == "0")
                                            return RedirectToAction("Index", "Patient");
                                        if (_users.Type.ToString() == "1")
                                            return RedirectToAction("Dashboard", "Support");
                                        if (_users.Type.ToString() == "2")
                                            return RedirectToAction("Dashboard", "Therapist");
                                        if (_users.Type.ToString() == "3")
                                            return RedirectToAction("Dashboard", "Provider");
                                        if (_users.Type.ToString() == "6")
                                            return RedirectToAction("Dashboard", "PatientAdministrator");
                                    }

                                }
                                else if (string.IsNullOrEmpty(_users.LoginSessionId) && !string.IsNullOrEmpty(pUser.ruserid) && !string.IsNullOrEmpty(pUser.rtype))
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

                                        if ((Utilities.DecryptText(pUser.ruserid) == _users.UserId) && Utilities.DecryptText(pUser.rtype) == _users.Type.ToString())
                                        {
                                            if (!string.IsNullOrEmpty(pUser.rpage) && Utilities.DecryptText(pUser.rpage).ToLower() == "appointment")
                                                return RedirectToAction("Index", "Appointments");
                                            else
                                                return RedirectToAction("Index", "MessageView");
                                        }
                                        else
                                        {
                                            TempData["msg"] = "<script>alert('Invalid Username or Password');</script>";
                                            return RedirectToAction("Index", "Login", new { ruserid = pUser.ruserid, rtype = pUser.rtype, rpage = pUser.rpage });
                                        }
                                    }
                                }
                                else
                                {

                                    ViewBag.UserId = _users.UserId;
                                }
                            }
                            else
                            {
                                TempData["msg"] = "<script>alert('Invalid Username or Password');</script>";
                                if (!string.IsNullOrEmpty(pUser.ruserid) && !string.IsNullOrEmpty(pUser.rtype))
                                    return RedirectToAction("Index", "Login", new { ruserid = pUser.ruserid, rtype = pUser.rtype, rpage = pUser.rpage });

                            }

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


        //saving timezone 
        [HttpPost]
        public JsonResult savetimezone(string timezoneoffset = "", string timezoneid = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(timezoneoffset))
                {
                    HttpContext.Session.SetString("timezoneoffset", timezoneoffset);

                }
                if (!string.IsNullOrEmpty(timezoneid))
                {
                    HttpContext.Session.SetString("timezoneid", timezoneid);
                }

                return Json(new { result = "success" });
            }
            catch (Exception ex)
            {
                return Json("");
            }

        }



        [HttpGet]
        public JsonResult getUserId()
        {
            try
            {
                string userid = HttpContext.Session.GetString("UserId");

                if (!string.IsNullOrEmpty(userid))
                    return Json(new { result = "success" });
            }
            catch (Exception ex)
            {
                return Json("");
            }
            return Json("");
        }


        //get the patient review and update the patient review
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



    }
}
