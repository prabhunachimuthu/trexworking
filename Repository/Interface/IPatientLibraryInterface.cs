using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    public interface IPatientLibraryInterface : IDisposable
    {
        List<PatientLibrary> getPatientLibraryByPatientId(string patientid);
        PatientLibrary getPatientLibraryById(int id);
        List<PatientLibrary> getPatientLibrary();
        void InsertPatientLibrary(PatientLibrary pLibrary);
        void UpdatePatientLibrary(PatientLibrary pLibrary);
        string DeletePatientLibrary(int libraryId);
        List<Library> getPatientLibrary(string patientId,  string limb);
        List<Library> getPatientLibrary(string patientId, string limb, string exercise);
    }
}
