using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OneDirect.Helper;
using OneDirect.Models;
using OneDirect.Repository.Interface;
using OneDirect.Response;
using OneDirect.ViewModels;
using OneDirect.Vsee;
using OneDirect.VSee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository
{
    public class UserRepository : IUserInterface
    {
        private OneDirectContext context;

        public UserRepository(OneDirectContext context)
        {
            this.context = context;
        }

        public User getUser(string lUserID, string password)
        {
            return (from p in context.User
                    where p.UserId == lUserID && p.Password == password
                    select p).FirstOrDefault();
        }

        public User userLogin(string lUserID, string password, int type)
        {
            User luser = (from p in context.User
                          where p.UserId == lUserID && p.Password == password && p.Type == type
                          select p).FirstOrDefault();
            if (luser != null)
            {
                luser.LoginSessionId = Guid.NewGuid().ToString();
                context.Entry(luser).State = EntityState.Modified;
                int result = context.SaveChanges();
                if (result > 0)
                {
                    return luser;
                }
            }
            return null;
        }
        public User getUserbySessionId(string sessionId)
        {
            return (from p in context.User
                    where p.LoginSessionId == sessionId
                    select p).FirstOrDefault();
        }
        public User getUser(string lUserID, string password, int type)
        {
            return (from p in context.User
                    where p.UserId == lUserID && p.Password == password && p.Type == type
                    select p).FirstOrDefault();
        }
        public User getUserWithEncryptPassword(string lUserID, string password, int type)
        {
            return (from p in context.User
                    where p.UserId == lUserID && p.EncryptPasswrod == password && p.Type == type
                    select p).FirstOrDefault();
        }
        //kajal
        public User getUserNpi(string lNpi)
        {
            return (from p in context.User
                    where p.Npi == lNpi
                    select p).FirstOrDefault();
        }


        public User getUser(string lUserID)
        {
            return (from p in context.User
                    where p.UserId == lUserID
                    select p).FirstOrDefault();
        }

        public int deleteUser(User luser)
        {
            context.User.Remove(luser);
            return context.SaveChanges();
        }
        public EquipmentAssignment getEquUser(string lUserID)
        {
            return (from p in context.EquipmentAssignment
                    where p.PatientId == Convert.ToInt32(lUserID) && p.InstallerId == "th2"
                    select p).FirstOrDefault();
        }
        public List<User> getUserData(string lUserID)
        {
            return (from p in context.User
                    where p.UserId == lUserID
                    select p).ToList();
        }

        public List<User> getUserListByType(int lUserType)
        {

            return (from p in context.User
                    let cCount = (from pat in context.Patient where (p.Type == 2 ? pat.Therapistid == p.UserId : pat.ProviderId == p.UserId) select pat).Count()
                    where p.Type == lUserType
                    select new User
                    {
                        UserId = p.UserId,
                        Type = p.Type,
                        Name = p.Name,
                        Email = p.Email,
                        Phone = p.Phone,
                        Npi = p.Npi,

                        Address = p.Address,
                        Password = p.Password
                    }).ToList();
        }

        public List<UserListView> getInstallerUserList()
        {

            return (from p in context.User
                    let cCount = context.DeviceCalibration.Count(x => x.InstallerId == p.UserId)
                    where p.Type == ConstantsVar.Installer
                    select new UserListView
                    {
                        UserId = p.UserId,
                        Type = p.Type,
                        Name = p.Name,
                        Email = p.Email,
                        Phone = p.Phone,
                        Npi = p.Npi,

                        Address = p.Address,
                        Password = p.Password,
                        Count = cCount
                    }).ToList();
        }

        public List<UserListView> getPatientAdminUserList()
        {

            return (from p in context.User
                    let cCount = context.Patient.Count(x => x.Paid == p.UserId)
                    where p.Type == ConstantsVar.PatientAdministrator
                    select new UserListView
                    {
                        UserId = p.UserId,
                        Type = p.Type,
                        Name = p.Name,
                        Email = p.Email,
                        Phone = p.Phone,
                        Npi = p.Npi,

                        Address = p.Address,
                        Password = p.Password,
                        Count = cCount
                    }).ToList();
        }


        public List<UserListView> getUserListByType1243(int lUserType)
        {

            return (from p in context.User
                    let cCount = (from pat in context.Patient where (p.Type == 2 ? pat.Therapistid == p.UserId : pat.ProviderId == p.UserId) select pat).Count()
                    where p.Type == lUserType
                    select new UserListView
                    {
                        UserId = p.UserId,
                        Type = p.Type,
                        Name = p.Name,
                        Email = p.Email,
                        Phone = p.Phone,
                        Npi = p.Npi,
                        Count = cCount,
                        Address = p.Address,
                        Password = p.Password
                    }).ToList();
        }

        public List<SupportView> getUserListByTypeSup(int lUserType)
        {

            return (from p in context.User
                    let cCount = (from _sup in context.AppointmentSchedule where (_sup.UserId == p.UserId) select _sup).Count()
                    let cCount1 = (from _sup in context.Availability where (_sup.UserId == p.UserId) select _sup).Count()
                    where p.Type == lUserType
                    select new SupportView
                    {
                        UserId = p.UserId,
                        Type = p.Type,
                        Name = p.Name,
                        Email = p.Email,
                        Phone = p.Phone,
                        Npi = p.Npi,
                        Count = cCount > 0 ? cCount : cCount1,
                        Address = p.Address,
                        Password = p.Password
                    }).ToList();
        }

        public List<User> getUserListByTypeValue(int lUserType)
        {
            return null;

        }

        public List<UserViewModel> getUserListByTypeValueqq(int lUserType)
        {

            return null;

        }

        public List<NewPatient> getPatientListByType(int lUserType)
        {
            return (from p in context.Patient
                    join _provider in context.User on p.ProviderId equals _provider.UserId
                    join _patadmin in context.User on p.Paid equals _patadmin.UserId into patadmin
                    from lpatadmin in patadmin.DefaultIfEmpty()
                    let rx = context.PatientRx.FirstOrDefault(x => x.PatientId == p.PatientId)

                    select new NewPatient
                    {
                        PatientLoginId = p.PatientLoginId,
                        PatientId = p.PatientId,
                        PatientName = p.PatientName,
                        PhoneNumber = p.PhoneNumber,
                        ProviderId = _provider.Name,
                        TherapistId = p.Therapistid,
                        PatientAdminId = lpatadmin != null ? lpatadmin.Name : "",
                        AddressLine = p.AddressLine,
                        Dob = p.Dob,
                        City = p.City,
                        State = p.State,
                        SurgeryDate = p.SurgeryDate,
                        Ssn = p.Ssn,
                        Side = p.Side,
                        EquipmentType = p.EquipmentType,
                        Actuator = rx != null ? rx.DeviceConfiguration : ""
                    }).ToList();


        }
        public List<NewPatient> getUserListByTherapistId(string lTherapist)
        {
            return (from p in context.Patient
                    join _provider in context.User on p.ProviderId equals _provider.UserId
                    join _patadmin in context.User on p.Paid equals _patadmin.UserId into patadmin
                    from lpatadmin in patadmin.DefaultIfEmpty()

                    let rx = context.PatientRx.FirstOrDefault(x => x.PatientId == p.PatientId)
                    where p.Therapistid == lTherapist
                    select new NewPatient
                    {
                        PatientLoginId = p.PatientLoginId,
                        PatientId = p.PatientId,
                        PatientName = p.PatientName,
                        PhoneNumber = p.PhoneNumber,
                        ProviderId = _provider.Name,
                        TherapistId = p.Therapistid,
                        PatientAdminId = lpatadmin != null ? lpatadmin.Name : "",
                        AddressLine = p.AddressLine,
                        Dob = p.Dob,
                        City = p.City,
                        State = p.State,
                        SurgeryDate = p.SurgeryDate,
                        Ssn = p.Ssn,
                        Side = p.Side,

                        EquipmentType = p.EquipmentType,
                        Actuator = rx != null ? rx.DeviceConfiguration : ""
                    }).ToList();

        }

        public List<User> getTherapistListByProviderId(string lProvider)
        {

            return null;
        }

        public List<NewPatient> getPatientListByPatientAdmin(string lpatAdmin)
        {
            return (from p in context.Patient
                    join _provider in context.User on p.ProviderId equals _provider.UserId
                    join _patadmin in context.User on p.Paid equals _patadmin.UserId into patadmin
                    from lpatadmin in patadmin.DefaultIfEmpty()
                    let rx = context.PatientRx.FirstOrDefault(x => x.PatientId == p.PatientId)
                    where p.Paid == lpatAdmin
                    select new NewPatient
                    {
                        PatientLoginId = p.PatientLoginId,
                        PatientId = p.PatientId,
                        PatientName = p.PatientName,
                        PhoneNumber = p.PhoneNumber,
                        ProviderId = _provider.Type != 3 ? _provider.Name : "",
                        PatientAdminId = lpatadmin != null ? lpatadmin.Name : "",
                        AddressLine = p.AddressLine,
                        Dob = p.Dob,
                        City = p.City,
                        State = p.State,
                        SurgeryDate = p.SurgeryDate,
                        Ssn = p.Ssn,
                        Side = p.Side,
                        EquipmentType = p.EquipmentType,
                        Actuator = rx != null ? rx.DeviceConfiguration : ""
                    }).ToList();
        }

        public List<NewPatient> getPatientListByProviderId(string lProvider)
        {
            return (from p in context.Patient
                    join _provider in context.User on p.ProviderId equals _provider.UserId
                    join _patadmin in context.User on p.Paid equals _patadmin.UserId into patadmin
                    from lpatadmin in patadmin.DefaultIfEmpty()
                    let rx = context.PatientRx.FirstOrDefault(x => x.PatientId == p.PatientId)
                    where p.ProviderId == lProvider
                    select new NewPatient
                    {
                        PatientLoginId = p.PatientLoginId,
                        PatientId = p.PatientId,
                        PatientName = p.PatientName,
                        PhoneNumber = p.PhoneNumber,
                        ProviderId = _provider.Type != 3 ? _provider.Name : "",
                        PatientAdminId = lpatadmin != null ? lpatadmin.Name : "",
                        AddressLine = p.AddressLine,
                        Dob = p.Dob,
                        City = p.City,
                        State = p.State,
                        SurgeryDate = p.SurgeryDate,
                        Ssn = p.Ssn,
                        Side = p.Side,
                        EquipmentType = p.EquipmentType,
                        Actuator = rx != null ? rx.DeviceConfiguration : ""
                    }).ToList();
        }

        public string InsertUser(User pUser)
        {
            string result = string.Empty;
            User _user = null;


            if (pUser != null)
            {
                pUser.UserId = pUser.UserId.ToLower();
                _user = (from p in context.User
                         where p.UserId == pUser.UserId
                         select p).FirstOrDefault();
                if (_user == null)
                {
                    _user = (from p in context.User
                             where p.UserId == pUser.UserId.ToUpper()
                             select p).FirstOrDefault();
                }
                if (_user == null)
                {
                    if (!string.IsNullOrEmpty(pUser.Password) && (pUser.Type == ConstantsVar.Patient || pUser.Type == ConstantsVar.Therapist || pUser.Type == ConstantsVar.Support))
                    {
                        AddUser luser = new AddUser();
                        luser.secretkey = ConfigVars.NewInstance.secretkey;
                        luser.username = pUser.UserId;
                        luser.fn = pUser.Name;
                        luser.ln = pUser.Name;
                        luser.password = pUser.Password;

                        VSeeHelper lhelper = new VSeeHelper();
                        var resUser = lhelper.AddUser(luser);
                        if (resUser != null && resUser["status"] == "success")
                        {
                            pUser.Vseeid = "onedirect+" + pUser.UserId.ToLower();
                            context.User.Add(pUser);
                            context.SaveChanges();
                            result = "success";
                        }
                    }
                    else
                    {
                        context.User.Add(pUser);
                        context.SaveChanges();
                        result = "success";
                    }
                    //Prabhu

                }
                else
                {
                    result = "Username already exists";
                }
            }

            return result;
        }
        public string getUserdatabyPatientAndtherapist(string lpatientId, string lTherapistId)
        {
            string result = string.Empty;
            JsonUserData _user = new JsonUserData();
            _user.Users = new List<User>();
            try
            {
                var _patient = (from p in context.User
                                where p.UserId == lpatientId
                                select p).FirstOrDefault();
                var _therapist = (from p in context.User
                                  where p.UserId == lTherapistId
                                  select p).FirstOrDefault();

                if (_patient != null)
                    _user.Users.Add(_patient);
                if (_therapist != null)
                    _user.Users.Add(_therapist);

                _user.result = "success";
            }
            catch (Exception ex)
            {
                _user.result = "failed";
            }
            return JsonConvert.SerializeObject(_user);
        }

        public string UpdateSessionId(User pUser)
        {
            string result = string.Empty;
            context.Entry(pUser).State = EntityState.Modified;
            context.SaveChanges();
            result = "success";

            return result;
        }

        public string UpdateUser(User pUser)
        {
            string result = string.Empty;
            var _user = (from p in context.User
                         where p.UserId == pUser.UserId
                         select p).FirstOrDefault();
            if (_user != null)
            {
                dynamic resUser = null;
                VSeeHelper lhelper = new VSeeHelper();

                AddUser luser = new AddUser();
                luser.secretkey = ConfigVars.NewInstance.secretkey;
                luser.username = pUser.UserId;
                luser.fn = pUser.Name;
                luser.ln = pUser.Name;
                luser.password = pUser.Password;


                if (!string.IsNullOrEmpty(pUser.Password) && !string.IsNullOrEmpty(pUser.Vseeid) && (pUser.Type == ConstantsVar.Patient || pUser.Type == ConstantsVar.Therapist || pUser.Type == ConstantsVar.Support))
                {
                    resUser = lhelper.UpdateUser(luser);
                }
                else if (!string.IsNullOrEmpty(pUser.Password) && string.IsNullOrEmpty(pUser.Vseeid) && (pUser.Type == ConstantsVar.Patient || pUser.Type == ConstantsVar.Therapist || pUser.Type == ConstantsVar.Support))
                {
                    resUser = lhelper.AddUser(luser);
                }

                if (resUser != null && resUser["status"] == "success")
                {
                    _user.Vseeid = "onedirect+" + pUser.UserId.ToLower();
                }

                _user.Name = pUser.Name;
                _user.Email = pUser.Email;
                _user.Address = pUser.Address;
                _user.Phone = pUser.Phone;
                _user.Password = pUser.Password;
                _user.Type = pUser.Type;
                _user.Npi = pUser.Npi;
                _user.EncryptPasswrod = pUser.EncryptPasswrod;

                context.Entry(_user).State = EntityState.Modified;
                context.SaveChanges();
                result = "success";
            }

            return result;
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

            var resulta = (from p in context.Patient
                          .Include(pro => pro.Protocol)
                          .ThenInclude(s => s.Session)
                           where p.Paid == id
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
        private int getPercentage(DateTime? startDate, DateTime? EndDate)
        {
            var start = Convert.ToDateTime(startDate);
            var end = Convert.ToDateTime(EndDate);
            var total = (end - start).TotalDays;


            var percent = Convert.ToInt32(Math.Round((DateTime.Now - start).TotalDays * 100 / total));

            return percent;
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



        public User LoginUser(string username, string password)
        {
            return (from p in context.User
                    where p.UserId == username && p.Password == password
                    select p).FirstOrDefault();
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
