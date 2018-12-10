using Microsoft.EntityFrameworkCore;
using OneDirect.Helper;
using OneDirect.Models;
using OneDirect.Repository.Interface;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository
{
    public class MessageRepository : IMessageInterface
    {
        private OneDirectContext context;

        public MessageRepository(OneDirectContext context)
        {
            this.context = context;
        }

        public int InsertMessage(Messages pMessage)
        {
            context.Messages.Add(pMessage);
            return context.SaveChanges();
        }

        public int RemoveMessage(Messages pMessage)
        {
            context.Messages.Remove(pMessage);
            return context.SaveChanges();
        }

        public Messages getMessagesById(int id)
        {

            Messages lmsg = context.Messages.FirstOrDefault(x => x.MsgHeaderId == id);
            return lmsg;


        }
        public List<MessageView> getMessagesbyTimeZone(string patientId, string timezoneid)
        {
            List<MessageView> mlist = (from p in context.Messages
                                       where
                                       p.PatientId == patientId
                                       select new MessageView
                                       {
                                           MsgHeaderId = p.MsgHeaderId,
                                           PatientId = p.PatientId,
                                           SentReceivedFlag = p.SentReceivedFlag,
                                           UserGroup = p.UserGroup,
                                           UserType = p.UserType,
                                           UserId = p.UserId,
                                           UserName = p.UserName,
                                           Datetime = Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(p.Datetime, timezoneid)),
                                           ReadStatus = p.ReadStatus,
                                           BodyText = p.BodyText
                                       }).OrderBy(x => x.Datetime).ToList();
            return mlist;


        }


        public List<MessageView> getMessagesbyTimeZone(string patientId, string userId, string timezoneid)
        {
            List<MessageView> mlist = (from p in context.Messages
                                       where
                                       p.PatientId == patientId
                                       && p.UserId == userId
                                       select new MessageView
                                       {
                                           MsgHeaderId = p.MsgHeaderId,
                                           PatientId = p.PatientId,
                                           SentReceivedFlag = p.SentReceivedFlag,
                                           UserGroup = p.UserGroup,
                                           UserType = p.UserType,
                                           UserId = p.UserId,
                                           UserName = p.UserName,
                                           Datetime = Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(p.Datetime, timezoneid)),
                                           ReadStatus = p.ReadStatus,
                                           BodyText = p.BodyText
                                       }).OrderBy(x => x.Datetime).ToList();
            return mlist;


        }



        public List<MessageView> getMessages(string patientId)
        {
            List<MessageView> mlist = (from p in context.Messages
                                       where
                                       p.PatientId == patientId
                                       select new MessageView
                                       {
                                           MsgHeaderId = p.MsgHeaderId,
                                           PatientId = p.PatientId,
                                           SentReceivedFlag = p.SentReceivedFlag,
                                           UserGroup = p.UserGroup,
                                           UserType = p.UserType,
                                           UserId = p.UserId,
                                           UserName = p.UserName,
                                           Datetime = p.Datetime,
                                           ReadStatus = p.ReadStatus,
                                           BodyText = p.BodyText
                                       }).OrderBy(x => x.Datetime).ToList();
            return mlist;


        }

        public List<MessageView> getMessages(string patientId, string datetime)
        {
            List<MessageView> mlist = (from p in context.Messages
                                       where
                                       p.PatientId == patientId && p.Datetime > Convert.ToDateTime(datetime)
                                       select new MessageView
                                       {
                                           MsgHeaderId = p.MsgHeaderId,
                                           PatientId = p.PatientId,
                                           SentReceivedFlag = p.SentReceivedFlag,
                                           UserGroup = p.UserGroup,
                                           UserType = p.UserType,
                                           UserId = p.UserId,
                                           UserName = p.UserName,
                                           Datetime = p.Datetime,
                                           ReadStatus = p.ReadStatus,
                                           BodyText = p.BodyText
                                       }).OrderBy(x => x.Datetime).ToList();
            return mlist;


        }
        public List<MessageView> getMessages(string patientId, string datetime, int flag)
        {
            List<MessageView> mlist = (from p in context.Messages
                                       where
                                       p.PatientId == patientId && p.Datetime > Convert.ToDateTime(datetime) && p.SentReceivedFlag == flag
                                       select new MessageView
                                       {
                                           MsgHeaderId = p.MsgHeaderId,
                                           PatientId = p.PatientId,
                                           SentReceivedFlag = p.SentReceivedFlag,
                                           UserGroup = p.UserGroup,
                                           UserType = p.UserType,
                                           UserId = p.UserId,
                                           UserName = p.UserName,
                                           Datetime = p.Datetime,
                                           ReadStatus = p.ReadStatus,
                                           BodyText = p.BodyText
                                       }).OrderBy(x => x.Datetime).ToList();
            return mlist;


        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
