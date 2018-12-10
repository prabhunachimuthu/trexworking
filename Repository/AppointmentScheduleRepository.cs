using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OneDirect.Helper;
using OneDirect.Models;
using OneDirect.Repository.Interface;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository
{
    public class AppointmentScheduleRepository : IAppointmentScheduleInterface
    {
        private OneDirectContext context;
        
        public AppointmentScheduleRepository(OneDirectContext context)
        {
            
            this.context = context;
        }

        public List<AppointmentScheduleListView> getAppointmentList(string timezone)
        {
            try
            {
                List<AppointmentScheduleListView> list = (from p in context.AppointmentSchedule.Include(x => x.User)
                                                          join _patient in context.Patient.Include(pro => pro.Protocol).ThenInclude(s => s.Session).Include(rx => rx.PatientRx) on p.PatientId equals _patient.PatientId
                                                          join _provider in context.User on _patient.ProviderId equals _provider.UserId
                                                          where p.Datetime < Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(DateTime.Now, timezone))
                                                          select new AppointmentScheduleListView
                                                          {
                                                              AppointmentId = p.AppointmentId,
                                                              AppointmentDate = Utilities.ConverTimetoBrowserTimeZone(p.Datetime, timezone),
                                                              PatientId = p.PatientId.HasValue ? p.PatientId.Value.ToString() : "",
                                                              UserType = p.UserType,
                                                              UserId = p.UserId,
                                                              UserName = p.User.Name,
                                                              Urikey = p.VseeUrl,
                                                              Status = p.CallStatus,
                                                              CreateDate = p.CreateDate,
                                                              UpdatedDate = p.UpdateDate,
                                                              PatientName = _patient.PatientName,
                                                              PatientVseeId = (from m in context.User where m.UserId == _patient.PatientLoginId select m).FirstOrDefault().Vseeid,
                                                              Provider = _provider,
                                                              TotalSession = _patient.Session.Count,
                                                              PatientRx = _patient.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault()
                                                          }).OrderByDescending(x => x.AppointmentDate).ToList();

                return list;

            }
            catch (Exception ex)
            {
               
                return null;
            }
        }


        public List<AppointmentScheduleListView> getAppointmentListByUserId(string luserId, string timezone)
        {
            try
            {
                List<AppointmentScheduleListView> list = (from p in context.AppointmentSchedule.Include(x => x.User)
                                                          join _patient in context.Patient.Include(pro => pro.Protocol).ThenInclude(s => s.Session).Include(rx => rx.PatientRx) on p.PatientId equals _patient.PatientId
                                                          join _provider in context.User on _patient.ProviderId equals _provider.UserId
                                                        
                                                          where p.UserId == luserId && p.Datetime < Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(DateTime.Now, timezone))
                                                          select new AppointmentScheduleListView
                                                          {
                                                              AppointmentId = p.AppointmentId,
                                                              AppointmentDate = Utilities.ConverTimetoBrowserTimeZone(p.Datetime, timezone),
                                                              PatientId = p.PatientId.HasValue ? p.PatientId.Value.ToString() : "",
                                                              UserType = p.UserType,
                                                              UserId = p.UserId,
                                                              UserName = p.User.Name,
                                                              Urikey = p.VseeUrl,
                                                              Status = p.CallStatus,
                                                              CreateDate = p.CreateDate,
                                                              UpdatedDate = p.UpdateDate,
                                                              PatientName = _patient.PatientName,
                                                              PatientVseeId = (from m in context.User where m.UserId == _patient.PatientLoginId select m).FirstOrDefault().Vseeid,
                                                              Provider = _provider,
                                                             
                                                              TotalSession = _patient.Session.Count,
                                                              PatientRx = _patient.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault()
                                                          }).OrderByDescending(x => x.AppointmentDate).ToList();

                return list;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<AppointmentScheduleListView> getAppointmentListByPatientId(int lpatientId, string timezone)
        {
            try
            {
                List<AppointmentScheduleListView> list = (from p in context.AppointmentSchedule.Include(x => x.User)
                                                          join _patient in context.Patient.Include(pro => pro.Protocol).ThenInclude(s => s.Session).Include(rx => rx.PatientRx) on p.PatientId equals _patient.PatientId
                                                          join _provider in context.User on _patient.ProviderId equals _provider.UserId
                                                         
                                                          where p.PatientId == lpatientId && p.Datetime < Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(DateTime.Now, timezone))
                                                          select new AppointmentScheduleListView
                                                          {
                                                              AppointmentId = p.AppointmentId,
                                                              AppointmentDate = Utilities.ConverTimetoBrowserTimeZone(p.Datetime, timezone),
                                                              PatientId = p.PatientId.HasValue ? p.PatientId.Value.ToString() : "",
                                                              UserType = p.UserType,
                                                              UserId = p.UserId,
                                                              UserName = p.User.Name,
                                                              Urikey = p.VseeUrl,
                                                              Status = p.CallStatus,
                                                              CreateDate = p.CreateDate,
                                                              UpdatedDate = p.UpdateDate,
                                                              PatientName = _patient.PatientName,
                                                              PatientVseeId = (from m in context.User where m.UserId == _patient.PatientLoginId select m).FirstOrDefault().Vseeid,
                                                              Provider = _provider,
                                                            
                                                              TotalSession = _patient.Session.Count,
                                                              PatientRx = _patient.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault()
                                                          }).OrderByDescending(x => x.AppointmentDate).ToList();

                return list;

            }
            catch (Exception ex)
            {
                return null;
            }

        }



        public List<Patient> getAppointmentPatientList()
        {
            return (from p in context.AppointmentSchedule
                    join _patient in context.Patient on p.PatientId equals _patient.PatientId
                    select _patient).Distinct().OrderBy(x => x.PatientName).ToList();
        }
        public List<Patient> getAppointmentPatientListByUserId(string userId)
        {
            return (from p in context.AppointmentSchedule
                    join _patient in context.Patient on p.PatientId equals _patient.PatientId
                    where p.UserId == userId
                    select _patient).Distinct().OrderBy(x => x.PatientName).ToList();
        }




        public int InsertAppointment(AppointmentSchedule pAppointment)
        {
            context.AppointmentSchedule.Add(pAppointment);
            return context.SaveChanges();
        }

        public AppointmentSchedule GetAppointment(string appontmentId)
        {
            try
            {
                var appointment = (from p in context.AppointmentSchedule
                                   where p.AppointmentId == appontmentId
                                   select p).FirstOrDefault();
                return appointment;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public int UpdateAppointment(AppointmentSchedule appontment)
        {
            try
            {
                context.Entry(appontment).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return context.SaveChanges();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public List<AppointmentScheduleView> GetAppointments(int patientId)
        {
            try
            {
                var appointments = (from p in context.AppointmentSchedule
                                    join u in context.User on p.UserId equals u.UserId
                                    where p.PatientId == patientId && p.CallStatus == "Open"
                                    select new AppointmentScheduleView
                                    {
                                        AppointmentId = p.AppointmentId,
                                        UserType = p.UserType,
                                        UserID = p.UserId,
                                        UserName = u.Name,
                                        DateTime = p.Datetime.ToString("MM/dd/yyyy HH:mm:ss"),
                                        PatientId = p.PatientId,
                                        SlotStatus = p.SlotStatus,
                                        CallStatus = p.CallStatus,
                                        RecordedFile = p.RecordedFile,
                                        CreateDate = p.CreateDate,
                                        UpdateDate = p.UpdateDate
                                    }).ToList();
                return appointments;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public string CancelAppointments(int patientId, string userId, string datetime)
        {
            try
            {
                var appointments = (from p in context.AppointmentSchedule
                                    where p.PatientId == patientId && p.UserId == userId
                                    && p.Datetime >= Convert.ToDateTime(datetime)
                                    && p.Datetime.AddHours(1) > DateTime.Now
                                    select p).ToList();
                context.AppointmentSchedule.RemoveRange(appointments);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
        }
        public string NoShowAppointments(int patientId, string userId, string datetime)
        {
            try
            {
                var appointments = (from p in context.AppointmentSchedule
                                    where p.PatientId == patientId && p.UserId == userId
                                    && p.Datetime == Convert.ToDateTime(datetime) && p.Datetime.AddMinutes(30) < DateTime.Now
                                    select p).ToList();
                if (appointments != null && appointments.Count > 0)
                {
                    foreach (AppointmentSchedule app in appointments)
                    {
                        app.CallStatus = "Caller No Show";
                        context.Entry(app).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        int res = context.SaveChanges();
                    }
                  
                }



            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
        }

        public string GetAvailability(string userId, string day, string hour, string timezoneoffset)
        {
            try
            {

                Availability lavailability = (from p in context.Availability
                                              where p.UserId == userId && p.DayOfWeek == day
                                              select p).FirstOrDefault();
                if (lavailability != null)
                {
                    DateTime date = DateTime.Now.Date;
                    int dayofWeek = Convert.ToInt32(getDayofWeek(date.DayOfWeek.ToString()));
                    var addday = Convert.ToInt32(day) - dayofWeek;
                    date = date.AddDays(addday);
                    date = date.AddHours(Convert.ToInt32(hour));
                    string ldate = Utilities.ConverTimetoServerTimeZone(date, timezoneoffset);

                    List<string> hourofDay = lavailability.HourOfDay.Split(',').ToList();
                    string lhour = hourofDay.Where(x => x == Convert.ToDateTime(ldate).Hour.ToString()).FirstOrDefault();
                    if (!string.IsNullOrEmpty(lhour))
                    {
                        return lhour;
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            return "";
        }

        public string GetAvailability(string userId, string day, string hour, string minute, string timezoneoffset)
        {
            try
            {

                Availability lavailability = (from p in context.Availability
                                              where p.UserId == userId && p.DayOfWeek == day
                                              select p).FirstOrDefault();
                if (lavailability != null)
                {
                    DateTime date = DateTime.Now.Date;
                    int dayofWeek = Convert.ToInt32(getDayofWeek(date.DayOfWeek.ToString()));
                    var addday = Convert.ToInt32(day) - dayofWeek;
                    date = date.AddDays(addday);
                    date = date.AddHours(Convert.ToInt32(hour)).AddMinutes(Convert.ToInt32(minute));
                    string ldate = Utilities.ConverTimetoServerTimeZone(date, timezoneoffset);

                    List<string> hourminuteofDay = lavailability.HourOfDay.Split(',').ToList();
                    int slotid = (Convert.ToDateTime(ldate).Hour * 2) + (Convert.ToDateTime(ldate).Minute > 0 ? 1 : 0);
                    string lhourstlot = hourminuteofDay.Where(x => x == slotid.ToString()).FirstOrDefault();
                    if (!string.IsNullOrEmpty(lhourstlot))
                    {
                        return lhourstlot;
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            return "";
        }
        public string RemoveAvailability(string userId, string userType, string timezoneoffset, string day, string hour)
        {
            try
            {
                DateTime date = DateTime.Now.Date;
                int dayofWeek = Convert.ToInt32(getDayofWeek(date.DayOfWeek.ToString()));
                var addday = Convert.ToInt32(day) - dayofWeek;
                date = date.AddDays(addday);
                date = date.AddHours(Convert.ToInt32(hour));
                string ldate = Utilities.ConverTimetoServerTimeZone(date, timezoneoffset);


                Availability lavailability = (from p in context.Availability
                                              where p.UserId == userId && p.DayOfWeek == day
                                              select p).FirstOrDefault();
                if (lavailability != null)
                {
                    List<string> hourofDay = lavailability.HourOfDay.Split(',').ToList();
                    hourofDay.Remove(Convert.ToDateTime(ldate).Hour.ToString());
                    if (hourofDay.Count > 0)
                    {
                        string lhours = String.Join(",", hourofDay);

                        lavailability.HourOfDay = lhours;
                        context.Entry(lavailability).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        context.Availability.Remove(lavailability);
                        context.SaveChanges();
                    }
                }

                return "success";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string RemoveAvailability(string userId, string userType, string timezoneoffset, string day, string hour, string minute)
        {
            try
            {
                DateTime date = DateTime.Now.Date;
                int dayofWeek = Convert.ToInt32(getDayofWeek(date.DayOfWeek.ToString()));
                var addday = Convert.ToInt32(day) - dayofWeek;
                date = date.AddDays(addday);
                date = date.AddHours(Convert.ToInt32(hour)).AddMinutes(Convert.ToInt32(minute));
                string ldate = Utilities.ConverTimetoServerTimeZone(date, timezoneoffset);


                Availability lavailability = (from p in context.Availability
                                              where p.UserId == userId && p.DayOfWeek == day
                                              select p).FirstOrDefault();
                if (lavailability != null)
                {
                    List<string> hourofDay = lavailability.HourOfDay.Split(',').ToList();
                    int hourslot = (Convert.ToDateTime(ldate).Hour * 2) + (Convert.ToDateTime(ldate).Minute > 0 ? 1 : 0);
                    hourofDay.Remove(hourslot.ToString());
                    if (hourofDay.Count > 0)
                    {
                        string lhours = String.Join(",", hourofDay);

                        lavailability.HourOfDay = lhours;
                        context.Entry(lavailability).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        context.Availability.Remove(lavailability);
                        context.SaveChanges();
                    }
                }

                return "success";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public int DeleteAppointmentSchedule(string appointmentId)
        {
            try
            {
                AppointmentSchedule lappointment = (from p in context.AppointmentSchedule
                                                    where p.AppointmentId == appointmentId
                                                    select p).FirstOrDefault();
                if (lappointment != null)
                {
                    context.AppointmentSchedule.Remove(lappointment);
                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public AppointmentSchedule CheckAppointmentSchedule(string userId, string userType, DateTime datetime)
        {
            try
            {
                AppointmentSchedule lappointmentlist = (from p in context.AppointmentSchedule
                                                        where p.UserId == userId && p.UserType == userType && p.Datetime == datetime
                                                        select p).FirstOrDefault();

                return lappointmentlist;
            }
            catch (Exception ex)
            {

            }
            return null;
        }



        public List<AppointmentSchedule> CheckAppointmentSchedule(string userId, string userType, string timezoneoffset, string day, string hour)
        {
            try
            {
                List<AppointmentSchedule> result = new List<AppointmentSchedule>();
                List<AppointmentSchedule> lappointmentlist = (from p in context.AppointmentSchedule.Include(x => x.Patient)
                                                              where p.UserId == userId && (p.CallStatus == "Open" || p.CallStatus == "Extra")
                                                              select p).ToList();
                if (lappointmentlist != null && lappointmentlist.Count > 0)
                {
                    lappointmentlist = lappointmentlist.Where(x => x.Datetime.DayOfWeek == (DayOfWeek)Convert.ToInt32(day)).ToList();
                    if (lappointmentlist.Count > 0)
                    {
                        foreach (AppointmentSchedule lappointment in lappointmentlist)
                        {
                            string datetime = Utilities.ConverTimetoServerTimeZone(lappointment.Datetime.Date.AddHours(Convert.ToInt32(hour)), timezoneoffset);
                            if (!string.IsNullOrEmpty(datetime) && Convert.ToDateTime(datetime) == lappointment.Datetime)
                            {
                                lappointment.Datetime = Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(lappointment.Datetime, timezoneoffset));
                                result.Add(lappointment);
                            }
                        }

                    }
                }

                return result;
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        public List<AppointmentSchedule> CheckAppointmentSchedule(string userId, string userType, string timezoneoffset, string day, string hour, string minute)
        {
            try
            {
                List<AppointmentSchedule> result = new List<AppointmentSchedule>();
                List<AppointmentSchedule> lappointmentlist = (from p in context.AppointmentSchedule.Include(x => x.Patient)
                                                              where p.UserId == userId && p.CallStatus == "Open"
                                                              select p).ToList();
                if (lappointmentlist != null && lappointmentlist.Count > 0)
                {
                    lappointmentlist = lappointmentlist.Where(x => x.Datetime.DayOfWeek == (DayOfWeek)Convert.ToInt32(day)).ToList();
                    if (lappointmentlist.Count > 0)
                    {
                        foreach (AppointmentSchedule lappointment in lappointmentlist)
                        {
                            string datetime = Utilities.ConverTimetoServerTimeZone(lappointment.Datetime.Date.AddHours(Convert.ToInt32(hour)).AddMinutes(Convert.ToInt32(minute)), timezoneoffset);
                            if (!string.IsNullOrEmpty(datetime) && Convert.ToDateTime(datetime) == lappointment.Datetime)
                            {
                                lappointment.Datetime = Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(lappointment.Datetime, timezoneoffset));
                                result.Add(lappointment);
                            }
                        }

                    }
                }

                return result;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public string UpdateAppointmentSchedule(string userId, string userType, string timezoneoffset, string day, string hour)
        {
            try
            {
                List<AppointmentSchedule> result = new List<AppointmentSchedule>();
                List<AppointmentSchedule> lappointmentlist = (from p in context.AppointmentSchedule.Include(x => x.Patient)
                                                              where p.UserId == userId && p.CallStatus == "Open"
                                                              select p).ToList();
                if (lappointmentlist != null && lappointmentlist.Count > 0)
                {
                    lappointmentlist = lappointmentlist.Where(x => x.Datetime.DayOfWeek == (DayOfWeek)Convert.ToInt32(day)).ToList();
                    if (lappointmentlist.Count > 0)
                    {
                        foreach (AppointmentSchedule lappointment in lappointmentlist)
                        {
                            string datetime = Utilities.ConverTimetoServerTimeZone(lappointment.Datetime.Date.AddHours(Convert.ToInt32(hour)), timezoneoffset);
                            if (!string.IsNullOrEmpty(datetime) && Convert.ToDateTime(datetime) == lappointment.Datetime)
                            {
                                result.Add(lappointment);
                            }
                        }

                    }
                }
                if (result != null && result.Count > 0)
                {
                    int count = 0;
                    foreach (AppointmentSchedule app in result)
                    {
                        app.CallStatus = "Caller Cancelled";
                        context.Entry(app).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        int res = context.SaveChanges();
                        if (res > 0)
                        {
                            count = count + 1;
                        }
                    }
                    if (count > 0)
                        return "success";
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        public string UpdateAppointmentSchedule(string userId, string userType, string timezoneoffset, string day, string hour, string minute)
        {
            try
            {
                List<AppointmentSchedule> result = new List<AppointmentSchedule>();
                List<AppointmentSchedule> lappointmentlist = (from p in context.AppointmentSchedule.Include(x => x.Patient)
                                                              where p.UserId == userId && p.CallStatus == "Open"
                                                              select p).ToList();
                if (lappointmentlist != null && lappointmentlist.Count > 0)
                {
                    lappointmentlist = lappointmentlist.Where(x => x.Datetime.DayOfWeek == (DayOfWeek)Convert.ToInt32(day)).ToList();
                    if (lappointmentlist.Count > 0)
                    {
                        foreach (AppointmentSchedule lappointment in lappointmentlist)
                        {
                            string datetime = Utilities.ConverTimetoServerTimeZone(lappointment.Datetime.Date.AddHours(Convert.ToInt32(hour)).AddMinutes(Convert.ToInt32(minute)), timezoneoffset);
                            if (!string.IsNullOrEmpty(datetime) && Convert.ToDateTime(datetime) == lappointment.Datetime)
                            {
                                result.Add(lappointment);
                            }
                        }

                    }
                }
                if (result != null && result.Count > 0)
                {
                    int count = 0;
                    foreach (AppointmentSchedule app in result)
                    {
                        app.CallStatus = "Caller Cancelled";
                        context.Entry(app).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        int res = context.SaveChanges();
                        if (res > 0)
                        {
                            count = count + 1;
                        }
                    }
                    if (count > 0)
                        return "success";
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        public string InsertAvailability(string userId, string userType, string timezoneoffset, string day, string hour)
        {
            try
            {

                DateTime date = DateTime.Now.Date;
                int dayofWeek = Convert.ToInt32(getDayofWeek(date.DayOfWeek.ToString()));
                var addday = Convert.ToInt32(day) - dayofWeek;
                date = date.AddDays(addday);
                date = date.AddHours(Convert.ToInt32(hour));
                string ldate = Utilities.ConverTimetoServerTimeZone(date, timezoneoffset);

                string lhour = Convert.ToDateTime(ldate).Hour.ToString();

                Availability lavailability = (from p in context.Availability
                                              where p.UserId == userId && p.DayOfWeek == day
                                              select p).FirstOrDefault();
                if (lavailability != null)
                {
                    List<string> hourofDay = lavailability.HourOfDay.Split(',').ToList();
                    hourofDay.Add(lhour);
                    string lhours = String.Join(",", hourofDay);

                    lavailability.HourOfDay = lhours;
                    context.Entry(lavailability).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {
                    lavailability = new Availability();
                    lavailability.AvailabilityId = Guid.NewGuid().ToString();
                    lavailability.UserType = userType;
                    lavailability.UserId = userId;
                    lavailability.DayOfWeek = day;
                    lavailability.HourOfDay = lhour;
                    lavailability.CreatedDate = DateTime.UtcNow;
                    lavailability.IsDeleted = false;
                   
                    lavailability.TimeZoneOffset = TimeZoneInfo.Local.Id;
                    
                    context.Availability.Add(lavailability);
                    context.SaveChanges();
                }
                return "success";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public string InsertAvailability(string userId, string userType, string timezoneoffset, string day, string hour, string minute)
        {
            try
            {

                DateTime date = DateTime.Now.Date;
                int dayofWeek = Convert.ToInt32(getDayofWeek(date.DayOfWeek.ToString()));
                var addday = Convert.ToInt32(day) - dayofWeek;
                date = date.AddDays(addday);
                date = date.AddHours(Convert.ToInt32(hour)).AddMinutes(Convert.ToInt32(minute));
                string ldate = Utilities.ConverTimetoServerTimeZone(date, timezoneoffset);

                int lhourslot = (Convert.ToDateTime(ldate).Hour * 2) + (Convert.ToInt32(minute) > 0 ? 1 : 0);

                Availability lavailability = (from p in context.Availability
                                              where p.UserId == userId && p.DayOfWeek == day
                                              select p).FirstOrDefault();
                if (lavailability != null)
                {
                    List<string> hourofDay = lavailability.HourOfDay.Split(',').ToList();
                    hourofDay.Add(lhourslot.ToString());
                    string lhours = String.Join(",", hourofDay);

                    lavailability.HourOfDay = lhours;
                    context.Entry(lavailability).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {
                    lavailability = new Availability();
                    lavailability.AvailabilityId = Guid.NewGuid().ToString();
                    lavailability.UserType = userType;
                    lavailability.UserId = userId;
                    lavailability.DayOfWeek = day;
                    lavailability.HourOfDay = lhourslot.ToString();
                    lavailability.CreatedDate = DateTime.UtcNow;
                    lavailability.IsDeleted = false;
                   
                    lavailability.TimeZoneOffset = TimeZoneInfo.Local.Id;
                   
                    context.Availability.Add(lavailability);
                    context.SaveChanges();
                }
                return "success";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public List<availabilityView> GetAvailability(string userId, string browsertimezone)
        {
            List<availabilityView> result = null;
            try
            {

                List<Availability> lavailabilityList = (from p in context.Availability
                                                        where p.UserId == userId
                                                        select p).OrderBy(x => x.DayOfWeek).ToList();
                if (lavailabilityList != null && lavailabilityList.Count > 0)
                {
                    result = new List<availabilityView>();
                    foreach (Availability avail in lavailabilityList)
                    {
                        DateTime date = DateTime.Now.Date;
                        int dayofWeek = Convert.ToInt32(getDayofWeek(date.DayOfWeek.ToString()));
                        var addday = Convert.ToInt32(avail.DayOfWeek) - dayofWeek;
                        date = date.AddDays(addday);
                        List<string> hourofDay = avail.HourOfDay.Split(',').ToList();
                      
                        foreach (string hour in hourofDay)
                        {
                            string datetime = Utilities.ConverTimetoBrowserTimeZone(date.AddHours(Convert.ToInt32(hour)), browsertimezone);
                            if (!string.IsNullOrEmpty(datetime))
                            {
                                availabilityView availability = new availabilityView();
                                
                                availability.title = "";
                                string[] dow = new string[1];
                                dow[0] = avail.DayOfWeek.ToString();
                                availability.dow = dow;
                                
                                availability.start = Convert.ToDateTime(datetime).ToString("HH:mm");
                                
                                availability.end = (Convert.ToDateTime(datetime).AddHours(1)).ToString("HH:mm");
                                availability.backgroundColor = "green";
                                availability.borderColor = "green";
                                result.Add(availability);
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        public List<availabilityView> GetAvailabilityInMinutes(string userId, string browsertimezone)
        {
            List<availabilityView> result = null;
            try
            {

                List<Availability> lavailabilityList = (from p in context.Availability
                                                        where p.UserId == userId
                                                        select p).OrderBy(x => x.DayOfWeek).ToList();
                if (lavailabilityList != null && lavailabilityList.Count > 0)
                {
                    result = new List<availabilityView>();
                    foreach (Availability avail in lavailabilityList)
                    {
                        DateTime date = DateTime.Now.Date;
                        int dayofWeek = Convert.ToInt32(getDayofWeek(date.DayOfWeek.ToString()));
                        var addday = Convert.ToInt32(avail.DayOfWeek) - dayofWeek;
                        date = date.AddDays(addday);
                        List<string> hourofDay = avail.HourOfDay.Split(',').ToList();
                        
                        foreach (string hour in hourofDay)
                        {
                            int totalminute = convertSlotToMinutes(Convert.ToInt32(hour));
                            string datetime = Utilities.ConverTimetoBrowserTimeZone(date.AddMinutes(totalminute), browsertimezone);
                            if (!string.IsNullOrEmpty(datetime))
                            {
                                availabilityView availability = new availabilityView();
                               
                                availability.title = "";
                                string[] dow = new string[1];
                                dow[0] = avail.DayOfWeek.ToString();
                                availability.dow = dow;
                                
                                availability.start = Convert.ToDateTime(datetime).ToString("HH:mm");
                                
                                availability.end = (Convert.ToDateTime(datetime).AddMinutes(30)).ToString("HH:mm");
                                availability.backgroundColor = "green";
                                availability.borderColor = "green";
                                result.Add(availability);
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }
        private int convertSlotToMinutes(int slot)
        {
            int hour = slot / 2;
            int minutes = (slot - (hour * 2));
            if (minutes > 0)
            {
                int a = 0;
            }
            int res = (hour * 60) + (minutes > 0 ? 30 : 0);
            return res;
        }
        private string getDay(string day)
        {
            string result = "";
            switch (day)
            {
                case "1":
                    result = "Monday";
                    break;
                case "2":
                    result = "Tuesday";
                    break;
                case "3":
                    result = "Wednesday";
                    break;
                case "4":
                    result = "Thursday";
                    break;
                case "5":
                    result = "Friday";
                    break;
                case "6":
                    result = "Saturday";
                    break;
                case "0":
                    result = "Sunday";
                    break;
            }
            return result;
        }
        private string getDayofWeek(string day)
        {
            string result = "";
            switch (day)
            {
                case "Monday":
                    result = "1";
                    break;
                case "Tuesday":
                    result = "2";
                    break;
                case "Wednesday":
                    result = "3";
                    break;
                case "Thursday":
                    result = "4";
                    break;
                case "Friday":
                    result = "5";
                    break;
                case "Saturday":
                    result = "6";
                    break;
                case "Sunday":
                    result = "0";
                    break;
            }
            return result;
        }
        public List<AvailableSlot> GetAvailableSlots(string userId, DateTime rxStartDate, DateTime rxEndDate, int code)
        {
            List<AvailableSlot> result = null;
            try
            {
                if (code == 1)
                {
                    List<Availability> lavailabilityList = (from p in context.Availability
                                                            where p.UserId == userId && p.UserType == "Therapist"
                                                            select p).OrderBy(x => x.DayOfWeek).ToList();
                    if (lavailabilityList != null && lavailabilityList.Count > 0)
                    {
                        result = new List<AvailableSlot>();
                       
                        List<DateTime> alldates = Enumerable.Range(0, 1 + rxEndDate.Subtract(rxStartDate).Days)
                                        .Select(offset => rxStartDate.AddDays(offset))
                                        .ToList();
                        foreach (Availability avail in lavailabilityList)
                        {
                            User luser = (from p in context.User where p.UserId == avail.UserId select p).FirstOrDefault();
                            List<DateTime> dayofWeeks = alldates.Where(d => d.DayOfWeek == (DayOfWeek)Convert.ToInt32(avail.DayOfWeek)).ToList();
                            List<int> hourofDay = avail.HourOfDay.Split(',').Select(s => int.Parse(s)).ToList();
                            if (ConfigVars.NewInstance.slots.SlotDuration == "30")
                            {
                                hourofDay = hourofDay.Where(x => x >= (Convert.ToInt32(ConfigVars.NewInstance.slots.Therapist_StartSlot) * 2) && x < (Convert.ToInt32(ConfigVars.NewInstance.slots.Therapist_EndSlot) * 2)).ToList();
                            }
                            else
                            {
                                hourofDay = hourofDay.Where(x => x >= Convert.ToInt32(ConfigVars.NewInstance.slots.Therapist_StartSlot) && x < Convert.ToInt32(ConfigVars.NewInstance.slots.Therapist_EndSlot)).ToList();
                            }
                           
                            foreach (DateTime day in dayofWeeks)
                            {
                                foreach (int hour in hourofDay)
                                {
                                    string timezoneid = TimeZoneInfo.Local.Id;
                                    string datetime = "";
                                    if (ConfigVars.NewInstance.slots.SlotDuration == "30")
                                    {
                                        //Min Slot
                                        int min = convertSlotToMinutes(Convert.ToInt32(hour));
                                        datetime = Utilities.ConverTimetoServerTimeZone(day.AddMinutes(min), timezoneid);
                                    }
                                    else
                                    {
                                        ////Hour Slot
                                        datetime = Utilities.ConverTimetoServerTimeZone(day.AddHours(Convert.ToInt32(hour)), timezoneid);

                                    }
                                    if (!string.IsNullOrEmpty(datetime))
                                    {
                                        AvailableSlot slot = new AvailableSlot();
                                        slot.UserID = luser.UserId;
                                        slot.UserName = luser.Name;
                                        slot.DateTime = datetime;
                                        result.Add(slot);
                                    }

                                }
                            }
                        }
                        if (result != null && result.Count > 0)
                        {
                            List<AppointmentSchedule> lappointmentScheduleList = (from p in context.AppointmentSchedule
                                                                                  where p.UserId == userId && p.UserType == "Therapist" && p.CallStatus != "Extra"
                                                                                  select p).OrderBy(x => x.Datetime).ToList();
                            if (lappointmentScheduleList != null && lappointmentScheduleList.Count > 0)
                            {
                                result = result.Where(item => !lappointmentScheduleList.Any(item2 => item2.Datetime == Convert.ToDateTime(item.DateTime) && item2.UserId == item.UserID)).ToList();
                            }

                            //Add Extra time for particular date
                            List<AppointmentSchedule> lappointmentScheduleListExtra = (from p in context.AppointmentSchedule.Include(c => c.User)
                                                                                       where p.UserId == userId && p.UserType == "Therapist" && p.CallStatus == "Extra"
                                                                                       select p).OrderBy(x => x.Datetime).ToList();
                            foreach (AppointmentSchedule lschedule in lappointmentScheduleListExtra)
                            {
                                AvailableSlot slot = new AvailableSlot();
                                slot.UserID = lschedule.UserId;
                                slot.UserName = lschedule.User.Name;
                                slot.DateTime = lschedule.Datetime.ToString();
                                result.Add(slot);
                            }

                        }
                    }
                }
                else if (code == 2)
                {

                    List<Availability> lavailabilityList = (from p in context.Availability
                                                            where p.UserType == "Support"
                                                            select p).OrderBy(x => x.DayOfWeek).ToList();
                    if (lavailabilityList != null && lavailabilityList.Count > 0)
                    {
                        result = new List<AvailableSlot>();
                       
                        List<DateTime> alldates = Enumerable.Range(0, 1 + rxEndDate.Subtract(rxStartDate).Days)
                                        .Select(offset => rxStartDate.AddDays(offset))
                                        .ToList();
                        foreach (Availability avail in lavailabilityList)
                        {
                            User luser = (from p in context.User where p.UserId == avail.UserId select p).FirstOrDefault();
                            List<DateTime> dayofWeeks = alldates.Where(d => d.DayOfWeek == (DayOfWeek)Convert.ToInt32(avail.DayOfWeek)).ToList();
                            
                            List<int> hourofDay = avail.HourOfDay.Split(',').Select(s => int.Parse(s)).ToList();
                            if (ConfigVars.NewInstance.slots.SlotDuration == "30")
                            {
                                hourofDay = hourofDay.Where(x => x >= (Convert.ToInt32(ConfigVars.NewInstance.slots.Support_StartSlot) * 2) && x < (Convert.ToInt32(ConfigVars.NewInstance.slots.Support_EndSlot) * 2)).ToList();
                            }
                            else
                            {
                                hourofDay = hourofDay.Where(x => x >= Convert.ToInt32(ConfigVars.NewInstance.slots.Support_StartSlot) && x < Convert.ToInt32(ConfigVars.NewInstance.slots.Support_EndSlot)).ToList();
                            }
                           
                            foreach (DateTime day in dayofWeeks)
                            {
                                foreach (int hour in hourofDay)
                                {
                                    string timezoneid = TimeZoneInfo.Local.Id;
                                    string datetime = "";
                                    if (ConfigVars.NewInstance.slots.SlotDuration == "30")
                                    {
                                        //Min Slot
                                        int min = convertSlotToMinutes(Convert.ToInt32(hour));
                                        datetime = Utilities.ConverTimetoServerTimeZone(day.AddMinutes(min), timezoneid);
                                    }
                                    else
                                    {
                                        ////Hour Slot
                                        datetime = Utilities.ConverTimetoServerTimeZone(day.AddHours(Convert.ToInt32(hour)), timezoneid);
                                    }
                                    if (!string.IsNullOrEmpty(datetime))
                                    {
                                        AvailableSlot slot = new AvailableSlot();
                                        slot.UserID = luser.UserId;
                                        slot.UserName = luser.Name;
                                        slot.DateTime = datetime;
                                        result.Add(slot);
                                    }
                                }
                            }
                        }
                        if (result != null && result.Count > 0)
                        {
                            List<AppointmentSchedule> lappointmentScheduleList = (from p in context.AppointmentSchedule
                                                                                  where p.UserType == "Support" && p.CallStatus != "Extra"
                                                                                  select p).OrderBy(x => x.Datetime).ToList();
                            if (lappointmentScheduleList != null && lappointmentScheduleList.Count > 0)
                            {
                                result = result.Where(item => !lappointmentScheduleList.Any(item2 => item2.Datetime == Convert.ToDateTime(item.DateTime) && item2.UserId == item.UserID)).ToList();
                            }

                            //Add Extra time for particular date
                            List<AppointmentSchedule> lappointmentScheduleListExtra = (from p in context.AppointmentSchedule.Include(c => c.User)
                                                                                       where p.UserType == "Support" && p.CallStatus == "Extra"
                                                                                       select p).OrderBy(x => x.Datetime).ToList();
                            foreach (AppointmentSchedule lschedule in lappointmentScheduleListExtra)
                            {
                                AvailableSlot slot = new AvailableSlot();
                                slot.UserID = lschedule.UserId;
                                slot.UserName = lschedule.User.Name;
                                slot.DateTime = lschedule.Datetime.ToString();
                                result.Add(slot);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        public List<appointmentView> GetAvailableSlotsForAppointmentCalendar(string userId, string userType, DateTime startdate, DateTime enddate, string timezoneoffset)
        {
            List<appointmentView> result = null;
            try
            {
                List<Availability> lavailabilityList = (from p in context.Availability
                                                        where p.UserId == userId && p.UserType == userType
                                                        select p).OrderBy(x => x.DayOfWeek).ToList();
                if (lavailabilityList != null && lavailabilityList.Count > 0)
                {
                    result = new List<appointmentView>();
                    
                    List<DateTime> alldates = Enumerable.Range(0, 1 + enddate.Subtract(startdate).Days)
                                    .Select(offset => startdate.AddDays(offset))
                                    .ToList();
                    foreach (Availability avail in lavailabilityList)
                    {
                        User luser = (from p in context.User where p.UserId == avail.UserId select p).FirstOrDefault();
                        List<DateTime> dayofWeeks = alldates.Where(d => d.DayOfWeek == (DayOfWeek)Convert.ToInt32(avail.DayOfWeek)).ToList();
                        List<string> hourofDay = avail.HourOfDay.Split(',').ToList();
                       
                        foreach (DateTime day in dayofWeeks)
                        {
                            foreach (string hour in hourofDay)
                            {
                                string datetime = Utilities.ConverTimetoBrowserTimeZone(day.AddHours(Convert.ToInt32(hour)), timezoneoffset);
                                if (!string.IsNullOrEmpty(datetime))
                                {
                                  
                                    appointmentView availability = new appointmentView();
                                    availability.title = "";
                                    
                                    availability.start = datetime;
                                    availability.end = Convert.ToDateTime(datetime).AddHours(1).ToString(); ;
                                    availability.backgroundColor = "green";
                                    availability.borderColor = "green";
                                    result.Add(availability);
                                }
                            }
                        }
                    }
                    if (result != null && result.Count > 0)
                    {
                        List<AppointmentSchedule> lappointmentScheduleList = (from p in context.AppointmentSchedule.Include(x => x.Patient)
                                                                              where p.UserId == userId && p.UserType == userType
                                                                              select p).OrderBy(x => x.Datetime).ToList();
                        if (lappointmentScheduleList != null && lappointmentScheduleList.Count > 0)
                        {
                            Console.Write("Prabhu Start:");
                            lappointmentScheduleList = lappointmentScheduleList.Select(x =>
                            {
                                
                                x.Datetime = Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(x.Datetime, timezoneoffset));

                                return x;

                            }).ToList();
                            result = result.Where(item => !lappointmentScheduleList.Any(item2 => item2.Datetime == Convert.ToDateTime(item.start))).ToList();
                        }


                        foreach (AppointmentSchedule lapp in lappointmentScheduleList)
                        {
                            if (lapp.SlotStatus == "Booked")
                            {
                                appointmentView availability = new appointmentView();
                                availability.title = lapp.Patient.PatientName;

                                availability.start = lapp.Datetime.ToString();
                                availability.end = Convert.ToDateTime(lapp.Datetime).AddHours(1).ToString();
                                if (DateTime.Now >= Convert.ToDateTime(availability.start) && DateTime.Now <= Convert.ToDateTime(availability.end))
                                {
                                    availability.backgroundColor = "red";
                                    availability.borderColor = "red";
                                    availability.appointmentid = lapp.AppointmentId;
                                }
                                else
                                {
                                    availability.backgroundColor = "blue";
                                    availability.borderColor = "blue";
                                }

                                result.Add(availability);
                            }
                            else if (lapp.SlotStatus == "Blocked")
                            {
                                appointmentView availability = new appointmentView();
                                
                                availability.start = lapp.Datetime.ToString();
                                availability.end = Convert.ToDateTime(lapp.Datetime).AddHours(1).ToString();
                                availability.backgroundColor = "grey";
                                availability.borderColor = "grey";
                                result.Add(availability);
                            }
                            else if (lapp.SlotStatus == "Extra")
                            {
                                appointmentView availability = new appointmentView();
                               
                                availability.start = lapp.Datetime.ToString();
                                availability.end = Convert.ToDateTime(lapp.Datetime).AddHours(1).ToString();
                                availability.backgroundColor = "green";
                                availability.borderColor = "green";
                                result.Add(availability);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        public List<appointmentView> GetAvailableSlotsForAppointmentCalendarInMinutes(string userId, string userType, DateTime startdate, DateTime enddate, string timezoneoffset)
        {
            List<appointmentView> result = null;
            try
            {
                List<Availability> lavailabilityList = (from p in context.Availability
                                                        where p.UserId == userId && p.UserType == userType
                                                        select p).OrderBy(x => x.DayOfWeek).ToList();
                if (lavailabilityList != null && lavailabilityList.Count > 0)
                {
                    result = new List<appointmentView>();
                   
                    List<DateTime> alldates = Enumerable.Range(0, 1 + enddate.Subtract(startdate).Days)
                                    .Select(offset => startdate.AddDays(offset))
                                    .ToList();
                    foreach (Availability avail in lavailabilityList)
                    {
                        User luser = (from p in context.User where p.UserId == avail.UserId select p).FirstOrDefault();
                        List<DateTime> dayofWeeks = alldates.Where(d => d.DayOfWeek == (DayOfWeek)Convert.ToInt32(avail.DayOfWeek)).ToList();
                        List<string> hourofDay = avail.HourOfDay.Split(',').ToList();
                       
                        foreach (DateTime day in dayofWeeks)
                        {
                            foreach (string hour in hourofDay)
                            {
                                int min = convertSlotToMinutes(Convert.ToInt32(hour));
                                string datetime = Utilities.ConverTimetoBrowserTimeZone(day.AddMinutes(min), timezoneoffset);
                                if (!string.IsNullOrEmpty(datetime))
                                {
                                   
                                    appointmentView availability = new appointmentView();
                                    availability.title = "";
                                  
                                    availability.start = datetime;
                                    availability.end = Convert.ToDateTime(datetime).AddMinutes(30).ToString(); ;
                                    availability.backgroundColor = "green";
                                    availability.borderColor = "green";
                                    result.Add(availability);
                                }
                            }
                        }
                    }
                    if (result != null && result.Count > 0)
                    {
                        List<AppointmentSchedule> lappointmentScheduleList = (from p in context.AppointmentSchedule.Include(x => x.Patient)
                                                                              where p.UserId == userId && p.UserType == userType
                                                                              select p).OrderBy(x => x.Datetime).ToList();
                        if (lappointmentScheduleList != null && lappointmentScheduleList.Count > 0)
                        {
                            Console.Write("Prabhu Start:");
                            lappointmentScheduleList = lappointmentScheduleList.Select(x =>
                            {
                                
                                x.Datetime = Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(x.Datetime, timezoneoffset));

                                return x;

                            }).ToList();
                            result = result.Where(item => !lappointmentScheduleList.Any(item2 => item2.Datetime == Convert.ToDateTime(item.start))).ToList();
                        }


                        foreach (AppointmentSchedule lapp in lappointmentScheduleList)
                        {
                            if (lapp.SlotStatus == "Booked")
                            {
                                appointmentView availability = new appointmentView();
                                availability.title = lapp.Patient.PatientName;

                                availability.start = lapp.Datetime.ToString();
                                availability.end = Convert.ToDateTime(lapp.Datetime).AddMinutes(30).ToString();
                                if (DateTime.Now >= Convert.ToDateTime(availability.start) && DateTime.Now <= Convert.ToDateTime(availability.end))
                                {
                                    availability.backgroundColor = "red";
                                    availability.borderColor = "red";
                                    availability.appointmentid = lapp.AppointmentId;
                                }
                                else
                                {
                                    availability.backgroundColor = "blue";
                                    availability.borderColor = "blue";
                                }

                                result.Add(availability);
                            }
                            else if (lapp.SlotStatus == "Blocked")
                            {
                                appointmentView availability = new appointmentView();
                                
                                availability.start = lapp.Datetime.ToString();
                                availability.end = Convert.ToDateTime(lapp.Datetime).AddMinutes(30).ToString();
                                availability.backgroundColor = "grey";
                                availability.borderColor = "grey";
                                result.Add(availability);
                            }
                            else if (lapp.SlotStatus == "Extra")
                            {
                                appointmentView availability = new appointmentView();
                                
                                availability.start = lapp.Datetime.ToString();
                                availability.end = Convert.ToDateTime(lapp.Datetime).AddMinutes(30).ToString();
                                availability.backgroundColor = "green";
                                availability.borderColor = "green";
                                result.Add(availability);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
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
