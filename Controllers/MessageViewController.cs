using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Models;
using OneDirect.Repository.Interface;
using Microsoft.Extensions.Logging;
using OneDirect.Repository;
using Microsoft.AspNetCore.Http;
using OneDirect.ViewModels;
using OneDirect.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class MessageViewController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly IPatient IPatient;
        private readonly IMessageInterface lIMessageRepository;
        private readonly ISessionInterface lISessionRepository;
        private readonly IPainInterface lIPainRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;
        public MessageViewController(OneDirectContext context, ILogger<PainViewController> plogger, IPainInterface IPainRepository)
        {
            logger = plogger;
            lIUserRepository = new UserRepository(context);
            this.context = context;
            IPatient = new PatientRepository(context);
            lIMessageRepository = new MessageRepository(context);
            lISessionRepository = new SessionRepository(context);
            lIPainRepository = IPainRepository;
            lIEquipmentAssignmentRepository = new AssignmentRepository(context);
        }

        //load the message screen with assigned patient's and their messages with particular user
        // GET: /<controller>/
        public IActionResult Index(string patientid = "")
        {

            List<PatientMessage> lpatientList = new List<PatientMessage>();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") != "0")
            {
                if (HttpContext.Session.GetString("UserType") == ConstantsVar.Therapist.ToString())
                {
                    lpatientList = IPatient.GetPatientWithStatusByTherapistId(HttpContext.Session.GetString("UserId")).OrderBy(x => x.Patient.PatientName).ToList();
                }
                else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Provider.ToString())
                {
                    lpatientList = IPatient.GetPatientWithStatusByProviderId(HttpContext.Session.GetString("UserId")).OrderBy(x => x.Patient.PatientName).ToList();
                }
                else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Support.ToString())
                {
                    lpatientList = IPatient.GetAllPatientStatus(HttpContext.Session.GetString("UserId")).OrderBy(x => x.Patient.PatientName).ToList();
                }
                if (lpatientList != null && lpatientList.Count > 0)
                {
                    PatientMessage lpatient = (!string.IsNullOrEmpty(patientid) ? lpatientList.FirstOrDefault(x => x.Patient.PatientLoginId == patientid) : lpatientList.FirstOrDefault());
                    ViewBag.Patient = lpatient.Patient.PatientName;
                    ViewBag.PatientId = lpatient.Patient.PatientLoginId;
                    List<MessageView> lmessages = lIMessageRepository.getMessagesbyTimeZone(lpatient.Patient.PatientLoginId, HttpContext.Session.GetString("UserId"), HttpContext.Session.GetString("timezoneid"));

                    ViewBag.Messages = lmessages.OrderBy(x => x.Datetime);
                }
                ViewBag.PatientList = lpatientList;
            }
            return View();
        }

        //send the message to the patient using patient id by user
        [HttpPost]
        [ActionName("sendmessage")]
        public JsonResult sendmessage(string patientid = "", string message = "")
        {
            JsonResult lJson = null;
            try
            {
                if (!string.IsNullOrEmpty(patientid) && !string.IsNullOrEmpty(message))
                {
                    User luser = lIUserRepository.getUser(HttpContext.Session.GetString("UserId"));
                    if (luser != null)
                    {
                        Messages lmessage = new Messages();
                        lmessage.PatientId = patientid;
                        lmessage.BodyText = message;
                        lmessage.UserId = luser.UserId;
                        lmessage.UserType = luser.Type;
                        lmessage.UserName = luser.Name;
                        lmessage.SentReceivedFlag = 1;
                        lmessage.ReadStatus = 0;
                        lmessage.Datetime = DateTime.Now;
                        int res = lIMessageRepository.InsertMessage(lmessage);
                        if (res > 0)
                        {
                            lmessage.Datetime = Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(lmessage.Datetime, HttpContext.Session.GetString("timezoneid")));
                           
                            JsonSerializerSettings lsetting = new JsonSerializerSettings();
                            lsetting.ContractResolver = new CamelCasePropertyNamesContractResolver();
                            lsetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                            lJson = Json(new { result = "success", message = lmessage }, lsetting);
                            return lJson;
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                return Json("");
            }
            return Json("");
        }

        
        public IActionResult Messages(string patientId = "")
        {
            
            return View();
        }
       
    }
}
