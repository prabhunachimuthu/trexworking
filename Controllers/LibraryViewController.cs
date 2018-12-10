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
using CsvHelper;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneDirect.Extensions;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class LibraryViewController : Controller
    {
        private readonly IPatient IPatient;
        private readonly ILibraryInterface lILibraryRepository;
        private readonly IPatientLibraryInterface lIPatientLibraryRepository;
        private readonly IEquipmentExerciseInterface lEquipmentExerciseRepository;
        private readonly ISessionInterface lISessionRepository;
        private readonly IPainInterface lIPainRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;
        public LibraryViewController(OneDirectContext context, ILogger<PainViewController> plogger, IPainInterface IPainRepository, ILibraryInterface ILibraryRepository, IEquipmentExerciseInterface EquipmentExerciseRepository, IPatientLibraryInterface IPatientLibraryRepository)
        {
            logger = plogger;
            this.context = context;
            lISessionRepository = new SessionRepository(context);
            lIPainRepository = IPainRepository;
            lIEquipmentAssignmentRepository = new AssignmentRepository(context);
            lILibraryRepository = ILibraryRepository;
            lIPatientLibraryRepository = IPatientLibraryRepository;
            lEquipmentExerciseRepository = EquipmentExerciseRepository;
            IPatient = new PatientRepository(context);
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            List<Library> lLibrary = lILibraryRepository.getLibrary();

            return View(lLibrary);
        }

        public IActionResult AddEdit(int id)
        {
            try
            {
                LibraryView lLibrary = new LibraryView();
                List<EquipmentExercise> lprocol = lEquipmentExerciseRepository.GetEquipmentExerciseString();
                if (lprocol != null && lprocol.Count > 0)
                {
                    var ObjList = lprocol.Select(x => x.Limb).Distinct().Select(r => new SelectListItem
                    {
                        Value = r,
                        Text = r
                    });
                    ViewBag.Limb = new SelectList(ObjList, "Value", "Text");

                    var ObjListExercise = lprocol.Select(x => x.ExerciseEnum).Distinct().Select(r => new SelectListItem
                    {
                        Value = r,
                        Text = r
                    });
                    ViewBag.Exercise = new SelectList(ObjListExercise, "Value", "Text");


                    ViewBag.ProtocolList = lprocol;
                }

                if (id == 0)
                {
                    return View();
                }
                else
                {
                    Library pLibrary = lILibraryRepository.getLibraryById(Convert.ToInt32(id));
                    if (pLibrary != null)
                    {
                        var ObjListExercise = lprocol.Where(x => x.Limb == pLibrary.Limb).Select(x => x.ExerciseEnum).Distinct().Select(r => new SelectListItem
                        {
                            Value = r,
                            Text = r
                        });
                        ViewBag.Exercise = new SelectList(ObjListExercise, "Value", "Text");

                        lLibrary = LibraryExtension.LibraryToLibraryView(pLibrary);
                    }
                }
                return View(lLibrary);

            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);

            }
            return View();
        }

        [HttpPost]
        public IActionResult AddEdit(LibraryView pLibrary)
        {
            try
            {
                if (pLibrary != null)
                {
                    Library library = new Library();
                    library = LibraryExtension.LibraryViewToLibrary(pLibrary);

                    if (library.Id > 0)
                    {
                        lILibraryRepository.UpdateLibrary(library);
                    }
                    else
                    {
                        lILibraryRepository.InsertLibrary(library);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);

            }
            return RedirectToAction("Index", "LibraryView");
        }


        public IActionResult Delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    lILibraryRepository.DeleteLibrary(id);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            return RedirectToAction("Index", "LibraryView");

        }
        public IActionResult Deactivate(int id, int patId, string equipmentType, string actuator, string returnView, int patientlibraryid = 0)
        {
            try
            {
                if (id > 0 && patientlibraryid == 0 && patId > 0 && !string.IsNullOrEmpty(equipmentType) && !string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(returnView) && !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
                {
                    Library lLibrary = lILibraryRepository.getLibraryById(id);
                    Patient lpatient = IPatient.GetPatientByPatientID(patId);
                    if (lLibrary != null && lpatient != null)
                    {
                        PatientLibrary lPatientLibrary = new PatientLibrary();
                        lPatientLibrary.UserId = HttpContext.Session.GetString("UserId");
                        lPatientLibrary.Type = HttpContext.Session.GetString("UserType");
                        lPatientLibrary.Limb = lpatient.EquipmentType;
                        lPatientLibrary.Side = lpatient.Side;
                        lPatientLibrary.Exercise = actuator;
                        lPatientLibrary.Status = 1;
                        lPatientLibrary.Url = lLibrary.Url;
                        lPatientLibrary.Patient = lpatient.PatientLoginId;
                        lPatientLibrary.LibraryId = lLibrary.Id;
                        lIPatientLibraryRepository.InsertPatientLibrary(lPatientLibrary);

                        return RedirectToAction("Index", "Review", new { id = lpatient.PatientId, Username = lpatient.PatientName, EquipmentType = lpatient.EquipmentType, actuator = actuator, tab = "Library" });
                    }
                }
                else if (id > 0 && patientlibraryid > 0 && patId > 0 && !string.IsNullOrEmpty(equipmentType) && !string.IsNullOrEmpty(actuator) && !string.IsNullOrEmpty(returnView) && !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
                {
                    Library lLibrary = lILibraryRepository.getLibraryById(id);
                    Patient lpatient = IPatient.GetPatientByPatientID(patId);
                    PatientLibrary lpatientLibrary = lIPatientLibraryRepository.getPatientLibraryById(patientlibraryid);
                    if (lLibrary != null && lpatient != null && lpatientLibrary != null)
                    {
                        lIPatientLibraryRepository.DeletePatientLibrary(patientlibraryid);

                        return RedirectToAction("Index", "Review", new { id = lpatient.PatientId, Username = lpatient.PatientName, EquipmentType = lpatient.EquipmentType, actuator = actuator, tab = "Library" });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            return RedirectToAction("Index", "LibraryView");
        }

    }
}
