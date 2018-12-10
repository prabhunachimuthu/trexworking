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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class OrganizationAdministratorController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly IProtocolInterface lIProtocolInterface;
        private readonly IPatientRxInterface lIPatientRxRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public OrganizationAdministratorController(OneDirectContext context, ILogger<OrganizationAdministratorController> plogger, IPatientRxInterface IPatientRxRepository)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
            lIProtocolInterface = new ProtocolRepository(context);
            lIPatientRxRepository = IPatientRxRepository;
        }


        //give the list of organization administrators
        // GET: /<controller>/
        public IActionResult Index()
        {
            var response = new Dictionary<string, object>();
            List<User> pUser = new List<User>();
            try
            {
                logger.LogDebug("Installer Start");

                pUser = lIUserRepository.getUserListByType(ConstantsVar.Admin);
                if (pUser != null && pUser.Count > 0)
                {
                    pUser = pUser.Where(x => x.UserId != "admin").ToList();
                }
                ViewBag.userlist = pUser;
                return View();
            }
            catch (Exception ex)
            {
                logger.LogDebug("Installer Error: " + ex);
                return null;
            }

        }
        public IActionResult Add()
        {
           
            return View();
        }

        // add new record for organization administrator with unique npi and UserId
        [HttpPost]
        public IActionResult Add(UserViewModel pUser)
        {
            pUser.Type = ConstantsVar.Admin;
            User _user = lIUserRepository.getUser(pUser.UserId);
            User _user2 = lIUserRepository.getUserNpi(pUser.Npi);
            User _userNew = UserExtension.UserViewModelToUser(pUser);
            if ((_user == null) && (_user2 == null))
            {
                _userNew.Phone = RemoveSpecialChars(_userNew.Phone);
                string _User = lIUserRepository.InsertUser(_userNew);
                if (_User == "Username already exists")
                {
                    TempData["Admin"] = JsonConvert.SerializeObject(_userNew);
                    TempData["msg"] = "<script>Helpers.ShowMessage('User with same Organization Administrator Id is already registered, please use different one', 1);</script>";
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
                        TempData["Admin"] = JsonConvert.SerializeObject(_userNew);
                        TempData["msg"] = "<script>Helpers.ShowMessage('User with same Npi is already registered, please use different one', 1);</script>";
                        pUser.Npi = null;
                        return View(pUser);
                    }
                }
                if (_user.UserId == _userNew.UserId)
                {
                    TempData["Admin"] = JsonConvert.SerializeObject(_userNew);
                    TempData["msg"] = "<script>Helpers.ShowMessage('User with same Organization Administrator Id is already registered, please use different one', 1);</script>";
                    pUser.UserId = null;
                    return View(pUser);
                }
            }
            return RedirectToAction("Index");
        }

        //get and show the profile of organization administrator
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

        //updating the changes in profile of organization administrator
        [HttpPost]
        public IActionResult Profile(UserViewModel pUser)
        {
            pUser.Type = ConstantsVar.Admin;
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
            if (HttpContext.Session.GetString("UserType") != null && HttpContext.Session.GetString("UserType").ToString() == ConstantsVar.Admin.ToString())
            {
                return RedirectToAction("Profile", new { id = pUser.UserId });
            }
            return RedirectToAction("Index");
        }

        //remove all the special characters from phone number before saving in database
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

        //delete the record of organization administrator
        public IActionResult Delete(string id)
        {
            User pUser = lIUserRepository.getUser(id);
            if (pUser != null)
            {
                lIUserRepository.deleteUser(pUser);

            }
            return RedirectToAction("Index");
        }

    }
}
