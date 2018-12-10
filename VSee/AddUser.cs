using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.VSee
{
    public class AddUser
    {
        public string secretkey { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string fn { get; set; }
        public string ln { get; set; }
    }

    public class DeleteUser
    {
        public string secretkey { get; set; }
        public string username { get; set; }
    }
    public class StateUser
    {
        public string secretkey { get; set; }
        public long start { get; set; }
        public long end { get; set; }
        public int limit { get; set; }
    }

    public class GetUser
    {
        public string username { get; set; }
        public string uri { get; set; }
    }
}
