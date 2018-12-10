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
using Microsoft.AspNetCore.Mvc.Rendering;
using OneDirect.Extensions;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class PatientRxController : Controller
    {

        private readonly IUserInterface lIUserRepository;
        private readonly IPatientRxInterface lIPatientRxRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public PatientRxController(OneDirectContext context, ILogger<PatientController> plogger, IPatientRxInterface IPatientRxRepository)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
            lIPatientRxRepository = IPatientRxRepository;
        }


        //
        // GET: /<controller>/
        public IActionResult Index()
        {
            List<PatientRx> PatientRxList = null;
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                {
                    PatientRxList = lIPatientRxRepository.getByProviderId(HttpContext.Session.GetString("UserId"));
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Patent Rx Error: " + ex);
            }
            return View(PatientRxList);

        }

        //
        public IActionResult AddEdit(string id)
        {
            PatientRxView lPatientRx = new PatientRxView();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    getDetails();
                    PatientRx patientRx = lIPatientRxRepository.getById(id);
                    if (patientRx != null)
                        lPatientRx = patientRx.PatientRxToPatientRxViewModel();
                }
                else
                {
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                    {
                        lPatientRx.ProviderId = HttpContext.Session.GetString("UserId");
                        lPatientRx.RxDays = new List<checkboxModel>
                        {
                             new checkboxModel{id = 1, name = "SUN", isCheck = false},
                             new checkboxModel{id = 2, name = "MON", isCheck = false},
                             new checkboxModel{id = 3, name = "TUE", isCheck = false},
                             new checkboxModel{id = 4, name = "WED", isCheck = false},
                             new checkboxModel{id = 5, name = "THR", isCheck = false},
                             new checkboxModel{id = 6, name = "FRI", isCheck = false},
                             new checkboxModel{id = 7, name = "SAT", isCheck = false}
                        };
                        getDetails();
                    }
                    else
                    {
                        ViewBag.equipment = null;
                        ViewBag.Patients = null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Patent Rx Error: " + ex);
            }
            return View(lPatientRx);
        }

        //
        [HttpPost]
        public IActionResult AddEdit(PatientRxView pPatientRx)
        {
            try
            {
                if (pPatientRx != null && (!string.IsNullOrEmpty(pPatientRx.ProviderId)))
                {
                    if (ModelState.IsValid)
                    {
                        if (!string.IsNullOrEmpty(pPatientRx.RxId))
                        {
                            pPatientRx.DateModified = DateTime.Now;
                            PatientRx lpatientRx = PatientRxExtension.PatientRxViewToPatientRxModel(pPatientRx);

                            User lUser = lIUserRepository.getUser(lpatientRx.PatientId.ToString());
                            if (lUser != null && lpatientRx.Patient != null)
                            {
                                
                                lIPatientRxRepository.UpdatePatientRx(lpatientRx);
                            }
                            else
                            {
                                lIPatientRxRepository.UpdatePatientRx(lpatientRx);
                            }

                            return RedirectToAction("Dashboard", "Provider", new { id = lpatientRx.ProviderId });
                        }
                        else
                        {
                            PatientRx lpatientRx = lIPatientRxRepository.getByPatientIdAndEquipmentTypeAndProviderId(pPatientRx.ProviderId, pPatientRx.PatientId, pPatientRx.EquipmentType.Trim());

                            if (lpatientRx == null)
                            {
                                pPatientRx.RxId = Guid.NewGuid().ToString();
                                pPatientRx.DateCreated = DateTime.Now;
                                pPatientRx.DateModified = DateTime.Now;


                                lpatientRx = PatientRxExtension.PatientRxViewToPatientRxModel(pPatientRx);
                                User lUser = lIUserRepository.getUser(pPatientRx.PatientId);
                                if (lUser == null && lpatientRx.Patient != null)
                                {
                                   
                                    lIPatientRxRepository.InsertPatientRx(lpatientRx);
                                }
                                else
                                {
                                    lIPatientRxRepository.InsertPatientRx(lpatientRx);
                                }
                                return RedirectToAction("Dashboard", "Provider", new { id = lpatientRx.ProviderId });
                            }
                            else
                            {
                                TempData["msg"] = "<script>alert('Patient already registered with the surgery type');</script>";
                                getDetails();
                            }
                        }
                    }
                    else
                    {
                        getDetails();
                    }
                }
                else
                {
                    getDetails();
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Patent Rx Error: " + ex);
            }
            return View(pPatientRx);
        }

        //delete the patientRx record
        public IActionResult Delete(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    PatientRx patientRx = lIPatientRxRepository.getById(id);
                    if (patientRx != null)
                    {
                        lIPatientRxRepository.DeletePatientRx(patientRx);
                    }

                }

            }
            catch (Exception ex)
            {
                logger.LogDebug("Patent Rx Error: " + ex);
            }
            return RedirectToAction("Index");
        }


        public void getDetails()
        {
            List<SelectListItem> myList = new List<SelectListItem>()
                         {
                            new SelectListItem{ Value="1",Text="Ankle"},
                            new SelectListItem{ Value="2",Text="Knee"},
                            new SelectListItem{ Value="4",Text="Shoulder"}
                         };

            ViewBag.equipment = myList;

            var Patients = lIUserRepository.getPatientListByProviderId(HttpContext.Session.GetString("UserId")).ToList().OrderBy(r => r.PatientName).Select(r => new SelectListItem
            {
                Value = r.PatientId.ToString(),
                Text = r.PatientName
            });
            ViewBag.Patients = new SelectList(Patients, "Value", "Text");
        }
    }
}
