using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class PatientMessageView
    {
        public User Patient { get; set; }
        public int ReceiveMessage { get; set; }
        public int SentMessage { get; set; }
        public int TotalUnreadMessage { get; set; }
        public DateTime? LastMessageDate { get; set; }

    }
}
