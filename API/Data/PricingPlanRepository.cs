using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class PricingPlanRepository : IPricingPlanRepository
    {
        private readonly DataContext _context;

        public PricingPlanRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<PricingPlan>> GetAllPricingPlansAsync()
        {
            var pricingPlans = await _context.PricingPlans.ToListAsync();
            return pricingPlans;
        }

        public async Task<List<PricingPlanCompanies>> GetAllPricingPlansForCompaniesAsync()
        {
            var pricingPlans = await _context.PricingPlanCompanies.ToListAsync();
            return pricingPlans;
        }
    }
}
