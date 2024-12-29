using API.DTOs;
using API.Entities.Applications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Data
{
    public interface IUserApplicationsRepository
    {
        Task<List<UserApplication>> GetAllUserApplicationsAsync(int userId);
        Task<List<UserApplication>> GetApplicationsForSpecificCompanyJobPost(int companyJobPostId);
        Task<UserApplication> GetUserApplicationByIdAsync(int id);
        Task<UserApplication> UpdateUserApplicationStatusAsync(UpdateApplicationStatusRequest req);
        Task<bool> RejectSelectedCandidatesAsync(RejectSelectedCandidatesRequest req);
    }
}
