using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class User
    {
        public User()
        {
            ActivityLog = new HashSet<ActivityLog>();
            AppointmentSchedule = new HashSet<AppointmentSchedule>();
            Availability = new HashSet<Availability>();
            EquipmentAssignment = new HashSet<EquipmentAssignment>();
            MessagesPatient = new HashSet<Messages>();
            MessagesUser = new HashSet<Messages>();
            PatientConfiguration = new HashSet<PatientConfiguration>();
            PatientRx = new HashSet<PatientRx>();
        }

        public string UserId { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Npi { get; set; }
        public string Vseeid { get; set; }
        public string LoginSessionId { get; set; }
        public string EncryptPasswrod { get; set; }

        public ICollection<ActivityLog> ActivityLog { get; set; }
        public ICollection<AppointmentSchedule> AppointmentSchedule { get; set; }
        public ICollection<Availability> Availability { get; set; }
        public ICollection<EquipmentAssignment> EquipmentAssignment { get; set; }
        public ICollection<Messages> MessagesPatient { get; set; }
        public ICollection<Messages> MessagesUser { get; set; }
        public ICollection<PatientConfiguration> PatientConfiguration { get; set; }
        public ICollection<PatientRx> PatientRx { get; set; }
    }
}
