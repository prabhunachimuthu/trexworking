using Highsoft.Web.Mvc.Stocks;
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
    public class PatientRxRepository : IPatientRxInterface
    {
        private OneDirectContext context;
        private readonly IEquipmentExerciseInterface lIEquipmentExerciseRepository;


        public PatientRxRepository(OneDirectContext context, IEquipmentExerciseInterface EquipmentExerciseRepository)
        {
            this.context = context;
            lIEquipmentExerciseRepository = EquipmentExerciseRepository;
        }


        public PatientRx getPatientRxbyRxId(string rxid, string patid)
        {
            return (from p in context.PatientRx.Include(x => x.Patient).
                   Include(x => x.Session)
                   .Where(p => p.RxId == rxid && p.PatientId == Convert.ToInt32(patid))
                    select p).FirstOrDefault();

        }
        public PatientRx getPatientRxPain(string rxid, string patid)
        {
            return (from p in context.PatientRx.
                   Include(x => x.Session)
                   .ThenInclude(y => y.Pain)
                   .Where(p => p.RxId == rxid && p.PatientId == Convert.ToInt32(patid))
                    select p).FirstOrDefault();

        }

        public List<DashboardView> getDashboard(string id)
        {

            var resulta = (from p in context.Patient
                          .Include(pro => pro.Protocol)
                          .ThenInclude(s => s.Session)
                           where p.ProviderId == id
                           select p)
                          .ToList();
            if (resulta != null && resulta.Count > 0)
            {
                List<DashboardView> llist = new List<DashboardView>();
                for (int i = 0; i < resulta.Count; i++)
                {
                    List<PatientRx> lrx = (context.PatientRx.Include(x => x.Provider).Where(x => x.PatientId == resulta[i].PatientId).OrderBy(x => x.RxStartDate).ToList());
                    if (lrx != null && lrx.Count > 0)
                    {
                        DashboardView lview = new DashboardView();
                        lview.PatientRx = lrx.First();
                        lview.FirstUse = resulta[i].Session.Count == 0 ? null : resulta[i].Session.Min(t => t.SessionDate);
                        lview.LastUse = resulta[i].Session.Count == 0 ? null : resulta[i].Session.Max(t => t.SessionDate);
                        lview.MaxPain = resulta[i].Session.Count == 0 ? 0 : resulta[i].Session.Max(t => t.MaxPain);
                        resulta[i].PatientRx = lrx;
                        lview.Progress = getProgress(resulta[i]);
                        lview.TotalSession = resulta[i].Session.Count();
                        llist.Add(lview);
                    }
                }

                llist = llist.Count > 0 ? llist.Where(x => x.PatientRx != null).ToList() : llist;
                return llist;
            }
            return null;

        }
        public List<DashboardView> getDashboardForPatientAdmin(string id)
        {


            var resulta = (from p in context.Patient.Include(pro => pro.Protocol).ThenInclude(s => s.Session).Include(rx => rx.PatientRx).ThenInclude(pro => pro.Provider)
                           where p.Paid == id
                           select new DashboardView
                           {
                               PatientRx = p.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault(),
                               FirstUse = p.Session.Count == 0 ? null : p.Session.Min(t => t.SessionDate),
                               LastUse = p.Session.Count == 0 ? null : p.Session.Max(t => t.SessionDate),
                               MaxPain = p.Session.Count == 0 ? 0 : p.Session.Max(t => t.MaxPain),
                               Progress = getProgress(p),
                               TotalSession = p.Session.Count()
                           }).ToList();


            return resulta;

        }

        public List<DashboardView> getDashboardFromTherapistByPatientId(int id)
        {

            var resulta = (from p in context.Patient
                          .Include(pro => pro.Protocol)
                          .ThenInclude(s => s.Session)
                           where p.PatientId == id
                           select p)
                          .FirstOrDefault();
            if (resulta != null)
            {
                List<DashboardView> llist = new List<DashboardView>();

                List<PatientRx> lrx = (context.PatientRx.Include(x => x.Provider).Where(x => x.PatientId == resulta.PatientId).OrderBy(x => x.RxStartDate).ToList());
                if (lrx != null && lrx.Count > 0)
                {
                    DashboardView lview = new DashboardView();
                    lview.PatientRx = lrx.First();
                    if (lview.PatientRx.RxEndDate != null && lview.PatientRx.RxEndDate > DateTime.Now)
                    {
                        lview.percentage = getPercentage(lview.PatientRx.RxStartDate, lview.PatientRx.RxEndDate);
                    }
                    else
                    {
                        lview.percentage = 100;
                    }
                    lview.FirstUse = resulta.Session.Count == 0 ? null : resulta.Session.Min(t => t.SessionDate);
                    lview.LastUse = resulta.Session.Count == 0 ? null : resulta.Session.Max(t => t.SessionDate);
                    lview.MaxPain = resulta.Session.Count == 0 ? 0 : resulta.Session.Max(t => t.MaxPain);
                    resulta.PatientRx = lrx;
                    lview.Progress = getProgress(resulta);
                    lview.TotalSession = resulta.Session.Count();
                    llist.Add(lview);
                }


                llist = llist.Count > 0 ? llist.Where(x => x.PatientRx != null).ToList() : llist;
                return llist;
            }
            return null;

        }
        public List<DashboardView> getDashboardForTherapist(string id)
        {

            var resulta = (from p in context.Patient
                          .Include(pro => pro.Protocol)
                          .ThenInclude(s => s.Session)
                           where p.Therapistid == id
                           select p)
                          .ToList();
            if (resulta != null && resulta.Count > 0)
            {
                List<DashboardView> llist = new List<DashboardView>();
                for (int i = 0; i < resulta.Count; i++)
                {
                    List<PatientRx> lrx = (context.PatientRx.Include(x => x.Provider).Where(x => x.PatientId == resulta[i].PatientId).OrderBy(x => x.RxStartDate).ToList());
                    if (lrx != null && lrx.Count > 0)
                    {
                        DashboardView lview = new DashboardView();
                        lview.PatientRx = lrx.First();
                        if (lview.PatientRx.RxEndDate != null && lview.PatientRx.RxEndDate > DateTime.Now)
                        {
                            lview.percentage = getPercentage(lview.PatientRx.RxStartDate, lview.PatientRx.RxEndDate);
                        }
                        else
                        {
                            lview.percentage = 100;
                        }
                        lview.FirstUse = resulta[i].Session.Count == 0 ? null : resulta[i].Session.Min(t => t.SessionDate);
                        lview.LastUse = resulta[i].Session.Count == 0 ? null : resulta[i].Session.Max(t => t.SessionDate);
                        lview.MaxPain = resulta[i].Session.Count == 0 ? 0 : resulta[i].Session.Max(t => t.MaxPain);
                        resulta[i].PatientRx = lrx;
                        lview.Progress = getProgress(resulta[i]);
                        lview.TotalSession = resulta[i].Session.Count();
                        llist.Add(lview);
                    }
                }

                llist = llist.Count > 0 ? llist.Where(x => x.PatientRx != null).ToList() : llist;
                return llist;
            }
            return null;

        }


        public DashboardView getPatientRxByPatientLoginId(string id)
        {
            var resulta = (from p in context.Patient
                          .Include(pro => pro.Protocol)
                          .ThenInclude(s => s.Session)
                           where p.PatientLoginId == id
                           select p)
                          .FirstOrDefault();
            if (resulta != null)
            {
                List<PatientRx> lrx = (context.PatientRx.Include(x => x.Provider).Where(x => x.PatientId == resulta.PatientId).OrderBy(x => x.RxStartDate).ToList());
                if (lrx != null && lrx.Count > 0)
                {
                    DashboardView lview = new DashboardView();
                    lview.PatientRx = lrx.First();
                    if (lview.PatientRx.RxEndDate != null && lview.PatientRx.RxEndDate > DateTime.Now)
                    {
                        lview.percentage = getPercentage(lview.PatientRx.RxStartDate, lview.PatientRx.RxEndDate);
                    }
                    else
                    {
                        lview.percentage = 100;
                    }
                    lview.FirstUse = resulta.Session.Count == 0 ? null : resulta.Session.Min(t => t.SessionDate);
                    lview.LastUse = resulta.Session.Count == 0 ? null : resulta.Session.Max(t => t.SessionDate);
                    lview.MaxPain = resulta.Session.Count == 0 ? 0 : resulta.Session.Max(t => t.MaxPain);
                    resulta.PatientRx = lrx;
                    //lview.Progress = getProgress(resulta[i]);
                    lview.TotalSession = resulta.Session.Count();

                    return lview;
                }

            }
            return null;

        }
        private int getPercentage(DateTime? startDate, DateTime? EndDate)
        {
            var start = Convert.ToDateTime(startDate);
            var end = Convert.ToDateTime(EndDate);
            var total = (end - start).TotalDays;


            var percent = Convert.ToInt32(Math.Round((DateTime.Now - start).TotalDays * 100 / total));

            return percent;
        }

        public List<DashboardView> getDashboardForSupport()
        {

            var resulta = (from p in context.Patient
                          .Include(pro => pro.Protocol)
                          .ThenInclude(s => s.Session)
                           select p)
                          .ToList();
            if (resulta != null && resulta.Count > 0)
            {
                List<DashboardView> llist = new List<DashboardView>();
                for (int i = 0; i < resulta.Count; i++)
                {
                    List<PatientRx> lrx = (context.PatientRx.Include(x => x.Provider).Where(x => x.PatientId == resulta[i].PatientId).OrderBy(x => x.RxStartDate).ToList());
                    if (lrx != null && lrx.Count > 0)
                    {
                        DashboardView lview = new DashboardView();
                        lview.PatientRx = lrx.First();

                        lview.FirstUse = resulta[i].Session.Count == 0 ? null : resulta[i].Session.Min(t => t.SessionDate);
                        lview.LastUse = resulta[i].Session.Count == 0 ? null : resulta[i].Session.Max(t => t.SessionDate);
                        lview.MaxPain = resulta[i].Session.Count == 0 ? 0 : resulta[i].Session.Max(t => t.MaxPain);
                        resulta[i].PatientRx = lrx;
                        lview.Progress = getProgress(resulta[i]);
                        lview.TotalSession = resulta[i].Session.Count();
                        llist.Add(lview);
                    }
                }

                llist = llist.Count > 0 ? llist.Where(x => x.PatientRx != null).ToList() : llist;
                return llist;
            }
            return null;

        }

        private Progress getProgress(Patient patient)
        {
            Progress _progress = new Progress();
            _progress.Etype = patient.EquipmentType;
            List<Session> session = null;

            var patientRx = patient.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault();
            Func<Patient, PatientRx, List<Session>, int, int> computeProgress = (pPatient, pPatientRx, pSession, pType) =>
            {
                try
                {
                    var Goal = pType == 0 ? (pPatientRx.GoalFlexion - pPatientRx.CurrentFlexion) : (pPatientRx.GoalExtension - pPatientRx.CurrentExtension);
                    var PrescriptionDays = Math.Abs((pPatientRx.RxStartDate - pPatientRx.RxEndDate).Value.Days);
                    var ElapsedDays = Math.Abs((DateTime.Today - pPatientRx.RxStartDate).Value.Days);
                    var GoalRange = (Goal / (decimal)PrescriptionDays) * ElapsedDays;
                    var AchievedRange = pType == 0 ? (pSession.Max(t => t.MaxFlexion) - pPatientRx.CurrentFlexion) : (pSession.Max(t => t.MaxExtension) - patientRx.CurrentExtension);
                    var FlexionProgress = GoalRange == 0 ? 0 : Math.Abs(Convert.ToInt32((AchievedRange / (decimal)GoalRange) * 100));
                    return FlexionProgress;
                }
                catch
                {
                    return 0;
                }
            };

            switch (patient.EquipmentType.ToLower())
            {
                case "ankle":
                case "knee":
                    session = patient.Protocol.Where(p => p.DeviceConfiguration == "Flexion-Extension" && (p.ProtocolEnum == 1 || p.ProtocolEnum == 3) && p.Session.Count > 0).SelectMany(p => p.Session).ToList();

                    if (session != null && session.Count > 0)
                    {
                        _progress.Flexexion = computeProgress(patient, patient.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault(), session, 0);
                    }
                    else
                    {
                        _progress.Flexexion = 0;
                    }
                    session = patient.Protocol.Where(p => p.DeviceConfiguration == "Flexion-Extension" && (p.ProtocolEnum == 2 || p.ProtocolEnum == 3) && p.Session.Count > 0).SelectMany(p => p.Session).ToList();
                    if (session != null && session.Count > 0)
                    {
                        _progress.Extension = computeProgress(patient, patient.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault(), session, 1);
                    }
                    else
                    {
                        _progress.Extension = 0;
                    }
                    break;
                case "shoulder":
                    session = patient.Protocol.Where(p => p.DeviceConfiguration == "Forward Flexion" && (p.ProtocolEnum == 1) && p.Session.Count > 0).SelectMany(p => p.Session).ToList();
                    if (session != null && session.Count > 0)
                    {
                        _progress.Forward = computeProgress(patient, patient.PatientRx.Where(s => s.DeviceConfiguration == "Forward Flexion").FirstOrDefault(), session, 0);
                    }
                    else
                    {
                        _progress.Forward = 0;
                    }

                    session = patient.Protocol.Where(p => p.DeviceConfiguration == "External Rotation" && (p.ProtocolEnum == 1) && p.Session.Count > 0).SelectMany(p => p.Session).ToList();
                    if (session != null && session.Count > 0)
                    {
                        _progress.Rotation = computeProgress(patient, patient.PatientRx.Where(s => s.DeviceConfiguration == "External Rotation").FirstOrDefault(), session, 0);
                    }
                    else
                    {
                        _progress.Rotation = 0;
                    }


                    break;
            }

            return _progress;
        }
        public PatientRx getById(string id)
        {
            return (from p in context.PatientRx.Include(x => x.Patient)
                   .Where(p => p.RxId == id)
                    select p).FirstOrDefault();

        }
        public PatientRx getByRxIDId(int id, string eenum)
        {
            return (from p in context.PatientRx.Include(x => x.Patient)
                             .Where(p => p.PatientId == Convert.ToInt32(id) && p.DeviceConfiguration == eenum)
                    select p).FirstOrDefault();
        }
        public List<PatientRx> getByProviderId(string id)
        {
            return (from p in context.PatientRx.Include(x => x.Patient)
                   .Where(p => p.ProviderId == id)
                    select p).ToList();
        }

        public PatientRx getByPatientIdAndEquipmentTypeAndProviderId(string id, string patientId, string equipmentType)
        {
            return (from p in context.PatientRx.Include(x => x.Patient)
                   .Where(p => p.ProviderId == id && p.PatientId == Convert.ToInt32(patientId) && p.EquipmentType.ToLower() == equipmentType.Trim().ToLower())
                    select p).FirstOrDefault();

        }

        public PatientRx getPatientRxByPEDP(string patientId, string EquipmentType, string DeviceConfiguration, string PatientSide)
        {
            return (from p in context.PatientRx
                    where p.PatientId == Convert.ToInt32(patientId) && p.EquipmentType.ToLower() == EquipmentType.Trim().ToLower() && p.DeviceConfiguration == DeviceConfiguration && p.PatientSide == PatientSide
                    select p).FirstOrDefault();

        }
        public PatientRx getPatientRx(int id, string equipmentType, string eenum)
        {
            return (from p in context.PatientRx.Include(x => x.Patient).
                   Include(x => x.Session)
                   .Where(p => p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum)
                    select p).FirstOrDefault();

        }


        public PatientRx getPatientRxPain(int id, string equipmentType, string eenum)
        {
            return (from p in context.PatientRx.
                   Include(x => x.Session)
                   .ThenInclude(y => y.Pain)
                   .Where(p => p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum)
                    select p).FirstOrDefault();

        }

        public List<ROMViewModel> getPatientRxEquipmentROM(int id, string equipmentType, string eenum)
        {
            List<ROMViewModel> result = new List<ROMViewModel>();
            List<Session> lsessionList = (from s in context.Session
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new ROMViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Flexion = grp.Max(m => m.MaxFlexion), Extension = Math.Abs(grp.Min(m => m.MaxExtension)), Pain = grp.Max(m => m.MaxPain) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Flexion == 0)
                    {
                        result[i].Flexion = result[i - 1].Flexion;
                    }
                    if (result[i].Extension == 0)
                    {
                        result[i].Extension = result[i - 1].Extension;
                    }
                }


            }

            return result;
        }

        public List<ROMViewModel> getPatientRxEquipmentROMByProtocol(int id, string equipmentType, string eenum, int protocolenum)
        {
            List<ROMViewModel> result = new List<ROMViewModel>();
            List<Session> lsessionList = (from s in context.Session.Include(x => x.Protocol)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum && s.Protocol.ProtocolEnum == protocolenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new ROMViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Flexion = grp.Max(m => m.MaxFlexion), Extension = Math.Abs(grp.Min(m => m.MaxExtension)), Pain = grp.Max(m => m.MaxPain) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Flexion == 0)
                    {
                        result[i].Flexion = result[i - 1].Flexion;
                    }
                    if (result[i].Extension == 0)
                    {
                        result[i].Extension = result[i - 1].Extension;
                    }
                }
            }

            return result;
        }
        public List<FlexionViewModel> getPatientRxEquipmentROMByFlexion(int id, string equipmentType, string eenum, int protocolenum)
        {
            List<FlexionViewModel> result = new List<FlexionViewModel>();
            List<Session> lsessionList = (from s in context.Session.Include(x => x.Protocol)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum && s.Protocol.ProtocolEnum == protocolenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new FlexionViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Flexion = grp.Max(m => m.MaxFlexion), Pain = grp.Max(m => m.MaxPain) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Flexion == 0)
                    {
                        result[i].Flexion = result[i - 1].Flexion;
                    }
                }
            }

            return result;
        }

        public List<HightStockShoulderViewModelForJavaScript> getPatientRxEquipmentROMByFlexionInHighStockChartForJavaScript(int id, string equipmentType, string eenum)
        {
            List<HightStockShoulderViewModelForJavaScript> result = new List<HightStockShoulderViewModelForJavaScript>();

            PatientRx lrx = context.PatientRx.FirstOrDefault(p => p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum);
            if (lrx != null)
            {
                DateTime EndDate = lrx.RxEndDate.Value > DateTime.Now.Date ? DateTime.Now.Date : lrx.RxEndDate.Value;
                List<DateTime> lrxDateList = Enumerable.Range(0, 1 + EndDate.Subtract(lrx.RxStartDate.Value).Days)
                .Select(offset => lrx.RxStartDate.Value.AddDays(offset))
                .ToList();



                List<HightStockShoulderViewModelForJavaScript> lseriesROM = lrxDateList.ConvertAll(x => new HightStockShoulderViewModelForJavaScript { X = Utilities.ConvertToUnixTime(x.Date), High = 0, Low = 0, Pain = 0, Session = 0 });

                List<Session> lsessionList = (from s in context.Session.Include(x => x.Protocol)
                                              join p in context.PatientRx on s.RxId equals p.RxId
                                              where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                              select s).ToList();


                if (lsessionList != null && lsessionList.Count > 0)
                {
                    var appleData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new HightStockShoulderViewModelForJavaScript { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), High = grp.Max(m => m.MaxFlexion), Low = grp.Min(m => m.Rx.CurrentFlexion).Value, Pain = grp.Max(m => m.MaxPain), Session = grp.Count() }).ToList();
                    lseriesROM = lseriesROM.Where(p => !appleData.Any(x => p.X == x.X)).ToList();
                    lseriesROM = lseriesROM.Concat(appleData).OrderBy(x => x.X).ToList();
                    result = lseriesROM;
                }
            }

            return result;
        }

        public List<HightStockShoulderViewModelForJavaScript> getPatientRxEquipmentROMByExtensionInHighStockChartForJavaScript(int id, string equipmentType, string eenum)
        {

            List<HightStockShoulderViewModelForJavaScript> result = new List<HightStockShoulderViewModelForJavaScript>();

            PatientRx lrx = context.PatientRx.FirstOrDefault(p => p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum);
            if (lrx != null)
            {
                DateTime EndDate = lrx.RxEndDate.Value > DateTime.Now.Date ? DateTime.Now.Date : lrx.RxEndDate.Value;
                List<DateTime> lrxDateList = Enumerable.Range(0, 1 + EndDate.Subtract(lrx.RxStartDate.Value).Days)
                .Select(offset => lrx.RxStartDate.Value.AddDays(offset))
                .ToList();


                List<HightStockShoulderViewModelForJavaScript> lseriesROM = lrxDateList.ConvertAll(x => new HightStockShoulderViewModelForJavaScript { X = Utilities.ConvertToUnixTime(x.Date), High = 0, Low = 0, Pain = 0, Session = 0 });

                List<Session> lsessionList = (from s in context.Session.Include(x => x.Protocol)
                                              join p in context.PatientRx on s.RxId equals p.RxId
                                              where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                              select s).ToList();


                if (lsessionList != null && lsessionList.Count > 0)
                {
                    var appleData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new HightStockShoulderViewModelForJavaScript { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), High = grp.Max(m => m.MaxExtension), Low = grp.Min(m => m.Rx.CurrentExtension).Value, Pain = grp.Max(m => m.MaxPain), Session = grp.Count() }).ToList();
                    lseriesROM = lseriesROM.Where(p => !appleData.Any(x => p.X == x.X)).ToList();
                    lseriesROM = lseriesROM.Concat(appleData).OrderBy(x => x.X).ToList();
                    result = lseriesROM;
                }
            }

            return result;


        }

        public List<HightStockShoulderViewModelForJavaScript> getPatientRxEquipmentROMByFlexionExtensionInHighStockChartForJavaScript(int id, string equipmentType, string eenum)
        {


            List<HightStockShoulderViewModelForJavaScript> result = new List<HightStockShoulderViewModelForJavaScript>();

            PatientRx lrx = context.PatientRx.FirstOrDefault(p => p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum);
            if (lrx != null)
            {
                DateTime EndDate = lrx.RxEndDate.Value > DateTime.Now.Date ? DateTime.Now.Date : lrx.RxEndDate.Value;
                List<DateTime> lrxDateList = Enumerable.Range(0, 1 + EndDate.Subtract(lrx.RxStartDate.Value).Days)
                .Select(offset => lrx.RxStartDate.Value.AddDays(offset))
                .ToList();

                List<HightStockShoulderViewModelForJavaScript> lseriesROM = lrxDateList.ConvertAll(x => new HightStockShoulderViewModelForJavaScript { X = Utilities.ConvertToUnixTime(x.Date), High = 0, Low = 0, Pain = 0, Session = 0 });

                List<Session> lsessionList = (from s in context.Session.Include(x => x.Protocol)
                                              join p in context.PatientRx on s.RxId equals p.RxId
                                              where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                              select s).ToList();


                if (lsessionList != null && lsessionList.Count > 0)
                {
                    var appleData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new HightStockShoulderViewModelForJavaScript { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), High = grp.Max(m => m.MaxExtension), Low = grp.Min(m => m.MaxFlexion), Pain = grp.Max(m => m.MaxPain), Session = grp.Count() }).ToList();
                    lseriesROM = lseriesROM.Where(p => !appleData.Any(x => p.X == x.X)).ToList();
                    lseriesROM = lseriesROM.Concat(appleData).OrderBy(x => x.X).ToList();
                    result = lseriesROM;
                }
            }


            return result;


        }

        public List<HightStockShoulderViewModelForJavaScript> getPatientRxEquipmentROMForShoulderInHighStockChartForJavaScript(int id, string equipmentType, string eenum)
        {

            List<HightStockShoulderViewModelForJavaScript> result = new List<HightStockShoulderViewModelForJavaScript>();

            PatientRx lrx = context.PatientRx.FirstOrDefault(p => p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum);
            if (lrx != null)
            {
                DateTime EndDate = lrx.RxEndDate.Value > DateTime.Now.Date ? DateTime.Now.Date : lrx.RxEndDate.Value;
                List<DateTime> lrxDateList = Enumerable.Range(0, 1 + EndDate.Subtract(lrx.RxStartDate.Value).Days)
                .Select(offset => lrx.RxStartDate.Value.AddDays(offset))
                .ToList();

                List<HightStockShoulderViewModelForJavaScript> lseriesROM = lrxDateList.ConvertAll(x => new HightStockShoulderViewModelForJavaScript { X = Utilities.ConvertToUnixTime(x.Date), High = 0, Low = 0, Pain = 0, Session = 0 });

                List<Session> lsessionList = (from s in context.Session
                                              join p in context.PatientRx on s.RxId equals p.RxId
                                              where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                              select s).ToList();
                if (lsessionList != null && lsessionList.Count > 0)
                {
                    var appleData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new HightStockShoulderViewModelForJavaScript { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), High = grp.Max(m => m.MaxFlexion), Low = grp.Min(m => m.Rx.CurrentFlexion).Value, Pain = grp.Max(m => m.MaxPain), Session = grp.Count() }).ToList();
                    lseriesROM = lseriesROM.Where(p => !appleData.Any(x => p.X == x.X)).ToList();
                    lseriesROM = lseriesROM.Concat(appleData).OrderBy(x => x.X).ToList();
                    result = lseriesROM;
                }
            }
            return result;
        }


        public HightStockShoulderViewModel getPatientRxEquipmentROMByFlexionInHighStockChart(int id, string equipmentType, string eenum)
        {
            List<ColumnrangeSeriesData> appleData = new List<ColumnrangeSeriesData>();
            List<ColumnSeriesData> volumeData = new List<ColumnSeriesData>();
            List<ColumnSeriesData> painvolumeData = new List<ColumnSeriesData>();
            HightStockShoulderViewModel result = new HightStockShoulderViewModel();

            PatientRx lrx = context.PatientRx.FirstOrDefault(p => p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum);
            if (lrx != null)
            {
                DateTime EndDate = lrx.RxEndDate.Value > DateTime.Now.Date ? DateTime.Now.Date : lrx.RxEndDate.Value;
                List<DateTime> lrxDateList = Enumerable.Range(0, 1 + EndDate.Subtract(lrx.RxStartDate.Value).Days)
                .Select(offset => lrx.RxStartDate.Value.AddDays(offset))
                .ToList();


                List<ColumnSeriesData> lseries = lrxDateList.ConvertAll(x => new ColumnSeriesData { X = Utilities.ConvertToUnixTime(x.Date), Y = 0, Color = Utilities.getHighStockColor(0) });

                List<ColumnrangeSeriesData> lseriesROM = lrxDateList.ConvertAll(x => new ColumnrangeSeriesData { X = Utilities.ConvertToUnixTime(x.Date), High = 0, Low = 0 });

                List<Session> lsessionList = (from s in context.Session.Include(x => x.Protocol)
                                              join p in context.PatientRx on s.RxId equals p.RxId
                                              where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                              select s).ToList();


                if (lsessionList != null && lsessionList.Count > 0)
                {
                    appleData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new ColumnrangeSeriesData { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), High = grp.Max(m => m.MaxFlexion), Low = grp.Min(m => m.Rx.CurrentFlexion) }).ToList();
                    lseriesROM = lseriesROM.Where(p => !appleData.Any(x => p.X == x.X)).ToList();
                    lseriesROM = lseriesROM.Concat(appleData).OrderBy(x => x.X).ToList();
                    result.ROM = lseriesROM;


                    volumeData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new ColumnSeriesData { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), Y = grp.Count(), Color = Utilities.getHighStockColor(grp.Count()) }).ToList();
                    List<ColumnSeriesData> lseriesVolumn = lseries.Where(p => !volumeData.Any(x => p.X == x.X)).ToList();
                    lseriesVolumn = lseriesVolumn.Concat(volumeData).OrderBy(x => x.X).ToList();
                    result.Volume = lseriesVolumn;

                    painvolumeData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new ColumnSeriesData { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), Y = grp.Max(m => m.MaxPain), Color = Utilities.getHighStockColor(grp.Max(m => m.MaxPain)) }).ToList();
                    List<ColumnSeriesData> lseriesPain = lseries.Where(p => !painvolumeData.Any(x => p.X == x.X)).ToList();
                    lseriesPain = lseriesPain.Concat(painvolumeData).OrderBy(x => x.X).ToList();
                    result.PainVolume = lseriesPain;
                }
            }
            //if (result != null && result.Count > 0)
            //{
            //    for (int i = 1; i < result.Count; i++)
            //    {
            //        if (result[i].Flexion == 0)
            //        {
            //            result[i].Flexion = result[i - 1].Flexion;
            //        }
            //    }


            //}

            return result;
        }


        public List<FlexionViewModel> getPatientRxEquipmentROMByFlexion(int id, string equipmentType, string eenum)
        {
            List<FlexionViewModel> result = new List<FlexionViewModel>();
            List<Session> lsessionList = (from s in context.Session.Include(x => x.Protocol)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new FlexionViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Flexion = grp.Max(m => m.MaxFlexion), Pain = grp.Max(m => m.MaxPain) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Flexion == 0)
                    {
                        result[i].Flexion = result[i - 1].Flexion;
                    }
                }
            }

            return result;
        }



        public HightStockShoulderViewModel getPatientRxEquipmentROMByExtensionInHighStockChart(int id, string equipmentType, string eenum)
        {
            List<ColumnrangeSeriesData> appleData = new List<ColumnrangeSeriesData>();
            List<ColumnSeriesData> volumeData = new List<ColumnSeriesData>();
            List<ColumnSeriesData> painvolumeData = new List<ColumnSeriesData>();
            HightStockShoulderViewModel result = new HightStockShoulderViewModel();

            PatientRx lrx = context.PatientRx.FirstOrDefault(p => p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum);
            if (lrx != null)
            {
                DateTime EndDate = lrx.RxEndDate.Value > DateTime.Now.Date ? DateTime.Now.Date : lrx.RxEndDate.Value;
                List<DateTime> lrxDateList = Enumerable.Range(0, 1 + EndDate.Subtract(lrx.RxStartDate.Value).Days)
                .Select(offset => lrx.RxStartDate.Value.AddDays(offset))
                .ToList();

                List<ColumnSeriesData> lseries = lrxDateList.ConvertAll(x => new ColumnSeriesData { X = Utilities.ConvertToUnixTime(x.Date), Y = 0, Color = Utilities.getHighStockColor(0) });

                List<ColumnrangeSeriesData> lseriesROM = lrxDateList.ConvertAll(x => new ColumnrangeSeriesData { X = Utilities.ConvertToUnixTime(x.Date), High = 0, Low = 0, Color = "#7cb5ec" });

                List<Session> lsessionList = (from s in context.Session.Include(x => x.Protocol)
                                              join p in context.PatientRx on s.RxId equals p.RxId
                                              where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                              select s).ToList();


                if (lsessionList != null && lsessionList.Count > 0)
                {
                    appleData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new ColumnrangeSeriesData { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), High = grp.Max(m => m.MaxExtension), Low = grp.Min(m => m.Rx.CurrentExtension) }).ToList();
                    lseriesROM = lseriesROM.Where(p => !appleData.Any(x => p.X == x.X)).ToList();
                    lseriesROM = lseriesROM.Concat(appleData).OrderBy(x => x.X).ToList();
                    result.ROM = lseriesROM;


                    volumeData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new ColumnSeriesData { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), Y = grp.Count(), Color = Utilities.getHighStockColor(grp.Count()) }).ToList();
                    List<ColumnSeriesData> lseriesVolumn = lseries.Where(p => !volumeData.Any(x => p.X == x.X)).ToList();
                    lseriesVolumn = lseriesVolumn.Concat(volumeData).OrderBy(x => x.X).ToList();
                    result.Volume = lseriesVolumn;

                    painvolumeData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new ColumnSeriesData { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), Y = grp.Max(m => m.MaxPain), Color = Utilities.getHighStockColor(grp.Max(m => m.MaxPain)) }).ToList();
                    List<ColumnSeriesData> lseriesPain = lseries.Where(p => !painvolumeData.Any(x => p.X == x.X)).ToList();
                    lseriesPain = lseriesPain.Concat(painvolumeData).OrderBy(x => x.X).ToList();
                    result.PainVolume = lseriesPain;
                }
            }
            //if (result != null && result.Count > 0)
            //{
            //    for (int i = 1; i < result.Count; i++)
            //    {
            //        if (result[i].Flexion == 0)
            //        {
            //            result[i].Flexion = result[i - 1].Flexion;
            //        }
            //    }


            //}

            return result;


        }


        public HightStockShoulderViewModel getPatientRxEquipmentROMByFlexionExtensionInHighStockChart(int id, string equipmentType, string eenum)
        {

            List<ColumnrangeSeriesData> appleData = new List<ColumnrangeSeriesData>();
            List<ColumnSeriesData> volumeData = new List<ColumnSeriesData>();
            List<ColumnSeriesData> painvolumeData = new List<ColumnSeriesData>();
            HightStockShoulderViewModel result = new HightStockShoulderViewModel();

            PatientRx lrx = context.PatientRx.FirstOrDefault(p => p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum);
            if (lrx != null)
            {
                DateTime EndDate = lrx.RxEndDate.Value > DateTime.Now.Date ? DateTime.Now.Date : lrx.RxEndDate.Value;
                List<DateTime> lrxDateList = Enumerable.Range(0, 1 + EndDate.Subtract(lrx.RxStartDate.Value).Days)
                .Select(offset => lrx.RxStartDate.Value.AddDays(offset))
                .ToList();
                //List<DateTime> lrxDateList = Enumerable.Range(0, 1 + lrx.RxEndDate.Value.Subtract(lrx.RxStartDate.Value).Days)
                //.Select(offset => lrx.RxStartDate.Value.AddDays(offset))
                //.ToList();


                List<ColumnSeriesData> lseries = lrxDateList.ConvertAll(x => new ColumnSeriesData { X = Utilities.ConvertToUnixTime(x.Date), Y = 0, Color = Utilities.getHighStockColor(0) });

                List<ColumnrangeSeriesData> lseriesROM = lrxDateList.ConvertAll(x => new ColumnrangeSeriesData { X = Utilities.ConvertToUnixTime(x.Date), High = 0, Low = 0, Color = "#7cb5ec" });

                List<Session> lsessionList = (from s in context.Session.Include(x => x.Protocol)
                                              join p in context.PatientRx on s.RxId equals p.RxId
                                              where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                              select s).ToList();


                if (lsessionList != null && lsessionList.Count > 0)
                {
                    appleData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new ColumnrangeSeriesData { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), High = grp.Max(m => m.MaxExtension), Low = grp.Min(m => m.MaxFlexion) }).ToList();
                    lseriesROM = lseriesROM.Where(p => !appleData.Any(x => p.X == x.X)).ToList();
                    lseriesROM = lseriesROM.Concat(appleData).OrderBy(x => x.X).ToList();
                    result.ROM = lseriesROM;


                    volumeData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new ColumnSeriesData { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), Y = grp.Count(), Color = Utilities.getHighStockColor(grp.Count()) }).ToList();
                    List<ColumnSeriesData> lseriesVolumn = lseries.Where(p => !volumeData.Any(x => p.X == x.X)).ToList();
                    lseriesVolumn = lseriesVolumn.Concat(volumeData).OrderBy(x => x.X).ToList();
                    result.Volume = lseriesVolumn;

                    painvolumeData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new ColumnSeriesData { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), Y = grp.Max(m => m.MaxPain), Color = Utilities.getHighStockColor(grp.Max(m => m.MaxPain)) }).ToList();
                    List<ColumnSeriesData> lseriesPain = lseries.Where(p => !painvolumeData.Any(x => p.X == x.X)).ToList();
                    lseriesPain = lseriesPain.Concat(painvolumeData).OrderBy(x => x.X).ToList();
                    result.PainVolume = lseriesPain;
                }
            }
            //if (result != null && result.Count > 0)
            //{
            //    for (int i = 1; i < result.Count; i++)
            //    {
            //        if (result[i].Flexion == 0)
            //        {
            //            result[i].Flexion = result[i - 1].Flexion;
            //        }
            //    }


            //}

            return result;


        }

        public List<ExtensionViewModel> getPatientRxEquipmentROMByExtension(int id, string equipmentType, string eenum, int protocolenum)
        {
            List<ExtensionViewModel> result = new List<ExtensionViewModel>();
            List<Session> lsessionList = (from s in context.Session.Include(x => x.Protocol)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum && s.Protocol.ProtocolEnum == protocolenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new ExtensionViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Extension = Math.Abs(grp.Min(m => m.MaxExtension)), Pain = grp.Max(m => m.MaxPain) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Extension == 0)
                    {
                        result[i].Extension = result[i - 1].Extension;
                    }
                }
            }

            return result;
        }
        public List<ExtensionViewModel> getPatientRxEquipmentROMByExtension(int id, string equipmentType, string eenum)
        {
            List<ExtensionViewModel> result = new List<ExtensionViewModel>();
            List<Session> lsessionList = (from s in context.Session.Include(x => x.Protocol)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new ExtensionViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Extension = grp.Min(m => m.MaxExtension), Pain = grp.Max(m => m.MaxPain) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Extension == 0)
                    {
                        result[i].Extension = result[i - 1].Extension;
                    }
                }
            }

            return result;
        }


        public HightStockShoulderViewModel getPatientRxEquipmentROMForShoulderInHighStockChart(int id, string equipmentType, string eenum)
        {
            List<ColumnrangeSeriesData> appleData = new List<ColumnrangeSeriesData>();
            List<ColumnSeriesData> volumeData = new List<ColumnSeriesData>();
            List<ColumnSeriesData> painvolumeData = new List<ColumnSeriesData>();
            HightStockShoulderViewModel result = new HightStockShoulderViewModel();

            PatientRx lrx = context.PatientRx.FirstOrDefault(p => p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum);
            if (lrx != null)
            {
                DateTime EndDate = lrx.RxEndDate.Value > DateTime.Now.Date ? DateTime.Now.Date : lrx.RxEndDate.Value;
                List<DateTime> lrxDateList = Enumerable.Range(0, 1 + EndDate.Subtract(lrx.RxStartDate.Value).Days)
                .Select(offset => lrx.RxStartDate.Value.AddDays(offset))
                .ToList();
                //List<DateTime> lrxDateList = Enumerable.Range(0, 1 + lrx.RxEndDate.Value.Subtract(lrx.RxStartDate.Value).Days)
                //.Select(offset => lrx.RxStartDate.Value.AddDays(offset))
                //.ToList();


                List<ColumnSeriesData> lseries = lrxDateList.ConvertAll(x => new ColumnSeriesData { X = Utilities.ConvertToUnixTime(x.Date), Y = 0, Color = Utilities.getHighStockColor(0) });

                List<ColumnrangeSeriesData> lseriesROM = lrxDateList.ConvertAll(x => new ColumnrangeSeriesData { X = Utilities.ConvertToUnixTime(x.Date), High = 0, Low = 0 });

                List<Session> lsessionList = (from s in context.Session
                                              join p in context.PatientRx on s.RxId equals p.RxId
                                              where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                              select s).ToList();


                if (lsessionList != null && lsessionList.Count > 0)
                {
                    appleData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new ColumnrangeSeriesData { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), High = grp.Max(m => m.MaxFlexion), Low = grp.Min(m => m.Rx.CurrentFlexion) }).ToList();
                    lseriesROM = lseriesROM.Where(p => !appleData.Any(x => p.X == x.X)).ToList();
                    lseriesROM = lseriesROM.Concat(appleData).OrderBy(x => x.X).ToList();
                    result.ROM = lseriesROM;


                    volumeData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new ColumnSeriesData { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), Y = grp.Count(), Color = Utilities.getHighStockColor(grp.Count()) }).ToList();
                    List<ColumnSeriesData> lseriesVolumn = lseries.Where(p => !volumeData.Any(x => p.X == x.X)).ToList();
                    lseriesVolumn = lseriesVolumn.Concat(volumeData).OrderBy(x => x.X).ToList();
                    result.Volume = lseriesVolumn;

                    painvolumeData = lsessionList.GroupBy(x => x.SessionDate.Value.Date.Date).Select(grp => new ColumnSeriesData { X = Utilities.ConvertToUnixTime(grp.Key.Date.Date), Y = grp.Max(m => m.MaxPain), Color = Utilities.getHighStockColor(grp.Max(m => m.MaxPain)) }).ToList();
                    List<ColumnSeriesData> lseriesPain = lseries.Where(p => !painvolumeData.Any(x => p.X == x.X)).ToList();
                    lseriesPain = lseriesPain.Concat(painvolumeData).OrderBy(x => x.X).ToList();
                    result.PainVolume = lseriesPain;
                }
            }
            //if (result != null && result.Count > 0)
            //{
            //    for (int i = 1; i < result.Count; i++)
            //    {
            //        if (result[i].Flexion == 0)
            //        {
            //            result[i].Flexion = result[i - 1].Flexion;
            //        }
            //    }


            //}

            return result;



        }


        public List<ShoulderViewModel> getPatientRxEquipmentROMForShoulder(int id, string equipmentType, string eenum)
        {
            List<ShoulderViewModel> result = new List<ShoulderViewModel>();
            List<Session> lsessionList = (from s in context.Session
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new ShoulderViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Flexion = grp.Max(m => m.MaxFlexion), Pain = grp.Max(m => m.MaxPain) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Flexion == 0)
                    {
                        result[i].Flexion = result[i - 1].Flexion;
                    }
                }


            }

            return result;
        }



        public ROMChartViewModel getPatientRxROMChart(int id, string equipmentType, string eenum)
        {
            ROMChartViewModel result = new ROMChartViewModel();

            List<Session> lsessionList = (from s in context.Session
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                          select s).OrderBy(x => x.SessionDate).ToList();

            if (lsessionList != null && lsessionList.Count > 0)
            {


                List<Session> lsessionFlexion = lsessionList.Where(m => m.MaxFlexion != 0).OrderBy(x => x.SessionDate).ToList();
                List<Session> lsessionExtension = lsessionList.Where(m => m.MaxExtension != 0).OrderBy(x => x.SessionDate).ToList();

                if (lsessionFlexion != null && lsessionFlexion.Count > 0)
                {
                    var maxFlex = lsessionFlexion.Max(x => x.MaxFlexion);
                    var minFlex = lsessionFlexion.Max(x => x.MaxFlexion);
                    var flexincValu = Math.Abs((decimal)maxFlex / lsessionFlexion.Count);
                    decimal startFlex = 0;
                    List<ROMChartFlexion> resultFlex = new List<ROMChartFlexion>();
                    for (int i = 0; i < lsessionFlexion.Count; i++)
                    {
                        startFlex = startFlex + flexincValu;
                        ROMChartFlexion lr = new ROMChartFlexion() { Flexion = lsessionFlexion[i].MaxFlexion, dateValue = Math.Round(startFlex, 2) };
                        resultFlex.Add(lr);
                    }
                    result.ROMFlextion = resultFlex;
                    result.ymax = maxFlex + Convert.ToInt32((maxFlex * (20.0 / 100)));
                    result.ymin = minFlex - Convert.ToInt32((minFlex * (20.0 / 100)));
                }
                else
                {
                    result.ymax = 0;
                    result.ymin = 0;

                }

                if (lsessionExtension != null && lsessionExtension.Count > 0)
                {
                    var maxExt = (lsessionExtension != null && lsessionExtension.Count > 0) ? lsessionExtension.Max(x => x.MaxExtension) : 0;
                    var minExt = (lsessionExtension != null && lsessionExtension.Count > 0) ? lsessionExtension.Min(x => x.MaxExtension) : 0;
                    var extincValu = Math.Abs((decimal)maxExt / lsessionExtension.Count);
                    decimal startExt = 0;
                    List<ROMChartExtension> resultExt = new List<ROMChartExtension>();
                    for (int i = 0; i < lsessionExtension.Count; i++)
                    {
                        startExt = startExt + extincValu;
                        ROMChartExtension lr = new ROMChartExtension() { Extension = lsessionExtension[i].MaxExtension, dateValue = Math.Round(startExt, 2) };
                        resultExt.Add(lr);
                    }
                    result.ROMExtension = resultExt;
                    var ymin = minExt - Convert.ToInt32((minExt * (20.0 / 100)));
                    var ymax = maxExt + Convert.ToInt32((maxExt * (20.0 / 100)));

                    result.ymax = result.ymax > ymax ? result.ymax : ymax;
                    result.ymin = result.ymin < ymin ? result.ymin : ymin;
                }

            }



            return result;

        }


        public DashboardView getPatientRxROM(int id, string equipmentType, string eenum)
        {
            return (from p in context.PatientRx.Include(x => x.Patient).
                    Include(x => x.Session)
                    .Where(p => p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum)

                    select new DashboardView
                    {
                        PatientRx = p,
                        FirstUse = p.Session.Min(t => t.SessionDate),
                        LastUse = p.Session.Max(t => t.SessionDate),
                        MaxPain = p.Session.Max(t => t.MaxPain),
                        UpRom1 = p.Session.Max(t => t.MaxFlexion),
                        DownRom1 = p.Session.Where(x => x.MaxExtension != null && x.MaxExtension > 0).Min(t => t.MaxExtension),
                        TotalSession = p.Session.Count(),
                    }).FirstOrDefault();

        }
        public List<ROMViewModel> getPatientRxCompliance(int id, string equipmentType, string eenum)
        {
            List<ROMViewModel> result = new List<ROMViewModel>();
            List<Session> lsessionList = (from s in context.Session.Include(s => s.Pain)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new ROMViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Flexion = Math.Abs(grp.Sum(m => m.FlexionReps.Value)), Extension = Math.Abs(grp.Sum(m => m.ExtensionReps.Value)), Pain = grp.Sum(m => m.Pain.Count) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Flexion == 0)
                    {
                        result[i].Flexion = result[i - 1].Flexion;
                    }
                    if (result[i].Extension == 0)
                    {
                        result[i].Extension = result[i - 1].Extension;
                    }
                }


            }
            return result;
        }
        public List<ROMViewModel> getPatientRxComplianceByProtocol(int id, string equipmentType, string eenum, int protocolenum)
        {
            List<ROMViewModel> result = new List<ROMViewModel>();
            List<Session> lsessionList = (from s in context.Session.Include(s => s.Pain).Include(x => x.Protocol)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum && s.Protocol.ProtocolEnum == protocolenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new ROMViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Flexion = Math.Abs(grp.Sum(m => m.FlexionReps.Value)), Extension = Math.Abs(grp.Sum(m => m.ExtensionReps.Value)), Pain = grp.Sum(m => m.Pain.Count) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Flexion == 0)
                    {
                        result[i].Flexion = result[i - 1].Flexion;
                    }
                    if (result[i].Extension == 0)
                    {
                        result[i].Extension = result[i - 1].Extension;
                    }
                }


            }
            return result;
        }


        public List<FlexionViewModel> getPatientRxComplianceByFlexion(int id, string equipmentType, string eenum, int protocolenum)
        {
            List<FlexionViewModel> result = new List<FlexionViewModel>();
            List<Session> lsessionList = (from s in context.Session.Include(s => s.Pain).Include(x => x.Protocol)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum && s.Protocol.ProtocolEnum == protocolenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new FlexionViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Flexion = Math.Abs(grp.Sum(m => m.FlexionReps.Value)), Pain = grp.Sum(m => m.Pain.Count) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Flexion == 0)
                    {
                        result[i].Flexion = result[i - 1].Flexion;
                    }
                }


            }
            return result;
        }

        public List<FlexionViewModel> getPatientRxComplianceByFlexion(int id, string equipmentType, string eenum)
        {
            List<FlexionViewModel> result = new List<FlexionViewModel>();
            List<Session> lsessionList = (from s in context.Session.Include(s => s.Pain).Include(x => x.Protocol)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new FlexionViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Flexion = grp.Sum(m => m.Reps.Value), Pain = grp.Sum(m => m.Pain.Count) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Flexion == 0)
                    {
                        result[i].Flexion = result[i - 1].Flexion;
                    }
                }


            }
            return result;
        }
        public List<ExtensionViewModel> getPatientRxComplianceByExtension(int id, string equipmentType, string eenum, int protocolenum)
        {
            List<ExtensionViewModel> result = new List<ExtensionViewModel>();
            List<Session> lsessionList = (from s in context.Session.Include(s => s.Pain).Include(x => x.Protocol)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum && s.Protocol.ProtocolEnum == protocolenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new ExtensionViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Extension = Math.Abs(grp.Sum(m => m.ExtensionReps.Value)), Pain = grp.Sum(m => m.Pain.Count) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Extension == 0)
                    {
                        result[i].Extension = result[i - 1].Extension;
                    }
                }


            }
            return result;
        }

        public List<ExtensionViewModel> getPatientRxComplianceByExtension(int id, string equipmentType, string eenum)
        {
            List<ExtensionViewModel> result = new List<ExtensionViewModel>();
            List<Session> lsessionList = (from s in context.Session.Include(s => s.Pain).Include(x => x.Protocol)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new ExtensionViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Extension = grp.Sum(m => m.Reps.Value), Pain = grp.Sum(m => m.Pain.Count) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Extension == 0)
                    {
                        result[i].Extension = result[i - 1].Extension;
                    }
                }
            }
            return result;
        }


        public List<ShoulderViewModel> getPatientRxComplianceForShoulder(int id, string equipmentType, string eenum)
        {
            List<ShoulderViewModel> result = new List<ShoulderViewModel>();
            List<Session> lsessionList = (from s in context.Session.Include(s => s.Pain)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                          select s).ToList();
            if (lsessionList != null && lsessionList.Count > 0)
            {
                result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new ShoulderViewModel { dateValue = grp.Key.Date.ToString("yyyy-MM-dd"), Flexion = Math.Abs(grp.Sum(m => m.Reps.Value)), Pain = grp.Sum(m => m.Pain.Count) }).OrderBy(x => x.dateValue).ToList();
            }
            if (result != null && result.Count > 0)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i].Flexion == 0)
                    {
                        result[i].Flexion = result[i - 1].Flexion;
                    }

                }


            }
            return result;
        }

        public List<TreatmentCalendarViewModel> getTreatmentCalendar(int id, string equipmentType, string eenum)
        {
            PatientRx lrx = (from p in context.PatientRx where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum select p).FirstOrDefault();
            if (lrx != null)
            {
                DateTime EndDate = lrx.RxEndDate.Value > DateTime.Now.Date ? DateTime.Now.Date : lrx.RxEndDate.Value;
                List<TreatmentCalendarViewModel> lrxSessionList = Enumerable.Range(0, 1 + EndDate.Subtract(lrx.RxStartDate.Value).Days)
                .Select(offset => new TreatmentCalendarViewModel { title = "0", start = lrx.RxStartDate.Value.AddDays(offset), backgroundColor = "#e52929", borderColor = "#f56954" })
                .ToList();

                List<TreatmentCalendarViewModel> result = new List<TreatmentCalendarViewModel>();
                List<Session> lsessionList = (from s in context.Session
                                              join p in context.PatientRx on s.RxId equals p.RxId
                                              where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                              select s).ToList();
                if (lsessionList != null && lsessionList.Count > 0)
                {
                    result = lsessionList.GroupBy(x => x.SessionDate.Value.Date).Select(grp => new TreatmentCalendarViewModel { title = grp.Count().ToString(), start = grp.Key.Date, backgroundColor = "green", borderColor = "#f56954" }).OrderBy(x => x.start).ToList();
                    lrxSessionList = lrxSessionList.Where(p => !result.Any(x => p.start == x.start)).ToList();
                    lrxSessionList = lrxSessionList.Concat(result).OrderBy(x => x.start).ToList();
                    //result = lrxSessionList;
                }
                result = lrxSessionList;

                return result;
            }
            return null;
        }

        public string DeletePatientRecordsWithCasecade(int patid)
        {

            try
            {



                var pain = (from p in context.Pain where p.PatientId == patid select p).ToList();
                context.Pain.RemoveRange(pain);
                context.SaveChanges();
                var session = (from p in context.Session where p.PatientId == patid select p).ToList();
                context.Session.RemoveRange(session);
                context.SaveChanges();
                var protocol = (from p in context.Protocol where p.PatientId == patid select p).ToList();
                context.Protocol.RemoveRange(protocol);
                context.SaveChanges();
                var Rx = (from p in context.PatientRx where p.PatientId == patid select p).ToList();
                context.PatientRx.RemoveRange(Rx);
                context.SaveChanges();
                var patconfig = (from p in context.PatientConfiguration where p.PatientId == patid select p).ToList();
                context.PatientConfiguration.RemoveRange(patconfig);
                context.SaveChanges();

                string PatientId = patid.ToString();
                var patreviews = (from p in context.PatientReview where p.PatientId == PatientId select p).ToList();
                if (patreviews != null && patreviews.Count > 0)
                {
                    string reviewIdList = string.Join(",", patreviews.Select(m => m.ReviewId));
                    var userActivitys = context.UserActivityLog.Where(x => reviewIdList.Contains(x.ReviewId)).ToList();
                    if (userActivitys != null && userActivitys.Count > 0)
                    {
                        context.UserActivityLog.RemoveRange(userActivitys);
                        context.SaveChanges();
                    }
                    context.PatientReview.RemoveRange(patreviews);
                    context.SaveChanges();
                }


                Patient lpatient = (from p in context.Patient where p.PatientId == patid select p).FirstOrDefault();
                if (lpatient != null)
                {
                    var equipment = (from p in context.EquipmentAssignment where p.PatientId == lpatient.PatientId select p).ToList();
                    if (equipment.Count > 0)
                    {
                        context.EquipmentAssignment.RemoveRange(equipment);
                        context.SaveChanges();
                    }
                    var appointments = (from p in context.AppointmentSchedule where p.PatientId == lpatient.PatientId select p).ToList();
                    context.AppointmentSchedule.RemoveRange(appointments);
                    context.SaveChanges();

                    //var logs = (from p in context.TransactionLog where p.PatientUserId == lpatient.PatientLoginId select p).ToList();
                    //context.TransactionLog.RemoveRange(logs);
                    //context.SaveChanges();

                    var messages = (from p in context.Messages where p.UserId == lpatient.PatientLoginId select p).ToList();
                    context.Messages.RemoveRange(messages);
                    context.SaveChanges();

                    var messagesrecevived = (from p in context.Messages where p.PatientId == lpatient.PatientLoginId select p).ToList();
                    context.Messages.RemoveRange(messagesrecevived);
                    context.SaveChanges();

                    var user = (from p in context.User where p.UserId == lpatient.PatientLoginId select p).ToList();
                    context.User.RemoveRange(user);
                    context.SaveChanges();

                }

                var pat = (from p in context.Patient where p.PatientId == patid select p).ToList();
                context.Patient.RemoveRange(pat);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
        }
        public List<Session> getCurrentSessions(int id, string equipmentType, string eenum)
        {
            List<Session> lseesionList = (from s in context.Session.Include(s => s.Protocol)
                                          join p in context.PatientRx on s.RxId equals p.RxId
                                          where p.PatientId == id && p.EquipmentType.ToLower() == equipmentType.ToLower() && p.DeviceConfiguration == eenum
                                          select s).OrderByDescending(x => x.SessionDate).ToList();

            return lseesionList;

        }

        private string getProtocolType(string limb, int? proenum, string exenum, List<ExcerciseProtocol> ExcerciseProtocollist)
        {
            //List<ExcerciseProtocol> ExcerciseProtocollist = lIEquipmentExerciseRepository.GetExcerciseProtocol();
            if (ExcerciseProtocollist != null && ExcerciseProtocollist.Count > 0)
            {
                ExcerciseProtocol _EquipmentExcercise = ExcerciseProtocollist.Where(p => p.Limb.ToLower() == limb.ToLower() && p.ProtocolEnum == Convert.ToInt32(proenum) && p.ExcerciseEnum == exenum).FirstOrDefault();
                return _EquipmentExcercise != null ? _EquipmentExcercise.ProtocolName : "";
            }
            return "";
        }
        public PatientRx getPatientRx(string lRxID)
        {
            return (from p in context.PatientRx
                    where p.RxId == lRxID
                    select p).FirstOrDefault();

        }
        public List<PatientRx> getPatientRxByPatientId(string lPatientId)
        {
            return (from p in context.PatientRx
                    where p.PatientId == Convert.ToInt32(lPatientId)
                    select p).ToList();
        }
        public void InsertPatientRx(PatientRx pPatientRx)
        {
            if (pPatientRx != null)
            {
                context.PatientRx.Add(pPatientRx);
                context.SaveChanges();
            }
        }

        public void DeletePatientRx(PatientRx pPatientRx)
        {
            context.PatientRx.Remove(pPatientRx);
            context.SaveChanges();
        }

        public void UpdatePatientRx(PatientRx patientRx)
        {
            PatientRx _patientrx = (from p in context.PatientRx
                                    where p.RxId == patientRx.RxId
                                    select p).FirstOrDefault();
            if (_patientrx != null)
            {
                _patientrx.RxStartDate = patientRx.RxStartDate;
                _patientrx.RxEndDate = patientRx.RxEndDate;
                _patientrx.RxDaysPerweek = (int)patientRx.RxDaysPerweek;
                _patientrx.RxSessionsPerWeek = (int)patientRx.RxSessionsPerWeek;
                _patientrx.RxDays = patientRx.RxDays;

                _patientrx.DateModified = patientRx.DateModified;
                context.Entry(_patientrx).State = EntityState.Modified;
                context.SaveChanges();
            }
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
