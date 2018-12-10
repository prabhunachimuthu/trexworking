using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    public interface IPatient
    {
        Patient GetPatientByPatientLoginId(string patientloginid);
        List<Patient> GetPatientByProviderId(string providerid);
        List<PatientMessage> GetPatientWithStatusByProviderId(string providerid);
        PatientLoginView PatientLoginsReturnPatientLoginViewUsingPatientLoginId(string patientloinId, string PIN);
        PatientLoginView PatientLoginsReturnPatientLoginViewUsingPatientLoginIdWithEncryptPin(string patientloinId, string PIN);
        Patient GetPatientByPatientID(int patientId);
        PatientLoginView PatientLoginsReturnPatientLoginView(string patientPhone, string PIN);
        string ClaimPatient(string patientloginid, string SurgeryDate);
        string CreatePIN(string patientloginid, string SurgeryDate, string PIN);
        string CreateEncryptPIN(string patientloginid, string SurgeryDate, string PIN);
        string PatientLogin(string patientPhone, string PIN);
        Patient PatientLogins(string patientPhone, string PIN);
        Patient GetPaitentbyTherapistIDandPatientLoginId(string PatientLoginId, string therapistId);
        Patient GetPatientByPhone(string patientPhone, string PIN);
        Patient GetPatientByPatientLoginId(string patientloginid, string PIN);
        PatientView PatientLoginsReturnPatientView(string patientPhone, string PIN);
        List<Patient> GetPatientByTherapistId(string therapsitId);
        List<PatientMessage> GetPatientWithStatusByTherapistId(string therapsitId);
        List<Patient> GetPatientByPatientAdmin(string patientadminid);
        List<PatientMessage> GetAllPatientStatus(string userId);
        List<Patient> GetAllPatients();
        Patient GetPatientBySessionID(string sessionId);
        string UpdatePatientSessionId(string patientloginid);
    }
}
