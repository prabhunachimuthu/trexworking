using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    public interface INewPatient
    {
        List<NewProtocol> GetProtocolByRxId(string rxId);
        NewPatient GetPatientByPatitentLoginId(string patLoginId);
        PatientRx GetPatientRxByPatIdandDeviceConfig(int patId, string deviceConfig);
        List<PatientRx> CreateNewPatientByProvider(NewPatientWithProtocol NewPatient);
        List<Protocol> GetProtocolListByPatientId(string patId);
        List<NewProtocol> GetProtocolListBypatId(string patId);
        NewProtocol GetProtocolByproId(string proId);
        PatientRx GetNewPatientRxByRxId(string Rxid);
        List<PatientRx> GetNewPatientRxByPatId(string Patid);
        NewPatient GetPatientByPatId(int PatId);
        Protocol CreateProtocol(NewProtocol protocol);
        Patient UpdatePatient(NewPatient NewPatient);
        //string UpdatePatientRx(List<NewPatientRx> NewPatientRxs);
        List<PatientRx> UpdatePatientRx(List<NewPatientRx> NewPatientRxs, int PainThreshold = 0, int RateOfChange = 0, string usertype = "");
        string DeleteProtocolRecordsWithCasecade(string proId);
        int ChangeRxCurrent(string RxID, int CurrentFlexion, int CurrentExtension, string Code);
        int ChangeRxCurrentFlexion(string RxID, int CurrentFlexion, string Code);
        int ChangeRxCurrentExtension(string RxID, int CurrentExtension, string Code);
    }
}
