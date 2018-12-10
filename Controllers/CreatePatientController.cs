using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.ViewModels;
using OneDirect.Helper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OneDirect.Repository.Interface;
using OneDirect.Models;
using Microsoft.Extensions.Logging;
using OneDirect.Repository;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class CreatePatientController : Controller
    {
        // GET: /<controller>/
        private readonly IEquipmentExerciseInterface lIEquipmentExerciseRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly IUserActivityLogInterface lIUserActivityLogRepository;
        private readonly IDeviceCalibrationInterface lIDeviceCalibrationRepository;
        private readonly IPatientConfigurationInterface lIPatientConfigurationRepository;
        private readonly INewPatient INewPatient;
        private readonly ILogger logger;
        private OneDirectContext context;

        public CreatePatientController(OneDirectContext context, ILogger<CreatePatientController> plogger, IEquipmentExerciseInterface IEquipmentExerciseRepository, INewPatient lINewPatient)
        {
            lIEquipmentExerciseRepository = IEquipmentExerciseRepository;
            logger = plogger;
            this.context = context;
            INewPatient = lINewPatient;
            lIUserActivityLogRepository = new UserActivityLogRepository(context);
            lIUserRepository = new UserRepository(context);
            lIDeviceCalibrationRepository = new DeviceCalibrationRepository(context);
            lIPatientConfigurationRepository = new PatientConfigurationRepository(context);
        }

        public IActionResult ViewPatient(string patid = "", string operaton = "")
        {
            List<SelectListItem> list = new List<SelectListItem>();
            NewPatient _newPatient = new NewPatient();
            if (TempData["Patient"] != null && !string.IsNullOrEmpty(TempData["Patient"].ToString()))
            {
                _newPatient = JsonConvert.DeserializeObject<NewPatient>(TempData["Patient"].ToString());
            }
            getDetails();
            ViewBag.Action = operaton;
            List<Sides> Sides = Utilities.GetSide();
            list = new List<SelectListItem>();
            foreach (Sides ex in Sides)
            {
                list.Add(new SelectListItem { Text = ex.Side.ToString(), Value = ex.Side.ToString() });
            }
            ViewBag.sides = new SelectList(list, "Value", "Text");

            if (!String.IsNullOrEmpty(operaton) && operaton == "view")
            {
                _newPatient = INewPatient.GetPatientByPatId(Convert.ToInt32(patid));
                ViewBag.PatientName = _newPatient.PatientName;
                _newPatient.Action = operaton;
            }

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

            return View(_newPatient);
        }


        //check if new patient then opens empty form and if opertion is edit then show the details of patients in edit mode
        public IActionResult CreatePatient(string patid = "", string operaton = "")
        {
            List<SelectListItem> list = new List<SelectListItem>();
            NewPatient _newPatient = new NewPatient();
            if (TempData["Patient"] != null && !string.IsNullOrEmpty(TempData["Patient"].ToString()))
            {
                _newPatient = JsonConvert.DeserializeObject<NewPatient>(TempData["Patient"].ToString());
            }
            getDetails();
            ViewBag.Action = operaton;
            List<Sides> Sides = Utilities.GetSide();
            list = new List<SelectListItem>();
            foreach (Sides ex in Sides)
            {
                list.Add(new SelectListItem { Text = ex.Side.ToString(), Value = ex.Side.ToString() });
            }
            ViewBag.sides = new SelectList(list, "Value", "Text");

            if (!String.IsNullOrEmpty(operaton) && operaton == "edit")
            {
                _newPatient = INewPatient.GetPatientByPatId(Convert.ToInt32(patid));
                ViewBag.PatientName = _newPatient.PatientName;
                _newPatient.Action = operaton;
            }

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

            return View(_newPatient);

        }


        //check if the patient already exist with same patientId, if yes edit it, if not create one.
        [HttpPost]
        public IActionResult CreatePatient(NewPatient _NewPatient)
        {
            try
            {
                if (_NewPatient.Action == "edit")
                {
                    //getting existing record
                    Patient lexistingpatient = (from p in context.Patient
                                                where p.PatientId == _NewPatient.PatientId
                                                select p).FirstOrDefault();

                    //Insert existing record to User Activity Log
                    UserActivityLog llog1 = new UserActivityLog();
                    llog1.SessionId = HttpContext.Session.GetString("SessionId");
                    llog1.ActivityType = "Update";
                    llog1.StartTimeStamp = !string.IsNullOrEmpty(HttpContext.Session.GetString("SessionTime")) ? Convert.ToDateTime(HttpContext.Session.GetString("SessionTime")) : DateTime.Now;
                    llog1.Duration = Convert.ToInt32((DateTime.Now - Convert.ToDateTime(HttpContext.Session.GetString("SessionTime"))).TotalSeconds);
                    llog1.RecordChangeType = "Edit";
                    llog1.RecordType = "Patient";
                    llog1.Comment = "Record edited";
                    llog1.RecordExistingJson = JsonConvert.SerializeObject(lexistingpatient);
                    llog1.UserId = HttpContext.Session.GetString("UserId");
                    llog1.UserName = HttpContext.Session.GetString("UserName");
                    llog1.UserType = HttpContext.Session.GetString("UserType");
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ReviewID")))
                    {
                        llog1.ReviewId = HttpContext.Session.GetString("ReviewID");
                    }
                    //inserting the existing record in user activity log table
                    lIUserActivityLogRepository.InsertUserActivityLog(llog1);
                    //kajal
                    _NewPatient.PhoneNumber = RemoveSpecialChars(_NewPatient.PhoneNumber);
                    //update the patient
                    Patient _result = INewPatient.UpdatePatient(_NewPatient);
                    if (_result != null && llog1.ActivityId > 0)
                    {
                        //Insert to User Activity Log
                        UserActivityLog llog = lIUserActivityLogRepository.GetUserActivityLog(llog1.ActivityId);
                        if (llog == null)
                            llog = new UserActivityLog();
                        llog.SessionId = HttpContext.Session.GetString("SessionId");
                        llog.ActivityType = "Update";
                        llog.StartTimeStamp = !string.IsNullOrEmpty(HttpContext.Session.GetString("SessionTime")) ? Convert.ToDateTime(HttpContext.Session.GetString("SessionTime")) : DateTime.Now;
                        llog.Duration = Convert.ToInt32((DateTime.Now - Convert.ToDateTime(HttpContext.Session.GetString("SessionTime"))).TotalSeconds);
                        llog.RecordChangeType = "Edit";
                        llog.RecordType = "Patient";
                        llog.Comment = "Record edited";
                        llog.RecordJson = JsonConvert.SerializeObject(_result);
                        llog.UserId = HttpContext.Session.GetString("UserId");
                        llog.UserName = HttpContext.Session.GetString("UserName");
                        llog.UserType = HttpContext.Session.GetString("UserType");
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ReviewID")))
                        {
                            llog.ReviewId = HttpContext.Session.GetString("ReviewID");
                        }
                        if (llog.ActivityId != 0)
                            lIUserActivityLogRepository.UpdateUserActivityLog(llog);
                        else
                            lIUserActivityLogRepository.InsertUserActivityLog(llog);

                        //Prabhu
                        User pUser = lIUserRepository.getUser(_NewPatient.PatientLoginId);
                        if (pUser != null)
                        {
                            User lpatient = new Models.User();
                            lpatient.UserId = _NewPatient.PatientLoginId;
                            lpatient.Name = _NewPatient.PatientName;
                            lpatient.Password = _NewPatient.Pin.HasValue ? _NewPatient.Pin.Value.ToString() : "";
                            lpatient.Type = ConstantsVar.Patient;
                            lpatient.Email = "";
                            lpatient.Address = _NewPatient.AddressLine + " " + _NewPatient.City + " " + _NewPatient.State + " " + _NewPatient.Zip;
                            lpatient.Phone = _NewPatient.PhoneNumber;
                            lIUserRepository.UpdateUser(lpatient);
                        }
                        else
                        {
                            User lpatient = new Models.User();
                            lpatient.UserId = _NewPatient.PatientLoginId;
                            lpatient.Name = _NewPatient.PatientName;
                            lpatient.Password = _NewPatient.Pin.HasValue ? _NewPatient.Pin.Value.ToString() : "";
                            lpatient.Type = ConstantsVar.Patient;
                            lpatient.Email = "";
                            lpatient.Address = _NewPatient.AddressLine + " " + _NewPatient.City + " " + _NewPatient.State + " " + _NewPatient.Zip;


                            lIUserRepository.InsertUser(lpatient);
                        }
                        if (!string.IsNullOrEmpty(_NewPatient.returnView))
                        {
                            return RedirectToAction("Index", "Review", new { id = _NewPatient.PatientId, Username = _NewPatient.PatientName, EquipmentType = _NewPatient.EquipmentType, actuator = _NewPatient.Actuator, tab = "Details" });
                        }
                        else
                        {
                            if (HttpContext.Session.GetString("UserType") == ConstantsVar.Provider.ToString())
                            {
                                return RedirectToAction("Dashboard", "Provider", new { id = _NewPatient.ProviderId });
                            }
                            else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Admin.ToString())
                                return RedirectToAction("Index", "Patient");
                            else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Therapist.ToString())
                                return RedirectToAction("Dashboard", "Therapist", new { id = HttpContext.Session.GetString("UserId") });
                            else if (HttpContext.Session.GetString("UserType") == ConstantsVar.PatientAdministrator.ToString())
                                return RedirectToAction("Dashboard", "PatientAdministrator", new { id = HttpContext.Session.GetString("UserId") });
                            else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Support.ToString())
                                return RedirectToAction("Dashboard", "Support", new { id = HttpContext.Session.GetString("UserId") });
                            else
                            {
                                return RedirectToAction("Index", "Patient");
                            }
                        }
                    }
                }
                else
                {


                    string patientId = _NewPatient.Ssn + _NewPatient.PhoneNumber.Substring(_NewPatient.PhoneNumber.Length - 4);
                    NewPatient lexistpatient = INewPatient.GetPatientByPatitentLoginId(patientId);
                    if (lexistpatient == null)
                    {
                        NewPatientWithProtocol Patient = new NewPatientWithProtocol();
                        Patient.NewPatient = new NewPatient();
                        _NewPatient.PhoneNumber = RemoveSpecialChars(_NewPatient.PhoneNumber);
                        Patient.NewPatient = _NewPatient;
                        HttpContext.Session.SetString("NewPatient", JsonConvert.SerializeObject(Patient));

                        return RedirectToAction("PatientRX");
                    }
                    else
                    {
                        TempData["Patient"] = JsonConvert.SerializeObject(_NewPatient);
                        TempData["msg"] = "<script>Helpers.ShowMessage('Patient SSN and Phonenumber is already registered, please use different one', 1);</script>";
                        return RedirectToAction("CreatePatient");
                    }


                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            return RedirectToAction("PatientRX");
        }

        //removing all special characters from phone number before saving in database
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

        public IActionResult PatientRX(string patid = "", string operaton = "")
        {
            NewPatientWithProtocol newPatientWithProtocol = new NewPatientWithProtocol();
            List<EquipmentExcercise> EquipmentExcerciselist = lIEquipmentExerciseRepository.GetEquipmentExcercise();
            List<ExcerciseProtocol> ExcerciseProtocollist = lIEquipmentExerciseRepository.GetExcerciseProtocol();
            newPatientWithProtocol.NewPatientRXList = new List<NewPatientRx>();
            List<PatientRx> PatientRx = null;

            ViewBag.Action = operaton;
            List<EquipmentExercise> lExerciseString = lIEquipmentExerciseRepository.GetEquipmentExerciseString();
            if (!String.IsNullOrEmpty(operaton) && operaton == "view")
            {
                PatientRx = INewPatient.GetNewPatientRxByPatId(patid);
                if (PatientRx != null && PatientRx.Count > 0)
                {
                    foreach (PatientRx patRx in PatientRx)
                    {
                        ViewBag.EquipmentType = patRx.EquipmentType;
                        ViewBag.PatientName = patRx.Patient.PatientName;
                        ViewBag.SurgeryDate = patRx.Patient.SurgeryDate;
                        EquipmentExcercise EquipmentExcercise = EquipmentExcerciselist.Where(p => p.Limb.ToLower() == patRx.EquipmentType.ToLower() && p.ExcerciseEnum == patRx.DeviceConfiguration).FirstOrDefault();
                        ExcerciseProtocol ExcerciseProtocol = ExcerciseProtocollist.Where(p => p.Limb == patRx.EquipmentType && p.ExcerciseEnum == patRx.DeviceConfiguration).Distinct().FirstOrDefault();
                        NewPatientRx _NewPatientRx = new NewPatientRx();
                        _NewPatientRx.Action = "view";

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
                            _NewPatientRx.Flex_Goal_End = (ldeviceCalibration != null && ldeviceCalibration.Actuator2ExtendedAngle != null) ? Convert.ToInt32(ldeviceCalibration.Actuator2ExtendedAngle) : Constants.Knee_Flex_Goal_End;

                            _NewPatientRx.Ext_Current_Start = Constants.Knee_Ext_Current_Start;
                            _NewPatientRx.Ext_Current_End = Constants.Knee_Ext_Current_End;
                            _NewPatientRx.Ext_Goal_Start = Constants.Knee_Ext_Goal_Start;
                            _NewPatientRx.Ext_Goal_End = (ldeviceCalibration != null && ldeviceCalibration.Actuator2RetractedAngle != null) ? Convert.ToInt32(ldeviceCalibration.Actuator2RetractedAngle) : Constants.Knee_Ext_Goal_End;


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
            }
            else
            {
                NewPatientWithProtocol _NewPatient = new NewPatientWithProtocol();
                if (HttpContext.Session.GetString("NewPatient") != null)
                {
                    _NewPatient = JsonConvert.DeserializeObject<NewPatientWithProtocol>(HttpContext.Session.GetString("NewPatient").ToString());
                }
                else
                {
                    return RedirectToAction("CreatePatient");
                }

                EquipmentExcerciselist = EquipmentExcerciselist.Where(p => p.Limb.ToLower() == _NewPatient.NewPatient.EquipmentType.ToLower()).ToList();

                newPatientWithProtocol.NewPatientRXList = new List<NewPatientRx>();

                ExcerciseProtocollist = ExcerciseProtocollist.Where(p => p.Limb == _NewPatient.NewPatient.EquipmentType).Distinct().ToList();

                NewPatientRx _NewPatientRx = null;

                if (_NewPatient.NewPatient.EquipmentType.ToLower() == "shoulder")
                {
                    string flexionString = "Degree of Forward Flexion";
                    string flexionString1 = "Degree of External Rotation";
                    if (lExerciseString != null && lExerciseString.Count > 0)
                    {
                        var lexString = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "Forward Flexion").FirstOrDefault();
                        if (lexString != null)
                        {
                            flexionString = "Degree of " + lexString.FlexionString;
                        }
                        var lexString1 = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "External Rotation").FirstOrDefault();
                        if (lexString1 != null)
                        {
                            flexionString1 = "Degree of " + lexString1.FlexionString;
                        }

                    }
                    _NewPatientRx = new NewPatientRx();
                    _NewPatientRx.Action = "add";
                    _NewPatientRx.TherapyType = EquipmentExcerciselist[0].ExcerciseName;
                    _NewPatientRx.DeviceConfiguration = EquipmentExcerciselist[0].ExcerciseEnum;
                    _NewPatientRx.HeadingFlexion = flexionString;
                    _NewPatientRx.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                    _NewPatientRx.ProtocolEnum = ExcerciseProtocollist[0].ProtocolEnum;
                    _NewPatientRx.ProtocolName = ExcerciseProtocollist[0].ProtocolName;
                    _NewPatientRx.CurrentFlex = Constants.Sh_Flex_Current;
                    _NewPatientRx.GoalFlex = Constants.Sh_Flex_Goal;


                    _NewPatientRx.Flex_Current_Start = Constants.Sh_Flex_Current;
                    _NewPatientRx.Flex_Current_End = Constants.Sh_Flex_Goal;
                    _NewPatientRx.Flex_Goal_Start = Constants.Sh_Flex_Current;
                    _NewPatientRx.Flex_Goal_End = Constants.Sh_Flex_Goal;


                    newPatientWithProtocol.NewPatientRXList.Add(_NewPatientRx);


                    _NewPatientRx = new NewPatientRx();
                    _NewPatientRx.Action = "add";
                    _NewPatientRx.TherapyType = EquipmentExcerciselist[1].ExcerciseName;
                    _NewPatientRx.DeviceConfiguration = EquipmentExcerciselist[1].ExcerciseEnum;
                    _NewPatientRx.HeadingFlexion = flexionString1;
                    _NewPatientRx.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                    _NewPatientRx.ProtocolEnum = ExcerciseProtocollist[1].ProtocolEnum;
                    _NewPatientRx.ProtocolName = ExcerciseProtocollist[1].ProtocolName;
                    _NewPatientRx.CurrentFlex = Constants.Sh_ExRot_Current;
                    _NewPatientRx.GoalFlex = Constants.Sh_ExRot_Goal;


                    _NewPatientRx.Flex_Current_Start = Constants.Sh_ExRot_Current;
                    _NewPatientRx.Flex_Current_End = Constants.Sh_ExRot_Goal;
                    _NewPatientRx.Flex_Goal_Start = Constants.Sh_ExRot_Current;
                    _NewPatientRx.Flex_Goal_End = Constants.Sh_ExRot_Goal;
                    newPatientWithProtocol.NewPatientRXList.Add(_NewPatientRx);
                }
                else
                {
                    string flexionString = "Degree of Flexion";
                    string extensionString = "Degree of Extension";
                    if (lExerciseString != null && lExerciseString.Count > 0)
                    {
                        var lexString = lExerciseString.Where(x => x.Limb == _NewPatient.NewPatient.EquipmentType).FirstOrDefault();
                        if (lexString != null)
                        {
                            flexionString = "Degree of " + lexString.FlexionString;
                            extensionString = "Degree of " + lexString.ExtensionString;
                        }
                    }
                    _NewPatientRx = new NewPatientRx();
                    _NewPatientRx.Action = "add";
                    _NewPatientRx.TherapyType = EquipmentExcerciselist[0].ExcerciseName;
                    _NewPatientRx.DeviceConfiguration = EquipmentExcerciselist[0].ExcerciseEnum;
                    _NewPatientRx.HeadingFlexion = flexionString;
                    _NewPatientRx.HeadingExtension = extensionString;
                    _NewPatientRx.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                    _NewPatientRx.ProtocolEnum = ExcerciseProtocollist[0].ProtocolEnum;
                    _NewPatientRx.ProtocolName = ExcerciseProtocollist[0].ProtocolName;
                    if (_NewPatient.NewPatient.EquipmentType.ToLower() == "ankle")
                    {
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
                        _NewPatientRx.CurrentFlex = Constants.Knee_Flex_Current;
                        _NewPatientRx.GoalFlex = Constants.Knee_Flex_Goal;
                        _NewPatientRx.CurrentExten = Constants.Knee_Ext_Current;
                        _NewPatientRx.GoalExten = Constants.Knee_Ext_Goal;

                        _NewPatientRx.Flex_Current_Start = Constants.Knee_Flex_Current_Start;
                        _NewPatientRx.Flex_Current_End = Constants.Knee_Flex_Current_End;
                        _NewPatientRx.Flex_Goal_Start = Constants.Knee_Flex_Goal_Start;
                        _NewPatientRx.Flex_Goal_End = Constants.Knee_Flex_Goal_End;

                        _NewPatientRx.Ext_Current_Start = Constants.Knee_Ext_Current_Start;
                        _NewPatientRx.Ext_Current_End = Constants.Knee_Ext_Current_End;
                        _NewPatientRx.Ext_Goal_Start = Constants.Knee_Ext_Goal_Start;
                        _NewPatientRx.Ext_Goal_End = Constants.Knee_Ext_Goal_End;
                    }
                    newPatientWithProtocol.NewPatientRXList.Add(_NewPatientRx);
                }

                ViewBag.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                ViewBag.SurgeryDate = _NewPatient.NewPatient.SurgeryDate;
            }
            return View(newPatientWithProtocol);
        }




        //if Rx record is found for PatientId then edit mode will open otherwise empty form will open to add a new record
        public IActionResult PatientRXView(string patid = "", string operaton = "")
        {
            NewPatientWithProtocol newPatientWithProtocol = new NewPatientWithProtocol();
            List<EquipmentExcercise> EquipmentExcerciselist = lIEquipmentExerciseRepository.GetEquipmentExcercise();
            List<ExcerciseProtocol> ExcerciseProtocollist = lIEquipmentExerciseRepository.GetExcerciseProtocol();
            newPatientWithProtocol.NewPatientRXList = new List<NewPatientRx>();
            List<PatientRx> PatientRx = null;

            ViewBag.Action = operaton;
            List<EquipmentExercise> lExerciseString = lIEquipmentExerciseRepository.GetEquipmentExerciseString();
            if (!String.IsNullOrEmpty(operaton) && operaton == "edit")
            {
                PatientRx = INewPatient.GetNewPatientRxByPatId(patid);
                if (PatientRx != null && PatientRx.Count > 0)
                {
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
                            _NewPatientRx.Flex_Goal_End = (ldeviceCalibration != null && ldeviceCalibration.Actuator2ExtendedAngle != null) ? Convert.ToInt32(ldeviceCalibration.Actuator2ExtendedAngle) : Constants.Knee_Flex_Goal_End;

                            _NewPatientRx.Ext_Current_Start = Constants.Knee_Ext_Current_Start;
                            _NewPatientRx.Ext_Current_End = Constants.Knee_Ext_Current_End;
                            _NewPatientRx.Ext_Goal_Start = Constants.Knee_Ext_Goal_Start;
                            _NewPatientRx.Ext_Goal_End = (ldeviceCalibration != null && ldeviceCalibration.Actuator2RetractedAngle != null) ? Convert.ToInt32(ldeviceCalibration.Actuator2RetractedAngle) : Constants.Knee_Ext_Goal_End;


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
            }
            else
            {
                NewPatientWithProtocol _NewPatient = new NewPatientWithProtocol();
                if (HttpContext.Session.GetString("NewPatient") != null)
                {
                    _NewPatient = JsonConvert.DeserializeObject<NewPatientWithProtocol>(HttpContext.Session.GetString("NewPatient").ToString());
                }
                else
                {
                    return RedirectToAction("CreatePatient");
                }

                EquipmentExcerciselist = EquipmentExcerciselist.Where(p => p.Limb.ToLower() == _NewPatient.NewPatient.EquipmentType.ToLower()).ToList();

                newPatientWithProtocol.NewPatientRXList = new List<NewPatientRx>();

                ExcerciseProtocollist = ExcerciseProtocollist.Where(p => p.Limb == _NewPatient.NewPatient.EquipmentType).Distinct().ToList();

                NewPatientRx _NewPatientRx = null;

                if (_NewPatient.NewPatient.EquipmentType.ToLower() == "shoulder")
                {
                    string flexionString = "Degree of Forward Flexion";
                    string flexionString1 = "Degree of External Rotation";
                    if (lExerciseString != null && lExerciseString.Count > 0)
                    {
                        var lexString = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "Forward Flexion").FirstOrDefault();
                        if (lexString != null)
                        {
                            flexionString = "Degree of " + lexString.FlexionString;
                        }
                        var lexString1 = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "External Rotation").FirstOrDefault();
                        if (lexString1 != null)
                        {
                            flexionString1 = "Degree of " + lexString.FlexionString;
                        }

                    }
                    _NewPatientRx = new NewPatientRx();
                    _NewPatientRx.Action = "add";
                    _NewPatientRx.TherapyType = EquipmentExcerciselist[0].ExcerciseName;
                    _NewPatientRx.DeviceConfiguration = EquipmentExcerciselist[0].ExcerciseEnum;
                    _NewPatientRx.HeadingFlexion = flexionString;
                    _NewPatientRx.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                    _NewPatientRx.ProtocolEnum = ExcerciseProtocollist[0].ProtocolEnum;
                    _NewPatientRx.ProtocolName = ExcerciseProtocollist[0].ProtocolName;
                    _NewPatientRx.CurrentFlex = Constants.Sh_Flex_Current;
                    _NewPatientRx.GoalFlex = Constants.Sh_Flex_Goal;


                    _NewPatientRx.Flex_Current_Start = Constants.Sh_Flex_Current;
                    _NewPatientRx.Flex_Current_End = Constants.Sh_Flex_Goal;
                    _NewPatientRx.Flex_Goal_Start = Constants.Sh_Flex_Current;
                    _NewPatientRx.Flex_Goal_End = Constants.Sh_Flex_Goal;


                    newPatientWithProtocol.NewPatientRXList.Add(_NewPatientRx);


                    _NewPatientRx = new NewPatientRx();
                    _NewPatientRx.Action = "add";
                    _NewPatientRx.TherapyType = EquipmentExcerciselist[1].ExcerciseName;
                    _NewPatientRx.DeviceConfiguration = EquipmentExcerciselist[1].ExcerciseEnum;
                    _NewPatientRx.HeadingFlexion = flexionString1;
                    _NewPatientRx.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                    _NewPatientRx.ProtocolEnum = ExcerciseProtocollist[1].ProtocolEnum;
                    _NewPatientRx.ProtocolName = ExcerciseProtocollist[1].ProtocolName;
                    _NewPatientRx.CurrentFlex = Constants.Sh_ExRot_Current;
                    _NewPatientRx.GoalFlex = Constants.Sh_ExRot_Goal;


                    _NewPatientRx.Flex_Current_Start = Constants.Sh_ExRot_Current;
                    _NewPatientRx.Flex_Current_End = Constants.Sh_ExRot_Goal;
                    _NewPatientRx.Flex_Goal_Start = Constants.Sh_ExRot_Current;
                    _NewPatientRx.Flex_Goal_End = Constants.Sh_ExRot_Goal;
                    newPatientWithProtocol.NewPatientRXList.Add(_NewPatientRx);
                }
                else
                {
                    string flexionString = "Degree of Flexion";
                    string extensionString = "Degree of Extension";
                    if (lExerciseString != null && lExerciseString.Count > 0)
                    {
                        var lexString = lExerciseString.Where(x => x.Limb == _NewPatient.NewPatient.EquipmentType).FirstOrDefault();
                        if (lexString != null)
                        {
                            flexionString = "Degree of " + lexString.FlexionString;
                            extensionString = "Degree of " + lexString.ExtensionString;
                        }
                    }
                    _NewPatientRx = new NewPatientRx();
                    _NewPatientRx.Action = "add";
                    _NewPatientRx.TherapyType = EquipmentExcerciselist[0].ExcerciseName;
                    _NewPatientRx.DeviceConfiguration = EquipmentExcerciselist[0].ExcerciseEnum;
                    _NewPatientRx.HeadingFlexion = flexionString;
                    _NewPatientRx.HeadingExtension = extensionString;
                    _NewPatientRx.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                    _NewPatientRx.ProtocolEnum = ExcerciseProtocollist[0].ProtocolEnum;
                    _NewPatientRx.ProtocolName = ExcerciseProtocollist[0].ProtocolName;
                    if (_NewPatient.NewPatient.EquipmentType.ToLower() == "ankle")
                    {
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
                        _NewPatientRx.CurrentFlex = Constants.Knee_Flex_Current;
                        _NewPatientRx.GoalFlex = Constants.Knee_Flex_Goal;
                        _NewPatientRx.CurrentExten = Constants.Knee_Ext_Current;
                        _NewPatientRx.GoalExten = Constants.Knee_Ext_Goal;

                        _NewPatientRx.Flex_Current_Start = Constants.Knee_Flex_Current_Start;
                        _NewPatientRx.Flex_Current_End = Constants.Knee_Flex_Current_End;
                        _NewPatientRx.Flex_Goal_Start = Constants.Knee_Flex_Goal_Start;
                        _NewPatientRx.Flex_Goal_End = Constants.Knee_Flex_Goal_End;

                        _NewPatientRx.Ext_Current_Start = Constants.Knee_Ext_Current_Start;
                        _NewPatientRx.Ext_Current_End = Constants.Knee_Ext_Current_End;
                        _NewPatientRx.Ext_Goal_Start = Constants.Knee_Ext_Goal_Start;
                        _NewPatientRx.Ext_Goal_End = Constants.Knee_Ext_Goal_End;
                    }
                    newPatientWithProtocol.NewPatientRXList.Add(_NewPatientRx);
                }

                ViewBag.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                ViewBag.SurgeryDate = _NewPatient.NewPatient.SurgeryDate;
            }
            return View(newPatientWithProtocol);
        }



        //check if the patientRx is available for particular patientId, if not then create new record and edit the existing Rx record
        [HttpPost]
        public IActionResult PatientRX(NewPatientWithProtocol _NewPatientRx)
        {
            JsonSerializerSettings lsetting = new JsonSerializerSettings();
            lsetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            if (_NewPatientRx.NewPatientRXList[0].Action == "edit")
            {
                //getting Rx record using PatientId
                List<PatientRx> PatientRx = INewPatient.GetNewPatientRxByPatId(_NewPatientRx.NewPatientRXList[0].PatientId.ToString());
                //inserting user activity log for existing record
                UserActivityLog llog1 = new UserActivityLog();
                llog1.SessionId = HttpContext.Session.GetString("SessionId");
                llog1.ActivityType = "Update";
                llog1.StartTimeStamp = !string.IsNullOrEmpty(HttpContext.Session.GetString("SessionTime")) ? Convert.ToDateTime(HttpContext.Session.GetString("SessionTime")) : DateTime.Now;
                llog1.Duration = Convert.ToInt32((DateTime.Now - Convert.ToDateTime(HttpContext.Session.GetString("SessionTime"))).TotalSeconds);
                llog1.RecordChangeType = "Edit";
                llog1.RecordType = "Rx";
                llog1.Comment = "Record edited";
                llog1.UserId = HttpContext.Session.GetString("UserId");
                llog1.UserName = HttpContext.Session.GetString("UserName");
                llog1.UserType = HttpContext.Session.GetString("UserType");
                llog1.RecordExistingJson = JsonConvert.SerializeObject(PatientRx, lsetting);
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ReviewID")))
                {
                    llog1.ReviewId = HttpContext.Session.GetString("ReviewID");
                }
                lIUserActivityLogRepository.InsertUserActivityLog(llog1);
                //updating patient Rx record 
                List<PatientRx> _result = INewPatient.UpdatePatientRx(_NewPatientRx.NewPatientRXList, _NewPatientRx.PainThreshold, _NewPatientRx.RateOfChange, HttpContext.Session.GetString("UserType"));
                if (_result != null && _result.Count > 0 && llog1.ActivityId > 0)
                {
                    //Insert to User Activity Log
                    UserActivityLog llog = lIUserActivityLogRepository.GetUserActivityLog(llog1.ActivityId);
                    if (llog == null)
                        llog = new UserActivityLog();
                    llog.SessionId = HttpContext.Session.GetString("SessionId");
                    llog.ActivityType = "Update";
                    llog.StartTimeStamp = !string.IsNullOrEmpty(HttpContext.Session.GetString("SessionTime")) ? Convert.ToDateTime(HttpContext.Session.GetString("SessionTime")) : DateTime.Now;
                    llog.Duration = Convert.ToInt32((DateTime.Now - Convert.ToDateTime(HttpContext.Session.GetString("SessionTime"))).TotalSeconds);
                    llog.RecordChangeType = "Edit";
                    llog.RecordType = "Rx";
                    llog.Comment = "Record edited";
                    llog.UserId = HttpContext.Session.GetString("UserId");
                    llog.UserName = HttpContext.Session.GetString("UserName");
                    llog.UserType = HttpContext.Session.GetString("UserType");
                    llog.RecordJson = JsonConvert.SerializeObject(_result, lsetting);
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ReviewID")))
                    {
                        llog.ReviewId = HttpContext.Session.GetString("ReviewID");
                    }
                    if (llog.ActivityId != 0)
                        //updating User Activity log for the same Activity id
                        lIUserActivityLogRepository.UpdateUserActivityLog(llog);
                    else
                        lIUserActivityLogRepository.InsertUserActivityLog(llog);

                    if (!string.IsNullOrEmpty(_NewPatientRx.returnView))
                    {
                        return RedirectToAction("Index", "Review", new { id = PatientRx[0].Patient.PatientId, Username = PatientRx[0].Patient.PatientName, EquipmentType = PatientRx[0].EquipmentType, actuator = PatientRx[0].DeviceConfiguration, tab = "Rx" });
                    }
                    else
                    {
                        if (HttpContext.Session.GetString("UserType") == ConstantsVar.Provider.ToString())
                            return RedirectToAction("Dashboard", "Provider", new { id = _NewPatientRx.ProviderId });
                        else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Admin.ToString())
                            return RedirectToAction("Index", "Patient");
                        else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Therapist.ToString())
                            return RedirectToAction("Dashboard", "Therapist", new { id = HttpContext.Session.GetString("UserId") });
                        else if (HttpContext.Session.GetString("UserType") == ConstantsVar.PatientAdministrator.ToString())
                            return RedirectToAction("Dashboard", "PatientAdministrator", new { id = HttpContext.Session.GetString("UserId") });
                        else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Support.ToString())
                            return RedirectToAction("Dashboard", "Support", new { id = HttpContext.Session.GetString("UserId") });
                    }
                }
            }
            else
            {
                NewPatientWithProtocol _NewPatient = new NewPatientWithProtocol();
                if (HttpContext.Session.GetString("NewPatient") != null)
                {
                    _NewPatient = JsonConvert.DeserializeObject<NewPatientWithProtocol>(HttpContext.Session.GetString("NewPatient").ToString());
                }
                _NewPatientRx.NewPatient = new NewPatient();
                _NewPatientRx.NewPatient = _NewPatient.NewPatient;

                _NewPatient.NewPatientRX = new NewPatientRx();
                _NewPatientRx.NewPatientRX = _NewPatient.NewPatientRX;

                if (HttpContext.Session.GetString("UserId") != null)
                {
                    //prabhu
                    if (HttpContext.Session.GetString("UserType") == ConstantsVar.Provider.ToString())
                        _NewPatientRx.ProviderId = HttpContext.Session.GetString("UserId").ToString();
                    else
                        _NewPatientRx.ProviderId = _NewPatient.NewPatient.ProviderId;

                    List<PatientRx> _result = INewPatient.CreateNewPatientByProvider(_NewPatientRx);
                    if (_result != null && _result.Count > 0)
                    {
                        //Prabhu
                        Patient ppatient = context.Patient.FirstOrDefault(x => x.PatientId == _result[0].PatientId);
                        User pUser = lIUserRepository.getUser(_NewPatient.NewPatient.PatientLoginId);
                        if (pUser == null)
                        {

                            if (ppatient != null)
                            {
                                User lpatient = new Models.User();
                                lpatient.UserId = ppatient.PatientLoginId;
                                lpatient.Name = _NewPatient.NewPatient.PatientName;
                                lpatient.Password = "";
                                lpatient.Type = ConstantsVar.Patient;
                                lpatient.Email = "";
                                lpatient.Address = _NewPatient.NewPatient.AddressLine + " " + _NewPatient.NewPatient.City + " " + _NewPatient.NewPatient.State + " " + _NewPatient.NewPatient.Zip;
                                lpatient.Phone = _NewPatient.NewPatient.PhoneNumber;
                                lIUserRepository.InsertUser(lpatient);
                            }
                        }

                        //Insert to User Activity Log -Patient
                        UserActivityLog llog1 = new UserActivityLog();
                        llog1.SessionId = HttpContext.Session.GetString("SessionId");
                        llog1.ActivityType = "Update";
                        llog1.StartTimeStamp = !string.IsNullOrEmpty(HttpContext.Session.GetString("SessionTime")) ? Convert.ToDateTime(HttpContext.Session.GetString("SessionTime")) : DateTime.Now;
                        llog1.Duration = Convert.ToInt32((DateTime.Now - Convert.ToDateTime(HttpContext.Session.GetString("SessionTime"))).TotalSeconds);
                        llog1.RecordChangeType = "Add";
                        llog1.RecordType = "Patient";
                        llog1.Comment = "Record added";

                        llog1.RecordJson = JsonConvert.SerializeObject(ppatient, lsetting);
                        llog1.UserId = HttpContext.Session.GetString("UserId");
                        llog1.UserName = HttpContext.Session.GetString("UserName");
                        llog1.UserType = HttpContext.Session.GetString("UserType");
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ReviewID")))
                        {
                            llog1.ReviewId = HttpContext.Session.GetString("ReviewID");
                        }
                        lIUserActivityLogRepository.InsertUserActivityLog(llog1);
                        //Insert to User Activity Log -Patrx
                        UserActivityLog llog = new UserActivityLog();
                        llog.SessionId = HttpContext.Session.GetString("SessionId");
                        llog.ActivityType = "Update";
                        llog.StartTimeStamp = !string.IsNullOrEmpty(HttpContext.Session.GetString("SessionTime")) ? Convert.ToDateTime(HttpContext.Session.GetString("SessionTime")) : DateTime.Now;
                        llog.Duration = Convert.ToInt32((DateTime.Now - Convert.ToDateTime(HttpContext.Session.GetString("SessionTime"))).TotalSeconds);
                        llog.RecordChangeType = "Add";
                        llog.RecordType = "Rx";
                        llog.Comment = "Record added";
                        llog.RecordJson = JsonConvert.SerializeObject(_result, lsetting);
                        llog.UserId = HttpContext.Session.GetString("UserId");
                        llog.UserName = HttpContext.Session.GetString("UserName");
                        llog.UserType = HttpContext.Session.GetString("UserType");
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ReviewID")))
                        {
                            llog.ReviewId = HttpContext.Session.GetString("ReviewID");
                        }
                        lIUserActivityLogRepository.InsertUserActivityLog(llog);

                        HttpContext.Session.SetString("NewPatient", "");

                        if (HttpContext.Session.GetString("UserType") == ConstantsVar.Provider.ToString())
                            return RedirectToAction("Dashboard", "Provider", new { id = _NewPatientRx.ProviderId });
                        else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Admin.ToString())
                            return RedirectToAction("Index", "Patient");
                        else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Therapist.ToString())
                            return RedirectToAction("Dashboard", "Therapist", new { id = HttpContext.Session.GetString("UserId") });
                        else if (HttpContext.Session.GetString("UserType") == ConstantsVar.PatientAdministrator.ToString())
                            return RedirectToAction("Dashboard", "PatientAdministrator", new { id = HttpContext.Session.GetString("UserId") });
                        else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Support.ToString())
                            return RedirectToAction("Dashboard", "Support", new { id = HttpContext.Session.GetString("UserId") });
                    }
                }
            }

            return RedirectToAction("CreatePatient");
        }
        public IActionResult ProtocolList(string patId, string eType, string Username = "")
        {
            ViewBag.PatientName = Username;
            List<NewProtocol> _result = INewPatient.GetProtocolListBypatId(patId);
            ViewBag.VisibleAddButton = false;
            ViewBag.VisibleAddButtonExt = false;
            if (!string.IsNullOrEmpty(eType))
            {
                if (eType.ToLower() == "shoulder")
                {
                    PatientConfiguration lconfig = lIPatientConfigurationRepository.getPatientConfigurationbyPatientId(Convert.ToInt32(patId), eType, "Forward Flexion");
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
                    PatientConfiguration lconfigext = lIPatientConfigurationRepository.getPatientConfigurationbyPatientId(Convert.ToInt32(patId), eType, "External Rotation");
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
                    PatientConfiguration lconfig = lIPatientConfigurationRepository.getPatientConfigurationbyPatientId(Convert.ToInt32(patId), eType);

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

                }
            }


            ViewBag.ProtocolList = _result;
            ViewBag.etype = eType;

            if (String.IsNullOrEmpty(Username))
                ViewBag.PatientName = Username;
            if (_result != null && _result.Count > 0 && String.IsNullOrEmpty(Username))
            {

                ViewBag.PatientName = _result[0].PatientName;
            }

            return View(_result);
        }


        public IActionResult Protocol(string protocolid = "", string protocolName = "", string ExeName = "", string RXID = "", string returnView = "")
        {
            NewProtocol _protocol = new NewProtocol();
            PatientRx PatientRx = null;
            DeviceCalibration lDeviceCalibration = null;
            ViewBag.Configuration = protocolName;

            ViewBag.ProtocolName = protocolName;
            //getting the list of equipment of exercises
            List<EquipmentExcercise> EquipmentExcercise = lIEquipmentExerciseRepository.GetEquipmentExcercise();
            //getting the list of exercises
            List<ExcerciseProtocol> ExcerciseProtocol = lIEquipmentExerciseRepository.GetExcerciseProtocol();
            if (String.IsNullOrEmpty(protocolid) && !String.IsNullOrEmpty(RXID))
            {
                //getting Patient's Rx record from PatientRx and Patient table using Rx id
                PatientRx = INewPatient.GetNewPatientRxByRxId(RXID);
                if (PatientRx != null)
                {       //getting DeviceCalibration record using Rx Id
                    lDeviceCalibration = lIDeviceCalibrationRepository.getDeviceCalibrationByRxID(RXID);
                    ViewBag.PatientName = PatientRx.Patient.PatientName;
                    _protocol.PatientName = PatientRx.Patient.PatientName;
                    _protocol.EquipmentType = PatientRx.EquipmentType;
                    ViewBag.EType = PatientRx.EquipmentType;
                    //getting equipment exercise from EquipmentExercise list using EquipmentType and DeviceConfiguration
                    EquipmentExcercise _EquipmentExcercise = EquipmentExcercise.Where(p => p.Limb.ToLower() == PatientRx.EquipmentType.ToLower() && p.ExcerciseEnum == PatientRx.DeviceConfiguration).FirstOrDefault();
                    _protocol.ExcerciseName = _EquipmentExcercise.ExcerciseName;
                    _protocol.ExcerciseEnum = _EquipmentExcercise.ExcerciseEnum;
                    ViewBag.Configuration = _EquipmentExcercise.ExcerciseName;
                    ViewBag.ProtocolName = _EquipmentExcercise.ExcerciseName;
                    //getting exercise from ExerciseProtocol list using EquipmentType and DeviceConfiguration
                    ExcerciseProtocol _ExcerciseProtocol = ExcerciseProtocol.Where(p => p.Limb.ToLower() == PatientRx.EquipmentType.ToLower() && p.ExcerciseEnum == PatientRx.DeviceConfiguration).FirstOrDefault();
                    ViewBag.Exercise = _ExcerciseProtocol.ProtocolName;
                    _protocol.ProtocolName = _ExcerciseProtocol.ProtocolName;
                    _protocol.LaymanName = _ExcerciseProtocol.ProtocolName;
                    _protocol.ProtocolEnum = _ExcerciseProtocol.ProtocolEnum;
                    _protocol.PatientId = PatientRx.PatientId;
                    _protocol.StartDate = PatientRx.RxStartDate;
                    _protocol.EndDate = PatientRx.RxEndDate;

                    _protocol.RateOfChange = PatientRx.RateOfChange;

                    _protocol.SurgeryDate = PatientRx.RxStartDate;
                    _protocol.RxEndDate = PatientRx.RxEndDate;

                    _protocol.RestAt = 0;
                    _protocol.RepsAt = 0;
                    _protocol.Speed = 0;
                    //getting Exercise list using EquipmentType and DeviceConfiguration
                    List<ExcerciseProtocol> prolist = ExcerciseProtocol.Where(p => p.Limb.ToLower() == PatientRx.EquipmentType.ToLower() && p.ExcerciseEnum == PatientRx.DeviceConfiguration).ToList();
                    List<SelectListItem> list = new List<SelectListItem>();
                    foreach (ExcerciseProtocol ex in prolist)
                    {
                        //adding the exercises to the list for dropdown
                        list.Add(new SelectListItem { Text = ex.ProtocolName.ToString(), Value = ex.ProtocolEnum.ToString() });
                    }
                    //storing the list of exercises in viewbag
                    ViewBag.Protocol = new SelectList(list, "Value", "Text");
                }
            }

            NewPatientWithProtocol _NewPatient = new NewPatientWithProtocol();


            if (!String.IsNullOrEmpty(protocolid))//when protocol id is not null, edit
            {
                //getting protocol using protocol id
                _protocol = INewPatient.GetProtocolByproId(protocolid);
                ViewBag.RxId = _protocol.RxId;
                ViewBag.RateOfChange = INewPatient.GetNewPatientRxByRxId(ViewBag.RxId).RateOfChange;
                ViewBag.Configuration = getExcercise(_protocol.EquipmentType, _protocol.ExcerciseEnum);
                ViewBag.ProtocolName = getExcercise(_protocol.EquipmentType, _protocol.ExcerciseEnum);
                ViewBag.Excercise = _protocol.ExcerciseName;
                ViewBag.Action = "edit";
                ViewBag.EType = _protocol.EquipmentType;
                ViewBag.PatientName = _protocol.PatientName;

            }
            List<EquipmentExercise> lExerciseString = lIEquipmentExerciseRepository.GetEquipmentExerciseString();
            if (_protocol.EquipmentType.ToLower() == "shoulder")
            {
                if (PatientRx != null)
                {
                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;
                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);
                    ViewBag.RateOfChange = PatientRx.RateOfChange;
                }
                if (_protocol.ExcerciseEnum == "Forward Flexion")
                {
                    if (lExerciseString != null && lExerciseString.Count > 0)
                    {
                        var lexString = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "Forward Flexion").FirstOrDefault();
                        if (lexString != null)
                        {
                            ViewBag.flexionString = lexString.FlexionString;
                        }
                    }
                    _protocol.CurrentFlex = Constants.Sh_Flex_Current;
                    _protocol.GoalFlex = Constants.Sh_Flex_Goal;
                    if (String.IsNullOrEmpty(protocolid))//when protocol id is not null, edit
                    {
                        _protocol.Plane = PlaneProperty.FrontalAngle;
                    }

                    _protocol.Flex_Current_Start = Constants.Sh_Flex_Current;
                    _protocol.Flex_Current_End = Constants.Sh_Flex_Goal;
                    _protocol.Flex_Goal_Start = Constants.Sh_Flex_Current;
                    _protocol.Flex_Goal_End = Constants.Sh_Flex_Goal;
                }

                if (_protocol.ExcerciseEnum == "External Rotation")
                {
                    if (lExerciseString != null && lExerciseString.Count > 0)
                    {
                        var lexString = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "External Rotation").FirstOrDefault();
                        if (lexString != null)
                        {
                            ViewBag.flexionString = lexString.FlexionString;
                        }
                    }
                    _protocol.CurrentFlex = Constants.Sh_ExRot_Current;
                    _protocol.GoalFlex = Constants.Sh_ExRot_Goal;

                    _protocol.Flex_Current_Start = Constants.Sh_ExRot_Current;
                    _protocol.Flex_Current_End = Constants.Sh_ExRot_Goal;
                    _protocol.Flex_Goal_Start = Constants.Sh_ExRot_Current;
                    _protocol.Flex_Goal_End = Constants.Sh_ExRot_Goal;
                }
                _protocol.RateOfChange = Convert.ToInt32(ViewBag.RateOfChange);
            }
            else if (_protocol.EquipmentType.ToLower() == "ankle")
            {
                if (lExerciseString != null && lExerciseString.Count > 0)
                {
                    var lexString = lExerciseString.Where(x => x.Limb == "Ankle").FirstOrDefault();
                    if (lexString != null)
                    {
                        ViewBag.flexionString = lexString.FlexionString;
                        ViewBag.extensionString = lexString.ExtensionString;
                    }
                }
                if (PatientRx != null)
                {
                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;
                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);

                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;
                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);
                    ViewBag.RateOfChange = PatientRx.RateOfChange;
                }
                _protocol.CurrentFlex = Constants.Ankle_Flex_Current;
                _protocol.GoalFlex = Constants.Ankle_Flex_Goal;
                _protocol.CurrentExten = Constants.Ankle_Ext_Current;
                _protocol.GoalExten = Constants.Ankle_Ext_Goal;


                _protocol.Flex_Current_Start = Constants.Ankle_Flex_Current;
                _protocol.Flex_Current_End = Constants.Ankle_Flex_Goal;
                _protocol.Flex_Goal_Start = Constants.Ankle_Flex_Current;
                _protocol.Flex_Goal_End = Constants.Ankle_Flex_Goal;

                _protocol.Ext_Current_Start = Constants.Ankle_Ext_Current;
                _protocol.Ext_Current_End = Constants.Ankle_Ext_Goal;
                _protocol.Ext_Goal_Start = Constants.Ankle_Ext_Current;
                _protocol.Ext_Goal_End = Constants.Ankle_Ext_Goal;
                _protocol.RateOfChange = Convert.ToInt32(ViewBag.RateOfChange);
            }
            else
            {
                if (lExerciseString != null && lExerciseString.Count > 0)
                {
                    var lexString = lExerciseString.Where(x => x.Limb == "Knee").FirstOrDefault();
                    if (lexString != null)
                    {
                        ViewBag.flexionString = lexString.FlexionString;
                        ViewBag.extensionString = lexString.ExtensionString;
                    }
                }
                if (PatientRx != null)
                {
                    //storing Rx record of patient in _protocol 
                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;
                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);


                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;
                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);

                    ViewBag.RateOfChange = PatientRx.RateOfChange;
                }
                //getting and storing all range values, defined for degree of flexion and extension
                _protocol.CurrentFlex = Constants.Knee_Flex_Current;
                _protocol.GoalFlex = Constants.Knee_Flex_Goal;
                _protocol.CurrentExten = Constants.Knee_Ext_Current;
                _protocol.GoalExten = Constants.Knee_Ext_Goal;

                _protocol.Flex_Current_Start = Constants.Knee_Flex_Current_Start;
                _protocol.Flex_Current_End = Constants.Knee_Flex_Current_End;
                _protocol.Flex_Goal_Start = Constants.Knee_Flex_Goal_Start;
                _protocol.Flex_Goal_End = (lDeviceCalibration != null && lDeviceCalibration.Actuator2ExtendedAngle != null) ? Convert.ToInt32(lDeviceCalibration.Actuator2ExtendedAngle) : Constants.Knee_Flex_Goal_End;

                _protocol.Ext_Current_Start = Constants.Knee_Ext_Current_Start;
                _protocol.Ext_Current_End = Constants.Knee_Ext_Current_End;
                _protocol.Ext_Goal_Start = Constants.Knee_Ext_Goal_Start;
                _protocol.Ext_Goal_End = (lDeviceCalibration != null && lDeviceCalibration.Actuator2RetractedAngle != null) ? Convert.ToInt32(lDeviceCalibration.Actuator2RetractedAngle) : Constants.Knee_Ext_Goal_End;
                _protocol.RateOfChange = Convert.ToInt32(ViewBag.RateOfChange);
            }

            if (!string.IsNullOrEmpty(returnView))
            {
                _protocol.returnView = returnView;
            }
            return View(_protocol);
        }

        public IActionResult ViewProtocol(string protocolid = "", string protocolName = "", string ExeName = "", string RXID = "", string returnView = "")
        {
            NewProtocol _protocol = new NewProtocol();
            PatientRx PatientRx = null;
            DeviceCalibration lDeviceCalibration = null;
            ViewBag.Configuration = protocolName;

            ViewBag.ProtocolName = protocolName;
            List<EquipmentExcercise> EquipmentExcercise = lIEquipmentExerciseRepository.GetEquipmentExcercise();
            List<ExcerciseProtocol> ExcerciseProtocol = lIEquipmentExerciseRepository.GetExcerciseProtocol();
            if (String.IsNullOrEmpty(protocolid) && !String.IsNullOrEmpty(RXID))
            {

                PatientRx = INewPatient.GetNewPatientRxByRxId(RXID);
                if (PatientRx != null)
                {
                    lDeviceCalibration = lIDeviceCalibrationRepository.getDeviceCalibrationByRxID(RXID);
                    ViewBag.PatientName = PatientRx.Patient.PatientName;
                    _protocol.EquipmentType = PatientRx.EquipmentType;
                    ViewBag.EType = PatientRx.EquipmentType;
                    EquipmentExcercise _EquipmentExcercise = EquipmentExcercise.Where(p => p.Limb.ToLower() == PatientRx.EquipmentType.ToLower() && p.ExcerciseEnum == PatientRx.DeviceConfiguration).FirstOrDefault();
                    _protocol.ExcerciseName = _EquipmentExcercise.ExcerciseName;
                    _protocol.ExcerciseEnum = _EquipmentExcercise.ExcerciseEnum;
                    ViewBag.Configuration = _EquipmentExcercise.ExcerciseName;
                    ViewBag.ProtocolName = _EquipmentExcercise.ExcerciseName;

                    ExcerciseProtocol _ExcerciseProtocol = ExcerciseProtocol.Where(p => p.Limb.ToLower() == PatientRx.EquipmentType.ToLower() && p.ExcerciseEnum == PatientRx.DeviceConfiguration).FirstOrDefault();
                    ViewBag.Exercise = _ExcerciseProtocol.ProtocolName;
                    _protocol.ProtocolName = _ExcerciseProtocol.ProtocolName;
                    _protocol.LaymanName = _ExcerciseProtocol.ProtocolName;
                    _protocol.ProtocolEnum = _ExcerciseProtocol.ProtocolEnum;
                    _protocol.PatientId = PatientRx.PatientId;
                    _protocol.StartDate = PatientRx.RxStartDate;
                    _protocol.EndDate = PatientRx.RxEndDate;

                    _protocol.RateOfChange = PatientRx.RateOfChange;

                    _protocol.SurgeryDate = PatientRx.RxStartDate;
                    _protocol.RxEndDate = PatientRx.RxEndDate;

                    _protocol.RestAt = 0;
                    _protocol.RepsAt = 0;
                    _protocol.Speed = 0;

                    List<ExcerciseProtocol> prolist = ExcerciseProtocol.Where(p => p.Limb.ToLower() == PatientRx.EquipmentType.ToLower() && p.ExcerciseEnum == PatientRx.DeviceConfiguration).ToList();
                    List<SelectListItem> list = new List<SelectListItem>();
                    foreach (ExcerciseProtocol ex in prolist)
                    {
                        list.Add(new SelectListItem { Text = ex.ProtocolName.ToString(), Value = ex.ProtocolEnum.ToString() });
                    }
                    ViewBag.Protocol = new SelectList(list, "Value", "Text");
                }
            }

            NewPatientWithProtocol _NewPatient = new NewPatientWithProtocol();

            List<EquipmentExercise> lExerciseString = lIEquipmentExerciseRepository.GetEquipmentExerciseString();
            if (!String.IsNullOrEmpty(protocolid))
            {
                _protocol = INewPatient.GetProtocolByproId(protocolid);
                ViewBag.Configuration = getExcercise(_protocol.EquipmentType, _protocol.ExcerciseEnum);
                ViewBag.ProtocolName = getExcercise(_protocol.EquipmentType, _protocol.ExcerciseEnum);
                ViewBag.Excercise = _protocol.ExcerciseName;
                ViewBag.Action = "edit";
                ViewBag.EType = _protocol.EquipmentType;
                ViewBag.PatientName = _protocol.PatientName;

            }
            if (_protocol.EquipmentType.ToLower() == "shoulder")
            {
                if (PatientRx != null)
                {
                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;
                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);

                }
                if (_protocol.ExcerciseEnum == "Forward Flexion")
                {
                    if (lExerciseString != null && lExerciseString.Count > 0)
                    {
                        var lexString = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "Forward Flexion").FirstOrDefault();
                        if (lexString != null)
                        {
                            ViewBag.flexionString = lexString.FlexionString;
                        }
                    }
                    _protocol.CurrentFlex = Constants.Sh_Flex_Current;
                    _protocol.GoalFlex = Constants.Sh_Flex_Goal;
                    if (String.IsNullOrEmpty(protocolid))//when protocol id is not null, edit
                    {
                        _protocol.Plane = PlaneProperty.FrontalAngle;
                    }

                    _protocol.Flex_Current_Start = Constants.Sh_Flex_Current;
                    _protocol.Flex_Current_End = Constants.Sh_Flex_Goal;
                    _protocol.Flex_Goal_Start = Constants.Sh_Flex_Current;
                    _protocol.Flex_Goal_End = Constants.Sh_Flex_Goal;
                }

                if (_protocol.ExcerciseEnum == "External Rotation")
                {
                    if (lExerciseString != null && lExerciseString.Count > 0)
                    {
                        var lexString = lExerciseString.Where(x => x.Limb == "Shoulder" && x.ExerciseEnum == "External Rotation").FirstOrDefault();
                        if (lexString != null)
                        {
                            ViewBag.flexionString = lexString.FlexionString;
                        }
                    }
                    _protocol.CurrentFlex = Constants.Sh_ExRot_Current;
                    _protocol.GoalFlex = Constants.Sh_ExRot_Goal;

                    _protocol.Flex_Current_Start = Constants.Sh_ExRot_Current;
                    _protocol.Flex_Current_End = Constants.Sh_ExRot_Goal;
                    _protocol.Flex_Goal_Start = Constants.Sh_ExRot_Current;
                    _protocol.Flex_Goal_End = Constants.Sh_ExRot_Goal;
                }
            }
            else if (_protocol.EquipmentType.ToLower() == "ankle")
            {
                if (lExerciseString != null && lExerciseString.Count > 0)
                {
                    var lexString = lExerciseString.Where(x => x.Limb == "Knee").FirstOrDefault();
                    if (lexString != null)
                    {
                        ViewBag.flexionString = lexString.FlexionString;
                        ViewBag.extensionString = lexString.ExtensionString;
                    }
                }
                if (PatientRx != null)
                {
                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;
                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);

                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;
                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);

                }
                _protocol.CurrentFlex = Constants.Ankle_Flex_Current;
                _protocol.GoalFlex = Constants.Ankle_Flex_Goal;
                _protocol.CurrentExten = Constants.Ankle_Ext_Current;
                _protocol.GoalExten = Constants.Ankle_Ext_Goal;


                _protocol.Flex_Current_Start = Constants.Ankle_Flex_Current;
                _protocol.Flex_Current_End = Constants.Ankle_Flex_Goal;
                _protocol.Flex_Goal_Start = Constants.Ankle_Flex_Current;
                _protocol.Flex_Goal_End = Constants.Ankle_Flex_Goal;

                _protocol.Ext_Current_Start = Constants.Ankle_Ext_Current;
                _protocol.Ext_Current_End = Constants.Ankle_Ext_Goal;
                _protocol.Ext_Goal_Start = Constants.Ankle_Ext_Current;
                _protocol.Ext_Goal_End = Constants.Ankle_Ext_Goal;
            }
            else
            {
                if (lExerciseString != null && lExerciseString.Count > 0)
                {
                    var lexString = lExerciseString.Where(x => x.Limb == "Knee").FirstOrDefault();
                    if (lexString != null)
                    {
                        ViewBag.flexionString = lexString.FlexionString;
                        ViewBag.extensionString = lexString.ExtensionString;
                    }
                }
                if (PatientRx != null)
                {
                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;
                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);

                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;
                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);

                }
                _protocol.CurrentFlex = Constants.Knee_Flex_Current;
                _protocol.GoalFlex = Constants.Knee_Flex_Goal;
                _protocol.CurrentExten = Constants.Knee_Ext_Current;
                _protocol.GoalExten = Constants.Knee_Ext_Goal;

                _protocol.Flex_Current_Start = Constants.Knee_Flex_Current_Start;
                _protocol.Flex_Current_End = Constants.Knee_Flex_Current_End;
                _protocol.Flex_Goal_Start = Constants.Knee_Flex_Goal_Start;
                _protocol.Flex_Goal_End = (lDeviceCalibration != null && lDeviceCalibration.Actuator2ExtendedAngle != null) ? Convert.ToInt32(lDeviceCalibration.Actuator2ExtendedAngle) : Constants.Knee_Flex_Goal_End;

                _protocol.Ext_Current_Start = Constants.Knee_Ext_Current_Start;
                _protocol.Ext_Current_End = Constants.Knee_Ext_Current_End;
                _protocol.Ext_Goal_Start = Constants.Knee_Ext_Goal_Start;
                _protocol.Ext_Goal_End = (lDeviceCalibration != null && lDeviceCalibration.Actuator2RetractedAngle != null) ? Convert.ToInt32(lDeviceCalibration.Actuator2RetractedAngle) : Constants.Knee_Ext_Goal_End;

            }
            if (!string.IsNullOrEmpty(returnView))
            {
                _protocol.returnView = returnView;
            }
            return View(_protocol);
        }


        private string getProtocolType(string limb, int? proenum, string exenum)
        {
            List<ExcerciseProtocol> ExcerciseProtocollist = lIEquipmentExerciseRepository.GetExcerciseProtocol();
            ExcerciseProtocol _EquipmentExcercise = ExcerciseProtocollist.Where(p => p.Limb.ToLower() == limb.ToLower() && p.ProtocolEnum == Convert.ToInt32(proenum) && p.ExcerciseEnum == exenum).FirstOrDefault();
            return _EquipmentExcercise.ProtocolName;
        }
        private string getExcercise(string limb, string exenum)
        {
            List<EquipmentExcercise> EquipmentExcercise = lIEquipmentExerciseRepository.GetEquipmentExcercise();
            EquipmentExcercise _EquipmentExcercise = EquipmentExcercise.Where(p => p.Limb.ToLower() == limb.ToLower() && p.ExcerciseEnum == exenum).FirstOrDefault();
            return _EquipmentExcercise.ExcerciseName;
        }

        [HttpPost]
        public IActionResult Protocol(NewProtocol NewProtocol)
        {

            //Insert to User Activity Log
            UserActivityLog llog = new UserActivityLog();
            llog.SessionId = HttpContext.Session.GetString("SessionId");
            llog.ActivityType = "Update";
            llog.StartTimeStamp = !string.IsNullOrEmpty(HttpContext.Session.GetString("SessionTime")) ? Convert.ToDateTime(HttpContext.Session.GetString("SessionTime")) : DateTime.Now;
            llog.Duration = Convert.ToInt32((DateTime.Now - Convert.ToDateTime(HttpContext.Session.GetString("SessionTime"))).TotalSeconds);
            llog.RecordChangeType = !string.IsNullOrEmpty(NewProtocol.ProtocolId) ? "Edit" : "Add";
            llog.Comment = !string.IsNullOrEmpty(NewProtocol.ProtocolId) ? "Record edited" : "Record Added";
            llog.RecordType = "Exercise";
            llog.UserId = HttpContext.Session.GetString("UserId");
            llog.UserName = HttpContext.Session.GetString("UserName");
            llog.UserType = HttpContext.Session.GetString("UserType");
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ReviewID")))
            {
                llog.ReviewId = HttpContext.Session.GetString("ReviewID");
            }
            if (NewProtocol.EquipmentType.ToLower() == "shoulder" && NewProtocol.ExcerciseEnum == "Forward Flexion" && (NewProtocol.Plane == null || (NewProtocol.Plane != null && NewProtocol.Plane == 0)))
            {
                NewProtocol.Plane = PlaneProperty.FrontalAngle;
            }

            if (!string.IsNullOrEmpty(NewProtocol.ProtocolId))
            {
                Protocol pro = context.Protocol.FirstOrDefault(x => x.ProtocolId == NewProtocol.ProtocolId);
                llog.RecordExistingJson = JsonConvert.SerializeObject(pro);
                lIUserActivityLogRepository.InsertUserActivityLog(llog);
            }


            //creating protocol record in Protocol table, protocol id will generate using GUID
            Protocol _result = INewPatient.CreateProtocol(NewProtocol);
            if (_result != null)
            {
                if (llog.ActivityId > 0)
                {
                    llog.RecordJson = JsonConvert.SerializeObject(_result);
                    lIUserActivityLogRepository.UpdateUserActivityLog(llog);
                }
                else
                {
                    //when new record generated, save it in RecordJson parameter of "UserActivityLog"
                    llog.RecordJson = JsonConvert.SerializeObject(_result);
                    //insert the activity in UserActivityLog
                    lIUserActivityLogRepository.InsertUserActivityLog(llog);
                }
                if (!string.IsNullOrEmpty(NewProtocol.returnView))
                {
                    return RedirectToAction("Index", "Review", new { id = NewProtocol.PatientId, Username = NewProtocol.PatientName, EquipmentType = NewProtocol.EquipmentType, actuator = NewProtocol.ExcerciseEnum, tab = "Exercises" });
                }
                else
                {
                    return RedirectToAction("ProtocolList", new { patId = NewProtocol.PatientId, eType = NewProtocol.EquipmentType });
                }
            }
            if (!string.IsNullOrEmpty(NewProtocol.returnView))
            {
                return RedirectToAction("Index", "Review", new { id = NewProtocol.PatientId, Username = NewProtocol.PatientName, EquipmentType = NewProtocol.EquipmentType, actuator = NewProtocol.ExcerciseEnum, tab = "Exercises" });
            }
            else
            {
                return RedirectToAction("CreatePatient");
            }
        }


        //delete the exercise record.
        public IActionResult Delete(string proId, string patId, string patName, string eType, string returnView = "")
        {
            //deviceconfiguration set to empty
            string deviceConfiguration = string.Empty;
            try
            {
                //getting the protocol
                List<Protocol> lprotocol = context.Protocol.Where(x => x.ProtocolId == proId).ToList();
                if (lprotocol.Count > 0)
                {
                    //get the device configuration
                    deviceConfiguration = lprotocol[0].DeviceConfiguration;
                }
                //using the protocol id remove pain,session and protocol records from pain, sessions and protocol tables respectively
                string result = INewPatient.DeleteProtocolRecordsWithCasecade(proId);
                if (!string.IsNullOrEmpty(result) && result == "success" && lprotocol != null && lprotocol.Count > 0)
                {

                    //Insert to User Activity Log
                    UserActivityLog llog = new UserActivityLog();
                    llog.SessionId = HttpContext.Session.GetString("SessionId");
                    llog.ActivityType = "Update";
                    llog.StartTimeStamp = !string.IsNullOrEmpty(HttpContext.Session.GetString("SessionTime")) ? Convert.ToDateTime(HttpContext.Session.GetString("SessionTime")) : DateTime.Now;
                    llog.Duration = Convert.ToInt32((DateTime.Now - Convert.ToDateTime(HttpContext.Session.GetString("SessionTime"))).TotalSeconds);
                    llog.RecordChangeType = "Delete";
                    llog.RecordType = "Exercise";
                    llog.Comment = "Record deleted";
                    llog.RecordJson = JsonConvert.SerializeObject(lprotocol);
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
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            if (!string.IsNullOrEmpty(returnView) && !string.IsNullOrEmpty(deviceConfiguration))
            {
                return RedirectToAction("Index", "Review", new { id = patId, Username = patName, EquipmentType = eType, actuator = deviceConfiguration, tab = "Exercises" });
            }
            else
            {
                return RedirectToAction("ProtocolList", "CreatePatient", new { patId = patId, Username = patName, eType = eType });
            }
        }


        public void getDetails()
        {
            //this function will get the list of all equipments
            List<SelectListItem> myList = new List<SelectListItem>()
                         {
                            new SelectListItem{ Value="Ankle",Text="Ankle"},
                            new SelectListItem{ Value="Knee",Text="Knee"},
                            new SelectListItem{ Value="Shoulder",Text="Shoulder"}
                         };
            ViewBag.equipment = myList;

        }
    }
}
