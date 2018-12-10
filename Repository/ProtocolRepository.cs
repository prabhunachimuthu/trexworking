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
    public class ProtocolRepository : IProtocolInterface
    {
        private OneDirectContext context;

        public ProtocolRepository(OneDirectContext context)
        {
            this.context = context;
        }

        public Protocol getProtocol(string lprotocolID)
        {
            return (from p in context.Protocol
                    where p.ProtocolId == lprotocolID
                    select p).FirstOrDefault();
        }

        public Protocol getProtocol(List<SessionImport> lsessionList)
        {
            return context.Protocol.FirstOrDefault(o => lsessionList.Any(opts => opts.ProtocolId == o.ProtocolId));
        }
        public List<Protocol> getProtocol()
        {
            return (from p in context.Protocol
                    select p).ToList();
        }
        public List<Protocol> getMobileProtocol(string lpatientId)
        {
            return (from p in context.Protocol
                    where p.PatientId == Convert.ToInt32(lpatientId)
                    select p).ToList();

        }
        public List<Protocol> getProtocolList(string lpatientId)
        {

            return (from p in context.Protocol
                    join _user in context.Patient on p.PatientId equals _user.PatientId
                    where p.PatientId == Convert.ToInt32(lpatientId)
                    orderby _user.PatientName
                    select new Protocol
                    {
                        PatientId = _user.PatientId,
                        ProtocolId = p.ProtocolId,
                        ProtocolName = p.ProtocolName,
                        RestPosition = p.RestPosition,
                        MaxUpLimit = p.MaxUpLimit,
                        StretchUpLimit = p.StretchUpLimit,
                        MaxDownLimit = p.MaxDownLimit,
                        StretchDownLimit = p.StretchDownLimit,
                        RestAt = p.RestAt,
                        RepsAt = p.RepsAt,
                        Speed = p.Speed,
                        Reps = p.Reps,
                        EquipmentType = p.EquipmentType,
                        FlexUpLimit = p.FlexUpLimit,

                        StretchUpHoldtime = p.StretchUpHoldtime,
                        FlexDownLimit = p.FlexDownLimit,

                        StretchDownHoldtime = p.StretchDownHoldtime,

                        Time = p.Time,
                        Session = (from q in context.Session where q.ProtocolId == p.ProtocolId select q).ToList()
                    }).ToList();

        }

        public Protocol getProtocol(string lprotocolID, string lPatientId)
        {
            return (from p in context.Protocol
                    where p.ProtocolId == lprotocolID && p.PatientId == Convert.ToInt32(lPatientId)
                    select p).FirstOrDefault();
        }


        public int getProtocolCount(string lPatientId)
        {
            return (from p in context.Protocol
                    where p.PatientId == Convert.ToInt32(lPatientId)
                    select p).Count();

        }

        public string InsertProtocol(Protocol pProtocol)
        {
            context.Protocol.Add(pProtocol);
            context.SaveChanges();
            return "";
        }

        public string DeleteProtocol(Protocol pProtocol)
        {
            context.Protocol.Remove(pProtocol);
            context.SaveChanges();
            return "";
        }
        public string UpdateProtocol(Protocol pProtocol)
        {
            var _protocol = (from p in context.Protocol
                             where p.ProtocolId == pProtocol.ProtocolId
                             select p).FirstOrDefault();
            if (_protocol != null)
            {
                _protocol.ProtocolName = pProtocol.ProtocolName;
                _protocol.PatientId = pProtocol.PatientId;

                _protocol.EquipmentType = pProtocol.EquipmentType;

                _protocol.MaxUpLimit = pProtocol.MaxUpLimit;
                _protocol.MaxDownLimit = pProtocol.MaxDownLimit;

                _protocol.FlexUpLimit = pProtocol.FlexUpLimit;
                _protocol.FlexDownLimit = pProtocol.FlexDownLimit;


                _protocol.StretchUpLimit = pProtocol.StretchUpLimit;
                _protocol.StretchDownLimit = pProtocol.StretchDownLimit;
                _protocol.StretchUpHoldtime = pProtocol.StretchUpHoldtime;
                _protocol.StretchDownHoldtime = pProtocol.StretchDownHoldtime;

                _protocol.RestPosition = pProtocol.RestPosition;
                _protocol.Reps = pProtocol.Reps;
                _protocol.RestAt = pProtocol.RestAt;
                _protocol.RepsAt = pProtocol.RepsAt;
                _protocol.Speed = pProtocol.Speed;


                if (pProtocol != null && pProtocol.Time != null && !String.IsNullOrEmpty(pProtocol.Time.ToString()))
                    _protocol.Time = pProtocol.Time;

                context.Entry(_protocol).State = EntityState.Modified;
                context.SaveChanges();
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

        public List<Protocol> getProtocolListBySessionId(string SessionId)
        {
            return (from _patient in context.Patient
                    join p in context.Protocol on _patient.PatientId equals p.PatientId
                    where _patient.LoginSessionId.ToString() == SessionId
                    select p).ToList();


        }
    }
}
