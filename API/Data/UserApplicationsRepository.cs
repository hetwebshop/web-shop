using API.DTOs;
using API.Entities.Applications;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class UserApplicationsRepository : IUserApplicationsRepository
    {
        private readonly DataContext _context;

        public UserApplicationsRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<UserApplication>> GetAllUserApplicationsAsync(int userId)
        {
            var query = _context.UserApplications
                .Include(r => r.PreviousCompanies)
                .Include(r => r.EducationLevel)
                .Include(r => r.Educations)
                .Include(r => r.CompanyJobPost) 
                .ThenInclude(r => r.User)
                .ThenInclude(r => r.Company)
                .Include(r => r.CompanyJobPost.City)
                .Where(r => r.SubmittingUserId == userId).OrderByDescending(r => r.CreatedAt);
            return await query.ToListAsync();
        }
        public async Task<List<UserApplication>> GetApplicationsForSpecificCompanyJobPost(int companyJobPostId)
        {
            var query = _context.UserApplications
                .Include(r => r.PreviousCompanies)
                .Include(r => r.EducationLevel)
                .Include(r => r.Educations)
                .Include(r => r.CompanyJobPost)
                .ThenInclude(r => r.User)
                .Include(r => r.City)
                .Where(r => r.CompanyJobPostId == companyJobPostId).OrderByDescending(r => r.CreatedAt);
            return await query.ToListAsync();
        }

        public async Task<UserApplication> GetUserApplicationByIdAsync(int id)
        {
            var query = await _context.UserApplications
            .Include(r => r.PreviousCompanies)
            .Include(r => r.EducationLevel)
            .Include(r => r.Educations)
            .Include(r => r.CompanyJobPost)
            .ThenInclude(r => r.User)
            .ThenInclude(r => r.Company)
            .Include(r => r.City)
            .FirstOrDefaultAsync(r => r.Id == id);
            return query;
        }

        public async Task<UserApplication> UpdateUserApplicationStatusAsync(UpdateApplicationStatusRequest req)
        {
            var application = await _context.UserApplications.FirstOrDefaultAsync(r => r.Id == req.UserApplicationId);
            if (application == null)
                return null;
            application.Feedback = req.Feedback;
            application.IsOnlineMeeting = req.IsOnlineMeeting;
            application.OnlineMeetingLink = req.OnlineMeetingLink;
            application.MeetingDateTime = req.MeetingDateTimeDateType;
            application.ApplicationStatusId = req.ApplicationStatus;

            _context.UserApplications.Update(application);
            _context.SaveChanges();
            var updatedApplication = await GetUserApplicationByIdAsync(application.Id);
            return updatedApplication;
        }

        public async Task<bool> RejectSelectedCandidatesAsync(RejectSelectedCandidatesRequest req)
        {
            var applications = await _context.UserApplications
                .Where(r => req.Candidates.Contains(r.Id))
                .ToListAsync();

            foreach (var application in applications)
            {
                application.Feedback = req.Feedback;
                application.ApplicationStatusId = ApplicationStatus.Rejected;
                application.IsOnlineMeeting = false;
                application.MeetingDateTime = null;
            }

            _context.UserApplications.UpdateRange(applications);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
