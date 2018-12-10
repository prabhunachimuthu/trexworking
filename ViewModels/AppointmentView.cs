using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class AppointmentView
    {
        public string AppointmentId { get; set; }
        public string PatientUserId { get; set; }
        [Required(ErrorMessage = "Patient Name is required")]
        public string PatientName { get; set; }
        public string PatientVseeId { get; set; }
        public string AppointmentType { get; set; }
        public string TherapistUserId { get; set; }
        public string TherapistName { get; set; }
        public string SupportName { get; set; }
        public string SupportUserId { get; set; }
        public string AvailableSlots { get; set; }
        public DateTime? ConfirmedSlot { get; set; }
        public string Urikey { get; set; }
        public int? Status { get; set; }
        public int? Duration { get; set; }
        public string PatientComment { get; set; }
        public string TherapistSupportComment { get; set; }
        [Required(ErrorMessage = "Appointment Date is required")]
        public string AppointmentDate { get; set; }
        public string Slots { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string SlotId { get; set; }
        public string SlotTime { get; set; }
        public string Timezone { get; set; }
        public List<CheckModel> AppointmentSlots { get; set; }
        public int TotalSession { get; set; }
        public User Provider { get; set; }
        public PatientRx PatientRx { get; set; }
    }
    public class cancelappointment
    {
        public string UserID { get; set; }
        public string DateTime { get; set; }
    }

    public class bookappointment
    {
        public string UserID { get; set; }
        public string DateTime { get; set; }
    }
    public class downloadavailability
    {
        public string timeZoneOffset { get; set; }
        public List<AvailableSlot> availableSlots { get; set; }
    }
    public class availabilityView
    {
        public string url { get; set; }
        public string title { get; set; }
        public string[] dow { get; set; }
        //public string rendering { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
    }
    public class appointmentView
    {
        public string url { get; set; }
        public string title { get; set; }
        //public string rendering { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        public string appointmentid { get; set; }
    }
    public class AvailableSlot
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string DateTime { get; set; }
    }

    public class AppointmentScheduleView
    {

        public string AppointmentId { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public String DateTime { get; set; }
        public int? PatientId { get; set; }
        public string SlotStatus { get; set; }
        public string CallStatus { get; set; }
        public string RecordedFile { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }

    public class AppointmentScheduleListView
    {
        public string AppointmentId { get; set; }
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientVseeId { get; set; }
        public string UserType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Urikey { get; set; }
        public string Status { get; set; }
        public string AppointmentDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int TotalSession { get; set; }
        public User Provider { get; set; }
        public PatientRx PatientRx { get; set; }
        public int Count { get; set; }
    }
    public class TimeSlot
    {
        public string Support_StartTime { get; set; }
        public string Support_EndTime { get; set; }
        public string Therapist_StartTime { get; set; }
        public string Therapist_EndTime { get; set; }
        public string SlotDuration { get; set; }
        public string Support_StartSlot { get; set; }
        public string Support_EndSlot { get; set; }
        public string Therapist_StartSlot { get; set; }
        public string Therapist_EndSlot { get; set; }
    }
}
