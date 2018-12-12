using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Extensions
{
    public class LibraryExtension
    {
        public static LibraryView LibraryToLibraryView(Library lLibrary)
        {
            if (lLibrary == null)
                return null;
            LibraryView lLibraryView = new LibraryView()
            {
                Id = lLibrary.Id,
                Name=lLibrary.Name,
                Limb = lLibrary.Limb,
                Side = lLibrary.Side,
                Exercise = lLibrary.Exercise,
                Url = lLibrary.Url
            };
            return lLibraryView;
        }

        public static Library LibraryViewToLibrary(LibraryView lLibrary)
        {
            if (lLibrary == null)
                return null;
            Library pLibrary = new Library()
            {
                Id = lLibrary.Id,
                Limb = lLibrary.Limb,
                Side = lLibrary.Side,
                Exercise = lLibrary.Exercise,
                Url = lLibrary.Url,
                Name = lLibrary.Name
            };
            return pLibrary;
        }

    }
}
