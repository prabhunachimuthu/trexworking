using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Extensions
{
    public class SessionExtension
    {
        public static SessionView SessionToSessionView(Session lSession)
        {
            if (lSession == null)
                return null;
            SessionView lSessionView = new SessionView()
            {
                PatientId = lSession.PatientId,
                RxId = lSession.RxId,
                ProtocolId = lSession.ProtocolId,
                SessionId = lSession.SessionId,
                Reps = lSession.Reps,
                Duration = lSession.Duration,
                FlexionReps = lSession.FlexionReps,
                ExtensionReps = lSession.ExtensionReps,
                SessionDate = lSession.SessionDate,
                PainCount = lSession.PainCount,
                MaxFlexion = lSession.MaxFlexion,
                MaxExtension = lSession.MaxExtension,
                MaxPain = lSession.MaxPain,
                Boom1Position = lSession.Boom1Position,
                Boom2Position = lSession.Boom2Position,
                RangeDuration1 = lSession.RangeDuration1,
                RangeDuration2 = lSession.RangeDuration2,
                GuidedMode = lSession.GuidedMode,
            };
            return lSessionView;
        }

        public static Session SessionViewToSession(SessionView lSession)
        {
            if (lSession == null)
                return null;
            Session pSession = new Session()
            {
                PatientId = lSession.PatientId,
                RxId = lSession.RxId,
                ProtocolId = lSession.ProtocolId,
                SessionId = lSession.SessionId,
                Reps = lSession.Reps,
                Duration = lSession.Duration,
                FlexionReps = lSession.FlexionReps,
                ExtensionReps = lSession.ExtensionReps,
                SessionDate = lSession.SessionDate,
                PainCount = lSession.PainCount,
                MaxFlexion = lSession.MaxFlexion,
                MaxExtension = lSession.MaxExtension,
                MaxPain = lSession.MaxPain,
                Boom1Position = lSession.Boom1Position,
                Boom2Position = lSession.Boom2Position,
                RangeDuration1 = lSession.RangeDuration1,
                RangeDuration2 = lSession.RangeDuration2,
                GuidedMode = lSession.GuidedMode,
            };
            return pSession;
        }
    }
}
