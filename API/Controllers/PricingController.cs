using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class PricingController : BaseController
    {
        private readonly IPricingPlanService _pricingPlanService;

        public PricingController(IPricingPlanService pricingPlanService)
        {
            _pricingPlanService = pricingPlanService;
        }

        [HttpGet("pricingplans")]
        public async Task<IActionResult> GetAllPricingPlans()
        {
            var pricingPlans = await _pricingPlanService.GetAllPricingPlansAsync();
            return Ok(pricingPlans);
        }
    }
}
