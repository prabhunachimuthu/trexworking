using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Extensions
{
    public class MessageExtension
    {
        public static MessageView MessageToMessageView(Messages lMessage)
        {
            if (lMessage == null)
                return null;
            MessageView lDeviceCalibrationView = new MessageView()
            {
                MsgHeaderId = lMessage.MsgHeaderId,
                PatientId = lMessage.PatientId,
                SentReceivedFlag = lMessage.SentReceivedFlag,
                UserGroup = lMessage.UserGroup,
                UserType = lMessage.UserType,
                UserId = lMessage.UserId,
                UserName = lMessage.UserName,
                Datetime = lMessage.Datetime,
                ReadStatus = lMessage.ReadStatus,
                BodyText = lMessage.BodyText
            };
            return lDeviceCalibrationView;
        }

        public static Messages MessageViewToMessage(MessageView lMessage)
        {
            if (lMessage == null)
                return null;
            Messages lDeviceCalibrationView = new Messages()
            {
                MsgHeaderId = lMessage.MsgHeaderId,
                PatientId = lMessage.PatientId,
                SentReceivedFlag = lMessage.SentReceivedFlag,
                UserGroup = lMessage.UserGroup,
                UserType = lMessage.UserType,
                UserId = lMessage.UserId,
                UserName = lMessage.UserName,
                Datetime = lMessage.Datetime,
                ReadStatus = lMessage.ReadStatus,
                BodyText = lMessage.BodyText
            };
            return lDeviceCalibrationView;
        }

    }
}
