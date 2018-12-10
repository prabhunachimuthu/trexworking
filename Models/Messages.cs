using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Messages
    {
        public int MsgHeaderId { get; set; }
        public string PatientId { get; set; }
        public int SentReceivedFlag { get; set; }
        public int? UserGroup { get; set; }
        public int UserType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Datetime { get; set; }
        public int ReadStatus { get; set; }
        public string BodyText { get; set; }

        public User Patient { get; set; }
        public User User { get; set; }
    }
}
