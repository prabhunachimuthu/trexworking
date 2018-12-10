using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class UserListView
    {
        public string UserId { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Npi { get; set; }
        public string Vseeid { get; set; }
        public string LoginSessionId { get; set; }
        public int Count { get; set; }
     
    }
}
