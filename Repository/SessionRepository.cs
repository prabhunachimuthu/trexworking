using Microsoft.EntityFrameworkCore;
using OneDirect.Models;
using OneDirect.Repository.Interface;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository
{
    public class SessionRepository : ISessionInterface
    {
        private OneDirectContext context;

        public SessionRepository(OneDirectContext context)
        {
            this.context = context;
        }
        public Session getSession(string lsessionID)
        {
            return (from p in context.Session
                    where p.SessionId == lsessionID
                    select p).FirstOrDefault();
        }

        public List<SessionItem> getSessionList(int lpatientId, string RxId)
        {
            List<SessionItem> result = (from p in context.Session
                                        where p.PatientId == lpatientId && p.RxId == RxId
                                        select new SessionItem
                                        {
                                            PatientId = p.PatientId,
                                            RxId = p.RxId,
                                            ProtocolId = p.ProtocolId,
                                            SessionId = p.SessionId,
                                            Reps = p.Reps,
                                            Duration = p.Duration,
                                            FlexionReps = p.FlexionReps,
                                            ExtensionReps = p.ExtensionReps,
                                            SessionDate = p.SessionDate,
                                            PainCount = p.PainCount,
                                            MaxFlexion = p.MaxFlexion,
                                            MaxExtension = p.MaxExtension,
                                            MaxPain = p.MaxPain,
                                            Boom1Position = p.Boom1Position,
                                            Boom2Position = p.Boom2Position,
                                            RangeDuration1 = p.RangeDuration1,
                                            RangeDuration2 = p.RangeDuration2,
                                            GuidedMode = p.GuidedMode
                                        }).ToList();
            return result;
        }
        public UserSession getSessionbySessionId(string lsessionID)
        {
            return (from p in context.Session
                    join _user in context.Patient on p.PatientId equals _user.PatientId
                    join rx in context.PatientRx on p.RxId equals rx.RxId
                    join _protocol in context.Protocol on p.ProtocolId equals _protocol.ProtocolId
                    where p.SessionId == lsessionID
                    select new UserSession
                    {
                        PatientId = _user.PatientId.ToString(),
                        Duration = p.Duration,
                        Date = p.SessionDate,
                        Reps = p.Reps,
                        ProtocolId = _protocol.ProtocolName,
                        ProtocolEnum = _protocol.ProtocolEnum,
                        SessionId = p.SessionId,
                        Name = _user.PatientName,
                        FlexionArchieved = p.MaxFlexion,
                        FlexionGoal = rx.GoalFlexion,
                        ExtensionArchieved = p.MaxExtension,
                        ExtensionGoal = rx.GoalExtension,
                        Pain = p.PainCount,
                        EquipmentType = rx.EquipmentType,
                        GuidedMode = p.GuidedMode,
                        DeviceConfiguration = _protocol.DeviceConfiguration

                    }).FirstOrDefault();
        }
        public List<UserSession> getSessionList()
        {
            List<UserSession> _sessionlist = new List<UserSession>();
            try
            {
                _sessionlist = (from p in context.Session
                                join _user in context.Patient on p.PatientId equals _user.PatientId
                                join rx in context.PatientRx on p.RxId equals rx.RxId
                                join _protocol in context.Protocol on p.ProtocolId equals _protocol.ProtocolId
                                let cCount = (from c in context.Pain where p.SessionId == c.SessionId select c).Count()
                                orderby p.SessionDate, p.Duration descending
                                select new UserSession
                                {
                                    PatientId = _user.PatientId.ToString(),
                                    Duration = p.Duration,
                                    Date = p.SessionDate,
                                    Reps = p.Reps,
                                    ProtocolId = _protocol.ProtocolName,
                                    ProtocolEnum = _protocol.ProtocolEnum,
                                    SessionId = p.SessionId,
                                    Name = _user.PatientName,
                                    FlexionArchieved = p.MaxFlexion,
                                    FlexionGoal = rx.GoalFlexion,
                                    ExtensionArchieved = p.MaxExtension,
                                    ExtensionGoal = rx.GoalExtension,
                                    PainCount = cCount,
                                    Pro_reps = _protocol.Reps,
                                    GuidedMode = p.GuidedMode

                                }).ToList();
            }
            catch (Exception e)
            {
                throw;
            }

            return _sessionlist;
        }
        public List<UserSession> getSessionList(string lpatientId, string eenum)
        {
            List<UserSession> _sessionlist = new List<UserSession>();
            try
            {
                _sessionlist = (from rx in context.PatientRx
                                join p in context.Session on rx.RxId equals p.RxId //PatientRx join with Sessions using RxId
                                from _user in context.Patient.Where(u => u.PatientId == p.PatientId).DefaultIfEmpty() //on p.PatientId equals _user.UserId into usr
                                from _protocol in context.Protocol.Where(pr => pr.ProtocolId == p.ProtocolId).DefaultIfEmpty() //on p.ProtocolId equals _protocol.ProtocolId into pro
                                let cCount = (from c in context.Pain where p.SessionId == c.SessionId select c).Count()//getting count of Pains from Pain Table using SessionId
                                where rx.PatientId == Convert.ToInt32(lpatientId) && rx.DeviceConfiguration == eenum
                                orderby p.SessionDate, p.Duration descending
                                select new UserSession
                                {
                                    PatientId = _user.PatientId.ToString(),
                                    Duration = p.Duration,
                                    Date = p.SessionDate,
                                    Reps = p.Reps,
                                    ProtocolId = _protocol.ProtocolName,
                                    SessionId = p.SessionId,
                                    Name = _user.PatientName,
                                    FlexionArchieved = p.MaxFlexion,
                                    FlexionGoal = rx.GoalFlexion,
                                    ExtensionArchieved = p.MaxExtension,
                                    ProtocolEnum = _protocol.ProtocolEnum,
                                    ExtensionGoal = rx.GoalExtension,
                                    PainCount = cCount,
                                    Pro_reps = _protocol.Reps,
                                    GuidedMode = p.GuidedMode
                                }).ToList();
            }
            catch (Exception e)
            {
                throw;
            }


            return _sessionlist;
        }
        public List<UserSession> getSessionListByTherapistId(string lTherapist)
        {
            return (from p in context.EquipmentAssignment
                    join _session in context.Session on p.PatientId equals _session.PatientId
                    join rx in context.PatientRx on _session.RxId equals rx.RxId
                    join _user in context.Patient on _session.PatientId equals _user.PatientId
                    join _protocol in context.Protocol on _session.ProtocolId equals _protocol.ProtocolId
                    where p.InstallerId == lTherapist
                    select new UserSession
                    {
                        PatientId = _user.PatientId.ToString(),
                        Duration = _session.Duration,
                        Date = _session.SessionDate,
                        Reps = _session.Reps,
                        ProtocolId = _protocol.ProtocolName,
                        ProtocolEnum = _protocol.ProtocolEnum,
                        SessionId = _session.SessionId,
                        Name = _user.PatientName,
                        FlexionArchieved = _session.MaxFlexion,
                        FlexionGoal = rx.GoalFlexion,
                        ExtensionArchieved = _session.MaxExtension,
                        ExtensionGoal = rx.GoalExtension,
                        Pain = _session.PainCount,
                        GuidedMode = _session.GuidedMode
                    }).ToList();
        }
        public List<UserSession> getSessionListByProtocoloId(string lProtocol)
        {
            return (from _session in context.Session
                    join rx in context.PatientRx on _session.RxId equals rx.RxId
                    join _user in context.Patient on _session.PatientId equals _user.PatientId
                    join _protocol in context.Protocol on _session.ProtocolId equals _protocol.ProtocolId
                    where _session.ProtocolId == lProtocol
                    select new UserSession
                    {
                        PatientId = _user.PatientId.ToString(),
                        Duration = _session.Duration,
                        Date = _session.SessionDate,
                        Reps = _session.Reps,
                        ProtocolId = _protocol.ProtocolName,
                        SessionId = _session.SessionId,
                        Name = _user.PatientName,
                        FlexionArchieved = _session.MaxFlexion,
                        ProtocolEnum = _protocol.ProtocolEnum,
                        FlexionGoal = rx.GoalFlexion,
                        ExtensionArchieved = _session.MaxExtension,
                        ExtensionGoal = rx.GoalExtension,
                        Pain = _session.PainCount,
                        GuidedMode = _session.GuidedMode
                    }).ToList();
        }
        public void InsertSession(Session pSession)
        {
            context.Session.Add(pSession);
            context.SaveChanges();
        }
        public void UpdateSession(Session pSession)
        {
            //var _session = (from p in context.Session
            //                where p.SessionId == pSession.SessionId
            //                select p).FirstOrDefault();
            //if (_session != null)
            //{
            context.Entry(pSession).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            //}
        }

        public string DeleteSessionRecordsWithCasecade(string sessionId)
        {
            try
            {
                var pain = (from p in context.Pain where p.SessionId == sessionId select p).ToList();
                context.Pain.RemoveRange(pain);
                context.SaveChanges();
                var session = (from p in context.Session where p.SessionId == sessionId select p).ToList();
                context.Session.RemoveRange(session);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
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
