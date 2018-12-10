using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    public interface IPatientRxInterface : IDisposable
    {
        List<HightStockShoulderViewModelForJavaScript> getPatientRxEquipmentROMForShoulderInHighStockChartForJavaScript(int id, string equipmentType, string eenum);
        List<HightStockShoulderViewModelForJavaScript> getPatientRxEquipmentROMByFlexionExtensionInHighStockChartForJavaScript(int id, string equipmentType, string eenum);
        List<HightStockShoulderViewModelForJavaScript> getPatientRxEquipmentROMByExtensionInHighStockChartForJavaScript(int id, string equipmentType, string eenum);
        List<HightStockShoulderViewModelForJavaScript> getPatientRxEquipmentROMByFlexionInHighStockChartForJavaScript(int id, string equipmentType, string eenum);


        HightStockShoulderViewModel getPatientRxEquipmentROMByFlexionExtensionInHighStockChart(int id, string equipmentType, string eenum);
        HightStockShoulderViewModel getPatientRxEquipmentROMByExtensionInHighStockChart(int id, string equipmentType, string eenum);
        HightStockShoulderViewModel getPatientRxEquipmentROMByFlexionInHighStockChart(int id, string equipmentType, string eenum);
        HightStockShoulderViewModel getPatientRxEquipmentROMForShoulderInHighStockChart(int id, string equipmentType, string eenum);
        PatientRx getPatientRxPain(string rxid, string patid);
        PatientRx getPatientRxbyRxId(string rxid, string patid);
        List<DashboardView> getDashboardForSupport();
        List<DashboardView> getDashboardFromTherapistByPatientId(int id);
        List<DashboardView> getDashboardForTherapist(string id);
        List<ExtensionViewModel> getPatientRxComplianceByExtension(int id, string equipmentType, string eenum);
        List<FlexionViewModel> getPatientRxComplianceByFlexion(int id, string equipmentType, string eenum);
        List<ExtensionViewModel> getPatientRxEquipmentROMByExtension(int id, string equipmentType, string eenum);
        List<FlexionViewModel> getPatientRxEquipmentROMByFlexion(int id, string equipmentType, string eenum);
        //DashboardView getPatientRxROMKnee(int id, string equipmentType, int eenum);
        //DashboardView getPatientRxROMAnkle(int id, string equipmentType, int eenum);
        //DashboardView getPatientRxROMShoulder(int id, string equipmentType, int eenum);
        List<ROMViewModel> getPatientRxComplianceByProtocol(int id, string equipmentType, string eenum, int protocolenum);
        List<FlexionViewModel> getPatientRxComplianceByFlexion(int id, string equipmentType, string eenum, int protocolenum);
        List<ExtensionViewModel> getPatientRxComplianceByExtension(int id, string equipmentType, string eenum, int protocolenum);
        List<ExtensionViewModel> getPatientRxEquipmentROMByExtension(int id, string equipmentType, string eenum, int protocolenum);
        List<FlexionViewModel> getPatientRxEquipmentROMByFlexion(int id, string equipmentType, string eenum, int protocolenum);
        List<ROMViewModel> getPatientRxEquipmentROMByProtocol(int id, string equipmentType, string eenum, int protocolenum);
        List<ShoulderViewModel> getPatientRxComplianceForShoulder(int id, string equipmentType, string eenum);
        List<ShoulderViewModel> getPatientRxEquipmentROMForShoulder(int id, string equipmentType, string eenum);
        List<DashboardView> getDashboard(string id);
        List<DashboardView> getDashboardForPatientAdmin(string id);
        PatientRx getById(string id);
        PatientRx getByRxIDId(int id, string eenum);
        List<PatientRx> getByProviderId(string id);
        PatientRx getByPatientIdAndEquipmentTypeAndProviderId(string id, string patientId, string equipmentType);
        PatientRx getPatientRx(int id, string equipmentType, string eenum);
        PatientRx getPatientRxPain(int id, string equipmentType, string eenum);
        List<ROMViewModel> getPatientRxEquipmentROM(int id, string equipmentType, string eenum);
        DashboardView getPatientRxROM(int id, string equipmentType, string eenum);
        List<ROMViewModel> getPatientRxCompliance(int id, string equipmentType, string eenum);
        List<TreatmentCalendarViewModel> getTreatmentCalendar(int id, string equipmentType, string eenum);
        List<Session> getCurrentSessions(int id, string equipmentType, string eenum);
        PatientRx getPatientRx(string lRxID);

        List<PatientRx> getPatientRxByPatientId(string lPatientId);
        void InsertPatientRx(PatientRx pPatientRx);
        void UpdatePatientRx(PatientRx pPatientRx);
        void DeletePatientRx(PatientRx pPatientRx);

        string DeletePatientRecordsWithCasecade(int patid);

        ROMChartViewModel getPatientRxROMChart(int id, string equipmentType, string eenum);
        PatientRx getPatientRxByPEDP(string patientId, string EquipmentType, string DeviceConfiguration, string PatientSide);

        DashboardView getPatientRxByPatientLoginId(string id);
    }
}
