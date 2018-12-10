using OneDirect.Helper;
using OneDirect.Models;
using OneDirect.Repository.Interface;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository
{
    public class RomChangeLogRepository : IRomChangeLogInterface
    {
        private OneDirectContext context;

        public RomChangeLogRepository(OneDirectContext context)
        {
            this.context = context;
        }
      
        public void InsertRomChangeLog(RomchangeLog plog)
        {
            context.RomchangeLog.Add(plog);
            context.SaveChanges();
        }

        
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
