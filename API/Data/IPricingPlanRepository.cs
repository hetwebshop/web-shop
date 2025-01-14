using API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Data
{
    public interface IPricingPlanRepository
    {
        Task<List<PricingPlan>> GetAllPricingPlansAsync();
        Task<List<PricingPlanCompanies>> GetAllPricingPlansForCompaniesAsync();
    }
}
