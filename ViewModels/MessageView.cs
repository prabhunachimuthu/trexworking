using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class MessageView
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
    }

    public class MessageViewList
    {
        public List<MessageView> messages { get; set; }
        public string timezoneOffset { get; set; }
    }

    public class sendmessage
    {
        public MessageView message { get; set; }
        public string timezoneOffset { get; set; }
    }

    public class PatientMessage
    {
        
        public Patient Patient { get; set; }
        public int replyStatus { get; set; }

    }
}
