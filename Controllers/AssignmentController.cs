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
using OneDirect.Response;
using OneDirect.Helper;
using OneDirect.ViewModels;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    public class AssignmentController : Controller
    {

        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly IProtocolInterface lIProtocolRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public AssignmentController(OneDirectContext context, ILogger<AssignmentController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIProtocolRepository = new ProtocolRepository(context);
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


        //inserting the new record of equipment and updating the existing equipment assignment
        // POST api/values
        [HttpPost]
        public JsonResult Post([FromBody]EquipmentAPi pEquipmentAssignment)
        {
            ErrorResponse error = new ErrorResponse();
            EquipmentAssignment lEquipmentAssignment;
            var response = new Dictionary<string, object>();
            string result = string.Empty;
            try
            {
                logger.LogDebug("EquipmentAssignment Post Start");
                if (!String.IsNullOrEmpty(pEquipmentAssignment.AssignmentId))
                {
                    lEquipmentAssignment = lIEquipmentAssignmentRepository.getEquipmentAssignment(pEquipmentAssignment.AssignmentId);

                    if (lEquipmentAssignment == null)
                    {
                        lEquipmentAssignment = new EquipmentAssignment();
                        lEquipmentAssignment.AssignmentId = pEquipmentAssignment.AssignmentId;
                        lEquipmentAssignment.InstallerId = pEquipmentAssignment.InstallerId;
                        lEquipmentAssignment.PatientId = pEquipmentAssignment.PatientId;
                        lEquipmentAssignment.DateInstalled = pEquipmentAssignment.DateInstalled;
                        lEquipmentAssignment.DateRemoved = pEquipmentAssignment.DateRemoved;
                        lEquipmentAssignment.Limb = pEquipmentAssignment.Limb;
                        lEquipmentAssignment.Side = pEquipmentAssignment.Side;
                        lEquipmentAssignment.ExcerciseEnum = pEquipmentAssignment.ExcerciseEnum;
                        lEquipmentAssignment.ChairId = pEquipmentAssignment.ChairId;
                        lEquipmentAssignment.Boom1Id = pEquipmentAssignment.Boom1Id;
                        lEquipmentAssignment.Boom2Id = pEquipmentAssignment.Boom2Id;
                        lEquipmentAssignment.Boom3Id = pEquipmentAssignment.Boom3Id;

                        lEquipmentAssignment.CreatedDate =DateTime.UtcNow;

                        result = lIEquipmentAssignmentRepository.InsertEquipmentAssignment(lEquipmentAssignment);
                    }
                    else
                    {
                        lEquipmentAssignment.AssignmentId = pEquipmentAssignment.AssignmentId;
                        lEquipmentAssignment.InstallerId = pEquipmentAssignment.InstallerId;
                        lEquipmentAssignment.PatientId = pEquipmentAssignment.PatientId;
                        lEquipmentAssignment.DateInstalled = pEquipmentAssignment.DateInstalled;
                        lEquipmentAssignment.DateRemoved = pEquipmentAssignment.DateRemoved;
                        lEquipmentAssignment.Limb = pEquipmentAssignment.Limb;
                        lEquipmentAssignment.Side = pEquipmentAssignment.Side;
                        lEquipmentAssignment.ExcerciseEnum = pEquipmentAssignment.ExcerciseEnum;
                        lEquipmentAssignment.ChairId = pEquipmentAssignment.ChairId;
                        lEquipmentAssignment.Boom1Id = pEquipmentAssignment.Boom1Id;
                        lEquipmentAssignment.Boom2Id = pEquipmentAssignment.Boom2Id;
                        lEquipmentAssignment.Boom3Id = pEquipmentAssignment.Boom3Id;
                        lEquipmentAssignment.UpdatedDate = DateTime.UtcNow;
                        result = lIEquipmentAssignmentRepository.UpdateEquipmentAssignment(lEquipmentAssignment);
                    }
                }
                if (result == "success")
                    return Json(new { Status = (int)HttpStatusCode.OK, result = "Equipment inserted successfully", TimeZone = DateTime.UtcNow.ToString("s") });
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Equipment not inserted", TimeZone = DateTime.UtcNow.ToString("s") });
                }

            }
            catch (Exception ex)
            {
                logger.LogDebug("Assignment Post Error: " + ex);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = ex.ToString();
                response.Add("ErrorResponse", error);
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Equipment not inserted", TimeZone = DateTime.UtcNow.ToString("s") });

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
