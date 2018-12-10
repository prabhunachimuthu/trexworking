using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class DashboardView
    {
        public PatientRx PatientRx { get; set; }
        public int MaxPain { get; set; }
        public DateTime? FirstUse { get; set; }

        public DateTime? LastUse { get; set; }
        public int UpRom1 { get; set; }
        public int DownRom1 { get; set; }
        public int UpRom2 { get; set; }
        public int DownRom2 { get; set; }
        public int UpRom3 { get; set; }
        public int DownRom3 { get; set; }
        public string PercentageofUsage { get; set; }
        public int TotalSession { get; set; }
        public Progress Progress { get; set; }
        public int percentage { get; set; }
    }
    public class PatientSummary
    {
        public DateTime RxStartDate { get; set; }
        public DateTime RxEndDate { get; set; }
        public int RemainingDays { get; set; }
        public int RxDuration { get; set; }
        public int SessionCompleted { get; set; }
        public int SessionSuggested { get; set; }
        public int TrexMinutes { get; set; }
        public int FlexionExtensionMinutes { get; set; }
        public int MaxFlexionAchieved { get; set; }
        public int MaxExtensionAchieved { get; set; }
        public int MaxPainLevel { get; set; }
    }
}
