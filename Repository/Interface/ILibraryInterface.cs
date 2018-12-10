using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    public interface ILibraryInterface : IDisposable
    {
        List<Library> getLibrary(string limb, string exercise);
        Library getLibraryById(int id);
        List<Library> getLibrary();
        void InsertLibrary(Library pLibrary);
        void UpdateLibrary(Library pLibrary);
        string DeleteLibrary(int libraryId);
    }
}
