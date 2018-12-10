using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class PatientReviews
    {
        public string ReviewId { get; set; }
        public string UserId { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public string SessionId { get; set; }
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string ActivityType { get; set; }
        public DateTime StartTimeStamp { get; set; }
        public int Duration { get; set; }
        public string AssessmentComment { get; set; }
        public string AssessmentChecklist { get; set; }
        public int count { get; set; }
    }

    public class PatientReviewReport
    {
        public PatientReview Review { get; set; }
        public PatientDetails Patient { get; set; }
        public List<ROMReport> ROM { get; set; }
        public List<Protocol> ProtocolList { get; set; }
        public List<Protocol> ProtocolCurrentList { get; set; }
        public IDictionary<string, IDictionary<string, string>> ChangeList { get; set; }
    }

    public class ROMReport
    {
        public string Exercise { get; set; }
        public string GoalFlexion { get; set; }
        public string FlexionAchieved { get; set; }
        public string GoalExtension { get; set; }
        public string ExtensionAchieved { get; set; }
        public string PainLevel { get; set; }
    }

    public class PatientDetails
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string TherapistId { get; set; }
        public string TherapistName { get; set; }
        public string PatId { get; set; }
        public string PatAdminName { get; set; }
        public string DateofBirth { get; set; }
        public string Gender { get; set; }
        public string EvaluationDate { get; set; }
        public string Evaluator { get; set; }
        public string Side { get; set; }
        public string EquipmentType { get; set; }
        public string SurgeryDate { get; set; }

    }

}
