using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository.Interface;
using Microsoft.Extensions.Logging;
using OneDirect.Models;
using OneDirect.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneDirect.ViewModels;
using OneDirect.Extensions;
using System.Data;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Xml.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class EquipmentController : Controller
    {
        private readonly IUserActivityLogInterface lIUserActivityLogRepository;
        private readonly IDeviceCalibrationInterface lIDeviceCalibrationRepository;
        private readonly IAssignmentInterface lIAssignmentInterface;
        private readonly IUserInterface lIUserRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public EquipmentController(OneDirectContext context, ILogger<EquipmentController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIAssignmentInterface = new AssignmentRepository(context);
            lIDeviceCalibrationRepository = new DeviceCalibrationRepository(context);
            lIUserRepository = new UserRepository(context);
            lIUserActivityLogRepository = new UserActivityLogRepository(context);
        }


        // GET: /<controller>/
        public IActionResult Index(string id = "")
        {
            try
            {
                PatientConfigurationResult lresult = new PatientConfigurationResult();
                if (!string.IsNullOrEmpty(id))
                {
                    List<PatientConfigurationDetails> llist = lIDeviceCalibrationRepository.getPatientDeviceCalibrationByInstallerId(id);
                    List<DeviceConfigurationDetails> llist1 = lIDeviceCalibrationRepository.getDeviceCalibrationByInstallerId(id);

                    lresult.patientconfiguration = llist;
                    lresult.devicecalibration = llist1;
                }
                else
                {
                    List<PatientConfigurationDetails> llist = lIDeviceCalibrationRepository.getAllPatientDeviceCalibration();
                    List<DeviceConfigurationDetails> llist1 = lIDeviceCalibrationRepository.getDeviceCalibration();

                    lresult.patientconfiguration = llist;
                    lresult.devicecalibration = llist1;
                }

              
                return View(lresult);
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
        }

        public IActionResult Delete(string id)
        {
            DeviceCalibration ldevice = (from p in context.DeviceCalibration
                                         where p.SetupId == id
                                         select p).FirstOrDefault();
            if (ldevice != null)
            {
                int res = lIDeviceCalibrationRepository.deleteDeviceCalibrationCascade(id);
                if (res > 0)
                {
                    //Insert to User Activity Log
                    UserActivityLog llog = new UserActivityLog();
                    llog.SessionId = HttpContext.Session.GetString("SessionId");
                    llog.ActivityType = "Update";
                    llog.StartTimeStamp = !string.IsNullOrEmpty(HttpContext.Session.GetString("SessionTime")) ? Convert.ToDateTime(HttpContext.Session.GetString("SessionTime")) : DateTime.Now;
                    llog.Duration = Convert.ToInt32((DateTime.Now - Convert.ToDateTime(HttpContext.Session.GetString("SessionTime"))).TotalSeconds);
                    llog.RecordChangeType = "Delete";
                    llog.RecordType = "Equipment";
                    llog.Comment = "Record deleted";
                    llog.RecordJson = JsonConvert.SerializeObject(ldevice);
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
            return RedirectToAction("Index", "Equipment");
        }
        public IActionResult AddEdit(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    DeviceConfigurationDetails ldevice = lIDeviceCalibrationRepository.getDeviceCalibrationbySetupId(id);
                    return View(ldevice);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
            return null;
        }
        [HttpPost]
        public IActionResult AddEdit(DeviceConfigurationDetails pdevice)
        {
            try
            {
                if (pdevice != null && !string.IsNullOrEmpty(pdevice.devicecalibration.SetupId))
                {
                    DeviceCalibration ldevice = lIDeviceCalibrationRepository.getDeviceCalibration(pdevice.devicecalibration.SetupId);
                    if (ldevice != null)
                    {
                        ldevice.Description = pdevice.devicecalibration.Description;
                        lIDeviceCalibrationRepository.UpdateDeviceCalibration(ldevice);
                        return RedirectToAction("Index", "Equipment");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
            return null;
        }
        public IActionResult Add()
        {
            List<SelectListItem> myList = new List<SelectListItem>()
             {
                 new SelectListItem{ Value="1",Text="Ankle Unit"},
                 new SelectListItem{ Value="2",Text="Knee Unit"},
                 new SelectListItem{ Value="3",Text="Elbow Unit"},
                 new SelectListItem{ Value="4",Text="Shoulder Unit"},
             };

            ViewBag.equipment = myList;

            List<User> _userPatientlist = lIUserRepository.getUserListByType(1);

            var ObjList = _userPatientlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });
            ViewBag.Patient = new SelectList(ObjList, "Value", "Text");
            ViewBag.Patient = ObjList;

            List<User> _userTherapistlist = lIUserRepository.getUserListByType(2);

            var ObjTherapistList = _userTherapistlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });
            ViewBag.Therapists = new SelectList(ObjList, "Value", "Text");
            ViewBag.Therapists = ObjTherapistList;

            return View("Add");
        }
        private XElement GetAddress(string address)
        {
           
            string requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();
            httpClient.BaseAddress = new Uri(requestUri);
            string urlParameters = "";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            response = httpClient.GetAsync(urlParameters).Result;
            XElement resultEle = null;
            if (response.StatusCode.ToString().ToLower() == "ok")
            {
                var ss = response.Content.ReadAsStringAsync().Result.ToString();
                var xdoc = XDocument.Parse(ss);
                foreach (XElement element in xdoc.Descendants("location"))
                {
                    resultEle = element;
                    if (resultEle != null)
                        break;
                }
            }
            return resultEle;
        }
        [HttpPost]
        public IActionResult Add(EquipmentView pEquipmentAssignment)
        {

            XElement ele = GetAddress(pEquipmentAssignment.Address);
            if (ele != null)
            {
                pEquipmentAssignment.Latitude = ele.Element("lat").FirstNode.ToString();
                pEquipmentAssignment.Longitude = ele.Element("lng").FirstNode.ToString();
            }
            EquipmentAssignment pequipment = UserExtension.EquipmentViewToEquipment(pEquipmentAssignment);
            string str = lIAssignmentInterface.InsertEquipmentAssignment(pequipment);
            return RedirectToAction("Index");

        }
        public IActionResult Profile(string id)
        {
            EquipmentAssignment pUser = lIAssignmentInterface.getEquipmentAssignment(id);

           
            List<SelectListItem> myList = new List<SelectListItem>()
             {
                 new SelectListItem{ Value="1",Text="Ankle Unit"},
                 new SelectListItem{ Value="2",Text="Knee Unit"},
                 new SelectListItem{ Value="3",Text="Elbow Unit"},
                 new SelectListItem{ Value="4",Text="Shoulder Unit"},
             };

            ViewBag.equipment = myList;

            List<User> _userPatientlist = lIUserRepository.getUserListByType(1);

            var ObjList = _userPatientlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });
            ViewBag.Patient = new SelectList(ObjList, "Value", "Text");
            ViewBag.Patient = ObjList;

            List<User> _userTherapistlist = lIUserRepository.getUserListByType(2);

            var ObjTherapistList = _userTherapistlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });
            ViewBag.Therapists = new SelectList(ObjList, "Value", "Text");
            ViewBag.Therapists = ObjTherapistList;
            EquipmentView pequipment = UserExtension.EquipmentToEquipmentAssignmentExtension(pUser);
            return View(pequipment);
        }
        [HttpPost]
        public IActionResult Profile(EquipmentView pEquipmentAssignment)
        {
            XElement ele = GetAddress(pEquipmentAssignment.Address);
            if (ele != null)
            {
                pEquipmentAssignment.Latitude = ele.Element("lat").FirstNode.ToString();
                pEquipmentAssignment.Longitude = ele.Element("lng").FirstNode.ToString();
            }
            EquipmentAssignment pequipment = UserExtension.EquipmentViewToEquipment(pEquipmentAssignment);
            string _result = lIAssignmentInterface.UpdateEquipmentAssignment(pequipment);
            return RedirectToAction("Index");
        }
    }
}
