using Microsoft.EntityFrameworkCore;
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
    public class PatientReviewRepository : IPatientReviewInterface
    {
        private OneDirectContext context;

        public PatientReviewRepository(OneDirectContext context)
        {
            this.context = context;
        }

        public PatientReview GetPatientReview(string reviewId)
        {
            return (from p in context.PatientReview
                    where p.ReviewId == reviewId
                    select p).FirstOrDefault();

        }
        public List<PatientReview> GetPatientReviewList()
        {
            return (from p in context.PatientReview.Include(x => x.UserActivityLog)
                    orderby p.StartTimeStamp descending
                    select p).ToList();

        }

        public List<PatientReview> GetPatientReviewList(string userId)
        {
            return (from p in context.PatientReview.Include(x => x.UserActivityLog)
                    where p.UserId == userId
                    orderby p.StartTimeStamp descending
                    select p).ToList();

        }
        public List<PatientReviewTab> GetPatientReviewListTab(string PatientName)
        {
            return (from p in context.PatientReview.Include(x => x.UserActivityLog)
                    where p.PatientName == PatientName
                    orderby p.StartTimeStamp descending
                    select new PatientReviewTab
                    {
                        ReviewId = p.ReviewId,
                        UserId = p.UserId,
                        UserType = p.UserType,
                        UserName = p.UserName,
                        SessionId = p.SessionId,
                        PatientId = p.PatientId,
                        PatientName = p.PatientName,
                        ActivityType = p.ActivityType,
                        StartTimeStamp = p.StartTimeStamp,
                        Duration = p.Duration,
                        AssessmentComment = p.AssessmentComment,
                        AssessmentChecklist = p.AssessmentChecklist,
                        UserActivityLog = p.UserActivityLog

                    }).ToList();

        }

        public List<PatientReviews> GetPatientReviewsList()
        {
            return (from p in context.PatientReview
                    select new PatientReviews
                    {
                        ReviewId = p.ReviewId,
                        UserId = p.UserId,
                        UserType = p.UserType,
                        UserName = p.UserName,
                        SessionId = p.SessionId,
                        PatientId = p.PatientId,
                        PatientName = p.PatientName,
                        ActivityType = p.ActivityType,
                        StartTimeStamp = p.StartTimeStamp,
                        Duration = p.Duration,
                        AssessmentComment = p.AssessmentComment,
                        AssessmentChecklist = p.AssessmentChecklist,

                    }).ToList();

        }
        public int InsertPatientReview(PatientReview pPatientReview)
        {
            context.PatientReview.Add(pPatientReview);
            return context.SaveChanges();
        }

        public int UpdatePatientReview(PatientReview pPatientReview)
        {
            var _PatientReview = (from p in context.PatientReview
                                  where p.ReviewId == pPatientReview.ReviewId
                                  select p).FirstOrDefault();
            if (_PatientReview != null)
            {
                context.Entry(_PatientReview).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return context.SaveChanges();
            }
            return 0;
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
