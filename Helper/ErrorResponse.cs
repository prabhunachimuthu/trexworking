using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OneDirect.Helper
{
    public class ErrorResponse
    {
        public HttpStatusCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
