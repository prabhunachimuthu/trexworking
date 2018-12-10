using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository;
using OneDirect.Models;
using Microsoft.Extensions.Logging;
using OneDirect.Repository.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneDirect.ViewModels;
using OneDirect.Extensions;
using Microsoft.AspNetCore.Http;
using OneDirect.Helper;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class PatientController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public PatientController(OneDirectContext context, ILogger<PatientController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
        }



        public IActionResult Index(string id = "", string type = "", string name = "")
        {

            var response = new Dictionary<string, object>();
            try
            {

                if (!String.IsNullOrEmpty(type))
                {
                    ViewBag.Type = type;
                    ViewBag.Name = name;
                }
                string userid = HttpContext.Session.GetString("UserId");
                string _uType = HttpContext.Session.GetString("UserType");
                if (!String.IsNullOrEmpty(id))
                {
                    userid = id;

                }
                if (!String.IsNullOrEmpty(type))
                {
                    _uType = type;
                }
                List<DashboardView> pUsera = null;
                List<User> pUser = new List<Models.User>();
                logger.LogDebug("Pain Post Start");

                if (_uType == ConstantsVar.Admin.ToString() || _uType == ConstantsVar.Support.ToString())
                {
                    pUsera = lIUserRepository.getDashboardForSupport();
                }
                else if (_uType == ConstantsVar.Provider.ToString())
                {
                    pUsera = lIUserRepository.getDashboard(id);

                }
                else if (_uType == ConstantsVar.PatientAdministrator.ToString())
                {
                    pUsera = lIUserRepository.getDashboardForPatientAdmin(id);

                }
                else if (_uType == ConstantsVar.Therapist.ToString())
                {
                    pUsera = lIUserRepository.getDashboardForTherapist(id);

                }
                else
                {
                    pUsera = lIUserRepository.getDashboardForSupport();
                }



                return View(pUsera);
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
        }


        // GET: /<controller>/
        //public IActionResult Index(string id = "", string type = "", string name = "")
        //{

        //    var response = new Dictionary<string, object>();
        //    try
        //    {
               
        //        if (!String.IsNullOrEmpty(type))
        //        {
        //            ViewBag.Type = type;
        //            ViewBag.Name = name;
        //        }
        //        string userid = HttpContext.Session.GetString("UserId");
        //        string _uType = HttpContext.Session.GetString("UserType");
        //        if (!String.IsNullOrEmpty(id))
        //        {
        //            userid = id;
                    
        //        }
        //        if (!String.IsNullOrEmpty(type))
        //        {
        //            _uType = type;
        //        }
        //        List<NewPatient> pUsera = null;
        //        List<User> pUser = new List<Models.User>();
        //        logger.LogDebug("Pain Post Start");

        //        if (_uType == ConstantsVar.Admin.ToString() || _uType == ConstantsVar.Support.ToString())
        //        {
        //            pUsera = lIUserRepository.getPatientListByType(ConstantsVar.Admin);
        //        }
        //        else if (_uType == ConstantsVar.Provider.ToString())
        //        {
        //            pUsera = lIUserRepository.getPatientListByProviderId(id);
                   
        //        }
        //        else if (_uType == ConstantsVar.PatientAdministrator.ToString())
        //        {
        //            pUsera = lIUserRepository.getPatientListByPatientAdmin(id);
                  
        //        }
        //        else if (_uType == ConstantsVar.Therapist.ToString())
        //        {
        //            pUsera = lIUserRepository.getUserListByTherapistId(id);
                   
        //        }
        //        else
        //        {
        //            pUsera = lIUserRepository.getPatientListByType(ConstantsVar.Admin);
        //        }

             

        //        return View(pUsera);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogDebug("User Post Error: " + ex);
        //        return null;
        //    }
        //}
        public IActionResult Add()
        {
           

            List<User> _userProviderlist = lIUserRepository.getUserListByType(3);

            var ObjProviderList = _userProviderlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });

            ViewBag.ddProvider = new SelectList(ObjProviderList, "Value", "Text");
            ViewBag.ddProvider = ObjProviderList;

            return View();
        }
        [HttpPost]
        public IActionResult Add(UserViewModel pUser)
        {
            pUser.Type = 1;
            User _user = UserExtension.UserViewModelToUser(pUser);
            _user.Phone = RemoveSpecialChars(_user.Phone);
            string _User = lIUserRepository.InsertUser(_user);
            return RedirectToAction("Index");
        }
        public IActionResult Profile(string id)
        {
            User pUser = lIUserRepository.getUser(id);

            List<User> _userProviderlist = lIUserRepository.getUserListByType(3);

            var ObjProviderList = _userProviderlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });

            ViewBag.ddProvider = new SelectList(ObjProviderList, "Value", "Text");
            ViewBag.ddProvider = ObjProviderList;
            UserViewModel _user = UserExtension.UserToUserViewModel(pUser);
            return View(_user);
        }
        [HttpPost]
        public IActionResult Profile(UserViewModel pUser)
        {
            pUser.Type = 1;
            User _user = UserExtension.UserViewModelToUser(pUser);
            _user.Phone = RemoveSpecialChars(_user.Phone);
            string _result = lIUserRepository.UpdateUser(_user);
            return RedirectToAction("Index");
        }
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
    }
  
}