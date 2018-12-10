using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository;
using OneDirect.Repository.Interface;
using Microsoft.Extensions.Logging;
using OneDirect.Models;
using OneDirect.Helper;
using System.Net;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly IPainInterface lIPainRepository;
        private readonly IPatientRxInterface lIPatientRxRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public UserController(OneDirectContext context, ILogger<UserController> plogger, IPainInterface IPainRepository, IPatientRxInterface lIPatientRx)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
            lIPainRepository = IPainRepository;
            lIPatientRxRepository = lIPatientRx;
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
        // GET api/values/5
        [HttpGet]
        [Route("GetUser")]
        public string GetUser(string patientId, string therapistId)
        {
            string _result = lIUserRepository.getUserdatabyPatientAndtherapist(patientId, therapistId);
            return _result;
        }

        [HttpGet]
        [Route("GetRx")]
        public JsonResult GetRx(string patientId)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                if (!string.IsNullOrEmpty(patientId))
                {
                    List<PatientRx> _result = lIPatientRxRepository.getPatientRxByPatientId(patientId);
                    return Json(new { Status = (int)HttpStatusCode.OK, Rxs = _result });
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.InternalServerError;
                    error.ErrorMessage = "Not found the patient details";
                    response.Add("ErrorResponse", error);
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, response });
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Get Rx Error: " + ex);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = ex.ToString();
                response.Add("ErrorResponse", error);
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, response });
            }
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]User pUser)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("Pain Post Start");

                if (pUser != null && !string.IsNullOrEmpty(pUser.Name))
                {
                    User lUser = new User();
                    lUser.UserId = Guid.NewGuid().ToString();
                    lUser.Type = pUser.Type;
                    lUser.Name = pUser.Name;
                    lUser.Email = pUser.Email;
                    lUser.Phone = pUser.Phone;
                   
                    lUser.Address = pUser.Address;
                    lIUserRepository.InsertUser(lUser);
                    return Ok();
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.InternalServerError;
                    error.ErrorMessage = "User is null";
                    response.Add("ErrorResponse", error);
                    logger.LogDebug("User is null");
                    return new ObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = ex.ToString();
                response.Add("ErrorResponse", error);
                return new ObjectResult(response);
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
