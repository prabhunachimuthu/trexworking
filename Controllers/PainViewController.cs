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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [TypeFilter(typeof(LoginAuthorizeAttribute))]
    public class PainViewController : Controller
    {
        private readonly ISessionInterface lISessionRepository;
        private readonly IPainInterface lIPainRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;
        public PainViewController(OneDirectContext context, ILogger<PainViewController> plogger, IPainInterface IPainRepository)
        {
            logger = plogger;
            this.context = context;
            lISessionRepository = new SessionRepository(context);
            lIPainRepository = IPainRepository;
            lIEquipmentAssignmentRepository = new AssignmentRepository(context);
        }
        // GET: /<controller>/
        public IActionResult Index(string sessionId, string date, string time, string protocol = "", string returnView = "")
        {
            ViewBag.date = date;
            ViewBag.time = time;
            ViewBag.sessionId = sessionId;
            if (!string.IsNullOrEmpty(returnView))
            {
                ViewBag.returnView = returnView;
            }
            if (!String.IsNullOrEmpty(date))
                //setting date in sessions
                HttpContext.Session.SetString("sessionDate", Convert.ToDateTime(date).ToString("MMM-dd-yyy hh:mm:ss"));
            if (!String.IsNullOrEmpty(time))
                //setting sessionTime in sessions 
                HttpContext.Session.SetString("sessionTime", time);
            //get the sessions using SessionId
            UserSession _lsession = lISessionRepository.getSessionbySessionId(sessionId);
            //getting the list of pain using session id
            List<SessionPain> _lpain = lIPainRepository.getPainBySessionId(sessionId);
            ViewBag.SessionList = _lsession;
            ViewBag.PainList = _lpain;
            return View();
        }

        public IActionResult AddEdit(string id, string painId = "", string date = "", string time = "", string returnView = "")
        {
            try
            {
                ViewBag.date = date;
                ViewBag.time = time;
                ViewBag.returnView = returnView;
                ViewBag.sessionId = id;
                if (string.IsNullOrEmpty(painId))
                {
                    Session lSession = lISessionRepository.getSession(id);

                    if (lSession != null)
                    {
                        SessionPain lpain = lIPainRepository.getSessionDetailsForPainAdd(lSession.SessionId);
                        if (lpain != null)
                        {
                            lpain.date = date;
                            lpain.time = time;
                            lpain.returnView = returnView;
                        }
                        return View(lpain);
                    }
                }
                else
                {

                    SessionPain lpain = lIPainRepository.getPainByPainId(painId);
                    if (lpain != null)
                    {
                        lpain.date = date;
                        lpain.time = time;
                        lpain.returnView = returnView;
                    }
                    return View(lpain);

                }

            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);

            }
            return View();
        }

        [HttpPost]
        public IActionResult AddEdit(SessionPain pPain)
        {
            try
            {
                if (pPain != null)
                {
                    Pain lPain = new Pain();
                    lPain.PatientId = pPain.PatientId;
                    lPain.RxId = pPain.RxId;
                    lPain.ProtocolId = pPain.ProtocolId;
                    lPain.SessionId = pPain.SessionId;
                    lPain.PainId = pPain.PainId;
                    lPain.Angle = pPain.Angle;
                    lPain.RepeatNumber = pPain.RepeatNumber;
                    lPain.PainLevel = pPain.PainLevel;
                    lPain.FlexionRepNumber = pPain.FlexionRepNumber;
                    lPain.ExtensionRepNumber = pPain.ExtensionRepNumber;

                    if (string.IsNullOrEmpty(lPain.PainId))
                    {
                        lPain.PainId = Guid.NewGuid().ToString();
                        lIPainRepository.InsertPain(lPain);
                    }
                    else
                    {
                        lPain.PainId = pPain.PainId;
                        //Pain editPain = lIPainRepository.getPain(pPain.PainId);
                        //if (editPain != null)
                        //{
                        lIPainRepository.UpdatePain(lPain);
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);

            }
            return RedirectToAction("Index", "PainView", new { sessionId = pPain.SessionId, date = pPain.date, time = pPain.time, returnView = pPain.returnView });
        }


        public IActionResult Delete(string painId, string sessionId = "", string date = "", string time = "", string returnView = "", string patId = "", string patName = "", string eType = "", string actuator = "")
        {

            try
            {
                //using pain id remove the pain record
                string result = lIPainRepository.DeletePain(painId);
            }
            catch (Exception ex)
            {
                logger.LogDebug("Error: " + ex);
            }
            if (!string.IsNullOrEmpty(returnView) && !string.IsNullOrEmpty(actuator))
            {
                return RedirectToAction("Index", "PainView", new { sessionId = sessionId, date = date, time = time, returnView = returnView });
            }
            else
            {
                return RedirectToAction("Index", "PainView", new { sessionId = sessionId, date = date, time = time });
            }

        }

        [HttpPost]
        public IActionResult ImportPains(IFormFile fileInput, string id, string date = "", string time = "", string returnView = "")
        {
            try
            {
                if (fileInput == null || fileInput.Length == 0)
                {
                    TempData["msg"] = "<script>Helpers.ShowMessage('File is not selected', 1);</script>";
                }
                else if (fileInput != null && !string.IsNullOrEmpty(fileInput.FileName.Trim()))
                {
                    if (fileInput.FileName.Remove(0, fileInput.FileName.LastIndexOf(".") + 1) != "csv")
                    {
                        TempData["msg"] = "<script>Helpers.ShowMessage('Invalid csv file for Pain upload', 1);</script>";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            List<PainImport> lpains = new List<PainImport>();
                            using (var reader = new StreamReader(fileInput.OpenReadStream()))
                            {
                                using (var csv = new CsvReader(reader))
                                {
                                    csv.Configuration.IgnoreQuotes = true;
                                    csv.Configuration.IgnoreReferences = true;
                                    csv.Configuration.DetectColumnCountChanges = true;
                                    csv.Configuration.IgnoreBlankLines = true;
                                    lpains = csv.GetRecords<PainImport>().ToList();
                                }
                            }
                            if (lpains != null && lpains.Count > 0)
                            {
                                Session lsession = lISessionRepository.getSession(id);
                                if (lsession != null)
                                {
                                    for (int i = 0; i < lpains.Count; i++)
                                    {
                                        Pain pPain = new Pain();
                                        pPain.PainId = Guid.NewGuid().ToString();
                                        pPain.PatientId = lsession.PatientId;
                                        pPain.RxId = lsession.RxId;
                                        pPain.ProtocolId = lsession.ProtocolId;
                                        pPain.SessionId = lsession.SessionId;
                                        pPain.Angle = !string.IsNullOrEmpty(lpains[i].Angle) ? Convert.ToInt32(lpains[i].Angle) : (int?)null;
                                        pPain.RepeatNumber = !string.IsNullOrEmpty(lpains[i].RepeatNumber) ? Convert.ToInt32(lpains[i].RepeatNumber) : (int?)null;
                                        pPain.PainLevel = !string.IsNullOrEmpty(lpains[i].PainLevel) ? Convert.ToInt32(lpains[i].PainLevel) : (int?)null;
                                        pPain.FlexionRepNumber = !string.IsNullOrEmpty(lpains[i].FlexionRepNumber) ? Convert.ToInt32(lpains[i].FlexionRepNumber) : (int?)null;
                                        pPain.ExtensionRepNumber = !string.IsNullOrEmpty(lpains[i].ExtensionRepNumber) ? Convert.ToInt32(lpains[i].ExtensionRepNumber) : (int?)null;

                                        lIPainRepository.InsertPain(pPain);
                                    }
                                }
                                else
                                {
                                    TempData["msg"] = "<script>Helpers.ShowMessage('Session details is not found', 1);</script>";
                                }
                            }

                        }
                    }
                }
                else
                {
                    TempData["msg"] = "<script>Helpers.ShowMessage('Please attach a file for Pain Upload', 1);</script>";
                }

            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);

            }

            return RedirectToAction("Index", "PainView", new { sessionId = id, date = date, time = time, returnView = returnView });
        }
    }
}
