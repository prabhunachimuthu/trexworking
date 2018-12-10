using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Extensions
{
    public static class PatientRxExtension
    {
        public static PatientRxView PatientRxToPatientRxViewModel(this PatientRx patientRx)
        {
            if (patientRx == null)
                return null;

            PatientRxView patRx = new PatientRxView()
            {
                RxId = patientRx.RxId.ToString(),
                ProviderId = patientRx.ProviderId,
                PatientId = patientRx.PatientId.ToString(),
                EquipmentType = patientRx.EquipmentType,
                RxStartDate = Convert.ToDateTime(patientRx.RxStartDate).ToString("dd-MMM-yyyy"),
                RxEndDate = Convert.ToDateTime(patientRx.RxEndDate).ToString("dd-MMM-yyyy"),
                
                RxSessionsPerWeek = (int)patientRx.RxSessionsPerWeek,
               
                DateCreated = patientRx.DateCreated,
                DateModified = patientRx.DateModified,
                RxDays = new List<checkboxModel>
                        {
                             new checkboxModel{id = 1, name = "SUN", isCheck = (!string.IsNullOrEmpty(patientRx.RxDays) && patientRx.RxDays.Split(',').ToList().Where(x=>x=="SUN").Count() >0) ? true : false },
                             new checkboxModel{id = 2, name = "MON", isCheck = (!string.IsNullOrEmpty(patientRx.RxDays) && patientRx.RxDays.Split(',').ToList().Where(x=>x=="MON").Count() >0) ? true : false},
                             new checkboxModel{id = 3, name = "TUE", isCheck = (!string.IsNullOrEmpty(patientRx.RxDays) && patientRx.RxDays.Split(',').ToList().Where(x=>x=="TUE").Count() >0) ? true : false},
                             new checkboxModel{id = 4, name = "WED", isCheck = (!string.IsNullOrEmpty(patientRx.RxDays) && patientRx.RxDays.Split(',').ToList().Where(x=>x=="WED").Count() >0) ? true : false},
                             new checkboxModel{id = 5, name = "THR", isCheck = (!string.IsNullOrEmpty(patientRx.RxDays) && patientRx.RxDays.Split(',').ToList().Where(x=>x=="THR").Count() >0) ? true : false},
                             new checkboxModel{id = 6, name = "FRI", isCheck = (!string.IsNullOrEmpty(patientRx.RxDays) && patientRx.RxDays.Split(',').ToList().Where(x=>x=="FRI").Count() >0) ? true : false},
                             new checkboxModel{id = 6, name = "SAT", isCheck = (!string.IsNullOrEmpty(patientRx.RxDays) && patientRx.RxDays.Split(',').ToList().Where(x=>x=="FRI").Count() >0) ? true : false}
                },
                DateOfBirth = Convert.ToDateTime(patientRx.Patient.Dob).ToString("dd-MMM-yyyy"),
               
                Mobile = patientRx.Patient.PhoneNumber,
                Address = patientRx.Patient.AddressLine,
                PatientName = patientRx.Patient.PatientName
            };
            return patRx;
        }


        public static PatientRx PatientRxViewToPatientRxModel(this PatientRxView patientRx)
        {
            if (patientRx == null)
                return null;
            Console.WriteLine("Patient Rx: " + Newtonsoft.Json.JsonConvert.SerializeObject(patientRx));

            PatientRx patRx = new PatientRx()
            {
                RxId = patientRx.RxId,
                ProviderId = patientRx.ProviderId,
                PatientId = Convert.ToInt32(patientRx.PatientId),
                EquipmentType = patientRx.EquipmentType,
                RxStartDate = Convert.ToDateTime(patientRx.RxStartDate.ToString()),
                RxEndDate = Convert.ToDateTime(patientRx.RxEndDate.ToString()),
                RxDaysPerweek = patientRx.RxDays.Where(x => x.isCheck == true).Count(),
                RxSessionsPerWeek = (int)patientRx.RxSessionsPerWeek,
                
                DateCreated = patientRx.DateCreated,
                DateModified = patientRx.DateModified,
                RxDays = string.Join(",", patientRx.RxDays.Where(x => x.isCheck == true).Select(x => x.name)),
                Patient = new Patient()
                {
                    Dob = Convert.ToDateTime(patientRx.DateOfBirth),
                   
                    PhoneNumber = patientRx.Mobile,
                    AddressLine = patientRx.Address,
                    PatientName = patientRx.PatientName,
                    PatientId = Convert.ToInt32(patientRx.PatientId),
                    
                    ProviderId = patientRx.ProviderId
                }
            };
            return patRx;
        }

    }
}
