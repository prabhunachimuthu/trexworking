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
    public class DeviceCalibrationController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly IPatient IPatient;
        private readonly ISessionInterface lISessionRepository;
        private readonly IDeviceCalibrationInterface lIDeviceCalibrationRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public DeviceCalibrationController(OneDirectContext context, ILogger<DeviceCalibrationController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
            IPatient = new PatientRepository(context);
            lISessionRepository = new SessionRepository(context);
            lIDeviceCalibrationRepository = new DeviceCalibrationRepository(context);
            lIEquipmentAssignmentRepository = new AssignmentRepository(context);

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


        //Called with new set of data when a record for the Device Calibration does not exist
        // POST api/values
        [HttpPost]
        [Route("createdevicecalibration")]
        public JsonResult createdevicecalibration([FromBody]CreateDeviceCalibrationView pDeviceCalibration, string sessionid)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                
                if(string.IsNullOrEmpty(sessionid))
                {
                    return Json(new { Status = (int)HttpStatusCode.Accepted, result = "The Session Id should not be null", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                logger.LogDebug("Create device calibration Start");
                if (pDeviceCalibration != null)
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
                    User luser = lIUserRepository.getUserbySessionId(sessionid);
                    if (luser != null)
                    {
                        pDeviceCalibration.InstallerId = luser.UserId;
                        DeviceCalibration ldevice = DeviceCalibrationExtension.DeviceCalibrationViewToDeviceCalibration(pDeviceCalibration);
                        if (ldevice != null)
                        {
                            ldevice.CreatedDate = DateTime.UtcNow;
                            ldevice.UpdatedDate = DateTime.UtcNow;
                            ldevice.SetupId = Guid.NewGuid().ToString();
                            lIDeviceCalibrationRepository.InsertDeviceCalibration(ldevice);
                            return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration inserted successfully", setupid = ldevice.SetupId, TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                        else
                        {
                            return Json(new { Status = (int)HttpStatusCode.Created, result = "Device Calibration model is not valid", TimeZone = DateTime.UtcNow.ToString("s") });
                        }

                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Unauthorized, result = "Unauthorized User", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "Device Calibration Request is null", TimeZone = DateTime.UtcNow.ToString("s") });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Device Calibration insertion failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }

        }


        // check for the device calibration using installer's sessionID
        [HttpPost]
        [Route("checkdevicecalibration")]
        public JsonResult checkdevicecalibration([FromBody]CheckDeviceCalibration pDeviceCalibration, string sessionid)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("Check Device calibration Start");
                if (string.IsNullOrEmpty(sessionid))
                {
                    return Json(new { Status = (int)HttpStatusCode.Accepted, result = "The Session Id should not be null", TimeZone = DateTime.UtcNow.ToString("s") });
                }

                if (pDeviceCalibration != null)
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
                    User luser = lIUserRepository.getUserbySessionId(sessionid);
                    if (luser != null)
                    {
                        DeviceCalibration pdevice = lIDeviceCalibrationRepository.getDeviceCalibrationbyControllerId(pDeviceCalibration.ControllerId);
                        if (pdevice != null)
                        {
                            if (string.IsNullOrEmpty(pDeviceCalibration.BoomId3))
                            {
                                if (!string.IsNullOrEmpty(pDeviceCalibration.EquipmentType) && !string.IsNullOrEmpty(pDeviceCalibration.DeviceConfiguration) && !string.IsNullOrEmpty(pDeviceCalibration.PatientSide) && !string.IsNullOrEmpty(pDeviceCalibration.ChairId) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId1) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId2))
                                {
                                    pdevice = lIDeviceCalibrationRepository.getDeviceCalibrationbyCEDPCB1B2(pDeviceCalibration.ControllerId, pDeviceCalibration.EquipmentType, pDeviceCalibration.DeviceConfiguration, pDeviceCalibration.PatientSide, pDeviceCalibration.ChairId, pDeviceCalibration.BoomId1, pDeviceCalibration.BoomId2);
                                    if (pdevice != null)
                                    {
                                        return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information- Same setup being re-calibrated", usecasecode = 2, setupid = pdevice.SetupId, TimeZone = DateTime.UtcNow.ToString("s") });
                                    }
                                }
                            }
                            else if (string.IsNullOrEmpty(pDeviceCalibration.BoomId3) && string.IsNullOrEmpty(pDeviceCalibration.BoomId2))
                            {
                                if (!string.IsNullOrEmpty(pDeviceCalibration.EquipmentType) && !string.IsNullOrEmpty(pDeviceCalibration.DeviceConfiguration) && !string.IsNullOrEmpty(pDeviceCalibration.PatientSide) && !string.IsNullOrEmpty(pDeviceCalibration.ChairId) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId1))
                                {
                                    pdevice = lIDeviceCalibrationRepository.getDeviceCalibrationbyCEDPCB1(pDeviceCalibration.ControllerId, pDeviceCalibration.EquipmentType, pDeviceCalibration.DeviceConfiguration, pDeviceCalibration.PatientSide, pDeviceCalibration.ChairId, pDeviceCalibration.BoomId1);
                                    if (pdevice != null)
                                    {
                                        return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information- Same setup being re-calibrated", usecasecode = 2, setupid = pdevice.SetupId, TimeZone = DateTime.UtcNow.ToString("s") });
                                    }
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(pDeviceCalibration.EquipmentType) && !string.IsNullOrEmpty(pDeviceCalibration.DeviceConfiguration) && !string.IsNullOrEmpty(pDeviceCalibration.PatientSide) && !string.IsNullOrEmpty(pDeviceCalibration.ChairId) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId1) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId2) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId3))
                                {
                                    pdevice = lIDeviceCalibrationRepository.getDeviceCalibrationbyCEDPCB1B2B3(pDeviceCalibration.ControllerId, pDeviceCalibration.EquipmentType, pDeviceCalibration.DeviceConfiguration, pDeviceCalibration.PatientSide, pDeviceCalibration.ChairId, pDeviceCalibration.BoomId1, pDeviceCalibration.BoomId2, pDeviceCalibration.BoomId3);
                                    if (pdevice != null)
                                    {
                                        return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information- Same setup being re-calibrated", usecasecode = 2, setupid = pdevice.SetupId, TimeZone = DateTime.UtcNow.ToString("s") });
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(pDeviceCalibration.EquipmentType) && !string.IsNullOrEmpty(pDeviceCalibration.DeviceConfiguration) && !string.IsNullOrEmpty(pDeviceCalibration.PatientSide))
                            {
                                pdevice = lIDeviceCalibrationRepository.getDeviceCalibrationbyCEDP(pDeviceCalibration.ControllerId, pDeviceCalibration.EquipmentType, pDeviceCalibration.DeviceConfiguration, pDeviceCalibration.PatientSide);
                                if (pdevice != null)
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information - Same setup, minor faulty component other than controller replaced", usecasecode = 3, setupid = pdevice.SetupId, TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                                else
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information - The controller is being configured for a different type of exercise or limb with or without the same minor components", usecasecode = 4, setupid = "", TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(pDeviceCalibration.EquipmentType) && !string.IsNullOrEmpty(pDeviceCalibration.DeviceConfiguration) && !string.IsNullOrEmpty(pDeviceCalibration.PatientSide) && !string.IsNullOrEmpty(pDeviceCalibration.ChairId))
                            {
                                pdevice = lIDeviceCalibrationRepository.getDeviceCalibrationbyEDPC(pDeviceCalibration.EquipmentType, pDeviceCalibration.DeviceConfiguration, pDeviceCalibration.PatientSide, pDeviceCalibration.ChairId);
                                if (pdevice != null)
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information - New Controller replaced in an existing Device Calibration of same type", usecasecode = 5, setupid = pdevice.SetupId, TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }

                            if (!string.IsNullOrEmpty(pDeviceCalibration.EquipmentType) && !string.IsNullOrEmpty(pDeviceCalibration.DeviceConfiguration) && !string.IsNullOrEmpty(pDeviceCalibration.PatientSide) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId1))
                            {
                                pdevice = lIDeviceCalibrationRepository.getDeviceCalibrationbyEDPB1(pDeviceCalibration.EquipmentType, pDeviceCalibration.DeviceConfiguration, pDeviceCalibration.PatientSide, pDeviceCalibration.BoomId1);
                                if (pdevice != null)
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information - New Controller replaced in an existing Device Calibration of same type", usecasecode = 5, setupid = pdevice.SetupId, TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }
                            if (!string.IsNullOrEmpty(pDeviceCalibration.EquipmentType) && !string.IsNullOrEmpty(pDeviceCalibration.DeviceConfiguration) && !string.IsNullOrEmpty(pDeviceCalibration.PatientSide) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId2))
                            {
                                pdevice = lIDeviceCalibrationRepository.getDeviceCalibrationbyEDPB2(pDeviceCalibration.EquipmentType, pDeviceCalibration.DeviceConfiguration, pDeviceCalibration.PatientSide, pDeviceCalibration.BoomId2);
                                if (pdevice != null)
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information - New Controller replaced in an existing Device Calibration of same type", usecasecode = 5, setupid = pdevice.SetupId, TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }
                            if (!string.IsNullOrEmpty(pDeviceCalibration.EquipmentType) && !string.IsNullOrEmpty(pDeviceCalibration.DeviceConfiguration) && !string.IsNullOrEmpty(pDeviceCalibration.PatientSide) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId3))
                            {
                                pdevice = lIDeviceCalibrationRepository.getDeviceCalibrationbyEDPB3(pDeviceCalibration.EquipmentType, pDeviceCalibration.DeviceConfiguration, pDeviceCalibration.PatientSide, pDeviceCalibration.BoomId3);
                                if (pdevice != null)
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information - New Controller replaced in an existing Device Calibration of same type", usecasecode = 5, setupid = pdevice.SetupId, TimeZone = DateTime.UtcNow.ToString("s") });
                                }

                            }
                            if (!string.IsNullOrEmpty(pDeviceCalibration.EquipmentType) && !string.IsNullOrEmpty(pDeviceCalibration.DeviceConfiguration) && !string.IsNullOrEmpty(pDeviceCalibration.PatientSide) && !string.IsNullOrEmpty(pDeviceCalibration.ChairId) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId1) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId2) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId3))
                            {
                                pdevice = lIDeviceCalibrationRepository.getDeviceCalibrationbyEDPCB1B2B3(pDeviceCalibration.EquipmentType, pDeviceCalibration.DeviceConfiguration, pDeviceCalibration.PatientSide, pDeviceCalibration.ChairId, pDeviceCalibration.BoomId1, pDeviceCalibration.BoomId2, pDeviceCalibration.BoomId3);
                                if (pdevice != null)
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information -New Controller replaced in an existing Device Calibration of same type", usecasecode = 5, setupid = pdevice.SetupId, TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                                else
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information - Considered complete new Setup", usecasecode = 1, setupid = "", TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }
                            else if (!string.IsNullOrEmpty(pDeviceCalibration.EquipmentType) && !string.IsNullOrEmpty(pDeviceCalibration.DeviceConfiguration) && !string.IsNullOrEmpty(pDeviceCalibration.PatientSide) && !string.IsNullOrEmpty(pDeviceCalibration.ChairId) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId1) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId2))
                            {
                                pdevice = lIDeviceCalibrationRepository.getDeviceCalibrationbyEDPCB1B2(pDeviceCalibration.EquipmentType, pDeviceCalibration.DeviceConfiguration, pDeviceCalibration.PatientSide, pDeviceCalibration.ChairId, pDeviceCalibration.BoomId1, pDeviceCalibration.BoomId2);
                                if (pdevice != null)
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information - New Controller replaced in an existing Device Calibration of same type", usecasecode = 5, setupid = pdevice.SetupId, TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                                else
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information - Considered complete new Setup", usecasecode = 1, setupid = "", TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }
                            else if (!string.IsNullOrEmpty(pDeviceCalibration.EquipmentType) && !string.IsNullOrEmpty(pDeviceCalibration.DeviceConfiguration) && !string.IsNullOrEmpty(pDeviceCalibration.PatientSide) && !string.IsNullOrEmpty(pDeviceCalibration.ChairId) && !string.IsNullOrEmpty(pDeviceCalibration.BoomId1))
                            {
                                pdevice = lIDeviceCalibrationRepository.getDeviceCalibrationbyEDPCB1(pDeviceCalibration.EquipmentType, pDeviceCalibration.DeviceConfiguration, pDeviceCalibration.PatientSide, pDeviceCalibration.ChairId, pDeviceCalibration.BoomId1);
                                if (pdevice != null)
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information - New Controller replaced in an existing Device Calibration of same type", usecasecode = 5, setupid = pdevice.SetupId, TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                                else
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration Information - Considered complete new Setup", usecasecode = 1, setupid = "", TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Unauthorized, result = "Unauthorized User", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "Device Calibration Request is null", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Get Device Calibration failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }
            return Json(new { Status = (int)HttpStatusCode.Created, result = "Device Calibration Input not valid", TimeZone = DateTime.UtcNow.ToString("s") });
        }



        //Using the SetupID, find the Device Calibration record and update it
        [HttpPost]
        [Route("updatedevicecalibration")]
        public JsonResult updatedevicecalibration([FromBody]UpdateDeviceCalibrationView pDeviceCalibration, string sessionid)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("Pain Post Start");
                if (string.IsNullOrEmpty(sessionid))
                {
                    return Json(new { Status = (int)HttpStatusCode.Accepted, result = "The Session Id should not be null", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                if (pDeviceCalibration != null)
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
                    User luser = lIUserRepository.getUserbySessionId(sessionid);
                    if (luser != null)
                    {
                        DeviceCalibration ldevice = lIDeviceCalibrationRepository.getDeviceCalibration(pDeviceCalibration.SetupId);
                        if (ldevice != null)
                        {
                            //if (!string.IsNullOrEmpty(pDeviceCalibration.TabletId))
                            ldevice.TabletId = pDeviceCalibration.TabletId;
                            //if (!string.IsNullOrEmpty(pDeviceCalibration.MacAddress))
                            ldevice.MacAddress = pDeviceCalibration.MacAddress;
                            //if (!string.IsNullOrEmpty(pDeviceCalibration.ChairId))
                            ldevice.ChairId = ldevice.ChairId;
                            //if (!string.IsNullOrEmpty(pDeviceCalibration.BoomId1))
                            ldevice.BoomId1 = pDeviceCalibration.BoomId1;
                            //if (!string.IsNullOrEmpty(pDeviceCalibration.BoomId2))
                            ldevice.BoomId2 = pDeviceCalibration.BoomId2;
                            //if (!string.IsNullOrEmpty(pDeviceCalibration.BoomId3))
                            ldevice.BoomId3 = pDeviceCalibration.BoomId3;
                            //if (!string.IsNullOrEmpty(pDeviceCalibration.EquipmentType))
                            ldevice.EquipmentType = pDeviceCalibration.EquipmentType;
                            //if (!string.IsNullOrEmpty(pDeviceCalibration.DeviceConfiguration))
                            ldevice.DeviceConfiguration = pDeviceCalibration.DeviceConfiguration;
                            //if (!string.IsNullOrEmpty(pDeviceCalibration.PatientSide))
                            ldevice.PatientSide = pDeviceCalibration.PatientSide;


                            //if (pDeviceCalibration.Actuator1RetractedAngle > 0)
                            ldevice.Actuator1RetractedAngle = pDeviceCalibration.Actuator1RetractedAngle;
                            //if (pDeviceCalibration.Actuator1RetractedPulse > 0)
                            ldevice.Actuator1RetractedPulse = pDeviceCalibration.Actuator1RetractedPulse;
                            //if (pDeviceCalibration.Actuator1ExtendedAngle > 0)
                            ldevice.Actuator1ExtendedAngle = pDeviceCalibration.Actuator1ExtendedAngle;
                            //if (pDeviceCalibration.Actuator1ExtendedPulse > 0)
                            ldevice.Actuator1ExtendedPulse = pDeviceCalibration.Actuator1ExtendedPulse;
                            //if (pDeviceCalibration.Actuator1NeutralAngle > 0)
                            ldevice.Actuator1NeutralAngle = pDeviceCalibration.Actuator1NeutralAngle;
                            //if (pDeviceCalibration.Actuator1NeutralPulse > 0)
                            ldevice.Actuator1NeutralPulse = pDeviceCalibration.Actuator1NeutralPulse;

                            //if (pDeviceCalibration.Actuator2RetractedAngle != null)
                            ldevice.Actuator2RetractedAngle = pDeviceCalibration.Actuator2RetractedAngle;
                            //if (pDeviceCalibration.Actuator2RetractedPulse != null)
                            ldevice.Actuator2RetractedPulse = pDeviceCalibration.Actuator2RetractedPulse;
                            //if (pDeviceCalibration.Actuator2ExtendedAngle != null)
                            ldevice.Actuator2ExtendedAngle = pDeviceCalibration.Actuator2ExtendedAngle;
                            //if (pDeviceCalibration.Actuator2ExtendedPulse != null)
                            ldevice.Actuator2ExtendedPulse = pDeviceCalibration.Actuator2ExtendedPulse;
                            //if (pDeviceCalibration.Actuator2NeutralAngle != null)
                            ldevice.Actuator2NeutralAngle = pDeviceCalibration.Actuator2NeutralAngle;
                            //if (pDeviceCalibration.Actuator2NeutralPulse != null)
                            ldevice.Actuator2NeutralPulse = pDeviceCalibration.Actuator2NeutralPulse;

                            //if (pDeviceCalibration.Actuator3RetractedAngle != null)
                            ldevice.Actuator3RetractedAngle = pDeviceCalibration.Actuator3RetractedAngle;
                            //if (pDeviceCalibration.Actuator3RetractedPulse != null)
                            ldevice.Actuator3RetractedPulse = pDeviceCalibration.Actuator3RetractedPulse;
                            //if (pDeviceCalibration.Actuator3ExtendedAngle != null)
                            ldevice.Actuator3ExtendedAngle = pDeviceCalibration.Actuator3ExtendedAngle;
                            //if (pDeviceCalibration.Actuator3ExtendedPulse != null)
                            ldevice.Actuator3ExtendedPulse = pDeviceCalibration.Actuator3ExtendedPulse;
                            //if (pDeviceCalibration.Actuator3NeutralAngle != null)
                            ldevice.Actuator3NeutralAngle = pDeviceCalibration.Actuator3NeutralAngle;
                            //if (pDeviceCalibration.Actuator3NeutralPulse != null)
                            ldevice.Actuator3NeutralPulse = pDeviceCalibration.Actuator3NeutralPulse;

                            //if (!string.IsNullOrEmpty(pDeviceCalibration.InstallerId))
                            ldevice.InstallerId = pDeviceCalibration.InstallerId;

                            ldevice.InActive = pDeviceCalibration.InActive;
                            ldevice.UpdatedDate = DateTime.UtcNow;
                            //if (!string.IsNullOrEmpty(pDeviceCalibration.UpdatePending))
                            ldevice.UpdatePending = pDeviceCalibration.UpdatePending;
                            //if (!string.IsNullOrEmpty(pDeviceCalibration.NewControllerId))
                            ldevice.NewControllerId = pDeviceCalibration.NewControllerId;

                            //if (!string.IsNullOrEmpty(pDeviceCalibration.Description))
                            ldevice.Description = pDeviceCalibration.Description;
                            //if (pDeviceCalibration.Latitude != null)
                            ldevice.Latitude = pDeviceCalibration.Latitude;
                            //if (pDeviceCalibration.Longitude != null)
                            ldevice.Longitude = pDeviceCalibration.Longitude;

                            lIDeviceCalibrationRepository.UpdateDeviceCalibration(ldevice);

                            return Json(new { Status = (int)HttpStatusCode.OK, result = "Device Calibration updated successfully", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                        else
                        {
                            return Json(new { Status = (int)5, result = "Device Calibration record not found - Invalid SetupId", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Unauthorized, result = "Unauthorized User", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "Device Calibration Input not valid", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Device Calibration update failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }

        }

        // called to download the device calibration data, if exist return it
        [HttpGet]
        [Route("downloaddevicecalibration")]
        public JsonResult downloaddevicecalibration(string controllerid)
        {
            try
            {
                if (!string.IsNullOrEmpty(controllerid))
                {
                    DeviceCalibration _result = lIDeviceCalibrationRepository.getDeviceCalibrationbyControllerId(controllerid);
                    if (_result != null)
                    {
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", Calibration = _result, TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Redirect, result = "Device calibration is not exist", TimeZone = DateTime.UtcNow.ToString("s") });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Accepted, result = "request string is not proper", TimeZone = DateTime.UtcNow.ToString("s") });
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



























