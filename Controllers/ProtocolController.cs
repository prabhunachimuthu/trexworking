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
using OneDirect.Response;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    public class ProtocolController : Controller
    {
        private readonly IProtocolInterface lIProtocolRepository;
        private readonly IPainInterface lIPainRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public ProtocolController(OneDirectContext context, ILogger<ProtocolController> plogger, IPainInterface IPainRepository)
        {
            logger = plogger;
            this.context = context;
            lIProtocolRepository = new ProtocolRepository(context);
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
        public string Get(string id)
        {
            string _result = string.Empty;
            ProtocolList _protocolList = new ProtocolList();
            _protocolList.Protocol = new List<Protocol>();
            try
            {
                List<Protocol> lProtocollist = lIProtocolRepository.getMobileProtocol(id);
                if (lProtocollist != null && lProtocollist.Count > 0)
                {
                    _protocolList.Protocol = lProtocollist;
                    _protocolList.result = "success";
                    _result = Newtonsoft.Json.JsonConvert.SerializeObject(_protocolList);
                }
                else
                {
                    _protocolList.result = "success";
                    _result = Newtonsoft.Json.JsonConvert.SerializeObject(_protocolList);
                }
            }
            catch (Exception ex)
            {
                _protocolList.result = "failed";
                _result = Newtonsoft.Json.JsonConvert.SerializeObject(_protocolList);
            }
            return _result;
        }


        //create new protocol records and update the existing records
        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]List<Protocol> pProtocolList)
        {
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("Pain Post Start");

                if (pProtocolList != null && pProtocolList.Count > 0)
                {
                    foreach (Protocol pProtocol in pProtocolList)
                    {
                       
                        Protocol lProtocol = lIProtocolRepository.getProtocol(pProtocol.ProtocolId);
                        if (lProtocol == null)
                        {
                            lIProtocolRepository.InsertProtocol(pProtocol);
                        }
                        else
                        {
                            if (lProtocol != null)
                            {
                                lIProtocolRepository.UpdateProtocol(lProtocol);
                            }
                        }

                    }
                    return Ok();
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.InternalServerError;
                    error.ErrorMessage = "Pain List is null";
                    response.Add("ErrorResponse", error);
                    logger.LogDebug("Pain List is null");
                    return new ObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Pain Post Error: " + ex);
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

        //Use to download Exercises configured for a Patient for a particular Rx.
        [HttpGet]
        [Route("getprotocol")]
        public JsonResult getprotocol(string sessionid)
        {
            try
            {
                List<Protocol> _result = lIProtocolRepository.getProtocolListBySessionId(sessionid);
                if (_result != null)
                {
                    return Json(new { Status = (int)HttpStatusCode.OK, protocolList = _result, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, protocolList = "", result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch(Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, protocolList = "", result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });

            }
           
        }
    }
}
