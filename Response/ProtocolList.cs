using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Response
{
    public class ProtocolList
    {
        public List<Protocol> Protocol { get; set; }

        public string result { get; set; }
    }
}
