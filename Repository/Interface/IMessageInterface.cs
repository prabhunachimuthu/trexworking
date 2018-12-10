using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface IMessageInterface : IDisposable
    {
        
        List<MessageView> getMessagesbyTimeZone(string patientId, string timezoneid);
        List<MessageView> getMessagesbyTimeZone(string patientId, string userId, string timezoneid);
        int RemoveMessage(Messages pMessage);
        Messages getMessagesById(int id);
        int InsertMessage(Messages pMessage);
        List<MessageView> getMessages(string patientId);
        List<MessageView> getMessages(string patientId, string datetime);
        List<MessageView> getMessages(string patientId, string datetime, int flag);
    }
}
