using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository.Interface;
using OneDirect.Models;
using Microsoft.Extensions.Logging;
using OneDirect.Repository;
using OneDirect.Extensions;
using OneDirect.ViewModels;
using Microsoft.AspNetCore.Http;
using OneDirect.Helper;
using Newtonsoft.Json;
using OneDirect.Vsee;
using OneDirect.VSee;
using RestSharp.Extensions;
using Highsoft.Web.Mvc.Stocks;
using System.Net;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class ProviderController : Controller
    {
        private readonly IEquipmentExerciseInterface lIEquipmentExerciseRepository;
        private readonly IUserActivityLogInterface lIUserActivityLogRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly IPatientRxInterface lIPatientRxRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public ProviderController(OneDirectContext context, ILogger<ProviderController> plogger, IPatientRxInterface IPatientRxRepository, IEquipmentExerciseInterface IEquipmentExerciseRepository)
        {
            lIEquipmentExerciseRepository = IEquipmentExerciseRepository;
            logger = plogger;
            this.context = context;
            lIPatientRxRepository = IPatientRxRepository;
            lIUserRepository = new UserRepository(context);
            lIUserActivityLogRepository = new UserActivityLogRepository(context);
        }

        //show the list of providers
        // GET: /<controller>/
        public IActionResult Index()
        {
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("Pain Post Start");
                List<UserListView> pUser = lIUserRepository.getUserListByType1243(ConstantsVar.Provider);
                ViewBag.Count = pUser.Count;
                ViewBag.userlist = pUser;
                return View(pUser);
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
        }
        public IActionResult Add()
        {
            return View("Add");
        }

        //add a new provider record, by checking for the unique npi and provider id
        [HttpPost]
        public IActionResult Add(UserViewModel pUser)
        {
            pUser.Type = ConstantsVar.Provider;
            User _user = lIUserRepository.getUser(pUser.UserId);
            User _user2 = lIUserRepository.getUserNpi(pUser.Npi);
            User _userNew = UserExtension.UserViewModelToUser(pUser);
            if ((_user == null) && (_user2 == null))
            {
                _userNew.Phone = RemoveSpecialChars(_userNew.Phone);
                string _User = lIUserRepository.InsertUser(_userNew);
                if (_User == "Username already exists")
                {
                    TempData["Installer"] = JsonConvert.SerializeObject(_userNew);
                    TempData["msg"] = "<script>Helpers.ShowMessage('User with same Provider Id is already registered, please use different one', 1);</script>";
                    pUser.UserId = null;
                    return View(pUser);
                }
            }
            else
            {
                if (_user2 != null)
                {
                    if (_user2.Npi == _userNew.Npi)
                    {
                        TempData["Provider"] = JsonConvert.SerializeObject(_userNew);
                        TempData["msg"] = "<script>Helpers.ShowMessage('User with same Npi is already registered, please use different one', 1);</script>";
                        pUser.Npi = null;
                        return View(pUser);
                    }
                }
                if (_user.UserId == _userNew.UserId)
                {
                    TempData["Provider"] = JsonConvert.SerializeObject(_userNew);
                    TempData["msg"] = "<script>Helpers.ShowMessage('User with same Provider Id is already registered, please use different one', 1);</script>";
                    pUser.UserId = null;
                    return View(pUser);
                }
            }
            return RedirectToAction("Index");
        }

        //show the profile of a provider 
        public IActionResult Profile(string id)
        {
            User pUser = lIUserRepository.getUser(id);
            if (pUser != null)
            {
                UserViewModel _user = UserExtension.UserToUserViewModel(pUser);
                ViewBag.Name = _user.Name;

                return View(_user);
            }
            else
            {
                return View(null);
            }
        }

        //edit or update the provider's profile
        [HttpPost]
        public IActionResult Profile(UserViewModel pUser)
        {
            pUser.Type = ConstantsVar.Provider;
            if (pUser != null && !string.IsNullOrEmpty(pUser.UserId))
            {
                User luser = lIUserRepository.getUser(pUser.UserId);
                if (luser != null && luser.Npi != pUser.Npi)
                {

                    User _user2 = lIUserRepository.getUserNpi(pUser.Npi);
                    if (_user2 != null)
                    {

                        TempData["Installer"] = JsonConvert.SerializeObject(UserExtension.UserViewModelToUser(pUser));
                        TempData["msg"] = "<script>Helpers.ShowMessage('User with same Npi is already registered, please use different one', 1);</script>";
                        pUser.Npi = null;
                        return View(pUser);

                    }
                    else
                    {
                        User _user = UserExtension.UserViewModelToUser(pUser);
                        _user.Phone = RemoveSpecialChars(_user.Phone);
                        string _result = lIUserRepository.UpdateUser(_user);
                    }
                }
                else
                {
                    User _user = UserExtension.UserViewModelToUser(pUser);
                    _user.Phone = RemoveSpecialChars(_user.Phone);
                    string _result = lIUserRepository.UpdateUser(_user);
                }
            }

            if (HttpContext.Session.GetString("UserType") != null && HttpContext.Session.GetString("UserType").ToString() == ConstantsVar.Provider.ToString())
            {
                return RedirectToAction("Profile", new { id = pUser.UserId });
            }
            return RedirectToAction("Index");
        }

        //remove special characters from the phone number of provider
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


        //get the dashbord of particular providers using userID
        public IActionResult Dashboard()
        {

            ViewBag.shoulderflexionString = "Forward Flexion";
            ViewBag.shoulderflexionString1 = "External Rotation";
            ViewBag.ankleflexionString = "Flexion";
            ViewBag.ankleextensionString = "Extension";
            ViewBag.kneeflexionString = "Flexion";
            ViewBag.kneeextensionString = "Extension";
            List<EquipmentExercise> lExerciseString = lIEquipmentExerciseRepository.GetEquipmentExerciseString();
            if (lExerciseString != null && lExerciseString.Count > 0)
            {
                var lexString = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "Forward Flexion").FirstOrDefault();
                if (lexString != null)
                {
                    ViewBag.shoulderflexionString = lexString.FlexionString;
                }
                var lexString1 = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "External Rotation").FirstOrDefault();
                if (lexString1 != null)
                {
                    ViewBag.shoulderflexionString1 = lexString1.FlexionString;
                }

                var lexString2 = lExerciseString.Where(x => x.Limb == "Ankle").FirstOrDefault();
                if (lexString2 != null)
                {
                    ViewBag.ankleflexionString = lexString2.FlexionString;
                    ViewBag.ankleextensionString = lexString2.ExtensionString;
                }
                var lexString3 = lExerciseString.Where(x => x.Limb == "Knee").FirstOrDefault();
                if (lexString3 != null)
                {
                    ViewBag.kneeflexionString = lexString3.FlexionString;
                    ViewBag.kneeextensionString = lexString3.ExtensionString;
                }
            }
            
            List<DashboardView> lDashboardView = null;
            string _uType = HttpContext.Session.GetString("UserType");
            if (HttpContext.Session.GetString("UserId") != null && HttpContext.Session.GetString("UserType") != null && HttpContext.Session.GetString("UserType").ToString() == ConstantsVar.Provider.ToString())
            {
                try
                {
                    lDashboardView = lIPatientRxRepository.getDashboard(HttpContext.Session.GetString("UserId"));
                }
                catch (Exception ex)
                {
                    logger.LogDebug("Error: " + ex);
                }
                return View(lDashboardView);
            }
            else
            {

            }
            return View(null);
        }



        public IActionResult Patient(int id, string Username, string equipmentType, string actuator = "")
        {
            ViewBag.User = Username;
            ViewBag.actuator = actuator;

            string _uType = HttpContext.Session.GetString("UserType");
            if (_uType == "3" || _uType == "2" || _uType == "1" || _uType == "0" || _uType == "6")
            {

                {
                    PatientRx patientRx = lIPatientRxRepository.getByRxIDId(id, actuator);
                    ViewBag.PatientRx = patientRx;
                }
                //Usage
                PatientRx lPatientRx = lIPatientRxRepository.getPatientRx(id, equipmentType, actuator);
                UsageViewModel lusage = new UsageViewModel();
                lusage.MaxSessionSuggested = 0;
                lusage.PercentageCompleted = 0;
                lusage.PercentagePending = 100;
                ViewBag.Usage = lusage;
                if (lPatientRx != null)
                {
                    double requiredSession = (((Convert.ToDateTime(lPatientRx.RxEndDate) - Convert.ToDateTime(lPatientRx.RxStartDate)).TotalDays / 7) * (int)lPatientRx.RxDaysPerweek * (int)lPatientRx.RxSessionsPerWeek);
                    int totalSession = lPatientRx.Session != null ? lPatientRx.Session.ToList().Count : 0;
                    lusage.MaxSessionSuggested = (int)requiredSession;
                    lusage.PercentageCompleted = (int)((totalSession / requiredSession) * 100);
                    lusage.PercentagePending = 100 - lusage.PercentageCompleted;
                    ViewBag.Usage = lusage;
                }

                //Pain
                PatientRx lPatientRxPain = lIPatientRxRepository.getPatientRxPain(id, equipmentType, actuator);
                PainViewModel lpain = new PainViewModel();
                lpain.TotalPain = 0;
                lpain.LowPain = 0;
                lpain.MediumPain = 0;
                lpain.HighPain = 100;
                ViewBag.Pain = lpain;
                if (lPatientRxPain != null)
                {
                    List<Session> lSessionList = lPatientRx.Session != null ? lPatientRxPain.Session.ToList() : null;
                    if (lSessionList != null && lSessionList.Count > 0)
                    {
                        lpain.TotalPain = lSessionList.Select(x => x.Pain.Count).Sum();
                        lpain.LowPain = lpain.TotalPain > 0 ? (int)(((double)(lSessionList.Select(x => x.Pain.Where(y => y.PainLevel <= 2).Count()).Sum()) / lpain.TotalPain) * 100) : 0;
                        lpain.MediumPain = lpain.TotalPain > 0 ? (int)(((double)(lSessionList.Select(x => x.Pain.Where(y => y.PainLevel > 2 && y.PainLevel <= 5).Count()).Sum()) / lpain.TotalPain) * 100) : 0;
                        lpain.HighPain = 100 - lpain.MediumPain - lpain.LowPain;
                        ViewBag.Pain = lpain;
                    }
                }




                ROMChartViewModel ROM = lIPatientRxRepository.getPatientRxROMChart(id, equipmentType, actuator);
                if (ROM != null)
                {

                    ViewBag.ROM = ROM;
                }

                ViewBag.EquipmentType = equipmentType;
                if (equipmentType == "Shoulder")
                {
                    //Equipment ROM
                    ViewBag.EquipmentType = equipmentType;
                    List<ShoulderViewModel> ROMList = lIPatientRxRepository.getPatientRxEquipmentROMForShoulder(id, equipmentType, actuator);
                    if (ROMList != null && ROMList.Count > 0)
                    {
                        ViewBag.FlexionColor = (ROMList.Where(x => x.Flexion > 0).Count() > 0) ? "Orange" : "White";
                        ViewBag.EquipmentROM = ROMList;
                    }
                    //Compliance
                    List<ShoulderViewModel> ComplianceList = lIPatientRxRepository.getPatientRxComplianceForShoulder(id, equipmentType, actuator);
                    if (ComplianceList != null && ComplianceList.Count > 0)
                    {
                        ViewBag.ComFlexionColor = (ComplianceList.Where(x => x.Flexion > 0).Count() > 0) ? "Orange" : "White";
                        ViewBag.Compliance = ComplianceList;
                    }
                }
                else
                {
                    //Equipment ROM
                    ViewBag.EquipmentType = equipmentType;
                    List<FlexionViewModel> FlexionList = lIPatientRxRepository.getPatientRxEquipmentROMByFlexion(id, equipmentType, actuator);
                    if (FlexionList != null && FlexionList.Count > 0)
                    {

                        ViewBag.EquipmentFlexion = FlexionList;
                    }
                    List<ExtensionViewModel> ExtensionList = lIPatientRxRepository.getPatientRxEquipmentROMByExtension(id, equipmentType, actuator);
                    if (ExtensionList != null && ExtensionList.Count > 0)
                    {

                        ViewBag.EquipmentExtension = ExtensionList;
                    }

                    //Compliance

                    List<FlexionViewModel> CFlexionList = lIPatientRxRepository.getPatientRxComplianceByFlexion(id, equipmentType, actuator);
                    if (CFlexionList != null && CFlexionList.Count > 0)
                    {

                        ViewBag.FlexionCompliance = CFlexionList;
                    }
                    List<ExtensionViewModel> CExtensionList = lIPatientRxRepository.getPatientRxComplianceByExtension(id, equipmentType, actuator);
                    if (CExtensionList != null && CExtensionList.Count > 0)
                    {

                        ViewBag.ExtensionCompliance = CExtensionList;
                    }


                }


                //Treatment Calendar
                List<TreatmentCalendarViewModel> TreatmentCalendarList = lIPatientRxRepository.getTreatmentCalendar(id, equipmentType, actuator);
                if (TreatmentCalendarList != null && TreatmentCalendarList.Count > 0)
                {
                    ViewBag.TreatmentCalendar = TreatmentCalendarList;
                }

                //Current Sessions
                List<Session> SessionList = lIPatientRxRepository.getCurrentSessions(id, equipmentType, actuator);
                if (SessionList != null && SessionList.Count > 0)
                {
                    ViewBag.SessionList = SessionList;
                }


                if (actuator == "Forward Flexion" || actuator == "External Rotation")
                {
                    ViewBag.Extension = "External Rotation";
                    ViewBag.Flexion = "Flexion";
                }
                else
                {
                    ViewBag.Flexion = "Flexion";
                    ViewBag.Extension = "Extension";
                }
            }


            return View();

        }




        //delete the patient record 
        public IActionResult Delete(int patid)
        {
            try
            {
                Patient lpatient = context.Patient.FirstOrDefault(p => p.PatientId == patid);
                if (lpatient != null)
                {
                    string result = lIPatientRxRepository.DeletePatientRecordsWithCasecade(patid);
                    if (!string.IsNullOrEmpty(result) && result == "success")
                    {
                        //Insert to User Activity Log
                        UserActivityLog llog = new UserActivityLog();
                        llog.SessionId = HttpContext.Session.GetString("SessionId");
                        llog.ActivityType = "Update";
                        llog.StartTimeStamp = !string.IsNullOrEmpty(HttpContext.Session.GetString("SessionTime")) ? Convert.ToDateTime(HttpContext.Session.GetString("SessionTime")) : DateTime.Now;
                        llog.Duration = Convert.ToInt32((DateTime.Now - Convert.ToDateTime(HttpContext.Session.GetString("SessionTime"))).TotalSeconds);
                        llog.RecordChangeType = "Delete";
                        llog.RecordType = "Patient";
                        llog.Comment = "Record deleted";
                        llog.RecordJson = JsonConvert.SerializeObject(lpatient);
                        llog.UserId = HttpContext.Session.GetString("UserId");
                        llog.UserName = HttpContext.Session.GetString("UserName");
                        llog.UserType = HttpContext.Session.GetString("UserType");
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ReviewID")))
                        {
                            llog.ReviewId = HttpContext.Session.GetString("ReviewID");
                        }
                        lIUserActivityLogRepository.InsertUserActivityLog(llog);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") == "0")
                return RedirectToAction("Index", "Patient");
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") == "1")
                return RedirectToAction("Dashboard", "Support");
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") == "2")
                return RedirectToAction("Dashboard", "Therapist");
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") == "3")
                return RedirectToAction("Dashboard", "Provider");
            else
                return RedirectToAction("Dashboard", "Provider");
        }

        //delete the provider record not having any patients assigned
        public IActionResult DeleteProvider(string provider)
        {

            try
            {

                var _user = (from p in context.User where p.UserId == provider select p).FirstOrDefault();

                context.User.Remove(_user);


                int res = context.SaveChanges();
                if (res > 0)
                {
                    VSeeHelper lhelper = new VSeeHelper();
                    DeleteUser luser = new DeleteUser();
                    luser.secretkey = ConfigVars.NewInstance.secretkey;
                    luser.username = _user.UserId;
                    var resUser = lhelper.DeleteUser(luser);
                    if (resUser != null && resUser["status"] == "success")
                    {
                        //Insert to User Activity Log
                        UserActivityLog llog = new UserActivityLog();
                        llog.SessionId = HttpContext.Session.GetString("SessionId");
                        llog.ActivityType = "Update";
                        llog.StartTimeStamp = !string.IsNullOrEmpty(HttpContext.Session.GetString("SessionTime")) ? Convert.ToDateTime(HttpContext.Session.GetString("SessionTime")) : DateTime.Now;
                        llog.Duration = Convert.ToInt32((DateTime.Now - Convert.ToDateTime(HttpContext.Session.GetString("SessionTime"))).TotalSeconds);
                        llog.RecordChangeType = "Delete";
                        llog.RecordType = "Provider";
                        llog.Comment = "Record deleted";
                        llog.RecordJson = JsonConvert.SerializeObject(_user);
                        llog.UserId = HttpContext.Session.GetString("UserId");
                        llog.UserName = HttpContext.Session.GetString("UserName");
                        llog.UserType = HttpContext.Session.GetString("UserType");
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ReviewID")))
                        {
                            llog.ReviewId = HttpContext.Session.GetString("ReviewID");
                        }
                        lIUserActivityLogRepository.InsertUserActivityLog(llog);
                    }

                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Provider");
            }
            return RedirectToAction("Index", "Provider");
        }

    }

}
