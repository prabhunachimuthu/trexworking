using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class PatientView
    {
        public int PatientId { get; set; }
        public string ProviderId { get; set; }
        public string PatientName { get; set; }
        public DateTime? Dob { get; set; }
        public string AddressLine { get; set; }
        
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Ssn { get; set; }
        public string EquipmentType { get; set; }
        public DateTime? SurgeryDate { get; set; }
        public string Side { get; set; }
        public int? Pin { get; set; }
        public Guid? LoginSessionId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string Therapistid { get; set; }
        public string VSeeId { get; set; }
    }

    public class PatientLoginView
    {
        public string PatientLoginId { get; set; }
        public int PatientId { get; set; }
        public string PatientFirstName { get; set; }
        public string TherapistId { get; set; }
        public string TherapistName { get; set; }
        public string TherapistContactNo { get; set; }
        public string ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string ProviderContactNo { get; set; }
        public string InstallerId { get; set; }
        public string InstallerName { get; set; }
        public string InstallerContactNo { get; set; }
        public string VSeeId { get; set; }
        public string RxStartDate { get; set; }
        public string RxEndDate { get; set; }
        public Guid? LoginSessionId { get; set; }


    }
}
