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

                context.Entry(_user).State = EntityState.Modified;
                context.SaveChanges();
                result = "success";





            }

            return result;
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
