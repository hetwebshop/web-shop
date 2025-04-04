﻿using API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IPricingPlanService
    {
        Task<List<PricingPlan>> GetAllPricingPlansAsync();
        Task<List<PricingPlanCompanies>> GetAllPricingPlansForCompaniesAsync();
    }
}
