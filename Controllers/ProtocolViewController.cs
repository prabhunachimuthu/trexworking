using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Models;
using OneDirect.Repository.Interface;
using Microsoft.Extensions.Logging;
using OneDirect.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OneDirect.ViewModels;
using OneDirect.Extensions;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class ProtocolViewController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly IProtocolInterface lIProtocolInterface;
        private readonly ILogger logger;
        private OneDirectContext context;

        public ProtocolViewController(OneDirectContext context, ILogger<ProtocolViewController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
            lIProtocolInterface = new ProtocolRepository(context);
        }


        // GET: /<controller>/
        public IActionResult Index(string id, string Username, string equipmentid = "", string rxid = "")
        {
            var response = new Dictionary<string, object>();
            try
            {
                ViewBag.PatientName = Username;
                ViewBag.patientId = id;
                if (!string.IsNullOrEmpty(equipmentid))
                {
                    ViewBag.EquipmentType = equipmentid;
                }
                if (!string.IsNullOrEmpty(rxid))
                {
                    ViewBag.RxId = rxid;
                }
                logger.LogDebug("Pain Post Start");
                if (!String.IsNullOrEmpty(Username))
                    HttpContext.Session.SetString("PatientName", Username);
                if (HttpContext.Session.GetString("PatientName") != null && HttpContext.Session.GetString("PatientName").ToString() != "")
                {
                    ViewBag.PatientName = HttpContext.Session.GetString("PatientName").ToString();
                }
                List<Protocol> pProtocol = lIProtocolInterface.getProtocolList(id);
                ViewBag.ProtocolList = pProtocol;
               
                return View();

            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
        }
        public IActionResult Add(string id, string equipmentid = "", string rxid = "")
        {
            List<SelectListItem> myList = new List<SelectListItem>()
             {
                 new SelectListItem{ Value="1",Text="Ankle Unit"},
                 new SelectListItem{ Value="2",Text="Knee Unit"},
                 new SelectListItem{ Value="3",Text="Elbow Unit"},
                 new SelectListItem{ Value="4",Text="Shoulder Unit"},
             };

            ViewBag.equipment = myList;

            ViewBag.Patient = id;

            List<User> _userTherapistlist = lIUserRepository.getUserListByType(2);

            var ObjTherapistList = _userTherapistlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });
            ViewBag.Therapists = new SelectList(ObjTherapistList, "Value", "Text");
            ProtocolView lprotocol = new ProtocolView();
            lprotocol.PatientId = id;
           
            if (HttpContext.Session.GetString("UserType") == "3")
            {
                lprotocol.TherapistId = "the1";
                if (!string.IsNullOrEmpty(equipmentid))
                {
                    lprotocol.EquipmentType = int.Parse(equipmentid);
                }
                if (!string.IsNullOrEmpty(rxid))
                {
                    lprotocol.RxId = rxid;
                }
                lprotocol.Actuator = 1;
            }
            if (HttpContext.Session.GetString("UserId") != null && HttpContext.Session.GetString("UserType") != null && HttpContext.Session.GetString("UserType") == "2")
            {
                lprotocol.TherapistId = HttpContext.Session.GetString("UserId");
            }
            return View(lprotocol);
        }
        [HttpPost]
        public IActionResult Add(ProtocolView pProtocol)
        {
            Protocol _protocol = UserExtension.ProtocolViewToProtocol(pProtocol);
            pProtocol.ProtocolId = Guid.NewGuid().ToString();
            pProtocol.Time = DateTime.Now;
            string _User = lIProtocolInterface.InsertProtocol(_protocol);
            return RedirectToAction("Index", new { id = pProtocol.PatientId.Trim(), Username = HttpContext.Session.GetString("PatientName").ToString() });

        }
        public IActionResult Profile(string id)
        {
            Protocol pProtocol = lIProtocolInterface.getProtocol(id);
            List<SelectListItem> myList = new List<SelectListItem>()
             {
                 new SelectListItem{ Value="1",Text="Ankle Unit"},
                 new SelectListItem{ Value="2",Text="Knee Unit"},
                 new SelectListItem{ Value="3",Text="Elbow Unit"},
                 new SelectListItem{ Value="4",Text="Shoulder Unit"},
             };

            ViewBag.equipment = myList;

            ViewBag.PatientId = pProtocol.PatientId;
            List<User> _userPatientlist = lIUserRepository.getUserListByType(1);

            var ObjList = _userPatientlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });
            ViewBag.Patient = new SelectList(ObjList, "Value", "Text");
           
            List<User> _userTherapistlist = lIUserRepository.getUserListByType(2);

            var ObjTherapistList = _userTherapistlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });
            ViewBag.Therapists = new SelectList(ObjTherapistList, "Value", "Text");
            
            ProtocolView _protocol = UserExtension.ProtocolToProtocolView(pProtocol);
            return View(_protocol);
        }
        [HttpPost]
        public IActionResult Profile(ProtocolView pProtocol)
        {
            Protocol _protocol = UserExtension.ProtocolViewToProtocol(pProtocol);
            string _result = lIProtocolInterface.UpdateProtocol(_protocol);

            return RedirectToAction("Index", new { id = pProtocol.PatientId.Trim(), Username = HttpContext.Session.GetString("PatientName").ToString() });


        }
        public IActionResult New(string id)
        {
            List<SelectListItem> myList = new List<SelectListItem>()
             {
                 new SelectListItem{ Value="1",Text="Ankle Unit"},
                 new SelectListItem{ Value="2",Text="Knee Unit"},
                 new SelectListItem{ Value="3",Text="Elbow Unit"},
                 new SelectListItem{ Value="4",Text="Shoulder Unit"},
             };

            ViewBag.equipment = myList;
            return View(new ProtocolView());
        }


        public IActionResult Delete(string id)
        {
            Protocol pProtocol = lIProtocolInterface.getProtocol(id);
            if (pProtocol != null)
            {
                lIProtocolInterface.DeleteProtocol(pProtocol);
            }
            return RedirectToAction("Index", new { id = pProtocol.PatientId.ToString().Trim(), Username = HttpContext.Session.GetString("PatientName").ToString() });
        }
    }
}
