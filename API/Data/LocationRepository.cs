using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Data
{
    public class LocationRepository : ILocationRepository
    {
        private readonly DataContext _context;

        public LocationRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<City>> GetAllCitiesAsync()
        {
            return await _context.Cities.ToListAsync();
        }

        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _context.Countries.ToListAsync();
        }

        //public async Task<List<Municipality>> GetAllMunicipalitiesAsync()
        //{
        //    return await _context.Municipalities.ToListAsync();
        //}

        //public async Task<List<Canton>> GetAllCantonsAsync()
        //{
        //    return await _context.Cantons.ToListAsync();
        //}
    }
}
