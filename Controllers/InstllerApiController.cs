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
using OneDirect.Helper;
using System.Data;
using OneDirect.ViewModels;
using Newtonsoft.Json;
using OneDirect.Vsee;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    public class InstallerApiController : Controller
    {

        private readonly ISessionAuditTrailInterface lISessionAuditTrailRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly IPatient IPatient;
        private readonly ILogger logger;
        private OneDirectContext context;

        public InstallerApiController(OneDirectContext context, ILogger<PatientApiController> plogger)
        {
            logger = plogger;
            this.context = context;
            IPatient = new PatientRepository(context);
            lIUserRepository = new UserRepository(context);

            lISessionAuditTrailRepository = new SessionAuditTrailRepository(context);
        }

        [HttpGet]
        [Route("installerlogin")]
        public JsonResult installerlogin(string installerid, string password)
        {
            try
            {
                User _result = null;
                string loginsessionId = "";
                if (!string.IsNullOrEmpty(installerid) && !string.IsNullOrEmpty(password))
                {
                    _result = lIUserRepository.getUser(installerid.ToLower(), password, 4);
                    if (_result == null)
                    {
                        _result = lIUserRepository.getUser(installerid.ToUpper(), password, 4);
                    }
                }
                if (_result != null)
                {
                    loginsessionId = _result.LoginSessionId;
                    _result = lIUserRepository.userLogin(_result.UserId, password, 4);
                    if (_result != null)
                    {
                        if (!string.IsNullOrEmpty(loginsessionId))
                        {
                            lISessionAuditTrailRepository.UpdateSessionAuditTrail(_result.UserId, "API", "Forced Logout");
                        }
                        lISessionAuditTrailRepository.InsertSessionAuditTrail(_result, "API", "Open", loginsessionId);
                        return Json(new { Status = (int)HttpStatusCode.OK, Installer = _result, SessionId = _result.LoginSessionId, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, SessionId = "", result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, SessionId = "", result = "invalid username or password", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, SessionId = "", result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }

        }


        [HttpGet]
        [Route("encryptinstallerlogin")]
        public JsonResult encryptpatientlogin(string installerid, string password)
        {
            try
            {
                User _result = null;
                string loginsessionId = "";
                if (!string.IsNullOrEmpty(installerid) && !string.IsNullOrEmpty(password))
                {
                    _result = lIUserRepository.getUserWithEncryptPassword(installerid.ToLower(), password, 4);
                    if (_result == null)
                    {
                        _result = lIUserRepository.getUserWithEncryptPassword(installerid.ToUpper(), password, 4);
                    }
                }
                if (_result != null)
                {
                    loginsessionId = _result.LoginSessionId;
                    _result = lIUserRepository.userLogin(_result.UserId, _result.Password, 4);
                    if (_result != null)
                    {
                        if (!string.IsNullOrEmpty(loginsessionId))
                        {
                            lISessionAuditTrailRepository.UpdateSessionAuditTrail(_result.UserId, "API", "Forced Logout");
                        }
                        lISessionAuditTrailRepository.InsertSessionAuditTrail(_result, "API", "Open", loginsessionId);
                        return Json(new { Status = (int)HttpStatusCode.OK, Installer = _result, SessionId = _result.LoginSessionId, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, SessionId = "", result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, SessionId = "", result = "invalid username or password", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, SessionId = "", result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }


        [HttpGet]
        [Route("createencryptpassword")]
        public JsonResult createencryptpassword(string installerid, string password)
        {
            try
            {
                User _result = null;
                if (!string.IsNullOrEmpty(installerid) && !string.IsNullOrEmpty(password))
                {
                    _result = lIUserRepository.getUser(installerid.ToLower());
                    if (_result != null)
                    {
                        _result.EncryptPasswrod = password;
                        string res = lIUserRepository.UpdateUser(_result);
                        if (!string.IsNullOrEmpty(res) && res == "success")
                            return Json(new { Status = (int)HttpStatusCode.OK, Installer = _result, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                        else
                            return Json(new { Status = (int)HttpStatusCode.Created, result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Created, result = "invalid User", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "request string is not proper", TimeZone = DateTime.UtcNow.ToString("s") });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
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
    }

}
