using Hangfire.Annotations;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        

        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpcontext = context.GetHttpContext();
           

            return true;
        }
    }
}
