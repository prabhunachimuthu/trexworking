using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface IPatientReviewInterface : IDisposable
    {
        PatientReview GetPatientReview(string reviewId);
        int InsertPatientReview(PatientReview pPatientReview);
        int UpdatePatientReview(PatientReview pPatientReview);
        List<PatientReview> GetPatientReviewList();
        List<PatientReview> GetPatientReviewList(string userId);
        List<PatientReviews> GetPatientReviewsList();
        List<PatientReviewTab> GetPatientReviewListTab(string PatientName);

    }
}
