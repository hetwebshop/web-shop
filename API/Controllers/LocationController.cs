using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class LocationController : BaseController
    {
        private readonly ILocationService locationService;

        public LocationController(ILocationService locationService)
        {
            this.locationService = locationService;
        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetAllCities()
        {
            var cities = await locationService.GetAllCitiesAsync();
            return Ok(cities);
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await locationService.GetAllCountriesAsync();
            return Ok(countries);
        }

        //[HttpGet("cantons")]
        //public async Task<IActionResult> GetAllCantons()
        //{
        //    var cantons = await locationService.GetAllCantonsAsync();
        //    return Ok(cantons);
        //}

        //[HttpGet("municipalities")]
        //public async Task<IActionResult> GetAllMunicipalities()
        //{
        //    var municipalities = await locationService.GetAllMunicipalitiesAsync();
        //    return Ok(municipalities);
        //}
    }
}
