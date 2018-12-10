using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface IAppointmentScheduleInterface : IDisposable
    {
        List<AppointmentScheduleListView> getAppointmentList(string timezone);
        List<appointmentView> GetAvailableSlotsForAppointmentCalendarInMinutes(string userId, string userType, DateTime startdate, DateTime enddate, string timezoneoffset);
        List<availabilityView> GetAvailabilityInMinutes(string userId, string browsertimezone);
        string UpdateAppointmentSchedule(string userId, string userType, string timezoneoffset, string day, string hour, string minute);
        string RemoveAvailability(string userId, string userType, string timezoneoffset, string day, string hour, string minute);
        string InsertAvailability(string userId, string userType, string timezoneoffset, string day, string hour, string minute);
        List<AppointmentSchedule> CheckAppointmentSchedule(string userId, string userType, string timezoneoffset, string day, string hour, string minute);
        string GetAvailability(string userId, string day, string hour, string minute, string timezoneoffset);
        List<AppointmentScheduleListView> getAppointmentListByPatientId(int lpatientId, string timezone);
        List<AppointmentScheduleListView> getAppointmentListByUserId(string luserId, string timezone);
        List<Patient> getAppointmentPatientList();
        List<Patient> getAppointmentPatientListByUserId(string userId);
        AppointmentSchedule GetAppointment(string appontmentId);
        int UpdateAppointment(AppointmentSchedule appontment);
        int DeleteAppointmentSchedule(string appointmentId);
        AppointmentSchedule CheckAppointmentSchedule(string userId, string userType, DateTime datetime);
        List<appointmentView> GetAvailableSlotsForAppointmentCalendar(string userId, string userType, DateTime startdate, DateTime enddate, string timezoneoffset);
        string UpdateAppointmentSchedule(string userId, string userType, string timezoneoffset, string day, string hour);
        string RemoveAvailability(string userId, string userType, string timezoneoffset, string day, string hour);
        List<AppointmentSchedule> CheckAppointmentSchedule(string userId, string userType, string timezoneoffset, string day, string hour);
        string InsertAvailability(string userId, string userType, string timezoneoffset, string day, string hour);
        string GetAvailability(string userId, string day, string hour, string timezoneoffset);
        List<availabilityView> GetAvailability(string userId, string browsertimezone);
        int InsertAppointment(AppointmentSchedule pAppointment);
        List<AppointmentScheduleView> GetAppointments(int patientId);
        string CancelAppointments(int patientId, string userId, string datetime);
        string NoShowAppointments(int patientId, string userId, string datetime);
        List<AvailableSlot> GetAvailableSlots(string userId, DateTime rxStartDate, DateTime rxEndDate, int code);


    }
}
