using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Helper;
using OneDirect.Repository.Interface;
using OneDirect.Models;
using Microsoft.Extensions.Logging;
using OneDirect.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneDirect.ViewModels;
using OneDirect.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.DependencyInjection;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class SessionAuditTrailController : Controller
    {
        private readonly IEquipmentExerciseInterface lIEquipmentExerciseRepository;
        private readonly INewPatient INewPatient;
        private readonly IPatientReviewInterface lIPatientReviewRepository;
        private readonly IUserActivityLogInterface lIUserActivityLogRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly ISessionAuditTrailInterface lISessionAuditTrailInterface;
        private readonly IPatientRxInterface lIPatientRxRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public SessionAuditTrailController(OneDirectContext context, ILogger<SessionAuditTrailController> plogger, IPatientRxInterface IPatientRxRepository, INewPatient lINewPatient, IEquipmentExerciseInterface IEquipmentExerciseRepository)
        {
            logger = plogger;
            this.context = context;
            lIEquipmentExerciseRepository = IEquipmentExerciseRepository;
            INewPatient = lINewPatient;
            lIPatientReviewRepository = new PatientReviewRepository(context);
            lIUserRepository = new UserRepository(context);
            lIUserActivityLogRepository = new UserActivityLogRepository(context);
            lISessionAuditTrailInterface = new SessionAuditTrailRepository(context);
            lIPatientRxRepository = IPatientRxRepository;
        }

        // GET: /<controller>/
        public IActionResult Index(string id = "", string userid = "")
        {

            List<SessionAuditTrail> ptrailList = new List<SessionAuditTrail>();
            try
            {

                if (!string.IsNullOrEmpty(id) && id == "Session")
                {
                    ptrailList = lISessionAuditTrailInterface.GetSessionAuditTrail();

                    ViewBag.Session = ptrailList;
                    ViewBag.page = "Session";
                }
                else if (!string.IsNullOrEmpty(id) && id == "Activity")
                {

                    if (!string.IsNullOrEmpty(userid))
                    {
                        User luser = lIUserRepository.getUser(userid);
                        if (luser != null)
                        {

                            List<UserActivityLog> luseractivity = lIUserActivityLogRepository.UserActivityViewList2(userid);
                            List<UserActivityLog> luseractivity1 = lIUserActivityLogRepository.UserActivityViewList2();
                            ViewBag.Activity1 = luseractivity1;
                            ViewBag.Activity = luseractivity;
                            ViewBag.page = "Activity";
                            ViewBag.SelectedUser = luser.Name;
                        }
                        else
                        {
                            ViewBag.page = "Activity";
                            ViewBag.SelectedUser = "All";
                        }

                    }
                    else
                    {

                        List<UserActivityLog> luseractivity = lIUserActivityLogRepository.UserActivityViewList2();
                        List<UserActivityLog> luseractivity1 = lIUserActivityLogRepository.UserActivityViewList2();
                        ViewBag.Activity1 = luseractivity1;
                        ViewBag.Activity = luseractivity;
                        ViewBag.page = "Activity";
                        ViewBag.SelectedUser = "All";
                    }
                }
                else
                {
                    ptrailList = lISessionAuditTrailInterface.GetSessionAuditTrail();
                    ViewBag.Session = ptrailList;
                    ViewBag.page = "Session";

                }

                return View();
            }
            catch (Exception ex)
            {
                logger.LogDebug("Session Audit Trail Error: " + ex);
                return View();
            }

        }

        //give the list of patient reviews
        public IActionResult PatientReview()
        {

            List<PatientReview> pReviewList = new List<PatientReview>();
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") != ConstantsVar.Admin.ToString())
                {
                    pReviewList = lIPatientReviewRepository.GetPatientReviewList(HttpContext.Session.GetString("UserId"));
                }
                else
                {
                    pReviewList = lIPatientReviewRepository.GetPatientReviewList();
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Patient Review Error: " + ex);
                return View();
            }
            return View(pReviewList);
        }

        [HttpPost]
        public JsonResult GetSessionAuditTrail(IFormCollection formCollection)
        {
            List<SessionAuditTrail> ptrailList = null;
            List<SessionAuditTrailView> ptrailListView = new List<SessionAuditTrailView>();
            int recordsTotal = 0, recordsFiltered = 0;
            Microsoft.Extensions.Primitives.StringValues pdraw = string.Empty;
            string draw = string.Empty;

            try
            {
                HttpContext.Request.Form.TryGetValue("draw", out pdraw);
                draw = pdraw.FirstOrDefault();
                Microsoft.Extensions.Primitives.StringValues start1;
                Microsoft.Extensions.Primitives.StringValues length1;
                Microsoft.Extensions.Primitives.StringValues search1;

                HttpContext.Request.Form.TryGetValue("start", out start1);
                var start = start1.FirstOrDefault();
                HttpContext.Request.Form.TryGetValue("length", out length1);
                var length = length1.FirstOrDefault();
                HttpContext.Request.Form.TryGetValue("search[value]", out search1);
                var search = search1.FirstOrDefault();


                ////Find Order Column

                Microsoft.Extensions.Primitives.StringValues sortColumn1;
                Microsoft.Extensions.Primitives.StringValues orderBy1;

                HttpContext.Request.Form.TryGetValue("order[0][column]", out sortColumn1);
                var sortColumn = sortColumn1.FirstOrDefault();
                HttpContext.Request.Form.TryGetValue("order[0][dir]", out orderBy1);
                var orderBy = orderBy1.FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;



                Console.WriteLine("Start Time :" + DateTime.Now);
                ptrailList = lISessionAuditTrailInterface.GetSessionAuditTrail(sortColumn, orderBy, search, skip, pageSize, ref recordsTotal, ref recordsFiltered);
                Console.WriteLine("End Time Query :" + DateTime.Now);
                if (ptrailList != null && ptrailList.Count > 0)
                {


                    ptrailListView = ptrailList.Select(x => new SessionAuditTrailView
                    {
                        AuditTrailId = x.AuditTrailId,
                        SessionId = x.SessionId,
                        SessionType = x.SessionType,
                        SessionStatus = x.SessionStatus,
                        LinkedSession = x.LinkedSession,
                        SessionOpenTime = Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(x.SessionOpenTime, HttpContext.Session.GetString("timezoneid"))).ToString("MMM-dd-yyyy hh:mm:ss tt"),
                        SessionClosedTime = x.SessionClosedTime != null ? Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(x.SessionClosedTime.Value, HttpContext.Session.GetString("timezoneid"))).ToString("MMM-dd-yyyy hh:mm:ss tt") : "",

                        UserId = x.UserId,
                        Type = x.Type,
                        Name = x.Name,
                        EmailId = x.EmailId,
                        PasswordUsed = x.PasswordUsed

                    }).ToList();
                }
                Console.WriteLine("End Time :" + DateTime.Now);
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            return Json(new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = ptrailListView });
        }

        public IActionResult ViewActivities(string sessionid = "", string reviewid = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(sessionid) && string.IsNullOrEmpty(reviewid))
                {

                    List<UserActivityLog> luseractivity = context.UserActivityLog.Where(x => x.SessionId == sessionid).ToList();
                    ViewBag.Activity = luseractivity;
                }
                else if (string.IsNullOrEmpty(sessionid) && !string.IsNullOrEmpty(reviewid))
                {

                    List<UserActivityLog> luseractivity = context.UserActivityLog.Where(x => x.ReviewId == reviewid).ToList();
                    ViewBag.Activity = luseractivity;
                }
                ViewBag.page = "Activity";
                return View();
            }
            catch (Exception ex)
            {
                logger.LogDebug("Session Audit Trail Error: " + ex);
                return View();
            }

        }


        public IActionResult ViewChanges(string sessionid = "", string reviewid = "", string emailid = "")
        {
            PatientReviewReport lreport = new PatientReviewReport();
            PatientDetails lpatient = null;
            List<ROMReport> lRom = new List<ROMReport>();
            JsonSerializerSettings lsetting = new JsonSerializerSettings();
            lsetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            try
            {
                if (!string.IsNullOrEmpty(emailid))
                {
                    TempData["msg"] = "<script>Helpers.ShowMessage('SOAP report is emailed to your emailid " + emailid + "', 1);</script>";
                }
                if (!string.IsNullOrEmpty(reviewid))
                {


                    PatientReview lreview = lIPatientReviewRepository.GetPatientReview(reviewid);
                    if (lreview != null)
                    {
                        lreport.Review = lreview;
                        if (!string.IsNullOrEmpty(lreview.PatientId))
                        {
                            //Getting Patient Details
                            NewPatient lnewPatient = INewPatient.GetPatientByPatId(Convert.ToInt32(lreview.PatientId));
                            if (lnewPatient != null)
                            {
                                lpatient = new PatientDetails();
                                lpatient.PatientId = lnewPatient.PatientId;
                                lpatient.PatientName = lnewPatient.PatientName;
                                lpatient.ProviderId = lnewPatient.ProviderId;
                                lpatient.ProviderName = context.User.FirstOrDefault(x => x.UserId == lnewPatient.ProviderId).Name;
                                lpatient.PatId = lnewPatient.PatientAdminId;
                                lpatient.PatAdminName = context.User.FirstOrDefault(x => x.UserId == lnewPatient.PatientAdminId).Name;
                                lpatient.TherapistId = lnewPatient.TherapistId;
                                lpatient.TherapistName = context.User.FirstOrDefault(x => x.UserId == lnewPatient.TherapistId).Name;
                                lpatient.DateofBirth = lnewPatient.Dob.HasValue ? lnewPatient.Dob.Value.ToString("MM-dd-yyyy") : "";

                                lpatient.EvaluationDate = DateTime.Now.ToString("MM-dd-yyyy");
                                lpatient.Evaluator = HttpContext.Session.GetString("UserName");
                                lpatient.Side = lnewPatient.Side;
                                lpatient.EquipmentType = lnewPatient.EquipmentType;
                                lpatient.SurgeryDate = lnewPatient.SurgeryDate.HasValue ? lnewPatient.SurgeryDate.Value.ToString("MM-dd-yyyy") : "";
                                lreport.Patient = lpatient;

                                List<EquipmentExercise> lExerciseString = lIEquipmentExerciseRepository.GetEquipmentExerciseString();
                                if (lExerciseString != null && lExerciseString.Count > 0 && lpatient.EquipmentType == "Shoulder")
                                {
                                    var lexString = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "Forward Flexion").FirstOrDefault();
                                    if (lexString != null)
                                    {
                                        ViewBag.flexionString = lexString.FlexionString;
                                    }
                                    var lexString1 = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "External Rotation").FirstOrDefault();
                                    if (lexString1 != null)
                                    {
                                        ViewBag.flexionString1 = lexString1.FlexionString;
                                    }
                                }
                                else if (lExerciseString != null && lExerciseString.Count > 0)
                                {
                                    var lexString = lExerciseString.Where(x => x.Limb == lpatient.EquipmentType).FirstOrDefault();
                                    if (lexString != null)
                                    {
                                        ViewBag.flexionString = lexString.FlexionString;
                                        ViewBag.extensionString = lexString.ExtensionString;
                                    }
                                }
                            }
                            List<UserActivityLog> lActivityList = lIUserActivityLogRepository.UserActivityListByReviewId(reviewid);
                            if (lActivityList != null && lActivityList.Count > 0)
                            {
                                //Getting Rx record in the User activities
                                UserActivityLog lRxActivity = lActivityList.OrderBy(x => x.StartTimeStamp).FirstOrDefault(x => x.RecordType == "Rx" && x.RecordChangeType == "Edit");
                                if (lRxActivity != null)
                                {
                                    //Getting Rx records before it changed
                                    List<PatientRx> PatientRxList = JsonConvert.DeserializeObject<List<PatientRx>>(lRxActivity.RecordChangeType == "Edit" ? lRxActivity.RecordExistingJson : lRxActivity.RecordJson, lsetting);
                                    if (PatientRxList != null && PatientRxList.Count > 0)
                                    {
                                        foreach (PatientRx rx in PatientRxList)
                                        {
                                            ROMReport lromReport = new ROMReport();
                                            lromReport.Exercise = rx.DeviceConfiguration;
                                            lromReport.GoalFlexion = rx.GoalFlexion.ToString();
                                            lromReport.FlexionAchieved = rx.CurrentFlexion.ToString();
                                            if (rx.EquipmentType != "Shoulder")
                                            {
                                                lromReport.GoalExtension = rx.GoalExtension.ToString();
                                                lromReport.ExtensionAchieved = rx.CurrentExtension.ToString();
                                            }
                                            lromReport.PainLevel = context.Pain.Where(x => x.RxId == rx.RxId).Count() > 0 ? context.Pain.Where(x => x.RxId == rx.RxId).Max(x => x.PainLevel).Value.ToString() : "";
                                            lRom.Add(lromReport);
                                        }
                                        lreport.ROM = lRom;
                                    }

                                    //Getting Rx records changed properties
                                    List<UserActivityLog> lRxActivityList = lActivityList.OrderBy(x => x.StartTimeStamp).Where(x => x.RecordType == "Rx" && x.RecordChangeType == "Edit").ToList();

                                    if (lRxActivityList != null && lRxActivityList.Count > 0)
                                    {
                                        IDictionary<string, IDictionary<string, string>> ldictnaryList = new Dictionary<string, IDictionary<string, string>>();
                                        int k = 0;
                                        foreach (UserActivityLog lactivity in lRxActivityList)
                                        {
                                            string[] ignore = new string[] { "DateModified", "DateCreated" };

                                            List<PatientRx> lrxChangedCurrentList = JsonConvert.DeserializeObject<List<PatientRx>>(lactivity.RecordJson, lsetting);
                                            List<PatientRx> lrxChangedOldList = JsonConvert.DeserializeObject<List<PatientRx>>(lactivity.RecordExistingJson, lsetting);
                                            for (int i = 0; i < lrxChangedCurrentList.Count; i++)
                                            {
                                                IDictionary<string, string> ldict = PublicInstancePropertiesEqual(lrxChangedOldList[i], lrxChangedCurrentList[i], ignore);
                                                if (!ldict.ContainsKey(lrxChangedOldList[i].DeviceConfiguration) && !ldictnaryList.ContainsKey(lrxChangedOldList[i].DeviceConfiguration) && ldict.Count > 0)
                                                {
                                                    ldictnaryList.Add(lrxChangedOldList[i].DeviceConfiguration, ldict);
                                                }
                                                else
                                                {
                                                    //ldict.Remove(lrxChangedOldList[i].DeviceConfiguration);
                                                    //ldictnaryList.Remove(lrxChangedOldList[i].DeviceConfiguration);
                                                    ldictnaryList.Add(lrxChangedOldList[i].DeviceConfiguration + "_" + k, ldict);
                                                }
                                            }
                                            k++;

                                        }
                                        lreport.ChangeList = ldictnaryList;
                                    }

                                }
                                else
                                {
                                    List<PatientRx> lrxList = lIPatientRxRepository.getPatientRxByPatientId(lreview.PatientId);
                                    if (lrxList != null && lrxList.Count > 0)
                                    {
                                        foreach (PatientRx rx in lrxList)
                                        {
                                            ROMReport lromReport = new ROMReport();
                                            lromReport.Exercise = rx.DeviceConfiguration;
                                            lromReport.GoalFlexion = rx.GoalFlexion.ToString();
                                            lromReport.FlexionAchieved = rx.CurrentFlexion.ToString();
                                            if (rx.EquipmentType != "Shoulder")
                                            {
                                                lromReport.GoalExtension = rx.GoalExtension.ToString();
                                                lromReport.ExtensionAchieved = rx.CurrentExtension.ToString();
                                            }
                                            lromReport.PainLevel = context.Pain.Where(x => x.RxId == rx.RxId).Count() > 0 ? context.Pain.Where(x => x.RxId == rx.RxId).Max(x => x.PainLevel).Value.ToString() : "";
                                            lRom.Add(lromReport);
                                        }
                                        lreport.ROM = lRom;
                                    }
                                }


                                //Getting Existing Protocol

                                List<UserActivityLog> lActivityProtocolList = lActivityList.Where(x => x.RecordType == "Exercise" && x.RecordChangeType == "Edit").OrderByDescending(x => x.StartTimeStamp).ToList();
                                List<Protocol> _protocolReivewList = new List<Protocol>();
                                List<Protocol> _protocolChanged = new List<Protocol>();
                                if (lActivityProtocolList != null && lActivityProtocolList.Count > 0)
                                {
                                    List<Protocol> _protocolList = INewPatient.GetProtocolListByPatientId(lreview.PatientId.ToString());
                                    if (_protocolList != null && _protocolList.Count > 0)
                                    {
                                        lreport.ProtocolList = _protocolList;
                                    }
                                    foreach (UserActivityLog lactivity in lActivityProtocolList)
                                    {
                                        Protocol lprotocol = JsonConvert.DeserializeObject<Protocol>(lactivity.RecordExistingJson, lsetting);
                                        Protocol lprotocolCurrent = JsonConvert.DeserializeObject<Protocol>(lactivity.RecordJson, lsetting);
                                        Protocol lexist = _protocolReivewList.FirstOrDefault(x => x.ProtocolId == lprotocol.ProtocolId);
                                        if (lexist == null)
                                        {
                                            string[] ignore = new string[] { "DateModified", "DateCreated" };
                                            IDictionary<string, string> ldict = PublicInstancePropertiesEqual(lprotocol, lprotocolCurrent, ignore);
                                            if (ldict.Count > 0)
                                                _protocolChanged.Add(lprotocolCurrent);
                                            _protocolReivewList.Add(lprotocol);
                                        }
                                    }
                                    lreport.ProtocolCurrentList = _protocolChanged;


                                    if (_protocolList != null && _protocolList.Count > 0)
                                    {
                                        HashSet<string> diffids = new HashSet<string>(_protocolReivewList.Select(s => s.ProtocolId));
                                        _protocolList = _protocolList.Where(x => !diffids.Contains(x.ProtocolId)).ToList();
                                        _protocolList.AddRange(_protocolReivewList);
                                        lreport.ProtocolList = _protocolList;
                                    }
                                }
                                else
                                {
                                    List<Protocol> _protocolList = INewPatient.GetProtocolListByPatientId(lreview.PatientId.ToString());
                                    if (_protocolList != null && _protocolList.Count > 0)
                                    {
                                        lreport.ProtocolList = _protocolList;
                                    }
                                }


                            }
                            else
                            {
                                List<PatientRx> lrxList = lIPatientRxRepository.getPatientRxByPatientId(lreview.PatientId);
                                if (lrxList != null && lrxList.Count > 0)
                                {
                                    foreach (PatientRx rx in lrxList)
                                    {
                                        ROMReport lromReport = new ROMReport();
                                        lromReport.Exercise = rx.DeviceConfiguration;
                                        lromReport.GoalFlexion = rx.GoalFlexion.ToString();
                                        lromReport.FlexionAchieved = rx.CurrentFlexion.ToString();
                                        if (rx.EquipmentType != "Shoulder")
                                        {
                                            lromReport.GoalExtension = rx.GoalExtension.ToString();
                                            lromReport.ExtensionAchieved = rx.CurrentExtension.ToString();
                                        }
                                        lromReport.PainLevel = context.Pain.Where(x => x.RxId == rx.RxId).Count() > 0 ? context.Pain.Where(x => x.RxId == rx.RxId).Max(x => x.PainLevel).Value.ToString() : "";
                                        lRom.Add(lromReport);
                                    }
                                    lreport.ROM = lRom;
                                }
                                List<Protocol> _protocolList = INewPatient.GetProtocolListByPatientId(lreview.PatientId.ToString());
                                if (_protocolList != null && _protocolList.Count > 0)
                                {
                                    lreport.ProtocolList = _protocolList;
                                }
                            }


                        }
                        
                    }
                }
              

                return View(lreport);
            }
            catch (Exception ex)
            {
                logger.LogDebug("Session Audit Trail Error: " + ex);
                return View();
            }

        }

        public PatientReviewReport getViewReport(string reviewid)
        {
            PatientReviewReport lreport = new PatientReviewReport();
            PatientDetails lpatient = null;
            List<ROMReport> lRom = new List<ROMReport>();
            JsonSerializerSettings lsetting = new JsonSerializerSettings();
            lsetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            try
            {
                if (!string.IsNullOrEmpty(reviewid))
                {
                    PatientReview lreview = lIPatientReviewRepository.GetPatientReview(reviewid);
                    if (lreview != null)
                    {
                        lreport.Review = lreview;
                        if (!string.IsNullOrEmpty(lreview.PatientId))
                        {
                            //Getting Patient Details
                            NewPatient lnewPatient = INewPatient.GetPatientByPatId(Convert.ToInt32(lreview.PatientId));
                            if (lnewPatient != null)
                            {
                                lpatient = new PatientDetails();
                                lpatient.PatientId = lnewPatient.PatientId;
                                lpatient.PatientName = lnewPatient.PatientName;
                                lpatient.ProviderId = lnewPatient.ProviderId;
                                lpatient.ProviderName = context.User.FirstOrDefault(x => x.UserId == lnewPatient.ProviderId).Name;
                                lpatient.PatId = lnewPatient.PatientAdminId;
                                lpatient.PatAdminName = context.User.FirstOrDefault(x => x.UserId == lnewPatient.PatientAdminId).Name;
                                lpatient.TherapistId = lnewPatient.TherapistId;
                                lpatient.TherapistName = context.User.FirstOrDefault(x => x.UserId == lnewPatient.TherapistId).Name;
                                lpatient.DateofBirth = lnewPatient.Dob.HasValue ? lnewPatient.Dob.Value.ToString("MM-dd-yyyy") : "";

                                lpatient.EvaluationDate = DateTime.Now.ToString("MM-dd-yyyy");
                                lpatient.Evaluator = HttpContext.Session.GetString("UserName");
                                lpatient.Side = lnewPatient.Side;
                                lpatient.EquipmentType = lnewPatient.EquipmentType;
                                lpatient.SurgeryDate = lnewPatient.SurgeryDate.HasValue ? lnewPatient.SurgeryDate.Value.ToString("MM-dd-yyyy") : "";
                                lreport.Patient = lpatient;
                            }
                            List<UserActivityLog> lActivityList = lIUserActivityLogRepository.UserActivityListByReviewId(reviewid);
                            if (lActivityList != null && lActivityList.Count > 0)
                            {
                                //Getting Rx record in the User activities
                                UserActivityLog lRxActivity = lActivityList.OrderBy(x => x.StartTimeStamp).FirstOrDefault(x => x.RecordType == "Rx" && x.RecordChangeType == "Edit");
                                if (lRxActivity != null)
                                {
                                    //Getting Rx records before it changed
                                    List<PatientRx> PatientRxList = JsonConvert.DeserializeObject<List<PatientRx>>(lRxActivity.RecordChangeType == "Edit" ? lRxActivity.RecordExistingJson : lRxActivity.RecordJson, lsetting);
                                    if (PatientRxList != null && PatientRxList.Count > 0)
                                    {
                                        foreach (PatientRx rx in PatientRxList)
                                        {
                                            ROMReport lromReport = new ROMReport();
                                            lromReport.Exercise = rx.DeviceConfiguration;
                                            lromReport.GoalFlexion = rx.GoalFlexion.ToString();
                                            lromReport.FlexionAchieved = rx.CurrentFlexion.ToString();
                                            if (rx.EquipmentType != "Shoulder")
                                            {
                                                lromReport.GoalExtension = rx.GoalExtension.ToString();
                                                lromReport.ExtensionAchieved = rx.CurrentExtension.ToString();
                                            }
                                            lromReport.PainLevel = context.Pain.Where(x => x.RxId == rx.RxId).Count() > 0 ? context.Pain.Where(x => x.RxId == rx.RxId).Max(x => x.PainLevel).Value.ToString() : "";
                                            lRom.Add(lromReport);
                                        }
                                        lreport.ROM = lRom;
                                    }

                                    //Getting Rx records changed properties
                                    List<UserActivityLog> lRxActivityList = lActivityList.OrderBy(x => x.StartTimeStamp).Where(x => x.RecordType == "Rx" && x.RecordChangeType == "Edit").ToList();

                                    if (lRxActivityList != null && lRxActivityList.Count > 0)
                                    {
                                        IDictionary<string, IDictionary<string, string>> ldictnaryList = new Dictionary<string, IDictionary<string, string>>();
                                        foreach (UserActivityLog lactivity in lRxActivityList)
                                        {
                                            string[] ignore = new string[] { "DateModified", "DateCreated" };

                                            List<PatientRx> lrxChangedCurrentList = JsonConvert.DeserializeObject<List<PatientRx>>(lactivity.RecordJson, lsetting);
                                            List<PatientRx> lrxChangedOldList = JsonConvert.DeserializeObject<List<PatientRx>>(lactivity.RecordExistingJson, lsetting);
                                            for (int i = 0; i < lrxChangedCurrentList.Count; i++)
                                            {
                                                IDictionary<string, string> ldict = PublicInstancePropertiesEqual(lrxChangedOldList[i], lrxChangedCurrentList[i], ignore);
                                                if (ldict.Count > 0)
                                                    ldictnaryList.Add(lrxChangedOldList[i].DeviceConfiguration, ldict);
                                            }

                                        }
                                        lreport.ChangeList = ldictnaryList;
                                    }

                                }
                                else
                                {
                                    List<PatientRx> lrxList = lIPatientRxRepository.getPatientRxByPatientId(lreview.PatientId);
                                    if (lrxList != null && lrxList.Count > 0)
                                    {
                                        foreach (PatientRx rx in lrxList)
                                        {
                                            ROMReport lromReport = new ROMReport();
                                            lromReport.Exercise = rx.DeviceConfiguration;
                                            lromReport.GoalFlexion = rx.GoalFlexion.ToString();
                                            lromReport.FlexionAchieved = rx.CurrentFlexion.ToString();
                                            if (rx.EquipmentType != "Shoulder")
                                            {
                                                lromReport.GoalExtension = rx.GoalExtension.ToString();
                                                lromReport.ExtensionAchieved = rx.CurrentExtension.ToString();
                                            }
                                            lromReport.PainLevel = context.Pain.Where(x => x.RxId == rx.RxId).Count() > 0 ? context.Pain.Where(x => x.RxId == rx.RxId).Max(x => x.PainLevel).Value.ToString() : "";
                                            lRom.Add(lromReport);
                                        }
                                        lreport.ROM = lRom;
                                    }
                                }


                                //Getting Existing Protocol

                                List<UserActivityLog> lActivityProtocolList = lActivityList.Where(x => x.RecordType == "Exercise" && x.RecordChangeType == "Edit").OrderByDescending(x => x.StartTimeStamp).ToList();
                                List<Protocol> _protocolReivewList = new List<Protocol>();
                                List<Protocol> _protocolChanged = new List<Protocol>();
                                if (lActivityProtocolList != null && lActivityProtocolList.Count > 0)
                                {
                                    List<Protocol> _protocolList = INewPatient.GetProtocolListByPatientId(lreview.PatientId.ToString());
                                    if (_protocolList != null && _protocolList.Count > 0)
                                    {
                                        lreport.ProtocolList = _protocolList;
                                    }
                                    foreach (UserActivityLog lactivity in lActivityProtocolList)
                                    {
                                        Protocol lprotocol = JsonConvert.DeserializeObject<Protocol>(lactivity.RecordExistingJson, lsetting);
                                        Protocol lprotocolCurrent = JsonConvert.DeserializeObject<Protocol>(lactivity.RecordJson, lsetting);
                                        Protocol lexist = _protocolReivewList.FirstOrDefault(x => x.ProtocolId == lprotocol.ProtocolId);
                                        if (lexist == null)
                                        {
                                            string[] ignore = new string[] { "DateModified", "DateCreated" };
                                            IDictionary<string, string> ldict = PublicInstancePropertiesEqual(lprotocol, lprotocolCurrent, ignore);
                                            if (ldict.Count > 0)
                                                _protocolChanged.Add(lprotocolCurrent);
                                            _protocolReivewList.Add(lprotocol);
                                        }
                                    }
                                    lreport.ProtocolCurrentList = _protocolChanged;


                                    if (_protocolList != null && _protocolList.Count > 0)
                                    {
                                        HashSet<string> diffids = new HashSet<string>(_protocolReivewList.Select(s => s.ProtocolId));
                                        _protocolList = _protocolList.Where(x => !diffids.Contains(x.ProtocolId)).ToList();
                                        _protocolList.AddRange(_protocolReivewList);
                                        lreport.ProtocolList = _protocolList;
                                    }
                                }
                                else
                                {
                                    List<Protocol> _protocolList = INewPatient.GetProtocolListByPatientId(lreview.PatientId.ToString());
                                    if (_protocolList != null && _protocolList.Count > 0)
                                    {
                                        lreport.ProtocolList = _protocolList;
                                    }
                                }


                            }
                            else
                            {
                                List<PatientRx> lrxList = lIPatientRxRepository.getPatientRxByPatientId(lreview.PatientId);
                                if (lrxList != null && lrxList.Count > 0)
                                {
                                    foreach (PatientRx rx in lrxList)
                                    {
                                        ROMReport lromReport = new ROMReport();
                                        lromReport.Exercise = rx.DeviceConfiguration;
                                        lromReport.GoalFlexion = rx.GoalFlexion.ToString();
                                        lromReport.FlexionAchieved = rx.CurrentFlexion.ToString();
                                        if (rx.EquipmentType != "Shoulder")
                                        {
                                            lromReport.GoalExtension = rx.GoalExtension.ToString();
                                            lromReport.ExtensionAchieved = rx.CurrentExtension.ToString();
                                        }
                                        lromReport.PainLevel = context.Pain.Where(x => x.RxId == rx.RxId).Count() > 0 ? context.Pain.Where(x => x.RxId == rx.RxId).Max(x => x.PainLevel).Value.ToString() : "";
                                        lRom.Add(lromReport);
                                    }
                                    lreport.ROM = lRom;
                                }
                                List<Protocol> _protocolList = INewPatient.GetProtocolListByPatientId(lreview.PatientId.ToString());
                                if (_protocolList != null && _protocolList.Count > 0)
                                {
                                    lreport.ProtocolList = _protocolList;
                                }
                            }


                        }



                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }

            return lreport;
        }

        public static IDictionary<string, string> PublicInstancePropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                if (self != null && to != null)
                {

                    Type type = typeof(T);
                    List<string> ignoreList = new List<string>(ignore);
                    foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                    {
                        if (!ignoreList.Contains(pi.Name) && pi.PropertyType.BaseType != null && pi.PropertyType.BaseType != null && pi.PropertyType.BaseType.Name == "ValueType")
                        {
                            object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                            object toValue = type.GetProperty(pi.Name).GetValue(to, null);

                            if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                            {
                                if (pi.PropertyType == typeof(DateTime?) || pi.PropertyType == typeof(DateTime))
                                {
                                    dict.Add(pi.Name, Convert.ToDateTime(selfValue).ToString("MM-dd-yyyy") + "$" + Convert.ToDateTime(toValue).ToString("MM-dd-yyyy"));
                                }
                                else
                                {
                                    dict.Add(pi.Name, selfValue.ToString() + "$" + toValue.ToString());
                                }

                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //logger.LogDebug("Error: " + ex);
            }

            return dict;
        }

        [HttpGet]
        [ActionName("ShowChange")]
        public JsonResult ShowChange(string activityId = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(activityId) && Convert.ToInt32(activityId) > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    UserActivityLog llog = lIUserActivityLogRepository.GetUserActivityLog(Convert.ToInt32(activityId));
                    return Json(new { result = "success", log = llog });
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Session Audit Trail Show Change: " + ex);
            }
            return Json("");
        }


        public IActionResult EmailPDF(string id = "")
        {
            try
            {
                PatientReviewReport lreport = null;
                if (!string.IsNullOrEmpty(id))
                    lreport = getViewReport(id);
                if (lreport != null && !string.IsNullOrEmpty(id))
                {
                    StringBuilder sb = new StringBuilder();

                    int font1 = 12;
                    int font2 = 10;
                    int font3 = 8;


                    sb.Append("<html style ='height: auto;'><head><link type='text/css' rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css' media='all'>");
                    sb.Append("<link type='text/css' rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.5.0/css/font-awesome.min.css' media='all'>");
                    sb.Append("<link type='text/css' rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/ionicons/2.0.1/css/ionicons.min.css' media='all'>");
                    sb.Append("<link type='text/css' rel='stylesheet' href='https://cdn.datatables.net/1.10.12/css/dataTables.bootstrap.min.css' media ='all'>");

                    sb.Append("<link type='text/css' rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/admin-lte/2.4.0/css/AdminLTE.css' media ='all'>");

                    sb.Append("<link type='text/css' rel='stylesheet' href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600,700,300italic,400italic,600italic' media ='all'>");
                    sb.Append("<link type = 'text/css' rel = 'stylesheet' href = '" + ConfigVars.NewInstance.url + "/css/bootstrap_print.css' media ='print' >");
                    sb.Append("</head>");
                    sb.Append("<body>");
                    sb.Append("<div id = 'example1_wrapper' class='dataTables_wrapper form-inline dt-bootstrap'>");

                    sb.Append("<div class='row'>");
                    sb.Append("<div class='col-sm-12'>");
                    sb.Append("<div class='box-body no-padding' style='font-size:small'>");
                    sb.Append("<div class='col-sm-12 no-padding'>");
                    sb.Append("<div class='col-md-8  no-padding'>");
                    sb.Append("<div class='col-md-12'>");
                    sb.Append("<span style = 'font-size:" + font1 + "px' ><b> TREX Rehab</b></span>");
                    sb.Append("</div>");
                    if (lreport.Patient != null)
                    {

                        sb.Append("<div class='col-md-12 no-padding'>");
                        sb.Append("<div class='col-md-6'><span style = 'font-size:" + font3 + "px'><b>Patient Name</b> : " + lreport.Patient.PatientName + "</span></div>");
                        sb.Append("<div class='col-md-6'><span style = 'font-size:" + font3 + "px'><b>Evaluation Date</b> :" + lreport.Patient.EvaluationDate + "</span></div>");
                        sb.Append("</div>");

                        sb.Append("<div class='col-md-12 no-padding'>");
                        sb.Append("<div class='col-md-6'><span style = 'font-size:" + font3 + "px'><b>Primary Physician</b> : " + lreport.Patient.ProviderName + "</span></div>");
                        sb.Append("<div class='col-md-6'><span style = 'font-size:" + font3 + "px'><b>Evaluator</b> :" + lreport.Patient.Evaluator + "</span></div>");
                        sb.Append("</div>");

                        sb.Append("<div class='col-md-12 no-padding'>");
                        sb.Append("<div class='col-md-6'><span style = 'font-size:" + font3 + "px'><b>Primary Therapist</b> : " + lreport.Patient.TherapistName + "</span></div>");
                        sb.Append("</div>");

                        sb.Append("<div class='col-md-12 no-padding'>");
                        sb.Append("<div class='col-md-6'><span style = 'font-size:" + font3 + "px'><b>Primary Administrator</b> : " + lreport.Patient.PatAdminName + "</span></div>");
                        sb.Append("</div>");

                        sb.Append("<div class='col-md-12 no-padding'>");
                        sb.Append("<div class='col-md-6'><span style = 'font-size:" + font3 + "px'><b>Date of birth</b> : " + lreport.Patient.DateofBirth + "</span></div>");
                        sb.Append("</div>");


                        sb.Append("<div class='col-sm-12 no-padding'>");
                        sb.Append("<div class='col-md-12  no-padding'>");
                        sb.Append("<div class='col-md-12'>");
                        sb.Append("<span style = 'font-size:" + font2 + "px'><b> Subjective </b></span>");
                        sb.Append("</div>");

                        sb.Append("<div class='col-md-12'><span style = 'font-size:" + font3 + "px'>" + lreport.Patient.Side + "&nbsp;" + lreport.Patient.EquipmentType + "&nbsp;Surgery on&nbsp;" + lreport.Patient.SurgeryDate + "</span></div>");
                        sb.Append("</div>");
                        sb.Append("</div>");


                    }
                    string flexionString = string.Empty;
                    string flexionString1 = string.Empty;
                    string extensionString = string.Empty;
                    List<EquipmentExercise> lExerciseString = lIEquipmentExerciseRepository.GetEquipmentExerciseString();
                    if (lExerciseString != null && lExerciseString.Count > 0 && lreport.Patient.EquipmentType == "Shoulder")
                    {
                        var lexString = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "Forward Flexion").FirstOrDefault();
                        if (lexString != null)
                        {
                            flexionString = lexString.FlexionString;
                        }
                        var lexString1 = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "External Rotation").FirstOrDefault();
                        if (lexString1 != null)
                        {
                            flexionString1 = lexString1.FlexionString;
                        }
                    }
                    else if (lExerciseString != null && lExerciseString.Count > 0)
                    {
                        var lexString = lExerciseString.Where(x => x.Limb == lreport.Patient.EquipmentType).FirstOrDefault();
                        if (lexString != null)
                        {
                            flexionString = lexString.FlexionString;
                            extensionString = lexString.ExtensionString;
                        }
                    }
                    sb.Append("<div class='col-sm-12 no-padding'>");
                    sb.Append("<div class='col-md-12  no-padding'>");
                    sb.Append("<div class='col-md-12'>");
                    sb.Append("<span style = 'font-size:" + font2 + "px'><b> Objective </b></span>");
                    sb.Append("</div>");

                    sb.Append("<div class='col-md-12'>");
                    sb.Append("<div><span style = 'font-size:" + font3 + "px'><b>Range of Motion</b></span></div>");
                    sb.Append("<div style = 'overflow-x:auto'>");
                    sb.Append("<table id='example1' class='table table-bordered table-striped' style = 'font-size:" + font3 + "px' role='grid' aria-describedby='example1_info'>");
                    sb.Append("<thead>");
                    sb.Append("<tr role='row'>");
                    sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Protocol Name: activate to sort column descending'>Exercise</th>");
                    if (lreport.Patient.EquipmentType != "Shoulder")
                    {
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Patient: activate to sort column descending'>" + flexionString + " Goal</th>");
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Therapist: activate to sort column descending'>" + flexionString + " Achieved</th>");
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Patient: activate to sort column descending'>" + extensionString + " Goal</th>");
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Therapist: activate to sort column descending'>" + extensionString + " Achieved</th>");
                    }
                    else
                    {
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Patient: activate to sort column descending'>" + flexionString + " Goal</th>");
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Therapist: activate to sort column descending'>" + flexionString + " Achieved</th>");
                    }
                    sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Max Pain: activate to sort column descending'>Pain Level</th>");
                    sb.Append("</tr>");
                    sb.Append("</thead>");
                    sb.Append("<tbody>");
                    if (lreport.ROM != null && lreport.ROM.Count > 0)
                    {
                        foreach (ROMReport item in lreport.ROM)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td>" + item.Exercise + "</td>");
                            if (lreport.Patient.EquipmentType != "Shoulder")
                            {
                                sb.Append("<td>" + item.GoalFlexion + "</td>");
                                sb.Append("<td>" + item.FlexionAchieved + "</td>");
                                sb.Append("<td>" + item.GoalExtension + "</td>");
                                sb.Append("<td>" + item.ExtensionAchieved + "</td>");
                            }
                            else
                            {
                                sb.Append("<td>" + item.GoalFlexion + "</td>");
                                sb.Append("<td>" + item.FlexionAchieved + "</td>");
                            }
                            if (!string.IsNullOrEmpty(item.PainLevel))
                            {
                                sb.Append("<td>" + item.PainLevel + "</td>");
                            }
                            else
                            {
                                sb.Append("<td>0</td>");
                            }
                            sb.Append("</tr>");
                        }
                    }
                    sb.Append("</tbody>");
                    sb.Append("</table>");
                    sb.Append("</div>");
                    sb.Append("</div>");

                    if (lreport.ProtocolList != null && lreport.ProtocolList.Count > 0)
                    {
                        sb.Append("<div class='col-md-12'>");
                        sb.Append("<div>");
                        sb.Append("<b>");
                        sb.Append("<span style = 'font-size:" + font2 + "px'>Active Exercises</span>");
                        sb.Append("</b>");
                        sb.Append("</div>");
                        sb.Append("<div style='overflow-x:auto'>");
                        sb.Append("<table id= 'example1' class='table table-bordered table-striped' style = 'font-size:" + font3 + "px' role='grid' aria-describedby='example1_info'>");
                        sb.Append("<thead>");
                        sb.Append("<tr role = 'row'>");
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Protocol Name: activate to sort column descending'>Exercise</th>");
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Protocol Name: activate to sort column descending'>Type</th>");
                        if (lreport.Patient.EquipmentType != "Shoulder")
                        {
                            sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Patient: activate to sort column descending'>" + flexionString + " Current</th>");
                            sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Therapist: activate to sort column descending'>" + flexionString + " Goal</th>");
                            sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Patient: activate to sort column descending'>" + extensionString + " Current</th>");
                            sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Therapist: activate to sort column descending'>" + extensionString + " Goal</th>");
                        }
                        else
                        {
                            sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Patient: activate to sort column descending'>" + flexionString + " Current</th>");
                            sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Therapist: activate to sort column descending'>" + flexionString + " Goal</th>");
                        }

                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Max Pain: activate to sort column descending'>Start Date</th>");
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Max Pain: activate to sort column descending'>End Date</th>");
                        sb.Append("</tr>");
                        sb.Append("</thead>");
                        sb.Append("<tbody>");
                        if (lreport.ProtocolList != null && lreport.ProtocolList.Count > 0)
                        {
                            foreach (Protocol item in lreport.ProtocolList)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td>" + item.ProtocolName + "</td>");
                                sb.Append("<td>" + item.DeviceConfiguration + "</td>");
                                if (lreport.Patient.EquipmentType != "Shoulder")
                                {
                                    sb.Append("<td>" + item.FlexUpLimit + "</td>");
                                    sb.Append("<td>" + item.StretchUpLimit + "</td>");
                                    sb.Append("<td>" + item.FlexDownLimit + "</td>");
                                    sb.Append("<td>" + item.StretchDownLimit + "</td>");
                                }
                                else
                                {
                                    sb.Append("<td>" + item.FlexUpLimit + "</td>");
                                    sb.Append("<td>" + item.StretchUpLimit + "</td>");
                                }

                                sb.Append("<td>" + (item.StartDate != null ? Convert.ToDateTime(item.StartDate).ToString(string.Format("MM-dd-yyyy")) : "") + "</td>");
                                sb.Append("<td>" + (item.EndDate != null ? Convert.ToDateTime(item.EndDate).ToString(string.Format("MM-dd-yyyy")) : "") + "</td>");
                                sb.Append("</tr>");
                            }
                        }
                        sb.Append("</tbody>");
                        sb.Append("</table>");
                        sb.Append("</div>");
                        sb.Append("</div>");
                    }
                    sb.Append("</div>");
                    sb.Append("</div>");

                    sb.Append("<div class='col-sm-12 no-padding'>");
                    sb.Append("<div class='col-md-12 no-padding'>");
                    sb.Append("<div class='col-md-12'>");
                    sb.Append("<span style = 'font-size:" + font2 + "px'><b> Assessment </b></span>");
                    sb.Append("</div>");
                    sb.Append("<div class='col-md-12' style = 'font-size:" + font3 + "px'>" + lreport.Review.AssessmentComment + "</div>");
                    sb.Append("</div>");
                    sb.Append("</div>");

                    sb.Append("<div class='col-sm-12 no-padding'>");
                    sb.Append("<div class='col-md-12 no-padding'>");
                    sb.Append("<div class='col-md-12'><span style = 'font-size:" + font2 + "px'><b> Plan </b></span></div>");
                    sb.Append("<div class='col-md-12'>");
                    if (lreport.ChangeList != null && lreport.ChangeList.Count > 0)
                    {
                        sb.Append("<div style = 'font-size:" + font3 + "px'><b> Range of Motion change </b></div>");

                        foreach (var item in lreport.ChangeList)
                        {
                            foreach (KeyValuePair<string, string> pair in item.Value)
                            {
                                if (!string.IsNullOrEmpty(pair.Value))
                                {
                                    var lsplit = pair.Value.Split("$");
                                    if (lsplit != null && lsplit.Length > 0)
                                    {
                                        if (lreport.Patient.EquipmentType == "Shoulder")
                                        {
                                            sb.Append("<span style = 'font-size:" + font3 + "px'>" + item.Key + "&nbsp;-&nbsp;" + pair.Key + "&nbsp;changed from&nbsp;" + lsplit[0] + " to " + @lsplit[1] + "</span><br/>");
                                        }
                                        else
                                        {
                                            sb.Append("<span style = 'font-size:" + font3 + "px'>" + pair.Key + "&nbsp;changed from&nbsp;" + lsplit[0] + " to " + @lsplit[1] + "</span><br/>");

                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (lreport.ProtocolCurrentList != null && lreport.ProtocolCurrentList.Count > 0)
                    {
                        sb.Append("<div class='col-md-12 no-padding'>");
                        sb.Append("<div style = 'font-size:" + font3 + "px'><b> Exercises changed </b></div>");
                        sb.Append("<div style = 'overflow-x:auto'>");
                        sb.Append("<table id= 'example1' class='table table-bordered table-striped' style = 'font-size:" + font3 + "px' role='grid' aria-describedby='example1_info'>");
                        sb.Append("<thead>");
                        sb.Append("<tr role = 'row'>");
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Protocol Name: activate to sort column descending'>Exercise</th>");
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Protocol Name: activate to sort column descending'>Type</th>");
                        if (lreport.Patient.EquipmentType != "Shoulder")
                        {
                            sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Patient: activate to sort column descending'>" + flexionString + " Current</th>");
                            sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Therapist: activate to sort column descending'>" + flexionString + " Goal</th>");
                            sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Patient: activate to sort column descending'>" + extensionString + " Current</th>");
                            sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Therapist: activate to sort column descending'>" + extensionString + " Goal</th>");
                        }
                        else
                        {
                            sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Patient: activate to sort column descending'>" + flexionString + " Current</th>");
                            sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Therapist: activate to sort column descending'>" + flexionString + " Goal</th>");
                        }
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Max Pain: activate to sort column descending'>Start Date</th>");
                        sb.Append("<th class='sorting_asc' tabindex='0' aria-controls='example1' rowspan='1' colspan='1' aria-sort='ascending' aria-label='Max Pain: activate to sort column descending'>End Date</th>");
                        sb.Append("</tr>");
                        sb.Append("</thead>");
                        sb.Append("<tbody>");
                        if (lreport.ProtocolCurrentList != null && lreport.ProtocolCurrentList.Count > 0)
                        {
                            foreach (Protocol item in lreport.ProtocolCurrentList)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td>" + item.ProtocolName + "</td>");
                                sb.Append("<td>" + item.DeviceConfiguration + "</td>");
                                if (lreport.Patient.EquipmentType != "Shoulder")
                                {
                                    sb.Append("<td>" + item.FlexUpLimit + "</td>");
                                    sb.Append("<td>" + item.StretchUpLimit + "</td>");
                                    sb.Append("<td>" + item.FlexDownLimit + "</td>");
                                    sb.Append("<td>" + item.StretchDownLimit + "</td>");
                                }
                                else
                                {
                                    sb.Append("<td>" + item.FlexUpLimit + "</td>");
                                    sb.Append("<td>" + item.StretchUpLimit + "</td>");
                                }

                                sb.Append("<td>" + (item.StartDate != null ? Convert.ToDateTime(item.StartDate).ToString(string.Format("MM-dd-yyyy")) : "") + "</td>");
                                sb.Append("<td>" + (item.EndDate != null ? Convert.ToDateTime(item.EndDate).ToString(string.Format("MM-dd-yyyy")) : "") + "</td>");
                                sb.Append("</tr>");
                            }
                        }
                        sb.Append("</tbody>");
                        sb.Append("</table>");
                        sb.Append("</div>");
                        sb.Append("</div>");
                    }
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("<div class='col-md-4' align='center'>");
                    sb.Append("<div class='col-md-12'>");
                    sb.Append("<span style = 'font-size:" + font2 + "px' ><b> Physical therapy Evaluation</b></span>");
                    sb.Append("</div>");
                    sb.Append("<div class='col-md-12 no-padding'>");

                    if (lreport.Patient.EquipmentType == "Shoulder")
                    {
                        string image = ConfigVars.NewInstance.url + "/images/shoulder1.png";
                        sb.Append("<img src = '" + image + "'");
                    }
                    else if (lreport.Patient.EquipmentType == "Ankle")
                    {
                        string image = ConfigVars.NewInstance.url + "/images/ankle.png";
                        sb.Append("<img src = '" + image + "'");
                    }
                    else
                    {
                        string image = ConfigVars.NewInstance.url + "/images/knee.png";
                        sb.Append("<img src = '" + image + "'");
                    }
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("</body>");
                    sb.Append("</html>");


                    Console.WriteLine("Prabhu Current Directory-" + Utilities.env.WebRootPath);

                    Console.WriteLine("Prabhu Html String-" + sb.ToString());


                    TempData["HtmlString"] = sb.ToString();


                    // Instantiate DI container for the application

                    var serviceCollection = new ServiceCollection();

                    //Register NodeServices  
                    serviceCollection.AddNodeServices();
                    //Request the DI container to supply the shared INodeServices instance  
                    var serviceProvider = serviceCollection.BuildServiceProvider();
                    var nodeService = serviceProvider.GetRequiredService<INodeServices>();

                    Stream stream = new MemoryStream();
                    stream = GeneratePDF(nodeService).Result;
                    byte[] pdfBytes;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        pdfBytes = ms.ToArray();
                    }

                    if (pdfBytes.Length > 0)
                    {
                        User luser = lIUserRepository.getUser(HttpContext.Session.GetString("UserId"));
                        if (luser != null && !string.IsNullOrEmpty(luser.Email))
                        {
                            Console.WriteLine("User Is Not Null, Email ID:" + luser.Email);
                            string subject = "SOAP Review Report for Patient " + lreport.Patient.PatientName;
                            string message = "Dear " + luser.Name + ",<br/><br/> Attached please find the SOAP review report you requested for patient" + lreport.Patient.PatientName + " on " + DateTime.Now.ToString("MMM-dd-yyyy") + " at " + DateTime.Now.ToString("hh:mm tt") + "<br/><br/><br/>Thank you<br/>OneDirect Trex";
                            string filename = lreport.Patient.PatientName + "" + lreport.Patient.PatientId + ".pdf";
                            Smtp.SendGridEmailWithAttachment(luser.Email, subject, message, filename, pdfBytes);

                            TempData["msg"] = "<script>Helpers.ShowMessage('SOAP report is emailed to your emailid " + luser.Email + "', 1);</script>";

                            return View("ViewChanges", lreport);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                logger.LogDebug("Session Audit Trail Show Change: " + ex);
            }
            return RedirectToAction("PatientReview");
        }

        public async Task<string> Add([FromServices] INodeServices nodeServices)
        {
            var num1 = 10;
            var num2 = 20;
            var result = await nodeServices.InvokeAsync<int>("generatePDF.js", num1, num2);
            return result.ToString();
        }
        public async Task<Stream> GeneratePDF([FromServices] INodeServices nodeServices)
        {
            var options = new
            {

            };

            string htmlstring = TempData["HtmlString"].ToString();
            var result = await nodeServices.InvokeAsync<Stream>(Path.Combine(OneDirect.Helper.Utilities.env.WebRootPath, "Node", "generatePDF.js"), htmlstring, options);

            return result;
        }

    }
}
