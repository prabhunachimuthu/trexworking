using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface IRomChangeLogInterface : IDisposable
    {
       
        void InsertRomChangeLog(RomchangeLog plog);
       
    }
}
