using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    public interface IUserInterface : IDisposable
    {
        int deleteUser(User luser);
        User getUserbySessionId(string sessionId);
        User userLogin(string lUserID, string password, int type);
        User getUser(string lUserID, string password, int type);
        User getUserWithEncryptPassword(string lUserID, string password, int type);
        User getUser(string lUserID, string password);

        User getUser(string lUserID);
        User getUserNpi(string lNpi);
        string getUserdatabyPatientAndtherapist(string lpatientId, string lTherapistId);
        List<User> getUserData(string lUserID);
        List<User> getUserListByType(int lUserType);
        List<UserListView> getInstallerUserList();

        List<UserListView> getPatientAdminUserList();
        List<UserListView> getUserListByType1243(int lUserType);
        List<SupportView> getUserListByTypeSup(int lUserType);
        List<NewPatient> getUserListByTherapistId(string lTherapist);
        List<User> getTherapistListByProviderId(string lProvider);
        List<NewPatient> getPatientListByPatientAdmin(string lpatAdmin);
        List<NewPatient> getPatientListByProviderId(string lProvider);

        List<User> getUserListByTypeValue(int lUserType);
        string InsertUser(User pUser);
        string UpdateUser(User pUser);
        string UpdateSessionId(User pUser);
        User LoginUser(string username, string password);
        EquipmentAssignment getEquUser(string lUserID);

        List<UserViewModel> getUserListByTypeValueqq(int lUserType);
        List<NewPatient> getPatientListByType(int lUserType);
        List<DashboardView> getDashboardForSupport();
        List<DashboardView> getDashboard(string id);
        List<DashboardView> getDashboardForPatientAdmin(string id);
        List<DashboardView> getDashboardForTherapist(string id);

        //meghna
        List<UserListView> getUserList(int lUserType, String UserID);
    }
}
