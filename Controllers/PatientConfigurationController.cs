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
using OneDirect.ViewModels;
using OneDirect.Extensions;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    public class PatientConfigurationController : Controller
    {
        private readonly IEquipmentExerciseInterface lIEquipmentExerciseRepository;
        private readonly IDeviceCalibrationInterface lIDeviceCalibrationRepository;
        private readonly IPatientRxInterface IPatientRx;
        private readonly INewPatient INewPatient;
        private readonly IUserInterface lIUserRepository;
        private readonly IPatient IPatient;
        private readonly ISessionInterface lISessionRepository;
        private readonly IPatientConfigurationInterface lIPatientConfigurationRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public PatientConfigurationController(OneDirectContext context, ILogger<PatientConfigurationController> plogger, IEquipmentExerciseInterface IEquipmentExerciseRepository, IPatientRxInterface lIPatientRx, INewPatient lINewPatient)
        {
            lIEquipmentExerciseRepository = IEquipmentExerciseRepository;
            logger = plogger;
            this.context = context;
            IPatientRx = lIPatientRx;
            INewPatient = lINewPatient;
            lIUserRepository = new UserRepository(context);
            IPatient = new PatientRepository(context);
            lISessionRepository = new SessionRepository(context);
            lIPatientConfigurationRepository = new PatientConfigurationRepository(context);
            lIEquipmentAssignmentRepository = new AssignmentRepository(context);
            lIDeviceCalibrationRepository = new DeviceCalibrationRepository(context);

        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        //called to check if the patient is configured
        [HttpPost]
        [Route("checkpatientconfiguration")]
        public JsonResult checkpatientconfiguration([FromBody]checkpatientconfiguration pPatientConfiguration, string sessionid)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("check patientconfiguration Start");
                if (string.IsNullOrEmpty(sessionid))
                {
                    return Json(new { Status = (int)HttpStatusCode.Accepted, result = "The Session Id should not be null", TimeZone = DateTime.UtcNow.ToString("s") });
                }

                User luser = lIUserRepository.getUserbySessionId(sessionid);
                if (luser != null)
                {
                    if (pPatientConfiguration != null)
                    {
                        if (ModelState.IsValid == false)
                        {
                            var modelErrors = new List<string>();
                            foreach (var modelState in ModelState.Values)
                            {
                                foreach (var modelError in modelState.Errors)
                                {
                                    modelErrors.Add(modelError.ErrorMessage);
                                }
                            }
                            return Json(new { Status = (int)HttpStatusCode.Accepted, result = "The Required filed are missing", Error = modelErrors, TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                        NewPatient lpatient = INewPatient.GetPatientByPatitentLoginId(pPatientConfiguration.PatientId.ToLower());

                        if (lpatient != null)
                        {
                            PatientRx patientRx = IPatientRx.getPatientRxByPEDP(lpatient.PatientId.ToString(), pPatientConfiguration.EquipmentType, pPatientConfiguration.DeviceConfiguration, pPatientConfiguration.PatientSide);
                            if (patientRx == null)
                            {
                                return Json(new { Status = (int)HttpStatusCode.OK, result = "Patient Configuration information", usecasecode = 1, RxId = "", patientName = lpatient.PatientName.Split(new char[0]).Length > 0 ? lpatient.PatientName.Split(new char[0])[0] : lpatient.PatientName, TimeZone = DateTime.UtcNow.ToString("s") });
                            }
                            else
                            {
                                PatientConfiguration lpatientConfig = lIPatientConfigurationRepository.getPatientConfiguration(lpatient.PatientId.ToString(), pPatientConfiguration.EquipmentType, pPatientConfiguration.DeviceConfiguration, pPatientConfiguration.PatientSide);
                                if (lpatientConfig != null)
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Patient Configuration information", usecasecode = 2, RxId = lpatientConfig.RxId, patientName = lpatient.PatientName.Split(new char[0]).Length > 0 ? lpatient.PatientName.Split(new char[0])[0] : lpatient.PatientName, TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                                else
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Patient Configuration information", usecasecode = 3, RxId = patientRx.RxId, patientName = lpatient.PatientName.Split(new char[0]).Length > 0 ? lpatient.PatientName.Split(new char[0])[0] : lpatient.PatientName, TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }
                        }
                        else
                        {
                            return Json(new { Status = (int)4, result = "Check Patient Configuration failed - Patient Details is not found", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Created, result = "Check Patient Configuration failed", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Unauthorized, result = "Unauthorized User", TimeZone = DateTime.UtcNow.ToString("s") });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Patient Configuration insertion failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }

        }


        //Called with new set of data when a record for the Patient configuration does not exist. It will create new configuration record and update it
        // POST api/values
        [HttpPost]
        [Route("createpatientconfiguration")]
        public JsonResult createpatientconfiguration([FromBody]PatientConfigurationView pPatientConfiguration, string sessionid)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("create patientconfiguration Start");
                if (string.IsNullOrEmpty(sessionid))
                {
                    return Json(new { Status = (int)HttpStatusCode.Accepted, result = "The Session Id should not be null", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                User luser = lIUserRepository.getUserbySessionId(sessionid);
                if (luser != null)
                {
                    if (pPatientConfiguration != null)
                    {
                        if (ModelState.IsValid == false)
                        {
                            var modelErrors = new List<string>();
                            foreach (var modelState in ModelState.Values)
                            {
                                foreach (var modelError in modelState.Errors)
                                {
                                    modelErrors.Add(modelError.ErrorMessage);
                                }
                            }
                            return Json(new { Status = (int)HttpStatusCode.Accepted, result = "The Required filed are missing", Error = modelErrors, TimeZone = DateTime.UtcNow.ToString("s") });
                        }

                        DeviceCalibration ldevice = lIDeviceCalibrationRepository.getDeviceCalibration(pPatientConfiguration.patientconfiguration.SetupId);
                        if (ldevice == null)
                        {
                            return Json(new { Status = (int)5, result = "The SetupId is not matching with device calibration records", TimeZone = DateTime.UtcNow.ToString("s") });
                        }

                        //if (ldevice != null)
                        //{
                        //    ldevice = lIDeviceCalibrationRepository.getDeviceCalibration(pPatientConfiguration.patientconfiguration.SetupId, luser.UserId);
                        //    if (ldevice == null)
                        //    {
                        //        return Json(new { Status = (int)5, result = "The SetupId is not matching with installer, The setup has configured by another installer", TimeZone = DateTime.UtcNow.ToString("s") });
                        //    }
                        //}
                        //else
                        //{
                        //    return Json(new { Status = (int)5, result = "The SetupId is not matching with device calibration records", TimeZone = DateTime.UtcNow.ToString("s") });
                        //}

                        NewPatient lnewpatient = INewPatient.GetPatientByPatitentLoginId(pPatientConfiguration.patientconfiguration.PatientId);

                        if (lnewpatient != null)
                        {
                            pPatientConfiguration.patientconfiguration.CreatedDate = DateTime.UtcNow;
                            pPatientConfiguration.patientconfiguration.UpdatedDate = DateTime.UtcNow;
                            pPatientConfiguration.patientconfiguration.RxId = pPatientConfiguration.Rxid;

                            PatientRx lrx = INewPatient.GetPatientRxByPatIdandDeviceConfig(lnewpatient.PatientId, pPatientConfiguration.patientconfiguration.DeviceConfiguration);
                            if (lrx != null && lrx.RxId == pPatientConfiguration.Rxid)
                            {

                                pPatientConfiguration.patientconfiguration.PatientFirstName = lnewpatient.PatientName.Split(new char[0]).Length > 0 ? lnewpatient.PatientName.Split(new char[0])[0] : lnewpatient.PatientName;

                                pPatientConfiguration.patientconfiguration.InstallerId = luser.UserId;
                                pPatientConfiguration.patientconfiguration.PatientId = lnewpatient.PatientId.ToString();
                                PatientConfiguration lpatConfig = PatientConfigurationExtension.PatientConfigurationViewToPatientConfiguration(pPatientConfiguration.patientconfiguration);
                                if (lpatConfig != null)
                                {
                                    lIPatientConfigurationRepository.InsertPatientConfiguration(lpatConfig);
                                    int resultId = lpatConfig.Id;
                                    if (resultId > 0)
                                    {
                                        int res = INewPatient.ChangeRxCurrent(pPatientConfiguration.Rxid, pPatientConfiguration.patientconfiguration.CurrentFlexion, pPatientConfiguration.patientconfiguration.CurrentExtension, "Installer");
                                        if (res > 0)
                                        {
                                            List<NewProtocol> protocolList = INewPatient.GetProtocolByRxId(pPatientConfiguration.Rxid);
                                            if (protocolList == null || (protocolList != null && protocolList.Count == 0))
                                            {
                                                string result = createProtocol(pPatientConfiguration.Rxid, lpatConfig);
                                            }
                                            return Json(new { Status = (int)HttpStatusCode.OK, result = "Patient Configuration inserted successfully", configurationId = lpatConfig.Id, TimeZone = DateTime.UtcNow.ToString("s") });
                                        }
                                        else
                                        {
                                            lIPatientConfigurationRepository.DeletePatientConfigurationbyConfigId(lpatConfig.Id);
                                            return Json(new { Status = (int)HttpStatusCode.Created, result = "Patient Configuration insertion failed", TimeZone = DateTime.UtcNow.ToString("s") });
                                        }
                                    }
                                    else
                                    {
                                        return Json(new { Status = (int)HttpStatusCode.Created, result = "Patient Configuration insertion failed", TimeZone = DateTime.UtcNow.ToString("s") });
                                    }
                                }
                                else
                                {
                                    return Json(new { Status = (int)HttpStatusCode.Created, result = "Patient Configuration is null", TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }
                            else
                            {
                                return Json(new { Status = (int)6, result = "Patient Configuration insertion failed, RxId is not matiching", TimeZone = DateTime.UtcNow.ToString("s") });
                            }
                        }
                        else
                        {
                            return Json(new { Status = (int)4, result = "Patient Configuration insertion failed, Patient ID is not matiching", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Created, result = "Patient Configuration request is null", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Unauthorized, result = "Unauthorized User", TimeZone = DateTime.UtcNow.ToString("s") });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Patient Configuration insertion failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }

        }

        public string createProtocol(string RXID, PatientConfiguration pPatientConfiguration)
        {
            string result = "";

            try
            {
                if (!string.IsNullOrEmpty(RXID) && pPatientConfiguration != null)
                {
                    DeviceCalibration ldevice = lIDeviceCalibrationRepository.getDeviceCalibration(pPatientConfiguration.SetupId);

                    PatientRx PatientRx;
                    PatientRx = INewPatient.GetNewPatientRxByRxId(RXID);
                    if (PatientRx != null && ldevice != null)
                    {
                        List<ExcerciseProtocol> ExcerciseProtocols = lIEquipmentExerciseRepository.GetExcerciseProtocol();
                        ExcerciseProtocols = ExcerciseProtocols.Where(p => p.Limb == pPatientConfiguration.EquipmentType && p.ExcerciseEnum == pPatientConfiguration.DeviceConfiguration).ToList();

                        foreach (ExcerciseProtocol ep in ExcerciseProtocols)
                        {
                            NewProtocol _protocol = new NewProtocol();

                            //List<EquipmentExcercise> EquipmentExcercise = Utilities.GetEquipmentExcercise();


                            _protocol.RxId = PatientRx.RxId;
                            _protocol.EquipmentType = PatientRx.EquipmentType;

                            _protocol.ExcerciseEnum = ep.ExcerciseEnum;


                            _protocol.ProtocolName = "Initial " + ep.ProtocolName;

                            _protocol.LaymanName = "Initial " + ep.ProtocolName;

                            _protocol.ExcerciseName = ep.ProtocolEnum.ToString();
                            _protocol.PatientId = PatientRx.PatientId;

                            _protocol.StartDate = PatientRx.RxStartDate;
                            _protocol.RxEndDate = PatientRx.RxEndDate;
                            _protocol.EndDate = PatientRx.RxEndDate;
                            _protocol.SurgeryDate = PatientRx.RxStartDate;

                            _protocol.RestAt = 0;
                            _protocol.RepsAt = 0;
                            _protocol.Speed = 0;
                            _protocol.Reps = 5;
                            _protocol.RestTime = 30;
                            _protocol.Speed = 100;
                            _protocol.RestPosition = pPatientConfiguration.CurrentRestPosition > 0 ? pPatientConfiguration.CurrentRestPosition : 1;
                            _protocol.Time = Math.Abs((((Convert.ToInt32(_protocol.RestTime) + Convert.ToInt32(_protocol.StretchUpHoldtime) + Convert.ToInt32(_protocol.StretchDownHoldtime)) / 60) + 2) * Convert.ToInt32(_protocol.Reps));

                            int limitValueFlex = 0;
                            int limitValueExt = 0;
                            if (_protocol.EquipmentType.ToLower() == "shoulder")
                            {
                                _protocol.FlexUpLimit = PatientRx.CurrentFlexion;

                                _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);
                                _protocol.StretchUpHoldtime = 10;
                                _protocol.StretchDownHoldtime = 10;
                                if (_protocol.ExcerciseEnum == "Forward Flexion")
                                {
                                    _protocol.CurrentFlex = Constants.Sh_Flex_Current;
                                    _protocol.GoalFlex = Constants.Sh_Flex_Goal;
                                    _protocol.Plane = PlaneProperty.FrontalAngle;

                                    limitValueFlex = ldevice.Actuator2ExtendedAngle.HasValue ? ldevice.Actuator2ExtendedAngle.Value : 0;
                                }

                                if (_protocol.ExcerciseEnum == "External Rotation")
                                {
                                    _protocol.CurrentFlex = Constants.Sh_ExRot_Current;
                                    _protocol.GoalFlex = Constants.Sh_ExRot_Goal;
                                    limitValueFlex = ldevice.Actuator3ExtendedAngle.HasValue ? ldevice.Actuator3ExtendedAngle.Value : 0;
                                }

                                //StretchUpLimit should be less than or equal to LimitValueFlex
                                if (_protocol.StretchUpLimit > limitValueFlex)
                                {
                                    _protocol.StretchUpLimit = limitValueFlex;
                                }

                                //StretchUpLimit should be less than or equal to Goal Flexion
                                if (_protocol.StretchUpLimit > _protocol.GoalFlex)
                                {
                                    _protocol.StretchUpLimit = _protocol.GoalFlex;
                                }
                            }
                            else if (_protocol.EquipmentType.ToLower() == "ankle")
                            {
                                limitValueFlex = ldevice.Actuator1RetractedAngle;
                                limitValueExt = ldevice.Actuator1ExtendedAngle;
                                if (ep.ProtocolEnum.ToString() == "1")
                                {
                                    _protocol.CurrentFlex = Constants.Ankle_Flex_Current;
                                    _protocol.GoalFlex = Constants.Ankle_Flex_Goal;

                                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;

                                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchUpLimit should be less than or equal to LimitValueFlex
                                    if (_protocol.StretchUpLimit > limitValueFlex)
                                    {
                                        _protocol.StretchUpLimit = limitValueFlex;
                                    }
                                    //StretchUpLimit should be less than or equal to Goal Flexion
                                    if (_protocol.StretchUpLimit > _protocol.GoalFlex)
                                    {
                                        _protocol.StretchUpLimit = _protocol.GoalFlex;
                                    }
                                    _protocol.StretchUpHoldtime = 10;
                                    _protocol.StretchDownHoldtime = 10;
                                }
                                else if (ep.ProtocolEnum.ToString() == "2")
                                {

                                    _protocol.CurrentExten = Constants.Ankle_Ext_Current;
                                    _protocol.GoalExten = Constants.Ankle_Ext_Goal;

                                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;

                                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchDownLimit should be greater than or equal to LimitValueExt
                                    if (_protocol.StretchDownLimit < limitValueExt)
                                    {
                                        _protocol.StretchDownLimit = limitValueExt;
                                    }
                                    //StretchDownLimit should be greater than or equal to Goal Extension
                                    if (_protocol.StretchDownLimit < _protocol.GoalExten)
                                    {
                                        _protocol.StretchDownLimit = _protocol.GoalExten;
                                    }
                                    _protocol.StretchDownHoldtime = 10;
                                    _protocol.StretchUpHoldtime = 10;
                                }
                                else
                                {
                                    _protocol.CurrentFlex = Constants.Ankle_Flex_Current;
                                    _protocol.GoalFlex = Constants.Ankle_Flex_Goal;
                                    _protocol.CurrentExten = Constants.Ankle_Ext_Current;
                                    _protocol.GoalExten = Constants.Ankle_Ext_Goal;

                                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;

                                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchUpLimit should be less than or equal to LimitValueFlex
                                    if (_protocol.StretchUpLimit > limitValueFlex)
                                    {
                                        _protocol.StretchUpLimit = limitValueFlex;
                                    }
                                    //StretchUpLimit should be less than or equal to Goal Flexion
                                    if (_protocol.StretchUpLimit > _protocol.GoalFlex)
                                    {
                                        _protocol.StretchUpLimit = _protocol.GoalFlex;
                                    }
                                    _protocol.StretchUpHoldtime = 10;

                                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;

                                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);
                                    //StretchDownLimit should be greater than or equal to LimitValueExt
                                    if (_protocol.StretchDownLimit < limitValueExt)
                                    {
                                        _protocol.StretchDownLimit = limitValueExt;
                                    }
                                    //StretchDownLimit should be greater than or equal to Goal Extension
                                    if (_protocol.StretchDownLimit < _protocol.GoalExten)
                                    {
                                        _protocol.StretchDownLimit = _protocol.GoalExten;
                                    }
                                    _protocol.StretchDownHoldtime = 10;
                                }

                            }
                            else
                            {
                                limitValueFlex = ldevice.Actuator2ExtendedAngle.HasValue ? ldevice.Actuator2ExtendedAngle.Value : 0; ;
                                limitValueExt = ldevice.Actuator2RetractedAngle.HasValue ? ldevice.Actuator2RetractedAngle.Value : 0;
                                if (ep.ProtocolEnum.ToString() == "1")
                                {
                                    _protocol.CurrentFlex = Constants.Knee_Flex_Current;
                                    _protocol.GoalFlex = Constants.Knee_Flex_Goal;

                                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;

                                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchUpLimit should be less than or equal to LimitValueFlex
                                    if (_protocol.StretchUpLimit > limitValueFlex)
                                    {
                                        _protocol.StretchUpLimit = limitValueFlex;
                                    }
                                    //StretchUpLimit should be less than or equal to Goal Flexion
                                    if (_protocol.StretchUpLimit > _protocol.GoalFlex)
                                    {
                                        _protocol.StretchUpLimit = _protocol.GoalFlex;
                                    }
                                    _protocol.StretchUpHoldtime = 10;
                                    _protocol.StretchDownHoldtime = 10;
                                }
                                else if (ep.ProtocolEnum.ToString() == "2")
                                {

                                    _protocol.CurrentExten = Constants.Knee_Ext_Current;
                                    _protocol.GoalExten = Constants.Knee_Ext_Goal;

                                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;

                                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchDownLimit should be greater than or equal to LimitValueExt
                                    if (_protocol.StretchDownLimit < limitValueExt)
                                    {
                                        _protocol.StretchDownLimit = limitValueExt;
                                    }
                                    //StretchDownLimit should be greater than or equal to Goal Extension
                                    if (_protocol.StretchDownLimit < _protocol.GoalExten)
                                    {
                                        _protocol.StretchDownLimit = _protocol.GoalExten;
                                    }
                                    _protocol.StretchDownHoldtime = 10;
                                    _protocol.StretchUpHoldtime = 10;
                                }
                                else
                                {
                                    _protocol.CurrentFlex = Constants.Knee_Flex_Current;
                                    _protocol.GoalFlex = Constants.Knee_Flex_Goal;
                                    _protocol.CurrentExten = Constants.Knee_Ext_Current;
                                    _protocol.GoalExten = Constants.Knee_Ext_Goal;

                                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;

                                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchUpLimit should be less than or equal to LimitValueFlex
                                    if (_protocol.StretchUpLimit > limitValueFlex)
                                    {
                                        _protocol.StretchUpLimit = limitValueFlex;
                                    }
                                    //StretchUpLimit should be less than or equal to Goal Flexion
                                    if (_protocol.StretchUpLimit > _protocol.GoalFlex)
                                    {
                                        _protocol.StretchUpLimit = _protocol.GoalFlex;
                                    }
                                    _protocol.StretchUpHoldtime = 10;

                                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;

                                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchDownLimit should be greater than or equal to LimitValueExt
                                    if (_protocol.StretchDownLimit < limitValueExt)
                                    {
                                        _protocol.StretchDownLimit = limitValueExt;
                                    }
                                    //StretchDownLimit should be greater than or equal to Goal Extension
                                    if (_protocol.StretchDownLimit < _protocol.GoalExten)
                                    {
                                        _protocol.StretchDownLimit = _protocol.GoalExten;
                                    }
                                    _protocol.StretchDownHoldtime = 10;
                                }

                            }

                            Protocol pro = INewPatient.CreateProtocol(_protocol);
                            if (pro != null)
                            {
                                result = "success";
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
                return null;
            }
            return result;

        }


        public string updateProtocol(string RXID, PatientConfiguration pPatientConfiguration)
        {
            string result = "";

            try
            {
                if (!string.IsNullOrEmpty(RXID) && pPatientConfiguration != null)
                {
                    DeviceCalibration ldevice = lIDeviceCalibrationRepository.getDeviceCalibration(pPatientConfiguration.SetupId);

                    PatientRx PatientRx;
                    PatientRx = INewPatient.GetNewPatientRxByRxId(RXID);
                    if (PatientRx != null && ldevice != null)
                    {
                        List<NewProtocol> protocolList = INewPatient.GetProtocolByRxId(RXID);
                        foreach (NewProtocol _protocol in protocolList)
                        {
                            _protocol.RestPosition = pPatientConfiguration.CurrentRestPosition > 0 ? pPatientConfiguration.CurrentRestPosition : 1;
                            _protocol.Time = Math.Abs((((Convert.ToInt32(_protocol.RestTime) + Convert.ToInt32(_protocol.StretchUpHoldtime) + Convert.ToInt32(_protocol.StretchDownHoldtime)) / 60) + 2) * Convert.ToInt32(_protocol.Reps));

                            int limitValueFlex = 0;
                            int limitValueExt = 0;
                            if (_protocol.EquipmentType.ToLower() == "shoulder")
                            {
                                _protocol.FlexUpLimit = PatientRx.CurrentFlexion;

                                _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);
                                _protocol.StretchUpHoldtime = 10;
                                _protocol.StretchDownHoldtime = 10;
                                if (_protocol.ExcerciseEnum == "Forward Flexion")
                                {
                                    _protocol.CurrentFlex = Constants.Sh_Flex_Current;
                                    _protocol.GoalFlex = Constants.Sh_Flex_Goal;
                                    if (_protocol.Plane == null || (_protocol.Plane != null && _protocol.Plane == 0))
                                        _protocol.Plane = PlaneProperty.FrontalAngle;
                                    limitValueFlex = ldevice.Actuator2ExtendedAngle.HasValue ? ldevice.Actuator2ExtendedAngle.Value : 0;
                                }
                                if (_protocol.ExcerciseEnum == "External Rotation")
                                {
                                    _protocol.CurrentFlex = Constants.Sh_ExRot_Current;
                                    _protocol.GoalFlex = Constants.Sh_ExRot_Goal;
                                    limitValueFlex = ldevice.Actuator3ExtendedAngle.HasValue ? ldevice.Actuator3ExtendedAngle.Value : 0;
                                }

                                //StretchUpLimit should be less than or equal to LimitValueFlex
                                if (_protocol.StretchUpLimit > limitValueFlex)
                                {
                                    _protocol.StretchUpLimit = limitValueFlex;
                                }

                                //StretchUpLimit should be less than or equal to Goal Flexion
                                if (_protocol.StretchUpLimit > _protocol.GoalFlex)
                                {
                                    _protocol.StretchUpLimit = _protocol.GoalFlex;
                                }
                            }
                            else if (_protocol.EquipmentType.ToLower() == "ankle")
                            {
                                limitValueFlex = ldevice.Actuator1ExtendedAngle;
                                limitValueExt = ldevice.Actuator1RetractedAngle;
                                if (_protocol.ProtocolEnum.ToString() == "1")
                                {
                                    _protocol.CurrentFlex = Constants.Ankle_Flex_Current;
                                    _protocol.GoalFlex = Constants.Ankle_Flex_Goal;

                                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;

                                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchUpLimit should be less than or equal to LimitValueFlex
                                    if (_protocol.StretchUpLimit > limitValueFlex)
                                    {
                                        _protocol.StretchUpLimit = limitValueFlex;
                                    }
                                    //StretchUpLimit should be less than or equal to Goal Flexion
                                    if (_protocol.StretchUpLimit > _protocol.GoalFlex)
                                    {
                                        _protocol.StretchUpLimit = _protocol.GoalFlex;
                                    }
                                    _protocol.StretchUpHoldtime = 10;
                                    _protocol.StretchDownHoldtime = 10;
                                }
                                else if (_protocol.ProtocolEnum.ToString() == "2")
                                {

                                    _protocol.CurrentExten = Constants.Ankle_Ext_Current;
                                    _protocol.GoalExten = Constants.Ankle_Ext_Goal;

                                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;

                                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchDownLimit should be greater than or equal to LimitValueExt
                                    if (_protocol.StretchDownLimit < limitValueExt)
                                    {
                                        _protocol.StretchDownLimit = limitValueExt;
                                    }
                                    //StretchDownLimit should be greater than or equal to Goal Extension
                                    if (_protocol.StretchDownLimit < _protocol.GoalExten)
                                    {
                                        _protocol.StretchDownLimit = _protocol.GoalExten;
                                    }
                                    _protocol.StretchDownHoldtime = 10;
                                    _protocol.StretchUpHoldtime = 10;
                                }
                                else
                                {
                                    _protocol.CurrentFlex = Constants.Ankle_Flex_Current;
                                    _protocol.GoalFlex = Constants.Ankle_Flex_Goal;
                                    _protocol.CurrentExten = Constants.Ankle_Ext_Current;
                                    _protocol.GoalExten = Constants.Ankle_Ext_Goal;

                                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;

                                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchUpLimit should be less than or equal to LimitValueFlex
                                    if (_protocol.StretchUpLimit > limitValueFlex)
                                    {
                                        _protocol.StretchUpLimit = limitValueFlex;
                                    }
                                    //StretchUpLimit should be less than or equal to Goal Flexion
                                    if (_protocol.StretchUpLimit > _protocol.GoalFlex)
                                    {
                                        _protocol.StretchUpLimit = _protocol.GoalFlex;
                                    }
                                    _protocol.StretchUpHoldtime = 10;

                                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;

                                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);
                                    //StretchDownLimit should be greater than or equal to LimitValueExt
                                    if (_protocol.StretchDownLimit < limitValueExt)
                                    {
                                        _protocol.StretchDownLimit = limitValueExt;
                                    }
                                    //StretchDownLimit should be greater than or equal to Goal Extension
                                    if (_protocol.StretchDownLimit < _protocol.GoalExten)
                                    {
                                        _protocol.StretchDownLimit = _protocol.GoalExten;
                                    }
                                    _protocol.StretchDownHoldtime = 10;
                                }

                            }
                            else
                            {
                                limitValueFlex = ldevice.Actuator2ExtendedAngle.HasValue ? ldevice.Actuator2ExtendedAngle.Value : 0; ;
                                limitValueExt = ldevice.Actuator2RetractedAngle.HasValue ? ldevice.Actuator2RetractedAngle.Value : 0;
                                if (_protocol.ProtocolEnum.ToString() == "1")
                                {
                                    _protocol.CurrentFlex = Constants.Knee_Flex_Current;
                                    _protocol.GoalFlex = Constants.Knee_Flex_Goal;

                                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;

                                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchUpLimit should be less than or equal to LimitValueFlex
                                    if (_protocol.StretchUpLimit > limitValueFlex)
                                    {
                                        _protocol.StretchUpLimit = limitValueFlex;
                                    }
                                    //StretchUpLimit should be less than or equal to Goal Flexion
                                    if (_protocol.StretchUpLimit > _protocol.GoalFlex)
                                    {
                                        _protocol.StretchUpLimit = _protocol.GoalFlex;
                                    }
                                    _protocol.StretchUpHoldtime = 10;
                                    _protocol.StretchDownHoldtime = 10;
                                }
                                else if (_protocol.ProtocolEnum.ToString() == "2")
                                {

                                    _protocol.CurrentExten = Constants.Knee_Ext_Current;
                                    _protocol.GoalExten = Constants.Knee_Ext_Goal;

                                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;

                                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchDownLimit should be greater than or equal to LimitValueExt
                                    if (_protocol.StretchDownLimit < limitValueExt)
                                    {
                                        _protocol.StretchDownLimit = limitValueExt;
                                    }
                                    //StretchDownLimit should be greater than or equal to Goal Extension
                                    if (_protocol.StretchDownLimit < _protocol.GoalExten)
                                    {
                                        _protocol.StretchDownLimit = _protocol.GoalExten;
                                    }
                                    _protocol.StretchDownHoldtime = 10;
                                    _protocol.StretchUpHoldtime = 10;
                                }
                                else
                                {
                                    _protocol.CurrentFlex = Constants.Knee_Flex_Current;
                                    _protocol.GoalFlex = Constants.Knee_Flex_Goal;
                                    _protocol.CurrentExten = Constants.Knee_Ext_Current;
                                    _protocol.GoalExten = Constants.Knee_Ext_Goal;

                                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;

                                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchUpLimit should be less than or equal to LimitValueFlex
                                    if (_protocol.StretchUpLimit > limitValueFlex)
                                    {
                                        _protocol.StretchUpLimit = limitValueFlex;
                                    }
                                    //StretchUpLimit should be less than or equal to Goal Flexion
                                    if (_protocol.StretchUpLimit > _protocol.GoalFlex)
                                    {
                                        _protocol.StretchUpLimit = _protocol.GoalFlex;
                                    }
                                    _protocol.StretchUpHoldtime = 10;

                                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;

                                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);

                                    //StretchDownLimit should be greater than or equal to LimitValueExt
                                    if (_protocol.StretchDownLimit < limitValueExt)
                                    {
                                        _protocol.StretchDownLimit = limitValueExt;
                                    }
                                    //StretchDownLimit should be greater than or equal to Goal Extension
                                    if (_protocol.StretchDownLimit < _protocol.GoalExten)
                                    {
                                        _protocol.StretchDownLimit = _protocol.GoalExten;
                                    }
                                    _protocol.StretchDownHoldtime = 10;
                                }

                            }
                            Protocol pro = INewPatient.CreateProtocol(_protocol);
                            if (pro != null)
                            {
                                result = "success";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
                return null;
            }
            return result;

        }


        //Called to Update Patient Configuration if a Patient is already configured
        [HttpPost]
        [Route("updatepatientconfiguration")]
        public JsonResult updatepatientconfiguration([FromBody]PatientConfig pPatientConfiguration, string sessionid)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("Update Patient Configuration Post Start");
                if (string.IsNullOrEmpty(sessionid))
                {
                    return Json(new { Status = (int)HttpStatusCode.Accepted, result = "The Session Id should not be null", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                User luser = lIUserRepository.getUserbySessionId(sessionid);
                if (luser != null)
                {
                    if (pPatientConfiguration != null)
                    {
                        if (ModelState.IsValid == false)
                        {
                            var modelErrors = new List<string>();
                            foreach (var modelState in ModelState.Values)
                            {
                                foreach (var modelError in modelState.Errors)
                                {
                                    modelErrors.Add(modelError.ErrorMessage);
                                }
                            }
                            return Json(new { Status = (int)HttpStatusCode.Accepted, result = "The Required filed are missing", Error = modelErrors, TimeZone = DateTime.UtcNow.ToString("s") });
                        }

                        DeviceCalibration ldevice = lIDeviceCalibrationRepository.getDeviceCalibration(pPatientConfiguration.SetupId);

                        if (ldevice == null)
                        {
                            return Json(new { Status = (int)5, result = "The SetupId is not matching with device calibration records", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                        //if (ldevice != null)
                        //{
                        //    ldevice = lIDeviceCalibrationRepository.getDeviceCalibration(pPatientConfiguration.SetupId, luser.UserId);
                        //    if (ldevice == null)
                        //    {
                        //        return Json(new { Status = (int)5, result = "The SetupId is not matching with installer, The setup has configured by another installer", TimeZone = DateTime.UtcNow.ToString("s") });
                        //    }
                        //}
                        //else
                        //{
                        //    return Json(new { Status = (int)5, result = "The SetupId is not matching with device calibration records", TimeZone = DateTime.UtcNow.ToString("s") });
                        //}
                        if (!string.IsNullOrEmpty(pPatientConfiguration.RxId))
                        {
                            PatientRx lrx = IPatientRx.getById(pPatientConfiguration.RxId);
                            if (lrx == null)
                            {
                                return Json(new { Status = (int)6, result = "The RxId is not matching with PatientRx records", TimeZone = DateTime.UtcNow.ToString("s") });
                            }
                        }

                        NewPatient lnewpatient = INewPatient.GetPatientByPatitentLoginId(pPatientConfiguration.PatientId);

                        if (lnewpatient != null)
                        {
                            PatientConfiguration lconfig = lIPatientConfigurationRepository.getPatientConfiguration(lnewpatient.PatientId, pPatientConfiguration.SetupId);
                            if (lconfig != null)
                            {

                                lconfig.InstallerId = luser.UserId;
                                if (!string.IsNullOrEmpty(pPatientConfiguration.EquipmentType))
                                    lconfig.EquipmentType = pPatientConfiguration.EquipmentType;
                                if (!string.IsNullOrEmpty(pPatientConfiguration.DeviceConfiguration))
                                    lconfig.DeviceConfiguration = pPatientConfiguration.DeviceConfiguration;
                                if (!string.IsNullOrEmpty(pPatientConfiguration.PatientSide))
                                    lconfig.PatientSide = pPatientConfiguration.PatientSide;
                                if (pPatientConfiguration.CurrentFlexion > 0)
                                    lconfig.CurrentFlexion = pPatientConfiguration.CurrentFlexion;
                                if (pPatientConfiguration.CurrentExtension > 0)
                                    lconfig.CurrentExtension = pPatientConfiguration.CurrentExtension;
                                if (pPatientConfiguration.CurrentRestPosition > 0)
                                    lconfig.CurrentRestPosition = pPatientConfiguration.CurrentRestPosition;
                                if (pPatientConfiguration.UserMode > 0)
                                    lconfig.UserMode = pPatientConfiguration.UserMode;
                                lconfig.UpdatedDate = DateTime.UtcNow;
                                lconfig.SagittalAngle = pPatientConfiguration.SagittalAngle;
                                lconfig.ScapularAngle = pPatientConfiguration.ScapularAngle;
                                lconfig.FrontalAngle = pPatientConfiguration.FrontalAngle;




                                lconfig.PatientFirstName = lnewpatient.PatientName.Split(new char[0]).Length > 0 ? lnewpatient.PatientName.Split(new char[0])[0] : lnewpatient.PatientName;

                                lIPatientConfigurationRepository.UpdatePatientConfiguration(lconfig);

                                INewPatient.ChangeRxCurrent(lconfig.RxId, lconfig.CurrentFlexion, lconfig.CurrentExtension, "Installer");

                                updateProtocol(lconfig.RxId, lconfig);

                                return Json(new { Status = (int)HttpStatusCode.OK, result = "Patient Configuration updated successfully", TimeZone = DateTime.UtcNow.ToString("s") });
                            }
                            else
                            {
                                return Json(new { Status = (int)HttpStatusCode.NoContent, result = "Patient Configuration update failed -Patient configuration is not exist with the combination of PatientId and SetupId", TimeZone = DateTime.UtcNow.ToString("s") });
                            }
                        }
                        else
                        {
                            return Json(new { Status = (int)4, result = "Patient Configuration update failed - Patient Details is not found", TimeZone = DateTime.UtcNow.ToString("s") });
                        }


                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Created, result = "Patient Configuration is null", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Unauthorized, result = "Unauthorized User", TimeZone = DateTime.UtcNow.ToString("s") });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Patient Configuration failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }

        }


        //This API is used to download the Patient’s patientconfiguration information for a specific deviceconfiguration required for patient
        [HttpGet]
        [Route("downloadpatientconfiguration")]
        public JsonResult downloadpatientconfiguration(string setupId, string patientId)
        {
            try
            {
                //NewPatient lnewpatient = INewPatient.GetPatientByPatitentLoginId(patientId);
                if (!string.IsNullOrEmpty(setupId) && !string.IsNullOrEmpty(patientId))
                {
                    DeviceCalibration ldevice = lIDeviceCalibrationRepository.getDeviceCalibration(setupId);
                    if (ldevice == null)
                    {
                        return Json(new { Status = (int)5, result = "The SetupId is not matching with device calibration records", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    NewPatient lnewpatient = INewPatient.GetPatientByPatitentLoginId(patientId);

                    if (lnewpatient != null)
                    {
                        PatientConfiguration pPatientConfiguration = lIPatientConfigurationRepository.getPatientConfiguration(lnewpatient.PatientId, setupId);
                        if (pPatientConfiguration != null)
                        {

                            PatientConfig patConfig = PatientConfigurationExtension.PatientConfigurationToPatientConfigurationView(pPatientConfiguration);
                            if (patConfig != null)
                            {
                                patConfig.PatientId = lnewpatient.PatientLoginId;
                                PatientRx patientRx = IPatientRx.getPatientRxByPEDP(pPatientConfiguration.PatientId.ToString(), pPatientConfiguration.EquipmentType, pPatientConfiguration.DeviceConfiguration, pPatientConfiguration.PatientSide);
                                if (patientRx != null)
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "success", patientconfiguration = patConfig, Rxid = patientRx.RxId, TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                                else
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "success", patientconfiguration = patConfig, TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }
                            else
                            {
                                return Json(new { Status = (int)HttpStatusCode.Redirect, result = "Patient configuration is not exist", TimeZone = DateTime.UtcNow.ToString("s") });
                            }


                        }
                        else
                        {
                            return Json(new { Status = (int)HttpStatusCode.NoContent, result = "Patient configuration is not exist with the combination of PatientId and SetupId", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)4, result = "Patient details is not exist", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else

                {
                    if (string.IsNullOrEmpty(setupId) && !string.IsNullOrEmpty(patientId))
                    {
                        NewPatient lnewpatient = INewPatient.GetPatientByPatitentLoginId(patientId);
                        if (lnewpatient != null)
                        {
                            List<PatientConfiguration> pPatientConfiguration = lIPatientConfigurationRepository.getPatientConfigurationByPatientId(lnewpatient.PatientId);
                            if (pPatientConfiguration != null && pPatientConfiguration.Count > 0)
                            {

                                List<PatientConfig> patConfig = pPatientConfiguration.Select(x => PatientConfigurationExtension.PatientConfigurationToPatientConfigurationView(x)).ToList();
                                if (patConfig != null)
                                {

                                    patConfig = patConfig.Select(x => { x.PatientId = lnewpatient.PatientLoginId; return x; }).ToList();

                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "success", patientconfiguration = patConfig, TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                                else
                                {
                                    return Json(new { Status = (int)HttpStatusCode.Redirect, result = "Patient configuration is not exist", TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }
                            else
                            {
                                return Json(new { Status = (int)HttpStatusCode.Redirect, result = "Patient configuration is not exist", TimeZone = DateTime.UtcNow.ToString("s") });
                            }


                        }
                        else
                        {
                            return Json(new { Status = (int)4, result = "Patient details is not exist", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Accepted, result = "request string is not proper", TimeZone = DateTime.UtcNow.ToString("s") });
                    }



                }

            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
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



























