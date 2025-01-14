using API.Data;
using API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public class PricingPlanService : IPricingPlanService
    {
        private readonly IPricingPlanRepository _pricingPlanRepository;
        public PricingPlanService(IPricingPlanRepository pricingPlanRepository)
        {
            _pricingPlanRepository = pricingPlanRepository;
        }

        public Task<List<PricingPlan>> GetAllPricingPlansAsync()
        {
            return _pricingPlanRepository.GetAllPricingPlansAsync();
        }

        public Task<List<PricingPlanCompanies>> GetAllPricingPlansForCompaniesAsync()
        {
            return _pricingPlanRepository.GetAllPricingPlansForCompaniesAsync();
        }
    }
}
