using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using OneDirect.Models;
using OneDirect.Repository;
using OneDirect.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect
{
    public class LoginAuthorizeAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        private readonly IUserInterface lIUserRepository;

        public LoginAuthorizeAttribute(IUserInterface IUserRepository)
        {
            lIUserRepository = IUserRepository;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.Method) && filterContext.HttpContext.Request.Method.ToLower() == "get")
                if (filterContext.ActionDescriptor.RouteValues != null && filterContext.ActionDescriptor.RouteValues.Count > 1 && filterContext.ActionDescriptor.RouteValues["controller"] == "CreatePatient" && filterContext.ActionDescriptor.RouteValues["action"] == "PatientRX"
                  && filterContext.ActionDescriptor.Parameters != null && filterContext.ActionDescriptor.Parameters.Count > 1 && filterContext.ActionDescriptor.Parameters[1].Name == "operaton" && filterContext.ActionDescriptor.Parameters[1].BindingInfo == null)
                {

                }
                else
                {
                    filterContext.HttpContext.Session.SetString("SessionTime", DateTime.Now.ToString());
                }

        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetString("UserId");
            var sessionId = context.HttpContext.Session.GetString("SessionId");
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "SignOut", area = "", id = "Expired" }));
                //context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "PatientView", action = "SignOut", area = "Patient", id = "Expired" }));
            }
            else if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(sessionId))
            {
                User luser = lIUserRepository.getUser(userId);
                if (luser != null)
                {
                    if (sessionId != luser.LoginSessionId)
                    {
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "SignOut", area = "", id = "Expired" }));
                        //context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "PatientView", action = "SignOut",area="Patient", id = "Expired" }));
                    }
                }
            }
        }
    }
}
