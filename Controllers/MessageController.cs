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
using Newtonsoft.Json;
using OneDirect.ViewModels;
using OneDirect.Extensions;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly IPatient IPatient;
        private readonly IMessageInterface lIMessageRepository;
        private readonly IProtocolInterface lIProtocolRepository;
        private readonly IPainInterface lIPainRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public MessageController(OneDirectContext context, ILogger<ProtocolController> plogger, IPainInterface IPainRepository)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
            IPatient = new PatientRepository(context);
            lIMessageRepository = new MessageRepository(context);
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
            return "";

        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Messages pMessage)
        {

            return null;

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


        //downloading or getting messages using sessionId and datetime
        [HttpGet]
        [Route("downloadmessages")]
        public JsonResult downloadmessages(string sessionid, string datetime = "")
        {

            try
            {
                if (!string.IsNullOrEmpty(sessionid))
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null)
                    {
                        MessageViewList lmessageList = new MessageViewList();
                        if (string.IsNullOrEmpty(datetime))
                        {
                            lmessageList.messages = lIMessageRepository.getMessages(lpatient.PatientLoginId);
                        }
                        else
                        {
                            lmessageList.messages = lIMessageRepository.getMessages(lpatient.PatientLoginId, datetime);
                        }

                        string timezoneid = TimeZoneInfo.Local.SupportsDaylightSavingTime ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;
                        lmessageList.timezoneOffset = timezoneid;
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", timezoneOffset = lmessageList.timezoneOffset, messages = lmessageList.messages });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Forbidden, result = "patient is not configured", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Forbidden, result = "input is not valid", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Get Rx Error: " + ex);

                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "getting messages failed" });
            }
        }

        //sending message by patient and inserting it using the mapping of user and patient
        [HttpPost]
        [Route("sendmessage")]
        public JsonResult sendmessage([FromBody]sendmessage message, string sessionid)
        {

            try
            {
                if (!string.IsNullOrEmpty(sessionid) && message != null)
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    User luser = lIUserRepository.getUser(message.message.UserId);
                    if (lpatient != null)
                    {
                        if (luser != null)
                        {
                            if (((message.message.UserType == ConstantsVar.PatientAdministrator && lpatient.Paid == message.message.UserId) || (message.message.UserType == ConstantsVar.Therapist && lpatient.Therapistid == message.message.UserId) || (message.message.UserType == ConstantsVar.Provider && lpatient.ProviderId == message.message.UserId) || message.message.UserType == ConstantsVar.Support))
                            {
                                DateTime mdatetime = Convert.ToDateTime(Utilities.ConverTimetoServerTimeZone(Convert.ToDateTime(message.message.Datetime), message.timezoneOffset));
                                Messages lmessage = MessageExtension.MessageViewToMessage(message.message);
                                lmessage.PatientId = lpatient.PatientLoginId;
                                lmessage.Datetime = mdatetime;
                                int res = lIMessageRepository.InsertMessage(lmessage);
                                if (res > 0)
                                {
                                    string content = "Patient has sent a message.<br><a href='" + ConfigVars.NewInstance.url + "?ruserid=" + Utilities.EncryptText(luser.UserId) + "&rtype=" + Utilities.EncryptText(luser.Type.ToString()) + "&rpage=" + Utilities.EncryptText("meg") + "'> Click to view</a>";
                                    Smtp.SendGridEmail(luser.Email, "Messages", content);
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "success", MessageHeaderID = lmessage.MsgHeaderId });
                                }
                                else
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "not inserted" });

                            }
                            else
                            {
                                return Json(new { Status = (int)HttpStatusCode.Forbidden, result = "patient and user is not mapping", TimeZone = DateTime.UtcNow.ToString("s") });
                            }
                        }
                        else
                        {
                            return Json(new { Status = (int)HttpStatusCode.Forbidden, result = "user is not configured", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Forbidden, result = "patient is not configured", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Forbidden, result = "input is not valid", TimeZone = DateTime.UtcNow.ToString("s") });

                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Get Rx Error: " + ex);

                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "getting messages failed" });
            }
        }


        //removing the message 
        [HttpGet]
        [Route("deletemessage")]
        public JsonResult deletemessage(string sessionid, string messageheaderid = "")
        {

            try
            {
                int id = 0;
                if (!string.IsNullOrEmpty(sessionid) && !string.IsNullOrEmpty(messageheaderid) && int.TryParse(messageheaderid, out id))
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null && id > 0)
                    {
                        Messages lmsg = lIMessageRepository.getMessagesById(id);
                        if (lmsg != null)
                        {
                            int res = lIMessageRepository.RemoveMessage(lmsg);
                            if (res > 0)
                            {
                                return Json(new { Status = (int)HttpStatusCode.OK, result = "success", messageheaderid = lmsg.MsgHeaderId });
                            }
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.Forbidden, result = "patient is not configured", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Forbidden, result = "input is not valid", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Get Rx Error: " + ex);

                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "getting messages failed" });
            }
            return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "failure" });
        }

    }
}
