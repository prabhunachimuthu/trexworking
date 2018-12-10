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
using Microsoft.AspNetCore.Mvc.Rendering;
using Highsoft.Web.Mvc.Stocks;
using System.Net;
using Newtonsoft.Json.Serialization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class ReviewController : Controller
    {
        private readonly ILibraryInterface lILibraryRepository;
        private readonly IPatientLibraryInterface lIPatientLibraryRepository;
        private readonly IEquipmentExerciseInterface lIEquipmentExerciseRepository;
        private readonly IPatient IPatient;
        private readonly IPatientReviewInterface lIPatientReviewRepository;
        private readonly IMessageInterface lIMessageRepository;
        private readonly IAppointmentScheduleInterface lIAppointmentScheduleRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly IUserActivityLogInterface lIUserActivityLogRepository;
        private readonly IDeviceCalibrationInterface lIDeviceCalibrationRepository;
        private readonly IPatientConfigurationInterface lIPatientConfigurationRepository;
        private readonly INewPatient INewPatient;
        private readonly IPatientRxInterface lIPatientRxRepository;
        private readonly ISessionInterface lISessionInterface;
        private readonly ILogger logger;
        private OneDirectContext context;

        public ReviewController(OneDirectContext context, ILogger<ReviewController> plogger, INewPatient lINewPatient, IPatientRxInterface IPatientRxRepository, IEquipmentExerciseInterface IEquipmentExerciseRepository, ILibraryInterface ILibraryRepository, IPatientLibraryInterface IPatientLibraryRepository)
        {
            lILibraryRepository = ILibraryRepository;
            lIPatientLibraryRepository = IPatientLibraryRepository;
            lIEquipmentExerciseRepository = IEquipmentExerciseRepository;
            logger = plogger;
            this.context = context;
            IPatient = new PatientRepository(context);
            lIPatientReviewRepository = new PatientReviewRepository(context);
            lIPatientRxRepository = IPatientRxRepository;
            lIUserRepository = new UserRepository(context);
            lIUserActivityLogRepository = new UserActivityLogRepository(context);
            lIDeviceCalibrationRepository = new DeviceCalibrationRepository(context);
            lIPatientConfigurationRepository = new PatientConfigurationRepository(context);
            INewPatient = lINewPatient;
            lISessionInterface = new SessionRepository(context);
            lIAppointmentScheduleRepository = new AppointmentScheduleRepository(context);
            lIMessageRepository = new MessageRepository(context);
        }


        // GET: /<controller>/

        public IActionResult Index(int id, string Username, string equipmentType, string actuator = "", string tab = "")
        {

            ViewBag.page = "Review";
            ReviewModel lmodel = new ReviewModel();
            ViewBag.tab = tab;
            ViewBag.User = Username;
            ViewBag.actuator = actuator;
            ViewBag.EquipmentType = equipmentType;
            ViewBag.Id = id;
            UsageViewModel lusage = new UsageViewModel();
            lusage.MaxSessionSuggested = 0;
            lusage.PercentageCompleted = 0;
            lusage.PercentagePending = 0;
            ViewBag.Usage = lusage;
            PainViewModel lpain = new PainViewModel();
            lpain.TotalPain = 0;
            lpain.LowPain = 0;
            lpain.MediumPain = 0;
            lpain.HighPain = 0;
            ViewBag.Pain = lpain;
            JsonSerializerSettings lsetting1 = new JsonSerializerSettings();
            lsetting1.ContractResolver = new CamelCasePropertyNamesContractResolver();
            lsetting1.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            ViewBag.FlexionData = JsonConvert.SerializeObject(new List<HightStockShoulderViewModelForJavaScript>(), lsetting1);
            ViewBag.ExtensionData = JsonConvert.SerializeObject(new List<HightStockShoulderViewModelForJavaScript>(), lsetting1);
            ViewBag.FlexionExtensionData = JsonConvert.SerializeObject(new List<HightStockShoulderViewModelForJavaScript>(), lsetting1);

            try
            {
                //Insert to User Activity Log -Patient
                JsonSerializerSettings lsetting = new JsonSerializerSettings();
                lsetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                List<EquipmentExercise> lExerciseString = lIEquipmentExerciseRepository.GetEquipmentExerciseString();
                if (HttpContext.Session.GetString("UserType") != ConstantsVar.Admin.ToString())
                {
                    if (string.IsNullOrEmpty(HttpContext.Session.GetString("ReviewID")))
                    {
                        PatientReview lreview = new PatientReview();
                        lreview.ReviewId = Guid.NewGuid().ToString();
                        lreview.UserId = HttpContext.Session.GetString("UserId");
                        lreview.UserName = HttpContext.Session.GetString("UserName");
                        lreview.UserType = HttpContext.Session.GetString("UserType");
                        lreview.SessionId = HttpContext.Session.GetString("SessionId");
                        Patient ppatient = context.Patient.FirstOrDefault(x => x.PatientId == id);
                        if (ppatient != null)
                        {
                            lreview.PatientId = id.ToString();
                            lreview.PatientName = ppatient.PatientName;
                        }
                        lreview.ActivityType = "Review";
                        lreview.StartTimeStamp = DateTime.Now;
                        lreview.Duration = 0;
                        int res = lIPatientReviewRepository.InsertPatientReview(lreview);
                        if (!string.IsNullOrEmpty(lreview.ReviewId) && res > 0)
                        {
                            HttpContext.Session.SetString("ReviewID", lreview.ReviewId);
                            HttpContext.SetCookie("ReviewID", lreview.ReviewId, 5, CookieExpiryIn.Hours);
                            ViewBag.StartTimer = "true";
                        }
                    }
                }


                if (!string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(tab) && tab == "Dashboard")
                {

                    string _uType = HttpContext.Session.GetString("UserType");
                    if (_uType == "3" || _uType == "2" || _uType == "1" || _uType == "0" || _uType == "6")
                    {

                        {
                            PatientRx patientRx = lIPatientRxRepository.getByRxIDId(id, actuator);
                            ViewBag.PatientRx = patientRx;
                        }
                        //Usage
                        PatientRx lPatientRx = lIPatientRxRepository.getPatientRx(id, equipmentType, actuator);

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

                        if (lExerciseString != null && lExerciseString.Count > 0 && equipmentType.ToLower() == "shoulder")
                        {
                            var lexString = lExerciseString.Where(x => x.Limb == equipmentType && x.ExerciseEnum == "Forward Flexion").FirstOrDefault();
                            if (lexString != null)
                            {
                                ViewBag.flexionString = lexString.FlexionString;
                            }
                            var lexString1 = lExerciseString.Where(x => x.Limb == equipmentType && x.ExerciseEnum == "External Rotation").FirstOrDefault();
                            if (lexString1 != null)
                            {
                                ViewBag.flexionString1 = lexString1.FlexionString;
                            }
                        }
                        else if (lExerciseString != null && lExerciseString.Count > 0)
                        {
                            var lexString = lExerciseString.Where(x => x.Limb == equipmentType).FirstOrDefault();
                            if (lexString != null)
                            {
                                ViewBag.flexionString = lexString.FlexionString;
                                ViewBag.extensionString = lexString.ExtensionString;
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
                            //Equipment ROM HighStock Chart
                            List<HightStockShoulderViewModelForJavaScript> highStockModel = lIPatientRxRepository.getPatientRxEquipmentROMForShoulderInHighStockChartForJavaScript(id, equipmentType, actuator);

                            ViewBag.FlexionData = JsonConvert.SerializeObject(highStockModel, lsetting1);
                            ViewBag.ExtensionData = JsonConvert.SerializeObject(new List<HightStockShoulderViewModelForJavaScript>(), lsetting1);
                            ViewBag.FlexionExtensionData = JsonConvert.SerializeObject(new List<HightStockShoulderViewModelForJavaScript>(), lsetting1);

                            //Equipment ROM
                            ViewBag.EquipmentType = equipmentType;

                        }
                        else
                        {
                            List<HightStockShoulderViewModelForJavaScript> highStockModelFlexion = lIPatientRxRepository.getPatientRxEquipmentROMByFlexionInHighStockChartForJavaScript(id, equipmentType, actuator);
                            List<HightStockShoulderViewModelForJavaScript> highStockModelExtension = lIPatientRxRepository.getPatientRxEquipmentROMByExtensionInHighStockChartForJavaScript(id, equipmentType, actuator);
                            List<HightStockShoulderViewModelForJavaScript> highStockModelFlexionExtension = lIPatientRxRepository.getPatientRxEquipmentROMByFlexionExtensionInHighStockChartForJavaScript(id, equipmentType, actuator);
                            ViewBag.FlexionData = JsonConvert.SerializeObject(highStockModelFlexion, lsetting1);
                            ViewBag.ExtensionData = JsonConvert.SerializeObject(highStockModelExtension, lsetting1);
                            ViewBag.FlexionExtensionData = JsonConvert.SerializeObject(highStockModelFlexionExtension, lsetting1);

                            //Equipment ROM
                            ViewBag.EquipmentType = equipmentType;


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
                }
                else if (!string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(tab) && tab == "Details")
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    NewPatient _newPatient = new NewPatient();
                    if (TempData["Patient"] != null && !string.IsNullOrEmpty(TempData["Patient"].ToString()))
                    {
                        _newPatient = JsonConvert.DeserializeObject<NewPatient>(TempData["Patient"].ToString());
                    }
                    getDetails();
                    List<Sides> Sides = Utilities.GetSide();
                    list = new List<SelectListItem>();
                    foreach (Sides ex in Sides)
                    {
                        list.Add(new SelectListItem { Text = ex.Side.ToString(), Value = ex.Side.ToString() });
                    }
                    ViewBag.sides = new SelectList(list, "Value", "Text");

                    _newPatient = INewPatient.GetPatientByPatId(Convert.ToInt32(id));
                    ViewBag.PatientName = _newPatient.PatientName;
                    _newPatient.Action = "edit";
                    _newPatient.Actuator = actuator;

                    if (HttpContext.Session.GetString("UserType") == ConstantsVar.Admin.ToString() || HttpContext.Session.GetString("UserType") == ConstantsVar.Support.ToString())
                    {
                        List<User> _userProviderlist = lIUserRepository.getUserListByType(ConstantsVar.Provider);

                        var ObjList = _userProviderlist.Select(r => new SelectListItem
                        {
                            Value = r.UserId.ToString(),
                            Text = r.Name
                        });
                        ViewBag.Provider = new SelectList(ObjList, "Value", "Text");


                    }
                    List<User> _userTherapistlist = lIUserRepository.getUserListByType(ConstantsVar.Therapist);

                    var ObjListTherapist = _userTherapistlist.Select(r => new SelectListItem
                    {
                        Value = r.UserId.ToString(),
                        Text = r.Name
                    });
                    ViewBag.Therapist = new SelectList(ObjListTherapist, "Value", "Text");

                    List<User> _patientAdministratorlist = lIUserRepository.getUserListByType(ConstantsVar.PatientAdministrator);

                    var ObjListPatientAdmin = _patientAdministratorlist.Select(r => new SelectListItem
                    {
                        Value = r.UserId.ToString(),
                        Text = r.Name
                    });
                    ViewBag.PatientAdministrator = new SelectList(ObjListPatientAdmin, "Value", "Text");
                    _newPatient.returnView = "review";
                    lmodel.Patient = _newPatient;
                }
                else if (!string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(tab) && tab == "Rx")
                {
                    NewPatientWithProtocol newPatientWithProtocol = new NewPatientWithProtocol();
                    List<EquipmentExcercise> EquipmentExcerciselist = lIEquipmentExerciseRepository.GetEquipmentExcercise();
                    List<ExcerciseProtocol> ExcerciseProtocollist = lIEquipmentExerciseRepository.GetExcerciseProtocol();
                    newPatientWithProtocol.NewPatientRXList = new List<NewPatientRx>();
                    List<PatientRx> PatientRx = null;

                    ViewBag.Action = "edit";

                    PatientRx = INewPatient.GetNewPatientRxByPatId(id.ToString());
                    if (PatientRx != null && PatientRx.Count > 0)
                    {
                        //Need to display patientrx by deviceconfiguration orderby flexionString and ExtensionString in EquipmentExercise table
                        if (PatientRx[0].EquipmentType.ToLower() != "ankle")
                        {
                            PatientRx = PatientRx.OrderByDescending(x => x.DeviceConfiguration).ToList();
                        }
                        foreach (PatientRx patRx in PatientRx)
                        {
                            ViewBag.EquipmentType = patRx.EquipmentType;
                            ViewBag.PatientName = patRx.Patient.PatientName;
                            ViewBag.SurgeryDate = patRx.Patient.SurgeryDate;
                            EquipmentExcercise EquipmentExcercise = EquipmentExcerciselist.Where(p => p.Limb.ToLower() == patRx.EquipmentType.ToLower() && p.ExcerciseEnum == patRx.DeviceConfiguration).FirstOrDefault();
                            ExcerciseProtocol ExcerciseProtocol = ExcerciseProtocollist.Where(p => p.Limb == patRx.EquipmentType && p.ExcerciseEnum == patRx.DeviceConfiguration).Distinct().FirstOrDefault();
                            NewPatientRx _NewPatientRx = new NewPatientRx();
                            _NewPatientRx.Action = "edit";

                            DeviceCalibration ldeviceCalibration = lIDeviceCalibrationRepository.getDeviceCalibrationByRxID(patRx.RxId);
                            if (patRx.EquipmentType.ToLower() == "shoulder")
                            {

                                if (patRx.DeviceConfiguration == "Forward Flexion")
                                {
                                    string flexionString = "Degree of Flexion";
                                    if (lExerciseString != null && lExerciseString.Count > 0)
                                    {
                                        var lexString = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == patRx.DeviceConfiguration).FirstOrDefault();
                                        if (lexString != null)
                                        {
                                            flexionString = "Degree of " + lexString.FlexionString;
                                        }
                                    }
                                    _NewPatientRx.HeadingFlexion = flexionString;
                                    _NewPatientRx.CurrentFlex = Constants.Sh_Flex_Current;
                                    _NewPatientRx.GoalFlex = Constants.Sh_Flex_Goal;

                                    _NewPatientRx.Flex_Current_Start = Constants.Sh_Flex_Current;
                                    _NewPatientRx.Flex_Current_End = Constants.Sh_Flex_Goal;
                                    _NewPatientRx.Flex_Goal_Start = Constants.Sh_Flex_Current;
                                    _NewPatientRx.Flex_Goal_End = Constants.Sh_Flex_Goal;
                                }
                                if (patRx.DeviceConfiguration == "External Rotation")
                                {
                                    string flexionString = "Degree of External Rotation";
                                    if (lExerciseString != null && lExerciseString.Count > 0)
                                    {
                                        var lexString = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == patRx.DeviceConfiguration).FirstOrDefault();
                                        if (lexString != null)
                                        {
                                            flexionString = "Degree of " + lexString.FlexionString;
                                        }
                                    }
                                    _NewPatientRx.HeadingFlexion = flexionString;
                                    _NewPatientRx.CurrentFlex = Constants.Sh_ExRot_Current;
                                    _NewPatientRx.GoalFlex = Constants.Sh_ExRot_Goal;

                                    _NewPatientRx.Flex_Current_Start = Constants.Sh_ExRot_Current;
                                    _NewPatientRx.Flex_Current_End = Constants.Sh_ExRot_Goal;
                                    _NewPatientRx.Flex_Goal_Start = Constants.Sh_ExRot_Current;
                                    _NewPatientRx.Flex_Goal_End = Constants.Sh_ExRot_Goal;
                                }

                            }
                            else if (patRx.EquipmentType.ToLower() == "ankle")
                            {
                                string flexionString = "Degree of Flexion";
                                string extensionString = "Degree of Extension";
                                if (lExerciseString != null && lExerciseString.Count > 0)
                                {
                                    var lexString = lExerciseString.Where(x => x.Limb == "Ankle").FirstOrDefault();
                                    if (lexString != null)
                                    {
                                        flexionString = "Degree of " + lexString.FlexionString;
                                        extensionString = "Degree of " + lexString.ExtensionString;
                                    }
                                }
                                _NewPatientRx.HeadingFlexion = flexionString;
                                _NewPatientRx.HeadingExtension = extensionString;
                                _NewPatientRx.CurrentFlex = Constants.Ankle_Flex_Current;
                                _NewPatientRx.GoalFlex = Constants.Ankle_Flex_Goal;
                                _NewPatientRx.CurrentExten = Constants.Ankle_Ext_Current;
                                _NewPatientRx.GoalExten = Constants.Ankle_Ext_Goal;



                                _NewPatientRx.Flex_Current_Start = Constants.Ankle_Flex_Current;
                                _NewPatientRx.Flex_Current_End = Constants.Ankle_Flex_Goal;
                                _NewPatientRx.Flex_Goal_Start = Constants.Ankle_Flex_Current;
                                _NewPatientRx.Flex_Goal_End = Constants.Ankle_Flex_Goal;

                                _NewPatientRx.Ext_Current_Start = Constants.Ankle_Ext_Current;
                                _NewPatientRx.Ext_Current_End = Constants.Ankle_Ext_Goal;
                                _NewPatientRx.Ext_Goal_Start = Constants.Ankle_Ext_Current;
                                _NewPatientRx.Ext_Goal_End = Constants.Ankle_Ext_Goal;
                            }
                            else
                            {
                                string flexionString = "Degree of Flexion";
                                string extensionString = "Degree of Extension";
                                if (lExerciseString != null && lExerciseString.Count > 0)
                                {
                                    var lexString = lExerciseString.Where(x => x.Limb == "Knee").FirstOrDefault();
                                    if (lexString != null)
                                    {
                                        flexionString = "Degree of " + lexString.FlexionString;
                                        extensionString = "Degree of " + lexString.ExtensionString;
                                    }
                                }
                                _NewPatientRx.HeadingFlexion = flexionString;
                                _NewPatientRx.HeadingExtension = extensionString;
                                _NewPatientRx.CurrentFlex = Constants.Knee_Flex_Current;
                                _NewPatientRx.GoalFlex = Constants.Knee_Flex_Goal;
                                _NewPatientRx.CurrentExten = Constants.Knee_Ext_Current;
                                _NewPatientRx.GoalExten = Constants.Knee_Ext_Goal;

                                _NewPatientRx.Flex_Current_Start = Constants.Knee_Flex_Current_Start;
                                _NewPatientRx.Flex_Current_End = Constants.Knee_Flex_Current_End;
                                _NewPatientRx.Flex_Goal_Start = Constants.Knee_Flex_Goal_Start;
                                _NewPatientRx.Flex_Goal_End = (ldeviceCalibration != null && ldeviceCalibration.Actuator2ExtendedAngle != null && (Convert.ToInt32(ldeviceCalibration.Actuator2ExtendedAngle) > Constants.Knee_Flex_Goal_End)) ? Convert.ToInt32(ldeviceCalibration.Actuator2ExtendedAngle) : Constants.Knee_Flex_Goal_End;
                                ViewBag.Knee_Flexion_Goal_End = _NewPatientRx.Flex_Goal_End;

                                _NewPatientRx.Ext_Current_Start = Constants.Knee_Ext_Current_Start;
                                _NewPatientRx.Ext_Current_End = Constants.Knee_Ext_Current_End;
                                _NewPatientRx.Ext_Goal_Start = Constants.Knee_Ext_Goal_Start;
                                _NewPatientRx.Ext_Goal_End = (ldeviceCalibration != null && ldeviceCalibration.Actuator2RetractedAngle != null && (Convert.ToInt32(ldeviceCalibration.Actuator2RetractedAngle) < Constants.Knee_Ext_Goal_End)) ? Convert.ToInt32(ldeviceCalibration.Actuator2RetractedAngle) : Constants.Knee_Ext_Goal_End;
                                ViewBag.Knee_Extension_Goal_End = _NewPatientRx.Ext_Goal_End;


                            }
                            _NewPatientRx.TherapyType = EquipmentExcercise.ExcerciseName;
                            _NewPatientRx.DeviceConfiguration = EquipmentExcercise.ExcerciseEnum;
                            _NewPatientRx.ProtocolEnum = ExcerciseProtocol.ProtocolEnum;
                            _NewPatientRx.ProtocolName = ExcerciseProtocol.ProtocolName;
                            _NewPatientRx.EquipmentType = patRx.EquipmentType;

                            _NewPatientRx.RxId = patRx.RxId;
                            _NewPatientRx.RxDaysPerweek = patRx.RxDaysPerweek;
                            _NewPatientRx.RxSessionsPerWeek = patRx.RxSessionsPerWeek;
                            _NewPatientRx.RxStartDate = patRx.RxStartDate;
                            _NewPatientRx.RxEndDate = patRx.RxEndDate;
                            _NewPatientRx.ProviderId = patRx.ProviderId;
                            _NewPatientRx.PatientId = patRx.PatientId;

                            _NewPatientRx.CurrentExtension = patRx.CurrentExtension;
                            _NewPatientRx.CurrentFlexion = patRx.CurrentFlexion;
                            _NewPatientRx.GoalExtension = patRx.GoalExtension;
                            _NewPatientRx.GoalFlexion = patRx.GoalFlexion;
                            _NewPatientRx.PainThreshold = patRx.PainThreshold;
                            _NewPatientRx.RateOfChange = patRx.RateOfChange;

                            newPatientWithProtocol.PainThreshold = patRx.PainThreshold;
                            newPatientWithProtocol.RateOfChange = patRx.RateOfChange;
                            newPatientWithProtocol.NewPatientRXList.Add(_NewPatientRx);
                        }
                    }
                    newPatientWithProtocol.returnView = "Rx";
                    lmodel.Rx = newPatientWithProtocol;
                }
                else if (!string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(tab) && tab == "Exercises")
                {
                    ViewBag.PatientName = Username;
                    List<NewProtocol> _result = INewPatient.GetProtocolListBypatId(id.ToString());
                    ViewBag.VisibleAddButton = false;
                    ViewBag.VisibleAddButtonExt = false;
                    if (!string.IsNullOrEmpty(equipmentType))
                    {
                        if (equipmentType.ToLower() == "shoulder")
                        {
                            if (lExerciseString != null && lExerciseString.Count > 0)
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
                            PatientConfiguration lconfig = lIPatientConfigurationRepository.getPatientConfigurationbyPatientId(id, equipmentType, "Forward Flexion");
                            if (lconfig != null)
                            {
                                ViewBag.VisibleAddButton = true;
                                if (_result != null)
                                {
                                    List<NewProtocol> _resultFlex = _result.Where(x => x.ExcerciseEnum == "Forward Flexion").ToList();
                                    if (_resultFlex == null || (_resultFlex != null && _resultFlex.Count == 0))
                                    {
                                        ViewBag.RxId = lconfig.RxId;
                                    }
                                }
                            }
                            PatientConfiguration lconfigext = lIPatientConfigurationRepository.getPatientConfigurationbyPatientId(Convert.ToInt32(id), equipmentType, "External Rotation");
                            if (lconfigext != null)
                            {
                                ViewBag.VisibleAddButtonExt = true;
                                if (_result != null)
                                {
                                    List<NewProtocol> _resultExt = _result.Where(x => x.ExcerciseEnum == "External Rotation").ToList();
                                    if (_resultExt == null || (_resultExt != null && _resultExt.Count == 0))
                                    {
                                        ViewBag.RxIdExt = lconfig.RxId;
                                    }
                                }

                            }
                        }
                        else
                        {
                            PatientConfiguration lconfig = lIPatientConfigurationRepository.getPatientConfigurationbyPatientId(Convert.ToInt32(id), equipmentType);

                            if (lconfig != null)
                            {
                                ViewBag.VisibleAddButton = true;

                                if (_result == null || (_result != null && _result.Count == 0))
                                {
                                    ViewBag.RxId = lconfig.RxId;
                                }
                                else
                                {
                                    ViewBag.RxId = _result[0].RxId;
                                }
                            }
                            if (lExerciseString != null && lExerciseString.Count > 0)
                            {
                                var lexString = lExerciseString.Where(x => x.Limb == equipmentType).FirstOrDefault();
                                if (lexString != null)
                                {
                                    ViewBag.flexionString = lexString.FlexionString;
                                    ViewBag.extensionString = lexString.ExtensionString;
                                }
                            }
                        }
                    }


                    ViewBag.ProtocolList = _result;
                    ViewBag.etype = equipmentType;

                    if (String.IsNullOrEmpty(Username))
                        ViewBag.PatientName = Username;
                    if (_result != null && _result.Count > 0 && String.IsNullOrEmpty(Username))
                    {

                        ViewBag.PatientName = _result[0].PatientName;
                    }

                    lmodel.Exercises = _result;
                }
                else if (!string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(tab) && tab == "Sessions")
                {
                    if (!String.IsNullOrEmpty(id.ToString()))
                    {
                        ViewBag.actuator = actuator;
                        ViewBag.EquipmentType = equipmentType;
                        ViewBag.patientId = id;
                        ViewBag.PatientName = Username;
                        HttpContext.Session.SetString("PatientID", id.ToString());
                    }
                    if (string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(equipmentType))
                    {
                        if (equipmentType.ToLower() == "shoulder")
                        {
                            actuator = "Forward Flexion";
                        }
                        else
                        {
                            actuator = "Flexion-Extension";
                        }
                        ViewBag.actuator = actuator;
                    }
                    if (lExerciseString != null && lExerciseString.Count > 0 && equipmentType.ToLower() == "shoulder")
                    {
                        var lexString = lExerciseString.Where(x => x.Limb == equipmentType && x.ExerciseEnum == "Forward Flexion").FirstOrDefault();
                        if (lexString != null)
                        {
                            ViewBag.flexionString = lexString.FlexionString;
                        }
                        var lexString1 = lExerciseString.Where(x => x.Limb == equipmentType && x.ExerciseEnum == "External Rotation").FirstOrDefault();
                        if (lexString1 != null)
                        {
                            ViewBag.flexionString1 = lexString1.FlexionString;
                        }
                    }
                    else if (lExerciseString != null && lExerciseString.Count > 0)
                    {
                        var lexString = lExerciseString.Where(x => x.Limb == equipmentType).FirstOrDefault();
                        if (lexString != null)
                        {
                            ViewBag.flexionString = lexString.FlexionString;
                            ViewBag.extensionString = lexString.ExtensionString;
                        }
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

                    HttpContext.Session.SetString("ProtocolName", "");
                    HttpContext.Session.SetString("ProtocoloId", "");
                    if (!String.IsNullOrEmpty(Username) && !String.IsNullOrEmpty(id.ToString()))
                        pSession = lISessionInterface.getSessionList(id.ToString(), actuator);
                    else
                    {
                        if (String.IsNullOrEmpty(Username) && String.IsNullOrEmpty(id.ToString()) && HttpContext.Session.GetString("UserId") != null && HttpContext.Session.GetString("UserType") != null && HttpContext.Session.GetString("UserType").ToString() == "2")
                        {
                            pSession = lISessionInterface.getSessionListByTherapistId(HttpContext.Session.GetString("UserId"));
                        }
                        else
                        {
                            pSession = lISessionInterface.getSessionList();
                        }
                    }
                    if (pSession != null && pSession.Count > 0)
                    {
                        lmodel.Sessions = pSession;
                        ViewBag.SessionList = pSession;
                    }
                }
                else if (!string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(tab) && tab == "History")
                {
                    ViewBag.History = lIAppointmentScheduleRepository.getAppointmentListByPatientId(Convert.ToInt32(id), HttpContext.Session.GetString("timezoneid"));
                }
                else if (!string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(tab) && tab == "Messages")
                {
                    Patient lpatient = IPatient.GetPatientByPatientID(id);
                    if (lpatient != null)
                    {
                        List<MessageView> lmessages = lIMessageRepository.getMessagesbyTimeZone(lpatient.PatientLoginId, HttpContext.Session.GetString("UserId"), HttpContext.Session.GetString("timezoneid"));
                        ViewBag.PatientId = lpatient.PatientLoginId;
                        ViewBag.Messages = lmessages.OrderBy(x => x.Datetime);
                    }
                }
                else if (!string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(tab) && tab == "PatientReviews")
                {
                    List<PatientReviewTab> lPatientReviews = new List<PatientReviewTab>();
                    try
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                        {
                            ViewBag.lPatientReviews = lIPatientReviewRepository.GetPatientReviewListTab(ViewBag.User);

                            lmodel.PatientReviews = lPatientReviews;
                        }
                        else
                        {
                            ViewBag.lPatientReviews = lIPatientReviewRepository.GetPatientReviewList();
                            lmodel.PatientReviews = ViewBag.lPatientReviews;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogDebug("Patient Review Error: " + ex);
                        return View();
                    }

                }
                else if (!string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(tab) && tab == "Library")
                {
                    Patient lpatient = IPatient.GetPatientByPatientID(id);
                    if (lpatient != null)
                    {
                        ViewBag.Library = lIPatientLibraryRepository.getPatientLibrary(lpatient.PatientLoginId, lpatient.EquipmentType, actuator);
                        ViewBag.PatientLibrary = lIPatientLibraryRepository.getPatientLibraryByPatientId(lpatient.PatientLoginId);
                    }

                }
                return View(lmodel);
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
                return View(null);
            }

        }


        private List<ColumnrangeSeriesData> ColumnrangeSeriesDataToDatabase_Apple()
        {
            List<ColumnrangeSeriesData> appleData = new List<ColumnrangeSeriesData>();
            string url = "https://www.highcharts.com/samples/data/aapl-ohlcv.json";
            string json;

            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(url);
            }

            json = json.Substring(json.IndexOf('[') + 1);
            json = json.Substring(json.IndexOf('[') + 1);


            while (true)
            {
                if (json.IndexOf('[') == -1)
                    break;

                string entity = json.Substring(0, json.IndexOf(']'));
                string[] values = entity.Split(',');

                appleData.Add(new ColumnrangeSeriesData
                {
                    X = Convert.ToDouble(values[0]),
                    High = Convert.ToDouble(values[3]),
                    Low = Convert.ToDouble(Convert.ToDouble(values[4]) - new Random().Next(15))
                });

                json = json.Substring(json.IndexOf('[') + 1);
            }

            return appleData;

        }

        private List<ColumnSeriesData> VolumnJsonDataToDatabase_Apple()
        {
            List<ColumnSeriesData> appleData = new List<ColumnSeriesData>();
            string url = "https://www.highcharts.com/samples/data/aapl-ohlcv.json";
            string json;

            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(url);
            }

            json = json.Substring(json.IndexOf('[') + 1);
            json = json.Substring(json.IndexOf('[') + 1);


            while (true)
            {
                if (json.IndexOf('[') == -1)
                    break;

                string entity = json.Substring(0, json.IndexOf(']'));
                string[] values = entity.Split(',');

                appleData.Add(new ColumnSeriesData
                {
                    X = Convert.ToDouble(values[0]),
                    Y = Convert.ToDouble(values[5])
                });

                json = json.Substring(json.IndexOf('[') + 1);
            }

            return appleData;

        }

        public void getDetails()
        {
            List<SelectListItem> myList = new List<SelectListItem>()
                         {
                            new SelectListItem{ Value="Ankle",Text="Ankle"},
                            new SelectListItem{ Value="Knee",Text="Knee"},
                            new SelectListItem{ Value="Shoulder",Text="Shoulder"}
                         };
            ViewBag.equipment = myList;

        }

        [HttpPost]
        [ActionName("updatereviewactivity")]
        public JsonResult updatereviewactivity([FromBody]PatientReview log)
        {
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ReviewID")))
                {
                    User luser = lIUserRepository.getUserbySessionId(HttpContext.Session.GetString("SessionId"));
                    if (luser != null)
                    {
                        PatientReview lreview = lIPatientReviewRepository.GetPatientReview(HttpContext.Session.GetString("ReviewID"));
                        if (lreview != null)
                        {

                            lreview.Duration = Convert.ToInt32((DateTime.Now - lreview.StartTimeStamp).TotalSeconds);
                            lreview.AssessmentComment = log.AssessmentComment;

                            int _result = lIPatientReviewRepository.UpdatePatientReview(lreview);
                            if (_result > 0)
                            {
                                HttpContext.Session.SetString("ReviewID", "");
                                HttpContext.RemoveCookie("ReviewID");
                                return Json(new { result = "success" });
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            return Json(new { result = "failure" });
        }

    }

}
