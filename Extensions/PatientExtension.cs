using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Extensions
{
    public class PatientExtension
    {
        public static PatientView PatientToPatientView(Patient lPatient)
        {
            if (lPatient == null)
                return null;
            PatientView lpatientView = new PatientView()
            {
                PatientId = lPatient.PatientId,
                ProviderId = lPatient.ProviderId,
                PatientName = lPatient.PatientName,
                Dob = lPatient.Dob,
                AddressLine = lPatient.AddressLine,
                PhoneNumber = lPatient.PhoneNumber,
                City = lPatient.City,
                State = lPatient.State,
                Zip = lPatient.Zip,
                Ssn = lPatient.Ssn,
                EquipmentType = lPatient.EquipmentType,
                SurgeryDate = lPatient.SurgeryDate,
                Side = lPatient.Side,
                Pin = lPatient.Pin,
                LoginSessionId = lPatient.LoginSessionId,
                DateCreated = lPatient.DateCreated,
                DateModified = lPatient.DateModified,
                Therapistid = lPatient.Therapistid
            };
            return lpatientView;
        }

        public static Patient PatientViewToPatient(PatientView lPatient)
        {
            if (lPatient == null)
                return null;
            Patient ppatient = new Patient()
            {
                PatientId = lPatient.PatientId,
                ProviderId = lPatient.ProviderId,
                PatientName = lPatient.PatientName,
                Dob = lPatient.Dob,
                AddressLine = lPatient.AddressLine,
                PhoneNumber = lPatient.PhoneNumber,
                City = lPatient.City,
                State = lPatient.State,
                Zip = lPatient.Zip,
                Ssn = lPatient.Ssn,
                EquipmentType = lPatient.EquipmentType,
                SurgeryDate = lPatient.SurgeryDate,
                Side = lPatient.Side,
                Pin = lPatient.Pin,
                LoginSessionId = lPatient.LoginSessionId,
                DateCreated = lPatient.DateCreated,
                DateModified = lPatient.DateModified,
                Therapistid = lPatient.Therapistid
            };
            return ppatient;
        }
    }
}
