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

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    [Route("api/[controller]/[action]")]
    public class PainController : Controller
    {
        private readonly IPatient IPatient;
        private readonly ISessionInterface lISessionRepository;
        private readonly IPainInterface lIPainRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public PainController(OneDirectContext context, ILogger<PainController> plogger, IPainInterface IPainRepository)
        {
            logger = plogger;
            this.context = context;
            IPatient = new PatientRepository(context);
            lISessionRepository = new SessionRepository(context);
            lIPainRepository = IPainRepository;
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


        //Use to download all Pain previously reported by a patient for a Rx
        [HttpGet]
        [ActionName("downloadpainrecords")]
        public JsonResult downloadpainrecords(string sessionid, string rxid)
        {
            try
            {
                if (!string.IsNullOrEmpty(sessionid) && !string.IsNullOrEmpty(rxid))
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null)
                    {
                        List<Pain> _result = lIPainRepository.getPainbyPatientIdAndRxId(lpatient.PatientId, rxid);
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", Pains = _result, TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient is not registered", TimeZone = DateTime.UtcNow.ToString("s") });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "request string is not proper", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }

        //It is used to create new Pain record using sessionID of patient
        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]List<Pain> pPains, string sessionid)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("Pain Post Start");

                if (pPains != null && pPains.Count > 0 && !string.IsNullOrEmpty(sessionid))
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null)
                    {
                        foreach (Pain pPain in pPains)
                        {
                            if (!string.IsNullOrEmpty(pPain.PainId))
                            {
                                if (pPain.PainLevel > 0)
                                {
                                    Pain lPain = lIPainRepository.getPain(pPain.PainId);
                                    if (lPain == null)
                                    {
                                        pPain.PatientId = lpatient.PatientId;
                                        lIPainRepository.InsertPain(pPain);

                                    }
                                    else
                                    {
                                        lPain.PatientId = lpatient.PatientId;
                                        lPain.RxId = pPain.RxId;
                                        lPain.ProtocolId = pPain.ProtocolId;
                                        lPain.SessionId = pPain.SessionId;
                                        lPain.Angle = pPain.Angle;
                                        lPain.RepeatNumber = pPain.RepeatNumber;
                                        lPain.PainLevel = pPain.PainLevel > 0 ? pPain.PainLevel : 0;
                                        lPain.FlexionRepNumber = pPain.FlexionRepNumber;
                                        lPain.ExtensionRepNumber = pPain.ExtensionRepNumber;
                                        lIPainRepository.UpdatePain(pPain);
                                    }
                                }
                                else
                                {
                                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Painlevel should not aceept negative values", TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }
                            else
                            {
                                return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "Pain Request string is not in proper", TimeZone = DateTime.UtcNow.ToString("s") });
                            }
                        }
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "Pain inserted successfully", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient session is not valid", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Pain insertion failed", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {

                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Pain insertion failed", TimeZone = DateTime.UtcNow.ToString("s") });
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
