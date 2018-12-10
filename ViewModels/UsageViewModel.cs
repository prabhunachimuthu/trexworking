using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class UsageViewModel
    {
        public int MaxSessionSuggested { get; set; }
        public int PercentageCompleted { get; set; }
        public int PercentagePending { get; set; }
    }
}
