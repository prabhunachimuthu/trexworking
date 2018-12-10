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
using OneDirect.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneDirect.Extensions;
using System.IO;
using CsvHelper;
using System.Net.Http.Headers;
using OneDirect.Helper;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class SessionViewController : Controller
    {
        private readonly IEquipmentExerciseInterface lIEquipmentExerciseRepository;
        private readonly IRomChangeLogInterface IRomChangeLog;
        private readonly IPatientRxInterface IPatientRx;
        private readonly IUserInterface lIUserRepository;
        private readonly ISessionInterface lISessionInterface;
        private readonly INewPatient INewPatient;
        private readonly IProtocolInterface lIProtocolRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public SessionViewController(OneDirectContext context, ILogger<SessionViewController> plogger, INewPatient lINewPatient, IPatientRxInterface lIPatientRx, IEquipmentExerciseInterface IEquipmentExerciseRepository)
        {
            lIEquipmentExerciseRepository = IEquipmentExerciseRepository;
            logger = plogger;
            this.context = context;
            IRomChangeLog = new RomChangeLogRepository(context);
            IPatientRx = lIPatientRx;
            lIUserRepository = new UserRepository(context);
            lISessionInterface = new SessionRepository(context);
            INewPatient = lINewPatient;
            lIProtocolRepository = new ProtocolRepository(context);
        }

        public IActionResult Index(string id, string Username = "", string protocolid = "", string protocolName = "", string Etype = "", string actuator = "")
        {
            var response = new Dictionary<string, object>();
            try
            {

                if (!String.IsNullOrEmpty(id))
                {
                    ViewBag.actuator = actuator;
                    ViewBag.EquipmentType = Etype;
                    ViewBag.patientId = id;
                    ViewBag.PatientName = Username;
                    HttpContext.Session.SetString("PatientID", id);
                }
                if (string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(Etype))
                {
                    if (Etype.ToLower() == "shoulder")
                    {
                        actuator = "Forward Flexion";
                    }
                    else
                    {
                        actuator = "Flexion-Extension";
                    }
                    ViewBag.actuator = actuator;
                }
                logger.LogDebug("Pain Post Start");
                if (!String.IsNullOrEmpty(Username))
                    HttpContext.Session.SetString("PatientName", Username);
                if (HttpContext.Session.GetString("PatientName") != null && HttpContext.Session.GetString("PatientName").ToString() != "")
                {
                    if (!String.IsNullOrEmpty(Username))
                        ViewBag.PatientName = HttpContext.Session.GetString("PatientName").ToString();
                }
                List<UserSession> pSession = new List<UserSession>();
                if (String.IsNullOrEmpty(protocolid))
                {
                    HttpContext.Session.SetString("ProtocolName", "");
                    HttpContext.Session.SetString("ProtocoloId", "");
                    if (!String.IsNullOrEmpty(Username) && !String.IsNullOrEmpty(id))
                        pSession = lISessionInterface.getSessionList(id, actuator);
                    else
                    {
                        if (String.IsNullOrEmpty(Username) && String.IsNullOrEmpty(id) && HttpContext.Session.GetString("UserId") != null && HttpContext.Session.GetString("UserType") != null && HttpContext.Session.GetString("UserType").ToString() == "2")
                        {
                            pSession = lISessionInterface.getSessionListByTherapistId(HttpContext.Session.GetString("UserId"));
                        }
                        else
                        {
                            pSession = lISessionInterface.getSessionList();
                        }
                    }
                }
                else
                {
                    HttpContext.Session.SetString("PatientName", "");
                    HttpContext.Session.SetString("PatientID", "");
                    HttpContext.Session.SetString("ProtocolName", protocolName);
                    HttpContext.Session.SetString("ProtocoloId", protocolid);
                    if (!String.IsNullOrEmpty(protocolName))
                    {
                        ViewBag.PatientName = protocolName;
                    }
                    pSession = lISessionInterface.getSessionListByProtocoloId(protocolid);
                }
                if (pSession != null && pSession.Count > 0)
                    ViewBag.SessionList = pSession;

                return View();
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
        }
        public IActionResult Delete(string sessionId, int patId = 0, string patName = "", string etype = "", string returnView = "")
        {
            string deviceConfiguration = string.Empty;
            try
            {
                Session lsession = lISessionInterface.getSession(sessionId);
                if (lsession != null)
                {
                    Protocol lprotocol = context.Protocol.FirstOrDefault(x => x.ProtocolId == lsession.ProtocolId);
                    if (lprotocol != null)
                    {
                        deviceConfiguration = lprotocol.DeviceConfiguration;
                    }
                    string result = lISessionInterface.DeleteSessionRecordsWithCasecade(sessionId);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            if (!string.IsNullOrEmpty(returnView))
            {
                return RedirectToAction("Index", "Review", new { id = patId, Username = patName, EquipmentType = etype, actuator = deviceConfiguration, tab = "Sessions" });
            }
            else
            {
                return RedirectToAction("Index", "SessionView", new { id = patId, Username = patName, Etype = etype });
            }
        }
        public IActionResult AddEdit(int id, string Username = "", string Etype = "", string actuator = "", string returnView = "", string sessionid = "")
        {
            try
            {
                ViewBag.flexionString = "Flexion";
                ViewBag.extensionString = "Extension";
                List<EquipmentExercise> lExerciseString = lIEquipmentExerciseRepository.GetEquipmentExerciseString();
                if (lExerciseString != null && lExerciseString.Count > 0 && Etype == "Shoulder")
                {
                    var lexString = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == actuator).FirstOrDefault();
                    if (lexString != null)
                    {
                        ViewBag.flexionString = lexString.FlexionString;
                    }
                }
                else if (lExerciseString != null && lExerciseString.Count > 0)
                {
                    var lexString = lExerciseString.Where(x => x.Limb == Etype).FirstOrDefault();
                    if (lexString != null)
                    {
                        ViewBag.flexionString = lexString.FlexionString;
                        ViewBag.extensionString = lexString.ExtensionString;
                    }
                }
                if (string.IsNullOrEmpty(sessionid))
                {
                    List<NewProtocol> ptoList = INewPatient.GetProtocolListBypatId(id.ToString());
                    if (Etype == "Shoulder")
                    {
                        ptoList = ptoList.Where(p => p.ExcerciseEnum == actuator).ToList();
                    }
                    else
                    {
                        ptoList = ptoList.Where(p => p.ExcerciseEnum == "Flexion-Extension").ToList();
                    }
                    List<SelectListItem> list = new List<SelectListItem>();
                    foreach (NewProtocol ex in ptoList)
                    {

                        list.Add(new SelectListItem { Text = ex.ProtocolName.ToString(), Value = ex.ProtocolId.ToString() });
                    }
                    ViewBag.Protocol = list;
                    SessionView sv = new SessionView();
                    sv.PatientId = id;
                    sv.Patname = Username;
                    sv.EType = Etype;
                    sv.EEnum = actuator;

                    if (!string.IsNullOrEmpty(returnView))
                    {
                        sv.returnView = returnView;
                    }
                    return View(sv);
                }
                else
                {
                    Session lSession = lISessionInterface.getSession(sessionid);
                    if (lSession != null)
                    {
                        SessionView sv = SessionExtension.SessionToSessionView(lSession);
                        if (sv != null)
                        {
                            List<NewProtocol> ptoList = INewPatient.GetProtocolListBypatId(sv.PatientId.ToString());
                            if (Etype == "Shoulder")
                            {
                                ptoList = ptoList.Where(p => p.ExcerciseEnum == actuator).ToList();
                            }
                            else
                            {
                                ptoList = ptoList.Where(p => p.ExcerciseEnum == "Flexion-Extension").ToList();
                            }
                            List<SelectListItem> list = new List<SelectListItem>();
                            foreach (NewProtocol ex in ptoList)
                            {
                                list.Add(new SelectListItem { Text = ex.ProtocolName.ToString(), Value = ex.ProtocolId.ToString() });
                            }
                            ViewBag.Protocol = list;
                            sv.PatientId = id;
                            sv.Patname = Username;
                            sv.EType = Etype;
                            sv.EEnum = actuator;

                            if (!string.IsNullOrEmpty(returnView))
                            {
                                sv.returnView = returnView;
                            }
                        }
                        return View(sv);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);

            }
            return View();
        }
        [HttpPost]
        public IActionResult AddEdit(SessionView session)
        {
            try
            {
                NewProtocol ptoList = INewPatient.GetProtocolByproId(session.ProtocolId);
                if (ptoList != null)
                {
                    PatientRx lrx = IPatientRx.getPatientRx(ptoList.RxId);
                    if (lrx != null)
                    {
                        Session _session = new Session();


                        _session.PatientId = ptoList.PatientId;
                        _session.RxId = ptoList.RxId;
                        _session.ProtocolId = ptoList.ProtocolId;
                        _session.SessionDate = session.SessionDate;
                        _session.Duration = session.Duration;
                        _session.Reps = session.Reps;
                        _session.MaxExtension = session.MaxExtension;
                        _session.ExtensionReps = session.ExtensionReps;
                        _session.MaxFlexion = session.MaxFlexion;
                        _session.FlexionReps = session.FlexionReps;
                        _session.MaxPain = session.MaxPain;
                        _session.PainCount = session.PainCount;
                        _session.Boom1Position = session.Boom1Position;
                        _session.Boom2Position = session.Boom2Position;
                        _session.RangeDuration1 = session.RangeDuration1;
                        _session.RangeDuration2 = session.RangeDuration2;
                        _session.GuidedMode = _session.GuidedMode;
                        _session.TimeZoneOffset = "";

                        if (session.MaxFlexion > lrx.CurrentFlexion && session.MaxExtension > lrx.CurrentExtension)
                        {
                            int res = INewPatient.ChangeRxCurrent(lrx.RxId, session.MaxFlexion, session.MaxExtension, "Patient");
                        }
                        else if (session.MaxFlexion > lrx.CurrentFlexion)
                        {
                            int res = INewPatient.ChangeRxCurrentFlexion(lrx.RxId, session.MaxFlexion, "Patient");
                        }
                        else if (session.MaxExtension > lrx.CurrentExtension)
                        {
                            int res = INewPatient.ChangeRxCurrentExtension(lrx.RxId, session.MaxExtension, "Patient");
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
                        if (!string.IsNullOrEmpty(session.SessionId))
                        {
                            _session.SessionId = session.SessionId;

                            //Session editSession = lISessionInterface.getSession(session.SessionId);
                            //if (editSession != null)
                            //{
                            lISessionInterface.UpdateSession(_session);
                            //}
                        }
                        else
                        {
                            _session.SessionId = Guid.NewGuid().ToString();
                            lISessionInterface.InsertSession(_session);
                        }

                    }
                }
                if (!string.IsNullOrEmpty(session.returnView))
                {
                    return RedirectToAction("Index", "Review", new { id = session.PatientId, Username = session.Patname, EquipmentType = session.EType, actuator = session.EEnum, tab = "Sessions" });
                }
                else
                {
                    return RedirectToAction("Index", "SessionView", new { id = session.PatientId, Username = session.Patname, Etype = session.EType, actuator = session.EEnum });
                }
            }


            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
        }

        [HttpPost]
        public IActionResult ImportSessions(IFormFile fileInput, int id, string Username = "", string Etype = "", string actuator = "", string returnView = "")
        {
            try
            {
                if (fileInput == null || fileInput.Length == 0)
                {
                    TempData["msg"] = "<script>Helpers.ShowMessage('File is not selected', 1);</script>";
                }
                else if (fileInput != null && !string.IsNullOrEmpty(fileInput.FileName.Trim()))
                {
                    if (fileInput.FileName.Remove(0, fileInput.FileName.LastIndexOf(".") + 1) != "csv")
                    {
                        TempData["msg"] = "<script>Helpers.ShowMessage('Invalid csv file for Session upload', 1);</script>";
                    }
                    else
                    {
                        List<SessionImport> lsessions = new List<SessionImport>();
                        using (var reader = new StreamReader(fileInput.OpenReadStream()))
                        {
                            using (var csv = new CsvReader(reader))
                            {
                                csv.Configuration.IgnoreQuotes = true;
                                csv.Configuration.IgnoreReferences = true;
                                csv.Configuration.DetectColumnCountChanges = true;
                                csv.Configuration.IgnoreBlankLines = true;
                                lsessions = csv.GetRecords<SessionImport>().ToList();
                            }
                        }
                        if (lsessions != null && lsessions.Count > 0)
                        {
                            Protocol lprotocols = lIProtocolRepository.getProtocol(lsessions);
                            if (lprotocols != null)
                            {

                                for (int i = 0; i < lsessions.Count; i++)
                                {
                                    Session pSession = new Session();
                                    pSession.SessionId = Guid.NewGuid().ToString();
                                    if (!string.IsNullOrEmpty(lsessions[i].ProtocolId))
                                    {
                                        Protocol lprotocol = lIProtocolRepository.getProtocol(lsessions[i].ProtocolId);
                                        if (lprotocol != null)
                                        {
                                            pSession.PatientId = lprotocol.PatientId;
                                            pSession.RxId = lprotocol.RxId;
                                            pSession.ProtocolId = lprotocol.ProtocolId;
                                            pSession.Reps = !string.IsNullOrEmpty(lsessions[i].Reps) ? Convert.ToInt32(lsessions[i].Reps) : (int?)null;
                                            pSession.Duration = !string.IsNullOrEmpty(lsessions[i].Duration) ? Convert.ToInt32(lsessions[i].Duration) : (int?)null;
                                            pSession.FlexionReps = !string.IsNullOrEmpty(lsessions[i].FlexionReps) ? Convert.ToInt32(lsessions[i].FlexionReps) : (int?)null;
                                            pSession.ExtensionReps = !string.IsNullOrEmpty(lsessions[i].ExtensionReps) ? Convert.ToInt32(lsessions[i].ExtensionReps) : (int?)null;
                                            pSession.SessionDate = !string.IsNullOrEmpty(lsessions[i].SessionDate) ? Convert.ToDateTime(lsessions[i].SessionDate) : (DateTime?)null;
                                            pSession.PainCount = (int?)null;
                                            pSession.MaxFlexion = !string.IsNullOrEmpty(lsessions[i].MaxFlexion) ? Convert.ToInt32(lsessions[i].MaxFlexion) : 0;
                                            pSession.MaxExtension = !string.IsNullOrEmpty(lsessions[i].MaxExtension) ? Convert.ToInt32(lsessions[i].MaxExtension) : 0;
                                            pSession.MaxPain = !string.IsNullOrEmpty(lsessions[i].MaxPain) ? Convert.ToInt32(lsessions[i].MaxPain) : 0;
                                            pSession.Boom1Position = !string.IsNullOrEmpty(lsessions[i].Boom1Position) ? Convert.ToInt32(lsessions[i].Boom1Position) : (int?)null;
                                            pSession.Boom2Position = !string.IsNullOrEmpty(lsessions[i].Boom2Position) ? Convert.ToInt32(lsessions[i].Boom2Position) : (int?)null;
                                            pSession.RangeDuration1 = !string.IsNullOrEmpty(lsessions[i].RangeDuration1) ? Convert.ToInt32(lsessions[i].RangeDuration1) : (int?)null;
                                            pSession.RangeDuration2 = !string.IsNullOrEmpty(lsessions[i].RangeDuration2) ? Convert.ToInt32(lsessions[i].RangeDuration2) : (int?)null;
                                            pSession.GuidedMode = !string.IsNullOrEmpty(lsessions[i].GuidedMode) ? Convert.ToInt32(lsessions[i].GuidedMode) : (int?)null;
                                            pSession.TimeZoneOffset = "";

                                            lISessionInterface.InsertSession(pSession);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                TempData["msg"] = "<script>Helpers.ShowMessage('No Protocol is matching with the uploaded sessions', 1);</script>";
                            }
                        }
                        else
                        {
                            TempData["msg"] = "<script>Helpers.ShowMessage('No session records available in the import format', 1);</script>";
                        }
                    }
                }
                else
                {
                    TempData["msg"] = "<script>Helpers.ShowMessage('Please attach a file for Session Upload', 1);</script>";
                }

            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);

            }

            return RedirectToAction("Index", "Review", new { id = id, Username = Username, EquipmentType = Etype, actuator = actuator, tab = "Sessions" });
        }
    }
}
