using API.Data;
using API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            this.locationRepository = locationRepository;
        }

        public async Task<List<City>> GetAllCitiesAsync()
        {
            return await locationRepository.GetAllCitiesAsync();
        }

        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await locationRepository.GetAllCountriesAsync();
        }

        //public async Task<List<Municipality>> GetAllMunicipalitiesAsync()
        //{
        //    return await locationRepository.GetAllMunicipalitiesAsync();
        //}

        //public async Task<List<Canton>> GetAllCantonsAsync()
        //{
        //    return await locationRepository.GetAllCantonsAsync();
        //}
    }
}
